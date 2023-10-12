using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using BattleshipGame.services;
using System.Text.Json;

[assembly: LambdaSerializer(
    typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BattleshipGame.handlers;

public class BattleshipGameBaseRequest
{
    public string GameId { get; set; }
    public string PlayerId { get; set; }
}

public class BoardHandler
{
    private static readonly AmazonDynamoDBClient client = new();
    private readonly ResponseService _responseService;

    public BoardHandler()
    {
        _responseService = new ResponseService();
    }

    public async Task<APIGatewayProxyResponse> CreateBoard(APIGatewayProxyRequest request)
    {
        
        var gameRequest = JsonSerializer.Deserialize<BattleshipGameBaseRequest>(request.Body);
        var result = await CreateBoardToDynamoDb(gameRequest, CancellationToken.None);
        return _responseService.CreateResponse(result);
    }

    private async Task<PutItemResponse> CreateBoardToDynamoDb(BattleshipGameBaseRequest request,
        CancellationToken cancellationToken)
    {
        var putItem = new PutItemRequest
        {
            TableName = "BattleshipGame",
            Item = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue($"GAME#{request.GameId}") },
                { "SK", new AttributeValue($"PLAYER#{request.PlayerId}") },
                { "Board", new AttributeValue { L = Enumerable.Repeat(new AttributeValue("_"), 100).ToList() } }
            }
        };

        return await client.PutItemAsync(putItem, cancellationToken);
    }
}