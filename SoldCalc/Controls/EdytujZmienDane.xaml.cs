using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SoldCalc.Supporting;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using DataTable = System.Data.DataTable;
using TextBox = System.Windows.Controls.TextBox;

namespace SoldCalc.Controls
{
    public partial class EdytujZmienDane
    {
        private DataTable TableKoBranz = new DataTable();
        public static EdytujZmienDane ZmianaActiv;
        public EdytujZmienDane(bool data) : this()
        {
            if (data == false)
            {
                // KlientDane.Id = Nothing : KlientDane.Opiekun_klienta = Nothing : KlientDane.NIP = Nothing : KlientDane.Stan = Nothing : KlientDane.Numer_konta = Nothing : KlientDane.Nazwa_klienta = Nothing : KlientDane.Adres = Nothing : KlientDane.Kod_Poczta = Nothing : KlientDane.Poczta = Nothing : KlientDane.Forma_plac = Nothing : KlientDane.PraceList = Nothing : KlientDane.Branza = Nothing : KlientDane.Tel = Nothing : KlientDane.E_mail = Nothing : KlientDane.Branzysta = Nothing : KlientDane.BranzystaEmail = Nothing : KlientDane.Rabat_Double = Nothing
                Mw.EditKlientDane.Id = default;
                Mw.EditKlientDane.Opiekun_klienta = null;
                Mw.EditKlientDane.NIP = null;
                Mw.EditKlientDane.Stan = null;
                Mw.EditKlientDane.Numer_konta = null;
                Mw.EditKlientDane.Nazwa_klienta = null;
                Mw.EditKlientDane.Adres = null;
                Mw.EditKlientDane.Kod_Poczta = null;
                Mw.EditKlientDane.Poczta = null;
                Mw.EditKlientDane.Forma_plac = null;
                Mw.EditKlientDane.PraceList = null;
                Mw.EditKlientDane.Branza = null;
                Mw.EditKlientDane.Tel = null;
                Mw.EditKlientDane.E_mail = null;
                Mw.EditKlientDane.Branzysta = null;
                Mw.EditKlientDane.BranzystaEmail = null;
                Mw.EditKlientDane.Rabat_Double = default;

            }
            // Console.WriteLine(data)
            this.DataContext = Mw.EditKlientDane;
            Remowe.DataContext = Upr_User;

            // Console.WriteLine(data(0))
        }
        public EdytujZmienDane()
        {
            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ZmianaActiv = this;
            string Sqwery2 = "SELECT distinct KO, Branza FROM DaneKO WHERE Branza <>''";
            string serch = "Branza";
            CombGrData(Sqwery2, T11, serch);
            AktivDaneKL();
            FormPlacAdd();
            if (Get_KlientDane is null)
                Get_KlientDane = new DaneKlient();
        }


        public void CombGrData(string qwery, ComboBox Ctr, string Serch)
        {
            var da = new SQLiteDataAdapter(qwery, con);
            var dt = new DataTable();
            TableKoBranz = dt;
            da.Fill(TableKoBranz);
            foreach (DataRow d in dt.Rows)
            {
                if (d.ToString() == "00")
                    dt.Rows.Remove(d);
            }
            Ctr.ItemsSource = dt.DefaultView;
            Ctr.DisplayMemberPath = Serch;
        }    // wstaw wybrane ComboProd1

        private void AktivDaneKL()
        {
            T2.Items.Add("Aktywny");
            T2.Items.Add("Nieaktywne");
            T2.Items.Add("Potencjalny");
        }


        private void FormPlacAdd()
        {
            T10.Items.Clear();
            var WczytajCB = new StreamReader("TxtPraceList1.txt");

            while (WczytajCB.EndOfStream != true)
            {
                string Wind = WczytajCB.ReadLine();
                T10.Items.Add(Wind);
            }
            WczytajCB.Close();
            T9.Items.Clear();
            var WczytajFP = new StreamReader("TxtFormPlac.txt");
            while (WczytajFP.EndOfStream != true)
            {
                T9.Items.Add(WczytajFP.ReadLine());
            }
            WczytajFP.Close();
        }



        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex regex;
            if ((((TextBox)sender).Name == "T7") || (((TextBox)sender).Text == ""))
            {
                regex = new System.Text.RegularExpressions.Regex("[^0-9]-+");
            }
            else
            {
                regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            }

