using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Supporting
{
    //    internal class GenerateZK11
    //    {
    //    }
    public static class GenerateZK11
    {
        private static string a0, a1, a2;
        private static string strFilename = "";
        private readonly static string fileXexe;

        public static void GeneraeExce()
        {
            //try
            //{
            Console.WriteLine("GeneraeExce");
            string a3, a4, a5, a6, a7, a8;
            a1 = Get_KlientDane.NIP;
            if (Folder_Matka_Programu.ToString().Length < 3)
            {
                Interaction.MsgBox("wrs 19 - błąd"); return;
            }
            object Excel = Interaction.CreateObject("Excel.Application");
            string strPath = "ZK11 -" + Get_KlientDane.Numer_konta + ".xls";
            strFilename = System.IO.Path.Combine(Scieżka_Pliku_AppData_FilesSC, strPath);
            foreach (var row in ListTblOfr.Tbl_Add_prodList)
            {
                Console.WriteLine(" row.ZK11A1 = " + row.ZK11A1);
                if (row.ZK11A1 >= 0)
                    goto line1;
            }

            Interaction.MsgBox(" Do wygenerowania ZK11 konieczne jest wprowadzenie ceny niższej od ceny PriceList! "); return;
        line1:
            ;
            Microsoft.Office.Interop.Excel.Application XlApp = new Microsoft.Office.Interop.Excel.Application();
            if (XlApp == null)
            {
                Interaction.MsgBox("Wygląda na to, że program Excel nie jest zainstalowany na tym komputerze." + Microsoft.VisualBasic.Constants.vbCrLf + "Ta operacja wymaga zainstalowania MS Excel na tym komputerze." + Microsoft.VisualBasic.Constants.vbCrLf, MsgBoxStyle.Critical);
                return;
            }
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = XlApp.Workbooks.Add(misValue);
            xlWorkSheet = xlWorkBook.Sheets[1];
            int intI = 1;
            {
                var withBlock = xlWorkSheet;
                withBlock.Cells[1, 1].value = "nr klienta"; // "Nagłówek pliku Excel”
                withBlock.Cells[1, 2].value = Get_KlientDane.Numer_konta; // DTwybKlient.Rows(0)(4).ToString  '  Cennik_Add.T3.Text
                withBlock.Cells[1, 5].value = Get_KlientDane.Nazwa_klienta; // DTwybKlient.Rows(0)(5).ToString & " " & DTwybKlient.Rows(0)(6).ToString
                withBlock.Cells[3, 1].value = "Rabat ZPR-0";
                withBlock.Cells[3, 2].value = Get_KlientDane.Rabat_Double + "%";
                withBlock.Cells[5, 1].value = "NR SAP prod.";
                withBlock.Cells[5, 2].value = "ZK 11";
                withBlock.Cells[1, 1].EntireRow.Font.Bold = true;
                withBlock.Columns["A:F"].AutoFit();
                withBlock.Columns["A:F"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                intI = 6;
                double Zk11; double Zk12; double Zk13; string szt2 = ""; string szt3 = ""; string OldSZ2 = ""; string OldSZ3 = "";
                foreach (var row in ListTblOfr.Tbl_Add_prodList)
                {
                    if (row.szt2 != null || row.szt2 != "")
                        szt2 = row.szt2;
                    else
                        szt2 = "";
                    if (row.szt3 != null || row.szt3 != "")
                        szt3 = row.szt3;
                    else
                        szt3 = "";
                    if (row.ZK11A1 >= 0)
                    {
                        // Console.WriteLine(row.ZK11A1.ToString)
                        if (row.ZK11A1 >= 0)
                            Zk11 = row.ZK11A1;
                        else
                            goto LineNextintk;
                        if (row.ZK11A2 >= 0)
                            Zk12 = row.ZK11A2;
                        else
                            Zk12 = 0;
                        if (row.ZK11A3 >= 0)
                            Zk13 = row.ZK11A3;
                        else
                            Zk13 = 0;

                        if (Zk12 != 0 || Zk13 != 0)
                        {
                            withBlock.Range["C:C"].ColumnWidth = 20;
                            withBlock.Range["D:D"].ColumnWidth = 20;
                            withBlock.Cells[5, 3].value = "1 - Kaskadowo ZK 11";
                            withBlock.Cells[5, 4].value = "2 - Kaskadowo ZK 11";
                            if (OldSZ2 != szt2 && szt2 != "" || OldSZ3 != szt3 && szt3 != "")
                            {
                                withBlock.Cells[intI, 3].Value = "Powyżej " + szt2 + " Szt."; OldSZ2 = szt2;
                                withBlock.Cells[intI, 4].Value = "Powyżej " + szt3 + " Szt."; OldSZ3 = szt3;
                                intI += 1;
                            }
                        }
                        withBlock.Cells[intI, 1].Value = row.SAP;
                        withBlock.Cells[intI, 2].Value = Strings.Replace(Math.Round(Zk11, 2, MidpointRounding.AwayFromZero).ToString(), "-", "");
                        if (Zk12 != 0)
                        {
                            withBlock.Cells[intI, 3].Value = Strings.Replace(Math.Round(Zk12, 2, MidpointRounding.AwayFromZero).ToString(), "-", ""); szt2 = row.szt2;
                        }
                        if (Zk13 != 0)
                        {
                            withBlock.Cells[intI, 4].Value = Strings.Replace(Math.Round(Zk13, 2, MidpointRounding.AwayFromZero).ToString(), "-", ""); szt3 = row.szt3;
                        }
                        intI += 1;
                        a0 = row.ID.ToString(); a2 = row.SAP;
                        if (Information.IsDBNull(row.ZK11A1) || row.ZK11A1.ToString().Trim() == "")
                        {
                            a3 = "0"; a6 = "";
                        }
                        else
                        {
                            a3 = Strings.Replace(row.ZK11A1.ToString(), "-", ""); a6 = row.szt1;
                        }
                        if (Information.IsDBNull(row.ZK11A2) || row.ZK11A2.ToString().Trim() == "")
                        {
                            a4 = "0"; a7 = "";
                        }
                        else
                        {
                            a4 = Strings.Replace(row.ZK11A2.ToString(), "-", ""); a7 = row.szt2;
                        }
                        if (Information.IsDBNull(row.ZK11A3) || row.ZK11A3.ToString().Trim() == "")
                        {
                            a5 = "0"; a8 = "";
                        }
                        else
                        {
                            a5 = Strings.Replace(row.ZK11A3.ToString(), "-", ""); a8 = row.szt3;
                        }
                        a3 = Math.Round(Convert.ToDouble(a3), 2, MidpointRounding.AwayFromZero).ToString(); a4 = Math.Round(Convert.ToDouble(a4), 2, MidpointRounding.AwayFromZero).ToString(); a5 = Math.Round(Convert.ToDouble(a5), 2, MidpointRounding.AwayFromZero).ToString();
                        UsingSQLComand(StringComand.returnComandUpdateZK11(a1, a2, a3, a4, a5, a6, a7, a8), con);
                    LineNextintk:
                        ;
                    }
                }
            }
            xlWorkBook.SaveAs(strFilename.ToString(), Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            XlApp.Quit();

            ReleaseObject(xlWorkSheet); ReleaseObject(xlWorkBook); ReleaseObject(XlApp);
            //  Lastline:
            ;
            if (con.State == ConnectionState.Open)
                con.Close();
            Process[] procEx = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            string Title;
            foreach (Process ie in procEx)
            {
                Title = ie.MainWindowTitle.ToString();
                if (Title.Length <= 2)
                    ie.Kill();
            }

                //  lastline1:
                ;
            if (strFilename != "")
                OpenE_mail(strFilename);
            MainWindow.LiczOfr.WyswietlAllZK();
            MainWindow.LiczOfr.serchListVisible();
            //}
            //catch
            //{
            //}
        }

        public static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }
        // Microsoft.Office.Interop.Outlook;
        // Application = Microsoft.Office.Interop.Outlook.Application;




        public static void OpenE_mail(string strFilename)
        {
            string body = "Proszę o akceptację i podpięcie ZK11 - " + Get_KlientDane.Numer_konta + " - " + Get_KlientDane.Nazwa_klienta + Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbNewLine + Microsoft.VisualBasic.Constants.vbNewLine;
            string subject = "ZK11 -" + Get_KlientDane.Numer_konta;
            //string ccTo = "";
            string emailTo = Upr_User.KO_email;
            string file = Path.GetFullPath(strFilename);
            GenerateEmail(emailTo, "", "", file, subject, body);
            if (File.Exists(file))
                File.Delete(file);
        }
    }
}
