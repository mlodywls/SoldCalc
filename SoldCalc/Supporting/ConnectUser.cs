using Microsoft.VisualBasic;
using SoldCalc.Login;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.SupportingFunctions;


namespace SoldCalc.Supporting
{
    public static class ConnectUser
    {

        internal static BackgroundWorker ConnectUpdatePH;
        private static string Tim;

        public static bool SendBazaAktual = true;
        public static int ActivFunction;

        static ConnectUser()
        {
            if (Upr_User is null)
            {
                Upr_User = new UPR_Ranga();
            }
            if (ConnectUpdatePH is null)
            {
                ConnectUpdatePH = new BackgroundWorker();
                ConnectUpdatePH.DoWork += ConnectUpdatePH_DoWork;
                ConnectUpdatePH.ProgressChanged += ConnectUpdatePH_ProgressChanged;
                ConnectUpdatePH.RunWorkerCompleted += ConnectUpdatePH_RunWorkerCompleted;
            }
        }

        public static UPR_Ranga LoadUpr_ranga()
        {
            Mw.licz += 1;
            //Console.WriteLine("UPR_Ranga LoadUpr_ranga " + Mw.licz);
            URLstatus = Connect.FVerificaConnessioneInternet();
            string sqlstring = "DELETE FROM TblUser WHERE Imie LIKE '' AND Nazwisko  like '';";
            Connect.SqlComandDatabase(sqlstring, Connect.PHcon);

            DataTable dtUs = Connect.SqlComandDatabase(StringComand.ReturnComndBazaPH(), Connect.PHcon);
            UPR_Ranga Upr_User = LoadUser(dtUs);
            if (Upr_User == null)
            {
                //Console.WriteLine("ConnectUser / Upr_User == null");
                Mw.Panel_Pierwsze_Logowanie.Margin = new Thickness(0, 0, 0, 0);
                Mw.Panel_Pierwsze_Logowanie.Children.Add(new UserLog());
                Mw.Panel_Pierwsze_Logowanie.Visibility = Visibility.Visible;
            }
            else
            {
                // Console.WriteLine("ConnectUser / Upr_User == Upr_User zawiera dane");
                if (Mw.licz == 1)
                    StartUpdatePH();
            }

            return Upr_User;
        }



        public static UPR_Ranga LoadUser(DataTable dtUs)
        {

            if (dtUs != null && dtUs.Rows.Count > 0)
            {
                Upr_User.Ide = "public static UPR_Ranga LoadUser(DataTable dtUs)";
                Upr_User.Imie = dtUs.Rows[0]["Imie"].ToString();
                Upr_User.Nazwisko = dtUs.Rows[0]["Nazwisko"].ToString();
                Upr_User.Telefon = dtUs.Rows[0]["Telefon"].ToString();
                Upr_User.User_Email = dtUs.Rows[0]["Email"].ToString();
                Upr_User.KO_email = dtUs.Rows[0]["KO"].ToString();
                Upr_User.Ranga = dtUs.Rows[0]["Ranga"].ToString();
                Upr_User.CenaKO = bool.Parse(dtUs.Rows[0]["CenaKO"].ToString());
                if (Upr_User.Ranga == "KO")
                    Upr_User.UprKO = true;
                else
                    Upr_User.UprKO = false;
                if (Upr_User.Admin == true)
                    Upr_User.UprKO = true;
                Upr_User.WyslijInfoDoKO = bool.Parse(dtUs.Rows[0]["WyślijInfoDoKO"].ToString());
                Upr_User.MonitKO = bool.Parse(dtUs.Rows[0]["MonitKO"].ToString());
                Upr_User.Upr4 = bool.Parse(dtUs.Rows[0]["CenaKO"].ToString());
                Upr_User.NrPh = dtUs.Rows[0]["NrPh"].ToString();
                Upr_User.User_PH = Upr_User.Nazwisko + " " + Upr_User.Imie;

                Upr_User.User_PH = Upr_User.Nazwisko + " " + Upr_User.Imie;

            }
            return Upr_User;
        }

        public static void StartUpdatePH()
        {

            if (Connect.URLstatus)
            {
                bool b = FTPConect.Pobierz_baze_DB_FTP(SupportingFunctions.NEWFilePH, SupportingFunctions.LocatiAktual, ConnectUpdatePH);
                // Console.WriteLine("ConnectUser / StartUpdatePH - Baza Pobrana " + b);
            }
            //else
            //{
            //    //Console.WriteLine("\n Aktual Baza wrs 111 brak połaczenia z internetem \n Sprawdz połączenie! \n");
            //    return;
            //}
            ConnectUpdatePH.RunWorkerAsync(2000);
        }


