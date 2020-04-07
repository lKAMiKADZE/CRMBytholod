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
    public class ReportZakaz
    {

        public decimal SUCCES_proc { get; set; }
        public int SUCCES_ed { get; set; }
        public int SUCCES_summ { get; set; }

        public decimal DIAGNOSTIK_proc { get; set; }
        public int DIAGNOSTIK_ed { get; set; }
        public int DIAGNOSTIK_summ { get; set; }

        public decimal POVTORmoney_proc { get; set; }
        public int POVTORmoney_ed { get; set; }
        public int POVTORmoney_summ { get; set; }

        public decimal POVTORNOTmoney_proc { get; set; }
        public int POVTORNOTmoney_ed { get; set; }

        public decimal DENY_proc { get; set; }
        public int DENY_ed { get; set; }

        public int TOTAL_all_zakaz { get; set; }
        public int TOTAL_avg_getClient { get; set; }
        public int TOTAL_avg_firma { get; set; }


        public List<PointCircle> DiagramCircle_Zakaz { get; set; }
        public List<PointTime> DiagramLine_Firma  { get; set; }
        public List<PointString> DiagramLine_City  { get; set; }
        public List<PointString> DiagramLine_Organization  { get; set; }


        public ReportZakaz(reportZakazFiltr Filtr)
        {
            GetDataClass(Filtr);
            DiagramLine_Firma = GetDiagramLine_Firma(Filtr);
            DiagramLine_City = GetDiagramLine_City(Filtr);
            DiagramLine_Organization = GetDiagramLine_Organization(Filtr);
        }



        private void GetDataClass(reportZakazFiltr Filtr)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End}
            };


            #region sql

            string sqlText = @"

-- ReportZakaz

declare @allZakaz int,
		@ZakazPovtor int;


set @allZakaz = (
SELECT 	count(*) FROM [Zakaz] z
JOIN [Status] s ON s.ID_STATUS=z.ID_STATUS
WHERE 1=1
	AND	z.DateClose between @start AND @end
	AND s.ID_STATUS in (3,5,7)
)

set @ZakazPovtor = (
SELECT 	count(*) FROM [Zakaz] z
JOIN [Status] s ON s.ID_STATUS=z.ID_STATUS
WHERE 1=1
	AND	z.DateClose between @start AND @end
	AND s.ID_STATUS in (3,5,7)
	AND z.Povtor=1
)

-- // ОТЧЕТЫ ОБЪЕДИНЕНЫ ЮНИОН
------------------- UNION --------------

-- Вывод отчета по закрытым заказам (выполнено, отказано, диагностика)
SELECT 
s.NameStatus 
, s.ID_STATUS
,count(*) AS Cnt
, CAST( (CAST (count(*) AS FLOAT )/@allZakaz)*100 AS numeric(36,1)) AS Procent
,SUM(z.MoneyFirm) AS Firma
, 0 AS moneyAll
FROM [Zakaz] z
JOIN [Status] s ON s.ID_STATUS=z.ID_STATUS
WHERE 1=1
	AND	z.DateClose between @start AND @end
	AND s.ID_STATUS in (3,5,7)
GROUP BY s.NameStatus , s.ID_STATUS

-------
UNION
-------

-- Вывод отчета в разбивке по повторам
SELECT 
 'ПОВТОР РАБОЧИЙ' AS NameStatus
, 80 AS ID_STATUS
,count(*) AS Cnt
, CAST( (CAST (count(*) AS FLOAT )/@ZakazPovtor)*100 AS numeric(36,1)) AS Procent
,SUM(z.MoneyFirm) AS Firma
, 0 AS moneyAll
FROM [Zakaz] z
JOIN [Status] s ON s.ID_STATUS=z.ID_STATUS
WHERE 1=1
	AND	z.DateClose between @start AND @end
	AND s.ID_STATUS in (3,5,7)
	AND z.Povtor=1
	AND z.MoneyFirm > 0 
GROUP BY  z.Povtor

