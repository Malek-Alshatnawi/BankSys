
using BankSys.Middleware;
using BankSys.Models;
using BankSys.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

namespace BankSys
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            

            builder.Services.AddScoped<CustomerRepository>();
            builder.Services.AddScoped<AccountRepository>();
            builder.Services.AddScoped<TransactionRepository>();
            builder.Services.AddScoped<TransferRepository>();


            builder.Services.AddDbContext<MyDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {

                    Title = "Bank System APIs Mangamgemts Done By Malek Shatnawi",
                    Version = "v1",
                    Description = "This APIs are used to manage the bank APIs.",
                    Contact = new OpenApiContact
                    {
                        Name = "Malek Shatnawi",
                        Email = "malekshatnawi1@email.com"
                    }
                });
                options.EnableAnnotations();


            });


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
