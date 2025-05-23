# Tecnologias del aplicativo

esta aplicacion contiene las siguientes tecnologias:

- .NET8
- Oracle
- Dapper
- API REST
- Inyeccion de dependencia
- DTO (Data Transfer Object)

# Crear Proyecto en Visual Studio Code
## 1. Comando crear Api .net8

Abrir terminal y ejecutar el siguiente comando:

```bash
dotnet new webapi -n MyApiProject
```

## 2. Configurar Dapper

1.- instalaremos los siguientes paquetes:

```bash
dotnet add package Dapper --version 2.1.28
dotnet add package Oracle.ManagedDataAccess.Core --version 23.8.0
```

## 3. Crear archivo global.json .net8 (Opcional)

Para trabajar con una versión específica de .NET en nuestro proyecto, debemos crear un archivo `global.json` en la raiz del proyecto y especificaremos la versión del `SDK`. (esto se debe realizar solo si tenemos versiones superiores intaladas en nuestro equipo)

Ejemplo para trabajar en la version .NET8:

```bash
{
  "sdk": {
    "version": "8.0.409"
  }
}
```

Comando para listar SDKs
```bash
dotnet --list-sdks
```

## 4. Crear Modelo y Contexto

Esquema general del proyecto:

```bash
YourProject/
│
├── Controllers/         <- API Controllers
│   ├── CustomersController.cs
│   ├── OrderController.cs
│   ├── OrderItemController.cs
│
├── Data/                <- OracleDynamicParameters
│   ├── OracleDynamicParameters.cs
│
├── Models/              <- Entities and DTOs
│   ├── Entities/
│   │   ├── Customer.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   ├── DTOs/
│       ├── CustomerDTO.cs
│       ├── OrderDTO.cs
│       ├── OrderItemDTO.cs
│
├── Services/          <- Interfaces and implementations classes
│   ├── Interfaces/
│   │   ├── ICustomerService.cs
│   │   ├── IOrderService.cs
│   │   ├── IOrderItemService.cs
│   ├── Implementations/
│       ├── CustomerService.cs
│       ├── OrderService.cs
│       ├── OrderItemService.cs
│
│── appsettings.Development.json
│── appsettings.json
│── global.json
│── MyApiORACLE.csproj
│── Program.cs
```

1. Crear carpeta `/Models/Entities`.
2. Crear nuestros entidades `/Models/Entities/Customer.cs`, ejemplo:

```bash
namespace MiApiORACLE.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }//PK
        public string? EmailAddress { get; set; }
        public string? FullName { get; set; }

    }
}
```

**Nota**: Es fundamental respetar la nomenclatura de las columnas tal como aparecen en la base de datos. Por ejemplo, si una columna se denomina `CUSTOMER_ID` en la base de datos, en nuestro modelo deberíamos nombrarla de manera consistente, por ejemplo como `CustomerId` o `customerId`, para evitar inconvenientes al recuperar y mapear los datos.

3. Crea carpeta `/Data` y añadir arhivo `OracleDynamicParameters.cs`:

```bash
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using Oracle.ManagedDataAccess.Types;

namespace MyApiRestDapperOracle.Data
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly List<OracleParameter> oracleParameters = new();

        /// <summary>
        /// Agrega un parámetro Oracle con el nombre, tipo, dirección y valor especificados.
        /// </summary>
        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction, object? value = null)
        {
            var param = new OracleParameter(name, oracleDbType)
            {
                Direction = direction
            };
            if (value != null)
            {
                param.Value = value;
            }
            oracleParameters.Add(param);
        }

        /// <summary>
        /// Este método se invoca internamente por Dapper para añadir los parámetros al comando.
        /// </summary>
        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            // Si se trata de un OracleCommand, habilitamos la vinculación por nombre
            if (command is OracleCommand oracleCommand)
            {
                oracleCommand.BindByName = true;
            }
            foreach (var param in oracleParameters)
            {
                command.Parameters.Add(param);
            }
        }

        public T Get<T>(string name)
        {
            var param = oracleParameters.FirstOrDefault(x => x.ParameterName == name);
            if (param == null)
            {
                throw new ArgumentException($"No se encontró el parámetro '{name}'");
            }
            object value = param.Value;
            // Si el valor es OracleDecimal, conviertele al tipo deseado.
            if (value is OracleDecimal oracleDec)
            {
                if (typeof(T) == typeof(int))
                    return (T)(object)oracleDec.ToInt32();
                if (typeof(T) == typeof(long))
                    return (T)(object)oracleDec.ToInt64();
                if (typeof(T) == typeof(decimal))
                    return (T)(object)oracleDec.Value;
                // En caso de otros tipos, intentar conversión general.
                return (T)Convert.ChangeType(oracleDec.Value, typeof(T));
            }
            // Si no, usar conversión estándar.
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
```
**OracleDynamicParameters** optimiza el uso de Dapper con Oracle, asegurando que los parámetros de entrada y salida se gestionen correctamente sin tener que manipular manualmente objetos OracleCommand.

