﻿using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using snapcrateBackend.Model;

namespace snapcrateBackend.Helpers
{
    public class StorageHelper
    {
        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<bool> UploadFileToStorage(Stream fileStream, string fileName,
                                                            AzureStorageConfig _storageConfig,ImageModel imageData)
        {

            // Create a URI to the blob
            Uri blobUri = new Uri(imageData.imageUrl);

            // Create StorageSharedKeyCredentials object by reading
            // the values from the configuration (appsettings.json)
            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(_storageConfig.AccountName, _storageConfig.AccountKey);

            // Create the blob client.
            BlobClient blobClient = new BlobClient(blobUri, storageCredentials);

            // Upload the file
            await blobClient.UploadAsync(fileStream);

            return await Task.FromResult(true);
        }

        public static async Task<bool> DeleteFileFromStorage(AzureStorageConfig _storageConfig,ImageModel imageData)
        {

            // Create a URI to the blob
            Uri imageBlobUri = new Uri(imageData.imageUrl);
            Uri thumnailBlobUri = new Uri(imageData.imageUrl);

            // Create StorageSharedKeyCredentials object by reading
            // the values from the configuration (appsettings.json)
            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(_storageConfig.AccountName, _storageConfig.AccountKey);

            // Create the blob clients.
            BlobClient imageBlobClient = new BlobClient(imageBlobUri, storageCredentials);
            BlobClient thumnailBlobClient = new BlobClient(thumnailBlobUri, storageCredentials);

            // Upload the file
            await imageBlobClient.DeleteIfExistsAsync();
            await thumnailBlobClient.DeleteIfExistsAsync();

            return await Task.FromResult(true);
        }
        public static async Task<List<string>> GetThumbNailUrls(AzureStorageConfig _storageConfig)
        {
            List<string> thumbnailUrls = new List<string>();

            // Create a URI to the storage account
            Uri accountUri = new Uri("https://" + _storageConfig.AccountName + ".blob.core.windows.net/");
            
            Console.WriteLine("Creating service client");
            // Create BlobServiceClient from the account URI
            BlobServiceClient blobServiceClient = new BlobServiceClient(accountUri);
            Console.WriteLine("Created service client");
            // Get reference to the container
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(_storageConfig.ThumbnailContainer);

            if (container.Exists())
            {
                foreach (BlobItem blobItem in container.GetBlobs())
                {
                    thumbnailUrls.Add(container.Uri + "/" + blobItem.Name);
                }
            }

            return await Task.FromResult(thumbnailUrls);
        }
    }
}
