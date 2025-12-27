using MiniOrderApp.Services;
using MiniOrderApp.Repositories;
using MiniOrderApp.Import.Services;
using MiniOrderApp.Services.Interfaces;
using MiniOrderApp.Repositories.Interfaces;

namespace MiniOrderApp;

public class Program
{
    private const string LocalCorsPolicy = "LocalCors";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(LocalCorsPolicy, policy =>
            {
                policy
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IImportService, ImportService>();

        var app = builder.Build();

        app.UseMiddleware<GlobalExceptionMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(LocalCorsPolicy);

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}