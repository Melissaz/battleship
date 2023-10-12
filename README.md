# Overview:
The battleShipService serverless service is powered by AWS Lambda, written in .NET 6, and uses DynamoDB for its backend. This service supports creating a board, adding battleships to the board, and attacking ships on the board. Each function can be invoked via HTTP endpoints.

# Usage:

## Create a New Board
**Endpoint:**  
`POST` - https://n5dykjfw3e.execute-api.ap-southeast-2.amazonaws.com/dev/board

**Request:**  
```json
{
  "PlayerId": "1",
  "GameId": "1"
}
```

##  place a battleship on the board
`POST` - https://n5dykjfw3e.execute-api.ap-southeast-2.amazonaws.com/dev/add-battleship


**Request:**  
```json 
{
"PlayerId": "1",
"GameId": "1",
"StartX": 0,
"StartY": 0,
"Orientation": "vertical"
}
```

##  attack a coordinate on the board
`POST` - https://n5dykjfw3e.execute-api.ap-southeast-2.amazonaws.com/dev/attack

**Request:**  
```json 
{
"PlayerId": "1",
"GameId": "1",
"X": 0,
"Y": 1
}
```

Request schema files (create_board_request.json, add_battleship_request.json, attack_request.json) for the request body format.