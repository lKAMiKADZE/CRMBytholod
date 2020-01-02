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
        public long ID_USER { get; set; }
        public string Name { get; set; }
        public string Passw { get; set; }
        public string PasswMaster { get; set; }        
        public TypeUser TYPE_USER { get; set; }
        public string Login { get; set; }
        public string Phone { get; set; }
        public string Sessionid { get; set; }
        public DateTime DateAdd { get; set; }

        ///////////////////////////////////////
        //dop param

        public int OrderAwait { get; set; }
        public int OrderPovtor { get; set; }
        public int OrderSucces { get; set; }
        public int OrderDenied { get; set; }
        public int OrderInWork { get; set; }
        public int MoneyUpMaster { get; set; }

        ///////////////////////////////////////
        //Методы для мобилы АПИ
        public bool AuthMaster( string Login , string Passw)
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
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);

            if (dt.Rows.Count == 0)
                return false;

            foreach (DataRow row in dt.Rows)
            {
                Sessionid = (string)row["Sessionid"];
                return true;
            }


            return false;
        }
        public static User GetMasterStat(string Sessionid)
        {
            User us;


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid }
            };

            #region sql

            string sqlText = $@"
SELECT 
 u.Name
, sum(z.MoneyMaster) AS MoneyMaster
,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid --
		AND z.DateAdd >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=1
		
) AS Await

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DateAdd >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=2
) AS Povtor

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DateAdd >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=3
) AS Denied

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DateAdd >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=4
) AS InWork

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DateAdd >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=5
) AS Succes

 FROM [dbo].[User] u
JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
WHERE 1=1
	AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
	AND z.DateAdd >= CURRENT_TIMESTAMP-30
	
GROUP BY  u.Name


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                us = new User
                {
                    Name = (string)row["Name"],
                    MoneyUpMaster = (int)row["MoneyMaster"],
                    OrderAwait = (int)row["Await"],
                    OrderPovtor = (int)row["Povtor"],
                    OrderDenied = (int)row["Denied"],
                    OrderInWork = (int)row["InWork"],
                    OrderSucces = (int)row["Succes"]                    
                };

                return us;
            }


            return null;
        }


        //Методы для сайта

        public static bool AuthUser(string Login, string Passw)
        {
            User us;


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login },
                new SqlParameter(@"Passw",SqlDbType.NVarChar) { Value =Passw }
            };

            #region sql

            string sqlText = $@"

SELECT ID_USER,
	Name,
	ID_TYPE_USER,
	Login,
	Phone,
	PasswMaster, -- Пароль мастера для вывода админу
	DateAdd
 FROM [User]
WHERE LOWER(Login)=LOWER(@Login)
	AND Passw= @Passw

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                // попали в цикл, значит авторизовались, т.к. такой пользователь существует
                //us = new User
                //{
                //    Name = (string)row["Name"],
                //};

                return true;
            }


            return false;
        }

        /// <summary>
        /// Только для админа
        /// </summary>        
        public static List<User> GetAllUsers(string Login)
        {
            List<User> users= new List<User>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login }
            };

            #region sql

            string sqlText = $@"
declare @admin bit;

set @admin=(
SELECT count(1)
 FROM [User]
WHERE LOWER(Login)=LOWER(@Login)
	AND ID_TYPE_USER=1
)  

-- вывод всех пользователей мастеров и диспетчеров
if @admin=1
BEGIN

SELECT ID_USER,
	Name,
	tu.ID_TYPE_USER,
	tu.NameTU,
	Login,
	Phone,
	PasswMaster, -- Пароль мастера для вывода админу
	DateAdd
 FROM [User] u
 JOIN [TypeUser] tu ON u.ID_TYPE_USER=tu.ID_TYPE_USER
WHERE 1=1
	AND LOWER(Login)<>LOWER(@Login)
ORDER BY DateAdd DESC

END


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                TypeUser tu = new TypeUser
                {
                    ID_TYPE_USER= (long)row["ID_TYPE_USER"],
                    NameTU = (string)row["NameTU"]
                };

                // попали в цикл, значит авторизовались, т.к. такой пользователь существует
                User us = new User
                {
                    ID_USER=(long)row["ID_USER"],
                    Name = (string)row["Name"],
                    Login = (string)row["Login"],
                    PasswMaster = (string)row["PasswMaster"],
                    TYPE_USER = tu,
                    DateAdd = (DateTime)row["DateAdd"],
                    Phone = (string)row["Phone"]
                };

                users.Add(us);
            }


            return users;
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

                new SqlParameter(@"ID_USER", SqlDbType.NVarChar) { Value =ID_USER  },
                new SqlParameter(@"Name", SqlDbType.NVarChar) { Value =Name  },
                new SqlParameter(@"Passw", SqlDbType.NVarChar) { Value =Passw  },
                new SqlParameter(@"ID_TYPE_USER", SqlDbType.NVarChar) { Value =TYPE_USER.ID_TYPE_USER  },
                new SqlParameter(@"PasswMaster", SqlDbType.NVarChar) { Value =PasswMaster  },

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
