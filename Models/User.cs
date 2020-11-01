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
    public class User
    {
        public long ID_USER { get; set; }
        public string Name { get; set; }
        public string Passw { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        public string PasswMaster { get; set; }        
        public TypeUser TYPE_USER { get; set; }

        [Required(ErrorMessage = "Не указан Логин")]
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

        public bool Admin { get; set; }
        public bool Deleted { get; set; }
        public string msisdnMaster { get; set; }

        //статистика мастера

        public int MoneyMasterNal    { get; set; }
        public int MoneyMasterALL    { get; set; }
        public int MoneyGetFromFirm { get; set; }
        public int MoneyDetal       { get; set; }
        public int MoneyPutFirma     { get; set; }
        public int TotalAllZakaz     { get; set; }
        public int TotalComplete     { get; set; }
        public int TotalDiagnostik    { get; set; }
        public int TotalDeny           { get; set; }
        public int TotalPovtorNotRab { get; set; }
        public int TotalPovtorRab     { get; set; }

////Свойства

public string GetAdmin
        {
            get
            {
                if (Admin)
                    return "ДА";

                return "НЕТ";
            }
        }


        ///////////////////////////////////////
        //Методы для мобилы АПИ
        public bool AuthMaster( string Login , string Passw)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login.ToLower() },
                new SqlParameter(@"Passw",SqlDbType.NVarChar) { Value =Passw }
            };
            
            #region sql

            string sqlText = $@"
 declare @isAuth int;


SET @isAuth=(
 SELECT count(1)  FROM [dbo].[User]
WHERE 1=1
	AND ID_TYPE_USER=3
	AND LOWER(Login)=LOWER(@Login)
	AND PasswMaster=@Passw
	)

-- если авторизован и одна запись (нет дублей) то получаем айди сессии и входим
if @isAuth > 0  AND @isAuth <=1
BEGIN
	
	UPDATE [dbo].[User] SET Sessionid= NEWID()
	WHERE 1=1
		AND ID_TYPE_USER=3
	    AND LOWER(Login)=LOWER(@Login)
	    AND PasswMaster=@Passw
        AND Deleted=0

	SELECT Sessionid, 1 AS Auth FROM [dbo].[User]
	WHERE 1=1
		AND ID_TYPE_USER=3
	    AND LOWER(Login)=LOWER(@Login)
    	AND PasswMaster=@Passw

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
        public static User GetMasterInfo(string Sessionid)
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

,( SELECT isnull(sum(z.MoneyMaster),0) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid --
		AND z.DATA >= CURRENT_TIMESTAMP-30		
		
) AS MoneyMaster

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid --
		AND z.DATA >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=1
		
) AS Await

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DATA >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=2
) AS Povtor

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DATA >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=3
) AS Denied

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DATA >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=4
) AS InWork

,( SELECT count(1) FROM [dbo].[User] u
	JOIN [dbo].[Zakaz] z ON z.ID_MASTER=u.ID_USER
	WHERE 1=1
		AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
		AND z.DATA >= CURRENT_TIMESTAMP-30
		AND z.ID_STATUS=5
) AS Succes

 FROM [dbo].[User] u 
WHERE 1=1	
	AND u.Sessionid=@Sessionid --'CB80665A-93E0-4E91-A982-36063D546CE6' --@Sessionid
	
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

        public bool AuthUser(string Login, string Passw)
        {


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
	AND PasswMaster= @Passw
    AND Deleted=0

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                // попали в цикл, значит авторизовались, т.к. такой пользователь существует

                this.ID_USER = (long)row["ID_USER"];
                this.Login = (string)row["Login"];

                if ((long)row["ID_TYPE_USER"] == 1)
                    Admin = true;
                

                return true;
            }


            return false;
        }

        public string AuthUserTEST(string Login, string Passw)
        {


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
	AND PasswMaster= @Passw

";

            #endregion

            // получаем данные из запроса
            string testreturn = ExecuteSqlGetDataTableStaticTEST(sqlText, parameters);


            


            return testreturn;
        }

        /// <summary>
        /// Только для админа
        /// </summary>        
        public static List<User> GetAllUsers(long ID_USER)
        {
            List<User> users= new List<User>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"ID_USER",SqlDbType.BigInt) { Value =ID_USER }
            };

            #region sql

            string sqlText = $@"
