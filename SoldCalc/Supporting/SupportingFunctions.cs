using Microsoft.VisualBasic;
using SoldCalc.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using static SoldCalc.Supporting.Message;

namespace SoldCalc.Supporting
{
    public class SupportingFunctions
    {

        public static string PublicDB = "DBSC2";
        public static string FilePH = "SCPH1.db";
        public static string FileName = "SCBAZA1.db";
        public static object usher = Environment.UserName;
        public static string Locati = @"C:\Users\" + usher + @"\AppData\SC";
        public static string LocatiAktual = @"C:\Users\" + usher + @"\AppData\SC\Update";



        public static string Pas = RoadTxt(1);
        public static string Uide = RoadTxt(2);
        public static string Strim_URL = RoadTxt(3) + PublicDB + "/";
        public static string htmlSpam = RoadTxt(4);
        public static string EmailSerwer = RoadTxt(5);
        public static string HostName = RoadTxt(6);
        public static string EmailAdmin = RoadTxt(7);
        public static string CompanyDomain1 = RoadTxt(8);
        public static string CompanyDomain2 = RoadTxt(9);



        // scieżka pliku Baza klientów folder matka
        public static string PlkAktual = "Skt.txt";


        public static string FullPath = System.IO.Path.Combine(Locati, FileName);
        // Połączenie do bazy Klientów folder matka
        public static string ConnectionString = string.Format("Data Source =" + FullPath + "; Version=3; Persist Security Info=False;");

        // scieżka pliku Baza PH folder matka

        public static string FullPH = System.IO.Path.Combine(Locati, FilePH);
        public static string ConnectionStringPH = string.Format("Data Source =" + FullPH + "; Version=3;"); // Połączenie do bazy PH folder matka
                                                                                                            // scieżka plik text sprawdz co aktualizować
        public static string SprAktualTxt = Locati + @"\" + PlkAktual;
        public static string SprAktualTxtKOAktual = LocatiAktual + @"\" + PlkAktual;
        // scieżka pliku Baza klientów folder Aktualizacja
        public static string AktualFullPath = System.IO.Path.Combine(LocatiAktual, FileName);
        public static string AktualConnectionString = string.Format("Data Source =" + AktualFullPath + "; Version=3; "); // Połączenie do bazy Klientów skopiowanej z folderu do folderu Aktual 
        public static string FullTexSend = System.IO.Path.Combine(LocatiAktual, PlkAktual); // scieżka pliku Baza klientów folder matka
                                                                                            // scieżka pliku PH  folder Aktualizacja
        public static string AktualFullPH = System.IO.Path.Combine(LocatiAktual, FilePH);
        public static string AktualConnectionStringPH = string.Format("Data Source =" + AktualFullPH + "; Version=3;"); // Połączenie do bazy PH skopiowanej z folderu do folderu Aktual 
                                                                                                                        // scieżka pliku baza KLIENCI-URL  folder Aktualizacja
        public static string NEWFileName = "NEWSCBAZA1.db";
        public static string DownloadFullPath = System.IO.Path.Combine(LocatiAktual, NEWFileName);
        public static string DownloadConnectionString = string.Format("Data Source =" + DownloadFullPath + "; Version=3;");// ;Mode=ReadWrite;New=False;Compress=True;Journal Mode=Off;"); // Połączenie do bazy Klientów pobranej z URL
                                                                                                                           // scieżka pliku PH-URL  folder Aktualizacja
        public static string NEWFilePH = "NEWSCBAZAPH.db";
        public static string DownloadPHh = System.IO.Path.Combine(LocatiAktual, NEWFilePH);
        public static string DownloadConnectionStringPH = string.Format("Data Source =" + DownloadPHh + "; Version=3;"); // Połączenie do bazy PH pobranej z URL


        public static string Scieżka_Pliku_AppData_FilesSC = Locati + @"\FilesSC";
        public static string Folder_Matka_Programu;
        public static string Folder_Matka_Programu_FilesSC;
        public static string Folder_Matka_Programu_FilesSC_Update;
        public static string Folder_Matka_Programu_Update;


        public static string AktualCennik;
        public static string AktualZakupy;
        public static List<string[]> ListAktualPH = new List<string[]>();
        public static List<string[]> ListAktualKO = new List<string[]>();
        public static bool sprCennik;
        public static bool sprZakupy;

        public static int LoadUserlog;
        public static DataTable DtUser = new DataTable();

        public static int ukrujKO;

        private static BackgroundWorker _BackgroundALLACTUAL;