            e.Handled = regex.IsMatch(e.Text);
        }

        private void UClear_Click(object sender, MouseButtonEventArgs e)
        {
            ClearMe();
        }
        private void ClearMe()
        {
            RemoveMe();
            Mw.VievPageVisibli(false, false, "");
        }


        private void RemoveMe()
        {
            if (this.Parent != null)
            {
                DockPanel Par = (DockPanel)this.Parent;
                Par.Visibility = Visibility.Collapsed;
                Par.Children.Remove(this);
            }

        }
        public void WyswietlDoEdycja()
        {
            try
            {
                var textBoxes = Panel1.Children.OfType<TextBox>();
                var textBoxes3 = Panel1.Children.OfType<TextBox>();
                var CombBoxes = Panel1.Children.OfType<ComboBox>();
                var CombBoxes1 = Panel1.Children.OfType<ComboBox>();
                foreach (var txt in textBoxes3)
                {
                    txt.IsReadOnly = false;
                    txt.Background = new SolidColorBrush(Colors.WhiteSmoke);
                }
                foreach (var Comb in CombBoxes)
                {
                    Comb.IsEditable = true;
                    Comb.IsReadOnly = true;
                    Comb.Background = new SolidColorBrush(Colors.White);
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public void Aktualizuj_DaneKL()
        {
            try
            {
                tButton3.Content = "Zmień dane"; // Then AktualDane() : tButton1.Text = "Aktualizuj dane"
                tButton3.Visibility = Visibility.Visible; // = True
                WyswietlDoEdycja();
                string str = T3.Text;
                string[] strArr = str.Split(' ');
                if (Strings.Mid(str, 1, 1) == "1")
                {
                    // T1.ReadOnly = True
                    // T3.ReadOnly = True
                    // T11.Enabled = False
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            // If Mid(str, 1, 1) = "9" Then T11.Enabled = True
        }
        private void T1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string SerchNip = Strings.Replace(T1.Text, "-", "");
                if (T1.Text.Length == 10)
                {
                    string Qwerty = "Select * from BazaKl WHERE NIP  Like '%" + SerchNip + "%' ";
                    DTwybKlient = SqlComandDatabase_NewBaza(Qwerty, con);
                    if (DTwybKlient.Rows.Count >= 0)
                        UzupDaneKL();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // Sprawdz czy NIP istnieje w trakcie wprowadzania

        public void UzupDaneKL()
        {
            try
            {
                Xid.Content = int.Parse(DTwybKlient.Rows[0]["Id"].ToString());
                T1.Text = DTwybKlient.Rows[0]["NIP"].ToString(); // (2)
                T2.Text = DTwybKlient.Rows[0]["Stan"].ToString(); // (3)
                T3.Text = DTwybKlient.Rows[0]["Numer_konta"].ToString(); // (4)
                T4.Text = DTwybKlient.Rows[0]["Nazwa_klienta"].ToString() + " " + DTwybKlient.Rows[0]["Nazwa_CD"].ToString(); //(5) & "(6)
                T6.Text = DTwybKlient.Rows[0]["Adres"].ToString(); // (7)
                T7.Text = DTwybKlient.Rows[0]["Kod_Poczta"].ToString(); // (8)
                T8.Text = DTwybKlient.Rows[0]["Poczta"].ToString(); // (9)
                T9.Text = DTwybKlient.Rows[0]["Forma_plac"].ToString(); // (10)
                T10.Text = DTwybKlient.Rows[0]["PraceList"].ToString(); // (11)
                T11.Text = DTwybKlient.Rows[0]["Branza"].ToString(); // (12)
                T12.Text = DTwybKlient.Rows[0]["Tel"].ToString(); // (13)
                T13.Text = DTwybKlient.Rows[0]["E_mail"].ToString(); //(14)
                Spr_Aktywnosckonta();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public void WstawZaktualizowaneDaneKL()
        {
            string DtValStr = DtVal;
            if ((string)tButton3.Content == "Zmień dane")
            {
                {
                    var withBlock = Cennik_Add;
                    if (DtVal == default)
                        return;
                    Get_KlientDane.NIP = T1.Text;
                    Get_KlientDane.Stan = T2.Text;
                    Get_KlientDane.Numer_konta = T3.Text;
                    Get_KlientDane.Nazwa_klienta = T4.Text;
                    Get_KlientDane.Adres = T6.Text;
                    Get_KlientDane.Kod_Poczta = T7.Text;
                    Get_KlientDane.Poczta = T8.Text;
                    Get_KlientDane.Forma_plac = T9.Text;
                    Get_KlientDane.PraceList = T10.Text;
                    Get_KlientDane.Branza = T11.Text;
                    Get_KlientDane.Tel = T12.Text;
                    Get_KlientDane.E_mail = T13.Text;
                    Get_KlientDane.Rabat_Double = Zwroc_RAbat(T10.Text.ToString());
                }

            }
        }
        private void T2_SelectionChanged(object sender, TextChangedEventArgs e)
        {
            Spr_Aktywnosckonta();
        }
        private void Spr_Aktywnosckonta()
        {
            if (T2.Text == default)
                return;
            if (T3.Text == default)
                return;
            if (Strings.Mid(T2.Text.ToString(), 1, 4) == "Akty" & Strings.Mid(T3.Text.ToString(), 1, 1) == "1")
            {
                T1.IsReadOnly = true;  // nip
                T1.IsEnabled = false;
                T3.IsReadOnly = true; // SAP
                T3.IsEnabled = false;
                T4.IsReadOnly = true; // nazwa klienta
                T4.IsEnabled = false;
                T6.IsReadOnly = true; // Adres
                T6.IsEnabled = false;
                T7.IsReadOnly = true; // KodPoczta
                T7.IsEnabled = false;
                T8.IsReadOnly = true;  // poczta
                T8.IsEnabled = false;
                T11.IsReadOnly = false; // branza
                T11.IsEnabled = false;
            }
            else
            {
                T1.IsReadOnly = false;  // nip
                T1.IsEnabled = true;
                T3.IsReadOnly = false; // SAP
                T3.IsEnabled = true;
                T4.IsReadOnly = false; // nazwa klienta
                T4.IsEnabled = true;
                T6.IsReadOnly = false; // Adres
                T6.IsEnabled = true;
                T7.IsReadOnly = false; // KodPoczta
                T7.IsEnabled = true;
                T8.IsReadOnly = false;  // poczta
                T8.IsEnabled = true;
                T11.IsReadOnly = true; // branza
                T11.IsEnabled = true;

            }
        }


        public void AktualDane()
        {
            string Tim = TimeAktual();
            // (Opiekun_klienta,NIP,Stan,Numer_konta,Nazwa_klienta,Nazwa_CD,Adres,Kod_poczta,Poczta,Forma_plac,PraceList,Branza,Tel,E_mail,OstAkt) 
            var a0 = System.Windows.Forms.SystemInformation.UserName;
            string Nip = Strings.Replace(T1.Text, "-", "");
            string cT2 = T2.Text;
            string cT9 = T9.Text;
            string cT10 = T10.Text;
            string cT11 = T11.Text;
            string Opiekun_Update;
            if (Upr_User.UprKO == false)
                Opiekun_Update = "opiekun_klienta = '" + Upr_User.User_PH + "',";
            else
                Opiekun_Update = "";
            if (Nip.Length == 10)
            {
                string sqlstring = @" -- Try to update any existing row 
                                    UPDATE BazaKl SET " + Opiekun_Update + " Stan='" + cT2 + "',Numer_konta='" + T3.Text.ToString() + "',Nazwa_klienta='" + T4.Text.ToString() + "', Nazwa_CD = '', Adres='" + T6.Text.ToString() + "',Kod_poczta='" + T7.Text.ToString() + "',Poczta='" + T8.Text.ToString() + @"',
                                            Forma_plac='" + cT9 + "' , PraceList='" + cT10 + "', Branza='" + cT11 + "' , Tel='" + T12.Text + "' ,  E_mail='" + T13.Text + "' ,  OstAkt='" + Tim + @"' 
                                    WHERE  NIP like '%" + Nip + @"%' ;
                                
                                -- If no update happened (i.e. the row didn't exist) then insert one
                                    INSERT INTO BazaKl (Opiekun_klienta,NIP,Stan,Numer_konta,Nazwa_klienta,Nazwa_CD,Adres,Kod_poczta,Poczta,Forma_plac,PraceList,Branza,Tel,E_mail,OstAkt) 
                                    SELECT '" + Upr_User.User_PH + "','" + Nip + "','" + cT2 + "','" + T3.Text.ToString() + "','" + T4.Text.ToString() + "','','" + T6.Text.ToString() + "','" + T7.Text.ToString() + "','" + T8.Text.ToString() + "','" + cT9 + "','" + cT10 + "','" + cT11 + "','" + T12.Text.ToString() + "','" + T13.Text.ToString() + "','" + Tim + @"'
                                WHERE (Select Changes() = 0);";
                // Console.WriteLine(sqlstring)

                int a = UsingSQLComand(sqlstring, con);

                string ttip = Opiekun_Update + Microsoft.VisualBasic.Constants.vbCrLf + T13.Text;
                var branzyst = TableKoBranz.DefaultView.RowFilter = string.Format("Branza LIKE '%{0}%'", cT11.Substring(0, 2));

                Klient.KlientBaza.AddClientToListMw(Mw.ListKlient.Count + 1, Opiekun_Update, Nip, cT2, T3.Text.ToString(), T4.Text.ToString(), T6.Text.ToString(), T7.Text.ToString(), T8.Text.ToString(),
                    cT9, cT10, cT11, T12.Text, T13.Text, branzyst[0].ToString(), branzyst[1].ToString(), ttip, cT10);
                BazaKlient = SqlComandDatabase_NewBaza(StringComand.ReturnComandBazaKlient(), con);
                Klient.KlientBaza.AktualBza();
            }
            else
            {
                Interaction.MsgBox("wprowadz własciwe dane");
            }
        } // Aktualizuj dane klienta baza SQL(plik)



        private void Remowe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Nip = Strings.Replace(T1.Text, "-", "");
                Mw.VievPageVisibli(true, true, "");
                string stringqwey = "delete from BazaKl where Id Like '%" + Xid.Content + "%' ";
                UsingSQLComand(stringqwey, con);
                Klient.KlientBaza.ClearClientItemToListMw(Nip);
                BazaKlient = SqlComandDatabase_NewBaza(StringComand.ReturnComandBazaKlient(), con);
                Klient.KlientBaza.AktualBza();


                // Mw.WczytajBazaklient();
                RunClear_textBox();
                Mw.VievPageVisibli(false, false, "");

            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            ClearMe();
        }
        private void Clear_textBox(object sender, MouseButtonEventArgs e)
        {
            RunClear_textBox();
        }
        private void RunClear_textBox()
        {
            T1.Text = null;
            T2.Text = null;
            T3.Text = null;
            T4.Text = null;
            T6.Text = null;
            T7.Text = null;
            T8.Text = null;
            T9.Text = null;
            T10.Text = null;
            T11.Text = null;
            T12.Text = null;
            T13.Text = null;
        }

        private void ZatwierdzZapis_MouseDown(object sender, MouseButtonEventArgs e) // Handles tButton3.MouseDown
        {

            Mw.VievPageVisibli(true, true, "");
            try
            {
                if (T1.Text.Length != 10)
                {
                    Interaction.MsgBox("wprowadz prawidłowy NIP");
                    return;
                }
                if (T7.Text.Length != 6)
                {
                    Interaction.MsgBox("wprowadz prawidłowy Kod pocztowy");
                    return;
                }
                AktualDane();
                WstawZaktualizowaneDaneKL();
                RemoveMe();
                Mw.VievPageVisibli(false, false, "");
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }

        }

        private void T3_TextChanged(object sender, TextChangedEventArgs e)
        {
            var Spr_double = default(double);
            double.TryParse(T3.Text.ToString(), out Spr_double);
            if (Spr_double > 0d & Conversions.ToDouble(Strings.Mid(Spr_double.ToString(), 1, 1)) == 1d & Spr_double.ToString().Length == 7)
                T2.Text = "Aktywny";
        }

    }
}
