using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Settings;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using CRMBytholod.ViewModels;

namespace CRMBytholod.Models
{
    public class Order
    {
        public long ID_ZAKAZ { get; set; }
        public string STREET { get; set; }
        public string HOUSE { get; set; }
        public string KORPUS { get; set; }
        public string KVARTIRA { get; set; }
        public string PODEST { get; set; }
        public string ETAG { get; set; }
        public string KOD_DOMOFONA { get; set; }
        public string TELEFON_STACIONAR { get; set; }
        public string TELEFON_SOTOV { get; set; }
        public string HOLODILNIK_DEFECT { get; set; }
        public string MASTER { get; set; }
        public DateTime DATA { get; set; }
        public string VREMJA { get; set; }
        public string DISPETCHER { get; set; }
        public string PRIMECHANIE { get; set; }
        public bool VIPOLNENIE_ZAKAZA { get; set; }
        
        ////////////////////////
        
        public string Msisdn1 { get; set; }
        public string Msisdn2 { get; set; }
        public string Msisdn3 { get; set; }
        public Organization ORGANIZATION { get; set; }
        public User USER_MASTER { get; set; }
        public User USER_ADD { get; set; }        
        public DateTime DateAdd { get; set; }
        public DateTime? DateSendMaster { get; set; }
        public DateTime? DateOpenMaster { get; set; }
        public DateTime? DateClose { get; set; }        
        public Status STATUS { get; set; }
        public int MoneyAll { get; set; }
        public int MoneyFirm { get; set; }
        public int MoneyDetal { get; set; }
        public int MoneyMaster { get; set; }
        public string DescripClose { get; set; }
        public string NameClient { get; set; }
        public bool Povtor { get; set; }

        //dop param
        /////////////////////////////////

        public string DayWeek_Date { get; set; }// хранится название дня недели и дата

        /////////////////////////////////
        //Свойства

        public string GetAdresFull
        {
            get
            {
                if (KORPUS.Length == 0)// если корпус или нет
                    return $"{STREET}, д {HOUSE}, кв {KVARTIRA}";
                else
                    return $"{STREET}, корп {KORPUS}, д {HOUSE}, кв {KVARTIRA}";
            }
        }
        public string GetDATA
        {
            get
            {
                return DATA.ToShortDateString();
            }
        }

        public string GetMoneyAll
        {
            get
            {
                return MoneyAll + " Руб.";
            }
        }
        public string GetMoneyFirm
        {
            get
            {
                return MoneyFirm + " Руб.";
            }
        }
        public string GetMoneyDetal
        {
            get
            {
                return MoneyDetal + " Руб.";
            }
        }
        public string GetMoneyMaster
        {
            get
            {
                return MoneyMaster+ " Руб.";
            }
        }
        public string GetPovtor
        {
            get
            {
                if (Povtor)
                    return "ДА";

                return "НЕТ";
            }
        }



        public Order()
        {
            USER_MASTER = new User();
            USER_ADD = new User();
            STATUS = new Status();
        }


        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static List<Order> GetNewOrders(string Sessionid)
        {
            List<Order> Orders = new List<Order>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid }
            };

            #region sql

            string sqlText = $@"
 -- Если ранее не происходило проставка времени отправки заявки мастеру, то обновляем эту дату сейчас
UPDATE [dbo].[Zakaz] SET DateSendMaster=CURRENT_TIMESTAMP
WHERE ID_ZAKAZ in
(SELECT [ID_ZAKAZ]	   
  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	
	AND s.ID_STATUS in (1,2,4)

	AND DateSendMaster is null
)

-- сортируем данные сначала по дате прихода заявок к мастеру, а после, если время одинаковое, сортируем по айди заказу,
--таким образом не будут заявки перемешиваться
SELECT [ID_ZAKAZ]
      ,[STREET]
      ,[HOUSE]
      ,[KORPUS]
      ,[KVARTIRA]
      ,[HOLODILNIK_DEFECT]
      ,[DATA]
      ,[VREMJA]
      ,[DateAdd]
      ,[DateSendMaster]
      ,[DateOpenMaster]	  
      ,s.[ID_STATUS]
	  ,s.[NameStatus]
	  ,[Povtor]
  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	
	AND s.ID_STATUS in (1,2,4)
ORDER BY DateSendMaster DESC, ID_ZAKAZ DESC

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    ID_STATUS = (long)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };




                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = row["DateSendMaster"] != DBNull.Value ? (DateTime?)row["DateSendMaster"] : null,
                    DateOpenMaster = row["DateOpenMaster"] != DBNull.Value ? (DateTime?)row["DateOpenMaster"] : null,
                    Povtor= (bool)row["Povtor"],
                    STATUS = status
                };



                Orders.Add(order);
            }


            return Orders;
        }
        /// <summary>
        /// Metod from Master mobile
        /// </summary>        
        public static List<Order> _GetOldOrders(string Sessionid)
        {
            List<Order> Orders = new List<Order>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid }
            };

            #region sql

            string sqlText = $@"
