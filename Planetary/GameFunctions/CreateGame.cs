using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Planetary.Domain;

namespace Planetary.GameFunctions.CreateGame
{
    public class CreateGame
    {
        private readonly ILogger<CreateGame> _logger;


        private record CreateGameRequest(string UserId, string CallSign, string FirstName, string LastName);

        public CreateGame(ILogger<CreateGame> log)
        {
            _logger = log;
        }

        [FunctionName("CreateGame")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody("text/json", typeof(CreateGameRequest), Description = "The **game** parameter", Example = typeof(CreateGameRequest))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Game), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: Constants.DATABASE_NAME,
                collectionName: Constants.GAME_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING)] IAsyncCollector<object> games,
            [CosmosDB(Constants.DATABASE_NAME, Constants.DEFAULT_PLACE_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING,
                 Id = DefaultPlace.DefaultPlaceId,
                PartitionKey = DefaultPlace.DefaultPlaceId)]
                DefaultPlace place)
        {
            _logger.LogInformation("CreateGame called");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var input = JsonConvert.DeserializeObject<CreateGameRequest>(requestBody);

                var game = new Game
                {
                    Id = Guid.NewGuid(),
                    CallSign = input.CallSign,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    UserId = input.UserId,
                    CurrentPlace = place.Place
                };

                await games.AddAsync(game);

                return new OkObjectResult(game);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't insert item. Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

