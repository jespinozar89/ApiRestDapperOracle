using System.Data;
using Dapper;
using MyApiRestDapperOracle.Data;
using MyApiRestDapperOracle.Models.Entities;
using MyApiRestDapperOracle.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace MyApiRestDapperOracle.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly OracleConnection _dbConnection;

        public OrderService(OracleConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Order>> GetAll()
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
            var orders = await connection.QueryAsync<Order>(
                sql: "PKG_ORDERS.GetAllOrders",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return orders.ToList();
        }

        public async Task<Order> GetById(int id)
        {
            await using var connection = new OracleConnection(
                _dbConnection.ConnectionString
            );
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();

            parameters.Add(
                "p_order_id",
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
            var orders = await connection.QueryFirstOrDefaultAsync<Order>(
                sql: "PKG_ORDERS.GetOrderById",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return orders;
        }

        public async Task<int> Add(Order order)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();
          
            parameters.Add("p_order_tms", OracleDbType.Date, ParameterDirection.Input, order.OrderTms);
            parameters.Add("p_customer_id", OracleDbType.Int32, ParameterDirection.Input, order.CustomerId);
            parameters.Add("p_order_status", OracleDbType.Varchar2, ParameterDirection.Input, order.OrderStatus);
            parameters.Add("p_store_id", OracleDbType.Int32, ParameterDirection.Input, order.StoreId);
        
            parameters.Add("p_order_id", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_ORDERS.AddOrder",
                 param: parameters,
                 commandType: CommandType.StoredProcedure
            );

            // Obtén el valor generado y asígnalo al objeto order
            order.OrderId = parameters.Get<int>("p_order_id");

            return order.OrderId;
        }

        public async Task Update(Order order)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();
            // Parámetros de entrada
            parameters.Add("p_order_id", OracleDbType.Int32, ParameterDirection.Input, order.OrderId);
            parameters.Add("p_customer_id", OracleDbType.Int32, ParameterDirection.Input, order.CustomerId);
            parameters.Add("p_order_tms", OracleDbType.Date, ParameterDirection.Input, order.OrderTms);
            parameters.Add("p_order_status", OracleDbType.Varchar2, ParameterDirection.Input, order.OrderStatus);
            parameters.Add("p_store_id", OracleDbType.Int32, ParameterDirection.Input, order.StoreId);
            // Parámetro de salida para conocer cuántas filas fueron actualizadas.
            parameters.Add("p_rows_updated", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_ORDERS.UpdateOrder",
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
            parameters.Add("p_order_id", OracleDbType.Int32, ParameterDirection.Input, id);
            // Parámetro de salida: Número de filas eliminadas
            parameters.Add("p_rows_deleted", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_ORDERS.DeleteOrder",  // Asegúrate de utilizar el nombre del procedimiento según corresponda
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