using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace SoldCalc.UpdateWorker
{
    internal class UpdateClientBaseExcel
    {
    }

    public class UpdateApp
    {
        internal BackgroundWorker WorkerUpdate;
        public UpdateApp()
        {
            if (WorkerUpdate == null)
            {
                WorkerUpdate = new BackgroundWorker();
                WorkerUpdate.DoWork += PobierzPlik_do_aktual_winCalc_z_serwerFTP_DoWork;
                WorkerUpdate.ProgressChanged += WorkerUpdate_ProgressChanged;
                WorkerUpdate.RunWorkerCompleted += WorkerUpdate_RunWorkerCompleted;

            }
            WorkerUpdate.RunWorkerAsync();
        }



        public void PobierzPlik_do_aktual_winCalc_z_serwerFTP_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerUpdate.WorkerSupportsCancellation = true;
            WorkerUpdate.WorkerReportsProgress = true;

            if (!Directory.Exists(Folder_Matka_Programu_FilesSC))
                Directory.CreateDirectory(Folder_Matka_Programu_FilesSC);
            if (!Directory.Exists(Folder_Matka_Programu_Update))
                Directory.CreateDirectory(Folder_Matka_Programu_Update);
            if (!Directory.Exists(Folder_Matka_Programu_FilesSC_Update))
                Directory.CreateDirectory(Folder_Matka_Programu_FilesSC_Update);
            int serchUpdate = Serch_EXE_Update();
            if (serchUpdate < 15)
            {
                DownloadfileAsUpload(Folder_Matka_Programu, "Update", @"\Update.rar");
                WypakujNowyPlikRar_Zip(Folder_Matka_Programu, "Update.rar");
            }

            BackgroundWorker bw = sender as BackgroundWorker;
            if (e.Argument != null)
            {
                // int arg = (int)e.Argument;
            }

            string TxtName = "WinCaalcUpdate.txt";

            DownloadfileAsUpload(Folder_Matka_Programu_FilesSC_Update, "Update", @"\" + TxtName);
            string nametxt = RoadFileTxt_Line(Folder_Matka_Programu_FilesSC_Update + @"\" + TxtName);
            int dataFile = int.Parse(Regex.Replace(nametxt, @"\D", ""));
            int dateTitle = int.Parse(Regex.Replace(Mw.MWTitle, @"\D", ""));
            TxtName = "WinCalc" + dataFile + ".rar";

            // Console.WriteLine("Folder_Matka_Programu_FilesSC_Update {0} TxtName {1}", Folder_Matka_Programu_FilesSC_Update.ToString(), TxtName.ToString());
            // Console.WriteLine("TxtName = {0}   dateTitle {1}", TxtName, dateTitle);
            int a = 0;
            if (dataFile > dateTitle)
                a = DownloadfileAsUpload(Folder_Matka_Programu_FilesSC_Update.ToString(), "Update", @"\" + TxtName.ToString());
            //Console.WriteLine("WorkerUpdate " & String.Format("MainWindow Wiersz# {0}" & vbCrLf, (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()));
            e.Result = a;
            if (bw.CancellationPending)
                e.Cancel = true;
        }

        private void WorkerUpdate_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // lblStatus.Text = "Working... (" & e.ProgressPercentage & "%)"
        }

        private void WorkerUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Try
            if (e.Result.ToString() == "1")
            {
                Mw.UpdateMe.Visibility = Visibility.Visible;
            }
            else
            {
                Mw.UpdateMe.Visibility = Visibility.Collapsed;
            }

        }

        public int Serch_EXE_Update()
        {
            int i = 0;
            foreach (string file_path in Directory.GetFiles(Folder_Matka_Programu_Update))
                i += 1;
            return i;
        }

        private void WypakujNowyPlikRar_Zip(string WypakujDo, string PlikRar)
        {
            try
            {
                IArchive archive = ArchiveFactory.Open(PlikRar);
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        // Console.WriteLine(entry.Key & "Mainwindows " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
                        entry.WriteToDirectory(WypakujDo, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        public string RoadFileTxt_Line(string FileName)
        {
            try
            {
                string a = null;
                System.IO.StreamReader WczytajBr = new System.IO.StreamReader(FileName);
                while (!WczytajBr.EndOfStream == true)
                {
                    string[] splitLine = WczytajBr.ReadLine().Split('|');
                    a = splitLine[0];
                }
                WczytajBr.Close();
                return a;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }
    }

    public static class AktualBazaKlient
    //internal static partial class AktualBazaKlient
    {
        private static int LastRow;
        private static System.Data.DataTable DaneTbl;
        private static System.Data.DataTable dtTable = new System.Data.DataTable();
        internal static BackgroundWorker Background_AktualBazaKlient_Excel;

        static AktualBazaKlient()
        {
            if (Background_AktualBazaKlient_Excel == null)
            {
                Background_AktualBazaKlient_Excel = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
                Background_AktualBazaKlient_Excel.DoWork += Klient_Excel_DoWork;
                Background_AktualBazaKlient_Excel.RunWorkerCompleted += Klient_Excel_RunWorkerCompleted;
                Background_AktualBazaKlient_Excel.ProgressChanged += Klient_Excel_ProgressChanged;
            }
        }
        private static void Klient_Excel_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker bw = sender as BackgroundWorker;
                int arg = (int)e.Argument;

                e.Result = TimeConsumingOperation(bw);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private static void Klient_Excel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    MessageBox.Show("Operacja została anulowana");
                }
                else if (e.Error != null)
                {
                    string msg = string.Format("Wystąpił błąd: {0}", e.Error.Message);
                    MessageBox.Show(msg);
                }
                else
                {
                    string msg = "";
                    if (e.Result.ToString() == "1")
                    {
                        File.Copy(AktualFullPath, FullPath, true);
                        if (Acon.State == ConnectionState.Open)
                            Acon.Close();
                        msg = String.Format("Zakończono!");
                    }
                    if (e.Result.ToString() == "2")
                        msg = string.Format("Nieoczekiwany błąd!" + Microsoft.VisualBasic.Constants.vbCrLf + "Sprawdz czy wskazano własciwy plik");

                    if (!string.IsNullOrEmpty(msg))
                        MessageBox.Show(msg);
                }
                AktualNewBaza.ActivFunction = 0;
                if (Acon.State == ConnectionState.Open)
                    Acon.Close();
                if (con.State == ConnectionState.Open)
                    Dcon.Close();
                ConClose();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private static void Klient_Excel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {

                int _Stan = e.ProgressPercentage;

                lblTime.Content = _Stan + "%";
                LabIleZ.Content = AktualNewBaza.ileZ + "/" + "9";
                if (AktualNewBaza.ileZ == 20)
                {
                    lblTime.Content = "";
                    LabIleZ.Content = "Sendt";
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private static int TimeConsumingOperation(BackgroundWorker bw)
        {
            try
            {
                int result = 0;
                var rand = new Random();

                Tim = TimeAktual();
                while (!bw.CancellationPending)
                {
                    bool exit = false;
                    try
                    {
                        string a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13;
                        LastRow = DaneTbl.Rows.Count;
                        for (int i = 0, loopTo = DaneTbl.Rows.Count - 1; i <= loopTo; i++)
                        {
                            if (exit == true)
                                break;
                            {
                                var withBlock = DaneTbl.Rows[i];
                                a0 = withBlock["Opiekun_klienta"].ToString();
                                a1 = withBlock["NIP"].ToString();
                                a2 = withBlock["Stan"].ToString();
                                a3 = withBlock["Numer_konta"].ToString();
                                a4 = withBlock["Nazwa_klienta"].ToString();
                                a5 = withBlock["Nazwa_CD"].ToString();
                                a6 = withBlock["Adres"].ToString();
                                a7 = withBlock["Kod_poczta"].ToString();
                                a8 = withBlock["Poczta"].ToString();
                                a9 = withBlock["Forma_plac"].ToString();
                                a10 = withBlock["PraceList"].ToString();
                                a11 = withBlock["Branza"].ToString();
                                a12 = withBlock["Tel"].ToString();
                                a13 = withBlock["E_mail"].ToString();
                            }
                            if (Information.IsNumeric(a1))
                            {
                                if (a1.Length < 6)
                                    goto lastline;
                            }
                            else
                            {
                                goto lastline;
                            }
                            if (Information.IsNumeric(a3))
                            {
                                if (a3.Length < 6)
                                    goto lastline;
                            }
                            else
                            {
                                goto lastline;
                            }
                            if (Information.IsNumeric(a3) & Conversions.ToDouble(a3.Substring(0, Math.Min(a3.Length, 1))) == 1d | Information.IsNumeric(a3) & Conversions.ToDouble(a3.Substring(0, Math.Min(a3.Length, 1))) == 9d)
                            {
                                a0 = Strings.Replace(a0, ",", " ");
                                a1 = Strings.Replace(a1, " ", "");
                                a1 = Strings.Mid(a1, 1, 10).ToString();
                                a4 = Strings.Replace(a4, "'", "");
                                a5 = Strings.Replace(a5, "'", "");
                                a6 = Strings.Replace(a6, "'", "");
                                string sqlstring = @" -- Try to update any existing row
                                    UPDATE BazaKl
                                    SET opiekun_klienta = '" + a0 + "',Stan='" + a2 + "',Numer_konta='" + a3 + "',Nazwa_klienta='" + a4 + "',Nazwa_CD = '" + a5 + "',Adres='" + a6 + "',Kod_poczta='" + a7 + "',Poczta='" + a8 + @"',
                                            Forma_plac='" + a9 + "' , PraceList='" + a10 + "', Branza='" + a11 + "' , Tel='" + a12 + "' ,  E_mail='" + a13 + "' ,  OstAkt='" + Tim + @"' 
                                    WHERE  NIP like '%" + a1 + @"%' ;
                                
                                -- If no update happened (i.e. the row didn't exist) then insert one
                                    INSERT INTO BazaKl (Opiekun_klienta,NIP,Stan,Numer_konta,Nazwa_klienta,Nazwa_CD,Adres,Kod_poczta,Poczta,Forma_plac,PraceList,Branza,Tel,E_mail,OstAkt) 
                                    SELECT '" + a0 + "','" + a1 + "','" + a2 + "','" + a3 + "','" + a4 + "','" + a5 + "','" + a6 + "','" + a7 + "','" + a8 + "','" + a9 + "','" + a10 + "','" + a11 + "','" + a12 + "','" + a13 + "','" + Tim + @"'
                                WHERE (Select Changes() = 0);";
                                UsingSQLComand(sqlstring, Acon);
                            }

                        lastline:
                            ;


                            Background_AktualBazaKlient_Excel.ReportProgress(IntProgres(i, LastRow));

                        }
                        result = 1;
                        exit = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message).ToString();
                        result = 2;
                        exit = true;
                    }
                    if (exit)
                    {
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }
        public static System.Data.DataTable Import_Z_Pliku_Excel_to_Datatable(string filepath)
        {
            var dt = new System.Data.DataTable();
            string constring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
            var conE = new System.Data.OleDb.OleDbConnection(constring + "");
            try
            {
                var ds = new System.Data.DataTable();
                conE.Open();
                var myTableName = conE.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                string sqlquery = string.Format("SELECT * FROM [{0}]", myTableName);
                var da = new System.Data.OleDb.OleDbDataAdapter(sqlquery, conE);
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                if (conE.State == ConnectionState.Open)
                    conE.Close();
                MessageBox.Show(ex.Message).ToString();
                return dt;
            }
        }

        public static DataTable ImportToExCel(string textTitle)
        {
            var dt = new DataTable();
            var dialog = new System.Windows.Forms.OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx;*.xlsm", Multiselect = true, Title = textTitle };
            System.Windows.Forms.DialogResult Result;
            Result = dialog.ShowDialog();
            if (Result == System.Windows.Forms.DialogResult.OK)
            {
                dialog.DefaultExt = "txt";
                dt = Import_Z_Pliku_Excel_to_Datatable(dialog.FileName);
            }
            else if (Result == System.Windows.Forms.DialogResult.Cancel)
            {
                Console.WriteLine("Cancel");
                dt.TableName = "Cancel";
                dialog.Dispose();
            }
            return dt;
        }
        public static object AddBazaKLDoDTL()
        {
            try
            {
                Scie_KL = @"C:\Users\" + usher + @"\AppData\SC\Dne.cws";
                scie_user = @"C:\Users\" + usher + @"\AppData\SC\Usr.cws";
                string TextLine;
                int i = 0;
                DataRow wiersze;
                string[] wartosc;
                if (dtTable != null && dtTable.Rows.Count > 0)
                {
                    dtTable.Clear();
                }
                if (System.IO.Directory.Exists(Scie_KL))
                {
                    var objReader = new StreamReader(Scie_KL, Encoding.UTF8);

                    wartosc = objReader.ReadLine().Split('|');
                    try
                    {
                        {
                            ref var withBlock = ref dtTable;
                            withBlock.Columns.Add(new DataColumn("id"));
                            withBlock.Columns.Add(new DataColumn("NIP"));
                            withBlock.Columns.Add(new DataColumn("Stan"));
                            withBlock.Columns.Add(new DataColumn("Numer_konta"));
                            withBlock.Columns.Add(new DataColumn("Nazwa_klienta"));
                            withBlock.Columns.Add(new DataColumn("Nazwa_CD"));
                            withBlock.Columns.Add(new DataColumn("Adres"));
                            withBlock.Columns.Add(new DataColumn("Kod_Poczta"));
                            withBlock.Columns.Add(new DataColumn("Poczta"));
                            withBlock.Columns.Add(new DataColumn("Forma_plac"));
                            withBlock.Columns.Add(new DataColumn("PraceList"));
                            withBlock.Columns.Add(new DataColumn("Branza"));
                            withBlock.Columns.Add(new DataColumn("Tel"));
                            withBlock.Columns.Add(new DataColumn("E_mail"));
                            withBlock.Columns.Add(new DataColumn("Opiekun_klienta"));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message).ToString();
                    }
                    wiersze = dtTable.NewRow();
                    for (i = 0; i <= 13; i++)
                        wiersze[i] = wartosc[i].ToString();
                    dtTable.Rows.Add(wiersze);
                    i = 1;
                    while (objReader.Peek() != -1)
                    {
                        TextLine = "";
                        TextLine += objReader.ReadLine();
                        if (TextLine.Contains("|"))
                        {
                            string[] splitLine = TextLine.Split('|');
                            var dr = dtTable.NewRow();
                            for (int j = 0; j <= 13; j++)
                                dr[j] = splitLine[j];
                            dtTable.Rows.Add(dr);
                        }
                        ;
                        i += 1;
                    }

                    objReader.Close();
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(dtTable.Rows[0][1], "NIP", false)))
                        dtTable.Rows.Remove(dtTable.Rows[0]);
                }
                goto line2;
            line2:
                ;

                return dtTable;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }
        public static void PrzepiszDane()
        {
            try
            {
                string a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13;
                int _Stan = dtTable.Rows.Count;
                for (int i = 0, loopTo = dtTable.Rows.Count - 1; i <= loopTo; i++)
                {
                    {
                        var withBlock = dtTable.Rows[i];
                        a1 = withBlock["NIP"].ToString();
                        a2 = withBlock["Stan"].ToString();
                        a3 = withBlock["Numer_konta"].ToString();
                        a4 = withBlock["Nazwa_klienta"].ToString();
                        a5 = withBlock["Nazwa_CD"].ToString();
                        a6 = withBlock["Adres"].ToString();
                        a7 = withBlock["Kod_poczta"].ToString();
                        a8 = withBlock["Poczta"].ToString();
                        a9 = withBlock["Forma_plac"].ToString();
                        a10 = withBlock["PraceList"].ToString();
                        a11 = withBlock["Branza"].ToString();
                        a12 = withBlock["Tel"].ToString();
                        a13 = withBlock["E_mail"].ToString();
                    }
                    if (Information.IsNumeric(a3))
                    {
                        if (Conversions.ToDouble(a3.Substring(0, Math.Min(a3.Length, 1))) == 1d | Conversions.ToDouble(a3.Substring(0, Math.Min(a3.Length, 1))) == 9d)  // GoTo lastline
                        {
                            a1 = Strings.Replace(a1, " ", "");
                            a1 = Strings.Mid(a1, 1, 10).ToString();
                            a4 = Strings.Replace(a4, "'", "");
                            a5 = Strings.Replace(a5, "'", "");
                            a6 = Strings.Replace(a6, "'", "");
                            if (!string.IsNullOrEmpty(a1))
                            {
                                if (con.State == ConnectionState.Closed)
                                    con.Open();
                                if (Dcon.State == ConnectionState.Closed)
                                    Dcon.Open();
                                string sqlstring = @" -- Try to update any existing row
                                    UPDATE BazaKl
                                    SET opiekun_klienta = '" + Upr_User.User_PH + "',Stan='" + a2 + "',Numer_konta='" + a3 + "',Nazwa_klienta='" + a4 + "',Nazwa_CD = '" + a5 + "',Adres='" + a6 + "',Kod_poczta='" + a7 + "',Poczta='" + a8 + @"',
                                            Forma_plac='" + a9 + "' , PraceList='" + a10 + "', Branza='" + a11 + "' , Tel='" + a12 + "' ,  E_mail='" + a13 + "' ,  OstAkt='" + Tim + @"' 
                                    WHERE  NIP like '%" + a1 + @"%' ;
                                
                                -- If no update happened (i.e. the row didn't exist) then insert one
                                    INSERT INTO BazaKl (Opiekun_klienta,NIP,Stan,Numer_konta,Nazwa_klienta,Nazwa_CD,Adres,Kod_poczta,Poczta,Forma_plac,PraceList,Branza,Tel,E_mail,OstAkt) 
                                    SELECT '" + Upr_User.User_PH + "','" + a1 + "','" + a2 + "','" + a3 + "','" + a4 + "','" + a5 + "','" + a6 + "','" + a7 + "','" + a8 + "','" + a9 + "','" + a10 + "','" + a11 + "','" + a12 + "','" + a13 + "','" + Tim + @"'
                                WHERE (Select Changes() = 0);";
                                UsingSQLComand(sqlstring, Acon);
                            }
                        }
                    }
                    ;

                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
    }
}
