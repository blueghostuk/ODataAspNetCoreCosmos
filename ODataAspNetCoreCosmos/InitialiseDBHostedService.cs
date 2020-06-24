using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ODataAspNetCoreCosmos
{
    public class InitialiseDBHostedService : IHostedService
    {
        private readonly DbConnector _dbConnector;
        public InitialiseDBHostedService(DbConnector dbConnector)
        {
            this._dbConnector = dbConnector;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // create table if not exist
            var tableReference = this._dbConnector.GetWeatherTable();
            await tableReference.CreateIfNotExistsAsync().ConfigureAwait(false);

            var sampleData = WeatherForecast.CreateSampleData(50).ToList();
            var batchOperation = new TableBatchOperation();
            foreach (var entity in sampleData)
            {
                batchOperation.InsertOrMerge(entity);
            }
            await tableReference.ExecuteBatchAsync(batchOperation, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var tableReference = this._dbConnector.GetWeatherTable();
            await tableReference.DeleteIfExistsAsync().ConfigureAwait(false);
        }
    }
}
