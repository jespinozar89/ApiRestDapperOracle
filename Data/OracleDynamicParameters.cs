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