-- сортируем данные сначала по дате прихода заявок к мастеру, а после, если время одинаковое, сортируем по айди заказу,
--таким образом не будут заявки перемешиваться
SELECT [ID_ZAKAZ]
      ,[STREET]
      ,[HOUSE]
      ,[KORPUS]
      ,[KVARTIRA]
      ,[HOLODILNIK_DEFECT]
      ,[DATA]
      ,[VREMJA]
      ,o.[DateAdd]
      ,[DateSendMaster]
      ,[DateOpenMaster]	  
	  ,[DateClose]
      ,s.[ID_STATUS]
	  ,s.[NameStatus]
	  ,[Povtor]
	  	   
  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
WHERE 1=1
	AND u.Sessionid=@Sessionid--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	
	AND s.ID_STATUS in (3,5)
	AND DateClose > CURRENT_TIMESTAMP - 30
ORDER BY DateClose DESC, ID_ZAKAZ DESC


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            string prevRUdate = "";

            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    ID_STATUS = (long)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = row["DateSendMaster"] != DBNull.Value ? (DateTime?)row["DateSendMaster"] : null,
                    DateOpenMaster = row["DateOpenMaster"] != DBNull.Value ? (DateTime?)row["DateOpenMaster"] : null,
                    DateClose = row["DateClose"] != DBNull.Value ? (DateTime?)row["DateClose"] : null,
                    DayWeek_Date = "",
                    Povtor=(bool)row["Povtor"],

                    STATUS = status
                };


                // получение дня недели и даты, но заполняется только уникальная дата, т.е. новый день недели, должен быть у самой первой записи
                string RUdate = ((DateTime)row["DateClose"]).ToString("dddd dd/MM/yyyy", CultureInfo.GetCultureInfo("ru-ru"));

                //выполняется в 1 цикле
                if (String.IsNullOrEmpty(prevRUdate))
                {
                    prevRUdate = RUdate;// запоминаем дата для сравнения в след цикле
                    order.DayWeek_Date = RUdate;
                }

                if (!prevRUdate.Equals(RUdate))
                    order.DayWeek_Date = RUdate;


                prevRUdate = RUdate;

                Orders.Add(order);
            }


            return Orders;
        }
        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static List<Order> GetOldOrdersFiltr(string Sessionid, string Adres, DateTime DateStart, DateTime DateEnd,
            bool filtr_SUCCES, bool filtr_DENY, bool filtr_POVTOR, bool filtr_DATE, bool filtr_ADRES)
        {
            List<Order> Orders = new List<Order>();

            Adres = "%" + Adres + "%";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid },
                new SqlParameter(@"Adres",SqlDbType.NVarChar) { Value =Adres },
                new SqlParameter(@"DateStart",SqlDbType.DateTime) { Value =DateStart },
                new SqlParameter(@"DateEnd",SqlDbType.DateTime) { Value =DateEnd }
            };

            #region sql 

            string Where_povtor = "";
            string Where_Status = "";
            string Where_date = "";
            string Where_Adres = "";

            if (filtr_ADRES)
                Where_Adres = "AND STREET like @Adres";

            if (filtr_DATE)
                Where_date = "AND DateClose between @DateStart AND @DateEnd";

            if (filtr_POVTOR)
                Where_povtor = "AND o.povtor=1";
            else
                Where_povtor = "AND o.povtor=0";



            // будет активно в поиске либо оба статуса либо один из них,
            //если оба неактивны, то не выводим данные статусы
            if (filtr_SUCCES || filtr_DENY)
            {
                if (filtr_SUCCES && filtr_DENY)
                    Where_Status = "AND s.ID_STATUS in (3,5)";
                else
                    if (filtr_SUCCES)
                    Where_Status = "AND s.ID_STATUS in (5)";
                else
                    if (filtr_DENY)
                    Where_Status = "AND s.ID_STATUS in (3)";                  
            }
            else
                Where_Status = "AND s.ID_STATUS NOT IN (3,5)";


            string sqlText = $@"