-------
UNION
-------

SELECT 
'ПОВТОР НЕ РАБОЧИЙ' AS NameStatus
, 88 AS ID_STATUS
,count(*) AS Cnt
, CAST( (CAST (count(*) AS FLOAT )/@ZakazPovtor)*100 AS numeric(36,1)) AS Procent
,SUM(z.MoneyFirm) AS Firma
, 0 AS moneyAll
FROM [Zakaz] z
JOIN [Status] s ON s.ID_STATUS=z.ID_STATUS
WHERE 1=1
	AND	z.DateClose between @start AND @end
	AND s.ID_STATUS in (3,5,7)
	AND z.Povtor=1
	AND z.MoneyFirm = 0 
GROUP BY  z.Povtor

-------
UNION
-------


-- вывод ТОТАЛ значений
SELECT 
'TOTAL' AS NameStatus
, 1111 AS ID_STATUS
,count(*) AS Cnt
,0 AS procent
,AVG(z.MoneyFirm) AS avgFirmaMoney
,AVG(z.MoneyAll) AS avgAllMoney
FROM [Zakaz] z
JOIN [Status] s ON s.ID_STATUS=z.ID_STATUS
WHERE 1=1
	AND	z.DateClose between @start AND @end
	AND s.ID_STATUS in (3,5,7)


";
                       
            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);

//NameStatus ID_STATUS   Cnt Procent Firma moneyAll
//TOTAL   1111    310 0.0 1318    3222
//ВЫПОЛНЕН    5   199 64.2    407310  0
//ДИАГНОСТИКА 7   6   1.9 908 0
//ОТКАЗ   3   105 33.9    500 0
//ПОВТОР НЕ РАБОЧИЙ   88  15  78.9    0   0
//ПОВТОР РАБОЧИЙ  80  4   21.1    4675    0

            foreach (DataRow row in dt.Rows)
            {
                if ((long)row["ID_STATUS"] == 1111)// TOTAL
                {
                    TOTAL_all_zakaz =     (int)row["Cnt"];
                    TOTAL_avg_firma =     (int)row["Firma"];
                    TOTAL_avg_getClient = (int)row["moneyAll"];
                }
                if ((long)row["ID_STATUS"] == 5)// ВЫПОЛНЕН
                {
                    SUCCES_proc = (decimal)row["Procent"];
                    SUCCES_ed = (int)row["Cnt"];
                    SUCCES_summ = (int)row["Firma"];
                }
                if ((long)row["ID_STATUS"] == 7)// ДИАГНОСТИКА
                {

                    DIAGNOSTIK_proc = (decimal)row["Procent"];
                    DIAGNOSTIK_ed = (int)row["Cnt"];
                    DIAGNOSTIK_summ = (int)row["Firma"];
                }
                if ((long)row["ID_STATUS"] == 3)// ОТКАЗ
                {
                    DENY_proc= (decimal)row["Procent"];
                    DENY_ed = (int)row["Cnt"];
                }
                if ((long)row["ID_STATUS"] == 88)// ПОВТОР НЕ РАБОЧИЙ
                {
                    POVTORNOTmoney_proc = (decimal)row["Procent"];
                    POVTORNOTmoney_ed = (int)row["Cnt"];
                }
                if ((long)row["ID_STATUS"] == 80)// ПОВТОР РАБОЧИЙ
                {
                    POVTORmoney_proc = (decimal)row["Procent"];
                    POVTORmoney_ed = (int)row["Cnt"];
                    POVTORmoney_summ = (int)row["Firma"];
                }

            }

            DiagramCircle_Zakaz = new List<PointCircle>
            {
                new PointCircle(){ Y=SUCCES_proc, Xtitle="Выполнено"},
                new PointCircle(){ Y=DIAGNOSTIK_proc, Xtitle="Диагностика"},
                new PointCircle(){ Y=DENY_proc, Xtitle="Отказано"},
            };



        }
        private List<PointTime> GetDiagramLine_Firma(reportZakazFiltr Filtr)
        {
            List<PointTime> chart = new List<PointTime>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End}
            };
                                   

            #region sql

            string sqlText = @"
