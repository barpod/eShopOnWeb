using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserver;

public static class OrderItemsReserverFunction
{
    [FunctionName("OrderItemsReserver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "OrderItemsReserver/{orderId}")] HttpRequest req,
        [CosmosDB(
                databaseName: "ToDoItems",
                containerName: "cosmosdbsqlcontainer",
                Connection = "CosmosDBConnection")]IAsyncCollector<dynamic> documentsOut,
        string orderId,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        await documentsOut.AddAsync(new { Order = requestBody, id = orderId });
        return new OkObjectResult("Order id " + orderId);
    }
}
