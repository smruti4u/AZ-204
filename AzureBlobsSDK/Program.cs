using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureBlobsSDK
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string fileName = "Mobile-Engagement.svg";
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=azstorageaccount204;AccountKey=bB0Y/sDN/32ZtTBiynihekpjCI3cyk9MaJON7MWuh422QXRgv0bFE5oWYfvYDSlQJ7fEFiUqQeIsgkQ+q/TNUw==;EndpointSuffix=core.windows.net";
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("vscontainer");
            containerClient.CreateIfNotExists();

            // Uploading a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            blobClient.Upload(fileName, overwrite: true);

            BlobClient blob2 = containerClient.GetBlobClient("test/" + fileName);
            var content = blob2.Upload(fileName, overwrite: true);

            HandleConCurrency(ConCurrencyType.Default, blob2, content);
            HandleConCurrency(ConCurrencyType.Optimistic, blob2, content);
            HandleConCurrency(ConCurrencyType.Pessimistic, blob2, content);

            // Listing Blobs
            var blobs = containerClient.GetBlobs();

            foreach(var blob in blobs)
            {      
                
                Console.WriteLine(blob.Name);
                
            }

            var testBlobs = containerClient.GetBlobs(prefix: "test");

            foreach (var blob in testBlobs)
            {                
                Console.WriteLine(blob.Name);
            }

            IDictionary<string, string> metadata = new Dictionary<string, string>()
            {
                ["DocType"] = "Audit",
                ["Owner"] = "Sam"
            };
            await ListBlobsSegments(containerClient).ConfigureAwait(false);
            blob2.SetMetadata(metadata);
        }

        private static async Task ListBlobsSegments(BlobContainerClient conatiner)
        {
            try
            { 
                int segmentSize = 1;
                var resultPages = conatiner.GetBlobsAsync().AsPages(default, segmentSize);

                var resultPagesVersioning = conatiner.GetBlobsAsync(default, BlobStates.Version).AsPages(default, segmentSize);

                await foreach (var blobPage in resultPages)
                {
                    foreach (var blobItem in blobPage.Values)
                    {
                        Console.WriteLine(blobItem.Name);
                    }
                }

                await foreach (var page in resultPagesVersioning)
                {
                    foreach (var blobItem in page.Values)
                    {
                       if( blobItem.VersionId != null)
                        {
                            var blobUri = $"{conatiner.Uri}/{blobItem.Name}/{blobItem.VersionId}";
                            Console.WriteLine(blobUri);
                            Console.WriteLine($"Is Current Version : {blobItem.IsLatestVersion}");
                        }
                    }
                }
            }
            catch(Exception exe)
            {

            }
        }
        
        private static void HandleConCurrency(ConCurrencyType type, BlobClient blob, BlobContentInfo info)
        {
            BlobLeaseClient leaseClient = blob.GetBlobLeaseClient();
            BlobProperties properties= blob.GetProperties();

            switch (type)
            {
                case ConCurrencyType.Default:
                    IDictionary<string, string> metadata = new Dictionary<string, string>()
                    {
                        ["DocType"] = "Default",
                        ["Owner"] = "Sam"
                    };
                    blob.SetMetadata(metadata);
                    break;
                case ConCurrencyType.Pessimistic:
                   BlobLease leased =  leaseClient.Acquire(TimeSpan.FromSeconds(40));
                    IDictionary<string, string> metadata3 = new Dictionary<string, string>()
                    {
                        ["DocType"] = "Pessimistic",
                        ["Owner"] = "Sam"
                    };
                    
                    BlobRequestConditions blobRequest1 = new BlobRequestConditions()
                    {
                        LeaseId = leased.LeaseId
                    };
                    try
                    {
                        blob.SetMetadata(metadata3, blobRequest1);
                    }
                    catch(Exception exe)
                    {

                    }
                    
                    break;
                case ConCurrencyType.Optimistic:
                    IDictionary<string, string> metadata2 = new Dictionary<string, string>()
                    {
                        ["DocType"] = "optimistic",
                        ["Owner"] = "Sam"
                    };
                    BlobRequestConditions blobRequest = new BlobRequestConditions()
                    {
                        IfMatch = info.ETag
                    };
                    try
                    {
                        blob.SetMetadata(metadata2, blobRequest);
                    }
                    catch(Exception exe)
                    {

                    }
                    break;
            }
        }
    
    
    }
}
