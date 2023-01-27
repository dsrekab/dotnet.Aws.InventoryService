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

app.MapGet("/InventoryService", () => "InventorySerivce Endpoint is reachable");

app.MapGet("/InventoryService/GetAllInventoryItems", async () => await inventoryService.GetAllInventoryItems());
app.MapGet("/InventoryService/GetInventoryItem", async (int? inventoryId, string? upc) => await inventoryService.GetSingleInventoryItem(inventoryId, upc));

app.MapPost("/InventoryService/AddItem", async (Inventory inventoryItem) => await inventoryService.AddInventoryItem(inventoryItem));

app.MapPut("/InventoryService/UpdateItem", async (Inventory inventoryItem) => await inventoryService.UpdateInventoryItem(inventoryItem));

app.MapDelete("/InventoryService/DeleteItem", async (int inventoryId) => await inventoryService.DeleteInventoryItem(inventoryId));

app.Run();
