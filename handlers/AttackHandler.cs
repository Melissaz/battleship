using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BattleshipGame.services;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace BattleshipGame.handlers;

public class AttackRequest: BattleshipGameBaseRequest
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class AttackHandler
{
    private static readonly AmazonDynamoDBClient client = new();

    private static BattleshipService _battleshipService;
    private readonly ResponseService _responseService;

    public AttackHandler()
    {
        _battleshipService = new BattleshipService(client);
        _responseService = new ResponseService();
    }
    
    public async Task<APIGatewayProxyResponse> Attack(APIGatewayProxyRequest request)
    {
        var gameRequest = JsonSerializer.Deserialize<AttackRequest>(request.Body);
        var result = await AttackToDynamoDb(gameRequest, CancellationToken.None);

        return _responseService.CreateResponse(result);
    }

    private static async Task<string> AttackToDynamoDb(AttackRequest request,
        CancellationToken cancellationToken)
    {
        var getItemRequest = new GetItemRequest
        {
            TableName = "BattleshipGame",
            Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue($"GAME#{request.GameId}") },
                { "SK", new AttributeValue($"PLAYER#{request.PlayerId}") }
            }
        };

        var response = await client.GetItemAsync(getItemRequest, cancellationToken);
    
        var currentBoard = response.Item["Board"].L;

        var index = request.X + request.Y * 10;

        switch (currentBoard[index].S)
        {
            case "H":
                return "Previously Hit";
            case "M":
                return "Previously Missed";
            case "B":
            {
                currentBoard[index] = new AttributeValue("H");

                await _battleshipService.UpdateBoard(request, currentBoard, cancellationToken);

                return "Hit";
            }
            default:
            {
                currentBoard[index] = new AttributeValue("M");
            
                await _battleshipService.UpdateBoard(request, currentBoard, cancellationToken);

                return "Miss";
            }
        }
    }

}