declare @admin bit;

set @admin=(
SELECT count(1)
 FROM [User]
WHERE ID_USER=@ID_USER
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
	DateAdd,
    msisdnMaster
 FROM [User] u
 JOIN [TypeUser] tu ON u.ID_TYPE_USER=tu.ID_TYPE_USER
WHERE 1=1
    AND Deleted=0
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

                bool tmpAdmin = false;
                if (tu.ID_TYPE_USER == 1)
                    tmpAdmin = true;


                // попали в цикл, значит авторизовались, т.к. такой пользователь существует
                User us = new User
                {
                    ID_USER = (long)row["ID_USER"],
                    Name = (string)row["Name"],
                    Login = (string)row["Login"],
                    PasswMaster = (string)row["PasswMaster"],
                    TYPE_USER = tu,
                    DateAdd = (DateTime)row["DateAdd"],
                    Phone = (string)row["Phone"],
                    Admin = tmpAdmin,
                    msisdnMaster= (string)row["msisdnMaster"]

                };

                users.Add(us);
            }


            return users;
        }
        public static User GetUser(long ID_USER)
        {


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"ID_USER",SqlDbType.BigInt) { Value =ID_USER }
            };

            #region sql

            string sqlText = $@"

SELECT [ID_USER]
      ,[Name]
      ,[Passw]
      ,tu.[ID_TYPE_USER]
	  ,tu.[NameTU]
      ,[Login]
      ,[Phone]
      ,[Sessionid]
      ,[PasswMaster]
      ,[DateAdd]
      ,[Deleted]
      ,msisdnMaster
  FROM [dbo].[User] u
  JOIN [TypeUser] tu ON tu.ID_TYPE_USER=u.ID_TYPE_USER
  WHERE ID_USER=@ID_USER
  AND deleted=0


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                TypeUser tu = new TypeUser
                {
                    ID_TYPE_USER = (long)row["ID_TYPE_USER"],
                    NameTU = (string)row["NameTU"]
                };

                bool tmpAdmin = false;
                if (tu.ID_TYPE_USER == 1)
                    tmpAdmin = true;


                // попали в цикл, значит авторизовались, т.к. такой пользователь существует
                User us = new User
                {
                    ID_USER = (long)row["ID_USER"],
                    Name = (string)row["Name"],
                    Login = (string)row["Login"],
                    PasswMaster = (string)row["PasswMaster"],
                    TYPE_USER = tu,
                    DateAdd = (DateTime)row["DateAdd"],
                    Phone = (string)row["Phone"],
                    Admin = tmpAdmin,
                    msisdnMaster= (string)row["msisdnMaster"]

                };
                return us;
            }


            return null;
        }


        /// <summary>
        /// Mobile
        /// </summary>        
        public static User GetMasterStat(string Sessionid, DateTime DateStart, DateTime DateEnd)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"sessionid",SqlDbType.NVarChar) { Value =Sessionid },
                new SqlParameter(@"start",SqlDbType.DateTime) { Value =DateStart },
                new SqlParameter(@"end",SqlDbType.DateTime) { Value =DateEnd.AddSeconds(1) }
            };

            #region sql

            string sqlText = $@"


SELECT 
m.Name -- a
,z.ID_MASTER

