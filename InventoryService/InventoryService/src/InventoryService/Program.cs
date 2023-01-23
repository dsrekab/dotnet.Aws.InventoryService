using InventoryService;
using InventoryService.Exceptions;
using InventoryService.Models;
using InventoryService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Logging.AddAWSProvider();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddInventoryServices();

var app = builder.Build();

var inventoryService = app.Services.GetService<IInventoryService>();

if (inventoryService == null)
{
    throw new InventoryServiceException("Unable to inject IInventoryService implementation.");
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/GetAllInventoryItems", async () => await inventoryService.GetAllInventoryItems());
app.MapGet("/GetInventoryItem", async (int? inventoryId, string? upc) => await inventoryService.GetSingleInventoryItem(inventoryId, upc));

app.MapPost("/AddItem", async (Inventory inventoryItem) => await inventoryService.AddInventoryItem(inventoryItem));

app.MapPut("/UpdateItem", async (Inventory inventoryItem) => await inventoryService.UpdateInventoryItem(inventoryItem));

app.MapDelete("/DeleteItem", async (Inventory inventoryItem) => await inventoryService.DeleteInventoryItem(inventoryItem));

app.Run();
