using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace ODataAspNetCoreCosmos
{
    public class WeatherForecast : TableEntity
    {
        public const string PartionKey = "WeatherForecast";

        public WeatherForecast()
        {
            this.PartitionKey = PartionKey;
        }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public int TemperatureC { get; set; }

        [DataMember]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        [Required]
        public String Name { get; set; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public static IEnumerable<WeatherForecast> CreateSampleData(Int32 count)
        {
            var rng = new Random();
            return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                RowKey = index.ToString(),
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                Name = "BBC"
            })
            .ToArray();
        }
    }

    public class WeatherForecastDTO
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public int TemperatureC { get; set; }

        [DataMember]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public String Name { get; set; }
    }
}
