using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<OracleConnection>(_ => 
    new OracleConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Agrega servicios para controladores
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

app.Run();
