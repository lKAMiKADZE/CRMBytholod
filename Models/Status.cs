using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Settings;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CRMBytholod.Models
{
    public class Status
    {
        public long ID_STATUS { get; set; }
        public string NameStatus { get; set; }
        public string ColorHex { get; set; }


        public string GetNameStatus
        {
            get
            {
                if (String.IsNullOrEmpty(NameStatus))
                    return "";

                string _NameStatus = NameStatus.ToLower();
                return _NameStatus.Substring(0, 1).ToUpper() + (_NameStatus.Length > 1 ? _NameStatus.Substring(1) : "");
            }
        }

        public static string GetStatusName(long ID_STATUS)
        {
            switch (ID_STATUS)
            {
                case 1:
                    return "В ОЖИДАНИИ"; 
                case 2:
                    return "ПОВТОР"; 
                case 3:
                    return "ОТКАЗ"; 
                case 4:
                    return "В РАБОТЕ"; 
                case 5:
                    return "ВЫПОЛНЕН";
                default :
                    return ""; 
            }


        }


        public Status()
        {
            NameStatus = "";
            ColorHex = "";
        }



        ////////////////
        // Методы SQL
        ////////////////
        public SqlParameter[] GetSqlParametersInsert()
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                
                

                //example Null value
                //new SqlParameter(@"CoolWords",SqlDbType.NVarChar) { Value = CoolWords ?? String.Empty},

                //new SqlParameter(@"PROMOCODE_ID", SqlDbType.UniqueIdentifier) { Value = PROMOCODE_ID },

            };
            return parameters;

        }


        // Методы выполнения запросов и получения данных
        private async void ExecuteSql(string sqlText, SqlParameter[] parameters = null)
        {

            using (SqlConnection connection = new SqlConnection(Const.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    // перехват ошибок и выполнение запроса
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception e) { }

                    command.Parameters.Clear();
                }

                connection.Close();
            }
        }
        private async static void ExecuteSqlStatic(string sqlText, SqlParameter[] parameters = null)
        {

            using (SqlConnection connection = new SqlConnection(Const.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    // перехват ошибок и выполнение запроса
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (Exception e) { }

                    command.Parameters.Clear();
                }

                connection.Close();
            }
        }

        private static DataTable ExecuteSqlGetDataTableStatic(string sqlText, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(Const.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        SqlDataReader reader = command.ExecuteReader();
                        dt.Load(reader);
                    }
                    catch (Exception ex)
                    {

                    }

                    command.Parameters.Clear();


                }

                connection.Close();
            }
            return dt;

        }
    }
}