-- сортируем данные сначала по дате прихода заявок к мастеру, а после, если время одинаковое, сортируем по айди заказу,
--таким образом не будут заявки перемешиваться
SELECT [ID_ZAKAZ]
      ,[STREET]
      ,[HOUSE]
      ,[KORPUS]
      ,[KVARTIRA]
      ,[HOLODILNIK_DEFECT]
      ,[DATA]
      ,[VREMJA]
      ,o.[DateAdd]
      ,[DateSendMaster]
      ,[DateOpenMaster]	  
	  ,[DateClose]
      ,s.[ID_STATUS]
	  ,s.[NameStatus]
      ,o.[Povtor]
	  	   
  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
WHERE 1=1
	AND u.Sessionid=@Sessionid--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid		
    AND s.ID_STATUS NOT IN (1,2,4)-- не выводим новые заявки
	{Where_date}
	{Where_Adres}
    {Where_povtor}
    {Where_Status}

	
ORDER BY DateClose DESC, ID_ZAKAZ DESC


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);

            string prevRUdate = "";

            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    ID_STATUS = (long)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };

                //!!! сделать обработку и присвоить к DayWeek_Date
                //день недели + дату

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = row["DateSendMaster"] != DBNull.Value ? (DateTime?)row["DateSendMaster"] : null,
                    DateOpenMaster = row["DateOpenMaster"] != DBNull.Value ? (DateTime?)row["DateOpenMaster"] : null,
                    DateClose = row["DateClose"] != DBNull.Value ? (DateTime?)row["DateClose"] : null,
                    DayWeek_Date ="",
                    Povtor=(bool)row["Povtor"],


                    STATUS = status
                };


                // получение дня недели и даты, но заполняется только уникальная дата, т.е. новый день недели, должен быть у самой первой записи
                string RUdate = ((DateTime)row["DateClose"]).ToString("dddd dd/MM/yyyy", CultureInfo.GetCultureInfo("ru-ru"));

                //выполняется в 1 цикле
                if (String.IsNullOrEmpty(prevRUdate))
                {
                    prevRUdate = RUdate;// запоминаем дата для сравнения в след цикле
                    order.DayWeek_Date = RUdate;
                }

                if (!prevRUdate.Equals(RUdate))
                    order.DayWeek_Date = RUdate;

                prevRUdate = RUdate;

                Orders.Add(order);
            }


            return Orders;
        }
        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static List<Order> GetOldOrdersFromOrder(long ID_ZAKAZ)
        {
            List<Order> Orders = new List<Order>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ }
            };

            #region sql

            string sqlText = $@" 
  SELECT
   ID_ZAKAZ
  ,HOLODILNIK_DEFECT
  ,DateClose
  ,s.ID_STATUS
  ,s.NameStatus
  FROM [dbo].[Zakaz] o
  JOIN [dbo].[Status] s ON s.ID_STATUS=o.ID_STATUS
  WHERE 1=1
	AND o.ID_ZAKAZ<> @ID_ZAKAZ
	AND 
	(
		(	
				STREET = (SELECT STREET FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )	   -- @ID_ZAKAZ
			AND	HOUSE =	 (SELECT HOUSE FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )	   -- @ID_ZAKAZ
			AND	KORPUS = (SELECT KORPUS FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )	   -- @ID_ZAKAZ
			AND	KVARTIRA = (SELECT KVARTIRA FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )  -- @ID_ZAKAZ
		)
		OR
		(
				Msisdn1 = (SELECT z.Msisdn1 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn1 = (SELECT z.Msisdn2 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn1 = (SELECT z.Msisdn3 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			                                                                      
			OR	Msisdn2 = (SELECT z.Msisdn1 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn2 = (SELECT z.Msisdn2 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn2 = (SELECT z.Msisdn3 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			--                                                                    
			OR	Msisdn3 = (SELECT z.Msisdn1 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn3 = (SELECT z.Msisdn2 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn3 = (SELECT z.Msisdn3 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
		)
	)
	
	ORDER BY DateClose DESC

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    ID_STATUS = (long)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };


                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DateClose = row["DateClose"] != DBNull.Value ? (DateTime?)row["DateClose"] : null,
                    STATUS = status
                };




                Orders.Add(order);
            }


            return Orders;
        }
        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static Order GetOrder(string Sessionid, long ID_ZAKAZ)
        {
            List<Order> Orders = new List<Order>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ }
            };

            #region sql

            string sqlText = $@"

-- установка времени первого открытия заявки
UPDATE [dbo].[Zakaz] SET DateOpenMaster = CURRENT_TIMESTAMP
WHERE-DateOpenMaster is null
	AND ID_ZAKAZ=@ID_ZAKAZ

-- получение заявки
SELECT [ID_ZAKAZ]
      ,[STREET]
      ,[HOUSE]
      ,[KORPUS]
      ,[KVARTIRA]
      ,[PODEST]
      ,[ETAG]
      ,[KOD_DOMOFONA]
      ,[HOLODILNIK_DEFECT]
      ,[DATA]
      ,[VREMJA]
      ,[PRIMECHANIE]
      ,[Msisdn1]
      ,[Msisdn2]
      ,[Msisdn3]
      ,org.[ID_ORGANIZATION]
	  ,org.[NameOrg]
      ,[DateAdd]
      ,[DateSendMaster]
      ,[DateOpenMaster]
      ,s.[ID_STATUS]
	  ,s.[NameStatus]
      ,[MoneyAll]
      ,[MoneyFirm]
      ,[MoneyDetal]
      ,[MoneyMaster]
      ,[DescripClose]
      ,[NameClient]
      ,[ID_USER_ADD]
      ,[DateClose]
	  ,u.[Name]
  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
LEFT JOIN [Organization] org ON org.ID_ORGANIZATION=o.ID_ORGANIZATION
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	
	AND o.ID_ZAKAZ=@ID_ZAKAZ --2--@ID_ZAKAZ
";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText,parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    ID_STATUS = (long)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };

                Organization org = new Organization
                {
                    ID_ORGANIZATION = (long)row["ID_ORGANIZATION"],
                    NameOrg = (string)row["NameOrg"],
                };

                User userMaster = new User
                {
                    Name = (string)row["Name"]
                };

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = row["DateSendMaster"] != DBNull.Value ? (DateTime?)row["DateSendMaster"] : null,
                    DateOpenMaster = row["DateOpenMaster"] != DBNull.Value ? (DateTime?)row["DateOpenMaster"] : null,
                    DateClose = row["DateClose"] != DBNull.Value ? (DateTime?)row["DateClose"] : null,


                    DescripClose = (string)row["DescripClose"],
                    KOD_DOMOFONA = (string)row["KOD_DOMOFONA"],
                    MoneyAll = (int)row["MoneyAll"],
                    MoneyDetal = (int)row["MoneyDetal"],
                    MoneyFirm = (int)row["MoneyFirm"],
                    NameClient = (string)row["NameClient"],
                    PODEST = (string)row["PODEST"],
                    ETAG = (string)row["ETAG"],
                    PRIMECHANIE = (string)row["PRIMECHANIE"],
                    USER_MASTER = userMaster,

                    STATUS = status,
                    ORGANIZATION=org
                };

                return order;
            }


            return null;
        }        
        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static List<Order> GetNewNotifi(string Sessionid)
        {
            List<Order> Orders = new List<Order>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid }
            };

            #region sql

            string sqlText = $@"
