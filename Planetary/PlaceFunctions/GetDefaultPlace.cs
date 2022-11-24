using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Planetary.Domain;

namespace Planetary.PlaceFunctions
{
    public class GetDefaultPlace
    {
        private readonly ILogger<GetDefaultPlace> _logger;

        public GetDefaultPlace(ILogger<GetDefaultPlace> log)
        {
            _logger = log;
        }


        [FunctionName("GetDefaultPlace")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(Constants.DATABASE_NAME, Constants.DEFAULT_PLACE_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING,
                 Id = DefaultPlace.DefaultPlaceId,
                PartitionKey = DefaultPlace.DefaultPlaceId)]
                DefaultPlace place)
        {
            _logger.LogInformation("GetDefaultPlace called");

            try
            {
                return new OkObjectResult(new { DefaultPlace = place.Place });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't insert item. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }
    }
}

