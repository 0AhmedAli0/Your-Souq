
using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

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

            builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("cannot get redis connection string");
                var configuration = ConfigurationOptions.Parse(connection);
                return ConnectionMultiplexer.Connect(configuration);
            });

            builder.Services.AddCors(options =>//to make angular project pupllished on another domain(origin) to call this project
            {
                options.AddPolicy("CorsPolicy", options =>
                {
                    //options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                    options.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200", "http://localhost:4200");//�� ���� ���� ����� ���� ��� ������� ���� ������ �����
                });
            });

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<ICartService, CartService>();

            builder.Services.AddAuthentication();
            builder.Services.AddIdentityApiEndpoints<AppUser>()
                .AddRoles<IdentityRole>()//to configure rolles in application
                .AddEntityFrameworkStores<StoreContext>();

            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<ICouponService, CouponService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");//this middelWare will check if request comes from "https://localhost:4200"or"https://localhost:5100" if ok then requst pass if not request will refused

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();//to serve default files like index.html in wwwroot folder
            app.UseStaticFiles(); //to serve static files like images, css, js

            app.MapControllers();
            app.MapGroup("api").MapIdentityApi<AppUser>();//هذه الدالة تضيف مجموعة من نقاط النهاية الجاهزة للتعامل مع عمليات المصادقة وإدارة المستخدمين 
            //app.MapGroup("api") => result of this if we want to access the identity endpoints we should use this /api/account/register

            app.MapHub<NotificationHub>("/hub/notifications");//signalR حتي تعرف الدوت نت الي اين توجه اي طلبات خاصه ب

            app.MapFallbackToController("Index", "Fallback");//this will map any request that not match any controller to Index action in Fallback Controller

            //Update database and data seeding
            try
            {
                using var Scope = app.Services.CreateScope();
                var ScopeServices = Scope.ServiceProvider;
                //Update database
                var StoreContext = ScopeServices.GetRequiredService<StoreContext>();
                await StoreContext.Database.MigrateAsync();

                //data seeding
                var userManager = ScopeServices.GetRequiredService<UserManager<AppUser>>();
                await StoreContextSeed.SeedAsync(StoreContext, userManager);
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
