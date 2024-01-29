using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Infrastructure
{
    using System.Data.Common;
    using System.Data.SqlClient;

    public class Database : IDisposable
    {
        private readonly SqlConnection _connection;

        public SqlConnection Connection { get { return _connection; } }

        public Database()
        {
            var connectionString = "Data Source=LOCALHOST;Initial Catalog=BrainWare;Integrated Security=SSPI";
            var mdf = @"C:\GksTest\BrainWare-master\BrainWare-master\Web\App_Data\BrainWare.mdf";

            _connection = new SqlConnection(connectionString);

            _connection.Open();
        }

        public DbDataReader ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters)
        {
            var command = new SqlCommand(storedProcedureName, _connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            return command.ExecuteReader();
        }

        public int ExecuteNonQuery(string query)
        {
            var sqlQuery = new SqlCommand(query, _connection);

            return sqlQuery.ExecuteNonQuery();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
        }
    }


}