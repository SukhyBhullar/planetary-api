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

namespace Planetary.GameFunctions
{
    public class SetCurrentPlace
    {
        private record SetCurrentPlaceRequest(string Place);

        private readonly ILogger<SetCurrentPlace> _logger;

        public SetCurrentPlace(ILogger<SetCurrentPlace> log)
        {
            _logger = log;
        }

        [FunctionName("SetCurrentPlace")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userid", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **userid** parameter")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **id** parameter")]
        [OpenApiRequestBody("text/json", typeof(SetCurrentPlaceRequest), Description = "The **game** parameter", Example = typeof(SetCurrentPlaceRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Game), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user/{userid}/games/{id}/setCurrentPlace")] HttpRequest req,
            [CosmosDB(Constants.DATABASE_NAME, Constants.GAME_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING,
                 Id = "{id}",
                PartitionKey = "{userid}")]
                Game game,
            [CosmosDB(databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.GAME_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING)] DocumentClient client)
        {
            _logger.LogInformation("SetCurrentPlace called");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<SetCurrentPlaceRequest>(requestBody);
                game.CurrentPlace = input.Place;
                await client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(Constants.DATABASE_NAME, Constants.GAME_CONTAINER), game);

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

