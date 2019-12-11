using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.Settings;
using Microsoft.Data.SqlClient;
using System.Data;

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

        //dop param
        /////////////////////////////////
        public List<Order> PrevOrders { get; set; } // у повторных заявок, данный список будет заполняться предыдущими заявками от этого клиента

        /////////////////////////////////
        public List<Order> GetNewOrders(string Sessionid)
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
                    ID_STATUS = (int)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };




                Order order = new Order
                {
                    ID_ZAKAZ = (int)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = (DateTime?)row["DateSendMaster"],
                    DateOpenMaster = (DateTime?)row["DateOpenMaster"],
                    STATUS = status
                };

                Orders.Add(order);
            }


            return Orders;
        }


        public List<Order> GetOldOrders(string Sessionid)
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
      ,[DateAdd]
      ,[DateSendMaster]
      ,[DateOpenMaster]	  
	  ,[DateClose]
      ,s.[ID_STATUS]
	  ,s.[NameStatus]
	  	   
  FROM [dbo].[Zakaz] o
JOIN [User] u ON u.ID_USER=o.ID_MASTER
JOIN [Status] s ON s.ID_STATUS=o.ID_STATUS
WHERE 1=1
	AND u.Sessionid=@Sessionid--'CB80665A-93E0-4E91-A982-36063D546CE6'--@Sessionid	
	AND s.ID_STATUS in (3,5)
ORDER BY DateClose DESC, ID_ZAKAZ DESC


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                Status status = new Status
                {
                    ID_STATUS = (int)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };


                Order order = new Order
                {
                    ID_ZAKAZ = (int)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = (DateTime?)row["DateSendMaster"],
                    DateOpenMaster = (DateTime?)row["DateOpenMaster"],
                    DateClose = (DateTime?)row["DateClose"],
                    STATUS = status
                };

                Orders.Add(order);
            }


            return Orders;
        }


        public Order GetOrder(string Sessionid, long ID_ZAKAZ)
        {
            List<Order> Orders = new List<Order>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"Sessionid",SqlDbType.NVarChar) { Value =Sessionid },
                new SqlParameter(@"ID_ZAKAZ",SqlDbType.BigInt) { Value =ID_ZAKAZ }
            };

            #region sql

            string sqlText = $@"
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
  FROM [Bytholod].[dbo].[Zakaz] o
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
                    ID_STATUS = (int)row["ID_STATUS"],
                    NameStatus = (string)row["NameStatus"],
                };


                Order order = new Order
                {
                    ID_ZAKAZ = (int)row["ID_ZAKAZ"],
                    STREET = (string)row["STREET"],
                    HOUSE = (string)row["HOUSE"],
                    KORPUS = (string)row["KORPUS"],
                    KVARTIRA = (string)row["KVARTIRA"],
                    HOLODILNIK_DEFECT = (string)row["HOLODILNIK_DEFECT"],
                    DATA = (DateTime)row["DATA"],
                    VREMJA = (string)row["VREMJA"],
                    DateAdd = (DateTime)row["DateAdd"],
                    DateSendMaster = (DateTime?)row["DateSendMaster"],
                    DateOpenMaster = (DateTime?)row["DateOpenMaster"],
                    DateClose = (DateTime?)row["DateClose"],
                    STATUS = status
                };

                return order;
            }


            return null;
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
