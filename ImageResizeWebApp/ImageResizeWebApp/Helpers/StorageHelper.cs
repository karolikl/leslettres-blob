using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ImageResizeWebApp.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizeWebApp.Helpers
{
    public static class StorageHelper
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
                                                            AzureStorageConfig _storageConfig)
        {
            // Create a URI to the blob
            Uri blobUri = new Uri("https://" +
                                  _storageConfig.AccountName +
                                  ".blob.core.windows.net/" +
                                  _storageConfig.ImageContainer +
                                  "/" + fileName);

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

        public static async Task<List<string>> GetImageUrls(AzureStorageConfig _storageConfig)
        {
            List<string> imageUrls = new List<string>();

            // Create a URI to the storage account
            Uri accountUri = new Uri("https://" + _storageConfig.AccountName + ".blob.core.windows.net/");

            // Create BlobServiceClient from the account URI
            BlobServiceClient blobServiceClient = new BlobServiceClient(accountUri);

            // Get reference to the container
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(_storageConfig.ImageContainer);

            if (container.Exists())
            {
                foreach (BlobItem blobItem in container.GetBlobs())
                {
                    imageUrls.Add(container.Uri + "/" + blobItem.Name);
                }
            }

            return await Task.FromResult(imageUrls);
        }

        public static async Task<List<TranslatedLetter>> GetLetters(AzureStorageConfig _storageConfig)
        {
            List<TranslatedLetter> letters = new List<TranslatedLetter>();
            List<string> imageUrls = await GetImageUrls(_storageConfig);

            // Create a URI to the storage account
            Uri accountUri = new Uri("https://" + _storageConfig.AccountName + ".blob.core.windows.net/");

            // Create BlobServiceClient from the account URI
            BlobServiceClient blobServiceClient = new BlobServiceClient(accountUri);

            // Get reference to the container
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(_storageConfig.TranslatedTextContainer);
            if (container.Exists())
            {
                foreach (string imageUrl in imageUrls)
                {
                    var letter = new TranslatedLetter { ImageUrl = imageUrl };

                    BlobClient blobClient = container.GetBlobClient(letter.BlobNameWithJsonExtension);

                    BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

                    using (StreamReader reader = new StreamReader(blobDownloadInfo.Content, Encoding.UTF8))
                    {
                        string content = await reader.ReadToEndAsync();
                        letter.TranslatedText = content;
                    }

                    letters.Add(letter);
                }
            }

            return await Task.FromResult(letters);
        }
    }
}
