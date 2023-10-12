using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace BattleshipGame.services;

public class ResponseService
{
    public APIGatewayProxyResponse CreateResponse(object result)
    {
        var statusCode = (result != null) ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError;

        string body = (result != null) ?  JsonConvert.SerializeObject(result) : string.Empty;;

        var response = new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = body,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            }
        };

        return response;
    }
}