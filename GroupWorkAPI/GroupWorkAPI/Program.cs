using GroupWorkAPI.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseLazyLoadingProxies().UseNpgsql(connectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var myAllowSpecifirOrigins = "_myAllowSpecifirOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecifirOrigins,
                        policy =>
                        {
                            policy.WithOrigins("http://localhost:5000", "http://localhost:3000")
                                                .AllowAnyHeader()
                                                .AllowAnyMethod()
                                                .AllowAnyOrigin();
                        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors(myAllowSpecifirOrigins);

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
