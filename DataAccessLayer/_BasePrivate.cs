using MySql.Data.MySqlClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    /// <summary>
    /// put private methods here
    /// </summary>
    public partial class _Base
    {
        /// <summary>
        /// initialization casting for InitializeDataAccess()
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="Query"></param>
        private void castProvider(ProviderType type, string ConnectionString, string Query = null)
        {
            switch (type)
            {
                case ProviderType.Oledb:
                    conn = new OleDbConnection(ConnectionString);
                    cmd = new OleDbCommand(Query, (OleDbConnection)conn);
                    da = new OleDbDataAdapter();
                    break;
                case ProviderType.Odbc:
                    conn = new OdbcConnection(ConnectionString);
                    cmd = new OdbcCommand(Query, (OdbcConnection)conn);
                    da = new OdbcDataAdapter();
                    break;
                case ProviderType.SqlClient:
                    conn = new SqlConnection(ConnectionString);
                    cmd = new SqlCommand(Query, (SqlConnection)conn);
                    da = new SqlDataAdapter();
                    break;
                //case ProviderType.OracleClient:
                //    conn = new OracleConnection(ConnectionString);
                //    cmd = new OracleCommand(Query,(OracleConnection)conn);
                //    break;
                case ProviderType.MySql:
                    conn = new MySqlConnection(ConnectionString);
                    cmd = new MySqlCommand(Query, (MySqlConnection)conn);
                    da = new MySqlDataAdapter();
                    break;
            }
        }

        /// <summary>
        /// counts for number of rows.
        /// </summary>
        /// <param name="rowCount"></param>
        private void CheckForRows(int rowCount)
        {
            if (rowCount > 0)
                HasRows = true;
            else HasRows = false;
        }

        public _Base()
        {
        }

        public _Base(ProviderType type, string connectionString)
        {
            InitializeDataAccess(type, connectionString);
        }
    }
}
