using System;
using System.Collections.Generic;
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

namespace Planetary.GameFunctions
{
    public class GetGame
    {
        private readonly ILogger<GetGames> _logger;

        public GetGame(ILogger<GetGames> log)
        {
            _logger = log;
        }

        [FunctionName("GetGame")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userid", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **userid** parameter")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Game), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "user/{userid}/games/{id}")] HttpRequest req,
                        [CosmosDB(Constants.DATABASE_NAME, Constants.GAME_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING,
                 Id = "{id}",
                PartitionKey = "{userid}")]
                Game game)
        {
            _logger.LogInformation("GetGame called");

            if (game == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(game);
        }
    }
}

