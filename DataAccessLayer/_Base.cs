using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccessLayer
{
    public enum ProviderType
    {
        Oledb,
        Odbc,
        SqlClient,
        MySql
    }

    public partial class _Base
    {
        private IDbConnection conn { get; set; }
        private IDbCommand cmd { get; set; }
        private DataSet ds { get; set; }
        private IDbDataAdapter da { get; set; }
        private IDataReader dr { get; set; }

        /// <summary>
        /// use this to check if your SQL query has rows.
        /// </summary>
        public bool HasRows { get; private set; }

        /// <summary>
        /// Provides the Global ConnectionString across functions for a centralized connection.
        /// </summary>
        public string GlobalConnectionString { get; set; }

        /// <summary>
        /// passes the chosen type during initialization so that a developer doesn't need to specify a provider type again in each methods.
        /// </summary>
        private ProviderType chosenType { get; set; }

        /// <summary>
        /// use this if you intend to create a parameter-based command for query use. This method is similar to cmd.Parameters.AddWithValue()
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void CreateCommandParameters(string parameterName, object value)
        {
            switch (chosenType)
            {
                case ProviderType.Oledb:
                    cmd.Parameters.Add(new OleDbParameter(string.Format("@{0}", parameterName), value));
                    break;
                case ProviderType.Odbc:
                    cmd.Parameters.Add(new OdbcParameter(string.Format("@{0}", parameterName), value));
                    break;
                case ProviderType.SqlClient:
                    cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", parameterName), value));
                    break;
                case ProviderType.MySql:
                    cmd.Parameters.Add(new MySqlParameter(string.Format("@{0}", parameterName), value));
                    break;
            }
        }

        public string GetCmdParametersForInsertInto()
        {
            string generatedParameters = null;
            var isInitialized = false;
            foreach (IDbDataParameter param in cmd.Parameters)
            {
                if (isInitialized == false)
                {
                    generatedParameters += param.ParameterName.Cast<string>();
                    isInitialized = true;
                }
                else
                {
                    generatedParameters += string.Format(",{0}", param.ParameterName);
                }
            }
            return generatedParameters;
        }

        public string GetCmdParametersForUpdate(List<string> updateColumnNames)
        {
            string generatedParameters = null;
            var isInitialized = false;
            int loopNumber = 0;
            if (updateColumnNames.Count != cmd.Parameters.Count)
            {
                throw new Exception("Your parameters and column name count were not exact. Please make sure that they're equal and try again.");
            }
            foreach (var item in updateColumnNames)
            {
                if (isInitialized == false)
                {
                    generatedParameters += string.Format("{0}={1}", item, cmd.Parameters[loopNumber]);
                    isInitialized = true;
                }
                else
                    generatedParameters += string.Format(",{0}={1}", item, cmd.Parameters[loopNumber]);
            }
            return generatedParameters;
        }

        /// <summary>
        /// Creating command parameters explicitly, allowing you to specify its parameterdirection and fieldsize if you are using stored procedures..
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="parameterDirection"></param>
        /// <param name="fieldSize"></param>
        protected void CreateCommandParametersExplicit(string parameterName, object value, System.Data.ParameterDirection parameterDirection, int fieldSize)
        {
            IDbDataParameter parameterInputOutput = null;
            switch (chosenType)
            {
                case ProviderType.Oledb:
                    parameterInputOutput = new OleDbParameter
                    {
                        ParameterName = string.Format("@{0}", parameterName),
                        Direction = parameterDirection,
                        Size = fieldSize
                    };
                    //cmd.Parameters.Add(parameterInputOutput);
                    break;
                case ProviderType.Odbc:
                    parameterInputOutput = new OdbcParameter
                    {
                        ParameterName = string.Format("@{0}", parameterName),
                        Direction = parameterDirection,
                        Size = fieldSize
                    };
                    //cmd.Parameters.Add(parameterInputOutput);
                    break;
                case ProviderType.SqlClient:
                    parameterInputOutput = new SqlParameter
                    {
                        ParameterName = string.Format("@{0}", parameterName),
                        //SqlDbType = dbType,
                        Direction = parameterDirection,
                        Size = fieldSize
                    };
                    //cmd.Parameters.Add(parameterInputOutput);
                    break;
                case ProviderType.MySql:
                    parameterInputOutput = new MySqlParameter
                    {
                        ParameterName = string.Format("@{0}", parameterName),
                        //SqlDbType = dbType,
                        Direction = parameterDirection,
                        Size = fieldSize
                    };
                    //cmd.Parameters.Add(parameterInputOutput);
                    break;
            }
            cmd.Parameters.Add(parameterInputOutput);
        }

        /// <summary>
        /// Use this if you want to get the return value from Stored Procedures..
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="parameterDirection"></param>
        /// <returns></returns>
        protected IDbDataParameter CreateCommandParametersExplicit(string parameterName, DbType dbType, System.Data.ParameterDirection parameterDirection)
        {
            IDbDataParameter outPutParameter = null; //= new SqlParameter();
            object paramValue = 0;
            switch (chosenType)
            {
                case ProviderType.Oledb:
                    //cmd.Parameters.Add(new OleDbParameter(string.Format("@{0}", parameterName)));
                    break;
                case ProviderType.Odbc:
                    //cmd.Parameters.Add(new OdbcParameter(string.Format("@{0}", parameterName)));
                    break;
                case ProviderType.SqlClient:
                    outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = string.Format("@{0}", parameterName);
                    ((SqlParameter)outPutParameter).DbType = dbType; //System.Data.SqlDbType.Int;
                    outPutParameter.Direction = parameterDirection; //System.Data.ParameterDirection.Input;
                    outPutParameter.Size = 10;
                    cmd.Parameters.Add(outPutParameter);
                    //paramValue = ((SqlParameter)cmd.Parameters[string.Format("@{0}", parameterName)]).Value;
                    //cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", parameterName), value));
                    break;
                case ProviderType.MySql:
                    outPutParameter = new MySqlParameter();
                    outPutParameter.ParameterName = string.Format("@{0}", parameterName);
                    ((MySqlParameter)outPutParameter).DbType = dbType; //System.Data.SqlDbType.Int;
                    outPutParameter.Direction = parameterDirection; //System.Data.ParameterDirection.Input;
                    outPutParameter.Size = 10;
                    cmd.Parameters.Add(outPutParameter);
                    //paramValue = ((SqlParameter)cmd.Parameters[string.Format("@{0}", parameterName)]).Value;
                    //cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", parameterName), value));
                    break;
            }
            return outPutParameter;
        }

        /// <summary>
        /// Initializes Data Access before you can set perform CRUD Operations
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="Query"></param>
        public void InitializeDataAccess(ProviderType type, string ConnectionString, string Query)
        {
            castProvider(type, ConnectionString, Query);
            chosenType = type;
            ds = new DataSet();
        }

        /// <summary>
        /// Initializes Data Access before you can set perform CRUD Operations
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ConnectionString"></param>
        public void InitializeDataAccess(ProviderType type, string ConnectionString)
        {
            castProvider(type, ConnectionString);
            chosenType = type;
            ds = new DataSet();
        }

        /// <summary>
        /// Initializes Data Access before you can set perform CRUD Operations. Uses GlobalConnectionString variable as the basis for the ConnectionString so that you don't have to set it repeatedly across functions.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="Query"></param>
        public void InitializeDataAccess(string Query, ProviderType type)
        {
            castProvider(type, GlobalConnectionString, Query);
            chosenType = type;
            ds = new DataSet();
        }

        /// <summary>
        /// Initializes Data Access before you can set perform CRUD Operations. Uses GlobalConnectionString variable as the basis for the ConnectionString so that you don't have to set it repeatedly across functions.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ConnectionString"></param>
        public void InitializeDataAccess(ProviderType type)
        {
            castProvider(type, GlobalConnectionString);
            chosenType = type;
            ds = new DataSet();
        }


        /// <summary>
        /// In case you didn't provide a SQL query in InitializeDataAccess() method.
        /// </summary>
        /// <param name="Query"></param>
        public int SaveChanges(string Query, CommandType cmdType = CommandType.Text)
        {
            cmd.CommandText = Query;
            cmd.CommandType = cmdType;
            using (conn)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// provides a straightforward query execution IF you provided a SQL query during InitializeDataAccess() method. Returns number of rows affected.
        /// </summary>
        /// <returns>int</returns>
        public int SaveChanges(CommandType cmdType = CommandType.Text)
        {
            cmd.CommandType = cmdType;
            using (conn)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// provides a straightforward query execution. You need to provide an SQL query for this. Returns number of rows affected.
        /// </summary>
        /// <returns>int</returns>
        public int SaveChanges(string Query)
        {
            cmd.CommandText = Query;
            using (conn)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// provides a straightforward query execution. You need to provide an SQL query and CommandType(optional if not Stored Procedure) for this. Returns number of rows affected.
        /// </summary>
        /// <returns>int</returns>
        public int SaveChanges()
        {
            using (conn)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// Returns first row of the first column value within the resultset. Other values w/in the table are ignored. This is only useful if you are using a SQL functions that returns single column like SUM(), COUNT(), MIN(), MAX()
        /// </summary>
        /// <returns></returns>
        public object GetScalarValue(CommandType cmdType = CommandType.Text)
        {
            cmd.CommandType = cmdType;
            using (conn)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// Returns first row of the first column value within the resultset. Other values w/in the table are ignored. This is only useful if you are using a SQL functions or aggregate functions that returns single column like SUM(), COUNT(), MIN(), MAX()
        /// </summary>
        /// <returns></returns>
        public object GetScalarValue(string ScalarQuery, CommandType cmdType = CommandType.Text)
        {
            cmd.CommandText = ScalarQuery;
            cmd.CommandType = cmdType;
            using (conn)
            {
                try
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
        }

        /// <summary>
        /// for getting aggregate functions
        /// </summary>
        /// <typeparam name="T">explicit casting</typeparam>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        protected T GetScalarValue<T>(CommandType cmdType)
        {
            cmd.CommandType = cmdType;
            T myValue;
            try
            {
                using (conn)
                {
                    conn.Open();
                    myValue = (T)cmd.ExecuteScalar();
                }
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            return myValue;
        }

        /// <summary>
        /// support for datareader stored into collections(List<of T>)
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public List<object> getDataReader(string parameterName)
        {
            var list = new List<object>();
            using (conn)
            {
                try
                {
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        list.Add(dr[parameterName]);
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
            return list;
        }

        /// <summary>
        /// support for datareader stored into collections(List<of T>). Requires query if you didn't provided a query during initialization.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public List<object> getDataReader(string parameterName, string query)
        {
            var list = new List<object>();
            cmd.CommandText = query;
            using (conn)
            {
                try
                {
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.Read() == true)
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[parameterName]);
                        }
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.Message);
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                    cmd.Dispose();
                }
            }
            return list;
        }


        /// <summary>
        /// returns a table version of DataSet
        /// </summary>
        /// <param name="tableIndex"></param>
        /// <returns>DataTable</returns>
        public DataTable getDataTable(int tableIndex = 0, CommandType cmdType = CommandType.Text)
        {
            cmd.CommandType = cmdType;
            da.SelectCommand = cmd;

            try
            {
                da.Fill(ds);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            return ds.Tables[tableIndex];
        }

        /// <summary>
        /// returns a table version of DataSet
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable getDataTable(string SelectQuery)
        {
            cmd.CommandText = SelectQuery;
            da.SelectCommand = cmd;
            try
            {
                da.Fill(ds);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            CheckForRows(ds.Tables[0].Rows.Count);
            return ds.Tables[0];
        }        

        /// <summary>
        /// returns a table version of DataSet
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable getDataTable(int tableIndex, string SelectQuery, CommandType cmdType = CommandType.Text)
        {
            cmd.CommandText = SelectQuery;
            cmd.CommandType = cmdType;
            da.SelectCommand = cmd;
            try
            {
                da.Fill(ds);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            CheckForRows(ds.Tables[tableIndex].Rows.Count);
            return ds.Tables[tableIndex];
        }

        /// <summary>
        /// Gets a set of Tables involved during the query
        /// </summary>
        /// <param name="SelectQuery"></param>
        /// <returns></returns>
        public DataSet getDataSet(string SelectQuery)
        {
            cmd.CommandText = SelectQuery;
            da.SelectCommand = cmd;
            try
            {
                da.Fill(ds);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            return ds;
        }

        /// <summary>
        /// Gets a set of Tables involved during the query
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet getDataSet()
        {
            da.SelectCommand = cmd;
            try
            {
                da.Fill(ds);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
            return ds;
        }
    }
}
