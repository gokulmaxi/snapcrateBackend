using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using snapcrateBackend.Auth;
using snapcrateBackend.Model;

namespace snapcrateBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoldersController : ControllerBase
    {
        private readonly SnapCrateDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FoldersController(SnapCrateDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }

        // GET: api/Folders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FolderModel>>> GetFolderModel()
        {
          if (_context.FolderModel == null)
          {
              return NotFound();
          }

          var user = await _userManager.FindByNameAsync(User.Identity.Name);
          return await _context.FolderModel.
                Where(d => d.User == user)
                .ToListAsync();
        }
        // REVIEW Is this api really required
        // GET: api/Folders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FolderModel>> GetFolderModel(int id)
        {
          if (_context.FolderModel == null)
          {
              return NotFound();
          }
            var folderModel = await _context.FolderModel.FindAsync(id);

            if (folderModel == null)
            {
                return NotFound();
            }

            return folderModel;
        }

        // PUT: api/Folders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFolderModel(int id, FolderModel folderModel)
        {
            if (id != folderModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(folderModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FolderModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Folders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FolderModel>> PostFolderModel(FolderModel folderModel)
        {
          if (_context.FolderModel == null)
          {
              return Problem("Entity set 'SnapCrateDbContext.FolderModel'  is null.");
          }
            try
            {
                folderModel.User = await _userManager.FindByNameAsync(User.Identity.Name);
                _context.FolderModel.Add(folderModel);
                await _context.SaveChangesAsync();
            return CreatedAtAction("GetFolderModel", new { id = folderModel.Id }, folderModel);
            }
            catch
            {
                return Ok(new Response { Status = "Error", Message = "Folder creation failed! Please check user details and try again." });
            }
        }

        // DELETE: api/Folders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolderModel(int id)
        {
            if (_context.FolderModel == null)
            {
                return NotFound();
            }
            var folderModel = await _context.FolderModel.FindAsync(id);
            if (folderModel == null)
            {
                return NotFound();
            }

            _context.FolderModel.Remove(folderModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FolderModelExists(int id)
        {
            return (_context.FolderModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
