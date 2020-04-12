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
    public class ReportZakazMaster
    {
        public string FIOMaster { get; set; }
        public long ID_MASTER { get; set; }
        public int AllZakaz { get; set; }
        public int SUCCES_ed { get; set; }
        public decimal SUCCES_proc { get; set; }
        public int DIAGNOSTIK_ed { get; set; }
        public decimal DIAGNOSTIK_proc { get; set; }
        public int POVTORMoney_ed { get; set; }
        public decimal POVTORMoney_proc { get; set; }
        public int POVTORNotMoney_ed { get; set; }
        public decimal POVTORNotMoney_proc { get; set; }
        public int DENY_ed { get; set; }
        public decimal DENY_proc { get; set; }



        // Общая стата
        public static List<ReportZakazMaster> GetZakazMasters(reportZakazMasterFiltr Filtr)
        {
            List<ReportZakazMaster> masters = new List<ReportZakazMaster>();

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End}
            };


            #region sql


            string sqlText = @"
-- Кол-во заказов по мастерам
SELECT 
m.Name
,z.ID_MASTER
,count(z.ID_ZAKAZ) AS cntAllZakaz

--c.	Выполнено с деньгами кол-во
,(SELECT count(1) FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (5) 
		--AND zz.MoneyFirm>0
		AND	zz.DateClose between @start AND @end
) AS cntAllClose 

,(SELECT CAST( (CAST (count(*) AS FLOAT )/count(z.ID_ZAKAZ))*100 AS numeric(36,1)) AS Procent FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (5) 
		--AND zz.MoneyFirm>0
		AND	zz.DateClose between @start AND @end
) AS cntAllCloseProcent

--d.	Диагностик 
,(SELECT count(1) FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (7) 
		--AND zz.MoneyFirm>0
		AND	zz.DateClose between @start AND @end
) AS cntDiagnostik

,(SELECT CAST( (CAST (count(*) AS FLOAT )/count(z.ID_ZAKAZ))*100 AS numeric(36,1)) AS Procent FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (7) 
		--AND zz.MoneyFirm>0
		AND	zz.DateClose between @start AND @end
) AS cntDiagnostikProcent


--e.	Повторки с деньгами в фирму
,(SELECT count(1) FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (5,7) 
		AND zz.MoneyFirm>0		
		AND	zz.DateClose between @start AND @end
		AND zz.Povtor=1
) AS cntPovtor

,(SELECT CAST( (CAST (count(*) AS FLOAT )/count(z.ID_ZAKAZ))*100 AS numeric(36,1)) AS Procent FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (5,7) 
		AND zz.MoneyFirm>0
		AND	zz.DateClose between @start AND @end
		AND zz.Povtor=1
) AS cntPovtorProcent

--f.	Повторки без денег 
,(SELECT count(1) FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (5,7) 
		AND zz.MoneyFirm=0		
		AND	zz.DateClose between @start AND @end
		AND zz.Povtor=1
) AS cntPovtorNotMoney

,(SELECT CAST( (CAST (count(*) AS FLOAT )/count(z.ID_ZAKAZ))*100 AS numeric(36,1)) AS Procent FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (5,7) 
		AND zz.MoneyFirm=0
		AND	zz.DateClose between @start AND @end
		AND zz.Povtor=1
) AS cntPovtorNotMoneyProcent


--g.	Отказы
,(SELECT count(1) FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (3) 		
		AND	zz.DateClose between @start AND @end
) AS cntDeny

,(SELECT CAST( (CAST (count(*) AS FLOAT )/count(z.ID_ZAKAZ))*100 AS numeric(36,1)) AS Procent FROM [Zakaz] zz 
	WHERE zz.ID_MASTER=z.ID_MASTER 
		AND zz.ID_STATUS in (3) 		
		AND	zz.DateClose between @start AND @end
) AS cntDenyProcent


 FROM [Zakaz] z
JOIN [User] m ON m.ID_USER=z.ID_MASTER
WHERE z.ID_STATUS in (3,5,7)
	AND	z.DateClose between @start AND @end

GROUP BY m.Name, z.ID_MASTER
ORDER BY m.Name ASC


";

            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);
