using Auth.Data;
using Auth.Infrastructure.JsonConverters;
using Auth.Infrastructure.Mappers;
using Auth.Infrastructure;
using Auth.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks();

//builder.Services.AddDbContext<AppDBContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddInfrastructure(builder.Configuration);
//App Services
//builder.Services.AddMemoryCache();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddSingleton<UserMapper>();

//builder.Services.AddAWSService(); // Add AWS services to the container

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.Converters.Add(new TrimmingStringConverter())
    );
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