-- установка времени отправки уведомления мастеру
UPDATE [dbo].[Zakaz] SET DateSendMaster =CURRENT_TIMESTAMP
WHERE DateSendMaster is null
	AND ID_MASTER = (
	SELECT ID_USER FROM [User] 
	WHERE 1=1
		AND Sessionid=	@Sessionid	--'2847C1CF-9DAE-4FD0-826F-C5A8BFD507C8'--@Sessionid	
	
	)
	AND ID_STATUS in (1,2)
	AND DateSendMaster is null
	
-- Выводим все новые заказы мастера

SELECT ID_ZAKAZ,
		HOLODILNIK_DEFECT
				
FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	
	AND s.ID_STATUS in (1,2)
	AND o.DateSendMaster is null

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status();

                Organization org = new Organization();

                User userMaster = new User();

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],

                    USER_MASTER = userMaster,
                    STATUS = status,
                    ORGANIZATION = org
                };

                Orders.Add(order);
            }


            return Orders;
        }


        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static void SetStatus_InWork(string Sessionid, long ID_ZAKAZ)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ },
                new SqlParameter(@"ID_STATUS",SqlDbType.BigInt) { Value =4 }// в работе
            };

            #region sql

            string sqlText = $@"

  
declare @STATUS nvarchar(50),
		@USER bigint;



