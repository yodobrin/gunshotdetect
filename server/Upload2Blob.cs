using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
// using Azure.Storage.Blobs.Models;
// using System.IO;
// using System.Text;

using Newtonsoft.Json;

namespace server
{
    public static class Upload2Blob
    {
        [FunctionName("Upload2Blob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Upload2Blob function -  processing a request.");

            string connectionString = Environment.GetEnvironmentVariable("STORAGE");
            string containerName = Environment.GetEnvironmentVariable("LANDING_CONTAINER");
            log.LogInformation($"Container Name: {containerName}");

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);  
            
            dynamic uploadResponse = new System.Dynamic.ExpandoObject();

            var formdata = await req.ReadFormAsync();
                        
            string name = formdata["name"];         

            if(string.IsNullOrEmpty(name)) {
                // name of the blob must be provided
                uploadResponse.ErrorMessage = "name is a mandatory field";
                uploadResponse.Status = "Failure";
                return new BadRequestObjectResult(JsonConvert.SerializeObject(uploadResponse));

            }
            
            uploadResponse.SourceFileName = name;


            BlobClient blobClient = containerClient.GetBlobClient(name);
            
            if( req.Form.Files["file"] == null)
            {
                uploadResponse.ErrorMessage = "Please provide a file to upload";
                uploadResponse.Status = "Failure";
                return new BadRequestObjectResult(JsonConvert.SerializeObject(uploadResponse));
            }
            
            
            try{
                await blobClient.UploadAsync(req.Form.Files["file"].OpenReadStream());                    
                uploadResponse.UploadedFile = blobClient.Uri.ToString();               
                uploadResponse.Status = "Success";        
            }catch(Exception ex)
            {
                log.LogError($"Upload2Blob function - Exception thrown during upload to blob {ex.Message}");
                uploadResponse.ErrorMessage = $"Unable to upload {name} to the container: {containerName} ";
                uploadResponse.Status = "Failure";
            }
            log.LogInformation("Upload2Blob function completed.");
            return new OkObjectResult(JsonConvert.SerializeObject(uploadResponse));
        }

        }
    }

