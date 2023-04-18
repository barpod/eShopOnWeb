using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace OrderItemsReserver;

public static class OrderItemsReserverFunction
{
    [FunctionName("OrderItemsReserver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "OrderItemsReserver/{orderId}")] HttpRequest req,
        [Blob("order-reservation-container/order-{orderId}.json", FileAccess.Write)] Stream orderIdBlob,
        string orderId,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var bytes = Encoding.UTF8.GetBytes(requestBody);
        await orderIdBlob.WriteAsync(bytes);
        return new OkObjectResult("Order id " + orderId);
    }
}