        private static void ConnectUpdatePH_DoWork(object sender, DoWorkEventArgs e)
        {
            ConnectUpdatePH.WorkerReportsProgress = true;
            BackgroundWorker bw = sender as BackgroundWorker;
            int arg = (int)e.Argument;
            e.Result = TimeUpdatePH(bw); // , arg)
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }
        private static void ConnectUpdatePH_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int _Stan = e.ProgressPercentage;

            Console.WriteLine("Wyslij " + _Stan + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
        }
        private static void ConnectUpdatePH_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Operacja została anulowana");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("coś nie tak - Error !=");
            }
            else if (e.Result.ToString() == "1")
            {
                if (SendBazaAktual == true)
                {
                    // Console.WriteLine("ConnectUpdatePH_RunWorkerCompleted i-----> Wyslij_Pobraną_baze_DB__StartSerwer ");
                    FTPConect.Wyslij_Pobraną_baze_DB__StartSerwer(SupportingFunctions.NEWFilePH, SupportingFunctions.DownloadPHh, (BackgroundWorker)sender);

                }
            }
            ActivFunction = 0;
            DataTable dtUs = Connect.SqlComandDatabase(StringComand.ReturnComndBazaPH(), Connect.PHcon);
            Upr_User = LoadUser(dtUs);
            SupportingFunctions.DeletefilesDownload();
            Mw.StartApp();
        }

        private static int TimeUpdatePH(BackgroundWorker bw)
        {
            int result = 0;
            Tim = SupportingFunctions.TimeAktual();

            DataTable AllBazaUserDownload = SqlComandDatabase(StringComand.ReturnComndBazaPH(), PHDcon);
            while (!bw.CancellationPending)
            {
                if (AllBazaUserDownload != null)
                {
                    int i = 0;
                    i = Road_Upr_User(AllBazaUserDownload);
                    int e = UsingSQLComand(ReturnComndBazaUser_PH(), PHcon);
                    string f = "";
                    if (e == 1)
                    {
                        f = "OK!";
                        UsingSQLComand(ReturnComndBazaUser_PH(), PHDcon);
                    }
                }
                else
                {
                    Console.WriteLine("AllBazaUserDownload = null");
                }
                result = 1;
                return result;
            }
            return result;
        }

        internal static int Road_Upr_User(System.Data.DataTable AktBaza)
        {
            int i = 0;
            var loopTo = AktBaza.Rows.Count - 1;
            for (i = 0; i <= loopTo; i++)
            {
                // Console.WriteLine(AktBaza.Rows[i]["Email"].ToString());
                if (AktBaza.Rows[i]["Email"].ToString() == Upr_User.User_Email)
                {

                    {
                        Upr_User.Ide = default;
                        Upr_User.User_PH = default;
                        Upr_User.Id = default;
                        Upr_User.Ranga = default;
                        Upr_User.Imie = default;
                        Upr_User.Nazwisko = default;
                        Upr_User.Admin = default;
                        Upr_User.Telefon = default;
                        Upr_User.UprKO = default;
                        Upr_User.User_Email = default;
                        Upr_User.KO_email = default;
                        Upr_User.CenaKO = default;
                        Upr_User.WyslijInfoDoKO = default;
                        Upr_User.MonitKO = default;
                        Upr_User.Upr4 = default;
                        Upr_User.NrPh = default;
                        Upr_User.OstAkt = default;
                        Upr_User.Scale = default;
                        Upr_User.dpiTransform = default;
                        Upr_User.MaxData = default;
                        Upr_User.MaxD = default;
                        Upr_User.MaxDataRok = default;
                        Upr_User.MinData = default;
                        Upr_User.MinD = default;
                        Upr_User.O_Data = default;
                        Upr_User.WyswCennikAdmin = default;
                        Upr_User.SelectedDateMin = default;
                        Upr_User.SelectedDateMax = default;
                    }




                    Upr_User.Ide = "internal static int Road_Upr_User(System.Data.DataTable AktBaza)";
                    Upr_User.Ranga = AktBaza.Rows[i]["Ranga"].ToString();
                    Upr_User.Imie = AktBaza.Rows[i]["Imie"].ToString();
                    Upr_User.Nazwisko = AktBaza.Rows[i]["Nazwisko"].ToString();

                    Upr_User.Telefon = AktBaza.Rows[i]["Telefon"].ToString();
                    Upr_User.User_Email = AktBaza.Rows[i]["Email"].ToString();
                    Upr_User.KO_email = AktBaza.Rows[i]["KO"].ToString();
                    Upr_User.CenaKO = bool.Parse(AktBaza.Rows[i]["CenaKO"].ToString());
                    Upr_User.WyslijInfoDoKO = bool.Parse(AktBaza.Rows[i]["WyślijInfoDoKO"].ToString());
                    Upr_User.MonitKO = bool.Parse(AktBaza.Rows[i]["MonitKO"].ToString());
                    Upr_User.Upr4 = bool.Parse(AktBaza.Rows[i]["Upr4"].ToString());
                    Upr_User.NrPh = AktBaza.Rows[i]["NrPh"].ToString();
                    RejPh = AktBaza.Rows[i]["Rejon"].ToString();
                    if (RejPh != "")
                    {
                        Upr_User.Rejon = RejPh;
                    }
                    if (RejPh == "Admin")
                        Upr_User.Admin = true;
                    else
                        Upr_User.Admin = false;



                    try
                    {
                        Upr_User.NrPh = AktBaza.Rows[i]["NrPh"].ToString();
                        if (Upr_User.NrPh.Length < 15)
                            Upr_User.NrPh = Tim + Strings.Mid(Upr_User.Imie, 1, 1) + Strings.Mid(Upr_User.Nazwisko, 1, 1) + "/" + GetUserName().ToString();
                    }
                    catch
                    {
                    }
                    if (AktBaza.Rows[i]["Ranga"].ToString() == "KO")
                        Upr_User.UprKO = true;
                    else
                        Upr_User.UprKO = false;
                    if (Upr_User.Admin == true)
                        Upr_User.UprKO = true;

                    Upr_User.User_PH = Upr_User.Nazwisko + " " + Upr_User.Imie;
                    //     Console.WriteLine(Upr_User.User_PH   + " " + Upr_User.Telefon);























                    break;

                }
            }
            return i;
        }

        public static string ReturnComndBazaUser_PH()
        {
            Tim = SupportingFunctions.TimeAktual();
            if (Upr_User.NrPh == null)
            {
                //Console.WriteLine("Upr_User.NrPh = Null!!!"); 
                return "";
            }
            string[] Str_NrPh = Upr_User.NrPh.ToString().Split('/');
            try
            {
                Str_NrPh = Upr_User.NrPh.Split('/');
                if (Str_NrPh[1] == null)
                    Str_NrPh[1] = GetUserName().ToString();
            }
            catch
            {
            }
            // Console.WriteLine(Str_NrPh(0) & vbCrLf & Str_NrPh(1))
            string SqlString1 = @" -- Try to update any existing row
                                                        UPDATE TblUser
                                                        SET Ranga = '" + Upr_User.Ranga + "',Imie='" + Upr_User.Imie + "',Nazwisko='" + Upr_User.Nazwisko + "',Telefon='" + Upr_User.Telefon + "',Email='" + Upr_User.User_Email + "',KO='" + Upr_User.KO_email + "',CenaKO='" + Upr_User.CenaKO + "',WyślijInfoDoKO='" + Upr_User.WyslijInfoDoKO + "',MonitKO='" + Upr_User.MonitKO + "',Upr4='" + Upr_User.Upr4 + "',NrPh ='" + Upr_User.NrPh + "',  OstAkt='" + Tim + @"' 
                                                        WHERE Email like '%" + Upr_User.User_Email + "%' and NrPh like '%" + Str_NrPh[1] + @"%';                                                                                       
                                                        -- If no update happened (i.e. the row didn't exist) then insert one
                                                        INSERT INTO TblUser   (Ranga ,Imie ,Nazwisko ,Telefon  ,Email  ,KO  , CenaKO , WyślijInfoDoKO  ,MonitKO , Upr4, NrPh ,OstAkt)   SELECT '" + Upr_User.Ranga + "','" + Upr_User.Imie + "','" + Upr_User.Nazwisko + "','" + Upr_User.Telefon + "','" + Upr_User.User_Email + "','" + Upr_User.KO_email + "','" + Upr_User.CenaKO + "','" + Upr_User.WyslijInfoDoKO + "','" + Upr_User.MonitKO + "','" + Upr_User.Upr4 + "','" + Upr_User.NrPh + "','" + Tim + @"'
                                                        WHERE (Select Changes() = 0);";
            //Console.WriteLine(SqlString1);
            return SqlString1;
        }


    }
}
