## Required Software
- .[NET Core SDK 3.1 + Runtime](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- [Cosmos DB Emulator](https://aka.ms/cosmosdb-emulator)
    - run the emulator ("C:\Program Files\Azure Cosmos DB Emulator\Microsoft.Azure.Cosmos.Emulator.exe") with the following arguments as administrator:
        - [/EnableTableEndpoint](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator#table-api)
- Visual Studio 2109 or VS Code

## Issues

### Cannot filter on [Required] fields

Using postman I generated the following queries:

A query to "http://localhost:7292/weatherforecast/?$filter=Name eq 'BBC'" fails. 
Using fiddler I can see the request to the cosmos db as:
 - /ODataWeatherSample?$filter=%28PartitionKey%20eq%20%27WeatherForecast%27%29%20and%20%28%28%29%20eq%20%27BBC%27%29. 
 - i.e. (PartitionKey eq 'WeatherForecast') and (() eq 'BBC')

A query to a non [Required] field, e.g. "http://localhost:7292/weatherforecast/?$filter=summary eq 'Cool'" works. The fiddler request shows:
 - /ODataWeatherSample?$filter=%28PartitionKey%20eq%20%27WeatherForecast%27%29%20and%20%28Summary%20eq%20%27Cool%27%29
 - i.e. (PartitionKey eq 'WeatherForecast') and (Summary eq 'Cool')

The only difference I can see is the [Required] attribute on the Name property, hence my assumption this is causing the error.
If I remove the [Required] attribute, the first query works.

### Cannot query on DTOs

Ideally we would have a DTO inbetween the model and what is returned to the client. 

In the example endpoint "http://localhost:7292/weatherforecast/dto/?$filter=summary eq 'Cool'" this fails with `The filter query option cannot be specified after the select query option.`

This can be worked around, by ToList on the queryable, then transform to the DTO, but by default this would allow the client to filter on properties on the entity (as far as I know)