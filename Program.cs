using Microsoft.EntityFrameworkCore;
using Unilever.v1.Database.config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//using SQL Server service
builder.Services.AddDbContext<UnileverDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultCons"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin();
        });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAll");

app.Run();
