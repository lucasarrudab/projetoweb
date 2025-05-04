using ApiPDV.Context;
using ApiPDV.DTOs.Mapping;
using ApiPDV.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var mySqlConnection = builder.Configuration.GetConnectionString("AppContext");
builder.Services.AddDbContext<AppDbContext>(options =>
                                            options.UseMySql(mySqlConnection, ServerVersion
                                            .AutoDetect(mySqlConnection)));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();

builder.Services.AddAutoMapper(typeof(DTOMapping));


var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
         options.SwaggerEndpoint("/openapi/v1.json", "PDV api"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
