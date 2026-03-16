![[The Architecture of ASP.NET Core.png]]
![[Pasted image 20260228235125.png]]

Configuring the Application - For API
```cs
builder.Services.AddControllers();
app.MapControllers();
```

Using the Route Attribute
```cs
[Route("api/[controller]")]
public class PieController : ControllerBase
{
	private readonly IPieRepository _pieRepository;
	[HttpGet]
	public IActionResult GetAll()
	{
	...
	} 
	
	[HttpGet("{id}")]
	public IActionResult GetById(int id)
	{
	...
	}
}
```

## Action Result Methods
- Ok
- BadRequest
- NotFound
- NoContent

##  Using MapOpenApi()  && Bringing in the Swagger UI

```cs
builder.Services.AddOpenApi();
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(options =>
	options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}
```
- Requires NuGet package