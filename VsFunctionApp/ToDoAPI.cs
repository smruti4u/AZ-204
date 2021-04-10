using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsFunctionApp.Entities;

namespace VsFunctionApp
{
    public static class ToDoAPI
    {
        static List<ToDoItem> items = new List<ToDoItem>();

        [FunctionName("Create")]
        public static async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Function, "post", Route = "todos")] HttpRequest req, ILogger log)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<ToDoCreateViewModel>(body);

            ToDoItem newItem = new ToDoItem(input.Description);
            newItem.Id = Guid.NewGuid().ToString();
            newItem.IsCompleted = false;

            items.Add(newItem);

            return new OkObjectResult(newItem);

        }

        [FunctionName("Update")]
        public static async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "todos/{id}")] HttpRequest req, ILogger log, string id)
        {
            var currentItem = items.Where(x => x.Id == id).FirstOrDefault();
            if(currentItem == null)
            {
                return new NotFoundObjectResult($"Item not present with id {id}");
            }

            currentItem.IsCompleted = true;
            return new OkObjectResult(currentItem);

        }

        [FunctionName("GetAll")]
        public static async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos")] HttpRequest req, ILogger log)
        {
            var UnCompleteditems = items.Where(x => x.IsCompleted == false).ToList();
            return new OkObjectResult(UnCompleteditems);
        }

        [FunctionName("GetItem")]
        public static async Task<IActionResult> GetItem([HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos/{id}")] HttpRequest req, ILogger log, string id)
        {
            var currentItem = items.Where(x => x.Id == id).FirstOrDefault();
            if (currentItem == null)
            {
                return new NotFoundObjectResult($"Item not present with id {id}");
            }

            return new OkObjectResult(currentItem);
        }
    }
}
