using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using snapcrateBackend.Auth;
using snapcrateBackend.Helpers;
using snapcrateBackend.Model;

namespace snapcrateBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly AzureStorageConfig storageConfig = null;
                private readonly SnapCrateDbContext _context;


        public ImagesController(SnapCrateDbContext context,IOptions<AzureStorageConfig> config)
        {
            storageConfig = config.Value;
            _context = context;

        }

        // POST /api/images/upload
        [HttpPost("[action]")]
        public async Task<IActionResult> Upload([FromForm]int folderId,ICollection<IFormFile> files)
        {
            bool isUploaded = false;

            try
            {
                if (_context.FolderModel == null)
                {
                    return NotFound();
                }
                var folderModel = await _context.FolderModel.Include(d=>d.User).FirstAsync(d=>d.Id==folderId);

                if (folderModel == null)
                {
                    return NotFound();
                }

                if (files.Count == 0)
                    return BadRequest("No files received from the upload");

                if (storageConfig.AccountKey == string.Empty || storageConfig.AccountName == string.Empty)
                    return BadRequest("sorry, can't retrieve your azure storage details from appsettings.js, make sure that you add azure storage details there");

                if (storageConfig.ImageContainer == string.Empty)
                    return BadRequest("Please provide a name for your image container in the azure blob storage");
                var imageModel = new ImageModel();

                foreach (var formFile in files)
                {
                    if (StorageHelper.IsImage(formFile))
                    {
                        if (formFile.Length > 0)
                        {
                            using (Stream stream = formFile.OpenReadStream())
                            {
                                var imageUri = "https://" +
                                      storageConfig.AccountName +
                                      ".blob.core.windows.net/" +
                                      storageConfig.ImageContainer + "/" + folderModel.User.NormalizedUserName + "/" + folderModel.Name +
                                      "/" + formFile.FileName;
                                imageModel.name = formFile.FileName;
                                imageModel.imageUrl = imageUri;
                                imageModel.thumbnailUrl = imageUri.Replace(storageConfig.ImageContainer, "thumbnails");
                                imageModel.folder = folderModel;
                                isUploaded = await StorageHelper.UploadFileToStorage(stream, formFile.FileName, storageConfig, imageModel);
                                await _context.ImageModels.AddAsync(imageModel);
                                await _context.SaveChangesAsync();

                            }
                        }
                    }
                    else
                    {
                        return new UnsupportedMediaTypeResult();
                    }
                }

                if (isUploaded)
                {
                    if (storageConfig.ThumbnailContainer != string.Empty)
                        return new AcceptedAtActionResult("GetThumbNails", "Images", null, null);
                    else
                        return new AcceptedResult();
                }
                else
                    return BadRequest("Look like the image couldnt upload to the storage");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/images/thumbnails
        [HttpGet("thumbnails")]
        public async Task<IActionResult> GetThumbNails()
        {
            try
            {
                if (storageConfig.AccountKey == string.Empty || storageConfig.AccountName == string.Empty)
                    return BadRequest("Sorry, can't retrieve your Azure storage details from appsettings.js, make sure that you add Azure storage details there.");

                if (storageConfig.ImageContainer == string.Empty)
                    return BadRequest("Please provide a name for your image container in Azure blob storage.");

                List<string> thumbnailUrls = await StorageHelper.GetThumbNailUrls(storageConfig);
                return new ObjectResult(thumbnailUrls);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSharedFolders(int id)
        {
            if (_context.ImageModels == null)
            {
                return NotFound();
            }
            var imageData = await _context.ImageModels.FindAsync(id);
            if (imageData == null)
            {
                return NotFound();
            }
            _context.ImageModels.Remove(imageData);
            
            await _context.SaveChangesAsync();

            await StorageHelper.DeleteFileFromStorage(storageConfig, imageData);
            return NoContent();
        }
    }
}

