using SoldCalc.Supporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc
{
    public partial class LoadingData : UserControl
    {
        private BackgroundWorker _LoadingDataUploadBaseAll;
        public static ProgressBar Load_Progre;
        public static ProgressBar Load_Progresinf;
        public static DockPanel Start_Panel_Czeka;
        public static bool rp;
        private LoadingData MeCtr;

        public virtual BackgroundWorker LoadingDataUploadBaseAll
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _LoadingDataUploadBaseAll;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_LoadingDataUploadBaseAll != null)
                {
                    _LoadingDataUploadBaseAll.RunWorkerCompleted -= Start_Wczytaj_Baza_RunWorkerCompleted;
                    _LoadingDataUploadBaseAll.ProgressChanged -= Start_Wczytaj_Baza_ProgressChanged;
                    _LoadingDataUploadBaseAll.DoWork -= Start_Wczytaj_Baza_DoWork;
                    //Console.WriteLine("MethodImplOptions 1" + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                }

                _LoadingDataUploadBaseAll = value;
                if (_LoadingDataUploadBaseAll != null)
                { 
                    _LoadingDataUploadBaseAll.RunWorkerCompleted += Start_Wczytaj_Baza_RunWorkerCompleted;
                    _LoadingDataUploadBaseAll.ProgressChanged += Start_Wczytaj_Baza_ProgressChanged;
                    _LoadingDataUploadBaseAll.DoWork += Start_Wczytaj_Baza_DoWork;
                    //Console.WriteLine("MethodImplOptions 2 " + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                }
            }
        }
        public LoadingData()
        {
            InitializeComponent();
            Mw.VisibilityBlockApp();
            Mw.VievPageVisibli(false, false, "");
            Mw.LDLicz += 1;
            MeCtr = this;
            Start_Panel_Czeka = Start_Panel_Czekaj;
            Load_Progre = Load_Progres;
            Load_Progresinf = Load_ProgresInfo;
            rp = true;
            if (LoadingDataUploadBaseAll is null)
            {
                LoadingDataUploadBaseAll = new BackgroundWorker();
                LoadingDataUploadBaseAll.RunWorkerCompleted += Start_Wczytaj_Baza_RunWorkerCompleted;
                LoadingDataUploadBaseAll.ProgressChanged += Start_Wczytaj_Baza_ProgressChanged; // 3
                LoadingDataUploadBaseAll.DoWork += Start_Wczytaj_Baza_DoWork;
                LoadingDataUploadBaseAll.RunWorkerAsync();
            }
        }

        private void Start_Wczytaj_Baza_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
         
            string stringEx = "select * from BazaErr group by Err";
            System.Data.DataTable dataBaza = SqlComandDatabase(stringEx, con);
            try
            {
                if (dataBaza.Rows.Count > 0)
                {
                    string strtext = Stringerr(dataBaza);
                    dataBaza.Dispose();
                    if (!string.IsNullOrEmpty(strtext))
                    {
                        SendEmEX(Upr_User.User_PH, strtext);
                        strtext = null;
                    }
                    string strtext_er = "delete FROM BazaErr";
                    UsingSQLComand(strtext_er, con);
                }
            }
            catch
            {
                string strtext_er = "delete FROM BazaErr";
                UsingSQLComand(strtext_er, con);
            }
            if (e.Result.ToString() == "0" && e.Result != null)
            {
                Mw.VievPageVisibli(false, true, "1");
                NEW_UpdateURL();
            }
            else
            {

            }
            // Console.WriteLine("e.Result.ToString() = " + e.Result.ToString());

            // Console.WriteLine("Start_Wczytaj_Baza_RunWorkerCompleted " + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
            rp = false;
            dataBaza = default;
            Mw.StPH.DataContext = Upr_User;

                  
            Mw.VievPageVisibli(false, false, "");
            Mw.SerchCennik = true;
            Mw.InfoLab.Content = "";
            Mw.Panel_Pierwsze_Logowanie.Visibility = Visibility.Collapsed;
            Mw.Panel_Pierwsze_Logowanie.Children.Remove(this);
        }
        private string Stringerr(DataTable DB)
        {
            string strtext = null;
            try
            {
                if (DB.Rows.Count > 0)
                {
                    foreach (DataRow row in DB.Rows)
                    {
                        string Str = row["PH"].ToString() + Microsoft.VisualBasic.Constants.vbCrLf + row["data"].ToString() + Microsoft.VisualBasic.Constants.vbCrLf + row["Err"].ToString();
                        strtext = strtext + Microsoft.VisualBasic.Constants.vbCrLf + Str;
                    }
                }
            }
            catch
            { }
            return strtext;
        }
       private void Start_Wczytaj_Baza_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int _Stan = e.ProgressPercentage;
            if (_Stan == 101)
            {

            }
            if (_Stan < 100)
            {
                string Str = Info_Proces.Content.ToString();
                if (_Stan == 25 && !Str.Contains("ListKlient.Count"))
                {
                    Str = Str + Microsoft.VisualBasic.Constants.vbCrLf + "ListKlient.Count " + Mw.ListKlient.Count;
                    Mw.StPH.DataContext = Upr_User;
                 
                }


                if (_Stan == 40 && !Str.Contains("ListCennik.Count"))
                    Str = Str + Microsoft.VisualBasic.Constants.vbCrLf + "ListCennik.Count " + Mw.ListCennik.Count;
                if (_Stan == 50 && !Str.Contains("Upr_User.MinData"))
                    Str = Str + Microsoft.VisualBasic.Constants.vbCrLf + "Upr_User.MinData " + Upr_User.MinData + " < -- > " + "Upr_User.MaxData " + Upr_User.MaxData;
                if (_Stan == 70 && !Str.Contains("Zkp.Rows.Count"))
                    Str = Str + Microsoft.VisualBasic.Constants.vbCrLf + "Zkp.Rows.Count " + Zkp.Rows.Count;
                if (_Stan == 90 && !Str.Contains("Zkp2.Rows.Count"))
                {
                    Str = Str + Microsoft.VisualBasic.Constants.vbCrLf + "Zkp2.Rows.Count " + Zkp2.Rows.Count;
                    //Mw.Panel_Pierwsze_Logowanie.Visibility = Visibility.Collapsed;
                  
                }
                Load_Progre.Value = _Stan;
                Info_Proces.Content = Str;
            }

            if (_Stan >= 200)
            {                      
                Load_Progresinf.Value = _Stan - 200;
            }

            if (_Stan == 102)
            {
                ShowCenni.Visibility = Visibility.Visible;
                Load_Progresinf.Value = 0;
            }
            if (_Stan == 103)
            {
                PodgladHis.Visibility = Visibility.Visible;
                Load_Progresinf.Value =0;
            }
            if (_Stan == 104)
            {
                Load_Progresinf.Value = 0;
                Start_Panel_Czekaj.Visibility = Visibility.Collapsed;
                ShowOF.Visibility = Visibility.Visible;
                Mw.Main.Content = new Klient();
                Mw.VievPageVisibli(false, false, "");
                Mw.Panel_Pierwsze_Logowanie.Visibility = Visibility.Collapsed;
            }
        }

        private void Start_Wczytaj_Baza_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LoadingDataUploadBaseAll.WorkerReportsProgress = true;
                BackgroundWorker bw = sender as BackgroundWorker;
                e.Result = WczytajBazaKL();
                rp = false;
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
        public int WczytajBazaKL()
        {
            int result = 0;
            if (rp != true)
                return result;
            try
            {
                ConOpen();
            }
            catch
            {
                Mw.ErrNamber = "1";
            }
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                int last = WczytajBazaklient();
                Console.WriteLine("last - " + last);
                if (last == 0 || last <= 2)
                {
                    result = 0;
                    Console.WriteLine("last == 0  - " + last);
                    return result;
                }
                else
                    result = 1;


                LoadingDataUploadBaseAll.ReportProgress(25);
                LoadingDataUploadBaseAll.ReportProgress(101);
            }
            catch { }
            stopwatch.Stop();

            try
            {
                stopwatch.Restart(); stopwatch.Start();
                // Console.WriteLine("Cennik Start - Time elapsed: {0}", stopwatch.Elapsed);
                int LastItem = WczytajCennik();
            }
            catch { }

            LoadingDataUploadBaseAll.ReportProgress(102);
            LoadingDataUploadBaseAll.ReportProgress(40);
            stopwatch.Stop();
            stopwatch.Restart();
            stopwatch.Start();
            try
            {
                Upr_User.MaxData = SqlRoader_Jedna_wartosc(StringComand.ReturnComandMaxData(), con);
                Upr_User.MinData = SqlRoader_Jedna_wartosc(StringComand.ReturnComandMinData(), con);
                Upr_User.MaxDataRok = Upr_User.MaxData.Substring(0, 4);
                LoadingDataUploadBaseAll.ReportProgress(50); 
            }
            catch
            {
                // Console.WriteLine(" MaxData {0} MinData {1}", SqlRoader_Jedna_wartosc(ReturnComandMaxData, con), SqlRoader_Jedna_wartosc(ReturnComandMinData, con))
                Mw.ErrNamber = Mw.ErrNamber + "- 4";
            }

            do
            {
                try
                {
                    Upr_User.MaxD = int.Parse(Microsoft.VisualBasic.Strings.Mid(Upr_User.MaxData, 1, 4));
                    Upr_User.MinD = int.Parse(Microsoft.VisualBasic.Strings.Mid(Upr_User.MinData, 1, 4));
                    if (Upr_User.MaxD - 3 > Upr_User.MinD)
                        Upr_User.O_Data = Upr_User.MaxD - 3;
                    else
                        Upr_User.O_Data = Upr_User.MinD;
                    // Console.WriteLine("wiersz 974 Main Wind O_Data = {0} {1} {2}", Upr_User.O_Data, Upr_User.MaxD, Upr_User.MinD)
                    if (Upr_User.MaxData == Upr_User.MinData)
                        break;
                    string SqlComand = StringComand.ComandBranzaSQL(Upr_User.UprKO, false, Upr_User.O_Data.ToString());
                    Zkp = SqlComandDatabase_NewBaza(SqlComand, con);
                    int LastItem = GetDataHistSels(Zkp);
                    LoadingDataUploadBaseAll.ReportProgress(70);
                    strComand = SqlComand;
                }
                catch
                {
                }
            }
            while (false);

            try
            {
                Zkp2 = Wyswietl_PHZestawienieZKP(Upr_User.MinData, Upr_User.MaxData);
                LoadingDataUploadBaseAll.ReportProgress(90);
            }
            catch
            {
                Mw.ErrNamber = Mw.ErrNamber + "- 5";
            }
            BazaZakupyAllKl_Public = SqlComandDatabase_NewBaza(StringComand.ReturnComndBazaZakupy(), con);
            stopwatch.Stop();
            LoadingDataUploadBaseAll.ReportProgress(103);
            Modul_Road.Wczytaj_Ofr_PDF();

            rp = false;
            LoadingDataUploadBaseAll.ReportProgress(104);
            return result;
        }

        public int GetData(DataTable Baza)
        {
            Mw.ListKlient = new List<DaneKlient>();
            int i = 0;
            foreach (DataRow row in Baza.Rows)
            {
                //Console.WriteLine("GetData - NIP {0}", row["NIP"].ToString());
                string KOBranzysta;
                string KOBranzystaEmail;
                string PHOpiekunl;
                string Upr;
                if (row["Opiekun_klienta"].ToString() != "")
                    PHOpiekunl = row["Opiekun_klienta"].ToString();
                else
                    PHOpiekunl = "PH - Nieprzypisany";
                if (row["KO"].ToString() != "")
                    KOBranzysta = row["KO"].ToString();
                else
                    KOBranzysta = "";
                if (row["BrEma"].ToString() != "")
                    KOBranzystaEmail = row["BrEma"].ToString();
                else
                    KOBranzystaEmail = "";
                if (Upr_User.UprKO == false)
                    Upr = row["E_mail"].ToString();
                else
                    Upr = PHOpiekunl + Microsoft.VisualBasic.Constants.vbCrLf + row["E_mail"].ToString();
                Mw.ListKlient.Add(new DaneKlient()
                {
                    Id = int.Parse(row["id"].ToString()),
                    Opiekun_klienta = PHOpiekunl,
                    NIP = row["NIP"].ToString(),
                    Stan = row["Stan"].ToString(),
                    Numer_konta = row["Numer_konta"].ToString(),
                    Nazwa_klienta = row["Nazwa_klienta"].ToString(),
                    Adres = row["Adres"].ToString(),
                    Kod_Poczta = row["Kod_Poczta"].ToString(),
                    Poczta = row["Poczta"].ToString(),
                    Forma_plac = row["Forma_plac"].ToString(),
                    PraceList = row["PraceList"].ToString(),
                    Branza = row["Branza"].ToString(),
                    Tel = row["Tel"].ToString(),
                    E_mail = row["E_mail"].ToString(),
                    Branzysta = KOBranzysta,
                    BranzystaEmail = KOBranzystaEmail,
                    TollTipInfo = Upr,
                    Rabat_Double = Zwroc_RAbat(row["PraceList"].ToString())
                });
                i += 1;
                LoadingDataUploadBaseAll.ReportProgress(IntProgres(i, Baza.Rows.Count) + 200);
            }

            return i;
        }
        public int GetDataCennik(DataTable Baza)
        {
            if (Mw.ListCennik is null)
                Mw.ListCennik = new List<CennikData>();
            else
                Mw.ListCennik.Clear();
            string Nd = "";
            int i = 0;
            // Console.WriteLine(Baza.Rows.Count);
            foreach (DataRow row in Baza.Rows)
            {
                // Console.WriteLine(row["SAP"] + " /// " + row["KArtcOK"]);
                object img = null;
                object tds = null;
                if (row["Img"].ToString().Length > 10)
                {
                    img = row["Img"];
                }
                if (row["TDS"].ToString().Length > 10)
                {
                    tds = row["TDS"];
                }

                if (row["NazwProd"].ToString().Contains("N/D"))
                    Nd = "true";
                else
                    Nd = "false";
                try
                {
                    Mw.ListCennik.Add(new CennikData()
                    {
                        CbSelectRow = false,
                        Id = int.Parse(row["id"].ToString()),
                        Naglowek = row["Naglowek"].ToString(),
                        Lpgrup = row["Lpgrup"].ToString(),
                        SAP = row["SAP"].ToString(),
                        NazwProd = row["NazwProd"].ToString(),
                        Kszt = row["Kszt"].ToString(),
                        Poj = row["Poj"].ToString(),
                        CDM = ReturnToDouble(row["CDM"].ToString()),
                        CK = ReturnToDouble(row["CK"].ToString()),
                        PH = ReturnToDouble(row["PH"].ToString()),
                        ZPR0 = ReturnToDouble(row["ZPR0"].ToString()),
                        GRUPA = row["GRUPA"].ToString(),
                        KATEGORIA = row["KATEGORIA"].ToString(),
                        NAZEWNICTWO = row["NAZEWNICTWO"].ToString(),
                        BrakPrace = row["BrakPrace"].ToString(),
                        Img = row["Img"],
                        Tds = row["TDS"],
                        KartaCHAR = row["KC"],
                        Tds_Ok_True = bool.Parse(row["TdsOk"].ToString()),
                        Kchar_Ok_True = bool.Parse(row["KArtcOK"].ToString()),
                        CenaZPrace = row["ZPR0"].ToString()
                    });
                }
                catch
                {
                    Mw.ListCennik.Add(new CennikData()
                    {
                        CbSelectRow = false,
                        Id = int.Parse(row["id"].ToString()),
                        Naglowek = row["Naglowek"].ToString(),
                        Lpgrup = row["Lpgrup"].ToString(),
                        SAP = row["SAP"].ToString(),
                        NazwProd = row["NazwProd"].ToString(),
                        Kszt = row["Kszt"].ToString(),
                        Poj = row["Poj"].ToString(),
                        CDM = ReturnToDouble(row["CDM"].ToString()),
                        CK = ReturnToDouble(row["CK"].ToString()),
                        PH = ReturnToDouble(row["PH"].ToString()),
                        ZPR0 = ReturnToDouble(row["ZPR0"].ToString()),
                        GRUPA = row["GRUPA"].ToString(),
                        KATEGORIA = row["KATEGORIA"].ToString(),
                        NAZEWNICTWO = row["NAZEWNICTWO"].ToString(),
                        BrakPrace = row["BrakPrace"].ToString(),
                        Img = null,
                        Tds = null,
                        KartaCHAR = null,
                        Tds_Ok_True = false,
                        Kchar_Ok_True = false,
                        CenaZPrace = row["ZPR0"].ToString()
                    });
                }
                i += 1;
                LoadingDataUploadBaseAll.ReportProgress(IntProgres(i, Baza.Rows.Count) + 200);
            }
            //  Console.WriteLine("Cennik wczytano " + i + " z " + Baza.Rows.Count);
            int LastItem = Mw.ListCennik.Count;
            return LastItem;
        }

        public int WczytajBazaklient()
        {
            if (BazaKlient is null)
            {
                BazaKlient = new DataTable();
            }
            BazaKlient = SqlComandDatabase_NewBaza(StringComand.ReturnComandBazaKlient(), con);
            int lastRow = GetData(BazaKlient);
            Mw.startKlient = true;
            return lastRow;
        }
        public int WczytajCennik()
        {
            if (BazaCennik is null)
            {
                BazaCennik = new DataTable();
            }

            BazaCennik = SqlComandDatabase_NewBaza(StringComand.ReturnComndCennik(), con);
            //Console.WriteLine(BazaCennik.Rows.Count);
            int lastRow = GetDataCennik(BazaCennik);
            return lastRow;
        }

        public int GetDataHistSels(DataTable Baza)
        {
            if (Mw.ListHistSels is null)
                Mw.ListHistSels = new List<HistOfSeals>();
            else
                Mw.ListHistSels.Clear();
            string Nd = "";
            int i = 0;
            // Console.WriteLine(Baza.Rows.Count);
            foreach (DataRow row in Baza.Rows)
            {
                    Mw.ListHistSels.Add(new HistOfSeals()
                    {

                        PH = row["PH"].ToString(),
                        Branza = row["Branza"].ToString(),
                        KO = row["KO"].ToString(),
                        Klient = row["Klient"].ToString(),
                        Produkt = row["Produkt"].ToString(),
                        Selsr1 = row[((int)Upr_User.MaxD).ToString()].ToString(),
                        Selsr2 = row[((int)Upr_User.MaxD -1).ToString()].ToString(),
                        Selsr3 = row[((int)Upr_User.MaxD - 2).ToString()].ToString(),
                        Selsr4 = row[((int)Upr_User.MaxD - 3).ToString()].ToString(),
                        SelsSztt1 = row["szt " + ((int)Upr_User.MaxD).ToString()].ToString(),
                        SelsSztt2 = row["szt " + ((int)Upr_User.MaxD - 1).ToString()].ToString(),
                        SelsSztt3 = row["szt " + ((int)Upr_User.MaxD - 2).ToString()].ToString(),
                        SelsSztt4 = row["szt " + ((int)Upr_User.MaxD - 3).ToString()].ToString(),
                       
                    });

                i += 1;
                LoadingDataUploadBaseAll.ReportProgress(IntProgres(i, Baza.Rows.Count) + 200);
            }

            int LastItem = Mw.ListHistSels.Count;
            return LastItem;
        }


        public DataTable Wyswietl_PHZestawienieZKP(string mmDat, string Mxdat)
        {
            try
            {
                string Sqwerty = null;

                DateTime myDate1 = DateTime.ParseExact(mmDat, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime myDate2 = DateTime.ParseExact(Mxdat, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                int NumOfMonths = MonthDifference(myDate1, myDate2);
                var Dt = new string[NumOfMonths + 1];
                for (int i = 0, loopTo = NumOfMonths; i <= loopTo; i++)
                {
                    DateTime time = myDate2;
                    string format = "yyyy-MM";
                    var Tim = time.AddMonths(-i);
                    Dt[i] = Tim.ToString(format);
                    Sqwerty += ",sum(si.Turnover) filter(where  substr(SUBSTR(si.Datebilling,-4,1)|| SUBSTR(si.Datebilling,-3,1)|| SUBSTR(si.Datebilling,-2,1)|| SUBSTR(si.Datebilling,-1,1)||'-'|| SUBSTR(si.Datebilling,-7,1)|| SUBSTR(si.Datebilling,-6,1), 1, 7) = '" + Dt[i] + "') as '" + Dt[i] + "'";
                }

                string sqlqwerty2 = "SELECT md.Branza,substr(si.SoldTocustomer, 1, 7) || ' - ' || md.Nazwa_klienta || ' ' || md.Nazwa_CD as NazwaKL " + Sqwerty + @"
                                        From  BazaZKP si
                                        LEFT Join BazaKL md on
                                        md.Numer_konta = substr(si.SoldTocustomer, 1, 7) 
                                        group by si.SoldTocustomer";
                // Console.WriteLine("ZKp2 = " & sqlqwerty2)
                return SqlComandDatabase_NewBaza(sqlqwerty2, con).Copy();

            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }

    }

    internal static partial class Modul_Road
    {
        public static int Wczytaj_Ofr_PDF()
        {
            BazaOFR = SqlComandDatabase_NewBaza(StringComand.ReturnComandPdtFile(), con);
            int lastRow = GetDataOFR(BazaOFR);
            return lastRow;
        }
        public static int GetDataOFR(DataTable Baza)
        {
            try
            {
                Mw.ListOFR = new List<OFRData>();
                string PH, NazwOFR, Klient, sap;
                foreach (DataRow row in Baza.Rows)
                {
                    PH = row["Opiekun_klienta"].ToString();
                    NazwOFR = row["NrOFR"].ToString();
                    Klient = row["Nazwa_klienta"].ToString();
                    sap = row["SAP"].ToString();
                    Mw.ListOFR.Add(new OFRData()
                    {
                        Id = row["Id"].ToString(),
                        SAPnr = sap,
                        NazwOFR = NazwOFR,
                        OFR = row["PlkPdf"],
                        NazwKlient = Klient,
                        Opiekun = PH
                    });
                }
                return Baza.Rows.Count;
            }
            catch
            {
                return 0;
            }
        }
    }
}
