using VerificationService;
using VerificationService.Models;
using VerificationService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

builder.Services.AddVerificationServices();

var app = builder.Build();

var inventoryItemVerificationService = app.Services.GetService<IVerificationService<InventoryItem>>();

if (inventoryItemVerificationService == null)
{
    throw new InvalidOperationException("Unable to find IVerficationService<InventoryItem> implementation!");
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapGet("/", () => "VerificationService root Url is reachable.");

app.MapPost("/VerifyInventoryItem", async (InventoryItem item) => await inventoryItemVerificationService.VerifyRequiredFields(item));

app.Run();
