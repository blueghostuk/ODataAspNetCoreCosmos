using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;

namespace ODataAspNetCoreCosmos
{
    public class DbConnector
    {
        public CloudTableClient Client { get; }

        public DbConnector(IConfiguration configuration)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("Cosmos"));
            this.Client = storageAccount.CreateCloudTableClient();
        }

        public CloudTable GetWeatherTable()
        {
            return this.Client.GetTableReference("ODataWeatherSample");
        }
    }
}