-- изменение статуса у заявки
UPDATE [dbo].[Zakaz] SET ID_STATUS=@ID_STATUS
WHERE ID_ZAKAZ =(
SELECT ID_ZAKAZ  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	--
	AND o.ID_ZAKAZ=@ID_ZAKAZ --2--@ID_ZAKAZ --
	)


-- получаем айди пользователя и название статуса
SET @USER=(
SELECT ID_USER FROM [dbo].[User] WHERE Sessionid=@Sessionid
)

SET @STATUS = (
SELECT NameStatus FROM [dbo].[Status] WHERE ID_STATUS=@ID_STATUS
)

-- Логирование изменение статуса
INSERT INTO [dbo].[LogStatusOrder]
           ([ID_ZAKAZ]
           ,[STATUS]
           ,[USER]
           ,[DateChange])
     VALUES
           (@ID_ZAKAZ
           ,@STATUS
           ,@USER
           ,CURRENT_TIMESTAMP)



";

            #endregion

            // получаем данные из запроса
            ExecuteSqlStatic(sqlText, parameters);
        }
        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static void SetStatus_Succes(string Sessionid, long ID_ZAKAZ, int MoneyAll, int MoneyDetal, int MoneyFirm)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ },
                new SqlParameter(@"ID_STATUS",SqlDbType.BigInt) { Value = 5 },// Выполнен
                new SqlParameter(@"MoneyAll",SqlDbType.Int) { Value =MoneyAll },
                new SqlParameter(@"MoneyDetal",SqlDbType.Int) { Value =MoneyDetal },
                new SqlParameter(@"MoneyFirm",SqlDbType.Int) { Value =MoneyFirm }
            };

            #region sql

            string sqlText = $@"

declare @STATUS nvarchar(50),
		@USER bigint;



-- изменение статуса у заявки
UPDATE [dbo].[Zakaz] SET ID_STATUS=@ID_STATUS, MoneyAll=@MoneyAll, MoneyDetal=@MoneyDetal, MoneyFirm=@MoneyFirm, DateClose=CURRENT_TIMESTAMP, MoneyMaster=(MoneyAll-MoneyDetal-MoneyFirm)
WHERE ID_ZAKAZ =(
SELECT ID_ZAKAZ  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	--
	AND o.ID_ZAKAZ=@ID_ZAKAZ --2--@ID_ZAKAZ --
	)


-- получаем имя мастера и название статуса
SET @USER=(
SELECT ID_USER FROM [dbo].[User] WHERE Sessionid=@Sessionid
)

SET @STATUS = (
SELECT NameStatus FROM [dbo].[Status] WHERE ID_STATUS=@ID_STATUS
)

-- Логирование изменение статуса
INSERT INTO [dbo].[LogStatusOrder]
           ([ID_ZAKAZ]
           ,[STATUS]
           ,[USER]
           ,[DateChange])
     VALUES
           (@ID_ZAKAZ
           ,@STATUS
           ,@USER
           ,CURRENT_TIMESTAMP)



