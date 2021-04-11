using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VsFunctionApp
{
    public static class Function2
    {
        [FunctionName("ReadFromBlob")]
        [return: Table("MyTable")]
        public static TableItem Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo timer, [Blob("test/io.txt", System.IO.FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream blob, ILogger logger )
        {
            StreamReader reader = new StreamReader(blob);
            var content = reader.ReadToEnd();

            JObject jObject = JsonConvert.DeserializeObject<JObject>(content);
            return new TableItem() { jObject = jObject };
        }

        public class TableItem
        {
            public TableItem()
            {
                this.PartitionKey = new Guid().ToString();
                this.RowKey = "ConnectedCar";
            }

            public string PartitionKey { get; set; }
            public string RowKey { get; set; }

            public JObject jObject  {get; set;}
        }
    }
}
