using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SoldCalc.Controls;
using SoldCalc.Supporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using CheckBox = System.Windows.Controls.CheckBox;
using DataTable = System.Data.DataTable;
using Label = System.Windows.Controls.Label;
using Point = System.Windows.Point;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace SoldCalc
{
    public partial class LiczOferta //: Page
    {
        private BackgroundWorker BG_WgrajDane;
        public static StackPanel BG_StInfo;
        public static ListView BG_ZapisaneOferty;
        public DataTable TblSort { get; set; }
        public SolidColorBrush LabRgbActiv = new SolidColorBrush(Color.FromRgb(217, 217, 217));
        public SolidColorBrush LabRgbNotActiv = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private System.Windows.Controls.Button btn_to_drag;
        private System.Data.DataTable TblSortViev = new DataTable();
        private string _MyZatwierdz;
        public string MyZatwierdz
        {
            get
            {
                return _MyZatwierdz;
            }
            set
            {
                _MyZatwierdz = value;
            }
        }
        private string _ItmDelete;
        public string ItmDelete
        {
            get
            {
                return _ItmDelete;
            }
            set
            {
                _ItmDelete = value;
            }
        }
        private DaneKlient __Data;
        private DaneKlient _Data
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return __Data;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                __Data = value;
            }
        }

        private SeitingList PrdColWidth = new SeitingList();
        private string liczPage;
        public System.Data.DataTable TblZalup_private_of_LiczOfr;
        private System.Data.DataTable TblZapisaneOferta_of_liczOfr;
        private System.Data.DataTable TblAnalizaBranza;
        private List<CennikData> ListHistoriaZKP;
        private List<CennikData> ListZapisOferty;
        private List<ZK11Data> ListZapisZK;
        private List<AnalizaBranzaData> ListAnalizaBranza;
        private System.Data.DataTable TabelaOdczytZK;
        private int Grupnr = default;
        private List<CennikData> Tbl_selectedIndex = new List<CennikData>();
        public LiczOferta(DaneKlient data) : this()
        {
            _Data = new DaneKlient();
            _Data = data;
            CopydaneKL();
            OpisKl.DataContext = Get_KlientDane;
            StackPanelInfo.DataContext = Get_KlientDane;
            Spr_Wstawine_Admin.DataContext = Upr_User;

            ListTblOfr.Tbl_Add_prodList = new List<TblOfr>();
        }
        public LiczOferta()
        {
            BG_WgrajDane = new BackgroundWorker();
            TblZalup_private_of_LiczOfr = new System.Data.DataTable(); // Dim
            TblZapisaneOferta_of_liczOfr = new DataTable();
            TblAnalizaBranza = new DataTable();
            TabelaOdczytZK = new System.Data.DataTable();

            InitializeComponent();

            LiczOfr = this;
            this.DataContext = this;
            ControlToBG();

            Mw.AddKlient.Visibility = Visibility.Collapsed;
            Cennik.ItemsSource = BazaCennik.DefaultView;
            string Sqwery2 = "SELECT distinct rtrim(GRUPA, ' ') as GRUPA FROM Cennik WHERE GRUPA <>''";
            CombGr1.ItemsSource = SqlComandDatabase(Sqwery2, con).DefaultView;
            ZwinLab("rozwiń");
            WyswietlHistZakupow();

            WczytajZapisProd();

            CreateDynamicGridView();
            WczytajOfrDoCombo();
            WyswietlAllZK();
            SercgData("", true);
            StickpanGrand();
            if (Upr_User.CenaKO == false)
                Lloc.Text = "0";
            else
                Lloc.Text = "80";
            // serchListVisible();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //CopydaneKL();
            // Console.WriteLine("Page_Loaded Upr_User.MaxDataRok = " + Upr_User.Ide + "  " + Upr_User.MaxDataRok + "  " + Upr_User.User_PH + "  " + Upr_User.MaxData + "  " + Upr_User.MaxD);

            ControlToBG();
            TblZapisRobocze = SqlComandDatabase_NewBaza(WczytajRobocza(), con);
            // Console.WriteLine(WczytajRobocza() & " - " & TblZapisRobocze.Rows.Count)
            RoboczeVisible();
            Oferta.IsChecked = true;
            //Console.WriteLine("Włącz   // WczytajAnalizaBR();");
            WczytajAnalizaBR();

            serchListVisible();

        }

        public void RoboczeVisible()
        {
            if (TblZapisRobocze.Rows.Count == 0)
                Mw.RoboczeSql.Visibility = Visibility.Collapsed;
            else
                Mw.RoboczeSql.Visibility = Visibility.Visible;
        }

        public void CopydaneKL()
        {
            // Console.WriteLine(Get_KlientDane.NIP & "    " & _Data(2))
            if (Get_KlientDane.NIP != this._Data.NIP)
            {
                Get_KlientDane = _Data;
                //Get_KlientDane.Id = int.Parse(this._Data.Id.ToString());
                //Get_KlientDane.Opiekun_klienta = this._Data[1].ToString();
                //Get_KlientDane.NIP = this._Data[2].ToString();
                //Get_KlientDane.Stan = this._Data[3].ToString();
                //Get_KlientDane.Numer_konta = this._Data[4].ToString();
                //Get_KlientDane.Nazwa_klienta = this._Data[5].ToString();
                //Get_KlientDane.Adres = this._Data[6].ToString();
                //Get_KlientDane.Kod_Poczta = this._Data[7].ToString();
                //Get_KlientDane.Poczta = this._Data[8].ToString();
                //Get_KlientDane.Forma_plac = this._Data[9].ToString();
                //Get_KlientDane.PraceList = this._Data[10].ToString();
                //Get_KlientDane.Branza = this._Data[11].ToString();
                //Get_KlientDane.Tel = this._Data[12].ToString();
                //Get_KlientDane.E_mail = this._Data[13].ToString();
                //Get_KlientDane.Branzysta = this._Data[14].ToString();
                //Get_KlientDane.BranzystaEmail = this._Data[15].ToString();
                //Get_KlientDane.Rabat_Double = Zwroc_RAbat(this._Data[16].ToString());
            }
            Mw.EditKlientDane = Get_KlientDane;
            LiczOfr = this;
        }


        private System.Data.DataTable TblZapisRobocze = new System.Data.DataTable();

        private string WczytajRobocza()
        {
            string sqlqwert = @"DELETE FROM BazaOfr_robocze WHERE SAP Like '' or SAP is NULL;
                                    SELECT Id , Nip , Naglowek , Lpgrup , SAP , NazwProd , Kszt , Poj , CDM , KO , PH , ZPR0 , GRUPA , KATEGORIA , NAZEWNICTWO , IFNULL(BrakPrace, '') as BrakPrace , CenaZPrace , CenaDoOFR ,IFNULL( CenaDoOFR2, '0') as CenaDoOFR2 ,
		                            IFNULL(CenaDoOFR3,'0')as CenaDoOFR3 , IFNULL(Marza,'')as Marza , IFNULL(Marza2,'') as Marza2 , IFNULL(Marza3,'') as Marza3 , IFNULL(ZK11A1,'0') as ZK11A1 , IFNULL(ZK11A2,'0') as ZK11A2 , IFNULL(ZK11A3,'0') as ZK11A3 ,IFNULL( szt1,'') as szt1 , IFNULL(szt2 ,'') as szt2, IFNULL(szt3,'') as szt3 ,
                                    IFNULL( Cena_zapis_do_OFR,'0') as Cena_zapis_do_OFR , IFNULL(Opis_Cena_zapis_do_OFR,'') as Opis_Cena_zapis_do_OFR ,IFNULL( Opis_Cena_zapis_do_OFR2,'') as Opis_Cena_zapis_do_OFR2, IFNULL(Opis_Cena_zapis_do_OFR3,'') as Opis_Cena_zapis_do_OFR3,
                                    Img ,TDS ,KCH , IFNULL(Plik_Tds_True,'False') as Plik_Tds_True , IFNULL(Plik_Kch_True,'False') as Plik_Kch_True ,IFNULL(FileName,'') as FileName ,
                                    IFNULL(TDS_DO_OFR,'False') as TDS_DO_OFR ,IFNULL(CHAR_DO_OFR,'False') as CHAR_DO_OFR , IFNULL(PDF_DO_OFR,'False') as PDF_DO_OFR ,IFNULL(NazwaPdf,'') as NazwaPdf , IFNULL(OstAkt,'') as OstAkt 
                                FROM BazaOfr_robocze WHERE NIP like '%" + Get_KlientDane.NIP + "%' and SAP IS NOT NULL";
            return sqlqwert;
        }

        private void WczytajAnalizaBR()
        {
            TblAnalizaBranza = SqlComandDatabase(ReturnAnalizaString(), con);
            GetDataZapisAnaliza(TblAnalizaBranza);
            AnalizaBranza.ItemsSource = ListAnalizaBranza;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(AnalizaBranza.ItemsSource);
            var groupDescription = new PropertyGroupDescription("A_NrSAP");
            view.GroupDescriptions.Add(groupDescription);
        }


        public int GetDataZapisAnaliza(DataTable Baza)
        {
            //try
            //{
            int MaxQuant = 0;
            ListAnalizaBranza = new List<AnalizaBranzaData>();
            string r_0 = int.Parse(Upr_User.MaxDataRok).ToString(), r_1 = (int.Parse(Upr_User.MaxDataRok) - 1).ToString(), r_2 = (int.Parse(Upr_User.MaxDataRok) - 2).ToString(), r_3 = (int.Parse(Upr_User.MaxDataRok) - 3).ToString();

            foreach (DataRow row in Baza.Rows)
            {
                if (MaxQuant < int.Parse(row["ALLQuantity"].ToString()))
                    MaxQuant = int.Parse(row["ALLQuantity"].ToString());

                int allquant = int.Parse(row["ALLQuantity"].ToString());
                int Rok1 = int.Parse(row["szt " + r_0].ToString());
                int Rok2 = int.Parse(row["szt " + r_1].ToString());
                int Rok3 = int.Parse(row["szt " + r_2].ToString());
                int Rok4 = int.Parse(row["szt " + r_3].ToString());

                ListAnalizaBranza.Add(new AnalizaBranzaData()
                {
                    A_NrSAP = row["SAP"].ToString(),
                    A_NazwProd = row["Produkt"].ToString(),
                    ALLQuantity = LiczProc(int.Parse(row["ALLQuantity"].ToString()), MaxQuant).ToString(),
                    ALLQ = row["ALLQuantity"].ToString() + " szt.",
                    Sztt1 = int.Parse(row["szt " + r_0].ToString()) + " szt.",
                    Sztt2 = int.Parse(row["szt " + r_1].ToString()) + " szt.",
                    Sztt3 = int.Parse(row["szt " + r_2].ToString()) + " szt.",
                    Sztt4 = int.Parse(row["szt " + r_3].ToString()) + " szt.",
                    Szt_r1 = LiczProc(double.Parse(row["szt " + r_0].ToString()), allquant),
                    Szt_r1_pon_10 = LiczProc(double.Parse(row[r_0 + " 10ponkart"].ToString()), Rok1),
                    Szt_r1_pow_10 = LiczProc(double.Parse(row[r_0 + " 10pow_kart"].ToString()), Rok1),
                    Szt_r1_polPal = LiczProc(double.Parse(row[r_0 + " Pol_pal"].ToString()), Rok1),
                    Szt_r1_Pal = LiczProc(double.Parse(row[r_0 + " 1Pal"].ToString()), Rok1),
                    Szt_r2 = LiczProc(double.Parse(row["szt " + r_1].ToString()), allquant),
                    Szt_r2_pon_10 = LiczProc(double.Parse(row[r_1 + " 10ponkart"].ToString()), Rok2),
                    Szt_r2_pow_10 = LiczProc(double.Parse(row[r_1 + " 10pow_kart"].ToString()), Rok2),
                    Szt_r2_polPal = LiczProc(double.Parse(row[r_1 + " Pol_pal"].ToString()), Rok2),
                    Szt_r2_Pal = LiczProc(double.Parse(row[r_1 + " 1Pal"].ToString()), Rok2),
                    Szt_r3 = LiczProc(double.Parse(row["szt " + r_2].ToString()), allquant),
                    Szt_r3_pon_10 = LiczProc(double.Parse(row[r_2 + " 10ponkart"].ToString()), Rok3),
                    Szt_r3_pow_10 = LiczProc(double.Parse(row[r_2 + " 10pow_kart"].ToString()), Rok3),
                    Szt_r3_polPal = LiczProc(double.Parse(row[r_2 + " Pol_pal"].ToString()), Rok3),
                    Szt_r3_Pal = LiczProc(double.Parse(row[r_2 + " 1Pal"].ToString()), Rok3),
                    Szt_r4 = LiczProc(double.Parse(row["szt " + r_3].ToString()), allquant),
                    Szt_r4_pon_10 = LiczProc(double.Parse(row[r_3 + " 10ponkart"].ToString()), Rok4),
                    Szt_r4_pow_10 = LiczProc(double.Parse(row[r_3 + " 10pow_kart"].ToString()), Rok4),
                    Szt_r4_polPal = LiczProc(double.Parse(row[r_3 + " Pol_pal"].ToString()), Rok4),
                    Szt_r4_Pal = LiczProc(double.Parse(row[r_3 + " 1Pal"].ToString()), Rok4)
                });
            }
            //}
            //catch (Exception ex)
            //{
            //    TextMessage(ex.StackTrace.ToString());
            //}
            return default(int);
        }

        private int LiczProc(double Dzielna, double daielnik)
        {

            if (daielnik == 0)
            {
                //  Console.WriteLine("dzielna = 0     Dzielna {0} daielnik {1}", Dzielna, daielnik);
                return 0;
            }

            else
            {
                int a = ((int)((Dzielna / daielnik) * 100));
                // Console.WriteLine("dzielna =ok    Dzielna {0} daielnik {1} wynik {2}", Dzielna, daielnik, ((Dzielna / daielnik)*100));
                return a;// int.Parse(Math.Round(a, 2).ToString());
            }

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var vis = sender as Visual;

            while (vis != null)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }

                vis = VisualTreeHelper.GetParent(vis) as Visual;
            }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            var vis = sender as Visual;

            while (vis != null)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }

                vis = VisualTreeHelper.GetParent(vis) as Visual;
            }
        }

        public string ReturnAnalizaString()
        {
            // Console.WriteLine(Upr_User.MaxDataRok);

            string r_0 = int.Parse(Upr_User.MaxDataRok).ToString(), r_1 = (int.Parse(Upr_User.MaxDataRok) - 1).ToString(), r_2 = (int.Parse(Upr_User.MaxDataRok) - 2).ToString(), r_3 = (int.Parse(Upr_User.MaxDataRok) - 3).ToString();
            string SqlString = @" Select mc.SAP, IFNULL((mc.NazwProd), 'wycofany -' || si.Material) as Produkt, IFNULL(sum(printf('%d',si.Quantity *1 )),0) as ALLQuantity, " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_0 + "'),0) as 'szt " + r_0 + "', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_0 + "' AND (si.Quantity *1 < mc.Kszt *10) ) ,0) as '" + r_0 + " 10ponkart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_0 + "' AND (si.Quantity *1 >= mc.Kszt *10) AND (si.Quantity *1 < mc.Pszt /2) ) ,0) as '" + r_0 + " 10pow_kart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_0 + "' AND (si.Quantity *1 >= mc.Pszt /2) AND (si.Quantity *1 < mc.Pszt) ) ,0) as '" + r_0 + " Pol_pal', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_0 + "' AND (si.Quantity *1 >= mc.Pszt) ) ,0) as '" + r_0 + " 1Pal', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_1 + "') ,0) as 'szt " + r_1 + "', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_1 + "' AND (si.Quantity *1 < mc.Kszt *10) ) ,0) as '" + r_1 + " 10ponkart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_1 + "' AND (si.Quantity *1 > mc.Kszt *10) AND (si.Quantity *1 < mc.Pszt /2) ) ,0) as '" + r_1 + " 10pow_kart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_1 + "' AND (si.Quantity *1 > mc.Pszt /2) AND (si.Quantity *1 < mc.Pszt) ) ,0) as '" + r_1 + " Pol_pal', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_1 + "' AND (si.Quantity *1 >= mc.Pszt) ) ,0) as '" + r_1 + " 1Pal',	" +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_2 + "') ,0) as 'szt " + r_2 + "', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_2 + "' AND (si.Quantity *1 < mc.Kszt *10) ),0)  as '" + r_2 + " 10ponkart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_2 + "' AND (si.Quantity *1 > mc.Kszt *10) AND (si.Quantity *1 < mc.Pszt /2) ) ,0) as '" + r_2 + " 10pow_kart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_2 + "' AND (si.Quantity *1 > mc.Pszt /2) AND (si.Quantity *1 < mc.Pszt) ) ,0) as '" + r_2 + " Pol_pal', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_2 + "' AND (si.Quantity *1 >= mc.Pszt) ) ,0) as '" + r_2 + " 1Pal', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_3 + "') ,0) as 'szt " + r_3 + "', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_3 + "' AND (si.Quantity *1 < mc.Kszt *10) ) ,0) as '" + r_3 + " 10ponkart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_3 + "' AND (si.Quantity *1 > mc.Kszt *10) AND (si.Quantity *1 < mc.Pszt /2) ) ,0) as '" + r_3 + " 10pow_kart', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_3 + "' AND (si.Quantity *1 > mc.Pszt /2) AND (si.Quantity *1  < mc.Pszt) ),0)  as '" + r_3 + " Pol_pal', " +
        "IFNULL(sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + r_3 + "' AND (si.Quantity *1 >= mc.Pszt) ) ,0) as '" + r_3 + " 1Pal' " +
    " From BazaZKP si " +
        "LEFT JOIN BazaKL md ON md.Numer_konta = substr(si.SoldTocustomer, 1, 7) " +
       " LEFT JOIN Cennik mc ON mc.SAP = substr(si.Material, 1, 6) " +
    "WHERE md.Branza like '%" + Get_KlientDane.Branza.Substring(0, 2) + @" %' and si.Quantity  not like '%-%' and mc.KATEGORIA  NOT like 'MARKETING' " +
    " GROUP BY mc.SAP " +
    " Order By ALLQuantity DESC ;";


            //  Console.WriteLine(SqlString);
            return SqlString;
        }

        public void serchListVisible()
        {
            Dispatcher.Invoke(() =>
            {
                if (ListTblOfr.Tbl_Add_prodList.Count == 0)
                    Mw.PaneLnaw.Visibility = Visibility.Hidden;
                else
                    Mw.PaneLnaw.Visibility = Visibility.Visible;

                if (TabelaOdczytZK.Rows.Count == 0)
                    ZapisZK.Visibility = Visibility.Collapsed;
                else
                    ZapisZK.Visibility = Visibility.Visible;

                if (TblZapisaneOferta_of_liczOfr.Rows.Count == 0)
                    ZapisOFR.Visibility = Visibility.Collapsed;
                else
                    ZapisOFR.Visibility = Visibility.Visible;

                if (TblZalup_private_of_LiczOfr.Rows.Count == 0)
                    HistZak.Visibility = Visibility.Collapsed;
                else
                    HistZak.Visibility = Visibility.Visible;
            });
        }

        private void ControlToBG()
        {
            BG_StInfo = StInfo;
            BG_ZapisaneOferty = ZapisaneOferty;
        }

        public void BG_Aktualizuj_DoWork(object sender, DoWorkEventArgs e)
        {
            BG_WgrajDane.WorkerReportsProgress = true; BackgroundWorker bw = sender as BackgroundWorker; int arg = System.Convert.ToInt32(e.Argument);
            e.Result = Aktualizacja_bazy_dane(bw, arg);
            if (bw.CancellationPending)
                e.Cancel = true;
        }

        private int Aktualizacja_bazy_dane(BackgroundWorker bw, int sleepPeriod)
        {
            //int result = 0;
            while (!bw.CancellationPending)
            {
                // BG_WgrajDane.ReportProgress(1) : WyswietlHistZakupow()
                BG_WgrajDane.ReportProgress(2); WczytajZapisProd();
            }
            return 1;
        }

        private void BG_Aktualizuj_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
            } // TextMessage("Aktualizacja Zakończono z błędem. Zatrzymana została na działaniu " & ileZ) '  MsgBox("Zakończono z błędem")
            if (e.Cancelled)
                MessageBox.Show("Operacja została anulowana");
            else if (e.Error != null)
            {
                string msg = string.Format("Wystąpił błąd: {0}", e.Error.Message);
                MessageBox.Show(msg);
            }
            else
            {
                string msg; // = ""
                if (e.Result.ToString() == "1")
                {
                }
                if (e.Result.ToString() == "2")
                    msg = string.Format("Nieoczekiwany błąd!" + Microsoft.VisualBasic.Constants.vbCrLf + "Sprawdz czy wskazano własciwy plik");
                if (e.Result.ToString() == "3")
                {
                }
            }
            ZapisaneOferty.Visibility = Visibility.Visible;
        }

        private void BG_Aktualizuj_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int _Stan = (e.ProgressPercentage);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                STpanel.LayoutTransform = Upr_User.dpiTransform;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        public void WyswietlAllZK()
        {
            string searchQuery = @"Select si.NIP, si.NrSAP, md.NazwProd, si.ZK1 ,si.ZK1Info, si.ZK2, si.ZK2Info,  si.ZK3, si.ZK3Info 
                                    from TabZK si
                                    LEFT JOIN  Cennik md ON md.SAP = si.NrSAP
                                    WHERE si.NIP Like '%" + Get_KlientDane.NIP + "%' AND si.ZK1 not like '' ";
            TabelaOdczytZK = SqlComandDatabase(searchQuery, con).Copy();


            Dispatcher.Invoke(() =>
            {
                DataZK11.ItemsSource = TabelaOdczytZK.DefaultView;


                if (TabelaOdczytZK.Rows.Count == 0)
                    ZapisZK.Visibility = Visibility.Collapsed;
                else
                    ZapisZK.Visibility = Visibility.Visible;
                GetDataZapisZK(TabelaOdczytZK);
            });
        }

        private void WyswietlHistZakupow()
        {
            try
            {
                // If con.State = ConnectionState.Closed Then con.Open()
                string klient = Get_KlientDane.Numer_konta;
                if (klient == "")
                    klient = "00000000";
                string selectFilter = string.Format("SoldTocustomer LIKE '%{0}%' ", klient);
                TblZalup_private_of_LiczOfr = BazaZakupyAllKl_Public.Clone();
                DataRow[] dataRows = BazaZakupyAllKl_Public.Select(selectFilter);
                foreach (DataRow typeDataRow in dataRows)
                {
                    TblZalup_private_of_LiczOfr.ImportRow(typeDataRow);
                }
                //  Console.WriteLine("TblZalup_private_of_LiczOfr {0}", TblZalup_private_of_LiczOfr.Rows.Count);
                TblSort = Segreg(TblZalup_private_of_LiczOfr, "SoldTocustomer", "Material", "Yearbilling", "SalesP", "Quantity", "Datebilling", "Representative", "Turnover");
                GetDataZapisOFR(TblSort, ListHistoriaZKP);
                if (TblSort != null)
                    TblSortViev = TblSort.Copy();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        public void WczytajZapisProd()
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            TblZapisaneOferta_of_liczOfr.Clear();

            try
            {
                string nrKlient = Get_KlientDane.Numer_konta;
                if (nrKlient == "" | nrKlient == null)
                    nrKlient = "99999999999";
                string sqwerty = @"SELECT distinct Data,Numer_konta,SAP,NazwProd,CenaDoOFR,replace(ZK1,'-','') as ZK1,ZK2,ZK3,szt1,szt2,szt3 FROM TblOferta 
                                                WHERE Numer_konta not like '%%' and  Numer_konta like '%" + nrKlient + @"%' or Numer_konta 
                                            like '%" + Get_KlientDane.NIP + "%'";
                // Dim dt2 As New DataTable()
                TblZapisaneOferta_of_liczOfr = SqlComandDatabase(sqwerty, con);

                GetDataZapisOFR(TblZapisaneOferta_of_liczOfr, ListZapisOferty);
                ZapisaneOferty.ItemsSource = TblZapisaneOferta_of_liczOfr.DefaultView;

                if (TblZapisaneOferta_of_liczOfr.Rows.Count == 0)
                    ZapisOFR.Visibility = Visibility.Collapsed;
                else
                    ZapisOFR.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }

            ZapisaneOferty.Visibility = Visibility.Visible;
        }

        public void GetDataZapisOFR(System.Data.DataTable Baza, List<CennikData> AddRowBaza)
        {
            try
            {
                if (ListZapisOferty != null)
                    ListZapisOferty.Clear();
                AddRowBaza = new List<CennikData>();
                if (AddRowBaza.Count > 0)
                    AddRowBaza.Clear();
                if (Baza != null)
                {
                    foreach (DataRow row in Baza.Rows)
                    {
                        if (Baza.TableName == "TblSort")
                            AddRowBaza.Add(new CennikData() { SAP = row["Material"].ToString().Substring(0, 6) });
                        if (Baza.TableName == "TabZapisaneOferta")
                            AddRowBaza.Add(new CennikData() { SAP = row["SAP"].ToString().Substring(0, 6) });
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        public void GetDataZapisZK(DataTable Baza)
        {
            // Try
            if (ListZapisZK != null)
                ListZapisZK.Clear();
            ListZapisZK = new List<ZK11Data>();
            foreach (DataRow row in Baza.Rows)
                ListZapisZK.Add(new ZK11Data()
                {
                    NIP = row["NIP"].ToString(),
                    NrSAP = row["NrSAP"].ToString(),
                    NazwProd = row["NazwProd"].ToString(),
                    ZK1 = row["ZK1"].ToString(),
                    ZK2 = row["ZK2"].ToString(),
                    ZK3 = row["ZK3"].ToString(),
                    ZK1Info = row["ZK1Info"].ToString(),
                    ZK2Info = row["ZK2Info"].ToString(),
                    ZK3Info = row["ZK3Info"].ToString()
                });
        }

        private void Label_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            ZwinLab(((Label)sender).Content.ToString()); //; ((Label)sender).Content);
        }

        public void ZwinLab(string Txt)
        {
            if (Txt == "rozwiń")
            {
                Gr0.Height = GridLength.Auto;  // New GridLength(1.9, GridUnitType.Star) ' GridLength.Auto
                Gr1.Height = new GridLength(0.5, GridUnitType.Star);
                DocListViev.Visibility = Visibility.Visible;
                xKryj.Content = "zwiń";
            }
            else
            {
                // Gr0.Height = GridLength.Auto ' New GridLength(0.1, GridUnitType.Star)
                Gr1.Height = new GridLength(1, GridUnitType.Star); // GridLength.Auto
                DocListViev.Visibility = Visibility.Collapsed;
                xKryj.Content = "rozwiń";
            }
            //return null;
        }

        public void StickpanGrand()
        {
            try
            {
                if (Get_KlientDane.PraceList == null/* TODO Change to default(_) if this is not a reference type */ )
                {
                    LabInfoZRP.Content = "Nie przypisano rabatu podstawowego - PraceList. Zmień, jezeli wymagane!";
                    StackPanelInfo.Background = new SolidColorBrush(Color.FromRgb(255, 204, 204));
                }
                else
                {
                    LabInfoZRP.Content = "";
                    StackPanelInfo.Background = null;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            // return null;
        }

        private void Szukaj_folder(object sender, RoutedEventArgs e)
        {
            try
            {
                string UserName = Environment.UserName;
                MakeDir(Upr_User.User_PH, Strim_URL, Uide, Pas);
                MakeDir("BazaKL", Strim_URL + Upr_User.User_PH, Uide, Pas);
                MakeDir("BazaOfr", Strim_URL + Upr_User.User_PH, Uide, Pas);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return;
        }

        private void MakeDir(string dirName, string ServerIP, string UserId, string Password)
        {
            Stream ftpStream = null;
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ServerIP + "/" + dirName));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(UserId, Password);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
                Interaction.MsgBox("dodałem");
            }
            catch (Exception ex)
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                    ftpStream.Dispose();
                }
                TextMessage(ex.StackTrace.ToString());
            }
        }



        private void WstawImg()
        {
            try
            {
                var gridView = Cennik.View as GridView;
                int j = 0;
                foreach (var column in gridView.Columns)
                {
                    if (column.Header.ToString() == "Tds")
                        break;
                    j += 1;
                }
                int i = 0;
                foreach (var item in Mw.ListCennik)
                    // Cennik.Items(i)(j) = item.Tds
                    i += 1;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void CreateDynamicGridView()
        {
            if (TblSort == null)
                return;

            GridView grdView = new GridView();
            GridView grdView1 = new GridView();

            foreach (DataColumn col in TblSort.Columns)
            {
                GridViewColumn bookColumn = new GridViewColumn() { DisplayMemberBinding = new Binding(col.ColumnName), Header = col.ColumnName };
                bookColumn.Width = (col.ColumnName.Contains("Materi")) ? 200 : 100;
                if (col.ColumnName == "SoldTo")
                    bookColumn.Width = 0;
                if (col.ColumnName == "PH")
                    bookColumn.Width = 0;
                if (col.ColumnName == "Wyswietl")
                    bookColumn.Width = 0;
                grdView.Columns.Add(bookColumn);
            }

            HistoriaZakup.DataContext = grdView;


            foreach (DataColumn col in TblSort.Columns)
            {
                GridViewColumn bookColumn = new GridViewColumn() { DisplayMemberBinding = new Binding(col.ColumnName), Header = col.ColumnName };
                bookColumn.Width = (col.ColumnName.Contains("Materi")) ? 200 : 100;

                if (col.ColumnName == "SoldTo")
                    bookColumn.Width = 0;
                if (col.ColumnName == "PH")
                    bookColumn.Width = 0;
                if (col.ColumnName == "Wyswietl")
                    bookColumn.Width = 0;
                grdView1.Columns.Add(bookColumn);
            }
            HistoriaZakupViev.DataContext = grdView1;
            if (TblSort.Rows.Count == 0)
                HistZak.Visibility = Visibility.Collapsed;
            else
                HistZak.Visibility = Visibility.Visible;
            Serchhist(TblSort, "Rozwiń", " ");
            Binding bind = new Binding() { Source = TblSort.DefaultView };
            HistoriaZakup.SetBinding(ListView.ItemsSourceProperty, bind);
            Serchhist(TblSortViev, " ", " ");
            Binding bind1 = new Binding() { Source = TblSortViev.DefaultView };
            HistoriaZakupViev.SetBinding(ListView.ItemsSourceProperty, bind1);
            HistoriaZakupViev.Visibility = Visibility.Collapsed;
        }

        private void LvUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int SComp;
                SComp = HistoriaZakup.SelectedIndex;
                if (SComp >= 0)
                {
                    DataRowView item = HistoriaZakup.Items.GetItemAt(HistoriaZakup.SelectedIndex) as DataRowView;
                    GridView itTab = HistoriaZakup.DataContext as GridView;

                    if (!DBNull.Value.Equals(itTab.Columns.Count - 1))
                    {
                        Serchhist(TblSortViev, " ", (string)item[1]);
                    }
                    else
                    {
                        Serchhist(TblSortViev, " ", (string)item[1]);
                    }
                    SapPr.Text = (string)item[1];
                    NazwaPR.Content = item[2];
                    HistoriaZakupViev.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void AnalizaBranza_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var row_list = (AnalizaBranzaData)AnalizaBranza.SelectedItem;
                // Indnr.Content = row_list.A_NazwProd ' sender.SelectedItem.ToString
                if (row_list != null)
                    AnalizaNazwaPR.Text = Strings.Replace(row_list.A_NrSAP.ToString(), " ", ""); // "You Selected: " & row_list.A_NrSAP & " " & row_list.A_NazwProd
            }
            // Serchhist(TblSortViev, " ", row_list.A_NrSAP)
            catch
            {
            }
        }

        private void Serchhist(System.Data.DataTable DTHist, string Rozwin, string valueToSearch)
        {
            string T1 = Rozwin; // "Rozwiń" ' ""
            string T2 = valueToSearch; // ""
            try
            {
                DTHist.DefaultView.RowFilter = string.Format("Wyswietl LIKE '%{0}%' or Sap LIKE '%{1}%' ", T1, T2);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // szukaj text w DATAGRID VIEW _STRING FORMAT

        public void WczytajOfrDoCombo()
        {
            string sqSap = Get_KlientDane.Numer_konta;
            string sqNIP = Get_KlientDane.NIP; // KlientDane.Numer_konta
            ComboOfr.ItemsSource = null;
            if (con.State == ConnectionState.Closed)
                con.Open();
            try
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT distinct SAP,NrOFR FROM TblPdf WHERE SAP ='" + sqSap + "' or SAP ='" + sqNIP + "' AND PlkPdf IS NOT NULL", con);
                DataTable dtC = new DataTable();
                int i = da.Fill(dtC);
                if (i > 0)
                {
                    DataRow row = dtC.NewRow();
                    dtC.Rows.InsertAt(row, 0);
                    ComboOfr.ItemsSource = dtC.DefaultView;
                    ComboOfr.DisplayMemberPath = "NrOFR";
                }
                if (dtC.Rows.Count > 0)
                    ComOFR.Visibility = Visibility.Visible;
                else
                    ComOFR.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // Wpisz oferty klienta do ComboBox

        private void LOComboOfr_SChanged(object sender, TextChangedEventArgs e)
        {
            string sFileName = "";
            // Try
            string strSql;
            if (ComboOfr.Text.Length > 0)
                LabNazw.Visibility = Visibility.Collapsed;
            else
                LabNazw.Visibility = Visibility.Visible;
            sFileName = ComboOfr.Text.ToString(); // EncodeString(ComboOfr.Text.ToString)
            ClearComb();
            if (sFileName == null)
                return;
            strSql = "Select PlkPdf from TblPdf WHERE NrOFR like '%" + sFileName + "%';";
            if (con.State == ConnectionState.Closed)
                con.Open();
            SQLiteCommand sqlCmd = new SQLiteCommand(strSql, con);

            byte[] fileData = (byte[])sqlCmd.ExecuteScalar();

            string sTempFileName = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, sFileName);
            try
            {
                using (System.IO.FileStream FS = new System.IO.FileStream(sTempFileName, System.IO.FileMode.Create))
                {
                    FS.Write(fileData, 0, fileData.Length);
                    FS.Close();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }

            Open_Wind_of_Html_Add(fileData, sFileName);
        } // wyświetl wybierz Oferę do pokazania - Wyświetlenia


        private int ClearComb()
        {
            try
            {
                if (ComboOfr.Text != "")
                {
                    ComboOfr.Text = "";
                    ComboOfr.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return 0;
        }
        public void Czysc_OFR()
        {
            try
            {
                FlowLayoutPanel1.Children.Clear();
                ListTblOfr.Tbl_Add_prodList.Clear();
                Cennik.UnselectAll();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void ListViewItem_Img_clik(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
                ImgView(sender);
        }
        public void ImgView(object sender)
        {
            int i;
            for (i = 0; i <= BazaCennik.Rows.Count - 1; i++)
            {
                if ((BazaCennik.Rows[i]["SAP"].ToString() == ((Label)sender).Tag.ToString()) && (BazaCennik.Rows[i]["Img"].ToString() != ""))
                {
                    Mw.ClearImg();
                    byte[] imgData = (byte[])BazaCennik.Rows[i]["Img"];
                    Image DocPan = new Image();
                    {
                        var withBlock = DocPan;
                        withBlock.Height = 550;
                        withBlock.HorizontalAlignment = HorizontalAlignment.Center;
                        VerticalAlignment = VerticalAlignment.Center;
                        withBlock.Source = LoadImage(imgData);
                    }
                    Mw.VievPage.Children.Add(DocPan);
                    Mw.VievPage.Visibility = Visibility.Visible;
                    Mw.PageClear.Visibility = Visibility.Visible;
                    break;
                }

            }
        }
        public static BitmapImage LoadImage(byte[] imageData)
        {
            // Try
            if (imageData == null || imageData.Length == 0)
                return null/* TODO Change to default(_) if this is not a reference type */;
            var image = new BitmapImage();
            try
            {
                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return image;
        }

        private void ListViewItem_Tds_clik(object sender, MouseButtonEventArgs e)
        {
            PdfView(sender, "Tds");
        }
        private void ListViewItem_Char_clik(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PdfView(sender, "KC");
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }


        public void PdfView(object sender, string Typ)
        {

            int i;
            string tg = ((Label)sender).Tag.ToString();
            string SName = Typ; // "Tds" ' sender.Name
            for (i = 0; i <= BazaCennik.Rows.Count - 1; i++)
            {
                if ((BazaCennik.Rows[i]["SAP"].ToString() == tg) && (BazaCennik.Rows[i][SName].ToString() != ""))
                {
                    if (Wind_of_Html_Add == null)
                    {
                        Window window = new Wind_of_Html();
                        byte[] fileData = (byte[])BazaCennik.Rows[i][SName];
                        string Name = BazaCennik.Rows[i]["NAZEWNICTWO"].ToString() + "_" + SName;
                        Open_Wind_of_Html_Add(fileData, Name);
                        Wind_of_Html_Add.Activate();
                        Wind_of_Html_Add.Focus();
                    }
                    else
                    {
                        byte[] fileData = (byte[])BazaCennik.Rows[i][SName];
                        string Name = BazaCennik.Rows[i]["NAZEWNICTWO"].ToString() + "_" + SName;
                        Wind_of_Html_Add.Activate();
                        Wind_of_Html_Add.Focus();
                        Open_Wind_of_Html_Add(fileData, Name);
                    }
                    return;
                }
            }
        }
        internal void Open_Wind_of_Html_Add(byte[] fileData, string name)
        {
            try
            {
                if (fileData != null & name.Length > 1)
                {
                    if (Wind_of_Html_Add == null)
                    {
                        Window window = new Wind_of_Html();

                        window.Show();
                    }
                    else
                    {
                        Wind_of_Html_Add.Activate();
                        Wind_of_Html_Add.Focus();
                        Wind_of_Html_Add.Show();
                    }
                    name = EncodeString(name);
                    Wind_of_Html_Add.Generuj_TDS_Tabela(fileData, name);
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void ShowPdf(string tg)
        {
            do
            {
                try
                {
                    int i;
                    var loopTo = BazaCennik.Rows.Count - 1;
                    for (i = 0; i <= loopTo; i++)
                    {
                        if (BazaCennik.Rows[i]["SAP"].ToString() == tg & BazaCennik.Rows[i]["Tds"].ToString() != "")
                            goto line1;
                    }
                    break;
                line1:
                    ;

                    string sFileName = "plik.pdf"; // "Cennik_Add.ComboOfr.Text
                    byte[] fileData = (byte[])BazaCennik.Rows[i]["Tds"];

                    string sTempFileName = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, sFileName);
                    using (var FS = new FileStream(sTempFileName, FileMode.Create, FileAccess.Write))
                    {
                        FS.Write(fileData, 0, fileData.Length - 1);
                    }
                    var proc = new Process();
                    proc.StartInfo.FileName = sTempFileName;
                    proc.Start();
                }
                catch (Exception ex)
                {
                    TextMessage(ex.StackTrace.ToString());
                }
            }
            while (false);
        } // Wywołaj PDF oferta woknie systemowym Acrobat Roader


        private void Cennik_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            skaleListViev(sender);
        }

        private void courseView_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            var view = Cennik.View as GridView;
            AutoResizeGridViewColumns(view);
        }

        private static void AutoResizeGridViewColumns(GridView view)
        {
            if (view == null || view.Columns.Count < 1)
                return;

            foreach (var column in view.Columns)
            {
                // Console.WriteLine(column.Width);
                if (double.IsNaN(column.Width))
                    column.Width = 1;
                column.Width = double.NaN;
                //  Console.WriteLine(column.ActualWidth);
            }
            //    PrdColWidth.ColNameProdWidth = 200;
        }
        private void AdjustGridSize()
        {
        }


        private void skaleListViev(object sender)
        {
            int nrCol = 2;
            double remainingSpace;
            Lloc.Text = (Upr_User.CenaKO == false) ? "0" : "80";

            GridView colH = default;

            if (sender is DataGrid)
            {
                if (((DataGrid)sender).Name.ToString() == "HistoriaZakup")
                    nrCol = 2;
                if (((DataGrid)sender).Name.ToString() == "AnalizaBranza")
                    nrCol = 2;
                remainingSpace = (((DataGrid)sender).ActualWidth - 60);
                colH = ((DataGrid)sender).DataContext as GridView;
                foreach (var col in ((DataGrid)sender).Columns)
                {
                    col.Width = ((col.Header.ToString() == "Material") || (col.Header.ToString() == "Produkt")) ? new DataGridLength(200, DataGridLengthUnitType.Star) : 100;
                    if (col.Header.ToString() == "SoldTo")
                    {
                        col.Visibility = Visibility.Collapsed;
                    }
                    if (col.Header.ToString() == "PH")
                    {
                        col.Visibility = Visibility.Collapsed;
                    }
                    if (col.Header.ToString() == "Wyswietl")
                    {
                        col.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                if ((((ListView)sender).Name.ToString() == "Cennik"))
                    nrCol = 2;
                if ((((ListView)sender).Name.ToString() == "ZapisaneOferty") || (((ListView)sender).Name.ToString() == "DataZK11"))
                    nrCol = 3;
                remainingSpace = (((ListView)sender).ActualWidth - 60);
                colH = ((ListView)sender).View as GridView;
            }

            double i = 30;
            if (colH != null)
            {
                BladDatatableinfo.Content = "";
                for (int c = 0, loopTo = colH.Columns.Count - 1; c <= loopTo; c++)
                {
                    if (c != nrCol)
                        i += (double)colH.Columns[c].ActualWidth;
                }
                if (sender is DataGrid)
                {
                    (((DataGrid)sender).DataContext as GridView).Columns[nrCol].Width = Math.Round(remainingSpace - i, 2);
                    double TR = 0.99d;
                    ScaleTransform LSTTransform;
                    LSTTransform = new ScaleTransform(TR, TR);
                    ((DataGrid)sender).LayoutTransform = LSTTransform;
                }
                else
                {
                    if ((remainingSpace - i) > 1)
                    {
                        (((ListView)sender).View as GridView).Columns[nrCol].Width = Math.Round(remainingSpace - i, 2);
                        double TR = 0.99d;
                        ScaleTransform LSTTransform;
                        LSTTransform = new ScaleTransform(TR, TR);
                        ((ListView)sender).LayoutTransform = LSTTransform;
                    }
                }

            }
            else
            {
                // BladDatatableinfo.Content = "Baza zakupów nie może zostać wyświetlona" + Microsoft.VisualBasic.Constants.vbCrLf + "Na danym koncie mogły zaistnieć zmany !!";
            }

            //    PrdColWidth.ColNameProdWidth = 200;

        }




        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtAddProd.Text != "")
            {
                SzukajTxt.Content = ""; CombGr1.Text = ""; CombGr2.Text = ""; CombGr3.Text = ""; czyscTxtOfr.Visibility = Visibility.Visible;
            }
            else
            {
                SzukajTxt.Content = "Szukaj Produktu"; czyscTxtOfr.Visibility = Visibility.Collapsed;
            }
            SercgData(TxtAddProd.Text, true);
        }
        private void ComboBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((ComboBox)sender).Text == "")
                return;
            else
                TxtAddProd.Text = ""; // Console.WriteLine(sender.text)
            SercgData(CombGr1.Text, false);
            // Dim serchqwery As String = "SELECT distinct  KATEGORIA FROM Cennik WHERE GRUPA like '%" & CombGr1.Text & "' AND KATEGORIA IS NOT '' and  KATEGORIA IS NOT NULL"
            string serchqwery = "SELECT distinct rtrim(KATEGORIA, ' ') as KATEGORIA from Cennik WHERE GRUPA like '%" + CombGr1.Text + "' AND KATEGORIA IS NOT '' and  KATEGORIA IS NOT NULL";
            CombGr2.ItemsSource = SqlComandDatabase(serchqwery, con).DefaultView;
        }
        private void ComboBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((ComboBox)sender).Text == "")
                return;
            SercgData(CombGr2.Text, false);
            string serchqwery = "SELECT distinct rtrim(NAZEWNICTWO , ' ' ) as NAZEWNICTWO FROM Cennik WHERE KATEGORIA like '%" + CombGr2.Text + "' AND NAZEWNICTWO IS NOT '' and  NAZEWNICTWO IS NOT NULL";
            CombGr3.ItemsSource = SqlComandDatabase(serchqwery, con).DefaultView;
        }
        private void ComboBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((ComboBox)sender).Text == "")
                return;
            SercgData(CombGr3.Text, false);
        }

        private void SercgData(string valueToSearch, bool contI)
        {
            BazaCennik.DefaultView.RowFilter = null;
            if (valueToSearch == null)
            {
                BazaCennik.DefaultView.RowFilter = null; return;
            }
            string T1 = ""; string T2 = ""; string T3 = ""; string T4 = ""; string T5 = ""; string T6 = "";
            var splittext = valueToSearch.Split(' ');
            try
            {
                if (splittext[0].Length > 0)
                {
                    T1 = splittext[0].ToString(); valueToSearch = splittext[0].ToString();
                }
                else
                    T1 = "";
                if (splittext[1].Length > 0)
                    T2 = splittext[1].ToString();
                else
                    T2 = "";
                if (splittext[2].Length > 0)
                    T3 = splittext[2].ToString();
                else
                    T3 = "";
                if (splittext[3].Length > 0)
                    T4 = splittext[3].ToString();
                else
                    T4 = "";
                if (splittext[4].Length > 0)
                    T5 = splittext[4].ToString();
                else
                    T5 = "";
                if (splittext[5].Length > 0)
                    T6 = splittext[5].ToString();
                else
                    T6 = "";
            }
            catch
            {
            } // Console.WriteLine("T1 {0} T2 {1} T3 {2} T4 {3} T5 {4} T6 {5} ", T1, T2, T3, T4, T5, T6)
            string C1 = null; string C2 = null; string C3 = null; // 
            try
            {
                string serch_txt = null;
                if (contI == true)
                    serch_txt = @"NazwProd LIKE '%{0}%' and NazwProd Like '%{1}%' and NazwProd Like '%{2}%' and NazwProd Like '%{3}%' and NazwProd Like '%{4}%' and NazwProd Like '%{5}%'
                                     Or SAP LIKE '%{0}%' and SAP Like '%{1}%' and SAP Like '%{2}%' and SAP Like '%{3}%' and SAP Like '%{4}%' and SAP Like '%{5}%'
                                     Or GRUPA LIKE '%{0}%' and GRUPA Like '%{1}%' and GRUPA Like '%{2}%' and GRUPA Like '%{3}%' and GRUPA Like '%{4}%' and GRUPA Like '%{5}%'
                                     Or KATEGORIA LIKE '%{0}%' and KATEGORIA Like '%{1}%' and KATEGORIA Like '%{2}%' and KATEGORIA Like '%{3}%' and KATEGORIA Like '%{4}%' and KATEGORIA Like '%{5}%'
                                     Or NAZEWNICTWO LIKE '%{0}%' and NAZEWNICTWO Like '%{1}%' and NAZEWNICTWO Like '%{2}%' and NAZEWNICTWO Like '%{3}%' and NAZEWNICTWO Like '%{4}%' and NAZEWNICTWO Like '%{5}%'";
                else
                {
                    C1 = CombGr1.Text; C2 = CombGr2.Text; C3 = CombGr3.Text;
                    // If RTrim(C1) = "" Then
                    serch_txt = @"GRUPA LIKE '%{6}%'
                                     and KATEGORIA LIKE '%{7}%' 
                                     and NAZEWNICTWO LIKE '%{8}%' ";
                } // Console.WriteLine(serch_txt)
                BazaCennik.DefaultView.RowFilter = string.Format(serch_txt, T1, T2, T3, T4, T5, T6, C1, C2, C3);
            }
            catch
            {
            }
            if (valueToSearch == null)
                BazaCennik.DefaultView.RowFilter = null;
        } // szukaj text w DATAGRID VIEW _STRING FORMAT


        public void ZapiszRobocza()
        {
            try
            {
                string Tim = TimeAktual();
                if (ListTblOfr.Tbl_Add_prodList.Count >= 0)
                {
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    string a1, a2, a3, a4, a5, a6, a7, a8, a9;
                    double CZP = 0;
                    double CDO = 0;
                    DateTime dateTime = DateTime.Now;
                    string NrKL = Get_KlientDane.NIP.ToString();
                    string NrSap = Get_KlientDane.Numer_konta;
                    foreach (var item in ListTblOfr.Tbl_Add_prodList)
                    {
                        if (item.SAP.ToString() != "")
                            a1 = item.SAP.ToString();
                        else
                            a1 = "";
                        if (item.NazwProd.ToString() != "")
                            a2 = item.NazwProd.ToString();
                        else
                            a2 = "";
                        if (item.CenaDoOFR.ToString() != "")
                            a3 = item.CenaDoOFR.ToString();
                        else
                            a3 = "";
                        if (item.ZK11A1.ToString() != "")
                            a4 = item.ZK11A1.ToString();
                        else
                            a4 = "";
                        if (item.ZK11A2.ToString() != "")
                            a5 = item.ZK11A2.ToString();
                        else
                            a5 = "";
                        if (item.ZK11A3.ToString() != "")
                            a6 = item.ZK11A3.ToString();
                        else
                            a6 = "";
                        if (item.szt1 != null)
                            a7 = item.szt1;
                        else
                            a7 = "";
                        if (item.szt2 != null)
                            a8 = item.szt2;
                        else
                            a8 = "";
                        if (item.szt3 != null)
                            a9 = item.szt3;
                        else
                            a9 = "";
                        if (item.CenaZPrace.ToString() != "")
                            CZP = item.CenaZPrace;
                        else
                            CZP = 0;
                        if (item.CenaDoOFR.ToString() != "")
                            CDO = item.CenaDoOFR;
                        else
                            CDO = 0;
                        string stringSql;
                        CZP = Math.Round(CZP, 2);
                        CDO = Math.Round(CDO, 2);

                        stringSql = @" -- Try To update any existing row
                                       UPDATE TblOferta
                                               SET Representative = '" + Upr_User.User_PH + "',Data='" + dateTime + "',Numer_konta='" + NrKL + "',SAP='" + a1 + "',NazwProd='" + a2 + "',CenaDoOFR='" + a3 + @"'
                                               ,ZK1='" + a4 + "',Zk2='" + a5 + "',ZK3='" + a6 + "',szt1='" + a7 + "',szt2 ='" + a8 + "',szt3 ='" + a9 + "',OstAkt ='" + Tim + @"'
                                         WHERE SAP like '%" + a1 + "%' and  Numer_konta like '%" + NrKL + "%' or Numer_konta like '%" + NrSap + @"%' and Numer_konta not like '%%' ;
                                     -- If no update happened (i.e. the row didn't exist) then insert one

                                        INSERT INTO TblOferta                      
                                      (Representative ,Data, Numer_konta,SAP,NazwProd, CenaDoOFR, ZK1  , Zk2 , ZK3, szt1, szt2,  szt3,OstAkt)
                                       SELECT '" + Upr_User.User_PH + "','" + dateTime + "','" + NrKL + "','" + a1 + "','" + a2 + "','" + a3 + "','" + a4 + "','" + a5 + "','" + a6 + "','" + a7 + "','" + a8 + "','" + a9 + "','" + Tim + @"'
                                        WHERE (Select Changes() = 0);";


                        // If CDO = CZP Then stringSql = "delete From TblOferta WHERE SAP like '%" & a1 & "%' and  Numer_konta like '%" & NrKL & "%' or Numer_konta like '%" & NrSap & "%' ;"
                        // SqlComand.Text = CDO.ToString & " " & CZP.ToString & " " & stringSql
                        UsingSQLComand(stringSql, con);
                    }
                }
            }
            catch (Exception ex)
            {
                // MsgBox(ex.ToString)
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public BitmapImage ToImage(byte[] array)
        {
            try
            {
                if (array == null)
                {
                    return null;
                }
                using (var ms = new System.IO.MemoryStream(array))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }
        private static byte[] ImageToBytes(BitmapImage image)
        {
            byte[] Data = null;
            try
            {
                PngBitmapEncoder PngEncoder = new PngBitmapEncoder();
                PngEncoder.Frames.Add(BitmapFrame.Create(image));
                using (System.IO.MemoryStream MS = new System.IO.MemoryStream())
                {
                    PngEncoder.Save(MS);
                    Data = MS.ToArray();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return Data;
        }


        private string AddFieldValue(DataRow row, string fieldName)
        {
            try
            {
                if (!DBNull.Value.Equals(row[fieldName]))
                    return System.Convert.ToString(row[fieldName]);
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }

        public DataTable Segreg(DataTable dt, string SoldTo, string Material, string Yearbilling, string Sels, string Quantity, string Date_billing, string Representative, string Turnover)
        {
            int _c;
            DataTable NewTabel = Set_data(TblZalup_private_of_LiczOfr, "SoldTocustomer", "Material", "Yearbilling", "SalesP", "Quantity", "Datebilling", "Representative", "Turnover");
            //try
            //{

            foreach (DataRow row in dt.Rows)
            {
                //if (row.RowState != null)
                //{
                string TextPH = row[Representative].ToString();
                string SolTo = Strings.Mid(row[SoldTo].ToString(), 1, 7);
                string Sap = Strings.Mid(row[Material].ToString(), 1, 6).ToString();
                string NName = Strings.Mid(row[Material].ToString(), 10).ToString();
                string DateB = row[Date_billing].ToString();
                //string NewName = SerchNazw(Sap, NName);
                int YearB = int.Parse(row[Yearbilling].ToString());
                double SelsP = double.Parse(row[Sels].ToString());
                int Quanty = int.Parse(row[Quantity].ToString());
                _c = 0;
                foreach (DataColumn newcol in NewTabel.Columns)
                {
                    if (newcol.ColumnName.ToString() == YearB.ToString())
                    {
                        _c = newcol.Ordinal; goto line2;
                    }
                }
                NewTabel.Columns.Add(YearB.ToString());
                NewTabel.Columns.Add("Szt " + YearB.ToString());
                _c = NewTabel.Columns.Count - 2;
            line2:
                ;
                string Textsels;
                Textsels = SerchYear(SolTo, Sap, YearB.ToString(), DateB);
                foreach (DataRow Rov1 in NewTabel.Rows)
                {
                    if (Rov1["Sap"].ToString() == Sap.ToString())
                        goto line3;
                }

                DataRow drA = NewTabel.NewRow();
                double selsgrup = SelsP;
                for (int i = 2; i <= NewTabel.Columns.Count - 1; i++)
                {
                    int SerchYer;
                    if (NewTabel.Columns[i].ColumnName.ToString().Length == 4)
                    {
                        SerchYer = int.Parse(NewTabel.Columns[i].ColumnName.ToString());
                        Textsels = SerchYear(SolTo, Sap, SerchYer.ToString(), DateB);
                        string[] testArray = Strings.Split(Textsels, "|");
                        double SelsA = double.Parse(testArray[0].ToString());
                        int Qanty = int.Parse(testArray[1].ToString());
                        string QDataB = testArray[2];
                        string Sold = testArray[3];
                        drA[0] = Sold;
                        drA[1] = Sap;
                        drA[2] = NName;// NewName;
                        drA[i] = Math.Round(SelsA, 2) + " zł";
                        drA[i + 1] = Qanty + " szt";
                        if (SelsA == 0)
                            drA[i] = "";
                        if (Qanty == 0)
                            drA[i + 1] = "";
                        drA["PH"] = TextPH;
                        drA["Wyswietl"] = "Rozwiń";
                    }
                }
                NewTabel.Rows.Add(drA);
            line3:
                ;
                DataRow dr = NewTabel.NewRow();
                dr[0] = SolTo;
                dr[1] = Sap;
                dr[2] = NName;// NewName;
                dr[3] = DateB;
                double selsValue = SelsP;
                dr[_c] = Math.Round(selsValue, 2) + " zł";
                dr[_c + 1] = Quanty + " szt";
                dr["PH"] = TextPH;
                NewTabel.Rows.Add(dr);
                //}   
            }
            //}
            //catch (Exception ex)
            //{       
            //    TextMessage(ex.StackTrace.ToString());
            //    return null;
            //}

            //  lastline:
            //;
            return NewTabel;
        }
        public DataTable Set_data(DataTable dt, string SoldTo, string Material, string Yearbilling, string Sels, string Quantity, string DateB, string Ph, string Turnover)
        {
            var res = new DataTable();
            res.Columns.Add("SoldTo");
            res.Columns.Add("Sap");
            if (dt.Rows.Count > 0)
            {
                var dtg = dt.AsEnumerable().GroupBy(r => r[Material], r => r[Yearbilling].ToString());
                res.Columns.Add(Material, dt.Columns[Material].DataType);
                res.Columns.Add(DateB, dt.Columns[DateB].DataType);
                var colNames = dtg.SelectMany(rg => rg).Distinct().OrderBy(n => Operators.ConditionalCompareObjectGreaterEqual(n, n, false));

                foreach (var na in colNames)
                {
                    if (na.ToString() != "")
                    {
                        // Dim result As Decimal = dt.AsEnumerable().Sum(Function(row) row.Field(Of Decimal)(n))
                        double sum = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["Yearbilling"].ToString() == na.ToString())
                                sum += double.Parse(row["Turnover"].ToString());
                        }
                        Label LbL = new Label() { Content = na + " r.", FontSize = 12, Margin = new Thickness(30, 0, 0, 0) };
                        BG_StInfo.Children.Add(LbL);
                        Label LbL1 = new Label() { Content = Strings.Format(sum, "# ### ##0.00") + " zł.", FontSize = 12 };
                        BG_StInfo.Children.Add(LbL1);  // :  Console.WriteLine(na.ToString)
                        res.Columns.Add(na);
                        res.Columns.Add("Szt " + na);
                    }
                }
            }
            res.Columns.Add("PH");
            res.Columns.Add("Wyswietl");
            return res;
        }
        public string SerchYear(string SoldTo, string Material, string YB, string D_B) // , ByVal Yearbilling As String, ByVal Sels As String, ByVal Quantity As String) As DataTable
        {

            string TYear = "";
            //try
            //{
            int TQuantyQ = 0;
            double TSels = 0;
            string TD_B = D_B;
            string SoldT = Strings.Mid(SoldTo, 1, 7);
            foreach (DataRow row in TblZalup_private_of_LiczOfr.Rows)
            {
                //try
                //{
                if (Strings.Mid(row["SoldTocustomer"].ToString(), 1, 7) == Strings.Mid(SoldTo, 1, 7))
                {
                    string SoldTo1 = Strings.Mid(row["SoldTocustomer"].ToString(), 1, 7).ToString();
                    string Yearb = row["Yearbilling"].ToString();
                    double SelsA;
                    if (row["SalesP"].ToString() != "")
                        SelsA = double.Parse(row["SalesP"].ToString());
                    else
                        SelsA = 0;
                    int Qua = int.Parse(row["Quantity"].ToString());
                    string QDb = row["Datebilling"].ToString();
                    if (Strings.Mid(row["Material"].ToString(), 1, 6).ToString() == Material)
                    {
                        if (Yearb == YB)
                        {
                            TQuantyQ += Qua;
                            SoldT = SoldTo1;
                            if (TSels < SelsA)
                                TSels = SelsA;
                            TSels = Math.Round(TSels, 2);
                        }
                    }
                }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.ToString());
                //}
            }
            TYear = TSels + "|" + TQuantyQ + "|" + TD_B + "|" + SoldT;
            //}
            //catch (Exception ex)
            //{           
            //    TextMessage(ex.StackTrace.ToString());
            //    return null;
            //}

            return TYear;
        }
        public string SerchNazw(string newSap, string NewName)
        {
            //try
            //{
            //string NName = "";
            foreach (var dr in Mw.ListCennik)
            {
                if (newSap == dr.SAP)
                {
                    Console.WriteLine("1 - SerchNazw dr.SAP {0}  -  {1} -- {2} ////     {3}", dr.SAP, newSap, NewName, dr.NazwProd);
                    //NName = dr.NazwProd;
                    return dr.NazwProd.ToString();
                }
            }
            Console.WriteLine("Nie wychodzę 1 - SerchNazw dr.SAP {0}  -  {1}", newSap, NewName);
            return NewName;
            //}
            //catch
            //{
            //    return null;
            //}
        }


        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Mw.VievPageVisibli(true, false, "kryj");
                Mw.ClearImg();
                DockPanel DocPan = new DockPanel();
                {
                    var withBlock = DocPan;
                    EdytujZmienDane EditNew = new EdytujZmienDane(true);
                    withBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    VerticalAlignment = VerticalAlignment.Center;
                    withBlock.Children.Add(EditNew);
                    withBlock.Background = new SolidColorBrush(Colors.LightGray);
                }
                Mw.VievPage.Children.Add(DocPan);
                Mw.VievPage.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void FlowLayoutPanel1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (FlowLayoutPanel1.ActualHeight <= 50)
                    Mw.PaneLnaw.Visibility = Visibility.Hidden;
                else
                    Mw.PaneLnaw.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            ComboOfr.Text = "";
        }

        private void PClear_MouseDown(object sender, MouseButtonEventArgs e) // Handles PClear.MouseDown
        {
            NavigationService.GoBack();
        }



        private void T10_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                StickpanGrand();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void LOchkSelectAll_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (chkSelectAll.IsChecked.Value == true)
                {
                    if (Cennik.Visibility == Visibility.Visible)
                        Cennik.SelectAll();
                }
                else if (Cennik.Visibility == Visibility.Visible)
                    Cennik.UnselectAll();

                if (DGVOFRchkSelectAll.IsChecked.Value == true)
                {
                    if (ZapisOfert.Visibility == Visibility.Visible)
                        ZapisaneOferty.SelectAll();
                }
                else if (ZapisOfert.Visibility == Visibility.Visible)
                    ZapisaneOferty.UnselectAll();

                if (ZKchkSelectAll.IsChecked.Value == true)
                {
                    if (HistZK11.Visibility == Visibility.Visible)
                        DataZK11.SelectAll();
                }
                else if (HistZK11.Visibility == Visibility.Visible)
                    DataZK11.UnselectAll();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Chack_Add_Checked(object sender, RoutedEventArgs e)
        {
            string SerhTag = ((CheckBox)sender).Tag.ToString();
            if (Mw.ListCennik != null)
            {
                foreach (var itm in Mw.ListCennik)
                {
                    if (itm.SAP.ToString() == SerhTag)
                        AddIndex(itm);
                }
            }
        }

        private void Chack_Delete_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                string SerhTag = ((CheckBox)sender).Tag.ToString();
                foreach (var itm in Tbl_selectedIndex)
                {
                    if (itm.SAP == SerhTag)
                    {
                        Tbl_selectedIndex.Remove(itm); break;
                    }
                }
                if (Cennik.Visibility == Visibility.Visible)
                {
                    if (Tbl_selectedIndex.Count == 0)
                        chkSelectAll.IsChecked = false;
                } // Cennik.UnselectAll()
                if (ZapisOfert.Visibility == Visibility.Visible)
                {
                    if (Tbl_selectedIndex.Count == 0)
                        DGVOFRchkSelectAll.IsChecked = false;
                } // ZapisaneOferty.UnselectAll()
                if (HistZK11.Visibility == Visibility.Visible)
                {
                    if (Tbl_selectedIndex.Count == 0)
                        ZKchkSelectAll.IsChecked = false;
                } // DataZK11.UnselectAll()
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }


        public void AddIndex(CennikData obj) // As CennikData)
        {
            Tbl_selectedIndex.Add(new CennikData()
            {
                SAP = obj.SAP,
                CbSelectRow = obj.CbSelectRow,
                Id = obj.Id,
                Naglowek = obj.Naglowek,
                Lpgrup = obj.Lpgrup,
                NazwProd = obj.NazwProd,
                Kszt = obj.Kszt,
                Poj = obj.Poj,
                CDM = obj.CDM,
                CK = obj.CK,
                PH = obj.PH,
                ZPR0 = obj.ZPR0,
                GRUPA = obj.GRUPA,
                KATEGORIA = obj.KATEGORIA,
                NAZEWNICTWO = obj.NAZEWNICTWO,
                BrakPrace = obj.BrakPrace,
                Img = obj.Img,
                Tds = obj.Tds,
                KartaCHAR = obj.KartaCHAR,
                Tds_Ok_True = obj.Tds_Ok_True,
                Kchar_Ok_True = obj.Kchar_Ok_True,
                CenaZPrace = obj.CenaZPrace
            });
        }

        public double SerchOFrCzyZK11(DataTable Db, string nameRow, string Sap, string name)
        {
            double CdO = default(double);
            if (Db != null)
            {
                foreach (DataRow row in Db.Rows)
                {
                    if ((string)row[nameRow] == Sap.ToString())
                        CdO = double.Parse(row[name].ToString());
                }
            }
            return CdO;
        }

        public double ObliczCenaPoZK(double CenaZrp0, double Zk, bool Sprczywstaw)
        {
            double Cena = default(double);
            if (Sprczywstaw == true)
                Cena = CenaZrp0;
            if (Zk > 0)
                Cena = Math.Round(CenaZrp0 - (CenaZrp0 / (1 / Zk) / 100), 2);
            else
                Cena = Math.Round(CenaZrp0, 2);
            if (Sprczywstaw == false & Zk == 0)
                Cena = default(double);
            return Cena;
        }
        public void Add_list_to_robocze()
        {
            Add_prod_Robocza();
            Uzupelnij_controlProdukt(null);
        }
        private void Add_prod_Robocza()
        {
            //try
            //{
            foreach (DataRow obj in TblZapisRobocze.Rows)
            {
                if (obj["SAP"] is null || obj["SAP"].ToString() == "")
                {
                }
                TblOfr init = new TblOfr();
                ListTblOfr.Tbl_Add_prodList.Add((
                        init.SAP = obj["SAP"].ToString(),
                        init.ID = int.Parse(obj["ID"].ToString()),
                        init.Naglowek = obj["Naglowek"].ToString(),
                        init.Lpgrup = obj["Lpgrup"].ToString(),
                        init.NazwProd = obj["NazwProd"].ToString(),
                        init.GRUPA = obj["GRUPA"].ToString(),
                        init.KATEGORIA = obj["KATEGORIA"].ToString(),
                        init.NAZEWNICTWO = obj["NAZEWNICTWO"].ToString(),
                        init.Kszt = obj["Kszt"].ToString(),
                        init.Poj = obj["Poj"].ToString(),
                        init.CDM = double.Parse(obj["CDM"].ToString()),
                        init.KO = double.Parse(obj["KO"].ToString()),
                        init.PH = double.Parse(obj["PH"].ToString()),
                        init.ZPR0 = double.Parse(obj["ZPR0"].ToString()),
                        init.BrakPrace = obj["BrakPrace"].ToString(),
                        init.CenaZPrace = double.Parse(obj["CenaZPrace"].ToString()),
                        init.Cena_zapis_do_OFR = double.Parse(obj["Cena_zapis_do_OFR"].ToString()),
                        init.Opis_Cena_zapis_do_OFR = obj["Opis_Cena_zapis_do_OFR"].ToString(),
                        init.ZK11A1 = double.Parse(obj["ZK11A1"].ToString()),
                        init.ZK11A2 = double.Parse(obj["ZK11A2"].ToString()),
                        init.ZK11A3 = double.Parse(obj["ZK11A3"].ToString()),
                        init.szt1 = obj["szt1"].ToString(),
                        init.szt2 = obj["szt2"].ToString(),
                        init.szt3 = obj["szt3"].ToString(),
                        init.CenaDoOFR = double.Parse(obj["CenaDoOFR"].ToString()),
                        init.CenaDoOFR2 = double.Parse(obj["CenaDoOFR2"].ToString()),
                        init.CenaDoOFR3 = double.Parse(obj["CenaDoOFR3"].ToString()),
                        init.Img = obj["Img"] ?? DBNull.Value,
                        init.TDS = obj["TDS"] ?? DBNull.Value,
                        init.KCH = obj["KCH"] ?? DBNull.Value,
                        init.Plik_Tds_True = bool.Parse(obj["Plik_Tds_True"].ToString()),
                        init.Plik_Kch_True = bool.Parse(obj["Plik_Kch_True"].ToString()),
                        init.TDS_DO_OFR = bool.Parse(obj["Plik_Kch_True"].ToString()),
                        init.CHAR_DO_OFR = bool.Parse(obj["CHAR_DO_OFR"].ToString()),
                        init.PDF_DO_OFR = bool.Parse(obj["PDF_DO_OFR"].ToString()),
                        init.FileName = obj["FileName"].ToString(),
                        init.NazwaPdf = obj["NazwaPdf"].ToString(),
                        init.CenaBrutto = init.CenaDoOFR * 1.23d, init).init);
            }
            //}
            //catch (Exception ex)
            //{
            //    TextMessage(ex.StackTrace.ToString());
            //}
            List_Add_prodList.ItemsSource = ListTblOfr.Tbl_Add_prodList;
            List_Add_prodList.Items.Refresh();

        }

        private string Add_prod_TblOfr(CennikData obj)
        {

            double.TryParse(obj.CK.ToString(), out double Outdob);
            double.TryParse(obj.ZPR0.ToString(), out double Outdob1);
            double.TryParse(obj.PH.ToString(), out double Outdob2);
            double.TryParse(obj.CDM.ToString(), out double Outdob3);
            if (Outdob == 0 | Outdob1 == 0 | Outdob2 == 0)
            {
                if (Outdob3 == 0)
                {
                    Interaction.MsgBox(obj.SAP.ToString() + " - " + obj.NazwProd.ToString() + Microsoft.VisualBasic.Constants.vbCrLf + "Brak ceny CDM");
                    return obj.SAP;
                    //return default;
                }
                if (Outdob == 0)
                {
                    Interaction.MsgBox(obj.SAP.ToString() + " - " + obj.NazwProd.ToString() + Microsoft.VisualBasic.Constants.vbCrLf + "Brak ceny KO");
                    return obj.SAP;
                    //return default;
                }
                if (Outdob1 == 0)
                {
                    Interaction.MsgBox(obj.SAP.ToString() + " - " + obj.NazwProd.ToString() + Microsoft.VisualBasic.Constants.vbCrLf + "Brak ceny ZRP0");
                    return obj.SAP;
                    //return default;
                }
                if (Outdob2 == 0)
                {
                    Interaction.MsgBox(obj.SAP.ToString() + " - " + obj.NazwProd.ToString() + Microsoft.VisualBasic.Constants.vbCrLf + "Brak ceny PH");
                    return obj.SAP;
                    //return default;
                }
            }

            double CenaZpracelist = CenaZ_praceList(obj.BrakPrace, obj.ZPR0, Get_KlientDane.Rabat_Double);
            Console.WriteLine(" private string Add_prod_TblOfr(CennikData obj) = CenaZpracelist " + CenaZpracelist);
            double CenaZapisanaWOFR = AddDecimal_o_ofr(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "CenaDoOFR"); // , CenaZpracelist)
            Console.WriteLine(" private string Add_prod_TblOfr(CennikData obj) = CenaZapisanaWOFR" + CenaZapisanaWOFR);                                                                                                      // Dim SerchOfrAZk As Decimal = Nothing
            double Zk1_OFR = AddDecimalTo_ZK(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "ZK1");
            double Zk2_OFR = AddDecimalTo_ZK(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "ZK2");
            double Zk3_OFR = AddDecimalTo_ZK(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "ZK3");
            string OpisDta = null;
            double Zk1 = AddDecimalTo_ZK(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK1"); // : OpisDta = "ZapisanoZK11!"
            double Zk2 = AddDecimalTo_ZK(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK2");
            double Zk3 = AddDecimalTo_ZK(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK3");
            string OfrData = AddString_o_ofr(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "Data");
            if (!string.IsNullOrEmpty(OfrData))
                OfrData = "OFR! - " + Strings.Mid(OfrData, 1, 10) + Microsoft.VisualBasic.Constants.vbCrLf + AddString_o_ofr(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "CenaDoOFR");
            else
                OfrData = "";
            if (Zk1 == 0) //|| default(bool))
            {
                Zk1 = Zk1_OFR;
                OpisDta = OfrData;
            }
            else
                OpisDta = "ZK11! = " + ObliczCenaPoZK(CenaZpracelist, Zk1, true) + " zł";
            if (Zk2 == 0)
                Zk2 = Zk2_OFR;
            if (Zk3 == 0)
                Zk3 = Zk3_OFR;
            double CenaOFR1 = ObliczCenaPoZK(CenaZpracelist, Zk1, true);
            double CenaOFR2 = ObliczCenaPoZK(CenaZpracelist, Zk2, false);
            double CenaOFR3 = ObliczCenaPoZK(CenaZpracelist, Zk3, false);

            TblOfr init = new TblOfr();
            ListTblOfr.Tbl_Add_prodList.Add((
                init.SAP = obj.SAP,
                init.ID = ListTblOfr.Tbl_Add_prodList.Count,
                init.Naglowek = obj.Naglowek,
                init.Lpgrup = obj.Lpgrup,
                init.NazwProd = obj.NazwProd,
                init.GRUPA = obj.GRUPA,
                init.KATEGORIA = obj.KATEGORIA,
                init.NAZEWNICTWO = obj.NAZEWNICTWO,
                init.Kszt = obj.Kszt, init.Poj = obj.Poj,
                init.CDM = obj.CDM,
                init.KO = obj.CK,
                init.PH = obj.PH,
                init.ZPR0 = obj.ZPR0,
                init.BrakPrace = obj.BrakPrace,
                init.CenaZPrace = double.Parse(CenaZpracelist.ToString()),
                init.Cena_zapis_do_OFR = default,
                init.Opis_Cena_zapis_do_OFR = OpisDta,
                init.ZK11A1 = double.Parse(Zk1.ToString()),
                init.ZK11A2 = double.Parse(Zk2.ToString()),
                init.ZK11A3 = double.Parse(Zk3.ToString()),
                init.szt1 = AddString_o_ofr(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK1Info").ToString(),
                init.szt2 = AddString_o_ofr(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK2Info").ToString(),
                init.szt3 = AddString_o_ofr(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK3Info").ToString(),
                init.CenaDoOFR = double.Parse(CenaOFR1.ToString()),
                init.CenaDoOFR2 = double.Parse(CenaOFR2.ToString()),
                init.CenaDoOFR3 = double.Parse(CenaOFR3.ToString()),
                init.Img = obj.Img,
                init.TDS = obj.Tds, init.KCH = obj.KartaCHAR,
                init.Plik_Tds_True = obj.Tds_Ok_True, init.Plik_Kch_True = obj.Kchar_Ok_True,
                init.TDS_DO_OFR = false, init.CHAR_DO_OFR = false, init.PDF_DO_OFR = false,
                init.FileName = obj.NAZEWNICTWO, init.NazwaPdf = obj.NAZEWNICTWO,
                init.CenaBrutto = init.CenaDoOFR * 1.23, init).init);
            Console.WriteLine(" private string Add_prod_TblOfr(CennikData obj) = obj.NazwProd " + obj.NazwProd);
            List_Add_prodList.ItemsSource = ListTblOfr.Tbl_Add_prodList;
            List_Add_prodList.Items.Refresh();
            // MsgBox(Tbl_Add_prodList.Count)
            return obj.SAP;
        }


        private void TxT_PraceList_TextChanged(object sender, TextChangedEventArgs e)
        {
            // _Data(10) = ((TextBox)sender).Text;
            // Get_KlientDane.Rabat_Double = Zwroc_RAbat(_Data(10));
            foreach (var obj in ListTblOfr.Tbl_Add_prodList)
            {
                obj.CenaZPrace = CenaZ_praceList(obj.BrakPrace, obj.ZPR0, Get_KlientDane.Rabat_Double);
                {
                    var withBlock = obj;
                    double CenaZpracelist = CenaZ_praceList(obj.BrakPrace, obj.ZPR0, Get_KlientDane.Rabat_Double);
                    double CenaZapisanaWOFR = AddDecimal_o_ofr(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "CenaDoOFR");
                    // Dim SerchOfrAZk As Decimal = Nothing
                    double Zk1_OFR = AddDecimalTo_ZK(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "ZK1");
                    double Zk2_OFR = AddDecimalTo_ZK(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "ZK2");
                    double Zk3_OFR = AddDecimalTo_ZK(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "ZK3");
                    string OpisDta = null;
                    double Zk1 = AddDecimalTo_ZK(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK1");
                    double Zk2 = AddDecimalTo_ZK(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK2");
                    double Zk3 = AddDecimalTo_ZK(TabelaOdczytZK, "NrSAP", obj.SAP, "ZK3");
                    string OfrData = AddString_o_ofr(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "Data");
                    if (OfrData != "")
                        OfrData = "OFR! - " + Strings.Mid(OfrData, 1, 10) + Microsoft.VisualBasic.Constants.vbCrLf + AddString_o_ofr(TblZapisaneOferta_of_liczOfr, "SAP", obj.SAP, "CenaDoOFR");
                    else
                        OfrData = "";
                    if (Zk1 == 0 | default(Boolean))
                    {
                        Zk1 = Zk1_OFR; OpisDta = OfrData;
                    }
                    else
                        OpisDta = "ZK11! = " + ObliczCenaPoZK(CenaZpracelist, Zk1, true) + " zł";
                    if (Zk2 == 0)
                        Zk2 = Zk2_OFR;
                    if (Zk3 == 0)
                        Zk3 = Zk3_OFR;
                    double CenaOFR1 = ObliczCenaPoZK(CenaZpracelist, Zk1, true);
                    double CenaOFR2 = ObliczCenaPoZK(CenaZpracelist, Zk2, false);
                    double CenaOFR3 = ObliczCenaPoZK(CenaZpracelist, Zk3, false);
                    withBlock.CenaZPrace = (double)CenaZpracelist;
                    withBlock.CenaDoOFR = (double)CenaOFR1;
                    withBlock.CenaDoOFR2 = (double)CenaOFR2;
                    withBlock.CenaDoOFR3 = (double)CenaOFR3;
                }

                List_Add_prodList.ItemsSource = ListTblOfr.Tbl_Add_prodList;
                List_Add_prodList.Items.Refresh();
            }
        }


        public void Szukaj_ZK_Or_OFR(string Br_prace, double Zrp0, int Rabat, double CenaZPrace, double Cena_Z_baza, double Zk, string DataOfert, double CDM) // As String()
        {
            string[] cena = new string[4]; // = ""
            // Console.WriteLine("Br_prace {0}, Zrp0 {1}, Rabat {2}, CenaZPrace {3}, Cena_Z_baza {4}, Zk {5}, DataOfert {6}", Br_prace, Zrp0, Rabat, CenaZPrace, Cena_Z_baza, Zk, DataOfert)
            cena[0] = "0"; cena[1] = "0"; cena[2] = "0";
            if (Cena_Z_baza > 0)
            {
                cena[0] = Cena_Z_baza.ToString(); cena[1] = "OFR z d." + DataOfert; cena[2] = Cena_Z_baza.ToString();
            } // : Console.WriteLine("ofr cena1 {0} cena {1}  cena {2} ", cena(0), cena(1), cena(2))
            if (Zk > 0)
            {
                cena[0] = Sprawdz_cena_OFR_ZK11(Br_prace, Zrp0, Zk, Rabat).ToString(); cena[1] = "0"; cena[2] = cena[0];
            } // : Console.WriteLine("zk cena1 {0} cena {1}  cena {2} ", cena(0), cena(1), cena(2))
            // If cena(0) <> "" Then cena(0) = "0" : If CenaZPrace = cena(0) Then cena(0) = "0" : cena(1) = "0"
            if (cena[0] == Zrp0.ToString())
                cena[0] = "";
            cena[3] = Math.Round(((double.Parse(cena[2].ToString()) - CDM) / CDM) * 100, 2).ToString(); // & " %"
                                                                                                        // return cena;
        }

        private void czyscTxtOfr_MouseDown(object sender, MouseButtonEventArgs e) // Handles czyscTxtOfr.MouseDown
        {
            TxtAddProd.Text = null; TxtAddProd.Focus();
        }


        private void Usun_ZK11(object sender, MouseButtonEventArgs e)
        {
            ItmDelete = ((Label)sender).Tag.ToString();
            AddNewDock("Usunąć ZK11", "Czy na pewno chcesz Usunąć Zk11 ?");
        }

        public void AddNewDock(string Lab1, string Lab2)
        {
            DockPanel dpbaza = new DockPanel() { Name = "DocInfo", Background = new SolidColorBrush(Color.FromArgb(65, 204, 212, 230)), HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch }; //  ' light grey
            StackPanel dp = new StackPanel() { Width = 400, Height = 120, Background = new SolidColorBrush(Color.FromArgb(220, 211, 211, 211)), VerticalAlignment = VerticalAlignment.Center }; // light grey

            StackPanel st2 = new StackPanel() { HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            Label lb = new Label() { Content = Lab1, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center };
            Label lb2 = new Label() { Content = Lab2, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center };

            StackPanel st = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            Label LbAnuluj = new Label() { Content = "Anuluj", FontWeight = FontWeights.Bold, Width = 100, Height = 30, Margin = new Thickness(50, 25, 50, 0), Background = new SolidColorBrush(Color.FromRgb(128, 128, 128)), Opacity = 0.5, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center };  // grey
            Label LbZatwierdz = new Label() { Content = "Zatwierdz", FontWeight = FontWeights.Bold, Width = 100, Height = 30, Margin = new Thickness(50, 25, 50, 0), Background = new SolidColorBrush(Color.FromRgb(128, 128, 128)), Opacity = 0.5, HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center }; // red

            st.Children.Add(LbAnuluj);
            st.Children.Add(LbZatwierdz);
            st2.Children.Add(lb);
            st2.Children.Add(lb2);
            dp.Children.Add(st2);
            dp.Children.Add(st);
            // dp.Children.Add(st2)
            dpbaza.Children.Add(dp);
            LbAnuluj.MouseDown += Anuluj_Lab;
            LbZatwierdz.MouseDown += Zatwierdz_Lab;
            LbAnuluj.MouseEnter += Label_MouseEnter;
            LbZatwierdz.MouseEnter += Label_MouseEnter;
            LbAnuluj.MouseLeave += Label_MouseLeave;
            LbZatwierdz.MouseLeave += Label_MouseLeave;
            Mw.GridPan.Children.Add(dpbaza);
        }



        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Label)sender).Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            ((Label)sender).Opacity = 0.5;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Label)sender).Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            ((Label)sender).Opacity = 0.5;
        }
        private void Anuluj_Lab(object sender, MouseButtonEventArgs e)
        {
            // MsgBox("MyValue było " & MyValue)
            foreach (object ctr in Mw.GridPan.Children)
            {
                if (((DockPanel)ctr).Name == "DocInfo")
                {
                    Mw.GridPan.Children.Remove((UIElement)ctr); MyZatwierdz = "Anuluj"; break;
                }
            }
        }

        private void Zatwierdz_Lab(object sender, MouseButtonEventArgs e)
        {
            // MsgBox("MyValue było " & MyValue)
            foreach (object ctr in Mw.GridPan.Children)
            {
                if (((DockPanel)ctr).Name == "DocInfo")
                {
                    Mw.GridPan.Children.Remove((UIElement)ctr); MyZatwierdz = "Zatwierdz"; break;
                }
            }
            foreach (var ctr in ListZapisZK) // Tbl_Add_prodList
            {
                if (ItmDelete == ctr.NrSAP)
                {
                    UsingSQLComand(StringComand.returnComandUpdateZK11(ctr.NIP, ctr.NrSAP, "", "", "", "", "", ""), con);
                    ctr.ZK1 = null; ctr.ZK2 = null; ctr.ZK3 = null; ctr.ZK1Info = null; ctr.ZK2Info = null; ctr.ZK3Info = null;
                }
            }
            LiczOfr.WyswietlAllZK();
            serchListVisible();
        }


        private Point M_Point;
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Exit Sub
            M_Point = e.GetPosition(this);
            Point P = e.GetPosition(this);
            DataObject data = new DataObject(DataFormats.Serializable, (UserControl)e.Source);
            UserControl btn_to_move = (UserControl)e.Source;
            DragDrop.DoDragDrop((DependencyObject)e.Source, data, DragDropEffects.Move);
            double TR = 1;
            ScaleTransform LSTTransform;
            LSTTransform = new ScaleTransform(TR, TR);
            btn_to_move.LayoutTransform = LSTTransform;
            btn_to_move.Background = new SolidColorBrush(Colors.Transparent);
        }


        private void Button_DragEnter(object sender, DragEventArgs e)
        {
            UserControl btn_to_move = (UserControl)e.Data.GetData(DataFormats.Serializable);
            int where_to_move = FlowLayoutPanel1.Children.IndexOf((UIElement)e.Source);
            int what_to_move = FlowLayoutPanel1.Children.IndexOf(btn_to_move);
            Point p = e.GetPosition(this);
            // If p.X < 1000 And p.X > 1200 Then
            // wyssc.Text = "p-X:" & p.X
            // CTRAstualwyssc.Text = "p-Y:" & p.Y
            // GrupAstualwyssc.Text = M_Point.Y
            Point p11 = e.GetPosition(FlowLayoutPanel1);
            // Dim pointMouse = e.GetPosition(MouseMove)
            if (M_Point.X != p.X & M_Point.Y != p.Y)
            {
                FlowLayoutPanel1.Children.RemoveAt(what_to_move);
                FlowLayoutPanel1.Children.Insert(where_to_move, btn_to_move);
                try
                {
                    ListTblOfr.Tbl_Add_prodList = (List<TblOfr>)NewId_Tbl_prodList();
                }
                catch { }

                double TR = 0.8;
                ScaleTransform LSTTransform;
                LSTTransform = new ScaleTransform(TR, TR);
                btn_to_move.LayoutTransform = LSTTransform;
                bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Released;
                btn_to_move.Background = new SolidColorBrush(Color.FromRgb(255, 255, 204));
                MyScrollViewer.ScrollToVerticalOffset(p11.Y - 80);
                if (Mouse_position.Y < p.Y)
                    MyScrollViewer.ScrollToVerticalOffset(p.Y - 80);
            }
        }
        private Point Mouse_position;
        private void Page_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void Uzupelnij_controlProdukt(string rowLGrupa)
        {
            WstawProduktDoTablica_przepisz_z_list_do_Oferta();

            if (Get_KlientDane.Numer_konta.ToString().Substring(0, 1) == "9" || Get_KlientDane.Numer_konta == "")
                Mw.StZK.Visibility = Visibility.Collapsed;
            else
                Mw.StZK.Visibility = Visibility.Visible;
            int actH = Height_actual_control(rowLGrupa);
            // Console.WriteLine("actH - " & actH & " MyScrollViewer.ActualHeight  - " & MyScrollViewer.ActualHeight)
            if (actH > 1)
                MyScrollViewer.ScrollToVerticalOffset(actH);
            else
                MyScrollViewer.ScrollToEnd();

            Cennik.UnselectAll();
            HistoriaZakup.UnselectAll();

            ZapisaneOferty.UnselectAll();
            DataZK11.UnselectAll();
        }

        public object NewId_Tbl_prodList()
        {
            int NewId = 0;
            foreach (UserControl ctl in FlowLayoutPanel1.Children)
            {
                if ((ctl) is UserControl)
                {
                    GrupaProdukt SerchUC = (GrupaProdukt)ctl;
                    foreach (CtrProd Ctlt in SerchUC.FlowLayoutPanel2.Children)
                    {
                        foreach (TextBox ctrt in Ctlt.stName.Children)
                        {
                            if (ctrt.Name == "T_sap")
                            {
                                foreach (var itm in ListTblOfr.Tbl_Add_prodList)
                                {
                                    if (itm.SAP == ctrt.Text)
                                        itm.ID = NewId;
                                }
                                NewId += 1;
                            }
                        }
                    }
                }
            }
            var result1 = ListTblOfr.Tbl_Add_prodList.OrderBy(a => a.ID).ToList();
            // Dim colNames = dtg.SelectMany(Function(rg) rg).Distinct().OrderBy(Function(n) n >= n)
            return result1;  // Tbl_Add_prodList.OrderBy(Function(o) o.ID <= o.ID)
        }







        private void Cennik_KeyDown(object sender, KeyEventArgs e) // Handles Cennik.KeyDown
        {
            // MsgBox("Cennik_KeyDown")
            if (e.Key == Key.Enter)
                Add_Prod();
        }
        private void Add_Prod_Do_Lista(object sender, MouseButtonEventArgs e)
        {
            Add_Prod();
        }
        private void Add_Prod()
        {

            string rowLGrupa = default;
            if (HistoriaZKP.Visibility == Visibility.Visible)
            {
                if (Information.IsNumeric(SapPr.Text.ToString()))
                {
                    foreach (var itm in Mw.ListCennik)
                    {
                        if (itm.SAP == SapPr.Text.ToString())
                        {
                            // Console.WriteLine(" HistoriaZKP " + SapPr.Text + "  " + itm.CK);
                            AddIndex(itm); rowLGrupa = itm.SAP;
                        }
                    }
                }
                HistoriaZakupViev.UnselectAll();
                HistoriaZakupViev.Visibility = Visibility.Collapsed;
            }
            if (AnalizaZakup.Visibility == Visibility.Visible)
            {
                if (Information.IsNumeric(AnalizaNazwaPR.Text))
                {
                    foreach (var itm in Mw.ListCennik)
                    {
                        if (itm.SAP == AnalizaNazwaPR.Text)
                        {
                            //Console.WriteLine("AnalizaZakup " + AnalizaNazwaPR.Text);
                            AddIndex(itm); rowLGrupa = itm.SAP;
                        }
                    }
                }
            }
            foreach (var row in Tbl_selectedIndex)
            {
                bool AddItm = true;
                foreach (var itm in ListTblOfr.Tbl_Add_prodList)
                {
                    if (itm.SAP == row.SAP)
                    {
                        //Console.WriteLine("2 foreach Add_Prod {0}", itm.SAP);
                        AddItm = false; rowLGrupa = row.SAP;
                    }
                }
                if (AddItm == true)
                    rowLGrupa = Add_prod_TblOfr(row); // : rowFocus = row
            }
            Uzupelnij_controlProdukt(rowLGrupa);
        }
        public void Delete_row_LiczOfr_Tbl_selectedIndex(string ColName, string ControlName)
        {
            //   line1:
            ;

            // Console.WriteLine("Delete Tbl_Add_prodListcount " & ControlName & " - - - " & ColName)
            if (ControlName == "CtrProd")
            {
            Lin_Z_Rob1:
                ;
                foreach (DataRow row in TblZapisRobocze.Rows)
                {
                    if (ColName == row["SAP"].ToString())
                    {
                        TblZapisRobocze.Rows.Remove((DataRow)row); goto Lin_Z_Rob1;
                    }
                }

            Line_Z_Sel1:
                ;
                foreach (var item in Tbl_selectedIndex)
                {
                    if (ColName == item.SAP.ToString())
                    {
                        Tbl_selectedIndex.Remove(item); goto Line_Z_Sel1;
                    }
                }
                string sqldel = "delete From BazaOfr_robocze WHERE NIP like '%" + Get_KlientDane.NIP + "%' and SAP like '%" + ColName + "%';";
                UsingSQLComand(sqldel, con);
            line_addLin:
                ;
                foreach (var itm in ListTblOfr.Tbl_Add_prodList)
                {
                    if (itm.SAP == ColName)
                    {
                        ListTblOfr.Tbl_Add_prodList.Remove(itm); goto line_addLin;
                    }
                }
            }
            if (ControlName == "GrupaProdukt")
            {
            Lin_Z_Rob2:
                ;
                foreach (DataRow row in TblZapisRobocze.Rows)
                {
                    if (ColName == row["Lpgrup"].ToString())
                    {
                        TblZapisRobocze.Rows.Remove(row); goto Lin_Z_Rob2;
                    }
                }

            Line_Z_Sel2:
                ;
                foreach (var item in Tbl_selectedIndex)
                {
                    if (ColName == item.Lpgrup.ToString())
                    {
                        Tbl_selectedIndex.Remove(item); goto Line_Z_Sel2;
                    }
                }
                string sqldel = "delete From BazaOfr_robocze WHERE NIP like '%" + Get_KlientDane.NIP + "%' and Lpgrup like '%" + ColName + "%';";
                UsingSQLComand(sqldel, con);
            }
            if (ControlName == "Czysc")
            {
                Tbl_selectedIndex.Clear();
                TblZapisRobocze.Clear();
                string sqldel = "delete From BazaOfr_robocze WHERE NIP like '%" + Get_KlientDane.NIP + "%';";
                UsingSQLComand(sqldel, con);
            }

            List_Add_prodList.ItemsSource = ListTblOfr.Tbl_Add_prodList;
            List_Add_prodList.Items.Refresh();
            RoboczeVisible();
            //return null;
        }

        private void GetCtrBack(string activCtr)
        {
            foreach (UserControl ctl in FlowLayoutPanel1.Children)
            {
                GrupaProdukt SerchUC = (GrupaProdukt)ctl;
                foreach (CtrProd Ctlt in SerchUC.FlowLayoutPanel2.Children)
                {
                    // If Ctlt.Tag = activCtr Then
                    SolidColorBrush actcol = new SolidColorBrush(Colors.LightGray);
                    // actcol = Ctlt.Background
                    for (int j = 0; j <= 3; j++)
                    {
                        Ctlt.Background = new SolidColorBrush(Colors.Red);
                        for (int i = 0; i <= 1000000000; i++)
                            i += 1;
                        Ctlt.Background = actcol; // New SolidColorBrush(Colors.Red)
                    }
                }
            }
        }
        private int Height_actual_control(string rowLGrupa)
        {
            int Hei = 1;
            int Heictr = 0;
            int Coulist = 0;
            foreach (UserControl ctl in FlowLayoutPanel1.Children)
            {
                GrupaProdukt SerchUC = (GrupaProdukt)ctl;
                Hei += 50; Heictr += (int)ctl.ActualHeight;   // : Grupwyssc.Text = Heictr & " / " & ctl.Tag & " / " & rowLGrupa
                foreach (CtrProd Ctlt in SerchUC.FlowLayoutPanel2.Children)
                {
                    if (Ctlt.Tag.ToString() == rowLGrupa)
                        goto lastline; // Return Hei : Exit Function
                    Coulist += 1; Hei += (int)Ctlt.ActualHeight;  // :   CTRAstualwyssc.Text = Hei & " / " & Ctlt.Tag
                }
            }

        lastline:
            ;
            if (Coulist == 0)
                Hei = 2;
            if (Coulist > 0)
                Hei = Hei - 60;
            if (Coulist == ListTblOfr.Tbl_Add_prodList.Count - 1)
                Hei = 0;
            // CTRAstualwyssc.Text = Hei & " / " & Coulist & " / " & Tbl_Add_prodList.Count - 1
            return Hei; // - 60 ' Hei / Tbl_Add_prodList.Count
        }
        public void WstawProduktDoTablica_przepisz_z_list_do_Oferta()
        {
            if (Grupnr == default)
                Grupnr = 0;
            else
                Grupnr += 1;
            // Console.WriteLine("WstawProduktDoTablica_przepisz_z_list_do_Oferta")

            int j = 0;
            foreach (var ctr in ListTblOfr.Tbl_Add_prodList)
            {
                if ((ctr.ZPR0 >= 0) || (ctr.KO >= 0) || (ctr.PH >= 0))
                {
                    string NrTag = Conversions.ToString(ctr.ID);

                    foreach (UserControl ctl in FlowLayoutPanel1.Children)
                    {
                        if (ctl is UserControl)
                        {
                            if (((UserControl)ctl).Tag.ToString() == ctr.Lpgrup)
                            {
                                GrupaProdukt SerchUC = (GrupaProdukt)ctl;
                                foreach (CtrProd Ctlt in SerchUC.FlowLayoutPanel2.Children)
                                {
                                    if (Ctlt.Tag.ToString() == ctr.SAP)
                                        goto LastLine;
                                }
                                var Uct = new CtrProd(ctr) { Tag = ctr.SAP };
                                SerchUC.FlowLayoutPanel2.Children.Add(Uct);
                                SerchUC.Focus();
                                goto LastLine;
                            }
                            else
                            {
                                GrupaProdukt SerchUC = (GrupaProdukt)ctl;
                                foreach (CtrProd Ctlt in SerchUC.FlowLayoutPanel2.Children)
                                {
                                    if (Operators.ConditionalCompareObjectEqual(Ctlt.Tag, ctr.SAP, false))
                                        goto LastLine; // NrTag Then GoTo line2
                                }
                            }
                        }
                    }
                    var Ucontrol = new GrupaProdukt(ctr) { Tag = ctr.Lpgrup, AllowDrop = true };
                    Ucontrol.L_Naglowek.Content = ctr.Naglowek; // TxtLab
                    Ucontrol.TRabatReczny.Tag = ctr.Lpgrup;
                    Ucontrol.pmin.Tag = ctr.Lpgrup;
                    Ucontrol.pplus.Tag = ctr.Lpgrup;
                    Ucontrol.LKO.Tag = ctr.Lpgrup;
                    if (Upr_User.CenaKO == false)
                        Ucontrol.LKO.Visibility = Visibility.Collapsed;
                    else
                        Ucontrol.LKO.Visibility = Visibility.Visible;
                    Ucontrol.CDM.Tag = ctr.Lpgrup;
                    Ucontrol.PH.Tag = ctr.Lpgrup;
                    Ucontrol.ZRP0.Tag = ctr.Lpgrup;
                    Ucontrol.FlowLayoutPanel2.Tag = ctr.Lpgrup;
                    Ucontrol.Clear.Tag = ctr.Lpgrup;
                    var Uctl = new CtrProd(ctr) { Tag = ctr.SAP };
                    Ucontrol.FlowLayoutPanel2.Children.Add(Uctl);
                    FlowLayoutPanel1.Children.Add(Ucontrol);
                    _Index += 1;
                    Ucontrol.Width = FlowLayoutPanel1.Width - 130;
                    Ucontrol.Focus();
                LastLine:
                    ;

                    j += 1;
                }
            }
        }


        private void Add_prodList_CreateDynamicGridView()
        {
            try
            {
                // If TblSort Is Nothing Then Exit Sub

                GridView grdView = new GridView();
                GridView grdView1 = new GridView();
                DataTable Dg = new DataTable();
                // Dg = TryCast(Add_prodList.ItemsPanel, DataTable)
                foreach (DataColumn col in Dg.Columns)
                {
                    GridViewColumn bookColumn = new GridViewColumn() { DisplayMemberBinding = new Binding(col.ColumnName), Header = col.ColumnName };
                    grdView.Columns.Add(bookColumn);
                }
                List_Add_prodList.View = grdView;
                Binding bind1 = new Binding() { Source = List_Add_prodList.ItemsSource };
                List_Add_prodList.SetBinding(ListView.ItemsSourceProperty, bind1);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }


    }
}
