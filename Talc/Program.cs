
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Talc;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        string connectionString =
            builder.Configuration.GetConnectionString("ApplicationContext")
            ?? throw new InvalidOperationException("Connection string"
            + "'ApplicationContext' not found.");
        
        builder.Services.AddDbContext<ApplicationContext>(x => x.UseSqlite(connectionString));
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseAuthorization();

        app.Run();
    }
}