";

            #endregion

            // получаем данные из запроса
            ExecuteSqlStatic(sqlText, parameters);
        }
        /// <summary>
        /// Metod from Master mobile
        /// </summary>
        public static void SetStatus_Denied(string Sessionid, long ID_ZAKAZ, string DescripClose)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.Int) { Value =ID_ZAKAZ },
                new SqlParameter(@"ID_STATUS",SqlDbType.Int) { Value = 3 },// Отказ
                new SqlParameter(@"DescripClose",SqlDbType.NVarChar) { Value =DescripClose }
            };

            #region sql

            string sqlText = $@"

declare @STATUS nvarchar(50),
		@USER bigint;



-- изменение статуса у заявки
UPDATE [dbo].[Zakaz] SET ID_STATUS=@ID_STATUS, DescripClose=@DescripClose, DateClose=CURRENT_TIMESTAMP
WHERE ID_ZAKAZ =(
SELECT ID_ZAKAZ  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	--
	AND o.ID_ZAKAZ=@ID_ZAKAZ --2--@ID_ZAKAZ --
	)


-- получаем имя мастера и название статуса
SET @USER=(
SELECT ID_USER FROM [dbo].[User] WHERE Sessionid=@Sessionid
)

SET @STATUS = (
SELECT NameStatus FROM [dbo].[Status] WHERE ID_STATUS=@ID_STATUS
)

-- Логирование изменение статуса
INSERT INTO [dbo].[LogStatusOrder]
           ([ID_ZAKAZ]
           ,[STATUS]
           ,[USER]
           ,[DateChange])
     VALUES
           (@ID_ZAKAZ
           ,@STATUS
           ,@USER
           ,CURRENT_TIMESTAMP)

";

            #endregion

            // получаем данные из запроса
            ExecuteSqlStatic(sqlText, parameters);
        }

        

        /// <summary>
        /// Metod from site
        /// </summary>        
        public static void ChangeStatusDispetcher(string Login, long ID_ZAKAZ, Status Status)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ },
                new SqlParameter(@"STATUS",SqlDbType.NVarChar) { Value =Status.NameStatus },
                new SqlParameter(@"ID_STATUS",SqlDbType.BigInt) { Value =Status.ID_STATUS }
            };

            #region sql

            string sqlText = $@"
declare @ID_USER bigint;



-- изменение статуса у заявки
UPDATE [dbo].[Zakaz] SET ID_STATUS=@ID_STATUS
WHERE ID_ZAKAZ =(
SELECT ID_ZAKAZ  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
WHERE 1=1
	AND u.Sessionid=@Sessionid	--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	--
	AND o.ID_ZAKAZ=@ID_ZAKAZ --2--@ID_ZAKAZ --
	)


-- получаем айди пользователя 
SET @ID_USER=(
SELECT ID_USER FROM [dbo].[User] WHERE Login=@Login
)

-- Логирование изменение статуса
INSERT INTO [dbo].[LogStatusOrder]
           ([ID_ZAKAZ]
           ,[STATUS]
           ,[USER]
           ,[DateChange])
     VALUES
           (@ID_ZAKAZ
           ,@STATUS
           ,@ID_USER
           ,CURRENT_TIMESTAMP)


