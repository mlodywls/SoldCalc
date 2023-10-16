using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Supporting
{

    public class MoreFunctionMW
    {
        public static BackgroundWorker BackgroundNewPage;// = new BackgroundWorker();

        public MoreFunctionMW()
        {
            Mw.DwMath = 0;
            if (BackgroundNewPage == null)
            {
                BackgroundNewPage = new BackgroundWorker();
                BackgroundNewPage.DoWork += BackgroundNewPage_DoWork;
                BackgroundNewPage.RunWorkerCompleted += BackgroundNewPage_RunWorkerCompleted;
                BackgroundNewPage.ProgressChanged += BackgroundNewPage_ProgressChanged;
            }
            BackgroundNewPage.RunWorkerAsync();
        }


        public void BackgroundNewPage_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundNewPage.WorkerSupportsCancellation = true;
            BackgroundNewPage.WorkerReportsProgress = true;

            BackgroundWorker bw = sender as BackgroundWorker;
            Mw.DwMath += 1;
            Console.WriteLine("MoreFunctionMW = " + Mw.DwMath);
            // e.Result = WczytajBazaKL() 'bw)
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }

            dowsise = null;
            if (Mw.NameIMG != null & Mw.NameImgstring == "PiZK11")
            {
                if (ListTblOfr.Tbl_Add_prodList.Count > 0)
                {
                    e.Result = 1;
                    GenerateZK11.GeneraeExce();
                    //return;
                }
            }
            if (URLstatus == true)
            {
                if (!string.IsNullOrEmpty(Mw.NameLabel)) // && Mw.DwMath < 2)
                {
                    Console.WriteLine("Start MoreFunctionMW = " + Mw.DwMath);
                    if (Mw.NameLabel == "AktualPH")
                    {
                        Console.WriteLine(LocatiAktual + @"\" + "DB_Klient.db");
                        e.Result = 2;                  // BackgroundNewPage.ReportProgress(Pobierz_baze_DB_FTP("DB_Klient.db", LocatiAktual, BackgroundNewPage) ? 0 : 1, Pobierz_baze_DB_FTP("DB_ZAKUPY.db", LocatiAktual, default));
                        Pobierz_baze_DB_FTP("DB_Klient.db", LocatiAktual, BackgroundNewPage);
                        Pobierz_baze_DB_FTP("DB_ZAKUPY.db", LocatiAktual, BackgroundNewPage);
                    }
                    if (Mw.NameLabel == "AktCen")
                    {
                        Console.WriteLine(LocatiAktual + @"\" + "DB_Cennik.db");
                        e.Result = 3;// BackgroundNewPage.ReportProgress(Pobierz_baze_DB_FTP("DB_Cennik.db", LocatiAktual, BackgroundNewPage) ? 0 : 1);
                        Pobierz_baze_DB_FTP("DB_Cennik.db", LocatiAktual, BackgroundNewPage);
                    }
                    if (Mw.NameLabel == "AktZakup")
                    {
                        Console.WriteLine(LocatiAktual + @"\" + "DB_ZAKUPY.db");
                        e.Result = 4; //BackgroundNewPage.ReportProgress(Pobierz_baze_DB_FTP("DB_ZAKUPY.db", LocatiAktual, BackgroundNewPage) ? 0 : 1);
                        Pobierz_baze_DB_FTP("DB_ZAKUPY.db", LocatiAktual, BackgroundNewPage);
                    }
                    if (Mw.NameLabel == "AktBazaCennik")
                    {
                        Console.WriteLine(LocatiAktual + @"\" + "DB_Cennik.db");
                        e.Result = 5;// BackgroundNewPage.ReportProgress(Pobierz_baze_DB_FTP("DB_Cennik.db", LocatiAktual, BackgroundNewPage) ? 0 : 1);
                        Pobierz_baze_DB_FTP("DB_Cennik.db", LocatiAktual, BackgroundNewPage);
                    }
                    if (Mw.NameLabel == "DowCen")
                    {
                        e.Result = 6; Console.WriteLine("NameLabel == DowCen");
                        BackgroundNewPage.ReportProgress(Generuj_Cennik_to_excel(BackgroundNewPage));
                    }
                    if (Mw.NameLabel == "Add_Klient_To_Exel")
                    {
                        Console.WriteLine(LocatiAktual + @"\" + "DB_Klient.db");
                        e.Result = 7;//  BackgroundNewPage.ReportProgress(Pobierz_baze_DB_FTP("DB_Klient.db", LocatiAktual, BackgroundNewPage) ? 0 : 1);
                        Pobierz_baze_DB_FTP("DB_Klient.db", LocatiAktual, BackgroundNewPage);
                    }
                    // Console.WriteLine("End MoreFunctionMW = " + Mw.DwMath);
                }
                else if  (Mw.NameIMG != null & Mw.NameImgstring == "PiZK11")
                {
                    Mw.VievPageVisibli(false, false, "");
                }
            }
            else
            {
                Interaction.MsgBox("brak połaczenia z internetem" + Constants.vbCrLf + " Sprawdz połączenie!");
                // Console.WriteLine(vbCrLf & vbCrLf & "brak połaczenia z internetem" & vbCrLf & " Sprawdz połączenie!" & vbCrLf & vbCrLf)
            }
            return;


        } // ===========================================================================================================================|
        private void BackgroundNewPage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result.ToString() == "1")
            {
                LiczOfr.WyswietlAllZK();
            }
            try
            {
                if (Mw.NameLabel != "")
                {
                    if (Mw.NameLabel == "AktualPH")
                    {
                        Mw.Main.Content = new RejonPH();
                        Mw.VievPageVisibli(false, false, "");
                    }
                    if (Mw.NameLabel == "AktBazaCennik")
                    {
                        Mw.Main.Content = new Aktualizuj_Cennik();
                        Mw.VievPageVisibli(false, false, "");
                    }
                    if (Mw.NameLabel == "AktCen")
                    {

                        Aktual_Cennik_Z_Plik_Excel.ADaneCN("DB_Cennik");
                    }

                    if (Mw.NameLabel == "AktZakup")
                    {
                        Aktual_Baza_Zakupy_Z_Excel.ADaneBazaZKP();

                    }
                    if (Mw.NameLabel == "Add_Klient_To_Exel")
                        Aktual_Cennik_Z_Plik_Excel.ADaneCN("DB_Klient");
                    if (Mw.NameLabel == "DowCen" || Mw.NameLabel == "AktBazaCennik")
                    {
                        Mw.VievPageVisibli(false, false, "");
                    }
                }
                else if (Mw.NameImgstring == "PiZK11")
                    Mw.VievPageVisibli(false, false, "");
                else
                {
                    Mw.VievPageVisibli(false, false, "");
                }


                Mw.NameLab = default;
                Mw.NameIMG = default;
                Mw.NameLabString = "";
                Mw.NameImgstring = "";
                Mw.ComboPlacText = "";
                Mw.TIleDniText = "";
                Mw.NameLabString = "";
                Mw.NameLabel = "";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }

        } // ==================================================================================================|
        private void BackgroundNewPage_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (Dock_Aktual_Progre.Visibility == Visibility.Visible)
                { Mw.VievPageVisibli(true, false, ""); }

                int _Stan = e.ProgressPercentage;

                lblTime.Content = _Stan + "%";
                Dock_Aktual_LabProgr.Value = _Stan;
                labelProgres.Content = _Stan + "%";
                if (AktualNewBaza.ileZ == 20)
                {
                    lblTime.Content = "";
                    LabIleZ.Content = "Downl..";
                }
                try
                {
                    InfoLabelKryj.Content = "Download file";
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }





        public int Generuj_Cennik_to_excel(BackgroundWorker bg)
        {

            if (Upr_User.Ranga == "KO" || Upr_User.Rejon == "Admin")
            {
                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = Missing.Value;
                string savePath = "";
                var saveFileD = new SaveFileDialog();
                saveFileD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileD.RestoreDirectory = true;
                saveFileD.Title = "Zapisz plik przypisując mu nazwę - Wprowadź nazwę, lub wybierz z listy plików";
                saveFileD.Filter = "Excel XLS Files(*.xls)|*.xls";
                saveFileD.FilterIndex = 1;

                if (saveFileD.ShowDialog() == true)
                {
                    savePath = saveFileD.FileName.ToString();
                }
                if (string.IsNullOrEmpty(savePath))
                {
                    return default;
                }
                //Console.WriteLine("savePath = {0} saveFileD = {1} misValue = {2}", savePath, saveFileD, misValue);
                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = xlWorkBook.Sheets[1];

                xlWorkSheet.Cells[1, 1] = "SAP";
                xlWorkSheet.Cells[1, 2] = "Naza Prod.";
                xlWorkSheet.Cells[1, 3] = "CDM";
                xlWorkSheet.Cells[1, 4] = "KO";
                xlWorkSheet.Cells[1, 5] = "PH";
                xlWorkSheet.Cells[1, 6] = "ZRP0";
                xlWorkSheet.Cells[1, 7] = "Brak Prace List";

                int j = 2;
                foreach (var itm in Mw.ListCennik)
                {
                    xlWorkSheet.Cells[j, 1] = itm.SAP;
                    xlWorkSheet.Cells[j, 2] = itm.NazwProd;
                    double.TryParse(itm.CDM.ToString(), out double Outdob);
                    xlWorkSheet.Cells[j, 3] = Outdob;
                    double.TryParse(itm.CK.ToString(), out double Outdob1);
                    xlWorkSheet.Cells[j, 4] = Outdob1;
                    double.TryParse(itm.PH.ToString(), out double Outdob2);
                    xlWorkSheet.Cells[j, 5] = Outdob2;
                    double.TryParse(itm.ZPR0.ToString(), out double Outdob3);
                    xlWorkSheet.Cells[j, 6] = Outdob3;
                    xlWorkSheet.Cells[j, 7] = itm.BrakPrace;
                    j += 1;

                    try
                    {
                        if (bg != null)
                            bg.ReportProgress(IntProgres(j, Mw.ListCennik.Count));
                    }
                    catch (Exception ex)
                    {

                    }
                }
                xlWorkSheet.Columns["A:O"].AutoFit();
                if (savePath != null && !string.IsNullOrEmpty(savePath.Trim()))
                {
                    try
                    {
                        xlWorkBook.SaveAs(savePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    }
                    catch
                    {

                        MessageBox.Show("Plik o takiej nazwie istnieje i jest w tej chwili otwarty!!! " + Constants.vbCrLf + "Wprowadź inną nazwę pliku i zapisz ponownie!");
                    }

                }
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                ReleaseObject(xlWorkSheet);
                ReleaseObject(xlWorkBook);
                ReleaseObject(xlApp);
                if (File.Exists(savePath))
                    Process.Start("explorer.exe", savePath);
            }
            return default;
        }

        public static bool ReleaseObject(object obj)
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
            return default(Boolean);
        }
    }


    public static class Worker_licz_czas
    {
        private static BackgroundWorker _worker;

        internal static BackgroundWorker worker
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _worker;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_worker != null)
                {
                }

                _worker = value;
                if (_worker != null)
                {
                }
            }
        }

        public static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            worker.WorkerReportsProgress = true; worker.WorkerSupportsCancellation = true;
            try
            {
                for (int i = 0; i <= 100; i++)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }
                    worker.ReportProgress(i);
                    System.Threading.Thread.Sleep(250);
                }
                e.Result = 42;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private static void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
        private static void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
    }


}