,(SELECT ISNULL(SUM(z1.MoneyMaster),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DATA between @start AND @end
	AND z1.MoneyFirm > 0
	AND z1.OplataNal=1
	) AS MoneyMasterNal 

,(SELECT ISNULL(SUM(z1.MoneyMaster),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DATA between @start AND @end
	AND z1.MoneyFirm > 0
	) AS MoneyMasterALL

	
,(SELECT
 ISNULL((SUM(z1.MoneyMaster) + SUM(z1.MoneyDetal)),0) 
	FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DATA between @start AND @end
	AND z1.MoneyFirm > 0
	AND z1.OplataNal=0
	) AS MoneyGetFromFirm

,(SELECT ISNULL(SUM(z1.MoneyDetal),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DATA between @start AND @end
	AND z1.MoneyFirm > 0
	) AS MoneyDetal 
	
,(SELECT ISNULL(SUM(z1.MoneyFirm),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DATA between @start AND @end
	AND z1.MoneyFirm > 0
	) AS MoneyPutFirma 
	
--------
-- TOTAL
--------
--a.	Сколько было всего заказов ед
,(SELECT count(1) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (3,5,7)
	AND z1.DATA between @start AND @end
	) AS TotalAllZakaz

--b.	Выполнено с деньгами  ед
,(SELECT count(1) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DATA between @start AND @end
	AND z1.MoneyFirm>0
	) AS TotalComplete
	
--c.	Диагностик ед
,(SELECT count(1) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (7)
	AND z1.DATA between @start AND @end
	AND z1.MoneyFirm>0
	) AS TotalDiagnostik

--d.	Отказов ед
,(SELECT count(1) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (3)
	AND z1.DATA between @start AND @end	
	) AS TotalDeny
	
--e.	Повторов без денег ед
,(SELECT count(1) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (3,5,7)
	AND z1.DATA between @start AND @end	
	AND z1.Povtor=1
	AND z1.MoneyFirm=0
	) AS TotalPovtorNotRab

--f.	Повторов рабочих с деньгами (в фирму) ед
,(SELECT count(1) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DATA between @start AND @end	
	AND z1.Povtor=1
	AND z1.MoneyFirm>0
	) AS TotalPovtorRab

FROM [Zakaz] z
JOIN [User] m ON m.ID_USER=z.ID_MASTER
WHERE 1=1
	AND m.Sessionid=@sessionid	

	AND z.ID_STATUS in (3,5,7)
	
	AND z.DATA between @start AND @end 

GROUP BY m.Name, z.ID_MASTER
ORDER BY m.Name ASC







";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                // попали в цикл, значит авторизовались, т.к. такой пользователь существует
                User us = new User
                {
                    ID_USER = (long)row["ID_MASTER"],
                    Name = (string)row["Name"],
                    MoneyMasterNal = (int)row["MoneyMasterNal"],
                    MoneyMasterALL = (int)row["MoneyMasterALL"],
                    MoneyGetFromFirm = (int)row["MoneyGetFromFirm"],
                    MoneyDetal = (int)row["MoneyDetal"],
                    MoneyPutFirma = (int)row["MoneyPutFirma"],
                    TotalAllZakaz = (int)row["TotalAllZakaz"],
                    TotalComplete = (int)row["TotalComplete"],
                    TotalDiagnostik = (int)row["TotalDiagnostik"],
                    TotalDeny = (int)row["TotalDeny"],
                    TotalPovtorNotRab = (int)row["TotalPovtorNotRab"],
                    TotalPovtorRab = (int)row["TotalPovtorRab"]

                };
                return us;
            }


            return new User();
        }

        /// <summary>
        /// сайт
        /// </summary>
        /// <returns></returns>
        public static List<User> GetAllMasters()
        {
            List<User> users = new List<User>();


            SqlParameter[] parameters = new SqlParameter[]
            {
            };

            #region sql

            string sqlText = $@"
 SELECT 
  u.Name,
  u.ID_USER
   FROM [User] u
  JOIN [TypeUser] tu ON tu.ID_TYPE_USER=u.ID_TYPE_USER
  WHERE tu.ID_TYPE_USER=3
    AND Deleted=0
  ORDER BY name ASC

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText);


            foreach (DataRow row in dt.Rows)
            {

                // попали в цикл, значит авторизовались, т.к. такой пользователь существует
                User us = new User
                {
                    ID_USER = (long)row["ID_USER"],
                    Name = (string)row["Name"]
                };

                users.Add(us);
            }


            return users;
        }

        public void Update()
        {


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"ID_USER",SqlDbType.BigInt) { Value =ID_USER },
                new SqlParameter(@"Name",SqlDbType.NVarChar) { Value =Name ?? "" },
                new SqlParameter(@"Passw",SqlDbType.NVarChar) { Value =Passw ?? "" },
                new SqlParameter(@"ID_TYPE_USER",SqlDbType.BigInt) { Value =TYPE_USER.ID_TYPE_USER },
                new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login ?? "" },
                new SqlParameter(@"Phone",SqlDbType.NVarChar) { Value =Phone  ?? ""},
                new SqlParameter(@"PasswMaster",SqlDbType.NVarChar) { Value =PasswMaster ?? "" },
                new SqlParameter(@"msisdnMaster",SqlDbType.NVarChar) { Value =ConvertMsisdn(msisdnMaster) ?? "" }                
            };

            #region sql

            string sqlText = $@"