//Name	ID_MASTER	cntAllZakaz	cntAllClose	cntAllCloseProcent	cntDiagnostik	
//cntDiagnostikProcent	cntPovtor	cntPovtorProcent	cntPovtorNotMoney	cntPovtorNotMoneyProcent	cntDeny	cntDenyProcent
       

            foreach (DataRow row in dt.Rows)
            {

                ReportZakazMaster master = new ReportZakazMaster
                {
                    FIOMaster=(string)row["Name"],
                    ID_MASTER= (long)row["ID_MASTER"],
                    AllZakaz=(int)row["cntAllZakaz"],
                    SUCCES_ed = (int)row["cntAllClose"],
                    SUCCES_proc = (decimal)row["cntAllCloseProcent"],
                    DIAGNOSTIK_ed = (int)row["cntDiagnostik"],
                    DIAGNOSTIK_proc = (decimal)row["cntDiagnostikProcent"],
                    POVTORMoney_ed = (int)row["cntPovtor"],
                    POVTORMoney_proc = (decimal)row["cntPovtorProcent"],
                    POVTORNotMoney_ed = (int)row["cntPovtorNotMoney"],
                    POVTORNotMoney_proc = (decimal)row["cntPovtorNotMoneyProcent"],
                    DENY_ed = (int)row["cntDeny"],
                    DENY_proc = (decimal)row["cntDenyProcent"]
                };

                masters.Add(master);

            }


            return masters;
        }
        
        // Диаграммы
        public static List<PointTime> GetDiagramLine_ZakazClose(reportZakazMasterFiltr Filtr)
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

--c.	Выполнено с деньгами кол-во
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day
, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
FROM [Zakaz] z
	WHERE z.ID_MASTER= @ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm>0
		AND	z.DateClose between @start AND @end
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

--c.	Выполнено с деньгами кол-во
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day
, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
FROM [Zakaz] z
	WHERE z.ID_MASTER= @ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm>0
		AND	z.DateClose between @start AND @end
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	


";


            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

--c.	Выполнено с деньгами кол-во
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day
, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
FROM [Zakaz] z
	WHERE z.ID_MASTER= @ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm>0
		AND	z.DateClose between @start AND @end
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
                    Y = (int)row["cntZakazClose"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_Diagnostik(reportZakazMasterFiltr Filtr)
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

------------------------------------
--d.	Диагностик 
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day
, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (7) 
		AND z.MoneyFirm>0
		AND	z.DateClose between @start AND @end
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

------------------------------------
--d.	Диагностик 
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day
, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (7) 
		AND z.MoneyFirm>0
		AND	z.DateClose between @start AND @end
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

------------------------------------
--d.	Диагностик 
SELECT 
 DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day
, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (7) 
		AND z.MoneyFirm>0
		AND	z.DateClose between @start AND @end
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
                    Y = (int)row["cntZakazClose"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_PovtorMoney(reportZakazMasterFiltr Filtr)
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


------------------------------------
--e.	Повторки с деньгами в фирму
SELECT  
DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm>0		
		AND	z.DateClose between @start AND @end
		AND z.Povtor=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"


------------------------------------
--e.	Повторки с деньгами в фирму
SELECT  
DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm>0		
		AND	z.DateClose between @start AND @end
		AND z.Povtor=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	
	
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"


------------------------------------
--e.	Повторки с деньгами в фирму
SELECT  
DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm>0		
		AND	z.DateClose between @start AND @end
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
                    Y = (int)row["cntZakazClose"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_PovtorNotMoney(reportZakazMasterFiltr Filtr)
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

------------------------------------
--f.	Повторки без денег 
SELECT  
DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm=0		
		AND	z.DateClose between @start AND @end
		AND z.Povtor=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC
";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"

------------------------------------
--f.	Повторки без денег 
SELECT  
DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm=0		
		AND	z.DateClose between @start AND @end
		AND z.Povtor=1
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	
	
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"

------------------------------------
--f.	Повторки без денег 
SELECT  
DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
 FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (5,7) 
		AND z.MoneyFirm=0		
		AND	z.DateClose between @start AND @end
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
                    Y = (int)row["cntZakazClose"]
                };


                chart.Add(point);
            }

            return chart;
        }
        public static List<PointTime> GetDiagramLine_Deny(reportZakazMasterFiltr Filtr)
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


------------------------------------
--g.	Отказы
SELECT 
DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,DATEPART(DAY,z.DateClose) AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
  FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (3) 		
		AND	z.DateClose between @start AND @end
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose),DATEPART(DAY,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	,DATEPART(DAY,z.DateClose) ASC

";

            if (Filtr.GroupDate == GroupByDate.MONTH)
                sqlText = $@"


------------------------------------
--g.	Отказы
SELECT 
DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
  FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (3) 		
		AND	z.DateClose between @start AND @end
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC
	

	
	


";

            if (Filtr.GroupDate == GroupByDate.YEAR)
                sqlText = $@"


------------------------------------
--g.	Отказы
SELECT 
DATEPART(YEAR,z.DateClose) AS Year
,1 AS Month
,1 AS Day

, count(1) AS cntZakazClose
, SUM(z.MoneyFirm) AS summ
  FROM [Zakaz] z
	WHERE z.ID_MASTER=@ID_MASTER 
		AND z.ID_STATUS in (3) 		
		AND	z.DateClose between @start AND @end
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
                    Y = (int)row["cntZakazClose"]
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
