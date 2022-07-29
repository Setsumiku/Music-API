using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Music_API.Data.Context;
using Music_API.Data.DAL;
using Serilog;
using Serilog.Events;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title="Music Api",
        Description="API For managing music and music adjacent areas",
        Version="v1"
    });
    var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
    options.IncludeXmlComments(filePath);
});
var connectionString = builder.Configuration.GetConnectionString("Music_API_Db");
builder.Services.AddDbContext<Music_API_Context>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var logger = new LoggerConfiguration().WriteTo.File(builder.Configuration.GetValue<string>("Logging:FilePath")).CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json","Swagger Music API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
