The new behavior uses the `AddOpenApi()` and `MapOpenApi()` extension methods, which are imported from the [`Microsoft.AspNetCore.OpenApi` NuGet package](https://www.nuget.org/packages/Microsoft.AspNetCore.OpenApi/), to register and enable the OpenAPI middleware. 

Microsoft.AspNetCore.OpenApi
Swashbuckle.AspNetCore.SwaggerUI
Swashbuckle.AspNetCore.ReDoc
NSwag.AspNetCore
Scalar.AspNetCore


https://localhost:{port}/openapi/v1.jso 
https://localhost:{port}/swagger 


```cs 
var builder = WebApplication.CreateBuilder(args);
 
// Generate OpenAPI document
builder.Services.AddOpenApi();
 
var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    // Register an endpoint to access the OpenAPI document
    app.MapOpenApi();
 
    // Render the OpenAPI document using Swagger UI
    // Available at https://localhost:{port}/swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });
 
    // Render the OpenAPI document using NSwag's Swagger UI
    // Available at https://localhost:{port}/nswag-swagger
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
        // Update the path to not conflict with the Swashbuckle's version of Swagger UI
        options.Path = "/nswag-swagger";
    });
 
    // Render the OpenAPI document using Redoc
    // Available at https://localhost:{port}/api-docs
    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });
 
    // Render the OpenAPI document using NSwag's version of Redoc
    // Available at https://localhost:{port}/swagger
    app.UseReDoc(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
        // Update the path to not conflict with the Swagger UI
        options.Path = "/nswag-redoc";
    });
 
    // Render the OpenAPI document using Scalar
    // Available at https://localhost:{port}/scalar/v1
    app.MapScalarApiReference();
}
 
app.UseHttpsRedirection();
 
app.Run();

```