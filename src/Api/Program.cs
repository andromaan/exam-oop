using Api.Modules;
using Application.Extensions;
using Application.Implementation.Middlewares;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();
await app.InitialiseDb();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.WorkDemo();
}

app.UseMiddleware<MiddlewareExceptionHandling>();

app.MapControllers();

app.Run();

namespace Api
{
    public partial class Program;
}