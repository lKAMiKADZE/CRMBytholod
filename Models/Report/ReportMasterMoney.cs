using CRMBytholod.Settings;
using CRMBytholod.ViewModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Models.Report
{
    public class ReportMasterMoney
    {
        public string FIOMaster { get; set; }
        public long ID_MASTER { get; set; }        
        public int Firma { get; set; }
        public int Succes { get; set; }
        public int Diagnostik { get; set; }
        public int PovtorMoney { get; set; }
        public int Detal { get; set; }
        public int ZarabotalMasterNal { get; set; }
        public int ZarabotalMasterNotNal { get; set; }


        // Общая стата
        public static List<ReportMasterMoney> GetMoneyMasters(ReportMasterMoneyFiltr Filtr)
        {
            List<ReportMasterMoney> masters = new List<ReportMasterMoney>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End}
            };


            #region sql

            string sqlText = @"


-- отчет по мастерам

SELECT 
m.Name -- a
,z.ID_MASTER

,(SELECT ISNULL(SUM(z1.MoneyFirm),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DateClose between @start AND @end
	AND z1.MoneyFirm > 0
	) AS MoneyAllFirma --b

,(SELECT ISNULL(SUM(z1.MoneyFirm),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS=5
	AND z1.DateClose between @start AND @end
	AND z1.MoneyFirm > 0
	) AS MoneySucces --c
	
,(SELECT ISNULL(SUM(z1.MoneyFirm),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS=7
	AND z1.DateClose between @start AND @end
	AND z1.MoneyFirm > 0
	) AS MoneyDiagnostik --d

,(SELECT ISNULL(SUM(z1.MoneyFirm),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DateClose between @start AND @end
	AND z1.MoneyFirm > 0
	AND z1.Povtor=1
	) AS MoneyPovtorRab --e
	
,(SELECT ISNULL(SUM(z1.MoneyDetal),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS=5
	--AND z1.DateClose > CURRENT_TIMESTAMP - 30
	AND z1.MoneyFirm > 0
	) AS MoneyDetal --f
	
		
,(SELECT ISNULL(SUM(z1.MoneyMaster),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DateClose between @start AND @end
	AND z1.MoneyFirm > 0
	AND z1.OplataNal=1
	) AS MoneyNal --g
	
,(SELECT ISNULL(SUM(z1.MoneyMaster),0) FROM [Zakaz] z1
	WHERE z1.ID_MASTER=z.ID_MASTER
	AND z1.ID_STATUS in (5,7)
	AND z1.DateClose between @start AND @end
	AND z1.MoneyFirm > 0
	AND z1.OplataNal=0 
	) AS MoneyNotNal --h

, count(1) AS allZakazClose

 FROM [Zakaz] z
JOIN [User] m ON m.ID_USER=z.ID_MASTER
WHERE z.ID_STATUS in (3,5,7)
	AND z.DateClose between @start AND @end
GROUP BY m.Name, z.ID_MASTER
ORDER BY m.Name ASC


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);
            //Name	ID_MASTER	MoneyAllFirma	MoneySucces	MoneyDiagnostik	MoneyPovtorRab	MoneyDetal	MoneyNal	MoneyNotNal	allZakazClose
       

            foreach (DataRow row in dt.Rows)
            {

                ReportMasterMoney master = new ReportMasterMoney
                {
                    FIOMaster = (string)row["Name"],
                    ID_MASTER = (long)row["ID_MASTER"],
                    Firma = (int)row["MoneyAllFirma"],
                    Succes = (int)row["MoneySucces"],
                    Diagnostik = (int)row["MoneyDiagnostik"],
                    PovtorMoney = (int)row["MoneyPovtorRab"],
                    Detal = (int)row["MoneyDetal"],
                    ZarabotalMasterNal = (int)row["MoneyNal"],
                    ZarabotalMasterNotNal = (int)row["MoneyNotNal"]
                };

                masters.Add(master);

            }


            return masters;
        }

        // Диаграммы 
        public static List<PointTime> GetDiagramLine_UpFirma(ReportMasterMoneyFiltr Filtr)
        {
            List<PointTime> chart = new List<PointTime>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End},
                new SqlParameter(@"ID_MASTER",SqlDbType.BigInt) { Value = Filtr.ID_MASTER}
            };


            #region sql

            string sqlText = @"

-----------------------------------------------------
--b.	Сколько заработал всего (в фирму) руб
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER = @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

-----------------------------------------------------
--b.	Сколько заработал всего (в фирму) руб
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER = @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

-----------------------------------------------------
--b.	Сколько заработал всего (в фирму) руб
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER = @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	


";


            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                DateTime date = new DateTime(
                    (int)row["Year"],
                    (int)row["Month"],
                    (int)row["Day"]
                    );

                PointTime point = new PointTime
                {
                    X = date,
                    Y = (int)row["summ"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_UpSucces(ReportMasterMoneyFiltr Filtr)
        {
            List<PointTime> chart = new List<PointTime>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End},
                new SqlParameter(@"ID_MASTER",SqlDbType.BigInt) { Value = Filtr.ID_MASTER}
            };


            #region sql

            string sqlText = @"

-----------------------------------------------------
--c.	Сколько по выполненным заказам (в фирму) руб
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

-----------------------------------------------------
--c.	Сколько по выполненным заказам (в фирму) руб
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	


";


            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

-----------------------------------------------------
--c.	Сколько по выполненным заказам (в фирму) руб
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                DateTime date = new DateTime(
                    (int)row["Year"],
                    (int)row["Month"],
                    (int)row["Day"]
                    );

                PointTime point = new PointTime
                {
                    X = date,
                    Y = (int)row["summ"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_UpDiagnostik(ReportMasterMoneyFiltr Filtr)
        {
            List<PointTime> chart = new List<PointTime>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End},
                new SqlParameter(@"ID_MASTER",SqlDbType.BigInt) { Value = Filtr.ID_MASTER}
            };


            #region sql

            string sqlText = @"

-----------------------------------------------------
--d.	По диагностике руб (в фирму)
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

-----------------------------------------------------
--d.	По диагностике руб (в фирму)
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	


";


            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

-----------------------------------------------------
--d.	По диагностике руб (в фирму)
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
GROUP BY DATEPART(YEAR,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                DateTime date = new DateTime(
                    (int)row["Year"],
                    (int)row["Month"],
                    (int)row["Day"]
                    );

                PointTime point = new PointTime
                {
                    X = date,
                    Y = (int)row["summ"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_UpPovtorMoney(ReportMasterMoneyFiltr Filtr)
        {
            List<PointTime> chart = new List<PointTime>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End},
                new SqlParameter(@"ID_MASTER",SqlDbType.BigInt) { Value = Filtr.ID_MASTER}
            };


            #region sql

            string sqlText = @"

-----------------------------------------------------
--e.	По повторам рабочим с деньгами (в фирму) руб.
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER = @ID_MASTER
	AND z.Povtor=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

-----------------------------------------------------
--e.	По повторам рабочим с деньгами (в фирму) руб.
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER = @ID_MASTER
	AND z.Povtor=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

-----------------------------------------------------
--e.	По повторам рабочим с деньгами (в фирму) руб.
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER = @ID_MASTER
	AND z.Povtor=1
GROUP BY DATEPART(YEAR,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                DateTime date = new DateTime(
                    (int)row["Year"],
                    (int)row["Month"],
                    (int)row["Day"]
                    );

                PointTime point = new PointTime
                {
                    X = date,
                    Y = (int)row["summ"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_UpMasterNal(ReportMasterMoneyFiltr Filtr)
        {
            List<PointTime> chart = new List<PointTime>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End},
                new SqlParameter(@"ID_MASTER",SqlDbType.BigInt) { Value = Filtr.ID_MASTER}
            };


            #region sql

            string sqlText = @"

----------------------------------------------------- 
--g.	Сколько заработал сам мастер наличными
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyMaster) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
	AND z.OplataNal=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

----------------------------------------------------- 
--g.	Сколько заработал сам мастер наличными
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyMaster) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
	AND z.OplataNal=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

----------------------------------------------------- 
--g.	Сколько заработал сам мастер наличными
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyMaster) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
	AND z.OplataNal=1
GROUP BY DATEPART(YEAR,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	


";


            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                DateTime date = new DateTime(
                    (int)row["Year"],
                    (int)row["Month"],
                    (int)row["Day"]
                    );

                PointTime point = new PointTime
                {
                    X = date,
                    Y = (int)row["summ"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_UpMasterNotNal(ReportMasterMoneyFiltr Filtr)
        {
            List<PointTime> chart = new List<PointTime>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End},
                new SqlParameter(@"ID_MASTER",SqlDbType.BigInt) { Value = Filtr.ID_MASTER}
            };


            #region sql

            string sqlText = @"

-----------------------------------------------------
--h.	Сколько заработал сам мастер безналом
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyMaster) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
	AND z.OplataNal=0
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

-----------------------------------------------------
--h.	Сколько заработал сам мастер безналом
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyMaster) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
	AND z.OplataNal=0
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

-----------------------------------------------------
--h.	Сколько заработал сам мастер безналом
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyMaster) AS summ

 FROM [Zakaz] z
WHERE z.ID_STATUS in (5,7)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
	AND z.ID_MASTER =  @ID_MASTER
	AND z.OplataNal=0
GROUP BY DATEPART(YEAR,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	
	


";




            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {
                DateTime date = new DateTime(
                    (int)row["Year"],
                    (int)row["Month"],
                    (int)row["Day"]
                    );

                PointTime point = new PointTime
                {
                    X = date,
                    Y = (int)row["summ"]
                };


                chart.Add(point);
            }

            return chart;
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
