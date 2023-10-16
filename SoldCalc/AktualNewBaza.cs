using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
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
using System.Windows;
using System.Windows.Documents;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
//using Microsoft.Office.Interop.Excel;

namespace SoldCalc
{

    internal static partial class AktualNewBaza
    {
        internal static BackgroundWorker BC_Aktual_baza;
        private static int LastRow;
        private static string Tim;
        public static bool SendBazaAktual = true;
        public static int ActivFunction;
        public static int ileZ;
        public static DataTable NewBazaDownload = new DataTable();
        public static string SendBaza;
        public static string LadDzailanie;
        public static string LDzialanie;
        public static int DzialanieDoLabel;
        public static string SendFileName;
        public static string ZapisWys;
       // private static BackgroundWorker Backworker = default;
        private static int errUdate = 0;

        static AktualNewBaza()
        {
            if (BC_Aktual_baza == null)
            {
                BC_Aktual_baza = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
                BC_Aktual_baza.DoWork += BG_Aktualizuj_DoWork;
                BC_Aktual_baza.RunWorkerCompleted += BG_Aktualizuj_RunWorkerCompleted;
                BC_Aktual_baza.ProgressChanged += BG_Aktualizuj_ProgressChanged;
            }
        }
        public static void AktualizujBazaDanych_PH1()
        {
            try
            {
                ConOpen();
                File.Copy(FullPath, AktualFullPath, true);
                LabNazawaAktua.Content = "Pobieram";
                LabProgre.Content = "";
                lblTime.Content = "Pob.";
                LabIleZ.Content = "";
                LabInfoSen.Content = "";
                AktualBazaProgre.Value = 0;
                BC_Aktual_baza.RunWorkerAsync(2000);
                WyslProcent.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public static void BG_Aktualizuj_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (BlokujAktual == false)
                    return;
                ConOpen();
                BackgroundWorker bw = sender as BackgroundWorker;
                int arg = (int)e.Argument;
                e.Result = Aktualizacja_bazy_dane(bw, arg);
                if (bw.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString() + " działanie - " + ileZ);
            }
        }
      
