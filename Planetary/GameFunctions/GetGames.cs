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
    public class GetGames
    {
        private readonly ILogger<GetGames> _logger;

        public GetGames(ILogger<GetGames> log)
        {
            _logger = log;
        }

        [FunctionName("GetGames")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "userid", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The **userid** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Game>), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "user/{userid}/games/")] HttpRequest req,
                        [CosmosDB(Constants.DATABASE_NAME, Constants.GAME_CONTAINER,
                ConnectionStringSetting = Constants.CONNECTION_STRING,
                SqlQuery = "SELECT * FROM games g where g.UserId = {userid}")]
                IEnumerable<Game> games)
        {
            _logger.LogInformation("GetGamesByUser called");

            return new OkObjectResult(games);
        }
    }
}

