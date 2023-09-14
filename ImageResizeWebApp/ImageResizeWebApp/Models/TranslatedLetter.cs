using Azure.Storage.Blobs;
using System.IO;
using System;

namespace ImageResizeWebApp.Models
{
    public class TranslatedLetter
    {
        public string ImageUrl { get; set; }
        public string BlobName
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                    return string.Empty;

                var uri = new Uri(ImageUrl);
                var blobClient = new BlobClient(uri);
                string filenameWithoutExtension = Path.GetFileNameWithoutExtension(blobClient.Name);
                return filenameWithoutExtension;
            }
        }

        public string BlobNameWithJsonExtension
        {
            get
            {
                if (string.IsNullOrEmpty(BlobName))
                    return string.Empty;

                return string.Concat(BlobName, "_tranlation.json");
            }
        }

        public string TranslatedText { get; set; }
    }
}
