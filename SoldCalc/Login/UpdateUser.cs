using Microsoft.VisualBasic;
using SoldCalc.Supporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;



namespace SoldCalc.Login
{
    internal static partial class AktualNwwBaza_PH
    {
        internal static BackgroundWorker UpdatePH;

        static AktualNwwBaza_PH()
        {
            if (UpdatePH == null)
            {
                UpdatePH = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
                UpdatePH.DoWork += UpdatePH_DoWork;
                UpdatePH.ProgressChanged += UpdatePH_ProgressChanged;
                UpdatePH.RunWorkerCompleted += UpdatePH_RunWorkerCompleted;
            }

        }
        public static void StartUpdatePH()
        {
            //Console.WriteLine("AktualNwwBaza_PH " + string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
            try
            {
                URLstatus = FVerificaConnessioneInternet();
                if (URLstatus == true)
                {
                    Console.WriteLine("PobierzBazaFTP - " + Pobierz_baze_DB_FTP(NEWFilePH, LocatiAktual, UpdatePH));
                }
                else
                {
                    //Console.WriteLine(Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf + "Aktual Baza wrs 307 brak połaczenia z internetem" + Microsoft.VisualBasic.Constants.vbCrLf + " Sprawdz połączenie!" + Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf);
                    return;
                }
                ConClose();
                UpdatePH.RunWorkerAsync(2000);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private static void UpdatePH_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                UpdatePH.WorkerReportsProgress = true;
                BackgroundWorker bw = sender as BackgroundWorker;
                int arg = (int)e.Argument;
                e.Result = TimeUpdatePH(bw);
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
        private static void UpdatePH_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int _Stan = e.ProgressPercentage;
        }
        private static void UpdatePH_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Console.WriteLine("UpdatePH_RunWorkerCompleted - " + e.Result.ToString());
            try
            {
                if (e.Cancelled)
                {
                    // MessageBox.Show("Operacja została anulowana");
                }
                else if (e.Error != null)
                {
                    // MessageBox.Show("coś nie tak");
                }

                else if (e.Result.ToString() == "1")
                {
                    if (AktualNewBaza.SendBazaAktual == true)
                    {
                        Wyslij_Pobraną_baze_DB__StartSerwer(NEWFilePH, DownloadPHh, (BackgroundWorker)sender);
                        Upr_User = ConnectUser.LoadUpr_ranga();
                    }
                }

                AktualNewBaza.ActivFunction = 0;
                Connect.Mw.StPH.DataContext = Upr_User;
            }
            catch (Exception ex)
            {
                DeletefilesDownload();
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private static int TimeUpdatePH(BackgroundWorker bw)
        {
            try
            {
                int result = 0;
                var BazaKLUpdate = new System.Data.DataTable();
                Tim = TimeAktual();
                while (!bw.CancellationPending)
                {
                    int _licz = 0;
                    AllBazaUserDownload = SqlComandDatabase(StringComand.ReturnComndBazaPH(), PHDcon);
                    if (AllBazaUserDownload != null)
                    {
                        int i = 0;
                        if (_licz == 0)
                        {
                            i = Road_Upr_User(AllBazaUserDownload);
                            UsingSQLComand(StringComand.ReturnComndBazaUser_PH(), PHcon);
                            UsingSQLComand(StringComand.ReturnComndBazaUser_PH(), PHDcon);
                        }
                    }
                    result = 1;
                    return result;
                }
                return result;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }

        internal static int Road_Upr_User(System.Data.DataTable AktBaza)
        {
            int i = 0;
            var loopTo = AllBazaUserDownload.Rows.Count - 1;
            for (i = 0; i <= loopTo; i++)
            {
                if (AllBazaUserDownload.Rows[i]["Email"].ToString() == Upr_User.User_Email)
                {
                    Upr_User.Ranga = AllBazaUserDownload.Rows[i]["Ranga"].ToString();
                    Upr_User.Imie = AllBazaUserDownload.Rows[i]["Imie"].ToString();
                    Upr_User.Nazwisko = AllBazaUserDownload.Rows[i]["Nazwisko"].ToString();

                    Upr_User.Telefon = AllBazaUserDownload.Rows[i]["Telefon"].ToString();
                    Upr_User.User_Email = AllBazaUserDownload.Rows[i]["Email"].ToString();
                    Upr_User.KO_email = AllBazaUserDownload.Rows[i]["KO"].ToString();
                    Upr_User.CenaKO = bool.Parse(AllBazaUserDownload.Rows[i]["CenaKO"].ToString());
                    Upr_User.WyslijInfoDoKO = bool.Parse(AllBazaUserDownload.Rows[i]["WyślijInfoDoKO"].ToString());
                    Upr_User.MonitKO = bool.Parse(AllBazaUserDownload.Rows[i]["MonitKO"].ToString());
                    Upr_User.Upr4 = bool.Parse(AllBazaUserDownload.Rows[i]["Upr4"].ToString());
                    Upr_User.NrPh = AllBazaUserDownload.Rows[i]["NrPh"].ToString();
                    RejPh = AllBazaUserDownload.Rows[i]["Rejon"].ToString();
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
                        Upr_User.NrPh = AllBazaUserDownload.Rows[i]["NrPh"].ToString();
                        if (Upr_User.NrPh.Length < 15)
                            Upr_User.NrPh = Tim + Strings.Mid(Upr_User.Imie, 1, 1) + Strings.Mid(Upr_User.Nazwisko, 1, 1) + "/" + GetUserName().ToString();
                    }
                    catch
                    {
                    }
                    if (AllBazaUserDownload.Rows[i]["Ranga"].ToString() == "KO")
                        Upr_User.UprKO = true;
                    else
                        Upr_User.UprKO = false;
                    if (Upr_User.Admin == true)
                        Upr_User.UprKO = true;
                    Upr_User.User_PH = Upr_User.Nazwisko + " " + Upr_User.Imie;
                    break;
                }
            }
            return i;
        }


        public static void DownloadFileserchupdate()
        {
            try
            {
                URLstatus = FVerificaConnessioneInternet();
                if (URLstatus == true)
                    Pobierz_baze_DB_FTP(PlkAktual, LocatiAktual, UpdatePH);
                else
                {
                    Interaction.MsgBox("brak połaczenia z internetem" + Microsoft.VisualBasic.Constants.vbCrLf + " Sprawdz połączenie!");
                    return;
                }
                if (System.IO.Directory.Exists(SprAktualTxt) == false)
                    WczytajTxtAktual();
                var objReaderPH = new StreamReader(SprAktualTxt, Encoding.UTF8);
                while (objReaderPH.Peek() != -1)
                    ListAktualPH.Add(objReaderPH.ReadLine().Split(' '));
                objReaderPH.Close();
                var objReaderKO = new StreamReader(SprAktualTxtKOAktual, Encoding.UTF8);
                while (objReaderKO.Peek() != -1)
                    ListAktualKO.Add(objReaderKO.ReadLine().Split(' '));
                objReaderKO.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public static void WczytajTxtAktual()
        {
            try
            {
                AktualZakupy = "Zkp-" + DateTime.Now.ToString();
                AktualCennik = "Cen-" + DateTime.Now.ToString();
                var afile = new StreamWriter(SprAktualTxt, true);
                afile.WriteLine(AktualZakupy);
                afile.WriteLine(AktualCennik);
                afile.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }


    }
}