UPDATE [dbo].[User]
   SET [Name] =			@Name--<Name, nvarchar(50),>
      ,[Passw] =		@Passw	--<Passw, nvarchar(150),>
      ,[ID_TYPE_USER] = @ID_TYPE_USER --<ID_TYPE_USER, bigint,>
      ,[Login] =		@Login	-- <Login, nvarchar(20),>
      ,[Phone] =		@Phone--<Phone, nvarchar(15),>
      ,[PasswMaster] =	@PasswMaster	-- <PasswMaster, nvarchar(50),>
      ,[msisdnMaster]= @msisdnMaster
      
 WHERE ID_USER=@ID_USER



";

            #endregion

            // получаем данные из запроса
            ExecuteSqlStatic(sqlText, parameters);




        }

        public static void Delete(long ID_USER)
        {


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"ID_USER",SqlDbType.BigInt) { Value =ID_USER },
                new SqlParameter(@"Deleted",SqlDbType.Bit) { Value =1 }
            };

            #region sql

            string sqlText = $@"
UPDATE [dbo].[User]
   SET [Deleted] =	@Deleted    
 WHERE ID_USER=@ID_USER



";

            #endregion

            // получаем данные из запроса
            ExecuteSqlStatic(sqlText, parameters);




        }

        public void Save()
        {


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Name",SqlDbType.NVarChar) { Value =Name ?? ""},
                new SqlParameter(@"Passw",SqlDbType.NVarChar) { Value =Passw ?? ""},
                new SqlParameter(@"ID_TYPE_USER",SqlDbType.BigInt) { Value =TYPE_USER.ID_TYPE_USER },
                new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login },
                new SqlParameter(@"Phone",SqlDbType.NVarChar) { Value =Phone ?? "" },
                new SqlParameter(@"PasswMaster",SqlDbType.NVarChar) { Value =PasswMaster },
                new SqlParameter(@"msisdnMaster",SqlDbType.NVarChar) { Value =ConvertMsisdn(msisdnMaster) }
                
            };

            #region sql

            string sqlText = $@"
INSERT INTO [dbo].[User]
           ([Name]
           ,[Passw]
           ,[ID_TYPE_USER]
           ,[Login]
           ,[Phone]
           ,[Sessionid]
           ,[PasswMaster]
           ,[DateAdd]
           ,[Deleted]
           ,[msisdnMaster]
)
     VALUES
           (
		    @Name  --<Name, nvarchar(50),>
           ,@Passw  --<Passw, nvarchar(150),>
           ,@ID_TYPE_USER  --<ID_TYPE_USER, bigint,>
           ,@Login  --<Login, nvarchar(20),>
           ,@Phone  --<Phone, nvarchar(15),>
           ,'111'  --<Sessionid, nvarchar(150),>
           ,@PasswMaster  --<PasswMaster, nvarchar(50),>
           ,CURRENT_TIMESTAMP  --<DateAdd, datetime,>
           ,0  --<Deleted, bit,>
           ,@msisdnMaster
		   )



";

            #endregion

            // получаем данные из запроса
            ExecuteSqlStatic(sqlText, parameters);




        }


        private string ConvertMsisdn(string msisdn)
        {
            if (String.IsNullOrEmpty(msisdn))
                return "";

            msisdn = msisdn.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            return msisdn;
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
                    catch (Exception ex)
                    {

                    }

                    command.Parameters.Clear();


                }

                connection.Close();
            }
            return dt;

        }


        private static string ExecuteSqlGetDataTableStaticTEST(string sqlText, SqlParameter[] parameters = null)
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
                        return ex.Message;
                    }

                    command.Parameters.Clear();


                }

                connection.Close();
            }
            return "OK";

        }
    }
}
