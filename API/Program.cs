
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            //Update database and data seeding
            try
            {
                using var Scope = app.Services.CreateScope();
                var ScopeServices = Scope.ServiceProvider;
                //Update database
                var StoreContext = ScopeServices.GetRequiredService<StoreContext>();
                await StoreContext.Database.MigrateAsync();

                //data seeding
                await StoreContextSeed.SeedAsync(StoreContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            app.Run();
        }
    }
}
