using InventoryService;
using InventoryService.Exceptions;
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

app.MapGet("/GetItems", () => "Miminal API - GetItems");

app.MapPost("/AddItem", () => "Miminal API - AddItem");
app.MapPost("/AddItemCount", () => "Miminal API - AddItemCount");

app.MapPut("/UpdateItem", () => "Miminal API - UpdateItem");
app.MapPut("/UpdateItemCount", () => "Miminal API - UpdateItemCount");

app.MapDelete("/DeleteItem", () => "Miminal API - DeleteItem");


app.Run();
