using CRMBytholod.Settings;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Models
{
    public class User
    {
        public string ID_USER { get; set; }
        public string Name { get; set; }
        public string Passw { get; set; }
        public int ID_TYPE_USER { get; set; }
        public string Login { get; set; }
        public string Phone { get; set; }
        public string Sessionid { get; set; }

        public void Create()
        {

            SqlParameter[] parameters = GetSqlParametersInsert();

            string sqlText = $@" 
INSERT INTO [dbo].[PromoCode]
           ([PROMOCODE_ID]
           ,[Code]
           )
     VALUES
           (
		   @PROMOCODE_ID
		   
		   )
";
            ExecuteSqlStatic(sqlText, parameters);


        }

        public bool Auth( string Login , string Passw)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login },
                new SqlParameter(@"Passw",SqlDbType.NVarChar) { Value =Passw }
            };
            
            #region sql

            string sqlText = $@"
 declare @isAuth int;


SET @isAuth=(
 SELECT count(1)  FROM [dbo].[User]
WHERE 1=1
	AND ID_TYPE_USER=3
	AND Login=@Login
	AND Passw=@Passw
	)

-- если авторизован и одна запись (нет дублей) то получаем айди сессии и входим
if @isAuth > 0  AND @isAuth <=1
BEGIN
	
	UPDATE [dbo].[User] SET Sessionid= NEWID()
	WHERE 1=1
		AND ID_TYPE_USER=3
	    AND Login=@Login
	    AND Passw=@Passw

	SELECT Sessionid, 1 AS Auth FROM [dbo].[User]
	WHERE 1=1
		AND ID_TYPE_USER=3
	    AND Login=@Login
    	AND Passw=@Passw

END   

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText);

            if (dt.Rows.Count == 0)
                return false;

            foreach (DataRow row in dt.Rows)
            {
                Sessionid = (string)row["Sessionid"];
                return true;
            }


            return false;
        }

        // example
        public static List<string> GetFiltersUser(string USER_ID)
        {
            List<string> _Filters = new List<string>();

            // Запрос в БД и получение фильтров пользователя


            string sqlText = $@" SELECT 
	   f.[FILTER_ID]
      ,f.[Name] AS NameFiltr
       ";


            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText);

            foreach (DataRow row in dt.Rows)
            {
               

                //Filter fil = new Filter()
                //{
                //    FILTER_ID = (Guid)row["FILTER_ID"]                    
                //};

                //_Filters.Add(fil);
            }

            return _Filters;
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

                    SqlDataReader reader = command.ExecuteReader();
                    dt.Load(reader);

                    command.Parameters.Clear();


                }

                connection.Close();
            }
            return dt;

        }
    }
}