        private static void BG_Aktualizuj_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ConClose();
                // Console.WriteLine("Aktualizacja zakończona z wynikiem nr {0} Stwierdzono błąd {1}", e.Result, e.[Error])
                if (e.Result is null)
                {
                    // TextMessage("Aktualizacja Zakończono z błędem. Zatrzymana została na działaniu " & ileZ) '  MsgBox("Zakończono z błędem")
                    // errUdate += 1
                }
                else if (e.Cancelled)
                {
                    MessageBox.Show("Operacja została anulowana");
                }
                //else if (e.Error is null)
                //{
                //    string msg = string.Format("Wystąpił błąd: {0}", e.Error.Message);
                //    MessageBox.Show(msg);
                //}
                else
                {
                    string msg; // = ""
                    if (e.Result.ToString() == "1")
                    {
                        ConClose();
                        LoadingData.Start_Panel_Czeka.Visibility = Visibility.Visible;
                        try
                        {
                            Mw.Panel_Pierwsze_Logowanie.Margin = new Thickness(0, 0, 0, 0);
                            Mw.Panel_Pierwsze_Logowanie.Children.Add(new LoadingData());
                            Mw.Panel_Pierwsze_Logowanie.Visibility = Visibility.Visible;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    if (e.Result.ToString() == "2")
                    {
                        msg = string.Format("Nieoczekiwany błąd!" + Constants.vbCrLf + "Sprawdz czy wskazano własciwy plik");
                    }
                    if (e.Result.ToString() == "3")
                    {
                        ActivFunction = 0;
                        try
                        {
                            MainWindow.WyslProcent.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            MainWindow.WyslProcent.Visibility = Visibility.Collapsed;
            ActivFunction = 0;
            ConClose();
            MainWindow.lblTime.Content = "";
            MainWindow.LabIleZ.Content = "";
            MainWindow.LabProgre.Content = "";
            MainWindow.LabInfoSen.Content = "";
            MainWindow.AktualBazaProgre.Value = 0;
            MainWindow.StAktua.Width = 35;
        }
        private static void BG_Aktualizuj_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int _Stan = e.ProgressPercentage;
                MainWindow.lblTime.Content = _Stan + "%";
                MainWindow.LabIleZ.Content = ileZ + "/" + "10";
                MainWindow.LabProgre.Content = _Stan + "%";
                MainWindow.AktualBazaProgre.Value = _Stan;
                MainWindow.LabNazawaAktua.Content = LadDzailanie;
                if (LDzialanie == "1")
                    MainWindow.LabNazawaDzialani.Content = "pobieram bazę" + Constants.vbCrLf + "liczba wierszy " + LastRow;
                if (LDzialanie == "2")
                    MainWindow.LabNazawaDzialani.Content = "wysyłam bazę" + Constants.vbCrLf + "liczba wierszy " + LastRow;
                if (LDzialanie == "3")
                    MainWindow.LabNazawaDzialani.Content = "pobieram z serwer" + Constants.vbCrLf + "liczba plików " + e.ProgressPercentage.ToString();
                if (LDzialanie == "1")
                    MainWindow.LabNazawaDzialani.Content = "pobieram bazę";

                if (ileZ == 20)
                {
                    MainWindow.lblTime.Content = "";
                    MainWindow.LabIleZ.Content = "Send";
                }
            }
            catch (Exception ex)
            {
                TextMessage(" e progres " + e.ProgressPercentage.ToString() + " Lastrow  " + LastRow + ex.StackTrace.ToString());
            }
        }
        private static int Aktualizacja_bazy_dane(BackgroundWorker bw, int sleepPeriod)
        {
            int result = 0;
            // Console.WriteLine("Start - Aktualizacja_bazy_dane")
            var rand = new Random();
            int PRaport;
            string SqweryDelete = "delete FROM Cennik";
            string SqweryUpdate = "SELECT * FROM Cennik";
            bool AktualCennik = false;
            bool AktualZakupu = false;
            Tim = TimeAktual();
            ileZ = 1;
            //Wolkrer(1);
            while (!bw.CancellationPending)
            {
                Console.WriteLine("Pobierz wejsciowe {0}", 1);
                string SerchDataCennik = SqlRoader_Jedna_wartosc("SELECT Max(OstAkt) FROM Cennik Order By OstAkt desc limit 1;", Acon);
                string Data_Aktual_Cennik = Sprawdz_Data_BazaDane_FTP("DB_Cennik.db", LocatiAktual);// Console.WriteLine(" SerchDataCennik - " + SerchDataCennik + " /  Data_Aktual_Cennik - " + Data_Aktual_Cennik);
                Console.WriteLine("Pobierz wejsciowe {0}", 2);
                string SerchDataZakupy = SqlRoader_Jedna_wartosc("SELECT Max(OstAkt) FROM BazaZKP Order By OstAkt desc limit 1", Acon);
                string Data_Aktual_Zakupy = Sprawdz_Data_BazaDane_FTP("DB_ZAKUPY.db", LocatiAktual);//   Console.WriteLine(" SerchDataZakupy - " + SerchDataZakupy + " /  Data_Aktual_Zakupy - " + Data_Aktual_Zakupy);
                if ((SerchDataCennik ?? "") != (Data_Aktual_Cennik ?? "") || string.IsNullOrEmpty(SerchDataCennik))
                    AktualCennik = true;
                if ((SerchDataZakupy ?? "") != (Data_Aktual_Zakupy ?? "") || string.IsNullOrEmpty(SerchDataZakupy))
                    AktualZakupu = true;
                //  Console.WriteLine(" SerchDataZakupy - " + SerchDataZakupy + " /  Data_Aktual_Zakupy - " + Data_Aktual_Zakupy);
                LDzialanie = "4"; 
                if (URLstatus == true)
                {
                    
                    LadDzailanie = "Pobieram Baza Klientów"; Console.WriteLine("Pobierz wejsciowe {0}", 3);
                    Pobierz_baze_DB_FTP("DB_Klient.db", LocatiAktual, BC_Aktual_baza);                   
                    if (AktualZakupu == true)
                    {
                        LadDzailanie = "Pobieram Baza Zakupów"; Console.WriteLine("Pobierz zakupy {0}", 4);
                        Pobierz_baze_DB_FTP("DB_ZAKUPY.db", LocatiAktual, BC_Aktual_baza);
                    }                  
                    if (AktualCennik == true)
                    {
                        LadDzailanie = "Pobieram Baza Cennik"; Console.WriteLine("Pobierz Cennik {0}", 5);
                        Pobierz_baze_DB_FTP("DB_Cennik.db", LocatiAktual, BC_Aktual_baza);
                    }               
                    LadDzailanie = "Pobieram Baza ZK11"; Console.WriteLine("Pobierz wejsciowe {0}", 6);
                    Pobierz_baze_DB_FTP("DB_ZK.db", LocatiAktual, BC_Aktual_baza);
         
                    LadDzailanie = "Pobieram Baza Oferty"; Console.WriteLine("Pobierz wejsciowe {0}", 7);
                    Pobierz_baze_DB_FTP("DB_OFR.db", LocatiAktual, BC_Aktual_baza);
                }

                else
                {
                    Interaction.MsgBox("brak połaczenia z internetem" + Constants.vbCrLf + " Sprawdz połączenie!");
                    return 2;
                }

                Console.WriteLine("Działanie wejsciowe {0}", 1);
                Dcon.ConnectionString = ConectString("DB_Klient", Dcon);
                Console.WriteLine("Działanie wejsciowe {0}", 2);
                string NewData = SqlRoader_Jedna_wartosc("SELECT max(OstAkt *1) FROM BazaKl;", Acon);
                Console.WriteLine("Działanie wejsciowe {0}", 3);
                string Olddata = SqlRoader_Jedna_wartosc("SELECT max(OstAkt *1) FROM BazaKl;", Dcon);

                PRaport = 1;


                ileZ = 1;  Console.WriteLine("Działanie {0}", ileZ);
                LadDzailanie = "Aktualizuj baza klient";
                result = BzaKlient_Aktual_NewBaza(PRaport, Dcon, Acon, BC_Aktual_baza);

                ileZ = 2;  Console.WriteLine("Działanie {0}", ileZ);
                LadDzailanie = "Aktualizuj baza branża";
                result = KO_Branżysta_Aktual_NewBaza(PRaport, Dcon, Acon);
                SendBaza = "DB_Klient";
                //Backworker.RunWorkerAsync();
                SendBackGrnd.UpdateBackgroundSend_NewBaza("DB_Klient.db", LocatiAktual + @"\DB_Klient.db");

                ileZ = 3;  Console.WriteLine("Działanie {0}", ileZ);
                LadDzailanie = "Aktualizuj baza zakupy";
                if (AktualZakupu == true)
                {
                    Dcon.ConnectionString = ConectString("DB_ZAKUPY", Dcon);
                    result = Baza_Zakupy_Aktual_NewBaza(Data_Aktual_Zakupy);
                }

                ileZ = 4;  Console.WriteLine("Działanie {0}", ileZ);
                LadDzailanie = "Aktualizuj baza cennik";
                if (AktualCennik == true)
                {
                    Dcon.ConnectionString = ConectString("DB_Cennik", Dcon);
                    result = Exportuj_Importuj_CSV(PRaport, ileZ, SqweryDelete, SqweryUpdate, Data_Aktual_Cennik);
                    if (Dcon.State == ConnectionState.Open)
                        Dcon.Close();
                }

                ileZ = 5; Console.WriteLine("Działanie {0}", ileZ);
                LadDzailanie = "Aktualizuj baza ZK11";
                Dcon.ConnectionString = ConectString("DB_ZK", Dcon);
                LDzialanie = "1";
                result = ZK11_SubAktualizuj_NewBaza(PRaport, Acon, Dcon);
                LDzialanie = "2";
                result = ZK11_SubAktualizuj_NewBaza(PRaport, Dcon, Acon);
                ConClose();
                SendBaza = "DB_ZK";
                //Backworker.RunWorkerAsync();
                SendBackGrnd.UpdateBackgroundSend_NewBaza("DB_ZK.db", LocatiAktual + @"\DB_ZK.db");
                try
                {
                    ileZ = 6;  Console.WriteLine("Działanie {0}", ileZ);
                    LadDzailanie = "Aktualizuj baza oferty";
                    Dcon.ConnectionString = ConectString("DB_OFR", Dcon);
                    LDzialanie = "1";
                    result = Zapisane_Oferty_Aktual_NewBaza(PRaport, Acon, Dcon, "Z a do d");
                    LDzialanie = "2";
                    result = Zapisane_Oferty_Aktual_NewBaza(PRaport, Dcon, Acon, "z d do a");
                    ConClose();
                    SendBaza = "DB_OFR";
                    //  Backworker.RunWorkerAsync();
                    //System.Windows.Forms.MessageBox.Show("nie wysyłam").ToString();
                    SendBackGrnd.UpdateBackgroundSend_NewBaza("DB_OFR.db", LocatiAktual + @"\DB_OFR.db");
                }
                catch (Exception ex)
                {

                }
                try
                {
                    ileZ = 7;  Console.WriteLine("Działanie {0}", ileZ);
                    LadDzailanie = "Aktualizuj oferty PDF";
                    DataTable DT_OFR = PobierzPlik_IMG_PDF_Z_NEW_BazaFTP(Strim_URL, "BazaOfr/", BC_Aktual_baza);
                    AktualPlik_OFR_PDF_NewBaza(DT_OFR);
                    if (Dcon.State == ConnectionState.Open)
                        Dcon.Close();
                }
                catch (Exception ex)
                {

                }
                try
                {
                    ileZ = 8;   Console.WriteLine("Działanie {0}", ileZ);
                    LadDzailanie = "Aktualizuj zdjęcia";
                    result = WczytajDaneZBazaFTP_Plik_PDF_Plik_IMG_NewBaza(Acon);
                    if (Dcon.State == ConnectionState.Open)
                        Dcon.Close();
                    if (Acon.State == ConnectionState.Open)
                        Acon.Close();
                }
                catch (Exception ex)
                {

                }

                ileZ = 20;
                ConClose();
                File.Copy(AktualFullPath, FullPath, true);

                return 1;
            }
            return 1;
        }
        public static int BzaKlient_Aktual_NewBaza(int ProgRaport, SQLiteConnection ZDcon, SQLiteConnection ZAcon, BackgroundWorker BackGrund)
        {
            // Console.WriteLine("AktualNewBaza - Działanie nr - " & ileZ & " / " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
            LDzialanie = "  ";

            int result = 1;
            var stopwatch = new Stopwatch();
            string AserchQwery;
            string DserchQwery;
            stopwatch.Start();
            Tim = TimeAktual();
            if (Dcon.State == ConnectionState.Closed)
                Dcon.Open();
            if (Acon.State == ConnectionState.Closed)
                Acon.Open();
            AserchQwery = "SELECT max(OstAkt *1) FROM BazaKl;";
            if (Upr_User.UprKO == false)
                DserchQwery = "Select max(OstAkt *1) FROM BazaKl WHERE Opiekun_klienta Like '%" + Upr_User.Imie + "%' and Opiekun_klienta like '%" + Upr_User.Nazwisko + "%'  ;";
            else
                DserchQwery = "Select max(OstAkt *1) FROM BazaKl   ;";
            string AMaxData = SqlRoader_Jedna_wartosc(AserchQwery, Acon);
            if (AMaxData is null)
                AMaxData = "";
            string DMaxData = SqlRoader_Jedna_wartosc(DserchQwery, Dcon);
            if (DMaxData is null)
                DMaxData = "";
            if (Upr_User.UprKO == false)
            {
                AserchQwery = "Select * from BazaKl  WHERE OstAkt *1 > '" + DMaxData + "' * 1 And Opiekun_klienta like '%" + Upr_User.Imie + "%' and Opiekun_klienta like '%" + Upr_User.Nazwisko + "%'  ;";
                DserchQwery = "Select * from BazaKl  WHERE OstAkt *1 > '" + AMaxData + "' * 1   And Opiekun_klienta like '%" + Upr_User.Nazwisko + "%' and Opiekun_klienta like '%" + Upr_User.Nazwisko + "%'  ;";
            }
            else
            {
                AserchQwery = "Select * from BazaKl  WHERE OstAkt *1 > '" + DMaxData + "' * 1 ;";
                DserchQwery = "Select * from BazaKl  WHERE OstAkt *1 > '" + AMaxData + "' * 1 ;";
            }
            // Console.WriteLine(Dcon.ConnectionString & vbCrLf & "Dcon")
            var NewBaza = new DataTable();
            if (Dcon.State == ConnectionState.Closed)
                Dcon.Open();
            if (Acon.State == ConnectionState.Closed)
                Acon.Open();
            //Console.WriteLine("Działanie wejsciowe {0}", 1);
            NewBaza = SqlComandDatabase(AserchQwery, Acon);
            //Console.WriteLine("Działanie wejsciowe {0}", 2);
            NewBazaDownload = SqlComandDatabase(DserchQwery, Dcon);
            LastRow = NewBaza.Rows.Count;
            if (Dcon.State == ConnectionState.Open)
                Dcon.Close();
            if (Acon.State == ConnectionState.Open)
                Acon.Close();
            int p = 0;
            LDzialanie = 1.ToString();

            string Prag = "PRAGMA writable_schema=ON; VACUUM;";
            UsingSQLComand(Prag, Acon);

            if (LastRow > 0)
            {
                foreach (DataRow newrow in NewBaza.Rows)
                {
                    string sqlstring = WstawDaneKlientDoBaza(newrow, false);

                    UsingSQLComand(sqlstring, Dcon);

                    BackGrund.ReportProgress(IntProgres(p, LastRow));
                    p += 1;
                }

            }
            p = 1;

            LastRow = NewBazaDownload.Rows.Count;
            LDzialanie = 2.ToString();
            //Console.WriteLine("last {0} Ldziałanie {1}", LastRow, LDzialanie);
            foreach (DataRow newrow in NewBazaDownload.Rows)
            {
                string sqlstring = WstawDaneKlientDoBaza(newrow, true);
                try
                {
                    UsingSQLComand(sqlstring, Acon);

                    BackGrund.ReportProgress(IntProgres(p, LastRow));
                }
                catch
                {

                }
                p += 1;
            }
            if (Dcon.State == ConnectionState.Open)
                Dcon.Close();
            stopwatch.Stop();
            return result;

        }
        public static string WstawDaneKlientDoBaza(DataRow newrow, bool Tm)
        {
            string WstawTim, wstawtimInsert;
            if (Tm == true)
            {
                WstawTim = "',OstAkt='" + newrow["OstAkt"].ToString() + "'";
                wstawtimInsert = "'" + newrow["OstAkt"].ToString() + "'";
            }
            else
            {
                WstawTim = "'";
                wstawtimInsert = "','" + Tim + "";
            }
            string sqlstring = @" -- Try to update any existing row
                                    UPDATE BazaKl
                                    SET opiekun_klienta = '" + newrow["Opiekun_klienta"].ToString() + "',Stan='" + newrow["Stan"].ToString() + "',Numer_konta='" + newrow["Numer_konta"].ToString() + "',Nazwa_klienta='" + newrow["Nazwa_klienta"].ToString() + "',Nazwa_CD = '" + newrow["Nazwa_CD"].ToString() + "',Adres='" + newrow["Adres"].ToString() + "',Kod_poczta='" + newrow["Kod_poczta"].ToString() + "',Poczta='" + newrow["Poczta"].ToString() + @"',
                                            Forma_plac='" + newrow["Forma_plac"].ToString() + "' , PraceList='" + newrow["PraceList"].ToString() + "', Branza='" + newrow["Branza"].ToString() + "' , Tel='" + newrow["Tel"].ToString() + "' ,  E_mail='" + newrow["E_mail"].ToString() + WstawTim + @"
                                    WHERE  NIP like '%" + newrow["NIP"].ToString() + @"%' ;
                                
                                -- If no update happened (i.e. the row didn't exist) then insert one
                                    INSERT INTO BazaKl (Opiekun_klienta,NIP,Stan,Numer_konta,Nazwa_klienta,Nazwa_CD,Adres,Kod_poczta,Poczta,Forma_plac,PraceList,Branza,Tel,E_mail,OstAkt) 
                                    SELECT '" + newrow["Opiekun_klienta"].ToString() + "','" + newrow["NIP"].ToString() + "','" + newrow["Stan"].ToString() + "','" + newrow["Numer_konta"].ToString() + "','" + newrow["Nazwa_klienta"].ToString() + "','" + newrow["Nazwa_CD"].ToString() + "','" + newrow["Adres"].ToString() + "','" + newrow["Kod_poczta"].ToString() + "','" + newrow["Poczta"].ToString() + "','" + newrow["Forma_plac"].ToString() + "','" + newrow["PraceList"].ToString() + "','" + newrow["Branza"].ToString() + "','" + newrow["Tel"].ToString() + "','" + newrow["E_mail"].ToString() + "','" + Tim + @"'
                                WHERE (Select Changes() = 0);";
            // Console.WriteLine(sqlstring)
            return sqlstring;
        }
        public static int ExportImportCSV(int ProgRaport, int Id, string SqweryDelete, string SqweryUpdate, string DataAktual)
        {
            try
            {
                // Console.WriteLine("ExportImportCSV - Działanie nr - " & ileZ & " / " & String.Format(vbCrLf & " Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
                var stopwatch = new Stopwatch();
                int i = 0;
                int result;
                if (ProgRaport == 0)
                {
                    result = 2;
                    return result;
                }
                stopwatch.Start();
                Tim = TimeAktual();
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                if (Acon.State == ConnectionState.Closed)
                    Acon.Open();
                UsingSQLComand(SqweryDelete, Acon);
                try
                {
                    using (var dcmd = new SQLiteCommand(SqweryUpdate))
                    {
                        using (var sda = new SQLiteDataAdapter())
                        {
                            dcmd.Connection = Dcon;
                            sda.SelectCommand = dcmd;
                            using (var dt = new DataTable())
                            {
                                sda.Fill(dt);
                                LastRow = dt.Rows.Count;
                                LDzialanie = "pobieram bazę - licz. wiersz " + LastRow + " ID = " + Id;
                                // Console.WriteLine(LDzialanie)
                                foreach (DataRow row in dt.Rows)
                                {
                                    if (Id == 1)
                                        Klient_ALL_Aktual_NewBaza(row);
                                    if (Id == 3)
                                        BazaZAKUPY_Aktual_NewBaza(row, DataAktual, Acon);
                                    if (Id == 4)
                                        Cennik_Aktual_NewBaza(row, DataAktual);
                                    i += 1;

                                    BC_Aktual_baza.ReportProgress(IntProgres(i, LastRow));
                                }
                            }
                        }
                    }
                    result = 1;
                }
                catch
                {
                    result = 2;
                }
                stopwatch.Stop();
                Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
                return result;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }
        public static int KO_Branżysta_Aktual_NewBaza(int ProgRaport, SQLiteConnection Pcon, SQLiteConnection Wcon)
        {
            try
            {
                // Console.WriteLine("AktualNewBaza - Działanie nr - " & ileZ & " / " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
                string a0, a1, a2;
                int result;
                TimeAktual();
                try
                {
                    var newBaza = new DataTable();
                    string SqlString = "Select * from DaneKO";
                    newBaza = SqlComandDatabase(SqlString, Pcon);
                    LastRow = newBaza.Rows.Count;
                    for (int i = 0, loopTo = newBaza.Rows.Count - 1; i <= loopTo; i++)
                    {

                        a0 = newBaza.Rows[i]["KO"].ToString();
                        a1 = newBaza.Rows[i]["Email"].ToString();
                        a2 = newBaza.Rows[i]["Branza"].ToString();
                        SqlString = @" -- Try to update any existing row
                                            Update DaneKO
                                            SET KO = '" + a0 + "',Email='" + a1 + "',Branza='" + a2 + @"'
                                            WHERE Branza  like '%" + a2 + @"%';

                                        -- If no update happened (i.e. the row didn't exist) then insert one
                                            INSERT INTO DaneKO (KO,Email,Branza) 
                                            SELECT '" + a0 + "','" + a1 + "','" + a2 + @"'
                                             WHERE (Select Changes() = 0);";
                        UsingSQLComand(SqlString, Wcon);
                        // Console.WriteLine(SqlString)
                        SqlString = "UPDATE BazaKl SET Branza = '" + a2 + @"' 
                                                  WHERE Branza  like '%" + Strings.Mid(a2, 1, 2) + "%';";
                        UsingSQLComand(SqlString, Wcon);
                        UsingSQLComand(SqlString, Pcon);

                        BC_Aktual_baza.ReportProgress(IntProgres(i, LastRow));
                    }
                    result = 1;
                    ConClose();
                    return result;
                }
                catch
                {
                    result = 2;
                }
                ConClose();
                return result;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }
        public static int Baza_Zakupy_Aktual_NewBaza(string DataAktual)
        {
            // Console.WriteLine("Baza_Zakupy_Aktual_NewBaza - Działanie nr - " & ileZ & " / " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
            int result;
            string a5;
            decimal aa5;
            string serchQwery;
            int i = 0;
            string MaxData;
            TimeAktual();

            if (Dcon.State == ConnectionState.Closed)
                Dcon.Open();
            if (Acon.State == ConnectionState.Closed)
                Acon.Open();

            serchQwery = "SELECT max(OstAkt *1) FROM BazaZKP WHERE Yearbilling not like ''   LIMIT 1;";
            try
            {
                MaxData = SqlRoader_Jedna_wartosc(serchQwery, Acon);
            }
            catch
            {
                MaxData = "";
            }


            if ((NewBazaDownload != null) && (NewBazaDownload.Rows.Count > 0))
                NewBazaDownload.Clear();

            if (Upr_User.UprKO == true)
            {
                var Db = new DataTable();
                string SqlSerchZmiana = "SELECT * FROM InfoZmiana ";
                Db = SqlComandDatabase(SqlSerchZmiana, Dcon);

                if (Db != null)
                {
                    foreach (DataRow row in Db.Rows)
                    {
                        string UpdPHZmiana = @" UPDATE BazaZKP
                                                    SET Representative = '" + Strings.Replace(row["PHDo"].ToString(), "'", "") + "',OstAkt='" + DataAktual + @"' 
                                                    where  Representative like '%" + row["PHOd"].ToString() + "%'; ";
                        try
                        {
                            UsingSQLComand(UpdPHZmiana, Acon);
                        }
                        // Console.WriteLine(UpdPHZmiana)
                        catch
                        {
                        }
                    }

                }
            }

            if (Upr_User.UprKO == false)
            {
                serchQwery = "SELECT * FROM BazaZKP WHERE Representative Like '%" + Upr_User.Imie + "%' and Representative like '%" + Upr_User.Nazwisko + "%' and OstAkt * 1 > '" + MaxData + "' * 1 order by Document_Billing";
            }
            else
            {
                serchQwery = "SELECT * FROM BazaZKP  WHERE OstAkt * 1 > '" + MaxData + "' *1";
            }
            NewBazaDownload = SqlComandDatabase(serchQwery, Dcon);
            LastRow = NewBazaDownload.Rows.Count;
            // Console.WriteLine("Baza_Zakupy_Aktual_NewBaza - Działanie nr - " & ileZ & " / " & serchQwery & " / " & NewBazaDownload.Rows.Count & " / " & String.Format("  Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
            LDzialanie = 1.ToString();


            foreach (DataRow row in NewBazaDownload.Rows)
            {
                a5 = row["SalesP"].ToString();
                if (Information.IsNumeric(a5))
                {
                    aa5 = Conversions.ToDecimal(a5);
                    a5 = Math.Round(aa5, 2).ToString();
                }
                else
                    a5 = 0.ToString();
                string stringSQL = Conversions.ToString(Operators.ConcatenateObject(@" -- Try to update any existing row                      
                                   UPDATE BazaZKP
                                        SET Representative = '" + Strings.Replace(row["Representative"].ToString(), "'", "") + "',SoldTocustomer='" + Strings.Replace(row["SoldTocustomer"].ToString(), "'", "") + "',Material='" + row["Material"].ToString() + "', Quantity = '" + row["Quantity"].ToString() + "',Yearbilling='" + row["Yearbilling"].ToString() + "',SalesP='" + a5 + "',Turnover='" + row["Turnover"].ToString() + "', Datebilling='" + row["Datebilling"].ToString() + "', Document_Billing='" + row["Document_Billing"].ToString() + "' ,Order_Item='" + row["Order_Item"].ToString() + "' ,OstAkt='" + DataAktual + @"' 
                                   where  Document_Billing like '%" + row["Document_Billing"].ToString() + "%' AND Order_Item like '%" + row["Order_Item"].ToString() + "%'  AND Datebilling like '%" + row["Datebilling"].ToString(), Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(Operators.AddObject(@"%';
                                -- If no update happened (i.e. the row didn't exist) then insert one                                         
                                        INSERT INTO BazaZKP  (Representative ,SoldTocustomer ,Material ,Quantity ,Yearbilling ,SalesP ,Turnover ,Datebilling , Document_Billing ,Order_Item, OstAkt)
                                        SELECT'" + Strings.Replace(row["Representative"].ToString(), "'", "") + "','" + Strings.Replace(row["SoldTocustomer"].ToString(), "'", "") + "','" + row["Material"].ToString() + "','" + row["Quantity"].ToString() + "','" + row["Yearbilling"].ToString() + "','" + a5 + "','", row["Turnover"]), "','"), row["Datebilling"].ToString()), "','"), row["Document_Billing"].ToString()), "','"), row["Order_Item"].ToString()), "','"), DataAktual), @"'
                                    WHERE (Select Changes() = 0);")));
                UsingSQLComand(stringSQL, Acon);
                i += 1;

                BC_Aktual_baza.ReportProgress(IntProgres(i, LastRow));
            }
            if (Dcon.State == ConnectionState.Open)
                Dcon.Close();
            result = 1;
            AktualZKPZmianaKO = 0;

            return 1;
        }
        public static int Exportuj_Importuj_CSV(int ProgRaport, int Id, string SqweryDelete, string SqweryUpdate, string DataAktual)
        {
            try
            {
                // Console.WriteLine("AktualNewBaza - Działanie nr - " & ileZ & " / " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
                var stopwatch = new Stopwatch();
                int i = 0;
                int result;
                if (ProgRaport == 0)
                {
                    result = 2;
                    return result;
                }
                stopwatch.Start();
                Tim = TimeAktual();
                if (Acon.State == ConnectionState.Closed)
                    Acon.Open();
                UsingSQLComand(SqweryDelete, Acon);
                try
                {
                    // Console.WriteLine(SqweryUpdate)
                    using (var dcmd = new SQLiteCommand(SqweryUpdate))
                    {
                        using (var sda = new SQLiteDataAdapter())
                        {
                            dcmd.Connection = Dcon;
                            sda.SelectCommand = dcmd;
                            using (var dt = new DataTable())
                            {
                                sda.Fill(dt);
                                LastRow = dt.Rows.Count;
                                LDzialanie = 1.ToString();
                                foreach (DataRow row in dt.Rows)
                                {
                                    if (Id == 4)
                                        Cennik_NewBaza_Aktual(row, DataAktual);
                                    i += 1;

                                    BC_Aktual_baza.ReportProgress(IntProgres(i, LastRow));
                                }
                            }
                        }
                    }
                    result = 1;
                }
                catch
                {
                    result = 2;
                }
                stopwatch.Stop();

                if (Dcon.State == ConnectionState.Open)
                    Dcon.Close();
                return result;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }

        }
        public static void Cennik_NewBaza_Aktual(DataRow road, string DataAktual)
        {
            // Console.WriteLine("aktual cennik")
            try
            {
                string stringqwert = @" -- Try to update any existing row
                                    UPDATE Cennik
                                    SET ProdKod = '" + road["ProdKod"].ToString() + "',Naglowek='" + road["Naglowek"].ToString() + "',Lpgrup='" + road["Lpgrup"].ToString() + "', NazwProd = '" + road["NazwProd"].ToString() + "',Kszt='" + road["Kszt"].ToString() + "',Pszt='" + road["Pszt"].ToString() + "',Poj='" + road["Poj"].ToString() + "',Miara='" + road["Miara"].ToString() + "', Kolor='" + road["Kolor"].ToString() + "' , CDM='" + road["CDM"].ToString() + "',CK='" + road["CK"].ToString() + "',PH='" + road["PH"].ToString() + "',ZPR0='" + road["ZPR0"].ToString() + "',GRUPA='" + road["GRUPA"].ToString() + "',KATEGORIA='" + road["KATEGORIA"].ToString() + "',NAZEWNICTWO='" + road["NAZEWNICTWO"].ToString() + "',BrakPrace='" + road["BrakPrace"].ToString() + "',OstAkt='" + DataAktual.ToString() + @"'                           
                                    WHERE SAP like '%" + road["SAP"].ToString() + @"%';
                                -- If no update happened (i.e. the row didn't exist) then insert one                                         
                                    INSERT INTO Cennik  (ProdKod,Naglowek,Lpgrup,SAP,NazwProd,Kszt,Pszt,Poj,Miara,Kolor,CDM,CK,PH,ZPR0,GRUPA,KATEGORIA,NAZEWNICTWO,BrakPrace,OstAkt)
                                    SELECT '" + road["ProdKod"].ToString() + "','" + road["Naglowek"].ToString() + "','" + road["Lpgrup"].ToString() + "','" + road["SAP"].ToString() + "','" + road["NazwProd"].ToString() + "','" + road["Kszt"].ToString() + "','" + road["Pszt"].ToString() + "','" + road["Poj"].ToString() + "','" + road["Miara"].ToString() + "','" + road["Kolor"].ToString() + "','" + road["CDM"].ToString() + "','" + road["CK"].ToString() + "','" + road["PH"].ToString() + "','" + road["ZPR0"].ToString() + "','" + road["GRUPA"].ToString() + "','" + road["KATEGORIA"].ToString() + "','" + road["NAZEWNICTWO"].ToString() + "','" + road["BrakPrace"].ToString() + "','" + DataAktual.ToString() + @"'                    
                                    WHERE (Select Changes() = 0);";
                UsingSQLComand(stringqwert, Acon);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public static int Klient_ALL_Aktual_NewBaza(DataRow road)
        {
            try
            {
                using (var transaction = Acon.BeginTransaction())
                {
                    using (var command = Acon.CreateCommand())
                    {
                        command.CommandText = "  INSERT INTO BazaKl (Opiekun_klienta,NIP,Stan,Numer_konta,Nazwa_klienta,Nazwa_CD,Adres,Kod_poczta,Poczta,Forma_plac,PraceList,Branza,Tel,E_mail,OstAkt)  " + @" 
                                                    VALUES ($Opiekun_klienta,$NIP,$Stan,$Numer_konta,$Nazwa_klienta,$Nazwa_CD,$Adres,$Kod_poczta,$Poczta,$Forma_plac,$PraceList,$Branza,$Tel,$E_mail,$OstAkt) ;";
                        var a1 = command.CreateParameter(); a1.ParameterName = "$Opiekun_klienta"; command.Parameters.Add(a1);
                        var a2 = command.CreateParameter(); a2.ParameterName = "$NIP"; command.Parameters.Add(a2);
                        var a3 = command.CreateParameter(); a3.ParameterName = "$Stan"; command.Parameters.Add(a3);
                        var a4 = command.CreateParameter(); a4.ParameterName = "$Numer_konta"; command.Parameters.Add(a4);
                        var a5 = command.CreateParameter(); a5.ParameterName = "$Nazwa_klienta"; command.Parameters.Add(a5);
                        var a6 = command.CreateParameter(); a6.ParameterName = "$Nazwa_CD"; command.Parameters.Add(a6);
                        var a7 = command.CreateParameter(); a7.ParameterName = "$Adres"; command.Parameters.Add(a7);
                        var a8 = command.CreateParameter(); a8.ParameterName = "$Kod_poczta"; command.Parameters.Add(a8);
                        var a9 = command.CreateParameter(); a9.ParameterName = "$Poczta"; command.Parameters.Add(a9);
                        var a10 = command.CreateParameter(); a10.ParameterName = "$Forma_plac"; command.Parameters.Add(a10);
                        var a11 = command.CreateParameter(); a11.ParameterName = "$PraceList"; command.Parameters.Add(a11);
                        var a12 = command.CreateParameter(); a12.ParameterName = "$Branza"; command.Parameters.Add(a12);
                        var a13 = command.CreateParameter(); a13.ParameterName = "$Tel"; command.Parameters.Add(a13);
                        var a14 = command.CreateParameter(); a14.ParameterName = "$E_mail"; command.Parameters.Add(a14);
                        var a15 = command.CreateParameter(); a15.ParameterName = "$OstAkt"; command.Parameters.Add(a15);
                        a1.Value = road["Opiekun_klienta"].ToString();
                        a2.Value = road["NIP"].ToString();
                        a3.Value = road["Stan"].ToString();
                        a4.Value = road["Numer_konta"].ToString();
                        a5.Value = road["Nazwa_klienta"].ToString();
                        a6.Value = road["Nazwa_CD"].ToString();
                        a7.Value = road["Adres"].ToString();
                        a8.Value = road["Kod_poczta"].ToString();
                        a9.Value = road["Poczta"].ToString();
                        a10.Value = road["Forma_plac"].ToString();
                        a11.Value = road["PraceList"].ToString();
                        a12.Value = road["Branza"].ToString();
                        a13.Value = road["Tel"].ToString();
                        a14.Value = road["E_mail"].ToString();
                        a15.Value = Tim;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }
        public static int BazaZAKUPY_Aktual_NewBaza(DataRow road, string DataAktual, SQLiteConnection Z_con)
        {
            try
            {
                // Console.WriteLine("BazaZAKUPY_Aktual_NewBaza - Działanie nr - " & ileZ & " / " & String.Format(vbCrLf & " Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
                if (Z_con.State == ConnectionState.Closed)
                    Z_con.Open();
                using (var transaction = Z_con.BeginTransaction())
                {
                    using (var command = Z_con.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO BazaZKP (Representative ,SoldTocustomer ,Material ,Quantity ,Yearbilling ,SalesP ,Turnover ,Datebilling , Document_Billing ,Order_Item, OstAkt) " + @" 
                                        VALUES($Representative ,$SoldTocustomer ,$Material ,$Quantity ,$Yearbilling ,$SalesP ,$Turnover ,$Datebilling , $Document_Billing ,$Order_Item, $OstAkt);";
                        var a1 = command.CreateParameter(); a1.ParameterName = "$Representative"; command.Parameters.Add(a1);
                        var a2 = command.CreateParameter(); a2.ParameterName = "$SoldTocustomer"; command.Parameters.Add(a2);
                        var a3 = command.CreateParameter(); a3.ParameterName = "$Material"; command.Parameters.Add(a3);
                        var a4 = command.CreateParameter(); a4.ParameterName = "$Quantity"; command.Parameters.Add(a4);
                        var a5 = command.CreateParameter(); a5.ParameterName = "$Yearbilling"; command.Parameters.Add(a5);
                        var a6 = command.CreateParameter(); a6.ParameterName = "$SalesP"; command.Parameters.Add(a6);
                        var a7 = command.CreateParameter(); a7.ParameterName = "$Turnover"; command.Parameters.Add(a7);
                        var a8 = command.CreateParameter(); a8.ParameterName = "$Datebilling"; command.Parameters.Add(a8);
                        var a9 = command.CreateParameter(); a9.ParameterName = "$Document_Billing"; command.Parameters.Add(a9);
                        var a10 = command.CreateParameter(); a10.ParameterName = "$Order_Item"; command.Parameters.Add(a10);
                        var a11 = command.CreateParameter(); a11.ParameterName = "$OstAkt"; command.Parameters.Add(a11);

                        a1.Value = road["Representative"].ToString();
                        a2.Value = road["SoldTocustomer"].ToString();
                        a3.Value = road["Material"].ToString();
                        a4.Value = road["Quantity"].ToString();
                        a5.Value = road["Yearbilling"].ToString();
                        a6.Value = road["SalesP"].ToString();
                        a7.Value = road["Turnover"].ToString();
                        a8.Value = road["Datebilling"].ToString();
                        a9.Value = road["Document_Billing"].ToString();
                        a10.Value = Strings.Mid(road["Order_Item"].ToString(), 1, 10);
                        a11.Value = DataAktual;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        // Console.WriteLine(Tim)
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }
        public static void Cennik_Aktual_NewBaza(DataRow road, string DataAktual)
        {
            // Console.WriteLine("aktual cennik")
            try
            {
                using (var transaction = Acon.BeginTransaction())
                {
                    using (var command = Acon.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Cennik(ProdKod,Naglowek,Lpgrup,SAP,NazwProd,Kszt,Pszt ,Poj,Miara,Kolor,CDM,CK,PH,ZPR0,GRUPA,KATEGORIA,NAZEWNICTWO,BrakPrace,OstAkt) " + @" 
                                                   VALUES ($ProdKod,$Naglowek,$Lpgrup,$SAP,$NazwProd,$Kszt,$Pszt ,$Poj,$Miara,$Kolor,$CDM,$CK,$PH,$ZPR0,$GRUPA,$KATEGORIA,$NAZEWNICTWO,$BrakPrace,$OstAkt) ;"; // ($name, $email);"
                                                                                                                                                                                                               // Console.WriteLine(command.CommandText.ToString)
                        var a1 = command.CreateParameter(); a1.ParameterName = "$ProdKod"; command.Parameters.Add(a1);
                        var a2 = command.CreateParameter(); a2.ParameterName = "$Naglowek"; command.Parameters.Add(a2);
                        var a3 = command.CreateParameter(); a3.ParameterName = "$Lpgrup"; command.Parameters.Add(a3);
                        var a4 = command.CreateParameter(); a4.ParameterName = "$SAP"; command.Parameters.Add(a4);
                        var a5 = command.CreateParameter(); a5.ParameterName = "$NazwProd"; command.Parameters.Add(a5);
                        var a6 = command.CreateParameter(); a6.ParameterName = "$Kszt"; command.Parameters.Add(a6);
                        var a7 = command.CreateParameter(); a7.ParameterName = "$Pszt"; command.Parameters.Add(a7);
                        var a8 = command.CreateParameter(); a8.ParameterName = "$Poj"; command.Parameters.Add(a8);
                        var a9 = command.CreateParameter(); a9.ParameterName = "$Miara"; command.Parameters.Add(a9);
                        var a10 = command.CreateParameter(); a10.ParameterName = "$Kolor"; command.Parameters.Add(a10);
                        var a11 = command.CreateParameter(); a11.ParameterName = "$CDM"; command.Parameters.Add(a11);
                        var a12 = command.CreateParameter(); a12.ParameterName = "$CK"; command.Parameters.Add(a12);
                        var a13 = command.CreateParameter(); a13.ParameterName = "$PH"; command.Parameters.Add(a13);
                        var a14 = command.CreateParameter(); a14.ParameterName = "$ZPR0"; command.Parameters.Add(a14);
                        var a15 = command.CreateParameter(); a15.ParameterName = "$GRUPA"; command.Parameters.Add(a15);
                        var a16 = command.CreateParameter(); a16.ParameterName = "$KATEGORIA"; command.Parameters.Add(a16);
                        var a17 = command.CreateParameter(); a17.ParameterName = "$NAZEWNICTWO"; command.Parameters.Add(a17);
                        var a18 = command.CreateParameter(); a18.ParameterName = "$BrakPrace"; command.Parameters.Add(a18);
                        var a19 = command.CreateParameter(); a19.ParameterName = "$OstAkt"; command.Parameters.Add(a19);
                        a1.Value = road["ProdKod"].ToString();
                        a2.Value = road["Naglowek"].ToString();
                        a3.Value = road["Lpgrup"].ToString();
                        a4.Value = road["SAP"].ToString();
                        a5.Value = road["NazwProd"].ToString();
                        a6.Value = road["Kszt"].ToString();
                        a7.Value = road["Pszt"].ToString();
                        a8.Value = road["Poj"].ToString();
                        a9.Value = road["Miara"].ToString();
                        a10.Value = road["Kolor"].ToString();
                        a11.Value = road["CDM"].ToString();
                        a12.Value = road["CK"].ToString();
                        a13.Value = road["PH"].ToString();
                        a14.Value = road["ZPR0"].ToString();
                        a15.Value = road["GRUPA"].ToString();
                        a16.Value = road["KATEGORIA"].ToString();
                        a17.Value = road["NAZEWNICTWO"].ToString();
                        a18.Value = road["BrakPrace"].ToString();
                        a19.Value = DataAktual;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public static int ZK11_SubAktualizuj_NewBaza(int ProgRaport, SQLiteConnection Pcon, SQLiteConnection Wcon)
        {
            try
            {
                // Console.WriteLine("AktualNewBaza - Działanie nr - " & ileZ & " / " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
                string a0, a1, a2, a3, a4, a5, a6, a7, a8;
                int result;
                Tim = TimeAktual();
                try
                {
                    string SqlString;

                    if (Upr_User.UprKO == false)
                    {
                        SqlString = @" SELECT si.id, si.NIP, si.NrSAP,si.ZK1,si.ZK2,si.ZK3,si.ZK1Info,si.ZK2Info,si.ZK3Info,si.Representative,si.OstAkt
                                  From  TabZK si 
                                  WHERE si.Representative Like '%" + Upr_User.Imie + "%' and si.Representative like '%" + Upr_User.Nazwisko + "%';";
                    }
                    else
                    {
                        SqlString = @"SELECT si.id, si.NIP, si.NrSAP,si.ZK1,si.ZK2,si.ZK3,si.ZK1Info,si.ZK2Info,si.ZK3Info,si.Representative,si.OstAkt
                                 From  TabZK si;";
                    }

                    var newBaza = new DataTable();
                    if (Pcon.State == ConnectionState.Closed)
                        Pcon.Open();
                    if (Wcon.State == ConnectionState.Closed)
                        Wcon.Open();

                    newBaza = SqlComandDatabase(SqlString, Pcon);
                    LastRow = newBaza.Rows.Count;

                    for (int i = 0, loopTo = newBaza.Rows.Count - 1; i <= loopTo; i++)
                    {
                        a0 = newBaza.Rows[i]["NIP"].ToString();
                        a1 = newBaza.Rows[i]["NrSAP"].ToString();
                        a2 = newBaza.Rows[i]["ZK1"].ToString();
                        a3 = newBaza.Rows[i]["ZK2"].ToString();
                        a4 = newBaza.Rows[i]["ZK3"].ToString();
                        a5 = newBaza.Rows[i]["ZK1Info"].ToString();
                        a6 = newBaza.Rows[i]["ZK2Info"].ToString();
                        a7 = newBaza.Rows[i]["ZK3Info"].ToString();
                        a8 = newBaza.Rows[i]["Representative"].ToString();
                        string stringqwerty = @" -- Try to update any existing row
                                    UPDATE TabZK
                                    SET NIP = '" + a0 + "',NrSAP='" + a1 + "',ZK1='" + a2 + "',ZK2='" + a3 + "',ZK3 = '" + a4 + "',ZK1Info='" + a5 + "',ZK2Info='" + a6 + "',ZK3Info='" + a7 + "',Representative='" + a8 + "',OstAkt='" + Tim + @"'
                                    WHERE NIP  like '%" + a0 + "%' and NrSAP like '%" + a1 + @"%' ;

                                -- If no update happened (i.e. the row didn't exist) then insert one
                                    INSERT INTO TabZK  ( NIP ,NrSAP ,ZK1 ,ZK2 ,ZK3 ,ZK1Info ,ZK2Info ,ZK3Info,Representative,OstAkt )
                                    SELECT '" + a0 + "','" + a1 + "','" + a2 + "','" + a3 + "','" + a4 + "','" + a5 + "','" + a6 + "','" + a7 + "','" + a8 + "','" + Tim + @"'
                                WHERE (Select Changes() = 0);";
                        UsingSQLComand(stringqwerty, Wcon);
                        BC_Aktual_baza.ReportProgress(IntProgres(i, LastRow));

                    }
                    if (Pcon.State == ConnectionState.Open)
                        Pcon.Close();
                    string comand = "select NIP , Opiekun_klienta FROM BazaKL";
                    DataTable newBazaPH = SqlComandDatabase_NewBaza(comand, Acon);
                    string np, ph;
                    foreach (DataRow row in newBazaPH.Rows)
                    {
                        np = Conversions.ToString(row["NIP"]);
                        ph = Conversions.ToString(row["Opiekun_klienta"]);
                        string upd = @"  UPDATE TabZK
                                    SET Representative='" + ph + @"'
                                    WHERE NIP  like '%" + np + "%'  ;";
                        UsingSQLComand(upd, Wcon);
                    }
                    result = 1;
                    Console.WriteLine("aktual zk zakończono");
                }
                catch
                {
                    Console.WriteLine("BLĄD !!!!! aktual zk zakończono");
                    result = 2;
                }
                ConClose();
                return result;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }
        public static int Zapisane_Oferty_Aktual_NewBaza(int ProgRaport, SQLiteConnection Pcon, SQLiteConnection Wcon, string con_str)
        {
            Console.WriteLine(con_str + " AktualNewBaza - Działanie nr - " + ileZ + " / " + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
            int result;
            string a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11;
            string SqlString;
            if (Upr_User.UprKO == false)
            {
                SqlString = "Select * from TblOferta WHERE Representative Like '%" + Upr_User.Imie + "%' and Representative like '%" + Upr_User.Nazwisko + "%' ";
            }
            else
            {
                SqlString = "Select * from TblOferta";
            }
            var newBaza = new DataTable();
            if (Pcon.State == ConnectionState.Closed)
                Pcon.Open();
            if (Wcon.State == ConnectionState.Closed)
                Wcon.Open();
            newBaza = SqlComandDatabase(SqlString, Pcon);
            LastRow = newBaza.Rows.Count;
            for (int i = 0, loopTo = newBaza.Rows.Count - 1; i <= loopTo; i++)
            {
                a0 = newBaza.Rows[i]["Representative"].ToString();
                a1 = newBaza.Rows[i]["Data"].ToString();
                a2 = newBaza.Rows[i]["Numer_konta"].ToString();
                a3 = newBaza.Rows[i]["SAP"].ToString();
                a4 = newBaza.Rows[i]["NazwProd"].ToString();
                a5 = newBaza.Rows[i]["CenaDoOFR"].ToString();
                a6 = newBaza.Rows[i]["ZK1"].ToString();
                a7 = newBaza.Rows[i]["Zk2"].ToString();
                a8 = newBaza.Rows[i]["ZK3"].ToString();
                a9 = newBaza.Rows[i]["szt1"].ToString();
                a10 = newBaza.Rows[i]["szt2"].ToString();
                a11 = newBaza.Rows[i]["szt3"].ToString();
                string stringSql = @" -- Try to update any existing row
                                               UPDATE TblOferta
                                                        SET Representative = '" + a0 + "',Data='" + a1 + "',Numer_konta='" + a2 + "',SAP='" + a3 + "',NazwProd='" + a4 + "',CenaDoOFR='" + a5 + @"'
                                                        ,ZK1='" + a6 + "',Zk2='" + a7 + "',ZK3='" + a8 + "',szt1='" + a9 + "',szt2 ='" + a10 + "',szt3 ='" + a11 + "',OstAkt ='" + Tim + @"'
                                                WHERE  Numer_konta like '%" + a2 + "%' And  SAP like '%" + a3 + @"%';
                                                -- If no update happened (i.e. the row didn't exist) then insert one
                                                    INSERT INTO TblOferta                      
                                                    (Representative ,Data, Numer_konta,SAP,NazwProd, CenaDoOFR, ZK1  , Zk2 , ZK3, szt1, szt2,  szt3,OstAkt)
                                                    SELECT '" + a0 + "','" + a1 + "','" + a2 + "','" + a3 + "','" + a4 + "','" + a5 + "','" + a6 + "','" + a7 + "','" + a8 + "','" + a9 + "','" + a10 + "','" + a11 + "','" + Tim + @"'
                                                WHERE (Select Changes() = 0);";
                // Console.WriteLine(stringSql)
                UsingSQLComand(stringSql, Wcon);
                if (i > 1 && LastRow > 1)
                {
                    if (ProgRaport == 1)
                    {
                        BC_Aktual_baza.ReportProgress(IntProgres(i, LastRow));
                    }
                }
            }
            Console.WriteLine("end");
            result = 1;
            return result;
        }
        public static DataTable PobierzPlik_IMG_PDF_Z_NEW_BazaFTP(string ftp, string ftpFolder, BackgroundWorker BackGrund)
        {
            var dtFiles = new DataTable();
            //Console.WriteLine("PobierzPlik_IMG_PDF_Z_NEW_BazaFTP - Działanie nr - " + ileZ + " / " + ftp + ftpFolder + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
            try
            {
                LDzialanie = 3.ToString();
                int i = 0;
                string Serchdate = null;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(Uide, Pas);
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                DateTime lastModifiedDate = response.LastModified;
                var entries = new List<string>();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    entries = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                response.Close();
                dtFiles.Columns.AddRange(new DataColumn[3] { new DataColumn("Name", typeof(string)), new DataColumn("Size", typeof(decimal)), new DataColumn("Date", typeof(string)) });
                int size = default;


                foreach (string entry in entries)
                {
                    string[] splits = entry.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    bool isFile = splits[0].Substring(0, 1) != "d";
                    bool isDirectory = splits[0].Substring(0, 1) == "d";
                    if (isFile)
                    {
                        dtFiles.Rows.Add();
                        size = (int)Math.Round(decimal.Parse(splits[4]) / 1024m);
                        dtFiles.Rows[dtFiles.Rows.Count - 1]["Size"] = size;
                        Serchdate = string.Join(" ", splits[5], splits[6], splits[7]);
                        string AktData = Serchdate;
                        ConvertToDateTime(Serchdate);

                        string name = string.Empty;
                        for (int j = 8, loopTo = splits.Length - 1; j <= loopTo; j++)
                            name = string.Join(" ", name, splits[j]);
                        dtFiles.Rows[dtFiles.Rows.Count - 1]["Name"] = name.Trim();
                        try
                        {
                            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + "/" + name.Trim());
                            req.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                            req.Credentials = new NetworkCredential(Uide, Pas);

                            using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse())
                            {
                                string NewData = ConvertToDateTime(resp.LastModified.ToString());
                                dtFiles.Rows[dtFiles.Rows.Count - 1]["Date"] = NewData;
                            }
                        }
                        catch
                        {
                            dtFiles.Rows[dtFiles.Rows.Count - 1]["Date"] = AktData;
                        }

                    }

                    i += 1;
                    if (BackGrund != null)
                    {

                        BackGrund.ReportProgress(IntProgres(i, entries.Count));
                    }
                }
                i = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace.ToString());
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
            return dtFiles; 
        }
        public static int AktualPlik_OFR_PDF_NewBaza(DataTable DB_PDF)
        {
            int result = 1;
            do
            {
                try
                {
                    if (DB_PDF is null)
                    {
                        return 1;
                    }
                    LDzialanie = 1.ToString();
                    //Console.WriteLine("AktualPlik_OFR_PDF_NewBaza - Działanie nr - " + ileZ + " / " + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                    var newBaza = new DataTable();
                    string sqlstring = "Select * from TblPdf Where PlkPdf IS NOT NULL ;"; 
                    if (Acon.State == ConnectionState.Closed)
                        Acon.Open();
                    if (Dcon.State == ConnectionState.Closed)
                        Dcon.Open();
                    newBaza = SqlComandDatabase(sqlstring, Acon);
                    LastRow = DB_PDF.Rows.Count;
                    string Name_Nip;
                    string Name_Pdf;
                    string NEWDate;
                    for (int J = 0, loopTo = DB_PDF.Rows.Count - 1; J <= loopTo; J++)
                    {
                        Name_Nip = Strings.Mid(Conversions.ToString(DB_PDF.Rows[J]["Name"]), 1, 10);
                        Name_Pdf = Zmien_opisPDF(Strings.Mid(Conversions.ToString(DB_PDF.Rows[J]["Name"]), 11));
                        NEWDate = Conversions.ToString(DB_PDF.Rows[J]["Date"]);
                        string DserchQwery = "Select NIP FROM BazaKl WHERE Nip like '%" + Name_Nip + "%'  ;";
                        string SerchKl = SqlRoader_Jedna_wartosc(DserchQwery, Acon);
                        if (Information.IsNumeric(SerchKl))
                        {
                            // Console.WriteLine(J & " - " & NEWDate & " - " & SerchKl & " / NAME NIP - " & Name_Nip & " / Name Plik - " & Name_Pdf)

                            if ((SerchKl ?? "") == (Name_Nip ?? ""))
                            {
                                string SerchOFR = "Select OstAkt From TblPdf Where NrOFR Like '%" + Strings.Replace(Strings.Mid(Name_Pdf, 2), ".Pdf", "").Trim() + "%' AND PlkPdf IS NOT NULL ;";
                                string SerchPDF = SqlRoader_Jedna_wartosc(SerchOFR, Acon);
                                if ((NEWDate ?? "") != (SerchPDF ?? ""))
                                {
                                    // Console.WriteLine(J & " - Dopisz - " & SerchPDF & "//_-->" & SerchOFR)

                                    byte[] newFileData = null;
                                    bool ZezwDownl = true;
                                    string url = Encode(Strim_URL + "BazaOfr/" + DB_PDF.Rows[J]["Name"].ToString());

                                    // Console.WriteLine(url)
                                    if (ZezwDownl == true)
                                    {
                                        newFileData = DownloadFile_URL(url); 
                                        Aktual_PDF_NEWBAZA(Name_Nip, Strings.Mid(Name_Pdf, 2), newFileData, NEWDate, Acon);
                                    }
                                }
                            }
                        }

                        BC_Aktual_baza.ReportProgress(IntProgres(J, LastRow));
                    }
                    if (Dcon.State == ConnectionState.Open)
                        Dcon.Close();
                    result = 1;
                }
                catch (Exception ex)
                {
                    TextMessage(ex.StackTrace.ToString());
                    result = 2;
                }
            }
            while (false);
            return result;
        }

        private static object Aktual_PDF_NEWBAZA(string a0, string a1, byte[] bytes, string TimAct, SQLiteConnection Zcon)
        {
            int err;
            try
            {
                if (Zcon.State == ConnectionState.Closed)
                    Zcon.Open();
                string SqlComand = @" -- Try to update any existing row
                                            UPDATE TblPdf SET SAP =@SP ,NrOFR=@NO,PlkPdf=@PPdf,OstAkt=@OstAkt WHERE NrOFR like '%" + a1 + @"%';  
                                        -- If no update happened (i.e. the row didn't exist) then insert one
                                            INSERT INTO TblPdf  (SAP, NrOFR, PlkPdf,OstAkt) SELECT  @SP,@NO,@PPdf,@OstAkt WHERE (Select Changes() = 0);";
                // Console.WriteLine(SqlComand)
                using (var command = new SQLiteCommand(SqlComand, Zcon))
                {
                    command.Parameters.Add("@SP", (DbType)SqlDbType.VarChar).Value = a0;
                    command.Parameters.Add("@NO", (DbType)SqlDbType.VarChar).Value = a1;
                    command.Parameters.Add("@PPdf", (DbType)SqlDbType.Binary).Value = bytes;
                    command.Parameters.Add("@OstAkt", (DbType)SqlDbType.VarChar).Value = TimAct;
                    command.ExecuteNonQuery();
                }
                err = 0;
            }
            catch (Exception ex)
            {
                err = 1;
                TextMessage(ex.StackTrace.ToString());
                return err;
            }
            return err;
        }
        private static int WczytajDaneZBazaFTP_Plik_PDF_Plik_IMG_NewBaza(SQLiteConnection ZAcon)
        {
            // Console.WriteLine("WczytajDaneZBazaFTP_Plik_PDF_Plik_IMG_NewBaza nr - " & ileZ & " / " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
            try
            {
                Tim = TimeAktual();
                string Sqwery = @"Select si.Id, si.Lpgrup, si.SAP, si.NAZEWNICTWO, md.Img, md.Tds, md.KC,md.PDF2,md.OstAkt ,md.OstAktTDS ,md.OstAktKC ,md.OstAktPDF2                
                                From Cennik si
                                Left Join  Baza_PDF md ON md.SAP = si.SAP";
                Console.WriteLine(Sqwery + Constants.vbCrLf + ZAcon.ToString());
                DataTable DtTablePdf_FTP = SqlComandDatabase(Sqwery, ZAcon);

                LastRow = DtTablePdf_FTP.Rows.Count;
                DataTable DT_Img = null;
                DataTable DT_PDF = null;
                LadDzailanie = "Kończę";
                if (AktIMGStart == true)
                {
                    LadDzailanie = "Aktualizuj zdjęcia";
                    DT_Img = PobierzPlik_IMG_PDF_Z_NEW_BazaFTP(Strim_URL, "Img/", BC_Aktual_baza);
                }
                else
                    goto LinePDF;
                Przepisz_Plk_Do_BazaSQL_NewBaza(DtTablePdf_FTP, DT_Img, "Img/", ".jpg", "OstAkt", BC_Aktual_baza, ZAcon);
            LinePDF:
                ;

                LadDzailanie = "Kończę";
                ileZ = 9;
                LadDzailanie = "Aktualizuj Pliki TDS";
                if (AktTDSStart == true)
                {
                    LadDzailanie = "Aktualizuj Karty TDS";
                    DT_PDF = PobierzPlik_IMG_PDF_Z_NEW_BazaFTP(Strim_URL, "Pdf/", BC_Aktual_baza);
                }
                else
                    goto LineChar;
                Przepisz_Plk_Do_BazaSQL_NewBaza(DtTablePdf_FTP, DT_PDF, "Pdf/", ".pdf", "OstAktTDS", BC_Aktual_baza, ZAcon);
            LineChar:
                ;

                LadDzailanie = "Kończę";
                ileZ = 10;
                LadDzailanie = "Aktualizuj Pliki Charakterystyki";
                if (AktTDSStart == true)
                {
                    LadDzailanie = "Aktualizuj Karty Char.";
                    DT_PDF = PobierzPlik_IMG_PDF_Z_NEW_BazaFTP(Strim_URL, "Kart_Char/", BC_Aktual_baza);
                }
                else
                    goto Lastline;
                Przepisz_Plk_Do_BazaSQL_NewBaza(DtTablePdf_FTP, DT_PDF, "Kart_Char/", ".pdf", "OstAktKC", BC_Aktual_baza, ZAcon);
            Lastline:;

                string sqlString = " UPDATE Baza_PDF SET OstAkt= NULL WHERE Img IS NULL or Img like '';  UPDATE Baza_PDF SET OstAktTDS= NULL  WHERE Tds IS NULL or Tds like '';   UPDATE Baza_PDF SET OstAktKC= NULL WHERE KC IS NULL or KC like '';    UPDATE Baza_PDF SET OstAktPDF2= NULL WHERE PDF2 IS NULL or PDF2 like '' ;";
                UsingSQLComand(sqlString, Acon);
                // Console.WriteLine(ileZ + " Zakończ");
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return Conversions.ToInteger("1");
        }
        public static int Przepisz_Plk_Do_BazaSQL_NewBaza(DataTable DT_baza_Acon, DataTable DT_FTP, string FolderFTP, string TypeFil, string TabelData, BackgroundWorker BackGrund, SQLiteConnection Zacon)
        {
            byte[] newFileData = null;
            // Console.WriteLine("AktualPlik_OFR_PDF_NewBaza - Działanie nr - " & ileZ & " / " & String.Format(" Wiersz# {0}", (New StackTrace(New StackFrame(True))).GetFrame(0).GetFileLineNumber()))
            int j = 1;
            bool ZezwDownl = true;
            LDzialanie = 1.ToString();
            LastRow = DT_FTP.Rows.Count;
            foreach (DataRow RowFTP in DT_FTP.Rows)
            {
                for (int i = 0, loopTo = DT_baza_Acon.Rows.Count - 1; i <= loopTo; i++)
                {
                    if ((Strings.Replace(Conversions.ToString(RowFTP["Name"]), " ", "") ?? "") == (Strings.Replace(Conversions.ToString(Operators.ConcatenateObject(DT_baza_Acon.Rows[i]["NAZEWNICTWO"], TypeFil)), " ", "") ?? ""))
                    {
                        if ((RowFTP["Date"].ToString() ?? "") != (DT_baza_Acon.Rows[i][TabelData].ToString() ?? "") | DBNull.Value.Equals(DT_baza_Acon.Rows[i][TabelData]) == true)
                        {
                            string url = Strim_URL + FolderFTP + RowFTP["Name"].ToString();
                            if (ZezwDownl == true)
                            {
                                newFileData = DownloadFile_URL(url);
                            }
                            ZezwDownl = false;
                            if (FolderFTP == "Img/")
                                Wstaw_wrsByte_Baza_PDF(DT_baza_Acon.Rows[i]["Lpgrup"].ToString(), DT_baza_Acon.Rows[i]["SAP"].ToString(), newFileData, default, default, default, RowFTP["Date"].ToString(), Zacon);
                            if (FolderFTP == "Pdf/")
                                Wstaw_wrsByte_Baza_PDF(DT_baza_Acon.Rows[i]["Lpgrup"].ToString(), DT_baza_Acon.Rows[i]["SAP"].ToString(), default, newFileData, default, default, RowFTP["Date"].ToString(), Zacon);
                            if (FolderFTP == "Kart_Char/")
                                Wstaw_wrsByte_Baza_PDF(DT_baza_Acon.Rows[i]["Lpgrup"].ToString(), DT_baza_Acon.Rows[i]["SAP"].ToString(), default, default, newFileData, default, RowFTP["Date"].ToString(), Zacon);
                        }

                        BackGrund.ReportProgress(IntProgres(j, LastRow));
                    }
                }
                j += 1;

                ZezwDownl = true;
            }
            return Conversions.ToInteger("1");
        }
        public static string Wstaw_wrsByte_Baza_PDF(string NrGrupa, string NrSap, byte[] Image, byte[] PTds, byte[] PKC, byte[] PPlk, string FtpTime, SQLiteConnection Zcon)
        {
            int err;
            try
            {

                string Img = "";
                string AImg = "";
                string BImg = "";
                string TimImg = "";
                string BTimImg = "";
                string BTmImg = "";
                string tds = "";
                string Atds = "";
                string Btds = "";
                string TimTds = "";
                string BTimTds = "";
                string BTmTds = "";
                string chr = "";
                string Achr = "";
                string Bchr = "";
                string Timchr = "";
                string BTimchr = "";
                string BTmchr = "";
                string Pd2 = "";
                string AP2 = "";
                string BP2 = "";
                string BTimP2 = "";
                string BTmP2 = "";
                if (Image is null == false)
                {
                    Img = ",Img=@Img";
                    AImg = ",Img";
                    BImg = ",@Img";
                    TimImg = ",OstAkt=@OstAkt";
                    BTimImg = ",@OstAkt";
                    BTmImg = ",OstAkt";
                }
                if (PTds is null == false)
                {
                    tds = ",Tds=@Tds";
                    Atds = ",Tds";
                    Btds = ",@Tds";
                    TimTds = ",OstAktTDS=@OstAkt";
                    BTimTds = ",@OstAkt";
                    BTmTds = ",OstAktTDS";
                }
                if (PKC is null == false)
                {
                    chr = ",KC=@Kc";
                    Achr = ",KC";
                    Bchr = ",@Kc";
                    Timchr = ",OstAktKC=@OstAkt";
                    BTimchr = ",@OstAkt";
                    BTmchr = ",OstAktKC";
                }
                if (PPlk is null == false)
                {
                    Pd2 = ",PDF2=@PDF2";
                    AP2 = ",KC";
                    BP2 = " ,@PDF2";
                    BTimP2 = ",@OstAkt";
                    BTmP2 = ",OstAktPDF2";
                }
                string Sqwery = @" -- Try to update any existing row
                                        UPDATE Baza_PDF
                                        SET Lpgrup=@Lpg, SAP=@SAP" + Img + tds + chr + Pd2 + TimImg + TimTds + Timchr + @"
                                        WHERE SAP Like '%" + NrSap + @"%';  
                                 --If no update happened (i.e. the row didn't exist) then insert one                                           
                                        INSERT INTO Baza_PDF (Lpgrup, SAP" + AImg + Atds + Achr + AP2 + BTmImg + BTmTds + BTmchr + BTmP2 + @")
                                        Select  @Lpg,@SAP" + BImg + Btds + Bchr + BP2 + BTimImg + BTimTds + BTimchr + BTimP2 + @"
                                        WHERE(Select Changes() = 0);";
                // Console.WriteLine(ileZ & " ----- > " & Sqwery)
                using (var cmd = new SQLiteCommand(Sqwery, Zcon))
                {
                    cmd.Parameters.Add("@Lpg", (DbType)SqlDbType.VarChar).Value = NrGrupa;
                    cmd.Parameters.Add("@SAP", (DbType)SqlDbType.VarChar).Value = NrSap;
                    cmd.Parameters.Add("@Img", (DbType)SqlDbType.Binary).Value = Image;
                    cmd.Parameters.Add("@Tds", (DbType)SqlDbType.Binary).Value = PTds;
                    cmd.Parameters.Add("@Kc", (DbType)SqlDbType.Binary).Value = PKC;
                    cmd.Parameters.Add("@Pdf2", (DbType)SqlDbType.Binary).Value = PPlk;
                    cmd.Parameters.Add("@OstAkt", (DbType)SqlDbType.VarChar).Value = FtpTime;

                    if (Zcon.State == ConnectionState.Closed)
                        Zcon.Open();
                    cmd.ExecuteNonQuery();
                }
                err = 0;

                return err.ToString();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }


        //public static int Wolkrer(int w)
        //{
        //    if (Backworker is null)
        //    {
        //        Backworker = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
        //        Backworker.DoWork += Worker_DoWork_NewBaza;
        //        Backworker.ProgressChanged += Worker_ProgressChanged_NewBaza;
        //        Backworker.RunWorkerCompleted += Worker_RunWorkerCompleted_NewBaza;
        //    }
        //    return default;
        //}
        //public static void Worker_DoWork_NewBaza(object sender, DoWorkEventArgs e)
        //{
        //    //Console.WriteLine(SendBaza);
        //    if (SendBaza == "DB_Klient")
        //        SendFileName = "Wysłano Baza klient";
        //    if (SendBaza == "DB_ZK")
        //        SendFileName = "Wysłano Baza ZK";
        //    if (SendBaza == "DB_OFR")
        //        SendFileName = "Wysłano Baza Oferty";
        //    Wyslij_Pobraną_baze_DB__StartSerwer(SendBaza + ".db", LocatiAktual + @"\" + SendBaza + ".db", (BackgroundWorker)sender);
        //}
        //private static void Worker_ProgressChanged_NewBaza(object sender, ProgressChangedEventArgs e)
        //{
        //    int _Stan = e.ProgressPercentage;
        //}
        //private static void Worker_RunWorkerCompleted_NewBaza(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if (e.Cancelled)
        //    {
        //    }

        //    // lblStatus.Foreground = Brushes.Red
        //    // lblStatus.Text = "Cancelled by user..."
        //    else
        //    {
        //        // LabInfoSen.Content = vbCrLf & LabInfoSen.Content & SendFileName
        //        // lblStatus.Foreground = Brushes.Green 
        //        // lblStatus.Text = "Done... Calc result: " & e.Result
        //        // MsgBox("wysłano")
        //    }
        //}
        //public static DataTable AddBazaKLZplkTxt_NewBaza()
        //{
        //    try
        //    {
        //        Scie_KL = @"C:\Users\" + usher + @"\AppData\SC\Dne.cws";
        //        scie_user = @"C:\Users\" + usher + @"\AppData\SC\Usr.cws";
        //        string TextLine;
        //        int i = 0;
        //        DataRow wiersze;
        //        string[] wartosc;
        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();
        //        var NewTable = new DataTable();
        //        string a0 = Upr_User.User_PH;
        //        if (System.IO.Directory.Exists(Scie_KL))
        //        {
        //            var objReader = new StreamReader(Scie_KL, Encoding.Default);
        //            wartosc = objReader.ReadLine().Split('|');
        //            try
        //            {
        //                NewTable.Columns.Add(new DataColumn("id"));
        //                NewTable.Columns.Add(new DataColumn("NIP"));
        //                NewTable.Columns.Add(new DataColumn("Stan"));
        //                NewTable.Columns.Add(new DataColumn("Numer_konta"));
        //                NewTable.Columns.Add(new DataColumn("Nazwa_klienta"));
        //                NewTable.Columns.Add(new DataColumn("Nazwa_CD"));
        //                NewTable.Columns.Add(new DataColumn("Adres"));
        //                NewTable.Columns.Add(new DataColumn("Kod_Poczta"));
        //                NewTable.Columns.Add(new DataColumn("Poczta"));
        //                NewTable.Columns.Add(new DataColumn("Forma_plac"));
        //                NewTable.Columns.Add(new DataColumn("PraceList"));
        //                NewTable.Columns.Add(new DataColumn("Branza"));
        //                NewTable.Columns.Add(new DataColumn("Tel"));
        //                NewTable.Columns.Add(new DataColumn("E_mail"));
        //                NewTable.Columns.Add(new DataColumn("Opiekun_klienta"));
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message).ToString();
        //            }
        //            wiersze = NewTable.NewRow();
        //            for (i = 0; i <= 13; i++)
        //                wiersze[i] = wartosc[i].ToString();
        //            NewTable.Rows.Add(wiersze);
        //            i = 1;
        //            while (objReader.Peek() != -1)
        //            {
        //                TextLine = "";
        //                TextLine += objReader.ReadLine();
        //                if (TextLine.Contains("|"))
        //                {
        //                    string[] splitLine = TextLine.Split('|');
        //                    var dr = NewTable.NewRow();
        //                    for (int j = 0; j <= 14; j++)
        //                    {
        //                        if (j == 14)
        //                            dr[j] = a0;
        //                        else
        //                            dr[j] = splitLine[j];
        //                    }
        //                    NewTable.Rows.Add(dr);
        //                }
        //                i += 1;
        //            }
        //            objReader.Close();
        //            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(NewTable.Rows[0][1], "NIP", false)))
        //                NewTable.Rows.Remove(NewTable.Rows[0]);
        //        }
        //        stopwatch.Stop();
        //        return NewTable;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextMessage(ex.ToString());
        //        return null;
        //    }
        //}
    }

    internal static partial class AktualBazaKlient
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

    internal static partial class SendBackGrnd
    {
        internal static BackgroundWorker BG_Send;
        public static string NazwPlkSend;
        public static string SciezPlk;
        public static FileInfo fileInfo;

        static SendBackGrnd()
        {
            NewActualBackGrund();
        }

        public static void NewActualBackGrund()
        {
            if (BG_Send is null)
            {
                BG_Send = new BackgroundWorker();
                BG_Send.DoWork += BackgroundSend_DoWork_NewBaza;
                BG_Send.RunWorkerCompleted += BackgroundSend_RunWorkerCompleted_NewBaza;
                BG_Send.ProgressChanged += BackgroundSend_ProgressChanged_NewBaza;
            }
            return;
        }

        public static void UpdateBackgroundSend_NewBaza(string Send_NazwaPlik, string Send_SciezkaPlik)
        {
            if (BG_Send is null)
                NewActualBackGrund();
            try
            {
                NazwPlkSend = Send_NazwaPlik;
                SciezPlk = Send_SciezkaPlik;
                fileInfo = new FileInfo(SciezPlk);
                Console.WriteLine("UpdateBackgroundSend_NewBaza - " + " Nazwa plik-" + NazwPlkSend + " Sciezka plik -  " + SciezPlk);
                if (Upr_User.SendDB == true)
                {
                    BG_Send.RunWorkerAsync();
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("nie wysyłam !!!").ToString();
                }


            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public static void BackgroundSend_DoWork_NewBaza(object sender, DoWorkEventArgs e)
        {
            try
            {
                string fileFullPath = e.Argument as string;
                BG_Send.WorkerReportsProgress = true;
                BackgroundWorker bw = sender as BackgroundWorker;
                e.Result = Wyslij_Pobraną_baze_DB__StartSerwer(NazwPlkSend, SciezPlk, BG_Send);
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
        private static void BackgroundSend_RunWorkerCompleted_NewBaza(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ConClose();
                string msg = null;
                if (e.Cancelled)
                {
                    MessageBox.Show("Operacja została anulowana");
                }
                else if (e.Error != null)
                {
                    msg = string.Format("Wystąpił błąd:  {0}", e.Error.Message);
                }
                else
                {
                    if (e.Result.ToString() == "1")
                        msg = string.Format("zakończono powodzeniem");
                    if (e.Result.ToString() == "2")
                        msg = string.Format("Nieoczekiwany błąd!" + Microsoft.VisualBasic.Constants.vbCrLf + "Sprawdz czy wskazano własciwy plik");
                }

                try
                {
                    // LabIleZ.Content = "";
                    // lblTime.Content = "";
                    if (AktualNewBaza.SendBaza == "DB_Klient")
                        AktualNewBaza.SendFileName = "Wysłano Baza klient";
                    if (AktualNewBaza.SendBaza == "DB_ZK")
                        AktualNewBaza.SendFileName = "Wysłano Baza ZK";
                    if (AktualNewBaza.SendBaza == "DB_OFR")
                        AktualNewBaza.SendFileName = "Wysłano Baza Oferty";


                    InfoLabelKryj.Content = "";
                 
                    
                    Dock_Aktual_LabProgr.Value = 0;
                    labelProgres.Content = "";
                    SendtBazaProgre.Value = 0;
                    LabInfoSen.Content = "";

                    MainWindow.WyslProcent.Visibility = Visibility.Collapsed;
                    ConClose();
                    MainWindow.lblTime.Content = "";
                    MainWindow.LabIleZ.Content = "";
                    MainWindow.LabProgre.Content = "";
                    MainWindow.LabInfoSen.Content = "";
                    MainWindow.AktualBazaProgre.Value = 0;
                    MainWindow.StAktua.Width = 35;


                    ; Console.WriteLine("Sent File Done - BackgroundSend_RunWorkerCompleted_NewBaza");
                    MessageBox.Show("Sent File Done");
                    Mw.VievPageVisibli(false, false, "");
                }
                catch
                {

                }
                BlokClose = true;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private static void BackgroundSend_ProgressChanged_NewBaza(object sender, ProgressChangedEventArgs e)
        {
            int _Stan = e.ProgressPercentage;

            // lblTime.Content = _Stan + "%";
            //try
            //{
                if (Dock_Aktual_Progre.Visibility == Visibility.Visible)
                {
                    Dock_Aktual_LabProgr.Value = _Stan;
                    labelProgres.Content = _Stan + "%";
                    InfoLabelKryj.Content = "Sent file";
                }
            //}
            //catch {}

            //try
            //{
            //    //if (Mw.StAktual.Width >= 40 )
            //    //{
            //    //}
                
            //    //if (MainWindow.SendtBazaProgre != null)
            //    //{
            //    //    Console.WriteLine(_Stan);
            //    //    MainWindow.SendtBazaProgre.Value = _Stan;
            //    //    MainWindow.LabInfoSen.Content = _Stan + "%";
            //    //}

            //}
            //catch
            //{ 

            //}

            ////LabIleZ.Content = AktualNewBaza.ileZ + "/" + "9";
            //if (AktualNewBaza.ileZ == 20)
            //{
            //    //lblTime.Content = "";
            //    //LabIleZ.Content = "Sent";
            //}
            //try
            //{
            //    //InfoLabelKryj.Content = "Sent file";
            //    //LabIleZ.Content = AktualNewBaza.ileZ + "/" + "9";
            //}
            //catch
            //{
            //}

        }

    }


    internal static partial class Aktual_Cennik_Z_Plik_Excel
    {
        internal static BackgroundWorker Background_Aktual_Cennik_z_Excel;
        internal static BackgroundWorker Background_AktualBazaKlient_Excel;
        private static DataTable DaneTbl;
        private static int LastRow;

        static Aktual_Cennik_Z_Plik_Excel()
        {
            NewActualBackGrund();
        }

        public static void NewActualBackGrund()
        {
            if (Background_Aktual_Cennik_z_Excel == null)
            {
                Background_Aktual_Cennik_z_Excel = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
                Background_Aktual_Cennik_z_Excel.ProgressChanged += Cennik_z_Excel_ProgressChanged;
                Background_Aktual_Cennik_z_Excel.RunWorkerCompleted += Cennik_z_Excel_RunWorkerCompleted;
                Background_Aktual_Cennik_z_Excel.DoWork += Cennik_z_Excel_DoWork;
            }

            return;
        }

        public static void ADaneCN(string New_Db)
        {
            if (Background_Aktual_Cennik_z_Excel is null)
                NewActualBackGrund();

            Console.WriteLine("ADaneCN = " + New_Db);
            if (Dcon.State == ConnectionState.Open)
                Dcon.Close();
            Dcon.ConnectionString = ConectString(New_Db, Dcon);
            WyslijCennik = true;
            AktualNewBaza.ActivFunction = 2;
            InfoLabelKryj.Content = "Aktualizuj" + Strings.Replace(New_Db, "DB_", "") + Constants.vbCrLf + " Baza zawiera dane z";
            DaneTbl = AktualBazaKlient.ImportToExCel("Wyszukaj plik Excel z bazą" + Strings.Replace(New_Db, "DB_", ""));
            Console.WriteLine("ADaneCN  DaneTbl rows count = " + DaneTbl.Rows.Count);

            if (DaneTbl.TableName == "Cancel")
            {
                Dock_Aktual_LabProgr.Value = 0;
                labelProgres.Content = "";
                WyslProcent.Visibility = Visibility.Collapsed;
                Dock_Aktual_Progre.Visibility = Visibility.Collapsed;
                InfoStackPanelkryj.Visibility = Visibility.Collapsed;

                return;
            }
            int max = DaneTbl.Rows.Count;
            object[] parameters = new object[1] { New_Db };
            Background_Aktual_Cennik_z_Excel.RunWorkerAsync(parameters);

        }
        private static void Cennik_z_Excel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int _Stan = e.ProgressPercentage;
                lblTime.Content = _Stan + "%";
                Dock_Aktual_LabProgr.Value = _Stan;
           
                labelProgres.Content = _Stan + "%";
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
        private static void Cennik_z_Excel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            try
            {
                string msg = null;
                if (e.Cancelled || e.Result.ToString() == "4")
                {
                    msg = string.Format("Operacja została anulowana");
                }
                else if (e.Error != null)
                {
                    msg = string.Format("Wystąpił błąd: {0}", e.Error.Message);
                }
                else
                {
                    if (e.Result.ToString() == "1" || e.Result.ToString() == "2")
                    {

                        if (e.Result.ToString() == "1")
                            SendBackGrnd.UpdateBackgroundSend_NewBaza("DB_Cennik.db", LocatiAktual + @"\DB_Cennik.db");
                        if (e.Result.ToString() == "2")
                            SendBackGrnd.UpdateBackgroundSend_NewBaza("DB_Klient.db", LocatiAktual + @"\DB_Klient.db");
                        if (Dcon.State == ConnectionState.Open)
                            Dcon.Close();
                        if (Acon.State == ConnectionState.Open)
                            Acon.Close();
                        msg = string.Format("Zakończono!");
                    }
                    if (e.Result.ToString() == "3")
                        msg = string.Format("Nieoczekiwany błąd!" + Constants.vbCrLf + "Sprawdz czy wskazano własciwy plik");
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private static void Cennik_z_Excel_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            int arg = 0;
            object[] parameters = (object[])e.Argument;
            string Ctr_Name = parameters[0].ToString();


            if (parameters[0].ToString() == "DB_Cennik")
                e.Result = WykonajDlugaOperacja_przepisania_cennika_z_ExcelCennik(bw, arg);
            if (parameters[0].ToString() == "DB_Klient")
                e.Result = WykonajDlugaOperacja_przepisania_BazaKlient(bw, arg);
            if (bw.CancellationPending)
                e.Cancel = true;
        }

        private static int WykonajDlugaOperacja_przepisania_cennika_z_ExcelCennik(BackgroundWorker bw, int sleepPeriod)
        {
            int result = 0;

            Tim = TimeAktual();
            URLstatus = FVerificaConnessioneInternet();
            if (URLstatus == false)
            {
                Interaction.MsgBox("brak połaczenia z internetem" + Constants.vbCrLf + " Sprawdz połączenie!");
                return result;

            }
            if (Acon.State == ConnectionState.Closed)
                Acon.Open();
            if (Dcon.State == ConnectionState.Closed)
                Dcon.Open();
            string a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18;
            bool exit = false;
            bool exitWhile = false;
            while (!bw.CancellationPending)
            {
                if (exitWhile)
                    break;
                if (exit)
                    break;

                string sqlqwerty = "Update Cennik Set  CDM ='',CK='',PH='',ZPR0='';";
                UsingSQLComand(sqlqwerty, Dcon);
                LastRow = 859;

                if (DaneTbl.Columns.Count < 18)
                    goto line1;
                foreach (DataColumn col in DaneTbl.Columns)
                {

                    if (col.ColumnName.Contains("Produkty + KOD"))
                        col.ColumnName = "ProdKod";
                    if (col.ColumnName.Contains("Upo"))
                        col.ColumnName = "Lpgrup";
                    if (col.ColumnName.Contains("Nag"))
                        col.ColumnName = "Naglowek";
                    if (col.ColumnName.Contains("nr p"))
                        col.ColumnName = "SAP";
                    if (col.ColumnName.Contains("NAZWA PROD"))
                        col.ColumnName = "NazwProd";
                    if (col.ColumnName.Contains("karton"))
                        col.ColumnName = "Kszt";
                    if (col.ColumnName.Contains("Paleta"))
                        col.ColumnName = "Pszt";
                    if (col.ColumnName.Contains("Pojem"))
                        col.ColumnName = "Poj";
                    if (col.ColumnName.Contains("Jednostka"))
                        col.ColumnName = "Miara";
                    if (col.ColumnName == "KO")
                        col.ColumnName = "CK";
                    if (col.ColumnName == "ZPR 0")
                        col.ColumnName = "ZPR0";
                    if (col.ColumnName.Contains("kod poza"))
                        col.ColumnName = "BrakPrace";
                }

                for (int i = 0, loopTo = DaneTbl.Rows.Count - 1; i <= loopTo; i++)
                {
                    if (DaneTbl.Columns.Contains("SAP") != true)
                    {
                        MessageBox.Show("Niewłasciwy plik !!! Wybierz ponownie ");
                        result = 4;
                        goto Lastline;
                    }
                    if (exit)
                    {
                        exitWhile = true;
                        break;
                    }

                    var withBlock = DaneTbl;
                    {
                        var withBlock1 = withBlock.Rows[i];
                        if (i > 3 & withBlock1["SAP"].ToString() == "")
                            break;

                        a1 = withBlock1["ProdKod"].ToString();
                        a2 = Encode(withBlock1["Naglowek"].ToString()); 
                        a3 = withBlock1["Lpgrup"].ToString(); 
                        a4 = withBlock1["SAP"].ToString(); 
                        a5 = Encode(withBlock1["NazwProd"].ToString()); 
                        a6 = withBlock1["Kszt"].ToString(); 
                        a7 = withBlock1["Pszt"].ToString();
                        a8 = withBlock1["Poj"].ToString(); 
                        a9 = withBlock1["Miara"].ToString(); 
                        a10 = withBlock1["Kolor"].ToString(); 
                        double.TryParse(withBlock1["CDM"].ToString(), out double Outdob);
                        a11 = Math.Round(Outdob, 2, MidpointRounding.AwayFromZero).ToString();
                        double.TryParse(withBlock1["CK"].ToString(), out double Outdob1);
                        a12 = Math.Round(Outdob1, 2, MidpointRounding.AwayFromZero).ToString();
                        double.TryParse(withBlock1["PH"].ToString(), out double Outdob2);
                        a13 = Math.Round(Outdob2, 2, MidpointRounding.AwayFromZero).ToString();
                        double.TryParse(withBlock1["ZPR0"].ToString(), out double Outdob3);
                        a14 = Math.Round(Outdob3, 2, MidpointRounding.AwayFromZero).ToString();

                        a15 = withBlock1["GRUPA"].ToString(); 
                        a16 = withBlock1["KATEGORIA"].ToString(); 
                        a17 = withBlock1["NAZEWNICTWO"].ToString(); 
                    }
                    a18 = "";
                    for (int c = 0, loopTo1 = DaneTbl.Rows.Count - 1; c <= loopTo1; c++)
                    {
                        if (a4 != "" & a4 == withBlock.Rows[c]["BrakPrace"].ToString())
                            a18 = "BrakPrace";
                    }

                    string stringqwert = @" -- Try to update any existing row
                                    UPDATE Cennik
                                    SET ProdKod = '" + a1 + "',Naglowek='" + a2 + "',Lpgrup='" + a3 + "', NazwProd = '" + a5 + "',Kszt='" + a6 + "',Pszt='" + a7 + "',Poj='" + a8 + "',Miara='" + a9 + "', Kolor='" + a10 + "' ,     CDM='" + a11 + "',CK='" + a12 + "',PH='" + a13 + "',ZPR0='" + a14 + "',GRUPA='" + a15 + "',KATEGORIA='" + a16 + "',NAZEWNICTWO='" + a17 + "',BrakPrace='" + a18 + "',OstAkt='" + Tim + "'                           where SAP like '%" + a4 + @"%';
                                -- If no update happened (i.e. the row didn't exist) then insert one                                         
                                    INSERT INTO Cennik  (ProdKod,Naglowek,Lpgrup,SAP,NazwProd,Kszt,Pszt,Poj,Miara,Kolor,CDM,CK,PH,ZPR0,GRUPA,KATEGORIA,NAZEWNICTWO,BrakPrace,OstAkt)
                                    SELECT '" + a1 + "','" + a2 + "','" + a3 + "','" + a4 + "','" + a5 + "','" + a6 + "','" + a7 + "','" + a8 + "','" + a9 + "','" + a10 + "','" + a11 + "','" + a12 + "','" + a13 + "','" + a14 + "','" + a15 + "','" + a16 + "','" + a17 + "','" + a18 + "','" + Tim + "'                    WHERE (Select Changes() = 0);";
                    UsingSQLComand(stringqwert, Dcon);

                    Background_Aktual_Cennik_z_Excel.ReportProgress(IntProgres(i, LastRow));


                }



                result = 1;
                exit = true;
                goto Lastline;
            line1:;

                sqlqwerty = "Update Cennik Set  CDM ='',CK='',PH='',ZPR0='';";
                UsingSQLComand(sqlqwerty, Dcon);
                foreach (DataColumn col in DaneTbl.Columns)
                {

                    if (col.ColumnName.Contains("nr") == true && col.ColumnName.Contains("produktu") || col.ColumnName.Contains("Material") == true)
                        col.ColumnName = "SAP";
                    if (col.ColumnName.Contains("CDM") == true || col.ColumnName.Contains("Margin") == true)
                        col.ColumnName = "CDM";
                    if (col.ColumnName.Contains("KO") == true || col.ColumnName.Contains("Reg") == true)
                        col.ColumnName = "KO";
                    if (col.ColumnName.Contains("PH") == true || col.ColumnName.Contains("Sales") == true)
                        col.ColumnName = "PH";
                    if (col.ColumnName.Contains("ZPR") == true || col.ColumnName.Contains("ZPR") == true)
                        col.ColumnName = "ZPR0";
                }


                for (int i = 0, loopTo = DaneTbl.Rows.Count - 1; i <= loopTo; i++)
                {

                    {
                        var withBlock = DaneTbl;
                        {
                            var withBlock1 = withBlock.Rows[i];
                            a4 = withBlock1["SAP"].ToString();
                            double.TryParse(withBlock1["CDM"].ToString(), out double Outdob);
                            a11 = Math.Round(Outdob, 2, MidpointRounding.AwayFromZero).ToString();
                            double.TryParse(withBlock1["KO"].ToString(), out double Outdob1);
                            a12 = Math.Round(Outdob1, 2, MidpointRounding.AwayFromZero).ToString();
                            double.TryParse(withBlock1["PH"].ToString(), out double Outdob2);
                            a13 = Math.Round(Outdob2, 2, MidpointRounding.AwayFromZero).ToString();
                            double.TryParse(withBlock1["ZPR0"].ToString(), out double Outdob3);
                            a14 = Math.Round(Outdob3, 2, MidpointRounding.AwayFromZero).ToString();
                        }
                    }
                    string sqlString = "Update Cennik Set  CDM ='" + a11 + "',CK='" + a12 + "',PH='" + a13 + "',ZPR0='" + a14 + "',OstAkt='" + Tim + "'     where SAP like '%" + a4 + "%'";

                    if (a4 != "")
                        UsingSQLComand(sqlString, Dcon);

                    Background_Aktual_Cennik_z_Excel.ReportProgress(IntProgres(i, LastRow));
                }
                result = 1;
            Lastline:;
                if (exitWhile)
                    break;

                exit = true;
                if (exit)
                    break;
            }
            return result;
        }

        private static int WykonajDlugaOperacja_przepisania_BazaKlient(BackgroundWorker bw, int sleepPeriod)
        {

            int result = 0;

            Tim = TimeAktual();
            while (!bw.CancellationPending)
            {
                bool exit = false;
                try
                {
                    string a0, a1, a2, a3, a4, a5, a6, a7, a8, a10, a11, a12;
                    bool b0 = false; bool b1 = false; bool b2 = false; bool b3 = false; bool b4 = false; bool b5 = false; bool b6 = false; bool b7 = false; bool b8 = false; bool b9 = false; bool b10 = false; bool b11 = false; bool b12 = false; bool b13 = false;
                    LastRow = DaneTbl.Rows.Count;
                    foreach (DataColumn col in DaneTbl.Columns)
                    {

                        if (col.ColumnName.Contains("Opiekun klienta") | col.ColumnName.Contains("Account Manager"))
                        {
                            col.ColumnName = "Opiekun_klienta";
                            b0 = true;
                        }
                        if (col.ColumnName.Contains("NIP"))
                        {
                            col.ColumnName = "NIP";
                            b1 = true;
                        }
                        if (col.ColumnName.Contains("Stan"))
                        {
                            col.ColumnName = "Stan";
                            b2 = true;
                        }
                        if (col.ColumnName.Contains("Numer"))
                        {
                            col.ColumnName = "Numer_konta";
                            b3 = true;
                        }
                        if (col.ColumnName.Contains("Nazwa klienta") | col.ColumnName.Contains("Nazwa firmy"))
                        {
                            col.ColumnName = "Nazwa_klienta";
                            b4 = true;
                        }
                        if (col.ColumnName.Contains("Nazwa cd"))
                        {
                            col.ColumnName = "Nazwa_cd";
                            b5 = true;
                        }
                        if (col.ColumnName.Contains("ulica") | col.ColumnName.Contains("Ulica"))
                        {
                            col.ColumnName = "Adres";
                            b6 = true;
                        }
                        if (col.ColumnName.Contains("kod pocztowy") | col.ColumnName.Contains("Kod pocztowy"))
                        {
                            col.ColumnName = "Kod_poczta";
                            b7 = true;
                        }
                        if (col.ColumnName.Contains("miasto") | col.ColumnName.Contains("Miasto"))
                        {
                            col.ColumnName = "Poczta";
                            b8 = true;
                        }
                        if (col.ColumnName.Contains("Price"))
                        {
                            col.ColumnName = "PraceList";
                            b9 = true;
                        }
                        if (col.ColumnName.Contains("Customer"))
                        {
                            col.ColumnName = "Branza";
                            b10 = true;
                        }
                        if (col.ColumnName.Contains("Telefon"))
                        {
                            col.ColumnName = "Tel";
                            b11 = true;
                        }
                        if (col.ColumnName.Contains("E mail"))
                        {
                            col.ColumnName = "E_mail";
                            b12 = true;
                        }
                    }



                    for (int i = 0, loopTo = DaneTbl.Rows.Count - 1; i <= loopTo; i++)
                    {
                        if (exit == true)
                        {
                            return result;
                        }
                        {
                            var withBlock = DaneTbl.Rows[i];
                            a0 = withBlock["Opiekun_klienta"].ToString();
                            a1 = withBlock["NIP"].ToString();

                            a3 = withBlock["Numer_konta"].ToString();
                            a2 = "";
                            if (Information.IsNumeric(a3))
                            {
                                if (Conversions.ToDouble(a3.Substring(0, 1)) == 1)
                                    a2 = "Aktywny";
                                if (Conversions.ToDouble(a3.Substring(0, 1)) == 2)
                                    a2 = "Nieaktywne";
                                if (Conversions.ToDouble(a3.Substring(0, 1)) == 9)
                                {
                                    a2 = "Potencjalny";
                                    string serch = SqlRoader_Jedna_wartosc("select Numer_konta from BazaKL WHERE NIP like '" + a1 + "';", Dcon);//, "serch potenc");
                                    if (serch != null)
                                    {
                                        if (!string.IsNullOrEmpty(serch.ToString()))
                                        {
                                            if (Strings.Replace(serch, " ", "").Substring(0, 1).ToString() == "1")
                                                goto lastline;
                                        }
                                    }
                                }
                            }

                            a4 = withBlock["Nazwa_klienta"].ToString();
                            if (b5 == true)
                                a5 = withBlock["Nazwa_cd"].ToString();
                            else
                                a5 = "";
                            a6 = withBlock["Adres"].ToString();
                            a7 = withBlock["Kod_poczta"].ToString();
                            a8 = withBlock["Poczta"].ToString();

                            if (b9 == true)
                                a10 = withBlock["PraceList"].ToString();
                            else
                                a10 = "";
                            a11 = withBlock["Branza"].ToString();
                            a12 = withBlock["Tel"].ToString();

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

                        if (Information.IsNumeric(a3) & a3.Length > 2)
                        {
                            if (Conversions.ToDouble(a3.Substring(0, Math.Min(a3.Length, 1))) == 1d | Conversions.ToDouble(a3.Substring(0, Math.Min(a3.Length, 1))) == 9d)
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
                                             PraceList='" + a10 + "', Branza='" + a11 + "' , Tel='" + a12 + "' ,   OstAkt='" + Tim + @"' 
                                    WHERE  NIP like '%" + a1 + @"%' ;
                                
                                -- If no update happened (i.e. the row didn't exist) then insert one
                                    INSERT INTO BazaKl (Opiekun_klienta,NIP,Stan,Numer_konta,Nazwa_klienta,Nazwa_CD,Adres,Kod_poczta,Poczta,PraceList,Branza,Tel,OstAkt) 
                                    SELECT '" + a0 + "','" + a1 + "','" + a2 + "','" + a3 + "','" + a4 + "','" + a5 + "','" + a6 + "','" + a7 + "','" + a8 + "','" + a10 + "','" + a11 + "','" + a12 + "','" + Tim + @"'
                                WHERE (Select Changes() = 0);";
                                //Console.WriteLine(sqlstring);
                                UsingSQLComand(sqlstring, Dcon);
                            }
                        }

                    lastline:
                        ;

                        try
                        {

                            Background_Aktual_Cennik_z_Excel.ReportProgress(IntProgres(i, LastRow));
                        }
                        catch
                        {
                        }
                    }
                    result = 2;
                    exit = true;
                }
                catch
                {
                    result = 3;
                    exit = true;
                }
                if (exit)
                    break;
            }
            return result;
        }

    }

    internal static partial class Aktual_Baza_Zakupy_Z_Excel
    {
        internal static BackgroundWorker Background_Aktual_Zakupy_Excel;
        private static DataTable DaneTbl;
        private static int LastRow;
        private static DataTable DaneTblZKP;

        static Aktual_Baza_Zakupy_Z_Excel()
        {
            NewActualBackGrund();
        }

        public static void NewActualBackGrund()
        {
            if (Background_Aktual_Zakupy_Excel is null)
            {                      
                Background_Aktual_Zakupy_Excel = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
                Background_Aktual_Zakupy_Excel.ProgressChanged += Zakupy_Z_Excel_ProgressChanged;
                Background_Aktual_Zakupy_Excel.RunWorkerCompleted += Zakupy_Z_Excel_RunWorkerCompleted;
                Background_Aktual_Zakupy_Excel.DoWork += Zakupy_Z_Excel_DoWork;
            }
            return;
        }

        private static void Zakupy_Z_Excel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int _Stan = e.ProgressPercentage;
                lblTime.Content = _Stan + "%";
                Dock_Aktual_LabProgr.Value = _Stan;
                labelProgres.Content = _Stan + "%";
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
        private static void Zakupy_Z_Excel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                if (e.Cancelled)
                {
                    MessageBox.Show("Operacja została anulowana");
                }
                else if (e.Error != null)
                {
                    string msg = string.Format("Wystąpił błąd:  {0}", e.Error.Message.ToString()); // Podczas operacji wystąpił błąd.
                    MessageBox.Show(msg);
                }
                else
                {
                    Console.WriteLine(e.Result);
                    string msg = "";
                    if (e.Result.ToString() == "1")
                    {
                        int answer;
                        DaneTblZKP = SqlComandDatabase_NewBaza("select * from BazaZKP", Dcon);
                        NextExcel();
                        if (!string.IsNullOrEmpty(AktualNewBaza.ZapisWys))
                            answer = (int)Interaction.MsgBox("Baza zawiera dane" + Constants.vbCrLf + AktualNewBaza.ZapisWys + Constants.vbCrLf + "kontynuować aktualizację?", Constants.vbOKCancel);
                        else
                            answer = (int)Interaction.MsgBox("kontynuować aktualizację?", Constants.vbOKCancel);
                        if (answer == (int)Constants.vbOK)
                        {
                            Continue1("Yes");
                        }
                        else
                        {
                            Continue1("No");
                            if (Dcon.State == ConnectionState.Open)
                                Dcon.Close();
                            if (Acon.State == ConnectionState.Open)
                                Acon.Close();
                            SendBackGrnd.UpdateBackgroundSend_NewBaza("DB_ZAKUPY.db", LocatiAktual + @"\DB_ZAKUPY.db");
                        }
                    }
                    else
                    {
                    }
                    if (e.Result.ToString() == "2")
                        msg = string.Format("Nieoczekiwany błąd!" + Constants.vbCrLf + "Sprawdz czy wskazano własciwy plik");
                    if (!string.IsNullOrEmpty(msg))
                        MessageBox.Show(msg);
                }
                InfoLabelKryj.Content = " Czekaj ";
                labelProgres.Content = "";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }

        }
        private static void Zakupy_Z_Excel_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (Dcon.State == ConnectionState.Open)
                    Dcon.Close();
                Dcon.ConnectionString = ConectString("DB_ZAKUPY", Dcon);
                BackgroundWorker bw = sender as BackgroundWorker;
                e.Result = TimeConsumingOperationZKP(bw);
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
        public static void ADaneBazaZKP()
        {

            Console.WriteLine("ADaneBazaZKP = " + Mw.NameLabel);
            if (Background_Aktual_Zakupy_Excel is null)
                NewActualBackGrund();
            if (Dcon.State == ConnectionState.Open)
                Dcon.Close();
            Dcon.ConnectionString = ConectString("DB_ZAKUPY", Dcon);
            try
            {
                WyslProcent.Visibility = Visibility.Visible;
                InfoLabelKryj.Content = "";

                if (URLstatus == false)
                {
                    Interaction.MsgBox("brak połaczenia z internetem" + Constants.vbCrLf + " Sprawdz połączenie!");
                    return;
                }
                File.Copy(FullPath, AktualFullPath, true);

                if (Acon.State == ConnectionState.Closed)
                    Acon.Open();
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                DaneTblZKP = SqlComandDatabase_NewBaza("Select * from BazaZKP", Dcon);
                // Console.WriteLine("ADaneBazaZKP DaneTblZKP rows count = " + DaneTblZKP.Rows.Count);
                Continue1("Yes");
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.ToString());
                TextMessage(ex.ToString());
            }
        }
        private static void Continue1(string Y_N1)
        {
            try
            {
                /// Console.WriteLine("continue " + Y_N1);
                if (Y_N1 == "Yes")
                {
                    var dt = new DataTable();
                    string SqlQwery = "Select Yearbilling from BazaZKP GROUP by Yearbilling ;";
                    dt = SqlComandDatabase_NewBaza(SqlQwery, Dcon);
                    URLstatus = FVerificaConnessioneInternet();
                    WyslProcent.Visibility = Visibility.Visible;
                    InfoLabelKryj.Content = "Aktualizuj Baza Zakupy" + Constants.vbCrLf + " Baza zawiera dane z";
                    foreach (DataRow row in dt.Rows)
                        InfoLabelKryj.Content = (object)(InfoLabelKryj.Content.ToString() + Constants.vbCrLf, row[0].ToString());
                    DaneTbl = AktualBazaKlient.ImportToExCel("Wyszukaj plik Excel z bazą zakupów klientów");
                    if (DaneTbl.TableName == "Cancel")
                    {
                        Dock_Aktual_LabProgr.Value = 0;
                        labelProgres.Content = "";
                        WyslProcent.Visibility = Visibility.Collapsed;
                        Dock_Aktual_Progre.Visibility = Visibility.Collapsed;
                        InfoStackPanelkryj.Visibility = Visibility.Collapsed;
                        return;
                    }


                    if (DaneTbl != null && DaneTbl.Rows.Count > 0)
                        Background_Aktual_Zakupy_Excel.RunWorkerAsync(2000);
                    else
                        return;
                }
                else
                {
                    AktualNewBaza.ActivFunction = 0;
                    Interaction.MsgBox("Rekord wstawiony pomyślnie");
                    if (Acon.State == ConnectionState.Open)
                        Acon.Close();
                    if (Dcon.State == ConnectionState.Open)
                        Dcon.Close();
                    ConClose();



                }
            }
            catch (Exception ex)
            {
                if (Acon.State == ConnectionState.Open)
                    Acon.Close();
                if (Dcon.State == ConnectionState.Open)
                    Dcon.Close();
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public static int TimeConsumingOperationZKP(BackgroundWorker bw)
        {
            Tim = TimeAktual();
            int result = 0;
            string SqweryDelete;
            string Ds;
            if (Operators.CompareString(DaneTbl.Rows[0]["Year-billing"].ToString(), "2010", false) > 0 & Operators.CompareString(DaneTbl.Rows[0]["Year-billing"].ToString(), "2040", false) < 0)
                Ds = DaneTbl.Rows[0]["Year-billing"].ToString();
            else
                Ds = DaneTbl.Rows[1]["Year-billing"].ToString();
            Interaction.MsgBox(Ds);
            SqweryDelete = "delete from BazaZKP WHERE Yearbilling Like '%" + Ds + "%' ";
            if (Acon.State == ConnectionState.Closed)
                Acon.Open();
            if (Dcon.State == ConnectionState.Closed)
                Dcon.Open();
            UsingSQLComand(SqweryDelete, Dcon);
            LastRow = DaneTbl.Rows.Count;
            while (!bw.CancellationPending)
            {
                foreach (DataColumn col in DaneTbl.Columns)
                {
                    if (col.ColumnName == "Year-billing")
                        col.ColumnName = "Yearbilling";
                    if (col.ColumnName == "Date-billing")
                        col.ColumnName = "Datebilling";
                    if (col.ColumnName == "SoldTo customer")
                        col.ColumnName = "SoldTocustomer";
                    if (col.ColumnName == "Document-billing")
                        col.ColumnName = "Document_Billing";
                    if (col.ColumnName == "Order Item")
                        col.ColumnName = "Order_Item";
                    if (col.ColumnName == "Sales P")
                        col.ColumnName = "SalesP";

                }
                int i = 0;
                foreach (DataRow row in DaneTbl.Rows)
                {
                    AktualNewBaza.BazaZAKUPY_Aktual_NewBaza(row, Tim, Dcon);
                    i += 1;

                    Background_Aktual_Zakupy_Excel.ReportProgress(IntProgres(i, LastRow));
                }
                string sqlqwery = " delete From BazaZKP Where Yearbilling Like '' ";
                UsingSQLComand(sqlqwery, Dcon);
                result = 1;
                return result;
            }
            return result;
        }
        private static void NextExcel()
        {
            //Console.WriteLine("NextExcel");
            try
            {
                AktualNewBaza.ZapisWys = "";
                for (int i = 0, loopTo = DaneTblZKP.Rows.Count - 1; i <= loopTo; i++)
                {
                    int ka = 0;
                    string wyswietl = DaneTblZKP.Rows[i]["Yearbilling"].ToString();
                    if (string.IsNullOrEmpty(AktualNewBaza.ZapisWys))
                        AktualNewBaza.ZapisWys = wyswietl;
                    string[] testArray = Strings.Split(AktualNewBaza.ZapisWys, " ");
                    for (int j = 0, loopTo1 = testArray.Length - 1; j <= loopTo1; j++)
                    {
                        if ((testArray[j] ?? "") != (wyswietl ?? ""))
                            ka += 1;
                        if (ka - 1 == testArray.Length - 1)
                            AktualNewBaza.ZapisWys = AktualNewBaza.ZapisWys + " " + wyswietl;
                    }
                }
                ;
            }

            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
    }
}
