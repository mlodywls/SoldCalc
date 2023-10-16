using Microsoft.VisualBasic;
using Microsoft.Win32;
using SoldCalc.Supporting;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc
{

    public partial class HistZak : Page
    {

        internal static BackgroundWorker worker;
        private string WHEREUprKO;
        private string ViewKO, WievRepresentative;
        private string TxKlient, TxProdukt, CbBranza, CbRokTxt, CbPH, CbKO, Cb4;
        private string Tt1 = "";
        private string Tt2 = "";
        private string CTRTt3 = null;
        private bool ChackKl = false; 
        private string AktualMinData; 
        public string ConvertToDateTime(string value)
        {
            DateTime convertedDate;
            string zmien = null;
            try
            {
                convertedDate = Convert.ToDateTime(value);
                zmien = convertedDate.ToString();
                string format = "dd-MM-rr"; 
                zmien = convertedDate.ToString(format);
            }
            catch (FormatException)
            {
                zmien = value;
            }
            return zmien;
        }


        public HistZak()
        {
            base.Loaded += HistoriaZKP_Load;
            InitializeComponent();
            if (Upr_User.MinData == "")
                Upr_User.MinData = "2019";
            if (Upr_User.MinData != null)
            {
                int data = int.Parse(DateTime.Now.Year.ToString());
                for (int i = int.Parse(Strings.Mid(Upr_User.MinData, 1, 4)), loopTo = data; i <= loopTo; i++)
                {
                    ComboBoxRok.Items.Add(i);
                    if (i == data - 4)
                        AktualMinData = i.ToString();
                }
            }
        }

        private void HistoriaZKP_Load(object sender, EventArgs e)
        {

            if (Upr_User.UprKO == false)
                WHEREUprKO = "";
            else
                WHEREUprKO = "si.Representative as PH,";
            this.DataContext = this;
            StAdmin.DataContext = Upr_User;
            StPHCmb.DataContext = Upr_User;
            StPHTG.DataContext = Upr_User;
            St1.DataContext = Upr_User;

            Get_KlientDane.Numer_konta = "";

            ComboBoxRok.Text = Upr_User.O_Data.ToString();

            //Representative();
            //Comb_KO();
            //WczytajComboBR();
            //ShowKLMemory = "";
            //LinfoData.Content = "Aktualizacja do dnia - " + Upr_User.MaxData;
            //if (Upr_User.UprKO == false)
            //    ViewKO = "si.Representative Like '%" + Upr_User.Imie + "%' and si.Representative like '%" + Upr_User.Nazwisko + "%' ";
            //else
            //    ViewKO = "";
            //worker = new BackgroundWorker();
            //worker.WorkerSupportsCancellation = true;
            //worker.WorkerReportsProgress = true;

            //worker.DoWork += worker_DoWork;
            //worker.ProgressChanged += worker_ProgressChanged;
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            //SegregYear(Zkp);
            //CreateDynamicGridView(Zkp, DGHistZak);

            //UstawTotal();
            //CreateDynamicZestZKP(Zkp2);
            //if (con.State == ConnectionState.Open)
            //    con.Close();
            //SQLText.Text = strComand; 
            //ComboBoxRok.IsEditable = true;
        }
        private void Hist1_Checked(object sender, RoutedEventArgs e)
        {
        }
        private void Wyswietl_Checked(object sender, RoutedEventArgs e)
        {
            UkryjColDG2();
        }
        private void UkryjColDG2()
        {
            if (DGHistZak is null)
                return;
            if (TextBoxProdukt.Text.ToString() != "")
                ukryj_Kl.Visibility = Visibility.Visible;
            else
            {
                ukryj_Kl.Visibility = Visibility.Collapsed;
                ukryj_Kl.IsChecked = true;
            }


            foreach (var col in DGHistZak.Columns)
            {
                if (col.Header.ToString() == "Branza")
                {
                    if (ukryj_Branza.IsChecked == true)
                        col.Visibility = Visibility.Visible;
                    else
                        col.Visibility = Visibility.Collapsed;
                }
                if (col.Header.ToString() == "KO")
                {
                    if (ukryj_KO.IsChecked == true)
                        col.Visibility = Visibility.Visible;
                    else
                        col.Visibility = Visibility.Collapsed;
                }
                if (col.Header.ToString() == "Representative")
                {
                    if (ukryj_PH.IsChecked == true)
                        col.Visibility = Visibility.Visible;
                    else
                        col.Visibility = Visibility.Collapsed;
                }
                if (col.Header.ToString() == "Klient")
                {
                    if (ukryj_Kl.IsChecked == true)
                        col.Visibility = Visibility.Visible;
                    else
                        col.Visibility = Visibility.Collapsed;
                }
            }
            string txtProd = TextBoxProdukt.Text.ToString();
            if (ukryj_Kl.IsChecked == true & TextBoxSzukKlient.Text.ToString() == "")
            {
                TextBoxSzukKlient.Text = " ";
                return;
            }
            if (ukryj_Kl.IsChecked == false & TextBoxSzukKlient.Text.ToString() == " ")
            {
                TextBoxSzukKlient.Text = "";
                Reflash_Data();
            }
        }

        private void CreateDynamicGridView(System.Data.DataTable baza, DataGrid grid)
        {
            if (baza is null)
                return;
            var grdView = new GridView();
            foreach (DataColumn col in baza.Columns)
            {
                var bookColumn = new GridViewColumn() { DisplayMemberBinding = new Binding(col.ColumnName), Header = col.ColumnName };


                if (Information.IsNumeric(col.ColumnName))
                {
                    if (int.Parse(Strings.Mid(col.ColumnName.ToString(), 1, 4)) >= Upr_User.O_Data)
                        bookColumn.Width = 100;
                    if (int.Parse(Strings.Mid(col.ColumnName.ToString(), 1, 4)) >= int.Parse(Strings.Mid(Upr_User.MaxData.ToString(), 1, 4)))
                        goto LineNext;
                }
                if (Strings.Mid(col.ColumnName, 1, 4) == "szt ")
                {
                    if (int.Parse(Strings.Mid(col.ColumnName, 5, 8).ToString()) > int.Parse(Upr_User.MaxData.ToString()))
                        goto LineNext;
                }
                grdView.Columns.Add(bookColumn);
            LineNext:
                ;

            }
            grid.DataContext = grdView;
            var bind = new Binding() { Source = baza.DefaultView };
            grid.SetBinding(ListView.ItemsSourceProperty, bind);
            UkryjColDG2();
        }
        private void UstawTotal()
        {
            int WCtr = 0;
            GridView ListV = DGHistZak.DataContext as GridView;

            for (int i = 0, loopTo = ListV.Columns.Count - 1; i <= loopTo; i++)
            {
                if (Strings.Mid(DGHistZak.Columns[i].Header.ToString(), 1, 4) == Upr_User.O_Data.ToString())
                    DGHistZak.Columns[i].Header = Upr_User.O_Data.ToString() + " r." + Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf + Strings.Format(Val_Meany[0], "# ### ##0.00") + " zł"; // : 
                if (Strings.Mid(DGHistZak.Columns[i].Header.ToString(), 1, 4) == (Upr_User.O_Data + 1).ToString())
                    DGHistZak.Columns[i].Header = (Upr_User.O_Data + 1).ToString() + " r." + Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf + Strings.Format(Val_Meany[1], "# ### ##0.00") + " zł"; // :
                if (Strings.Mid(DGHistZak.Columns[i].Header.ToString(), 1, 4) == (Upr_User.O_Data + 2).ToString())
                    DGHistZak.Columns[i].Header = (Upr_User.O_Data + 2).ToString() + " r." + Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf + Strings.Format(Val_Meany[2], "# ### ##0.00") + " zł"; // : 
                if (Strings.Mid(DGHistZak.Columns[i].Header.ToString(), 1, 4) == (Upr_User.O_Data + 3).ToString())
                    DGHistZak.Columns[i].Header = (Upr_User.O_Data + 3).ToString() + " r." + Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf + Strings.Format(Val_Meany[3], "# ### ##0.00") + " zł"; // :
            }
            for (int i = 0, loopTo1 = ListV.Columns.Count - 1; i <= loopTo1; i++)
            {
                if (Strings.Mid(DGHistZak.Columns[i].Header.ToString(), 1, 2) != "20")
                    DGHistZak.Columns[i].Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                else
                {
                    try
                    {
                        DGHistZak.Columns[i].Width = double.NaN;
                    }
                    catch
                    {
                    }
                }
                if (DGHistZak.Columns[i].Header.ToString() == "KO")
                {
                    DGHistZak.Columns[i].Header = "KO";
                    DGHistZak.Columns[i].Width = 200;
                }
            }
        }
        private void Comb_KO()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                string sqlString = "Select KO from DaneKO group by KO";
                ComboBoxKO.ItemsSource = SqlComandDatabase(sqlString, con).DefaultView;
                ComboBoxKO.Text = "";
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
            }
            if (con.State == ConnectionState.Open)
                con.Close();
        }
        private void Representative()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                string sqlString;
                if (Upr_User.UprKO == false)
                {
                    sqlString = "select Representative from BazaZKP WHERE Representative like '%" + Upr_User.Nazwisko + "%' and Representative like '%" + Upr_User.Imie + "%'   group by Representative";
                }
                else
                {
                    sqlString = "select Representative from BazaZKP group by Representative";
                }
                ComboBoxPH.ItemsSource = SqlComandDatabase(sqlString, con).DefaultView;
                ComboBoxPH.Text = "";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            if (con.State == ConnectionState.Open)
                con.Close();

        }

        public int SegregYear(System.Data.DataTable TblBran)
        {
            try
            {
                Val_Meany[0] = 0; Val_Meany[1] = 0; Val_Meany[2] = 0; Val_Meany[3] = 0;
                int d = Upr_User.O_Data;
                for (int i = 0; i <= TblBran.Rows.Count - 1; i++)
                {
                    DataRow row = TblBran.Rows[i];
                    if (Information.IsNumeric(row[Upr_User.O_Data]))
                    {
                        double Spr_double;
                        double.TryParse(row[Upr_User.O_Data].ToString(), out Spr_double);
                        row[Upr_User.O_Data] = Strings.Format(Spr_double, "# ### ##0.00");
                        Val_Meany[0] += Spr_double; //Strings.Format(Spr_double, "# ### ##0.00");
                    }
                    if (Information.IsNumeric(row[Upr_User.O_Data + 1]))
                    {
                        double Spr_double;
                        double.TryParse(row[Upr_User.O_Data + 1].ToString(), out Spr_double);
                        row[Upr_User.O_Data + 1] = Strings.Format(Spr_double, "# ### ##0.00");
                        Val_Meany[1] += Spr_double; //Strings.Format(Spr_double, "# ### ##0.00");
                    }
                    if (Information.IsNumeric(row[Upr_User.O_Data + 2]))
                    {
                        double Spr_double;
                        double.TryParse(row[Upr_User.O_Data + 2].ToString(), out Spr_double);
                        row[Upr_User.O_Data + 2] = Strings.Format(Spr_double, "# ### ##0.00");
                        Val_Meany[2] += Spr_double; //Strings.Format(Spr_double, "# ### ##0.00");
                    }
                    if (Information.IsNumeric(row[Upr_User.O_Data + 3]))
                    {
                        double Spr_double;
                        double.TryParse(row[Upr_User.O_Data + 3].ToString(), out Spr_double);
                        row[Upr_User.O_Data + 3] = Strings.Format(Spr_double, "# ### ##0.00");
                        Val_Meany[3] += Spr_double; //Strings.Format(Spr_double, "# ### ##0.00");
                    }
                }
                return default(int);
            }
            catch
            {
            }
            return default(int);
        }


        private void TextBox6_TextChanged(object sender, TextChangedEventArgs e) // e As EventArgs)
        {
            object[] parameters = new object[1] { ((System.Windows.Controls.TextBox)sender).Name }; // , obje11, obje12}
            try
            {
                CheckBoxZMR.IsChecked = false;
                try
                {
                    worker.CancelAsync();
                }
                catch (Exception ex)
                {
                }
                GetDaneControls();
                string[] splittextC = TxProdukt.Split(' '); // As String()
                if (splittextC.Count() == splittextC.Count())
                {
                    if (splittextC[0].Length > 0)
                        Tt1 = splittextC[0].ToString();
                    else
                        Tt1 = "";
                    try
                    {
                        if (splittextC[1].Length > 0)
                            Tt2 = splittextC[1].ToString();
                        else
                            Tt2 = "";
                        if (splittextC[2].Length > 0)
                            CTRTt3 = splittextC[2].ToString();
                        else
                            CTRTt3 = "";
                    }
                    catch (Exception ex)
                    {
                    }
                }
                if (DGHistZak.Visibility == Visibility.Visible)
                {
                    Val_Meany[0] = 0;
                    Val_Meany[1] = 0;
                    Val_Meany[2] = 0;
                    Val_Meany[3] = 0;
                    Upr_User.O_Data = int.Parse(CbRokTxt);
                    try
                    {
                        worker.RunWorkerAsync(parameters);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    SercgDataZkp();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // szukaj produkt

        private void GetDaneControls()
        {
            try
            {
                TxKlient = TextBoxSzukKlient.Text.ToUpper();
                TxProdukt = TextBoxProdukt.Text;
                CbPH = ComboBoxPH.Text;
                CbBranza = ComboBranza.Text;
                CbRokTxt = ComboBoxRok.Text;
                CbKO = ComboBoxKO.Text;
                Upr_User.O_Data = int.Parse(CbRokTxt);
                ChackKl = bool.Parse(ukryj_Kl.IsChecked.ToString());
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    Console.WriteLine(" worker_RunWorkerCompleted -  e.Cancelled = true {0} ", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                }
                else
                {
                    Console.WriteLine(" worker_RunWorkerCompleted -  e.Cancelled = false {0} ", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                    if (Zkp != null)
                    {
                        Console.WriteLine(" worker_RunWorkerCompleted -  Zkp = isnot tothing {0} ", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                        SQLText.Text = strComand;
                        SegregYear(Zkp);
                        CreateDynamicGridView(Zkp, DGHistZak);
                        UstawTotal();
                    }
                    else
                    {
                        Console.WriteLine(" worker_RunWorkerCompleted -  Zkp = is nothing {0} ", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                        UstawTotal();
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public void WczytajComboBR()
        {
            try
            {
                string sqlString;
                if (con.State == ConnectionState.Closed)
                    con.Open();
                ComboBranza.ItemsSource = null; 
                ComboBranza.Items.Clear();
                if (ComboBoxKO.Text != "")
                    sqlString = "Select Branza ,KO from DaneKO  where KO like '%" + ComboBoxKO.Text + "%'  group by substr(Branza, 1, 2),KO";
                else
                    sqlString = "Select Branza from DaneKO  group by substr(Branza, 1, 2)";
                ComboBranza.ItemsSource = SqlComandDatabase(sqlString, con).DefaultView;
            }

            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            if (con.State == ConnectionState.Open)
                con.Close();
        }
        private void Kombo_DO_Worker_Select(object sender, EventArgs e)
        {

            object[] parameters = null; 

            try
            {
                if (DGHistZak.Visibility == Visibility.Visible)
                {
                    CheckBoxZMR.IsChecked = false;
                    if (((Image)sender).Name == "Pictureodswiez")
                    {
                        parameters = new object[1] { ((Image)sender).Name };
                        ComboBoxPH.Text = "";
                        ComboBranza.Text = "";
                        TextBoxSzukKlient.Text = "";
                        TextBoxProdukt.Text = "";
                        WczytajComboBR();
                    }
                    GetDaneControls();   
                    if (((ComboBox)sender).Name == "ComboBoxKO")
                        WczytajComboBR();
                    Zkp = null;
                    try
                    {
                        worker.CancelAsync();
                    }
                    catch
                    {
                    }
                    try
                    {
                        worker.RunWorkerAsync(parameters);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    SercgDataZkp();
                }
            }

            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } 
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine(" sKomenda_Do_worker_SQLString {0} ", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
            object[] parameters = e.Argument as object[];
            string Ctr_Name = parameters[0].ToString();
            string StringFormat;
            string DateString;
            string WyswietlProdukty, UprawnieniaKO;
            string WyswietlProduktNagl;
            bool Comand;
            if (TxProdukt.Length > 0)
                Comand = true;
            else
                Comand = false;
            string Serchcontrol = "GROUP BY si.Representative, si.SoldTocustomer ,si.Material";
            string WyswKlientNagl = "md.Nazwa_klienta || ' ' || md.Nazwa_CD  as Klient,";

            if (Ctr_Name.Contains("Combo") | Ctr_Name.Contains("Picture"))
            {
                Serchcontrol = "ComboBoxKO_PH";
                WyswKlientNagl = "";
            }
            if (Ctr_Name.Contains("TextBoxSzukKlient"))
                Serchcontrol = "TextBoxSzukKlient" + ChackKl;
            if (Ctr_Name.Contains("TextBoxProdukt"))
                Serchcontrol = "TextBoxProdukt";

            WyswietlProdukty = StringComand.GetComandSql(Comand, Serchcontrol);

            WyswietlProduktNagl = WyswKlientNagl + StringComand.GetComandSql(Comand, "ProduktNagl");
            DateString = StringComand.Suma_Yearbilling_SQL(Comand, Upr_User.O_Data.ToString());
            UprawnieniaKO = StringComand.GetComandSql(Upr_User.UprKO, "UprKO");
            WHEREUprKO = StringComand.GetComandSql(Upr_User.UprKO, "WyswietlPH");
            StringFormat = sKomenda_Do_worker_SQLString("worker", WyswietlProduktNagl, DateString, UprawnieniaKO, WyswietlProdukty);
            Console.WriteLine(" worker_DoWork {0} {1}", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()), StringFormat);

            strComand = StringFormat;
            while (!worker.CancellationPending)
            {
                bool exit = false;
                if (exit == true)
                    break;
                Zkp = SqlComandDatabase(StringFormat, con);
                exit = true;
                if (exit == true)
                    break;
            }
        }

        private void WstawZText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Reflash_Data();
        }

        private void Reflash_Data()
        {
            Zkp = SqlComandDatabase(SQLText.Text, con);
            SegregYear(Zkp);
            CreateDynamicGridView(Zkp, DGHistZak);
            UstawTotal();
        }
        public string sKomenda_Do_worker_SQLString(string Kolejnosc, string WyswietlProduktNagl, string DateString, string UprawnieniaKO, string WyswietlProdukty)
        {
            Console.WriteLine(" sKomenda_Do_worker_SQLString {0} ", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
            string Qwerty;
            string QwertyGrup;
            string QwertMarketing;
            if (Kolejnosc == "worker")
            {
                Qwerty = "   " + WHEREUprKO + "   md.Branza, mKO.KO, " + WyswietlProduktNagl + " " + DateString + "";
                QwertyGrup = WyswietlProdukty;
                QwertMarketing = "";
            }
            else
            {
                Qwerty = " " + WHEREUprKO + "  md.Nazwa_klienta || ' ' || md.Nazwa_CD  as Klient,  md.Branza, si.Material as Produkt, " + DateString + " ";
                QwertyGrup = " group by si.Representative ,si.SoldTocustomer , si.Material";
                QwertMarketing = "  AND si.SalesP like '0' and  mc.GRUPA not like 'MARKETING'";
            }
            string SQLqwerty = @" Select   
                                 " + Qwerty + @"
                            From BazaZKP si
                                LEFT JOIN  BazaKL md ON md.Numer_konta = substr(si.SoldTocustomer, 1, 7)
                                LEFT JOIN  DaneKO mKO ON substr(mKO.Branza,1,2) = substr(md.Branza ,1,2) 
                                LEFT JOIN  Cennik mc ON  mc.SAP = substr(si.Material, 1, 6) 
                            WHERE 
                               ((replace(UPPER(md.Nazwa_klienta), '-', '')  || ' ' || replace(UPPER(md.Nazwa_CD), '-', ''))  like '%" + Strings.Replace(TxKlient, " ", "") + @"%'
                                OR UPPER(si.SoldTocustomer) like '%" + TxKlient + @"%' )                                 
                                " + QwertMarketing + @"
                     
                                AND (replace(si.Material, '-', '') like '%" + Strings.Replace(TxProdukt, " ", "") + @"%'
                                OR (replace(mc.SAP, '-', '') like '%" + Strings.Replace(TxProdukt, " ", "") + @"%' ) 
								OR (mc.SAP || ' ' || mc.NazwProd)  like '%" + Strings.Replace(TxProdukt, " ", "") + @"%'
                                OR (replace(mc.NazwProd, '-', '') like '%" + Strings.Replace(TxProdukt, " ", "") + @"%'))
                                AND (md.Branza like '%" + Strings.Mid(CbBranza, 1, 2) + @"%')  
                                AND (si.Representative like '%" + CbPH + @"%')
                                AND (mKO.KO like '%" + CbKO + @"%')  

                             " + UprawnieniaKO + @"
                           " + QwertyGrup + " ;";
            strComand = SQLqwerty;
            // SQLText.Text = strComand
            return SQLqwerty;
        }


        private void Button3_Click_1(object sender, RoutedEventArgs e) // Handles BtnBranza.Click
        {
            try
            {
                ComboBoxKO.Text = "";
                ComboBoxPH.Text = "";
                ComboBranza.Text = "";
                TextBoxSzukKlient.Text = "";
                TextBoxProdukt.Text = "";
                CheckBoxZMR.IsChecked = false;
                WczytajComboBR();
                Console.WriteLine("wiersz 531 hist zak O_Data = {0} {1}", Upr_User.O_Data, ComboBoxRok.Text);
                Upr_User.O_Data = int.Parse(ComboBoxRok.Text.ToString());
                string SqlComand = StringComand.ComandBranzaSQL(Upr_User.UprKO, false, Upr_User.O_Data.ToString());
                Zkp = null;
                Zkp = SqlComandDatabase(SqlComand, con);
                SegregYear(Zkp);
                CreateDynamicGridView(Zkp, DGHistZak);
            }

            // UstawTotal()
            // SQLText.Text = strComand

            // If con.State = ConnectionState.Open Then con.Close()
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // Branża
        private void ZMR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetDaneControls();
                Zkp = null;
                Val_Meany[0] = 0;
                Val_Meany[1] = 0;
                Val_Meany[2] = 0;
                Val_Meany[3] = 0;
                // H_Val.Clear()
                var TblBran = new System.Data.DataTable();
                if (con.State == ConnectionState.Closed)
                    con.Open();
                string StringFormat = "";
                string UprawnieniaKO;
                string DateString;
                if (Upr_User.UprKO == false)
                {
                    UprawnieniaKO = "AND si.Representative Like '%" + Upr_User.Imie + "%' and si.Representative like '%" + Upr_User.Nazwisko + "%' ";
                    WHEREUprKO = "";
                }
                else
                {
                    UprawnieniaKO = "";
                    WHEREUprKO = "si.Representative as PH,";
                }
                if (CheckBoxZMR.IsChecked == true)
                    DateString = StringComand.Suma_YearbillingSUM_SQL(true, Upr_User.O_Data.ToString());
                else
                    DateString = StringComand.Suma_YearbillingSUM_SQL(false, Upr_User.O_Data.ToString());
                StringFormat = sKomenda_Do_worker_SQLString("ZMR", "", DateString, UprawnieniaKO, "");
                Console.WriteLine(" ZMR_Click {0} ", string.Format(" Wiersz# {0}", new StackTrace(new StackFrame(true)).GetFrame(0).GetFileLineNumber()));
                SQLText.Text = StringFormat;
                Zkp = SqlComandDatabase(StringFormat, con); // Newbaza.Copy
                if (con.State == ConnectionState.Open)
                    con.Close();
                if (Zkp != null)
                {
                    SegregYear(Zkp);
                    CreateDynamicGridView(Zkp, DGHistZak);

                    UstawTotal();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // ZMR

        private void Pobierz_plik_excel(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = Missing.Value;
                string savePath = null;
                var saveFileD = new SaveFileDialog();
                // .FileName = FilName
                saveFileD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);// global::My.Computer.FileSystem.SpecialDirectories.MyDocuments;
                saveFileD.RestoreDirectory = true;
                saveFileD.Filter = "Excel XLS Files(*.xls)|*.xls";
                saveFileD.FilterIndex = 1;
                if (saveFileD.ShowDialog() == true)
                {
                    savePath = saveFileD.FileName;
                }
                if (string.IsNullOrEmpty(savePath))
                    return;
                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = xlWorkBook.Sheets[1];
                for (int c = 0, loopTo = Zkp.Columns.Count - 1; c <= loopTo; c++) // dgv1.Columns.Count - 1
                {
                    try
                    {
                        {
                            var withBlock = xlWorkSheet.Cells[1, c + 1];
                            withBlock.Value = Zkp.Columns[c];
                            withBlock.EntireRow.Font.Bold = true;
                            withBlock.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                int B1 = 0;
                int B2 = 0;
                int B = 1;



                int j = 1;
                foreach (DataColumn col in Zkp.Columns)
                {
                    xlWorkSheet.Cells[1, j] = col.ColumnName;
                    j += 1;
                }
                for (int i = 0, loopTo1 = Zkp.Rows.Count - 1; i <= loopTo1; i++)
                {
                    int loopTo2 = Zkp.Columns.Count - B;
                    for (j = 0; j <= loopTo2; j++)
                        xlWorkSheet.Cells[(i + 2), (j + 1)] = Zkp.Rows[i][j];
                }
                // End If
                xlWorkSheet.Columns["A:O"].AutoFit();
                if (savePath != null && !string.IsNullOrEmpty(savePath.Trim()))
                {
                    xlWorkBook.SaveAs(savePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                }
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                MoreFunctionMW.ReleaseObject(xlWorkSheet);
                MoreFunctionMW.ReleaseObject(xlWorkBook);
                MoreFunctionMW.ReleaseObject(xlApp);
                MessageBox.Show("Twój plik został pomyślnie zapisany " + savePath);
                if (File.Exists(savePath))
                    Process.Start("explorer.exe", savePath);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }



        public void SercgDataZkp() // valueToSearch As String)
        {
            try
            {
                // https://wpf-tutorial.com/listview-control/listview-data-binding-item-template/
                string T1 = "";
                string T2 = "";
                string T3 = "";
                string T4 = "";
                string T5 = "";
                string T6 = "";
                string Cb1;
                if (ComboBranza.Text != "")
                    Cb1 = Strings.Mid(ComboBranza.Text, 1, 2);
                else
                    Cb1 = "";
                var splitklient = TextBoxSzukKlient.Text.Split(' ');
                try
                {
                    if (splitklient[0].Length > 0)
                        T1 = splitklient[0].ToString();
                    else
                        T1 = "";
                    if (splitklient[1].Length > 0)
                        T2 = splitklient[1].ToString();
                    else
                        T2 = "";
                    if (splitklient[2].Length > 0)
                        T3 = splitklient[2].ToString();
                    else
                        T3 = "";
                    if (splitklient[3].Length > 0)
                        T4 = splitklient[3].ToString();
                    else
                        T4 = "";
                    if (splitklient[4].Length > 0)
                        T5 = splitklient[4].ToString();
                    else
                        T5 = "";
                    if (splitklient[5].Length > 0)
                        T6 = splitklient[5].ToString();
                    else
                        T6 = "";
                }
                catch (Exception ex)
                {

                }

                Zkp2.DefaultView.RowFilter = string.Format(@"NazwaKL LIKE '%{0}%' and NazwaKL Like '%{1}%' and NazwaKL Like '%{2}%' and NazwaKL Like '%{3}%' and NazwaKL Like '%{4}%' and NazwaKL Like '%{5}%'
                                                        and Branza LIKE '%{6}%'                                                                                                                                                                
                                                        ", T1, T2, T3, T4, T5, T6, Cb1);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Test_polecenieSQL()
        {
            string SqlP = @"SELECT SoldTocustomer,
                                SUBSTR(Datebilling,-4,1)|| 
                                SUBSTR(Datebilling,-3,1)||
                                SUBSTR(Datebilling,-2,1)||
                                SUBSTR(Datebilling,-1,1)||'-'||
                                SUBSTR(Datebilling,-7,1)||
	                            SUBSTR(Datebilling,-6,1) as	'reversed' ,
	                            sum( Turnover) filter(where  Yearbilling = '2019')as '2019',
	                            sum( Turnover) filter(where  Yearbilling = '2020')as '2020',
                                sum( Turnover) filter(where  Yearbilling = '2021')as '2021',
	                            sum( Turnover) filter(where  Yearbilling = '2022')as '2022'
                                FROM  BazaZKP  group by SoldTocustomer 
                                ORDER BY
                                substr(reversed, 1, 4),
                                Yearbilling DESC";

            string sqlqwerty1 = @"SELECT SoldTocustomer,
                                    SUBSTR(Datebilling,-4,1)|| 
                                    SUBSTR(Datebilling,-3,1)||
                                    SUBSTR(Datebilling,-2,1)||
                                    SUBSTR(Datebilling,-1,1)||'-'||
                                    SUBSTR(Datebilling,-7,1)||
	                                SUBSTR(Datebilling,-6,1) as	'reversed' ,
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-01') as '2021-01',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-02') as '2021-02',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-03') as '2021-03',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-04') as '2021-04',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-05') as '2021-05',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-06') as '2021-06',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-07') as '2021-07',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-08') as '2021-08',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-09') as '2021-09',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-10') as '2021-10',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-11') as '2021-11',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-12') as '2021-12'
											

											
FROM  BazaZKP  group by SoldTocustomer 
ORDER BY
substr(reversed, 1, 4),
    Yearbilling DESC";

            string sqlqwerty2 = @"SELECT SoldTocustomer,
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-01') as '2021-01',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-02') as '2021-02',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-03') as '2021-03',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-04') as '2021-04',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-05') as '2021-05',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-06') as '2021-06',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-07') as '2021-07',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-08') as '2021-08',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-09') as '2021-09',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-10') as '2021-10',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-11') as '2021-11',
	                                sum( Turnover) filter(where  substr( SUBSTR(Datebilling,-4,1)|| 
											SUBSTR(Datebilling,-3,1)||
											SUBSTR(Datebilling,-2,1)||
											SUBSTR(Datebilling,-1,1)||'-'||
											SUBSTR(Datebilling,-7,1)||
											SUBSTR(Datebilling,-6,1), 1, 7) = '2020-12') as '2021-12'											
                                    FROM  BazaZKP  group by SoldTocustomer ";
        }
        private void YearOd_CalendarClosed(object sender, RoutedEventArgs e) // Handles YearOd.CalendarClosed
        {
            try
            {

                DateTime myDate1 = DateTime.ParseExact(YearOd.Text, "yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture);
                DateTime myDate2 = DateTime.ParseExact(YearDo.Text, "yyyy-MM-dd",
                                          System.Globalization.CultureInfo.InvariantCulture);


                // Zkp2 = Mw.Wyswietl_PHZestawienieZKP(myDate1.ToString().Substring(0,11), myDate2.ToString().Substring(0,11));
                CreateDynamicZestZKP(Zkp2);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void DatePicker_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                DatePicker datepicker = (DatePicker)sender;
                Popup popup = (Popup)datepicker.Template.FindName("PART_Popup", datepicker);
                System.Windows.Controls.Calendar cal = (System.Windows.Controls.Calendar)popup.Child;
                cal.DisplayMode = CalendarMode.Decade;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void CreateDynamicZestZKP(System.Data.DataTable baza)
        {
            try
            {
                if (baza is null)
                    return;
                var grdView = new GridView();
                foreach (DataColumn col in baza.Columns)
                {
                    var bookColumn = new GridViewColumn() { DisplayMemberBinding = new Binding(col.ColumnName), Header = col.ColumnName };
                    grdView.Columns.Add(bookColumn);
                }
                DataGridView3.DataContext = grdView;
                var bind = new Binding() { Source = baza.DefaultView };
                DataGridView3.SetBinding(ListView.ItemsSourceProperty, bind);
                foreach (DataGridColumn col in DataGridView3.Columns)
                {
                    // col.Width = New DataGridLength(1, DataGridLengthUnitType.Star)
                    if (col.Header == "Branza")
                        col.Width = 10;
                    if (col.Header == "NazwaKL")
                        col.Width = 500;
                    if (Strings.Mid(col.Header.ToString(), 1, 2) == "20")
                        col.Width = 60;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                worker.CancelAsync();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
    }


}
