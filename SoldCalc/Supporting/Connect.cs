using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Supporting
{
    public class Connect
    {


        public static SQLiteConnection PHcon = new SQLiteConnection();
        public static SQLiteCommand PHcmd = new SQLiteCommand();
        public static string connstringPH = ConnectionStringPH;

        public static SQLiteConnection con = new SQLiteConnection();
        public static SQLiteCommand cmd = new SQLiteCommand();
        public static string connstring = ConnectionString;


        public static SQLiteConnection PHDcon = new SQLiteConnection();
        public static SQLiteCommand PHDcmd = new SQLiteCommand();
        public static string ActualconnstringPH = DownloadConnectionStringPH;


        public static SQLiteConnection Acon = new SQLiteConnection();
        public static SQLiteCommand Acmd = new SQLiteCommand();
        public static string Actualconnstring = AktualConnectionString;

        public static SQLiteConnection Dcon = new SQLiteConnection();
        public static SQLiteCommand Dcmd = new SQLiteCommand();
        public static string Downloadconnstring = DownloadConnectionString;

        public static string Downloadconnstringph = DownloadConnectionString;

        public static bool SendBazaAktual = true;
        public static bool URLstatus = FVerificaConnessioneInternet();
        public static string ShowKLMemory;
        public static int wait = 0;
        public static int _StartLogin = 0;
        public static string Scie_Cen;
        public static string Scie_KL;
        public static string Scie_ZK;
        public static string scie_user;


        public static MainWindow Mw;
        public static string Tim;
        public static string RejPh;

        public Connect()
        {
            bool Connect = ConOpen();
            if (Connect)
            {
                CreateDataBase();
                Upr_User = new UPR_Ranga();
                Upr_User.Ide = "Connect";
                Upr_User = ConnectUser.LoadUpr_ranga();
            }
            else
            {
                Console.WriteLine("con err");
            }

        }

        public static bool FVerificaConnessioneInternet()
        {
            System.Net.NetworkInformation.Ping objPing = new System.Net.NetworkInformation.Ping();
            try
            {
                return objPing.Send("www.google.pl").Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        public static bool ConOpen()
        {
            string ErrCon = "";
            try
            {
                if (con.ConnectionString == null)
                    con.ConnectionString = connstring;
                if (Acon.ConnectionString == null)
                    Acon.ConnectionString = Actualconnstring;
                try
                {
                    if (Dcon.ConnectionString == null)
                        Dcon.ConnectionString = Downloadconnstring;
                }
                catch
                {
                    ErrCon += "- 0";
                }

                try
                {
                    if (PHcon.ConnectionString == null)
                        PHcon.ConnectionString = connstringPH;
                }
                catch
                {
                    ErrCon += "- 1";
                }

                try
                {
                    if (PHDcon.ConnectionString == null)
                        PHDcon.ConnectionString = DownloadConnectionStringPH;
                }
                catch
                {
                    ErrCon += "- 3";
                }
                return true;
            }
            catch (Exception ex)
            {
                // TextMessage(ex.StackTrace.ToString());
                return false;
            }
        }
        public static bool ConClose()
        {
            try
            {
                if (Acon != null)
                {
                    if (Acon.State == ConnectionState.Open)
                    {
                        Acon.Close(); Acon.Close(); Acmd.Dispose();
                    }
                }
                if (Dcon != null)
                {
                    if (Dcon.State == ConnectionState.Open)
                    {
                        Dcon.Close(); Dcon.Close(); Dcmd.Dispose();
                    }
                }
                if (PHDcon != null)
                {
                    if (PHDcon.State == ConnectionState.Open)
                    {
                        PHDcon.Close(); PHDcon.Close(); PHDcmd.Dispose();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CreateDataBase()
        {
            Folder_Matka_Programu = System.AppDomain.CurrentDomain.BaseDirectory;
            Folder_Matka_Programu_FilesSC = System.IO.Path.Combine(Folder_Matka_Programu, "FilesSC");
            Folder_Matka_Programu_FilesSC_Update = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, "Update");
            Folder_Matka_Programu_Update = System.IO.Path.Combine(Folder_Matka_Programu, "Update");

            // Console.WriteLine(ConnectionString)
            if (!System.IO.Directory.Exists(Folder_Matka_Programu_FilesSC))
                System.IO.Directory.CreateDirectory(Folder_Matka_Programu_FilesSC);
            if (!System.IO.Directory.Exists(Folder_Matka_Programu_Update))
                System.IO.Directory.CreateDirectory(Folder_Matka_Programu_Update);
            if (!Directory.Exists(Locati))
                Directory.CreateDirectory(Locati);
            if (!Directory.Exists(LocatiAktual))
                Directory.CreateDirectory(LocatiAktual);
            if (!Directory.Exists(Scieżka_Pliku_AppData_FilesSC))
                Directory.CreateDirectory(Scieżka_Pliku_AppData_FilesSC);
            string CreateTable;

            if (NewMethod(FullPH))
            {
                CreateTable = "CREATE TABLE If Not exists TblUser    (Id Integer Not NULL,Ranga TEXT,Imie TEXT,Nazwisko TEXT,Telefon  TEXT,Email  TEXT,KO  TEXT, CenaKO TEXT, WyślijInfoDoKO TEXT , MonitKO TEXT, Upr4  TEXT,NrPh TEXT,OstAkt TEXT, PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, PHcon);
            }

            if (!DuplicateDataBase(Locati + @"\" + FileName))
            {
                CreateTable = "CREATE TABLE If Not exists  BazaKL    (id INTEGER NOT NULL,Opiekun_klienta TEXT,NIP TEXT UNIQUE,Stan	TEXT,Numer_konta TEXT,Nazwa_klienta	TEXT,Nazwa_CD	TEXT,Adres TEXT,Kod_Poczta	TEXT,Poczta	TEXT,Forma_plac	TEXT,PraceList	TEXT,Branza	TEXT,Tel TEXT, E_mail TEXT, OstAkt TEXT, PRIMARY KEY(id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  BazaZKP   (Id INTEGER NOT NULL,Representative TEXT,SoldTocustomer TEXT,Material TEXT,Quantity TEXT,Yearbilling TEXT,SalesP TEXT,Turnover TEXT,Datebilling TEXT, Document_Billing TEXT,Order_Item TEXT, OstAkt TEXT,PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  Cennik    (Id INTEGER NOT NULL,ProdKod TEXT,Naglowek TEXT,Lpgrup TEXT,SAP TEXT UNIQUE ,NazwProd TEXT,Kszt TEXT,Pszt TEXT,Poj TEXT,Miara TEXT,Kolor TEXT,CDM TEXT,CK TEXT,PH TEXT,ZPR0 TEXT,GRUPA TEXT,KATEGORIA TEXT,NAZEWNICTWO TEXT,BrakPrace TEXT,OstAkt TEXT, PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  PraceL    (Id INTEGER NOT NULL,kod_poza_ZRP0 TEXT,OstAkt TEXT,PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  TabZK     (Id INTEGER NOT NULL, NIP TEXT,NrSAP TEXT,ZK1 TEXT,ZK2 TEXT,ZK3 TEXT,ZK1Info TEXT,ZK2Info TEXT,ZK3Info TEXT, Representative TEXT,OstAkt TEXT,PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  TblOferta (Id INTEGER NOT NULL,Representative TEXT,Data TEXT,Numer_konta TEXT,SAP TEXT,NazwProd TEXT,CenaDoOFR TEXT, ZK1 TEXT ,Zk2 TEXT,ZK3 TEXT ,szt1 TEXT, szt2 TEXT, szt3 TEXT,OstAkt TEXT,PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  TblPdf    (Id INTEGER NOT NULL,SAP TEXT,NrOFR TEXT,PlkPdf BLOB,OstAkt TEXT,PRIMARY KEY(Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  DaneKO    (Id INTEGER NOT NULL,KO TEXT,Email TEXT,Branza TEXT, Ranga TEXT, OstAkt TEXT,PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  Baza_PDF  (Id INTEGER NOT NULL, Lpgrup TEXT, SAP TEXT UNIQUE,Img BLOB,Tds BLOB, KC BLOB, PDF2 BLOB,OstAkt TEXT,OstAktTDS TEXT,OstAktKC TEXT,OstAktPDF2 TEXT,PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  Baza_IMG  (Id INTEGER NOT NULL, Lpgrup TEXT, SAP TEXT UNIQUE,Img BLOB,OstAkt TEXT,PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists  BazaErr   (Id Integer Not NULL, PH TEXT, Err TEXT, Data TEXT, PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
                CreateTable = "CREATE TABLE If Not exists BazaOfr_robocze (Id Integer Not NULL,Nip TEXT, Naglowek TEXT, Lpgrup TEXT, SAP TEXT, NazwProd TEXT, Kszt TEXT, Poj TEXT, CDM TEXT, KO TEXT, PH TEXT, ZPR0 TEXT, GRUPA TEXT, KATEGORIA TEXT, NAZEWNICTWO TEXT, BrakPrace TEXT, CenaZPrace TEXT, CenaDoOFR TEXT, CenaDoOFR2 TEXT, CenaDoOFR3 TEXT, Marza TEXT, Marza2 TEXT, Marza3 TEXT, ZK11A1 TEXT, ZK11A2 TEXT, ZK11A3 TEXT, szt1 TEXT, szt2 TEXT, szt3 TEXT, Cena_zapis_do_OFR TEXT, Opis_Cena_zapis_do_OFR TEXT, Opis_Cena_zapis_do_OFR2 TEXT, Opis_Cena_zapis_do_OFR3 TEXT,Img BLOB,TDS BLOB,KCH BLOB,Plik_Tds_True TEXT,Plik_Kch_True TEXT,FileName TEXT,TDS_DO_OFR TEXT,CHAR_DO_OFR TEXT,PDF_DO_OFR TEXT,NazwaPdf TEXT, OstAkt TEXT, PRIMARY KEY (Id));";
                UsingSQLComand(CreateTable, con);
            }

            try
            {
                string stringA = "Select Case(CNT) When 0 Then printf('True') WHEN 1 then printf('False') END as A FROM (SELECT COUNT(*) AS CNT FROM pragma_table_info('Baza_PDF') WHERE name='OstAktTDS');";
                bool A = bool.Parse(SqlRoader_Jedna_wartosc(stringA, con));
                stringA = "ALTER TABLE BAZA_Pdf ADD OstAktTDS TEXT; UPDATE Baza_PDF Set OstAktTDS='220601';";
                if (A == true)
                    UsingSQLComand(stringA, con);

                stringA = "select case(CNT) WHEN 0 then printf('True') WHEN 1 then printf('False') END as A FROM (SELECT COUNT(*) AS CNT FROM pragma_table_info('Baza_PDF') WHERE name='OstAktKC');";
                bool A1 = bool.Parse(SqlRoader_Jedna_wartosc(stringA, con));
                stringA = "ALTER TABLE BAZA_Pdf ADD OstAktKC TEXT; UPDATE Baza_PDF Set OstAktKC='220601';";
                if (A1 == true)
                    UsingSQLComand(stringA, con);

                stringA = "select case(CNT) WHEN 0 then printf('True') WHEN 1 then printf('False') END as A FROM (SELECT COUNT(*) AS CNT FROM pragma_table_info('Baza_PDF') WHERE name='OstAktPDF2');";
                bool A2 = bool.Parse(SqlRoader_Jedna_wartosc(stringA, con));
                stringA = "ALTER TABLE BAZA_Pdf ADD OstAktPDF2 TEXT; UPDATE Baza_PDF Set OstAktPDF2='220601';";
                if (A2 == true)
                    UsingSQLComand(stringA, con);
            }
            catch
            {
            }
        }

        public static int UsingSQLComand(string comandstring, SQLiteConnection conection)
        {
            int err = 0;
            if (comandstring != "")
            {
                if (conection.State == ConnectionState.Closed)
                    conection.Open();
                using (SQLiteCommand sda = conection.CreateCommand())
                {
                    sda.CommandType = CommandType.Text;
                    sda.CommandText = comandstring;
                    try
                    {
                        sda.ExecuteNonQuery();
                    }
                    catch { }

                    err = 0;
                }
                err = 1;

            }
            else
                err = 2;
            return err;
        }

        public static DataTable SqlComandDatabase(string comandstring, SQLiteConnection conection)
        {

            DataTable dt2 = new DataTable();
            try
            {
                if (conection.State == ConnectionState.Closed)
                    conection.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(comandstring))
                {
                    cmd.Connection = conection;
                    cmd.ExecuteNonQuery();
                    using (SQLiteDataAdapter daBaza = new SQLiteDataAdapter(cmd))
                    {
                        daBaza.Fill(dt2);
                    }

                }
            }
            catch (Exception ex)
            {
                string str = "Problem Danych w BazieDanych (sugerowane rozwiązanie) - sprawdz bazę danych i poprawność tych danych!!!! ";
                Console.WriteLine(str);
                Message.TextMessage(str + "     --- >    " + ex.StackTrace.ToString());
            }


            return dt2;
        }

        public static string SqlRoader_Jedna_wartosc(string comandstring, SQLiteConnection conection)
        {
            SQLiteDataAdapter da = new SQLiteDataAdapter(comandstring, conection);
            string s = null;
            if (conection.State == ConnectionState.Closed)
                conection.Open();
            using (SQLiteCommand command = new SQLiteCommand(comandstring, conection))
            {
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        s = (string)reader[0];
                    }
                    catch
                    {
                        s = null;
                    }
                }
            }
            return s;
        }


        public static bool NewMethod(object Sciezka_Plk)
        {
            try
            {
                return !DuplicateDataBase((string)Sciezka_Plk);
            }
            catch (Exception ex)
            {
                // TextMessage(ex.StackTrace.ToString());
                return false;
            }
        }

        public static bool DuplicateDataBase(string FullPath)
        {
            try
            {
                return System.IO.File.Exists(FullPath);
            }
            catch (Exception ex)
            {
                // TextMessage(ex.StackTrace.ToString());
                return false;
            }
        }




        public static string ConectString(string Constraint, SQLiteConnection Sconect)
        {
            if (Sconect != null)
            {
                if (Sconect.State == ConnectionState.Open)
                    Sconect.Close();
            }
            Constraint += ".db";
            string DownloadFullPath = System.IO.Path.Combine(LocatiAktual, Constraint);
            string DownloadConnectionString = string.Format("Data Source =" + DownloadFullPath + "; Version=3;");
            return DownloadConnectionString;
        }

    }



}