";

            #endregion

            // получаем данные из запроса
            ExecuteSqlStatic(sqlText, parameters);
        }
        /// <summary>
        /// Metod from site
        /// </summary>
        public static Order GetOrderSite(long ID_ZAKAZ)
        {

            SqlParameter[] parameters = new SqlParameter[]
            {
                //new SqlParameter(@"Login",SqlDbType.NVarChar) { Value =Login },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ }
            };

            #region sql

            string sqlText = $@"

-- получение заявки
SELECT [ID_ZAKAZ]
      ,[STREET]
      ,[HOUSE]
      ,[KORPUS]
      ,[KVARTIRA]
      ,[PODEST]
      ,[ETAG]
      ,[KOD_DOMOFONA]
      ,[HOLODILNIK_DEFECT]
      ,[DATA]
      ,[VREMJA]
      ,[PRIMECHANIE]
      ,[Msisdn1]
      ,[Msisdn2]
      ,[Msisdn3]
      ,[Povtor]
      ,org.[ID_ORGANIZATION]
	  ,org.[NameOrg]
      ,o.[DateAdd]
      ,[DateSendMaster]
      ,[DateOpenMaster]
      ,s.[ID_STATUS]
	  ,s.[NameStatus]
      ,s.[ColorHex]
      ,[MoneyAll]
      ,[MoneyFirm]
      ,[MoneyDetal]
      ,[MoneyMaster]
      ,[DescripClose]
      ,[NameClient]
      ,[ID_USER_ADD]
      ,[DateClose]
	  ,u.[Name] AS NameMaster
	  ,uadd.[Name] AS NameUserAdd
  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER -- ADD User MASTER
JOIN [User] uadd ON uadd.ID_USER=o.ID_USER_ADD -- ADD User DISP or ADMIN
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
LEFT JOIN [Organization] org ON org.ID_ORGANIZATION=o.ID_ORGANIZATION
WHERE 1=1	
	AND o.ID_ZAKAZ=@ID_ZAKAZ --2--@ID_ZAKAZ
";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    ID_STATUS = (long)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                    ColorHex = (string)row["ColorHex"]
                };

                Organization org = new Organization
                {
                    ID_ORGANIZATION = (long)row["ID_ORGANIZATION"],
                    NameOrg = (string)row["NameOrg"]
                };

                User userMaster = new User
                {
                    Name = (string)row["NameMaster"]
                };

                User userAdd = new User()
                {
                    Name = (string)row["NameUserAdd"]
                };

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = row["DateSendMaster"] != DBNull.Value ? (DateTime?)row["DateSendMaster"] : null,
                    DateOpenMaster = row["DateOpenMaster"] != DBNull.Value ? (DateTime?)row["DateOpenMaster"] : null,
                    DateClose = row["DateClose"] != DBNull.Value ? (DateTime?)row["DateClose"] : null,


                    DescripClose = (string)row["DescripClose"],
                    KOD_DOMOFONA = (string)row["KOD_DOMOFONA"],
                    MoneyAll = (int)row["MoneyAll"],
                    MoneyDetal = (int)row["MoneyDetal"],
                    MoneyFirm = (int)row["MoneyFirm"],
                    MoneyMaster= (int)row["MoneyMaster"],                    
                    NameClient = (string)row["NameClient"],
                    PODEST = (string)row["PODEST"],
                    ETAG = (string)row["ETAG"],
                    PRIMECHANIE = (string)row["PRIMECHANIE"],
                    Msisdn1 = (string)row["Msisdn1"],
                    Msisdn2 = (string)row["Msisdn2"],
                    Msisdn3 = (string)row["Msisdn3"],
                    Povtor = (bool)row["Povtor"],

                    USER_MASTER = userMaster,
                    USER_ADD= userAdd,

                    STATUS = status,
                    ORGANIZATION = org

                };

                return order;
            }


            return new Order();
        }

        /// <summary>
        /// Metod from site
        /// </summary>
        public static List<Order> GetOrdersSite(int page, int step, FiltrOrders filtrOrders)
        {
            List<Order> orders = new List<Order>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"DateStart",SqlDbType.DateTime) { Value =filtrOrders.DateStart },
                new SqlParameter(@"DateEnd",SqlDbType.DateTime) { Value =filtrOrders.DateEnd }
            };

            string WHERE_adres = "";
            string WHERE_betweenDate = "";
            string WHERE_povtor = "";
            string WHERE_idStatus = "";

            if ( !String.IsNullOrEmpty(filtrOrders.Adres))
                WHERE_adres = $" AND lower(STREET) like '%{filtrOrders.Adres.ToLower()}%'";

            WHERE_betweenDate = $"AND z.DateAdd between  @DateStart AND @DateEnd";

            if (filtrOrders.Povtor)
                WHERE_povtor = "AND povtor=1";

            if (filtrOrders.ID_STATUS != 0)
                WHERE_idStatus = $"AND z.ID_STATUS = {filtrOrders.ID_STATUS}";




            #region sql

            string sqlText = @$"
