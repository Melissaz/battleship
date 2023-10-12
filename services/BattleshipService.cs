using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BattleshipGame.handlers;

namespace BattleshipGame.services;

public class BattleshipService
{
    private static AmazonDynamoDBClient _client;

    public BattleshipService(AmazonDynamoDBClient client)
    {
        _client = client;
    }
    
    public async Task<UpdateItemResponse> UpdateBoard(BattleshipGameBaseRequest request, 
        List<AttributeValue> currentBoard, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateItemRequest
        {
            TableName = "BattleshipGame",
            Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue($"GAME#{request.GameId}") },
                { "SK", new AttributeValue($"PLAYER#{request.PlayerId}") }
            },
            UpdateExpression = "SET Board = :b",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":b", new AttributeValue { L = currentBoard } }
            }
        };

        return await _client.UpdateItemAsync(updateRequest, cancellationToken);
    }
}