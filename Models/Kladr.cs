using CRMBytholod.Settings;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Models
{
    public class Kladr
    {
        public long ID_KLADR { get; set; }
        public string NameStreet { get; set; }

        ///////////////////////////////////////
        //dop param


    
        public static List<Kladr> GetLikeStreet(string shortStreet)
        {
            List<Kladr> kladrs= new List<Kladr>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"NameStreet",SqlDbType.BigInt) { Value =shortStreet }
            };

            #region sql

            string sqlText = $@"
SELECT TOP (5) [ID_Kladr]
      ,[NameStreet]
  FROM [Bytholod].[dbo].[Kladr]
  WHERE NameStreet like '%{shortStreet}%'
  ORDER BY [NameStreet] ASC

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText);


            foreach (DataRow row in dt.Rows)
            {               
                Kladr us = new Kladr
                {
                    ID_KLADR = (long)row["ID_KLADR"],
                    NameStreet = (string)row["NameStreet"]
                  
                };

                kladrs.Add(us);
            }


            return kladrs;
        }
    




        ////////////////
        // Методы SQL
        ////////////////
        public SqlParameter[] GetSqlParametersInsert()
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                
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
                    catch(Exception ex)
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
