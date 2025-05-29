using System.Data;
using Dapper;
using MyApiRestDapperOracle.Data;
using MyApiRestDapperOracle.Models.Entities;
using MyApiRestDapperOracle.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;

namespace MyApiRestDapperOracle.Services.Implementations
{
    public class OrderItemService : IOrderItemService
    {
        private readonly OracleConnection _dbConnection;
        public OrderItemService(OracleConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<OrderItem>> GetAll()
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
            var orders = await connection.QueryAsync<OrderItem>(
                sql: "PKG_ORDERITEMS.GetAllOrderItems",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return orders.ToList();
        }

        public async Task<OrderItem> GetById(int orderId,int lineItemId)
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
                orderId
            );

            parameters.Add(
                "p_line_item_id",
                OracleDbType.Int32,
                ParameterDirection.Input,
                lineItemId
            );

            parameters.Add(
                "p_cursor",
                OracleDbType.RefCursor,
                ParameterDirection.Output
            );

            // Ejecutar el procedimiento almacenado y recuperar los datos
            var orderItem = await connection.QueryFirstOrDefaultAsync<OrderItem>(
                sql: "PKG_ORDERITEMS.GetOrderItemById",
                param: parameters,
                commandType: CommandType.StoredProcedure
            );

            return orderItem;
        }

        public async Task<int> Add(OrderItem order)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();
          
            parameters.Add("p_order_id", OracleDbType.Int32, ParameterDirection.Input, order.OrderId);
            parameters.Add("p_line_item_id", OracleDbType.Int32, ParameterDirection.Input, order.LineItemId);
            parameters.Add("p_product_id", OracleDbType.Int32, ParameterDirection.Input, order.ProductId);
            parameters.Add("p_unit_price", OracleDbType.Int32, ParameterDirection.Input, order.UnitPrice);
            parameters.Add("p_quantity", OracleDbType.Int32, ParameterDirection.Input, order.Quantity);
            parameters.Add("p_shipment_id", OracleDbType.Int32, ParameterDirection.Input, order.ShipmentId);
        
            parameters.Add("p_rows_inserted", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_ORDERITEMS.AddOrderItem",
                 param: parameters,
                 commandType: CommandType.StoredProcedure
            );

            // Obtener el número de filas insertadas
            return parameters.Get<int>("p_rows_inserted");
        }

        public async Task Update(OrderItem order)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();

            parameters.Add("p_order_id", OracleDbType.Int32, ParameterDirection.Input, order.OrderId);
            parameters.Add("p_line_item_id", OracleDbType.Int32, ParameterDirection.Input, order.LineItemId);
            parameters.Add("p_product_id", OracleDbType.Int32, ParameterDirection.Input, order.ProductId);
            parameters.Add("p_unit_price", OracleDbType.Int32, ParameterDirection.Input, order.UnitPrice);
            parameters.Add("p_quantity", OracleDbType.Int32, ParameterDirection.Input, order.Quantity);
            parameters.Add("p_shipment_id", OracleDbType.Int32, ParameterDirection.Input, order.ShipmentId);

            parameters.Add("p_rows_updated", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_ORDERITEMS.UpdateOrderItem",
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

        public async Task Delete(int orderId,int lineItemId)
        {
            await using var connection = new OracleConnection(_dbConnection.ConnectionString);
            await connection.OpenAsync();

            var parameters = new OracleDynamicParameters();
            // Parámetro de entrada: ID del cliente a eliminar
            parameters.Add("p_order_id", OracleDbType.Int32, ParameterDirection.Input, orderId);
            parameters.Add("p_line_item_id", OracleDbType.Int32, ParameterDirection.Input, lineItemId);
            // Parámetro de salida: Número de filas eliminadas
            parameters.Add("p_rows_deleted", OracleDbType.Int32, ParameterDirection.Output);

            await connection.ExecuteAsync(
                 sql: "PKG_ORDERITEMS.DeleteOrderItem",  // Asegúrate de utilizar el nombre del procedimiento según corresponda
                 param: parameters,
                 commandType: CommandType.StoredProcedure
            );

            int rowsDeleted = parameters.Get<int>("p_rows_deleted");

            // Verifica que se haya eliminado algún registro. 
            // Si no se eliminó ninguna fila, se lanza una excepción con un mensaje adecuado.
            if (rowsDeleted != 1)
            {
                throw new Exception($"No se eliminó ninguna orderItem con ID {orderId}-{lineItemId}. Filas eliminadas: {rowsDeleted}");
            }
        }
    }
}