using System.Data;
using Dapper;
using MyApiRestDapperOracle.Data;
using MyApiRestDapperOracle.Models.Entities;
using MyApiRestDapperOracle.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace MyApiRestDapperOracle.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly OracleConnection _dbConnection;

        public CustomerService(OracleConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<Customer?> GetById(int id)
        {
            await using var connection = new OracleConnection(
                _dbConnection.ConnectionString
            );
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();

            parameters.Add(
                "p_customer_id",
                OracleDbType.Int32,
                ParameterDirection.Input,
                id
            );

            parameters.Add(
                "p_cursor",
                OracleDbType.RefCursor,
                ParameterDirection.Output
            );

            // Ejecutar el procedimiento almacenado y recuperar los datos
            var customers = await connection.QueryFirstOrDefaultAsync<Customer>(
                sql: "PKG_CUSTOMERS.GetByIdCustomer",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return customers;
        }

        public async Task<List<Customer>> GetAll()
        {
            await using var connection = new OracleConnection(
                _dbConnection.ConnectionString
            );
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();

            parameters.Add(
                "p_cursor",
                OracleDbType.RefCursor,
                ParameterDirection.Output
            );

            // Ejecutar el procedimiento almacenado y recuperar los datos
            var customers = await connection.QueryAsync<Customer>(
                sql: "PKG_CUSTOMERS.GetAllCustomers",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return customers.ToList();
        }

        public async Task<int> Add(Customer customer)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();
            // Agregamos el parámetro de entrada para el email
            parameters.Add("p_email_address", OracleDbType.Varchar2, ParameterDirection.Input, customer.EmailAddress);
            // Agregamos el parámetro de entrada para el full name
            parameters.Add("p_full_name", OracleDbType.Varchar2, ParameterDirection.Input, customer.FullName);
            // Agregamos el parámetro de salida para el Customer ID
            parameters.Add("p_customer_id", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_CUSTOMERS.AddCustomer",
                 param: parameters,
                 commandType: CommandType.StoredProcedure
            );

            // Obtén el valor generado y asígnalo al objeto Customer
            customer.CustomerId = parameters.Get<int>("p_customer_id");

            return customer.CustomerId;
        }

        public async Task Update(Customer customer)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();
            // Parámetros de entrada
            parameters.Add("p_customer_id", OracleDbType.Int32, ParameterDirection.Input, customer.CustomerId);
            parameters.Add("p_email_address", OracleDbType.Varchar2, ParameterDirection.Input, customer.EmailAddress);
            parameters.Add("p_full_name", OracleDbType.Varchar2, ParameterDirection.Input, customer.FullName);
            // Parámetro de salida para conocer cuántas filas fueron actualizadas.
            parameters.Add("p_rows_updated", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_CUSTOMERS.UpdateCustomer",
                 param: parameters,
                 commandType: CommandType.StoredProcedure
            );

            int rowsUpdated = parameters.Get<int>("p_rows_updated");

            // Opcional: Verificar que efectivamente se haya actualizado 1 registro.
            if (rowsUpdated != 1)
            {
                throw new Exception($"La actualización no se completó correctamente. Filas actualizadas: {rowsUpdated}");
            }
        }

        public async Task Delete(int id)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();
            // Parámetro de entrada: ID del cliente a eliminar
            parameters.Add("p_customer_id", OracleDbType.Int32, ParameterDirection.Input, id);
            // Parámetro de salida: Número de filas eliminadas
            parameters.Add("p_rows_deleted", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_CUSTOMERS.DeleteCustomer",  // Asegúrate de utilizar el nombre del procedimiento según corresponda
                 param: parameters,
                 commandType: CommandType.StoredProcedure);

            int rowsDeleted = parameters.Get<int>("p_rows_deleted");

            // Verifica que se haya eliminado algún registro. 
            // Si no se eliminó ninguna fila, se lanza una excepción con un mensaje adecuado.
            if (rowsDeleted != 1)
            {
                throw new Exception($"No se eliminó ningún cliente con ID {id}. Filas eliminadas: {rowsDeleted}");
            }
        }
    }
}