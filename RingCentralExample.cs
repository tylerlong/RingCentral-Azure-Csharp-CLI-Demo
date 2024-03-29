using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RingCentral;

namespace MyAwesomeProject
{
    public static class RingCentralExample
    {
        [FunctionName("RingCentralExample")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var clientId = req.Query["clientId"];
            var clientSecret = req.Query["clientSecret"];
            var username = req.Query["username"];
            var extension = req.Query["extension"];
            var password = req.Query["password"];
            
            var rc = new RestClient(clientId, clientSecret, true);
            try {
                await rc.Authorize(username, extension, password);
                var extInfo = await rc.Restapi().Account().Extension().Get();
                return new OkObjectResult($"{DateTime.Now.ToString("yyyyMMdd HH:mm:ss ffff")}\n\nHello, {extInfo.contact.firstName} {extInfo.contact.lastName}.");
            } catch(RestException re) {
                return new OkObjectResult($"{DateTime.Now.ToString("yyyyMMdd HH:mm:ss ffff")}\n\n{Utils.FormatHttpMessage(re.httpResponseMessage, re.httpRequestMessage)}");
            }
        }
    }
}