------------------------------------------------------
-- Вывод даты и кол-во выполненых заказов
--Группировка по дате, сумма заказов и их кол-во
SELECT
DATEPART(YEAR, z.DateClose) AS Year
 , DATEPART(MONTH, z.DateClose) AS Month
   , DATEPART(DAY, z.DateClose) AS Day
     , count(1) AS cntComplete
     , SUM(z.MoneyFirm) AS sumMoneyFirm
 FROM[Zakaz] z
WHERE z.ID_STATUS in (5)

    AND z.DateClose between @start AND @end
    AND z.MoneyFirm > 0
GROUP BY DATEPART(YEAR, z.DateClose),DATEPART(MONTH, z.DateClose),DATEPART(DAY, z.DateClose)
ORDER BY DATEPART(YEAR, z.DateClose) ASC
	,DATEPART(MONTH, z.DateClose) ASC
	,DATEPART(DAY, z.DateClose) ASC
";

            if (Filtr.GroupDate== GroupByDate.MONTH)
                sqlText = $@"
SELECT 
DATEPART(YEAR,z.DateClose) AS Year
,DATEPART(MONTH,z.DateClose) AS Month
,1 AS Day
, count(1) AS cntComplete
, SUM(z.MoneyFirm) AS sumMoneyFirm
 FROM [Zakaz] z
WHERE z.ID_STATUS in (5)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
GROUP BY DATEPART(YEAR,z.DateClose),DATEPART(MONTH,z.DateClose)  
ORDER BY DATEPART(YEAR,z.DateClose) ASC
	,DATEPART(MONTH,z.DateClose) ASC


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
                    Y = (int)row["cntComplete"]                    
                };


                chart.Add(point);
            }

            return chart;
        }
        private List<PointString> GetDiagramLine_City(reportZakazFiltr Filtr)
        {
            List<PointString> chart = new List<PointString>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End}
            };


            #region sql

            string sqlText = @"
------------------------------------------------------
-- кол-во выполненых заказов в разбивке по Организациям
SELECT 
o.NameOrg
, count(1) AS cntComplete
, SUM(z.MoneyFirm) AS sumMoneyFirm
 FROM [Zakaz] z
JOIN [Organization] o ON o.ID_ORGANIZATION=z.ID_ORGANIZATION
WHERE z.ID_STATUS in (5)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
GROUP BY o.NameOrg
ORDER BY count(1) DESC
";





            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {


                PointString point = new PointString
                {
                    X = (string)row["NameOrg"],
                    Y = (int)row["cntComplete"]
                };


                chart.Add(point);
            }

            return chart;
        }
        private List<PointString> GetDiagramLine_Organization(reportZakazFiltr Filtr)
        {
            List<PointString> chart = new List<PointString>();


            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter(@"start",SqlDbType.DateTime) { Value = Filtr.Start},
                new SqlParameter(@"end",SqlDbType.DateTime) { Value = Filtr.End}
            };


            #region sql

            string sqlText = @"
------------------------------------------------------
-- кол-во выполненых заказов в разбивке по Городам
SELECT 
z.City
, count(1) AS cntComplete
, SUM(z.MoneyFirm) AS sumMoneyFirm
 FROM [Zakaz] z
WHERE z.ID_STATUS in (5)
	AND z.DateClose between @start AND @end
	AND z.MoneyFirm > 0
GROUP BY z.City
ORDER BY count(1) DESC

";





            #endregion

            DataTable dt = new DataTable();// при наличии данных
            // получаем данные из запроса
            dt = ExecuteSqlGetDataTableStatic(sqlText, parameters);


            foreach (DataRow row in dt.Rows)
            {


                PointString point = new PointString
                {
                    X = (string)row["City"],
                    Y = (int)row["cntComplete"]
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


    }// end class


}
