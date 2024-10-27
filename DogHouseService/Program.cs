using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add database context
builder.Services.AddDbContext<DogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use Swagger for API documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply rate-limiting middleware
app.UseMiddleware<RateLimitMiddleware>();

// Configure endpoints
app.MapGet("/ping", () => Results.Ok("Dogshouseservice.Version1.0.1"));

app.MapGet("/dogs", async (DogContext context, [FromQuery] string attribute, [FromQuery] string order, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) =>
{
    var dogsQuery = context.Dogs.AsQueryable();

    // Apply sorting based on attribute and order
    if (!string.IsNullOrEmpty(attribute) && !string.IsNullOrEmpty(order))
    {
        dogsQuery = attribute.ToLower() switch
        {
            "weight" when order.ToLower() == "desc" => dogsQuery.OrderByDescending(d => d.Weight),
            "weight" => dogsQuery.OrderBy(d => d.Weight),
            "tail_length" when order.ToLower() == "desc" => dogsQuery.OrderByDescending(d => d.TailLength),
            "tail_length" => dogsQuery.OrderBy(d => d.TailLength),
            _ => dogsQuery
        };
    }

    // Apply pagination
    var dogs = await dogsQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    return Results.Ok(dogs);
});

app.MapPost("/dog", async (DogContext context, Dog newDog) =>
{
    // Validation checks
    if (await context.Dogs.AnyAsync(d => d.Name == newDog.Name))
        return Results.BadRequest("A dog with this name already exists.");

    if (newDog.TailLength < 0 || newDog.Weight < 0)
        return Results.BadRequest("Tail length and weight must be positive numbers.");

    context.Dogs.Add(newDog);
    await context.SaveChangesAsync();
    return Results.Created($"/dogs/{newDog.Id}", newDog);
});

// Run the application
app.Run();


// Rate Limiting Middleware
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static int _requestCount = 0;
    private const int _requestLimit = 10;

    public RateLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _semaphore.WaitAsync();
        try
        {
            _requestCount++;
            if (_requestCount > _requestLimit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too Many Requests");
                return;
            }
        }
        finally
        {
            _semaphore.Release();
        }

        await _next(context);

        // Reset request count every second (simplified for demonstration)
        _requestCount = 0;
    }
}
