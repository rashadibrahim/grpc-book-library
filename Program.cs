using LibrarySystem.Models;
using LibrarySystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddDbContext<LibraryDbContext>(options =>
            {
                options.UseSqlite("Data Source=LibraryDatabase.db");
            });

            builder.Services.AddGrpc().AddJsonTranscoding();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<LibraryService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}