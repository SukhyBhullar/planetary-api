using System;
using System.Collections;
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
    public class SetDefaultPlace
    {
        private readonly ILogger<SetDefaultPlace> _logger;

        private record SetDefaultPlaceRequest(string DefaultPlace);

        public SetDefaultPlace(ILogger<SetDefaultPlace> log)
        {
            _logger = log;
        }

        [FunctionName("SetDefaultPlace")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("text/json", typeof(SetDefaultPlaceRequest), Description = "The **place** parameter", Example = typeof(SetDefaultPlaceRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.DEFAULT_PLACE_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING)] DocumentClient client)
        {
            _logger.LogInformation("SetDefaultPlace called");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<SetDefaultPlaceRequest>(requestBody);

                var toWrite = new DefaultPlace { Place = input.DefaultPlace };

                await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DATABASE_NAME, Constants.DEFAULT_PLACE_CONTAINER), toWrite);

                return new OkObjectResult(input);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't insert item. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

