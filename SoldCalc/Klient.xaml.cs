using Microsoft.VisualBasic;
using SoldCalc.Controls;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc
{
    public partial class Klient : Page
    {



        public static Klient KlientBaza;// = new Klient();
        public static ListView ttGrid = new ListView();


        public Klient()
        {

            InitializeComponent();
            KlientBaza = this;
            this.DataContext = KlientBaza;
            TTTDataGridView1.DataContext = Mw.ListKlient;
            ttGrid = TTTDataGridView1;
            if (Upr_User.UprKO == false)
            {
                CombPH.Visibility = Visibility.Collapsed; CombKO.Visibility = Visibility.Collapsed;
            }
            string Sqwery2 = "SELECT distinct KO, Branza FROM DaneKO WHERE Branza <>'' ORDER BY Branza ASC ";
            string serch = "Branza";

            CombGrData(Sqwery2, CombBR, serch);
            Sqwery2 = " Select distinct KO FROM DaneKO WHERE Branza <>'' group by KO ";

            CombGrData(Sqwery2, CombKO, "KO");
            Sqwery2 = "SELECT distinct Opiekun_klienta FROM BazaKL WHERE Opiekun_klienta not like '' group by Opiekun_klienta;";

            CombGrData(Sqwery2, CombPH, "Opiekun_klienta");
            if (Mw.startKlient == true)
            {
                TTTDataGridView1.ItemsSource = BazaKlient.DefaultView;
            }
            SercgData("");
            BlokujAktual = true;
            //if (BazaKlient.Rows.Count < 1)
            //{
            //    Grid1.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    Grid1.Visibility = Visibility.Visible;
            //}

        }

        private bool ClientFilter(object item)
        {
            if (string.IsNullOrEmpty(TxtAdd.Text))
                return true;
            else
                return (item as DaneKlient).Nazwa_klienta.IndexOf(TxtAdd.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void Grid1_Loaded(object sender, RoutedEventArgs e)
        {
            Panel1.LayoutTransform = Upr_User.dpiTransform;
            DokListKl.LayoutTransform = Upr_User.dpiTransform;
            TtGridSize();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Mw.AddKlient.Visibility = Visibility.Visible;
            Mw.PaneLnaw.Visibility = Visibility.Hidden;
            Mw.RoboczeSql.Visibility = Visibility.Hidden;
            TxtAdd.Focus();
            if (Mw.startKlient == true)
            {
                TTTDataGridView1.ItemsSource = BazaKlient.DefaultView;
            }
            SercgData(TxtAdd.Text);
        }

        public void CombGrData(string qwery, ComboBox Ctr, string Serch)
        {
            string ErrCombo = "";
            SQLiteDataAdapter da = new SQLiteDataAdapter(qwery, con);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);
            foreach (DataRow d in dt.Rows)
            {
                try
                {
                    if (d[Serch].ToString() == "00")
                        dt.Rows.Remove(d);
                }
                catch
                {
                    ErrCombo += Serch + " - " + d[Serch] + " - " + qwery;
                }
            }
            Ctr.ItemsSource = dt.DefaultView;
            Ctr.DisplayMemberPath = Serch;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtAdd.Text != "")
            {
                SzukajTxt.Content = ""; czyscTxt.Visibility = Visibility.Visible;
            }
            else
            {
                SzukajTxt.Content = "Szukaj Klienta"; czyscTxt.Visibility = Visibility.Collapsed;
            }

            if (CombBR.Text != "")
                CombBR.Text = "";
            SercgData(TxtAdd.Text);
            // CollectionViewSource.GetDefaultView(TTTDataGridView1.ItemsSource).Refresh();
        }

        private void CombBR_Selectiontext(object sender, TextChangedEventArgs e)
        {
            string T1 = Strings.Mid(CombBR.Text, 1, 2);
            BazaKlient.DefaultView.RowFilter = string.Format("Branza LIKE '%{0}%'", T1);
            if (CombBR.Text != "")
            {
                LabKO.Content = CombBR.SelectedItem;
                DataRowView yourstringname = (DataRowView)CombBR.Items.GetItemAt(CombBR.SelectedIndex);
                LabKO.Content = yourstringname[0];
            }
            else
                LabKO.Content = "";
            IleWrs.Content = TTTDataGridView1.Items.Count + " " + "klientów";
        }

        private void CombKO_Selectiontext(object sender, TextChangedEventArgs e)
        {
            string a = "";

            string T1 = ""; string T2 = ""; string T3 = ""; string T4 = "";
            var splitKO = CombKO.Text.Split(' ');
            var splitPH = CombPH.Text.Split(' ');
            if (splitKO.Count() == splitKO.Count())
            {
                if (splitKO[0].Length > 0)
                    T1 = splitKO[0].ToString();
                else
                    T1 = null;
                try
                {
                    if (splitKO[1].Length > 0)
                        T2 = splitKO[1].ToString();
                    else
                        T2 = null;
                }
                catch
                {
                    a += a + 1;
                }
            }
            if (splitPH.Count() == splitPH.Count())
            {
                if (splitPH[0].Length > 0)
                    T3 = splitPH[0].ToString();
                else
                    T3 = null;
                try
                {
                    if (splitPH[1].Length > 0)
                        T4 = splitPH[1].ToString();
                    else
                        T4 = null;
                }
                catch
                {
                    a += a + 2;
                }
            }
            else
            {
            }
            BazaKlient.DefaultView.RowFilter = string.Format(@"KO LIKE '%{0}%' AND KO LIKE '%{1}%' 
                                                            AND Opiekun_klienta LIKE '%{2}%' AND Opiekun_klienta LIKE '%{3}%'
                                                        ", T1, T2, T3, T4);
            try
            {
                if (CombKO.Text == "")
                    BazaKlient.DefaultView.RowFilter = string.Format("Opiekun_klienta LIKE '%{0}%' AND Opiekun_klienta LIKE '%{1}%'", T3, T4);
            }
            catch
            {
                a += a + 3;
            }
            try
            {
                if (CombPH.Text == "")
                    BazaKlient.DefaultView.RowFilter = string.Format("KO LIKE '%{0}%' AND KO LIKE '%{1}%' ", T1, T2);
            }
            catch
            {
                a += a + 4;
            }
            IleWrs.Content = TTTDataGridView1.Items.Count + " " + "klientów";
        }

        public bool SercgData(string valueToSearch)
        {
            if (BazaKlient == null)
                return false;
            if (valueToSearch == "")
            {
                BazaKlient.DefaultView.RowFilter = string.Format("Nazwa_klienta LIKE '%{0}%'", " ");
                return false;
            }
            string T1 = "";
            string T2 = "";
            string T3 = "";
            string T4 = "";
            string T5 = "";
            string T6 = "";
            var splittext = valueToSearch.Split(' ');
            int manyCharacters = Strings.Replace(valueToSearch, " ", "").Length;
            try
            {
                if (splittext[0] != null)
                {
                    if (splittext[0].Length > 0)
                        T1 = splittext[0].ToString();
                    else
                        T1 = "";
                }
                if (splittext[1] != null)
                {
                    if (splittext[1].Length > 0)
                        T2 = splittext[1].ToString();
                    else
                        T2 = "";
                }
                if (splittext[2] != null)
                {
                    if (splittext[2].Length > 0)
                        T3 = splittext[2].ToString();
                    else
                        T3 = "";
                }
                if (splittext[3] != null)
                {
                    if (splittext[3].Length > 0)
                        T4 = splittext[3].ToString();
                    else
                        T4 = "";
                }
                if (splittext[4] != null)
                {
                    if (splittext[4].Length > 0)
                        T5 = splittext[4].ToString();
                    else
                        T5 = "";
                }
                if (splittext[5] != null)
                {
                    if (splittext[5].Length > 0)
                        T6 = splittext[5].ToString();
                    else
                        T6 = "";
                }
            }
            catch
            {
            }

            BazaKlient.DefaultView.RowFilter = string.Format(@"Nazwa_klienta LIKE '%{0}%' and Nazwa_klienta Like '%{1}%' and Nazwa_klienta Like '%{2}%' and Nazwa_klienta Like '%{3}%' and Nazwa_klienta Like '%{4}%' and Nazwa_klienta Like '%{5}%'                                                                                                                 
                                                                  OR NIP LIKE '%{0}%' and NIP Like '%{1}%' and NIP Like '%{2}%' and NIP Like '%{3}%' and NIP Like '%{4}%' and NIP Like '%{5}%'
                                                                  OR Numer_konta LIKE '%{0}%' and Numer_konta Like '%{1}%' and Numer_konta Like '%{2}%' and Numer_konta Like '%{3}%' and Numer_konta Like '%{4}%' and Numer_konta Like '%{5}%'
                                                                  OR Kod_Poczta LIKE '%{0}%' and Kod_Poczta Like '%{1}%' and Kod_Poczta Like '%{2}%' and Kod_Poczta Like '%{3}%' and Kod_Poczta Like '%{4}%' and NIP Like '%{5}%'
                                                                  OR Poczta LIKE '%{0}%' and Poczta Like '%{1}%' and Poczta Like '%{2}%' and Poczta Like '%{3}%' and Poczta Like '%{4}%' and Poczta Like '%{5}%'
                                                                  OR Adres LIKE '%{0}%' and Adres Like '%{1}%' and Adres Like '%{2}%' and Adres Like '%{3}%' and Adres Like '%{4}%' and Adres Like '%{5}%'
                                                                  ", T1, T2, T3, T4, T5, T6);
            // Console.WriteLine(4)
            int RowCoun = TTTDataGridView1.Items.Count;
            IleWrs.Content = TTTDataGridView1.Items.Count + " " + "klientów";
            if (RowCoun == BazaKlient.Rows.Count)
                TTTDataGridView1.ItemsSource = BazaKlient.DefaultView;
            return true;
        }

        private void TxtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(TTTDataGridView1.ItemsSource).Refresh();
        }


        private void TTTDataGridView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView CompRow;
            int SComp;
            SComp = TTTDataGridView1.SelectedIndex;
            if (SComp >= 0)
            {
                CompRow = TTTDataGridView1.Items.GetItemAt(SComp) as DataRowView;
                Get_KlientDane.Id = int.Parse(CompRow["Id"].ToString());
                Get_KlientDane.Opiekun_klienta = Convert.ToString(CompRow["Opiekun_klienta"].ToString());
                Get_KlientDane.NIP = Convert.ToString(CompRow["NIP"].ToString());
                Get_KlientDane.Stan = Convert.ToString(CompRow["Stan"].ToString());
                Get_KlientDane.Numer_konta = Convert.ToString(CompRow["Numer_konta"].ToString());
                Get_KlientDane.Nazwa_klienta = Convert.ToString(CompRow["Nazwa_klienta"].ToString());
                Get_KlientDane.Adres = Convert.ToString(CompRow["Adres"].ToString());
                Get_KlientDane.Kod_Poczta = Convert.ToString(CompRow["Kod_Poczta"].ToString());
                Get_KlientDane.Poczta = Convert.ToString(CompRow["Poczta"].ToString());
                Get_KlientDane.Forma_plac = Convert.ToString(CompRow["Forma_plac"].ToString());
                Get_KlientDane.PraceList = Convert.ToString(CompRow["PraceList"].ToString());
                Get_KlientDane.Branza = Convert.ToString(CompRow["Branza"].ToString());
                Get_KlientDane.Tel = Convert.ToString(CompRow["Tel"].ToString());
                Get_KlientDane.E_mail = Convert.ToString(CompRow["E_mail"].ToString());
                Get_KlientDane.Branzysta = Convert.ToString(CompRow["KO"].ToString());
                try
                {
                    Get_KlientDane.BranzystaEmail = Convert.ToString(Strings.Replace(CompRow["BrEma"].ToString(), " ", ""));
                }
                catch
                {
                }
                Get_KlientDane.Rabat_Double = Zwroc_RAbat(Get_KlientDane.PraceList.ToString());
                LiczOferta expenseReportPage = new LiczOferta(Get_KlientDane);
                Cennik_Add = expenseReportPage;
                this.NavigationService.Navigate(expenseReportPage);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TtGridSize();
        }
        private void TtGridSize()
        {
            if (Grid1.Visibility == Visibility.Visible)
            {
                double remainingSpace = TTTDataGridView1.ActualWidth;
                GridView colH = TTTDataGridView1.View as GridView;
                double i = 0;
                for (int c = 0, loopTo = colH.Columns.Count - 1; c <= loopTo; c++)
                {
                    if (c != 1 & c != 2 & c != 4 & c != 6)
                        i += colH.Columns[c].ActualWidth;
                }
            (TTTDataGridView1.View as GridView).Columns[1].Width = Math.Ceiling((remainingSpace - i) / 4.2);
                (TTTDataGridView1.View as GridView).Columns[2].Width = Math.Ceiling((remainingSpace - i) / 4.2);
                (TTTDataGridView1.View as GridView).Columns[4].Width = Math.Ceiling((remainingSpace - i) / 4.1);
                (TTTDataGridView1.View as GridView).Columns[6].Width = Math.Ceiling((remainingSpace - i) / 4.1);
            }
        }

        private void TTTDataGridView1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TtGridSize();
        }

        private void HandleColumnHeaderSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (sizeChangedEventArgs.NewSize.Width <= 60)
            {
                sizeChangedEventArgs.Handled = true;
                ((GridViewColumnHeader)sender).Column.Width = 60;
            }
        }

        public void AddClientToListMw(int id, string op, string nip, string stan, string nrkont, string nazkl, string adr, string kodp, string pocz, string formplac, string pracel,
            string bran, string tel, string ema, string branzyst, string brEma, string tol, string rbt)
        {
            Mw.ListKlient.Add(new DaneKlient()
            {
                Id = int.Parse(id.ToString()),
                Opiekun_klienta = op,
                NIP = nip,
                Stan = stan,
                Numer_konta = nrkont,
                Nazwa_klienta = nazkl,
                Adres = adr,
                Kod_Poczta = kodp,
                Poczta = pocz,
                Forma_plac = formplac,
                PraceList = pracel,
                Branza = bran,
                Tel = tel,
                E_mail = ema,
                Branzysta = branzyst,
                BranzystaEmail = brEma,
                TollTipInfo = tol,
                Rabat_Double = Zwroc_RAbat(rbt.ToString())
            });
        }

        public void ClearClientItemToListMw(string nip)
        {
            foreach (var nrNip in Mw.ListKlient)
            {
                if (nrNip.NIP == nip)
                {
                    Mw.ListKlient.Remove(nrNip);
                }
                //Console.WriteLine("Amount is {0} and type is {1}", money.amount, money.type);
            }
        }



        public void AktualBza()
        {
            string cT2 = EdytujZmienDane.ZmianaActiv.T2.Text;
            string cT9 = EdytujZmienDane.ZmianaActiv.T9.Text;
            string cT10 = EdytujZmienDane.ZmianaActiv.T10.Text;
            string cT11 = EdytujZmienDane.ZmianaActiv.T11.Text;
            foreach (var row in Mw.ListKlient)
            {
                if (EdytujZmienDane.ZmianaActiv.T1.Text == row.NIP)
                {
                    Get_KlientDane.Id = row.Id;
                    Get_KlientDane.Opiekun_klienta = row.Opiekun_klienta;
                    Get_KlientDane.NIP = row.NIP;
                    if (Cennik_Add != null)
                        Cennik_Add.Txt_Nip.Text = EdytujZmienDane.ZmianaActiv.T1.Text;
                    row.Stan = cT2; Get_KlientDane.Stan = cT2;
                    Get_KlientDane.Stan = EdytujZmienDane.ZmianaActiv.T1.Text;
                    row.Numer_konta = EdytujZmienDane.ZmianaActiv.T3.Text;
                    Get_KlientDane.Numer_konta = row.Numer_konta;
                    if (Cennik_Add != null)
                        Cennik_Add.T3.Text = EdytujZmienDane.ZmianaActiv.T3.Text;
                    row.Nazwa_klienta = EdytujZmienDane.ZmianaActiv.T4.Text;
                    Get_KlientDane.Nazwa_klienta = EdytujZmienDane.ZmianaActiv.T4.Text;
                    if (Cennik_Add != null)
                        Cennik_Add.KlientNazwa.Text = EdytujZmienDane.ZmianaActiv.T4.Text;
                    row.Adres = EdytujZmienDane.ZmianaActiv.T6.Text;
                    Get_KlientDane.Adres = EdytujZmienDane.ZmianaActiv.T6.Text;
                    row.Kod_Poczta = EdytujZmienDane.ZmianaActiv.T7.Text;
                    Get_KlientDane.Kod_Poczta = EdytujZmienDane.ZmianaActiv.T7.Text;
                    row.Poczta = EdytujZmienDane.ZmianaActiv.T8.Text;
                    Get_KlientDane.Poczta = EdytujZmienDane.ZmianaActiv.T8.Text;
                    row.Forma_plac = cT9; Get_KlientDane.Forma_plac = cT9;
                    if (Cennik_Add != null)
                        Cennik_Add.T9.Text = cT9;
                    row.PraceList = cT10; Get_KlientDane.PraceList = cT10;
                    if (Cennik_Add != null)
                        Cennik_Add.T10.Text = cT10;
                    row.Branza = cT11; Get_KlientDane.Branza = cT11;
                    if (Cennik_Add != null)
                        Cennik_Add.T11.Text = cT11;
                    row.Tel = EdytujZmienDane.ZmianaActiv.T12.Text;
                    Get_KlientDane.Tel = EdytujZmienDane.ZmianaActiv.T12.Text;
                    row.E_mail = EdytujZmienDane.ZmianaActiv.T13.Text;
                    Get_KlientDane.E_mail = EdytujZmienDane.ZmianaActiv.T13.Text;
                    if (Cennik_Add != null)
                        Cennik_Add.T13.Text = EdytujZmienDane.ZmianaActiv.T13.Text;
                    Get_KlientDane.Branzysta = row.Branzysta;
                    Get_KlientDane.BranzystaEmail = row.BranzystaEmail;
                    Get_KlientDane.Rabat_Double = Zwroc_RAbat(Get_KlientDane.PraceList);
                }
            }
            if (Get_KlientDane.Numer_konta.Length > 0 && Get_KlientDane.Numer_konta != null)
            {
                Mw.StZK.Visibility = ((Get_KlientDane.Numer_konta.Substring(1, 1) == "9") || (Get_KlientDane.Numer_konta == "")) ? Visibility.Visible : Visibility.Collapsed;
            }
            TTTDataGridView1.ItemsSource = BazaKlient.DefaultView;
        }


        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtAdd.Text = null; TxtAdd.Focus();
        }





    }
}

