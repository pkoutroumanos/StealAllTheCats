using StealAllTheCats.Services;
using StealAllTheCats.Data;
using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Repositories;
using StealAllTheCats.Mapping;
using StealAllTheCats.Infastracture;
using StealAllTheCats.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<CatApiOptions>(builder.Configuration.GetSection("CatApi"));
builder.Services.AddHttpClient<ICatApiService, CatApiService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<CatApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
});
builder.Services.AddSingleton<ICacheTokenProvider, CacheTokenProvider>();

builder.Services.AddScoped<ICatRepository, CatRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ICatQueryService, CatQueryService>();
builder.Services.Decorate<ICatQueryService, CachedCatQueryService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StealAllTheCats API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
