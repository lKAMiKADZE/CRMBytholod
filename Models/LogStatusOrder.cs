using CRMBytholod.Settings;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Models
{
    public class LogStatusOrder
    {
        public long ID_LOG { get; set; }
        public Order ORDER { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public DateTime DateChange { get; set; }



        ///////////////////////////////////////
        //dop param



        ///////////////////////////////////////



        public static List<LogStatusOrder> GetHistoryStatusOrder(long ID_ZAKAZ)
        {
            List<LogStatusOrder> LogOrders = new List<LogStatusOrder>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ }
            };


            #region sql

            string sqlText = @$"


-- история изменения статуса заявки
SELECT
	logOrder.ID_ZAKAZ
	,logOrder.ID_LOG
	,logOrder.DateChange
	,logOrder.STATUS
	,u.Name	

  FROM [dbo].[Zakaz] o
  JOIN LogStatusOrder logOrder ON logOrder.ID_ZAKAZ=o.ID_ZAKAZ
  JOIN [User] u ON u.ID_USER=logOrder.ID_USER  
  WHERE 1=1
	AND o.ID_ZAKAZ=@ID_ZAKAZ

ORDER BY logOrder.DateChange DESC
";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
               
                User user = new User
                {
                    Name = (string)row["Name"]
                };

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    
                };

                LogStatusOrder logStatusOrder = new LogStatusOrder 
                {
                    ORDER=order,
                    User= user,
                    ID_LOG=(long)row["ID_LOG"],
                    Status= (string)row["Status"],
                    DateChange= (DateTime)row["DateChange"]
                };

                LogOrders.Add(logStatusOrder);
            }


            return LogOrders;
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