WITH OrdersPage AS
(
    SELECT 
	ID_ZAKAZ
	,HOLODILNIK_DEFECT
	,u.Name	
	,NameClient
	,STREET
	,HOUSE
	,KORPUS
	,KVARTIRA
	,Msisdn1
	,z.DateAdd
	,DATA
	,VREMJA
	,s.NameStatus
	,s.ColorHex
	
    ,ROW_NUMBER() OVER (ORDER BY z.[DateAdd] DESC) AS 'RowNumber'
    FROM [dbo].[Zakaz] z
	JOIN [Status] s ON s.ID_STATUS=z.ID_STATUS
	JOIN [User] u ON u.ID_USER=z.ID_MASTER
	WHERE 1=1
		{WHERE_adres}
        {WHERE_betweenDate}
        {WHERE_povtor}
        {WHERE_idStatus}
) 

SELECT
	 ID_ZAKAZ
	,HOLODILNIK_DEFECT
	,Name	
	,NameClient
	,STREET
	,HOUSE
	,KORPUS
	,KVARTIRA
	,Msisdn1
	,DateAdd
	,DATA
	,VREMJA
	,NameStatus
    ,ColorHex
	,RowNumber
FROM OrdersPage 
WHERE RowNumber BETWEEN ({page}-1)*{step} AND {page}*{step}
ORDER BY RowNumber ASC

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {                    
                    NameStatus = (string)row["NameStatus"],
                    ColorHex = (string)row["ColorHex"]
                };


                User userMaster = new User
                {
                    Name = (string)row["Name"]
                };

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    Msisdn1 = (string)row["Msisdn1"],

                    NameClient = (string)row["NameClient"],
                    USER_MASTER = userMaster,

                    STATUS = status
                };

                orders.Add(order);
            }


            return orders;
        }
        /// <summary>
        /// Metod from site
        /// </summary>
        public static List<Order> GetOrdersPrev(long ID_ZAKAZ)
        {
            List<Order> orders = new List<Order>();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ }
            };


            #region sql

            string sqlText = @$"
SELECT
  ID_ZAKAZ
	,HOLODILNIK_DEFECT
	,u.Name	
	,NameClient
	,STREET
	,HOUSE
	,KORPUS
	,KVARTIRA
	,Msisdn1
	,o.DateAdd
	,DATA
	,VREMJA
	,s.NameStatus
	,s.ColorHex
  FROM [dbo].[Zakaz] o
  JOIN [dbo].[Status] s ON s.ID_STATUS=o.ID_STATUS
  JOIN [User] u ON u.ID_USER=o.ID_MASTER
  WHERE 1=1
	AND o.ID_ZAKAZ<> @ID_ZAKAZ
	AND 
	(
		(	
				STREET = (SELECT STREET FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )	   -- @ID_ZAKAZ
			AND	HOUSE =	 (SELECT HOUSE FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )	   -- @ID_ZAKAZ
			AND	KORPUS = (SELECT KORPUS FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )	   -- @ID_ZAKAZ
			AND	KVARTIRA = (SELECT KVARTIRA FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ )  -- @ID_ZAKAZ
		)
		OR
		(
				Msisdn1 = (SELECT z.Msisdn1 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn1 = (SELECT z.Msisdn2 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn1 = (SELECT z.Msisdn3 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			                                                                      
			OR	Msisdn2 = (SELECT z.Msisdn1 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn2 = (SELECT z.Msisdn2 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn2 = (SELECT z.Msisdn3 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			--                                                                    
			OR	Msisdn3 = (SELECT z.Msisdn1 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn3 = (SELECT z.Msisdn2 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
			OR	Msisdn3 = (SELECT z.Msisdn3 FROM [dbo].[Zakaz] z WHERE z.ID_ZAKAZ=@ID_ZAKAZ ) -- @ID_ZAKAZ
		)
	)
	
	
	ORDER BY DateClose DESC

";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    NameStatus = (string)row["NameStatus"],
                    ColorHex = (string)row["ColorHex"]
                };


                User userMaster = new User
                {
                    Name = (string)row["Name"]
                };

                Order order = new Order
                {
                    ID_ZAKAZ = (long)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    Msisdn1 = (string)row["Msisdn1"],

                    NameClient = (string)row["NameClient"],
                    USER_MASTER = userMaster,

                    STATUS = status
                };

                orders.Add(order);
            }


            return orders;
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