4. Configurar Dapper para trabajar PascalCase o camelCase:

en la clase `program.cs` debemos agregar esta linea de codigo:

```bash
using Dapper;

DefaultTypeMap.MatchNamesWithUnderscores = true;
```


## 5. Configurar conexion Oracle

1. Abre el archivo `appsettings.json` y agrega la cadena de conexión:

```bash
"ConnectionStrings": {
    "DefaultConnection": "User Id=SYSTEM;Password=admin.2025;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SID=xe)));"
  }
```

2. Registra el contexto en `Program.cs`:

```bash
using MyApiProject.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Agrega servicios para controladores
builder.Services.AddControllers();

//Conexion BD Oracle
builder.Services.AddScoped<OracleConnection>(_ => 
    new OracleConnection(
        builder.Configuration
        .GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();

// Habilita el routing y los endpoints
app.UseRouting();
app.MapControllers();  // Mapea TODOS los controladores
```

## 6. Crear DTO

1. Crearemos la carpeta `/DTO` en `/Models`
2. Crearemos nuestros archivos DTO, ejemplo `CustomerDTO.cs`

```bash
namespace MiApiORACLE.DTO
{
    public class CustomerDTO
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
    }
}
```

## 7. Aplicar Servicios

1.Crearemos la carpetas `/Services` y dentro de ella `/Implementations` y `/Interfaces`.

2.Crear servicios específicos por entidad: Ejemplo Customer:

`/Interfaces`:

```bash
namespace MyApiRestDapperOracle.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetById(int id);
        Task<List<Customer>> GetAll();
        Task<int> Add(Customer customer);
        Task Update(Customer customer);
        Task Delete(int id);
    }
}
```

`/Implementations`

```bash
namespace MyApiRestDapperOracle.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly OracleConnection _dbConnection;

        public CustomerService(OracleConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        //Aqui agregar la implementaciones de los metodos de la intefaz

    }
}
```

## 8. Inyeccion de Dependencias

1. Registrar servicios: En `Program.cs`:

```bash

var builder = WebApplication.CreateBuilder(args);

// Registro de Servicios 
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

var app = builder.Build();
```

## 9. Inyectar repositorios en los controladores

1. Crear clases api controller en la carpeta `/Controllers`
2. Usa el constructor de tus controladores para recibir las dependencias:

```bash
namespace MyApiRestDapperOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        //Crear Metodos https

    }
}
```

3. usas tus DTOs: ejemplo metodo `HttpPost` con `try-catch`

```bash
[HttpPost]
public async Task<IActionResult> Add(CustomerDTO customer)
{
    try
    {
        if (customer == null)
        {
            return BadRequest("Customer object is null.");
        }

        var newCustomer = new Customer
        {
            EmailAddress = customer.EmailAddress,
            FullName = customer.FullName
        };

        newCustomer.CustomerId = await _customerService.Add(newCustomer);
        return CreatedAtAction(nameof(GetById), new { id = newCustomer.CustomerId }, newCustomer);
    }
    catch (OracleException ex) when (ex.Number >= 20000 && ex.Number <= 20999)
    {
        // Errores personalizados de Oracle (RAISE_APPLICATION_ERROR)
        return BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}
```