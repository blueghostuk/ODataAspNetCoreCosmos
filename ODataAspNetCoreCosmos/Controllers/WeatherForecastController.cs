using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ODataAspNetCoreCosmos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DbConnector _dbConnector;

        public WeatherForecastController(DbConnector dbConnector, ILogger<WeatherForecastController> logger)
        {
            _dbConnector = dbConnector;
            _logger = logger;
        }

        [Route("dto")]
        [HttpGet]
        public IQueryable<WeatherForecastDTO> GetDTO(ODataQueryOptions<WeatherForecastDTO> options)
        {
            /*
             * http://localhost:7292/weatherforecast/dto/?$filter=summary eq 'Cool' Fails with
   System.NotSupportedException: The filter query option cannot be specified after the select query option.
   at Microsoft.Azure.Cosmos.Table.Queryable.ResourceBinder.AddSequenceQueryOption(ResourceExpression target, QueryOptionExpression qoe)
   at Microsoft.Azure.Cosmos.Table.Queryable.ResourceBinder.AnalyzePredicate(MethodCallExpression mce)
   at Microsoft.Azure.Cosmos.Table.Queryable.ResourceBinder.VisitMethodCall(MethodCallExpression mce)
   at Microsoft.Azure.Cosmos.Table.Queryable.ALinqExpressionVisitor.Visit(Expression exp)
   at Microsoft.Azure.Cosmos.Table.Queryable.DataServiceALinqExpressionVisitor.Visit(Expression exp)
   at Microsoft.Azure.Cosmos.Table.Queryable.ResourceBinder.Bind(Expression e)
   at Microsoft.Azure.Cosmos.Table.TableQuery`1.Bind()
   at Microsoft.Azure.Cosmos.Table.TableQuery`1.GetEnumerator()
   at Microsoft.Azure.Cosmos.Table.TableQuery`1.System.Collections.IEnumerable.GetEnumerator()
   at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeList(JsonWriter writer, IEnumerable values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.SerializeValue(JsonWriter writer, Object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalWriter.Serialize(JsonWriter jsonWriter, Object value, Type objectType)
   at Newtonsoft.Json.JsonSerializer.SerializeInternal(JsonWriter jsonWriter, Object value, Type objectType)
   at Newtonsoft.Json.JsonSerializer.Serialize(JsonWriter jsonWriter, Object value)
   at Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonOutputFormatter.WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
   at Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonOutputFormatter.WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
   at Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonOutputFormatter.WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeResultAsync>g__Logged|21_0(ResourceInvoker invoker, IActionResult result)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResultFilterAsync>g__Awaited|29_0[TFilter,TFilterAsync](ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResultExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.ResultNext[TFilter,TFilterAsync](State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeResultFilters()
--- End of stack trace from previous location where exception was thrown ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|19_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)
            */

            var tableReference = this._dbConnector.GetWeatherTable();
            var dtoQuery = tableReference.CreateQuery<WeatherForecast>()
                .Where(q => q.PartitionKey == WeatherForecast.PartionKey)
                .Select(e => new WeatherForecastDTO
                {
                    Date = e.Date,
                    Summary = e.Summary,
                    TemperatureC = e.TemperatureC,
                    Name = e.Name
                });
            return options.ApplyTo(dtoQuery) as IQueryable<WeatherForecastDTO>;
        }

        [HttpGet]
        public IActionResult GetEntity(ODataQueryOptions<WeatherForecast> options)
        {
            // http://localhost:7292/weatherforecast/?$filter=summary eq 'Cool' works
            // the generated query to cosmos db is (PartitionKey eq 'WeatherForecast') and (Summary eq 'Cool')
            // /ODataWeatherSample?$filter=%28PartitionKey%20eq%20%27WeatherForecast%27%29%20and%20%28Summary%20eq%20%27Cool%27%29
            // http://localhost:7292/weatherforecast/?$filter=Name eq 'BBC' fails
            // the generated query to cosmos db is (PartitionKey eq 'WeatherForecast') and (() eq 'BBC')
            // /ODataWeatherSample?$filter=%28PartitionKey%20eq%20%27WeatherForecast%27%29%20and%20%28%28%29%20eq%20%27BBC%27%29
            var tableReference = this._dbConnector.GetWeatherTable();
            var entityQuery = tableReference.CreateQuery<WeatherForecast>()
                .Where(q => q.PartitionKey == WeatherForecast.PartionKey);

            return Ok(options.ApplyTo(entityQuery) as IQueryable<WeatherForecast>);
        }
    }
}
