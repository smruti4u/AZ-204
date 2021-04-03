using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebJob.SDK
{
    public class Functions
    {
        public static void ProcessOrder([QueueTrigger("order")] Order order, ILogger logger)
        {
            logger.LogInformation($"Order received {order.Price}");
        }

        public static void TimerTrigger([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo info, ILogger logger)
        {
            logger.LogInformation($"Triggered At  {DateTime.Now}");
        }
    }

    public class Order
    {
        public string Id { get; set; }
        public string ResturantName { get; set; }

        public string Price { get; set; }

        public Address adress { get; set; }
    }

    public class Address
    {
        public string State { get; set; }
        public string Country { get; set; }

        public string Code { get; set; }
    }


}
