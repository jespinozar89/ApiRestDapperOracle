using MyApiRestDapperOracle.Services.Implementations;
using MyApiRestDapperOracle.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Dapper;

DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

//conexion a la base de datos Oracle
builder.Services.AddScoped<OracleConnection>(_ => 
    new OracleConnection(
        builder.Configuration
        .GetConnectionString("DefaultConnection")
    )
);

// ✅ Agrega servicios para controladores
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddControllers();


// Add services to the container.
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

// Habilita el routing y los endpoints
app.UseRouting();
app.MapControllers();  // Mapea TODOS los controladores

app.Run();
