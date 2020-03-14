using CRMBytholod.Settings;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Models
{
	public class LogMoney
	{
		public long ID_LOG_MONEY { get; set; }
		public int old_All { get; set; }
		public int new_All { get; set; }
		public int old_Firma { get; set; }
		public int new_Firma { get; set; }
		public int old_Detal { get; set; }
		public int new_Detal { get; set; }
		public DateTime Date_add { get; set; }
        public long ID_ZAKAZ { get; set; }
        public int old_Diagnostik { get; set; }
        public int new_Diagnostik { get; set; }
        
        




        public static void Insert(int new_All, int new_Firma, int new_Detal, int new_Diagnostik, long ID_ZAKAZ)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"new_All",SqlDbType.Int) { Value =new_All },
                new SqlParameter(@"new_Firma",SqlDbType.Int) { Value =new_Firma },
                new SqlParameter(@"new_Detal",SqlDbType.Int) { Value =new_Detal },
                new SqlParameter(@"new_Diagnostik",SqlDbType.Int) { Value =new_Diagnostik },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ }
            };

            #region sql

            string sqlText = @$"

-- получаем предыдущие значения, запоминаем и их логируем
declare @old_All int
	,@old_Firma int
	,@old_Detal int 
	,@old_Diagnostik int;


	SELECT @old_All=MoneyAll
		,@old_Firma=MoneyFirm
		,@old_Detal=MoneyDetal
		,@old_Diagnostik=MoneyDiagnostik
	FROM Zakaz
	WHERE ID_ZAKAZ=@ID_ZAKAZ


INSERT INTO [LogMoney] (
	 [old_All]  
	,[new_All]  
	,[old_Firma]
	,[new_Firma]
	,[old_Detal]
	,[new_Detal]
	,[Date_add] 
    ,[ID_ZAKAZ]
    ,[old_Diagnostik]
    ,[new_Diagnostik]
	)
	VALUES
	(
	 @old_All
	,@new_All
	,@old_Firma
	,@new_Firma
	,@old_Detal
	,@new_Detal
	,CURRENT_TIMESTAMP	
    ,@ID_ZAKAZ
    ,@old_Diagnostik
    ,@new_Diagnostik
	)

";

            #endregion

            ExecuteSqlStatic(sqlText, parameters);

        }


        ////////////////
        // Методы SQL
        ////////////////

        // Методы выполнения запросов и получения данных
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
