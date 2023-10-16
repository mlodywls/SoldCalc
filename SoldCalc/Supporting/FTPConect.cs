using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Supporting
{
    public class FTPConect
    {
        public static bool Pobierz_baze_DB_FTP(string NazwaPliku, string sciezkapliku, BackgroundWorker bg)
        {
            string Serchdate = "";
            Connect.ConClose();
            bool Ret = false;
            Connect.SendBazaAktual = true;
            double size = default(double);
            var credentials = new NetworkCredential(SupportingFunctions.Uide, SupportingFunctions.Pas);
            WebRequest DataRequest = WebRequest.Create(SupportingFunctions.Strim_URL + NazwaPliku);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(SupportingFunctions.Strim_URL + NazwaPliku);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = credentials; // New NetworkCredential(Uide, Pas)
            request.UsePassive = true; request.UseBinary = true; request.EnableSsl = false;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            List<string> entries = new List<string>();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                entries = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            response.Close();

            foreach (string entry in entries)
            {
                string[] splits = entry.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                bool isFile = splits[0].Substring(0, 1) != "d";
                bool isDirectory = splits[0].Substring(0, 1) == "d";
                if (isFile)
                {
                    size = double.Parse(splits[4]) / (double)1024;
                    Serchdate = string.Join(" ", splits[5], splits[6], splits[7]);
                }
            } // Console.WriteLine("Data pliku - " & ConvertToDateTime(Serchdate) & " / Waga pliku - " & size)
            WebRequest request1 = WebRequest.Create(SupportingFunctions.Strim_URL + NazwaPliku);
            request1.Credentials = credentials;
            request1.Method = WebRequestMethods.Ftp.DownloadFile;
            using (Stream ftpStream = request1.GetResponse().GetResponseStream())
            {
                // Console.WriteLine(sciezkapliku + @"\" + NazwaPliku);
                using (Stream fileStream = File.Create(sciezkapliku + @"\" + NazwaPliku))
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    do
                    {
                        read = ftpStream.Read(buffer, 0, buffer.Length);
                        if (read > 0)
                        {
                            fileStream.Write(buffer, 0, read);
                            try
                            {
                                if (bg != null)
                                {
                                    var progress = System.Convert.ToInt32(((System.Convert.ToSingle(fileStream.Position) / (double)size) / (double)10)); // Console.WriteLine("Pobierz_baze_DB_FTP - pobrano {0} bytes", progress);// ' .Position)
                                    bg.ReportProgress(progress);
                                }

                            }
                            catch
                            {
                            }
                        }
                    }
                    while (read > 0);
                }
            }
            Ret = true;
            return Ret;
        }
        public static string SendFileUserName = "";
        public static int Wyslij_Pobraną_baze_DB__StartSerwer(string Send_newName, string Send_File, BackgroundWorker bg)
        {
            if (!Connect.URLstatus)
                return 0;
                
            FileInfo fileInfo = new FileInfo(Send_File);
            if (!System.IO.Directory.Exists(Send_File))
            {
                Connect.ConClose();
            }
            else
            {
                return 0;
            }
            SendFileUserName = CreateFilePh_FTP();
            long Fsize = fileInfo.Length;
            string FileTo = SupportingFunctions.Strim_URL + Send_newName;
            //Console.WriteLine("Wyslij_Pobraną_baze_DB__StartSerwer - FileTo = {0} ", FileTo);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FileTo);
            request.Credentials = new NetworkCredential(SupportingFunctions.Uide, SupportingFunctions.Pas);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            using (Stream fileStream = File.OpenRead(Send_File))
            {
                using (Stream ftpStream = request.GetRequestStream())
                {
                    int read;
                    do
                    {
                        byte[] buffer = new byte[10241];
                        read = fileStream.Read(buffer, 0, buffer.Length);
                        if (read > 0)
                        {
                            ftpStream.Write(buffer, 0, read);
                            //Console.WriteLine(System.Convert.ToInt32(((System.Convert.ToSingle(fileStream.Position) / (double)Fsize) * 100)));
                            try
                            {
                                if (bg != null)
                                {

                                    //Console.WriteLine("fileStream.Position {0}  Fsize  {1}", fileStream.Position, Fsize);
                                    var progress = System.Convert.ToInt32(((System.Convert.ToSingle(fileStream.Position) / (double)Fsize) * 100));
                                    bg.ReportProgress(progress);
                                }

                            }
                            catch
                            {
                            }
                        }
                    }

                    while (read > 0);
                    fileStream.Close();
                }
            }
            return 1;

        }

        public static string CreateFilePh_FTP()
        {
            string UserName = EncodeString(Upr_User.User_PH);// + "/";
            bool result = FtpDirectoryExists(UserName, Strim_URL, "1");
            if (result == false)
                MakeDir(UserName, Strim_URL, "1");
            result = FtpDirectoryExists("/BazaKL", Strim_URL + UserName, "2");
            if (result == false)
                MakeDir("/BazaKL", Strim_URL + UserName, "2");
            result = FtpDirectoryExists("/BazaOfr", Strim_URL + UserName, "3");

            if (result == false)
                MakeDir("/BazaOfr", Strim_URL + UserName, "3");

            result = FtpDirectoryExists("/SendBaza", Strim_URL + UserName, "4");

            if (result == false)
                MakeDir("/SendBaza", Strim_URL + UserName, "4");
            return UserName;
        }

        public static void Move_as_FTP(string FileTo, string FileDo)
        {
            Uri serverFile = new Uri(FileTo);
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(serverFile);
            reqFTP.Method = WebRequestMethods.Ftp.Rename;
            reqFTP.Credentials = new NetworkCredential(SupportingFunctions.Uide, SupportingFunctions.Pas);
            reqFTP.RenameTo = FileDo;

            FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
        }




        public static string Sprawdz_Data_BazaDane_FTP(string NazwaPliku, string sciezkapliku)
        {
            ConClose();
            SendBazaAktual = true;
            string Serchdate = null;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(Strim_URL + NazwaPliku);
            req.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            req.Credentials = new NetworkCredential(Uide, Pas);
            using (FtpWebResponse resp = (FtpWebResponse)req.GetResponse())
            {
                string NewData = resp.LastModified.ToString();
                Serchdate = ConvertToDateTime(NewData);
            }
            return Serchdate;
        }

        public static DataTable SqlComandDatabase_NewBaza(string comandstring, SQLiteConnection conection)
        {
            DataTable dt2 = new DataTable();
            using (SQLiteConnection dowcon = new SQLiteConnection(conection))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(comandstring, dowcon))
                {
                    if (conection.State == ConnectionState.Closed)
                    {
                        conection.Open();
                    }
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.FillLoadOption = LoadOption.OverwriteChanges;
                        da.AcceptChangesDuringFill = false;
                        da.MissingSchemaAction = MissingSchemaAction.Add;
                        da.Fill(dt2);
                    }
                }
            }
            // Console.WriteLine("dt2.Rows.Count {0}       Conect {1}", dt2.Rows.Count, comandstring);
            return dt2;
        }  // '  ' '''''''''''''' połączenie databaza SQL

        public static byte[] DownloadFile_URL(string url)
        {
            try
            {
                byte[] result = null;
                using (WebClient webClient = new WebClient())
                {
                    webClient.Credentials = new NetworkCredential(Uide, Pas);
                    result = webClient.DownloadData(url);
                    webClient.Dispose();
                }
                return result;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }


        public static object RenameFileName(string currentFilename, string FileString, string newFilename)
        {
            FtpWebRequest reqFTP = default;
            Stream ftpStream = null;
            if ((currentFilename ?? "") == (newFilename ?? ""))
            {
                return 1;
            }
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(Strim_URL + FileString + currentFilename));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFilename;
                // Console.WriteLine(Strim_URL & FileString & currentFilename & vbCrLf & Strim_URL & FileString & newFilename)
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Uide, Pas);

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
            }
            return 1;
        }

        public static bool FtpDirectoryExists(string dirName, string ServerIP, string nrdz)
        {
            bool IsExists = true;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ServerIP + dirName));
                Console.WriteLine("FtpDirectoryExists - " + ServerIP + dirName);
                request.Credentials = new NetworkCredential(Uide, Pas);
                request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch
            {
                IsExists = false;
            }

            return IsExists;
        }

        public static int MakeDir(string dirName, string ServerIP, string nrdz)
        {
            // Console.WriteLine("MakeDir {0}", lk)
            int Ret = 0;
            Stream ftpStream = null;
            bool net = FVerificaConnessioneInternet();
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ServerIP + dirName));
            Console.WriteLine("MakeDir - " + ServerIP + dirName);
            if (net == false)
            {
                return Ret; //return;
            }
            try
            {
                // Console.WriteLine(" dirName - " & dirName & "  ServerIP + dirName  - " & ServerIP + dirName & " Sam ServerIP - " & ServerIP)
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Uide, Pas);
                using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
                {
                    try
                    {
                        ftpStream = response.GetResponseStream();
                    }
                    catch
                    {
                    }
                    ftpStream.Close(); response.Close(); response.Dispose();
                }
                reqFTP.Abort();
                Ret = 1;
            }
            catch
            {
                if (ftpStream != null)
                {
                    ftpStream.Close(); ftpStream.Dispose();
                }
                reqFTP.Abort();
                return Ret;
            }
            return Ret;
        }


        public static void SendDownload(string Send_newName, string Send_File)
        {
            try
            {
                ConClose();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Strim_URL + Send_newName);
                request.Credentials = new NetworkCredential(Uide, Pas);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                using (Stream fileStream = File.OpenRead(Send_File))
                using (Stream ftpStream = request.GetRequestStream())
                {
                    int read;
                    do
                    {
                        byte[] buffer = new byte[10241];
                        read = fileStream.Read(buffer, 0, buffer.Length);
                        if (read > 0)
                            ftpStream.Write(buffer, 0, read);
                    }
                    while (read > 0);
                    // Console.WriteLine(read)
                    fileStream.Close();
                };
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                //                return null;
            }
        } // wyślij zaktualizowany   plik URL



        public static void Transfer_FTP_file1()
        {
            string Url_string = Strim_URL + "AAAbackup/";
            string DataFile = DateTime.Now.ToString();
            DataFile = Strings.Replace(Strings.Mid(DataFile, 1, 10), ".", "_");
            string UserName = EncodeString(DataFile);
            bool result = FtpDirectoryExists(UserName, Url_string, "1");
            if (result == true)
                MakeDir(UserName, Url_string, "1");

            bool Down;
            int dzialInt = default;
            string Plik_DO_FTP_copy = "AAAbackup/" + DataFile + "/";

            Down = Pobierz_baze_DB_FTP("NEWSCBAZAPH1.db", LocatiAktual, default);
            if (Down == true)
            {
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "NEWSCBAZAPH1.db", LocatiAktual + @"\NEWSCBAZAPH1.db", null);
            }
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "NEWSCBAZAPH1");
            else
                Interaction.MsgBox("błąd wysyłania - " + "NEWSCBAZAPH1");

            Down = Pobierz_baze_DB_FTP("DB_Klient.db", LocatiAktual, default);
            if (Down == true)
            {
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_Klient.db", LocatiAktual + @"\DB_Klient.db", null);
            }
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_Klient");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_Klient");

            Down = Pobierz_baze_DB_FTP("DB_ZAKUPY.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);
            if (Down == true)
            {
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_ZAKUPY.db", LocatiAktual + @"\DB_ZAKUPY.db", null);
            }
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_ZAKUPY");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_ZAKUPY");

            Down = Pobierz_baze_DB_FTP("DB_Cennik.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);
            if (Down == true)
            {
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_Cennik.db", LocatiAktual + @"\DB_Cennik.db", null);
            }
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_Cennik");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_Cennik");

            Down = Pobierz_baze_DB_FTP("DB_ZK.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);
            if (Down == true)
            {
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_ZK.db", LocatiAktual + @"\DB_ZK.db", null);
            }
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_ZK");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_ZK");

            Down = Pobierz_baze_DB_FTP("DB_OFR.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);
            if (Down == true)
            {
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_OFR.db", LocatiAktual + @"\DB_OFR.db", null);
            }
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_OFR");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_OFR");

            Interaction.MsgBox("ZAKOŃCZONO !!!!");
            return;
        }


        public static void Transfer_FTP_file()
        {
            string Url_string = Strim_URL + "AAAbackup/";
            string DataFile = DateTime.Now.ToString(); // = EncodeString(Upr_User.User_PH)
            DataFile = Strings.Replace(Strings.Mid(DataFile, 1, 10), ".", "_"); // & "/"
            string UserName = EncodeString(DataFile);
            bool result = FtpDirectoryExists(UserName, Url_string, "1");
            if (result == true)
                MakeDir(UserName, Url_string, "1");
            bool Down;
            int dzialInt = default;
            string Plik_DO_FTP_copy = "AAAbackup/" + DataFile + "/";
            Down = Pobierz_baze_DB_FTP("NEWSCBAZAPH1.db", LocatiAktual, default);
            if (Down == true)
                //dzialInt = Send_NewBaza(Plik_DO_FTP_copy + "NEWSCBAZAPH1.db", LocatiAktual + @"\NEWSCBAZAPH1.db");
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "NEWSCBAZAPH1.db", LocatiAktual + @"\NEWSCBAZAPH1.db", null);

            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "NEWSCBAZAPH1");
            else
                Interaction.MsgBox("błąd wysyłania - " + "NEWSCBAZAPH1");
            Down = Pobierz_baze_DB_FTP("DB_Klient.db", LocatiAktual, default);
            if (Down == true)
                // dzialInt = Send_NewBaza(Plik_DO_FTP_copy + "DB_Klient.db", LocatiAktual + @"\DB_Klient.db");
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_Klient.db", LocatiAktual + @"\DB_Klient.db", null);
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_Klient");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_Klient");
            Down = Pobierz_baze_DB_FTP("DB_ZAKUPY.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);
            if (Down == true)
                //dzialInt = Send_NewBaza(Plik_DO_FTP_copy + "DB_ZAKUPY.db", LocatiAktual + @"\DB_ZAKUPY.db");
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_ZAKUPY.db", LocatiAktual + @"\DB_ZAKUPY.db", null);
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_ZAKUPY");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_ZAKUPY");
            Down = Pobierz_baze_DB_FTP("DB_Cennik.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);
            if (Down == true)
                //dzialInt = Send_NewBaza(Plik_DO_FTP_copy + "DB_Cennik.db", LocatiAktual + @"\DB_Cennik.db");
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_Cennik.db", LocatiAktual + @"\DB_Cennik.db", null);
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_Cennik");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_Cennik");
            Down = Pobierz_baze_DB_FTP("DB_ZK.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);

            if (Down == true)
                //dzialInt = Send_NewBaza(Plik_DO_FTP_copy + "DB_ZK.db", LocatiAktual + @"\DB_ZK.db");
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_ZK.db", LocatiAktual + @"\DB_ZK.db", null);
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_ZK");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_ZK");
            Down = Pobierz_baze_DB_FTP("DB_OFR.db", LocatiAktual, AktualNewBaza.BC_Aktual_baza);
            if (Down == true)
                //dzialInt = Send_NewBaza(Plik_DO_FTP_copy + "DB_OFR.db", LocatiAktual + @"\DB_OFR.db");
                dzialInt = Wyslij_Pobraną_baze_DB__StartSerwer(Plik_DO_FTP_copy + "DB_OFR.db", LocatiAktual + @"\DB_OFR.db", null);
            if (dzialInt == 1)
                Interaction.MsgBox("wysłano - " + "DB_OFR");
            else
                Interaction.MsgBox("błąd wysyłania - " + "DB_OFR");
            Interaction.MsgBox("ZAKOŃCZONO !!!!");
            return;
        }



        public static int DownloadfileAsUpload(string sciezkapliku, string StringFolderFtp, string NazwaPliku)
        {
            int i = 0;
            string FileFtp = Strim_URL + StringFolderFtp + NazwaPliku;
            string FileDicdMeData = sciezkapliku + @"\" + NazwaPliku;
            // Console.WriteLine("scie" & FileDicdMeData)
            ConClose();
            SendBazaAktual = true;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FileFtp);
            request.Credentials = new NetworkCredential(Uide, Pas);
            var size = GetFtpFileSize(new Uri(FileFtp));
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            try
            {
                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(FileDicdMeData))
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    do
                    {
                        read = ftpStream.Read(buffer, 0, buffer.Length);
                        if (read > 0)
                            fileStream.Write(buffer, 0, read);
                    }
                    while (read > 0);
                }
                i = 1;
            }
            catch
            {
            }
            return i;
        } // Pobierz plik URL - ogólny
        public static int GetFtpFileSize(Uri requestUri)
        {
            var ftpWebRequest = GetFtpWebRequest(requestUri, WebRequestMethods.Ftp.GetFileSize);
            try
            {
                return int.Parse(((FtpWebResponse)ftpWebRequest.GetResponse()).ContentLength.ToString());
            }
            catch (Exception)
            {
                return default(int);
            }
        }

        public static FtpWebRequest GetFtpWebRequest(Uri requestUri, string method = null)
        {
            try
            {
                var ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUri);
                ftpWebRequest.Credentials = new NetworkCredential(Uide, Pas);
                if (!string.IsNullOrEmpty(method))
                    ftpWebRequest.Method = method;
                return ftpWebRequest;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }






    }
}
