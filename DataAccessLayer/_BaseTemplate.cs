using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLayer
{
    public class _BaseTemplate: _Base
    {
        public _BaseTemplate(string GlobalConnectionString)
        {
            this.GlobalConnectionString = GlobalConnectionString;
        }

        /// <summary>
        /// Executes a series of FoxPro commands in one run. You can use this to re-index the table or add/edit/delete data
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="ListOfFoxProCommands"></param>
        public void ExecFoxProCommands(List<String> ListOfFoxProCommands, String filePath)
        {
            /*Sample codes for you to use*/
            //List<String> ListOfFoxProCommands = new List<string>();
            //ListOfFoxProCommands.Add("use emp exclusive");
            //ListOfFoxProCommands.Add("index on pcode tag emp ");
            /*END*/

            //string vfpScript = "";
            //foreach (var item in ListOfFoxProCommands)
            //{
            //    vfpScript += String.Format("{0}\n", item);
            //}

            ////@"
            ////create table abc (text1 char(10), text2 char(10))
            ////
            ////index on upper(text1+text2) tag dummy";
            //string strCon = String.Format(@"Provider=vfpoledb;Data Source={0}", filePath);
            //OleDbConnection conn = new OleDbConnection(strCon);
            //OleDbCommand cmd = conn.CreateCommand();
            //conn.Open();
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandText = "ExecScript";
            //cmd.Parameters.AddWithValue("code", vfpScript);
            //cmd.ExecuteNonQuery();
            //conn.Close();

            string vfpScript = "";
            foreach (var item in ListOfFoxProCommands)
            {
                vfpScript += String.Format("{0}\n", item);
            }
            this.GlobalConnectionString = String.Format(@"Provider=vfpoledb;Data Source={0}", filePath);
            InitializeDataAccess("ExecScript", ProviderType.Oledb);
            CreateCommandParameters("code", vfpScript);
            //SaveChanges("ExecScript", CommandType.StoredProcedure);
            SaveChanges(CommandType.StoredProcedure);
        }

        /// <summary>
        /// Automates Stored Procedure. If the Stored Procedure has parameters, use this one. This has no return value
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="providerType"></param>
        /// <param name="parameterList"></param>
        public void AutomateStoredProcedure<T>(string procedureName, ProviderType providerType, Dictionary<String, T> parameterList)
        {
            InitializeDataAccess(procedureName, providerType);
            foreach (KeyValuePair<String, T> item in parameterList)
            {
                CreateCommandParameters(item.Key, item.Value);
            }
            SaveChanges(CommandType.StoredProcedure);
        }

        /// <summary>
        /// Automates Stored Procedure. If the Stored Procedure has parameters, use this one. Returns a value explicitly.
        /// </summary>
        /// <param name="procedureName">name of the Stored Procedure</param>
        /// <param name="providerType">Provider type you used in your database</param>
        public T AutomateStoredProcedure<T>(string procedureName, ProviderType providerType)
        {
            InitializeDataAccess(procedureName, ProviderType.SqlClient);
            var proc_retValue = CreateCommandParametersExplicit("RetVal", typeCastDbType(typeof(T)), ParameterDirection.ReturnValue);
            SaveChanges(CommandType.StoredProcedure);
            return (T)proc_retValue.Value;
        }

        /// <summary>
        /// Automates Stored Procedure. If the Stored Procedure has parameters, use this one. The return value can be casted explicitly.
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="providerType"></param>
        /// <param name="parameterList"></param>
        public T AutomateStoredProcedure<T>(string procedureName, ProviderType providerType, Dictionary<string, object> parameterList)
        {
            InitializeDataAccess(procedureName, ProviderType.SqlClient);
            foreach (KeyValuePair<string, object> item in parameterList)
            {
                CreateCommandParameters(item.Key, item.Value); //ex. "**Column name**, **Row value**"
            }
            var proc_retValue = CreateCommandParametersExplicit("RetVal", typeCastDbType(typeof(T)), ParameterDirection.ReturnValue);

            SaveChanges(CommandType.StoredProcedure);
            return (T)proc_retValue.Value;
        }

        /// <summary>
        /// for use of casting in AutomateStoredProcedure
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        private DbType typeCastDbType(Type T)
        {
            DbType type = DbType.Int32;

            if (T == typeof(int))
                type = DbType.Int32;
            else if (T == typeof(String))
                type = DbType.String;

            return type;
        }        
    }
}
