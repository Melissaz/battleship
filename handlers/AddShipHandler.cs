using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using BattleshipGame.services;
using System.Text.Json;
using Amazon.Lambda.Core;

namespace BattleshipGame.handlers;
public class AddShipRequest: BattleshipGameBaseRequest
{
    public int StartX { get; set; }
    public int StartY { get; set; }
    public string Orientation { get; set; }
}

public class AddShipHandler
{
    private static readonly AmazonDynamoDBClient client = new();

    private static BattleshipService _battleshipService;
    private readonly ResponseService _responseService;


    public AddShipHandler()
    {
        _battleshipService = new BattleshipService(client);
        _responseService = new ResponseService();
    }
    
    public async Task<APIGatewayProxyResponse> AddBattleship(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var gameRequest = JsonSerializer.Deserialize<AddShipRequest>(request.Body);
        var result = await AddBattleShipToDynamoDb(gameRequest, CancellationToken.None, context);
        return _responseService.CreateResponse(result);
    }

    private async Task<UpdateItemResponse> AddBattleShipToDynamoDb(AddShipRequest request,
        CancellationToken cancellationToken, ILambdaContext context)
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
        List<AttributeValue> currentBoard;
        try
        {
            currentBoard = response.Item["Board"].L;
        }
        catch (Exception e)
        {
            LogMessage(context, "error" + e.Message);

            throw;
        }

        //assume ship length is 5
        for (int i = 0; i < 5; i++)
        {
            int index;
            if (request.Orientation.ToLower() == "horizontal")
            {
                index = (request.StartX + i) + request.StartY * 10;
            }
            else
            {
                index = request.StartX + (request.StartY + i) * 10;
            }
            currentBoard[index] = new AttributeValue("B");
        }

        return await _battleshipService.UpdateBoard(request, currentBoard, cancellationToken);
    }
    
    void LogMessage(ILambdaContext ctx, string msg)
    {
        ctx.Logger.LogLine(
            string.Format("{0}:{1} - {2}",
                ctx.AwsRequestId,
                ctx.FunctionName,
                msg));
    }
}