        internal static BackgroundWorker BackgroundALLACTUAL
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _BackgroundALLACTUAL;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_BackgroundALLACTUAL != null)
                {
                }

                _BackgroundALLACTUAL = value;
                if (_BackgroundALLACTUAL != null)
                {
                }
            }
        }


        public static int AktualZKPZmianaKO = 0;
        public static bool AktIMGStart = false;
        public static bool AktTDSStart = false;

        public static bool BlokujAktual = false;


        public static DataTable AllBazaUserDownload = new DataTable();
        public static bool BlokClose;
        public static bool WyslijCennik = false;


        public static System.Data.DataTable Zkp = new System.Data.DataTable();
        public static System.Data.DataTable Zkp2 = new System.Data.DataTable();
        public static System.Data.DataTable BazaZakupyAllKl_Public = new System.Data.DataTable();
        public static string strComand;


        public static DataTable BazaKlient = new DataTable(); // podstawowa tabela danych klienta
        public static DataTable BazaCennik = new DataTable(); // podstawowa tabela cennik
        public static DataTable BazaZAkupy = new DataTable(); // podstawowa tabela zakupy
        public static DataTable BazaOFR = new DataTable();  // podstawowa tabela zakupy
        public static DataTable GVDataCennik = new DataTable();

        public static DaneKlient Get_KlientDane = new DaneKlient();
        public static Wind_of_Html Wind_of_Html_Add = null;

        public static LiczOferta Cennik_Add;
        public static DataTable DTwybKlient;
        public static string DtVal;
        public static int _Index = 0;


        public static double[] Val_Meany = new double[5] { 0, 0, 0, 0, 0 };


        public static ZapiszPDFvb ZapiszPDF = null;

        public static string RoadTxt(int i)
        {
            var Wind = new string[21];
            try
            {
                var WczytajCB = new StreamReader(Locati + "/Dane.txt");
                int j = 1;
                while (WczytajCB.EndOfStream != true)
                {
                    Wind[j] = WczytajCB.ReadLine();
                    j += 1;
                }
                WczytajCB.Close();
            }
            // Return Wind
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return Wind[i];
        }



        public static string TimeAktual()
        {
            DateTime time = DateTime.Now;
            string format = "yyMMddHHmm";
            //System.Windows.Forms.MessageBox.Show(time.ToString(format)).ToString();
            return time.ToString(format);
        }

        public static string ConvertToDateTime(string value)
        {
            DateTime convertedDate;
            string zmien = null;
            try
            {
                convertedDate = Convert.ToDateTime(value);
                zmien = convertedDate.ToString();
                string format = "yyMMddHHmm";
                zmien = convertedDate.ToString(format);
            }
            catch (FormatException)
            {
                zmien = value;
            }
            return zmien;
        }

        public static void DeletefilesDownload()
        {
            try
            {
                if (File.Exists(AktualFullPath))
                    File.Delete(AktualFullPath);
                if (File.Exists(DownloadPHh))
                    File.Delete(DownloadPHh);
            }
            catch
            {
            }
        }

        public static string GetUserName()
        {
            try
            {
                string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                return username;
            }
            catch (Exception ex)
            {
                // TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }

        public static void UpdateURL()
        {
            try
            {
                System.IO.File.Copy(FullPath, AktualFullPath, true);
                UpdateRejonPH();
            }
            catch (Exception ex)
            {
                Message.TextMessage(ex.StackTrace.ToString());
            }
        } // aktualizuj bazę ALL z pliku URL
        public static void UpdateRejonPH()
        {
            try
            {
                // BackgroundALLACTUAL.RunWorkerAsync(2000);
                MainWindow.WyslProcent.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception ex)
            {
                Message.TextMessage(ex.StackTrace.ToString());
            }
        }


        public static string Encode(string str)
        {
            try
            {
                System.Text.UTF8Encoding utf8Encoding = new System.Text.UTF8Encoding(true);
                byte[] encodedString;
                encodedString = utf8Encoding.GetBytes(str);
                return utf8Encoding.GetString(encodedString);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }

        public static string Zmien_opisPDF(string NameString)
        {
            if (NameString == "")
            {
                return null;
            }
            NameString = Strings.Replace(NameString, "\"", "").Trim();
            NameString = Strings.Replace(NameString, "..pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ". .pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ".pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, "pdf", "Pdf").Trim();
            NameString = Strings.Replace(NameString, ".Pdf.pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ".pdf.Pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ".pdf.pdf", ".Pdf").Trim();
            // ą, ć, ę, ł, ń, ó, ś, ź, ż.
            // Ą Ć Ę Ł Ń Ó Ś Ż Ź
            return NameString;
        }

        public static void NEW_UpdateURL()
        {
            AktualNewBaza.AktualizujBazaDanych_PH1();

        }
        public static bool TryParseDouble(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            else
            {
                const string Numbers = "0123456789.,";
                var numberBuilder = new StringBuilder();
                foreach (char c in input)
                {
                    if (Numbers.IndexOf(c) > -1)
                        numberBuilder.Append(c);
                }
                double value;
                return double.TryParse(numberBuilder.ToString(), out value);
            }
        }

        public static double Zwroc_RAbat(string Rabatbt)
        {
            double _Rabat = 0;
            string Rbt = Strings.Mid(Rabatbt, 4);
            Rbt = Strings.Replace(Rbt, "ZPR0", "");
            try
            {
                if (TryParseDouble(Rbt))
                {
                    _Rabat = double.Parse(string.Join(null, System.Text.RegularExpressions.Regex.Split(Rbt, "[^0-9,.]+")));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("rabat S bladz Rabat - - {0} ---> " + Rabatbt + " <----- ", ex);
            }
            return _Rabat;
        }

        public static int MonthDifference(DateTime first, DateTime second)
        {
            try
            {
                return Math.Abs(first.Month - second.Month + 12 * (first.Year - second.Year));
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }

        public static bool Usun_po_sprawdzeniu(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (Directory.Exists(filePath))
                Directory.Delete(filePath, true);

            return false;
        }

        public static string EncodeString(string NameString)
        {
            if (NameString == "" || NameString is null)
            {
                return null;
            }
            NameString = Strings.Replace(NameString, "\"", "").Trim();
            NameString = Strings.Replace(NameString, "ą", "a"); NameString = Strings.Replace(NameString, "Ą", "A");
            NameString = Strings.Replace(NameString, "ć", "c"); NameString = Strings.Replace(NameString, "Ć", "C");
            NameString = Strings.Replace(NameString, "ę", "e"); NameString = Strings.Replace(NameString, "Ę", "E");
            NameString = Strings.Replace(NameString, "ł", "l"); NameString = Strings.Replace(NameString, "Ł", "L");
            NameString = Strings.Replace(NameString, "ń", "n"); NameString = Strings.Replace(NameString, "Ń", "N");
            NameString = Strings.Replace(NameString, "ó", "o"); NameString = Strings.Replace(NameString, "Ó", "O");
            NameString = Strings.Replace(NameString, "ś", "s"); NameString = Strings.Replace(NameString, "Ś", "S");
            NameString = Strings.Replace(NameString, "ź", "z"); NameString = Strings.Replace(NameString, "Ź", "Z");
            NameString = Strings.Replace(NameString, "ż", "z"); NameString = Strings.Replace(NameString, "Ż", "Z");
            NameString = Strings.Replace(NameString, "..pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ". .pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ".pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, "pdf", "Pdf").Trim();
            NameString = Strings.Replace(NameString, ".Pdf.pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ".pdf.Pdf", ".Pdf").Trim(); NameString = Strings.Replace(NameString, ".pdf.pdf", ".Pdf").Trim();
            NameString = Strings.Replace(NameString, ". .", ".").Trim();
            NameString = Strings.Replace(NameString, "-", "_").Trim();
            NameString = Strings.Replace(NameString, ".  .", ".").Trim();
            NameString = Strings.Replace(NameString, "..", ".").Trim();
            NameString = Strings.Replace(NameString, "   ", " ").Trim();
            NameString = Strings.Replace(NameString, "  ", " ").Trim();
            NameString = Strings.Replace(NameString, " ", "_").Trim();
            NameString = Strings.Replace(NameString, " .", "_").Trim();
            NameString = Strings.Replace(NameString, ". ", "_").Trim();
            NameString = Strings.Replace(NameString, "__", "_").Trim();
            NameString = Strings.Replace(NameString, "%", "").Trim();
            NameString = Strings.Replace(NameString, "DBSC//", "DBSC/").Trim();


            // ą, ć, ę, ł, ń, ó, ś, ź, ż.
            // Ą Ć Ę Ł Ń Ó Ś Ż Ź
            return NameString;
        }

        public static double CenaZ_praceList(string BrakPrace, double ZPR0, double Rabat)
        {
            double DdO = default;
            if (ZPR0 > 0)
            {
                if (string.IsNullOrEmpty(BrakPrace) | string.IsNullOrEmpty(BrakPrace))
                    DdO = Odwruc_rabaT_Z_setek_uj_na_jednosc((ZPR0 * (1 - Rabat / 100)));
                else
                    DdO = ZPR0;
            }
            else
            {
                DdO = 0.00;
            }
            // Console.WriteLine(" CenaZ_praceList namerow {0} ", DdO)
            return DdO;
        }

        public static double Odwruc_rabaT_Z_setek_uj_na_jednosc(double dec)
        {
            double Procent = 0;
            if (dec > 0)
                Procent = Math.Truncate(dec * 100) / 100;
            // Console.WriteLine("procent =" & Procent)
            return Procent;
        }

        public static double AddDecimalTo_ZK(DataTable Db, string nameRow, string Sap, string name)
        {
            double CdO = 0;
            if (Db != null)
            {
                foreach (DataRow row in Db.Rows)
                {
                    if (row[nameRow].ToString() == Sap & Information.IsNumeric(row[name]))
                        CdO = double.Parse(row[name].ToString());
                }
            }
            //Console.WriteLine(CdO);
            return CdO;
        }

        public static double AddDecimal_o_ofr(DataTable Db, string nameRow, string Sap, string name) // , CenaZPrace As String) As Decimal
        {
            double CdO = 0;
            if (Db != null)
            {
                foreach (DataRow row in Db.Rows)
                {
                    if (row[nameRow].ToString() == Sap & Information.IsNumeric(row[name]))
                        CdO = double.Parse(row[name].ToString());
                }
            }
            return CdO;
        }

        public static string AddString_o_ofr(DataTable Db, string nameRow, string Sap, string name)
        {
            // Console.WriteLine(" AddString_o_ofr namerow {0} Name {1}", nameRow, name)
            string CdO = "0";
            if (Db != null)
            {
                foreach (DataRow row in Db.Rows)
                {
                    if (row[nameRow].ToString() == Sap)
                        CdO = row[name].ToString();
                }
            }
            return CdO;
        }

        public static object Sprawdz_cena_OFR_ZK11(string Br_prace, double Zrp0, double ZK11, int Rabat) // As String
        {
            string cena_Do_TxtBox = null;
            double CenaPoPrace = default;
            string PoPraceZK = "";
            double RabatZRP0 = default;
            string CenDoOFR = "";
            if (string.IsNullOrEmpty(Br_prace))
            {
                if (ZK11 > 0)
                {
                    RabatZRP0 = Odwruc_rabaT_Z_setek_uj_na_jednosc(Zrp0 * (1 - Rabat / 100));
                    double PoZK = Math.Round(RabatZRP0 - RabatZRP0 / (1 / ZK11) / 100, 2);
                    PoPraceZK = PoZK.ToString();
                    CenaPoPrace = RabatZRP0;
                    CenDoOFR = PoZK.ToString();
                }
                else
                {
                    RabatZRP0 = Odwruc_rabaT_Z_setek_uj_na_jednosc(Zrp0 * (1 - Rabat / 100));
                    CenaPoPrace = RabatZRP0;
                    CenDoOFR = Zrp0.ToString();
                }
            }
            else if (ZK11 > 0)
            {
                RabatZRP0 = Zrp0;
                double PoZK = Math.Round(RabatZRP0 - RabatZRP0 / (1 / ZK11) / 100, 2);
                PoPraceZK = PoZK.ToString();
                CenaPoPrace = RabatZRP0;
                CenDoOFR = PoZK.ToString();
            }
            else
            {
                CenaPoPrace = Zrp0;
                CenDoOFR = Zrp0.ToString();
            }
            double PoPraceDoble = CenaPoPrace;
            double PoPrace = (double)Math.Round(PoPraceDoble, 2, MidpointRounding.AwayFromZero);
            if ((double)Zrp0 == PoPrace)
                cena_Do_TxtBox = "0";
            else
                cena_Do_TxtBox = PoPrace.ToString();
            return cena_Do_TxtBox;
        }


        public static double ReturnToDouble(object value)
        {
            return value.ToString() != "" ? double.Parse(value.ToString()) : 0;
        }


        public static byte[] ObjectToByteArray(object obj)
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
               // byte[] fileData = ms.ToArray();// (byte[])BazaCennik.Rows[i][SName];
                return (byte[])ms.ToArray();
            }
        }

        public static int IntProgres(int i, int last)
        {
            int progress = (int)(((double)i / (double)last) * 100);
            return progress;
        }


    }
}
