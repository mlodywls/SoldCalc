using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SoldCalc.UpdateWorker;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using static SoldCalc.MainWindow;

namespace SoldCalc
{


    public partial class RejonPH : Page
    {
        private const int Panel1MaxWidth = 1100;
        private readonly int Split1Panel1MaxHeight = 180;
        private int strawdzzmiane;
        private int strawdzzmianePH;
        private System.Data.DataTable Ph_user = new System.Data.DataTable();
        public string NazwaBazaKL;
        public string NazwaBazaZak;
        private SQLiteConnection NewConKl = new SQLiteConnection();
        private SQLiteConnection NewCobZak = new SQLiteConnection();
        private BackgroundWorker worker1 = default;
        public BackgroundWorker SendBaza = default;

        public RejonPH()
        {
            InitializeComponent();
        }
        private void RejonPH_Load(object sender, EventArgs e)
        {
            NewConKl.ConnectionString = ConectString("DB_Klient", NewConKl);
            NewCobZak.ConnectionString = ConectString("DB_ZAKUPY", NewConKl);
            NazwaBazaKL = "DB_Klient.db";
            NazwaBazaZak = "DB_ZAKUPY.db";
            File.Copy(FullPath, AktualFullPath, true);
            ConClose();
            URLstatus = FVerificaConnessioneInternet();


            if (Acon.ConnectionString.ToString() == "")
                Acon.ConnectionString = Actualconnstring;
            if (PHcon.ConnectionString.ToString() == "")
                PHcon.ConnectionString = Actualconnstring;
            if (URLstatus == true)
                Wczytaj();
            RadioButton2.IsChecked = true;
            Linfo.Content = "";
            Mw.Dock_Aktual_Progres.Visibility = Visibility.Collapsed;
            strawdzzmiane = 0;
            strawdzzmianePH = 0;
            worker1 = new BackgroundWorker();
            worker1.WorkerSupportsCancellation = true;
            worker1.WorkerReportsProgress = true;
            this.worker1.DoWork += worker1_DoWork;
            this.worker1.ProgressChanged += worker_ProgressChanged;
            this.worker1.RunWorkerCompleted += worker1_RunWorkerCompleted;


            SendBaza = new BackgroundWorker();
            SendBaza.WorkerSupportsCancellation = true;
            SendBaza.WorkerReportsProgress = true;
            this.SendBaza.DoWork += SendBaza_DoWork;
            this.SendBaza.ProgressChanged += SendBaza_ProgressChanged;
            this.SendBaza.RunWorkerCompleted += SendBaza_RunWorkerCompleted;


        }

        private void Wyslij_Click(object sender, RoutedEventArgs e)
        {
            DowBazaPH();
        }

        private void DowBazaPH()
        {
            Mw.VievPageVisibli(true, false, "");
            SendBaza.RunWorkerAsync();
        }

        private void SendBaza_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                URLstatus = FVerificaConnessioneInternet();
                Console.WriteLine("SendBaza start");
                if (NewConKl.State == ConnectionState.Open)
                    NewConKl.Close();

                if (NewCobZak.State == ConnectionState.Open)
                    NewCobZak.Close();


                if (URLstatus == true)
                {
                    Console.WriteLine("SendBaza URLstatus = {0}", URLstatus);
                    int a = Wyslij_Pobraną_baze_DB__StartSerwer(NazwaBazaKL, LocatiAktual + @"\" + NazwaBazaKL, SendBaza);
                    Console.WriteLine("a = {0}", a);
                    int b = Wyslij_Pobraną_baze_DB__StartSerwer(NazwaBazaZak, LocatiAktual + @"\" + NazwaBazaZak, SendBaza);
                    Console.WriteLine("b = {0}", b);
                }
                else
                {
                    Interaction.MsgBox("brak połaczenia z internetem" + Microsoft.VisualBasic.Constants.vbCrLf + " Sprawdz połączenie!");
                    return;
                }
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void SendBaza_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Mw.VievPageVisibli(false, false, "");
        }
        private void SendBaza_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int _Stan = e.ProgressPercentage;

                lblTime.Content = _Stan + "%";
                Dock_Aktual_LabProgr.Value = _Stan;
                labelProgres.Content = _Stan + "%";
                if (AktualNewBaza.ileZ == 20)
                {
                    lblTime.Content = "";
                    LabIleZ.Content = "Send..";
                }
                try
                {
                    InfoLabelKryj.Content = "Send file";
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void DowBaza_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DowBazaPH();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Wczytaj()
        {
            try
            {
                WczytajBaza_KLIENT_DoDGV();
                C3_WczytajPHdoCombo();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void PictureBox3_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Wczytaj();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        public void WczytajBaza_KLIENT_DoDGV()
        {
            try
            {
                Ph_user.Clear();
                if (NewConKl.State == ConnectionState.Closed)
                    NewConKl.Open();
                string sqwert = "Select * from BazaKl";
                Ph_user = SqlComandDatabase_NewBaza(sqwert, NewConKl);
                try
                {
                    var grdView = new GridView();
                    foreach (DataColumn col in Ph_user.Columns)
                    {
                        var bookColumn = new GridViewColumn() { DisplayMemberBinding = new System.Windows.Data.Binding(col.ColumnName), Header = col.ColumnName };
                        grdView.Columns.Add(bookColumn);
                    }
                    ListRejonPH.View = grdView;
                    var bind = new System.Windows.Data.Binding() { Source = Ph_user.DefaultView };
                    ListRejonPH.SetBinding(System.Windows.Controls.ListView.ItemsSourceProperty, bind);
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }

                if (NewConKl.State == ConnectionState.Open)
                    NewConKl.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // Wczytaj Baza Klientów

        private readonly System.Data.DataTable DataCom;

        private void C3_WczytajPHdoCombo()
        {
            try
            {
                try
                {
                    C1.Items.Clear();
                    C2.Items.Clear();
                    C3.Items.Clear();
                }
                catch (Exception ex)
                {
                }
                if (NewConKl.State == ConnectionState.Closed)
                    NewConKl.Open();
                var da = new SQLiteDataAdapter("SELECT distinct Opiekun_klienta   FROM BazaKL  GROUP BY  Opiekun_klienta ", NewConKl);
                var dt = new System.Data.DataTable();
                int i = da.Fill(dt);
                if (i > 0)
                {
                    var row = dt.NewRow();
                    dt.Rows.InsertAt(row, 0);
                    C1.ItemsSource = dt.DefaultView;
                    C1.DisplayMemberPath = "Opiekun_klienta";
                    C2.ItemsSource = dt.DefaultView;
                    C2.DisplayMemberPath = "Opiekun_klienta";
                    C3.ItemsSource = dt.DefaultView;
                    C3.DisplayMemberPath = "Opiekun_klienta";
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void SercgData(string valueToSearch)
        {
            string T1 = "";
            string T2 = "";
            string T3 = "";
            string T4 = "";
            string T5 = "";
            string T6 = "";
            string T7 = C1.Text;
            string T8 = C2.Text;
            string[] splittext = valueToSearch.Split(' ');
            if (splittext.Count() == splittext.Count())
            {
                try
                {
                    if (splittext[0].Length > 0)
                    {
                        T1 = splittext[0].ToString();
                        valueToSearch = splittext[0].ToString();
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
                catch (Exception ex)
                {

                }
            }
            try
            {
                // "CREATE TABLE BazaKL , Opiekun_klienta	TEXT, Nip	TEXT UNIQUE, Stan	TEXT, Numer_konta	TEXT, Nazwa_klienta	TEXT, Nazwa_CD	TEXT, Adres TEXT, Kod_Poczta	TEXT, Poczta	TEXT, Forma_plac	TEXT, PraceList	TEXT, Branza	TEXT, Tel TEXT, E_mail TEXT, PRIMARY KEY(id));"
                Ph_user.DefaultView.RowFilter = string.Format(@"Opiekun_klienta Like '%{0}%' and Opiekun_klienta Like '%{1}%' and Opiekun_klienta Like '%{2}%' and Opiekun_klienta Like '%{3}%' and Opiekun_klienta Like '%{4}%' and Opiekun_klienta Like '%{5}%' and Opiekun_klienta Like '%{7}%' or Opiekun_klienta Like '%{6}%'
                                                                                And NIP LIKE '%{0}%' and NIP Like '%{1}%' and NIP Like '%{2}%' and NIP Like '%{3}%' and NIP Like '%{4}%' and NIP Like '%{5}%'
                                                                                OR Numer_konta LIKE '%{0}%' and Numer_konta Like '%{1}%' and Numer_konta Like '%{2}%' and Numer_konta Like '%{3}%' and Numer_konta Like '%{4}%' and Numer_konta Like '%{5}%'
                                                                                OR Nazwa_klienta LIKE '%{0}%' and Nazwa_klienta Like '%{1}%' and Nazwa_klienta Like '%{2}%' and Nazwa_klienta Like '%{3}%' and Nazwa_klienta Like '%{4}%' and Nazwa_klienta Like '%{5}%'
                                                                                OR Nazwa_CD LIKE '%{0}%' and Nazwa_CD Like '%{1}%' and Nazwa_CD Like '%{2}%' and Nazwa_CD Like '%{3}%' and Nazwa_CD Like '%{4}%' and Nazwa_CD Like '%{5}%'
                                                                                OR Kod_Poczta LIKE '%{0}%' and Kod_Poczta Like '%{1}%' and Kod_Poczta Like '%{2}%' and Kod_Poczta Like '%{3}%' and Kod_Poczta Like '%{4}%' and Kod_Poczta Like '%{5}%'
                                                                                OR Adres LIKE '%{0}%' and Adres Like '%{1}%' and Adres Like '%{2}%' and Adres Like '%{3}%' and Adres Like '%{4}%' and Adres Like '%{5}%'
                                                                                OR Poczta LIKE '%{0}%' and Poczta Like '%{1}%' and Poczta Like '%{2}%' and Poczta Like '%{3}%' and Poczta Like '%{4}%' and Poczta Like '%{5}%'
                                                                                OR Branza LIKE '%{0}%' and Branza Like '%{1}%' and Branza Like '%{2}%' and Branza Like '%{3}%' and Branza Like '%{4}%' and Branza Like '%{5}%'

                                                                                        ", T1, T2, T3, T4, T5, T6, T7, T8); // and Opiekun_klienta Like '%{6}%' or Opiekun_klienta Like '%{7}%' 
                if (string.IsNullOrEmpty(valueToSearch))
                    Ph_user.DefaultView.RowFilter = null;
                Linfo.Content = "Znaleziono " + Ph_user.DefaultView.Count + " pozycji";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // Działanie wyszukaj produkty z txtbox, combo1;2;3
        private void SerchInsChange(string t1, string t2)
        {
            try
            {
                Ph_user.DefaultView.RowFilter = string.Format(@"Opiekun_klienta LIKE '%{0}%' or Opiekun_klienta Like '%{1}%'                                                                          
                                                                                        ", t1, t2);
                Linfo.Content = "Znaleziono " + (Ph_user.Rows.Count - 1) + " pozycji";
            }

            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // Filtruj dane do zmiany
        private void C3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SercgData(Conversions.ToString(((System.Windows.Controls.ComboBox)sender).Text)); // , sender.Name)
                ListRejonPH.SelectedItem = false;
                XSap.Text = "";
                XNazw.Text = "";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // wywolaj działanie szukaj, usuń odwołanie do combo
        private void Tszukaj_TextChanged(object sender, TextChangedEventArgs e)
        {
            SercgData(Conversions.ToString(((System.Windows.Controls.TextBox)sender).Text)); // , sender.Name)
            ListRejonPH.SelectedItem = false;
            XSap.Text = "";
            XNazw.Text = "";
        }

        private void PictureBox1_Click(object sender, MouseButtonEventArgs e)
        {
            int serch = SprawdzZCyWszystko();
            string message;
            string[] PH1;// = C1.Text.Split(' ');
            string[] PH2;// = C2.Text.Split(' ');
            //string PH1, PH2;
            string err;
            if (serch == 0 && Tszukaj.Text != "")
            {
                Linfo.Content = "Operacja została anulowana!" + Microsoft.VisualBasic.Constants.vbCrLf + "Do wykonania tego rodzaju polecenia konieczne jest zaznaczenie pozycji do przekazania !!!!" + Microsoft.VisualBasic.Constants.vbCrLf + "Zaznacz pozycję po czym ponów próbę! ";
                return;
            }
            if (C1.Text == "" && C2.Text == "")
            {
                Linfo.Content = "Wybierz PH!!!";
                return;
            }


            if (((Image)sender).Name.ToString() == "Pb2")
            {
                PH1 = Strings.Replace(C1.Text, "  ", " ").Split(' ');// C1.Text.Split(' ');
                PH2 = Strings.Replace(C2.Text, "  ", " ").Split(' ');
            }
            else// if (((Image)sender).Name.ToString() == "Pb2")
            {
                PH1 = Strings.Replace(C2.Text, "  ", " ").Split(' ');
                PH2 = Strings.Replace(C1.Text, "  ", " ").Split(' ');
            }


            if (RadioButton2.IsChecked == true)
            {
                Interaction.MsgBox(RadioButton2.IsChecked.ToString() + Microsoft.VisualBasic.Constants.vbCrLf + PH1[0] + " " + PH1[1] + Microsoft.VisualBasic.Constants.vbCrLf + PH2[0] + " " + PH2[1]);

                if (serch == 0)
                {
                    message = Aktual_ZAKUPY(serch, PH1, PH2, NewConKl);
                }
                else
                {
                    message = Aktual_ZAKUPY(serch, PH1, PH2, NewCobZak);
                }
            }
            else
            {
                Interaction.MsgBox(RadioButton2.IsChecked.ToString() + Microsoft.VisualBasic.Constants.vbCrLf + PH1[0] + " " + PH1[1] + Microsoft.VisualBasic.Constants.vbCrLf + PH2[0] + " " + PH2[1]);
                if (serch == 0)
                {
                    Interaction.MsgBox(PH1 + Microsoft.VisualBasic.Constants.vbCrLf + PH2);
                    message = Aktual_ZAKUPY(serch, PH1, PH2, NewConKl);
                }
                else
                {
                    message = Aktual_ZAKUPY(serch, PH1, PH2, NewCobZak);

                }
            }

            string Tim = TimeAktual();


            if (message == "1")
                err = "1";
            else
                err = "";
            Wczytaj();
            if (message == "0")
            {
                Linfo.Content = "Zakończono powodzeniem !";
                DowBaza.Background = new SolidColorBrush(Colors.Red);
                DowBaza.Content = "Wyślij zaktualizowaną bazę";
            }
            if (message == "1")
            {
                strawdzzmiane = 0;
                Linfo.Content = "Bład aktualiacji Bazy Klienta !" + Microsoft.VisualBasic.Constants.vbCrLf + "Sprubój ponownie wykonać polecenie wprowadzając dane" + Microsoft.VisualBasic.Constants.vbCrLf + "Upewnij się czy wprowadzasz własciwe dane!" + Microsoft.VisualBasic.Constants.vbCrLf + "Możesz również zamknąc okno i urucomić ponownie! Po czym wykonać polecenia na nowo" + Microsoft.VisualBasic.Constants.vbCrLf + " - Dane nie zostaną zapisane z uwagi na błąd";
            }
            if (err == "1")
            {
                strawdzzmiane = 0;
                Linfo.Content = "Bład aktualiacji Bazy Klienta i Bazy Zakupów !" + Microsoft.VisualBasic.Constants.vbCrLf + "Sprubój ponownie wykonać polecenie wprowadzając dane" + Microsoft.VisualBasic.Constants.vbCrLf + "Upewnij się czy wprowadzasz własciwe dane!" + Microsoft.VisualBasic.Constants.vbCrLf + "Możesz również zamknąc okno i urucomić ponownie! Po czym wykonać polecenia na nowo" + Microsoft.VisualBasic.Constants.vbCrLf + " - Dane nie zostaną zapisane z uwagi na błąd";
            }
            else if (message == "2")
            {
                strawdzzmiane = 0;
                Linfo.Content = "Bład aktualiacji Bazy Zakupów !" + Microsoft.VisualBasic.Constants.vbCrLf + "Sprubój ponownie wykonać polecenie wprowadzając dane" + Microsoft.VisualBasic.Constants.vbCrLf + "Upewnij się czy wprowadzasz własciwe dane!" + Microsoft.VisualBasic.Constants.vbCrLf + "Możesz również zamknąc okno i urucomić ponownie! Po czym wykonać polecenia na nowo" + Microsoft.VisualBasic.Constants.vbCrLf + " - Dane nie zostaną zapisane z uwagi na błąd";
            }
        }
        private int SprawdzZCyWszystko()
        {
            try
            {
                int serch = 0;
                if (XSap.Text != "")
                {
                    serch = 1;
                }
                else
                {
                    serch = 0;
                }
                return serch;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }

        private string Aktual_ZAKUPY(int S, string[] a0, string[] a1, SQLiteConnection zcon)
        {
            int err;
            string Tim = TimeAktual();
            string Tmiserw = (Conversions.ToDouble(Strings.Mid(Tim, 6)) * 1d).ToString() + (Conversions.ToDouble(Strings.Mid(Tim, 6, 8)) * 1d + 1d) + Conversions.ToDouble(Strings.Mid(Tim, 8)) * 1d;

            //a0 = Strings.Replace(a0, "  ", " ");
            a0[0] = Strings.Replace(a0[0], " ", "");
            a0[1] = Strings.Replace(a0[1], " ", "");
            a1[0] = Strings.Replace(a1[0], " ", "");
            a1[1] = Strings.Replace(a1[1], " ", "");
            if (zcon.State == ConnectionState.Closed)
                zcon.Open();
            if (S == 0)
            {
                string SqlMin = "Select OstAkt From BazaZKP WHERE OstAkt not like '' Order By OstAkt ASC LIMIT 1";
                string Mindata = SqlRoader_Jedna_wartosc(SqlMin, NewCobZak);
                string sqwert = "UPDATE BazaKl SET opiekun_klienta = '" + a0[0] + " " + a0[1] + "' , OstAkt='" + Tim + "'  WHERE opiekun_klienta like '%" + a1[0] + "%' AND opiekun_klienta like '%" + a1[1] + "%' ;";
                Console.WriteLine("1 Aktual_ZAKUPY - " + sqwert);
                UsingSQLComand(sqwert, NewConKl);
                sqwert = "UPDATE BazaZKP SET Representative = '" + a0[0] + " " + a0[1] + "', OstAkt='" + Tim + "'  WHERE Representative like '%" + a1[0] + "%' AND Representative like '%" + a1[1] + "%';";
                Console.WriteLine("2 Aktual_All_PH_excel - " + sqwert);
                UsingSQLComand(sqwert, NewCobZak);
                string SqlZmiana = @" -- Try to update any existing row
	                                        UPDATE InfoZmiana
	                                        SET PHOd = '" + a1[0] + " " + a1[1] + "',PHDo='" + a0[0] + " " + a0[1] + "',InfoData='" + Mindata + @"'
	                                        WHERE PHOd like '%" + a1[0] + " " + a1[1] + @"%';
	                                   -- If no update happened (i.e. the row didn't exist) then insert one                                         
                                            INSERT INTO InfoZmiana  (PHOd ,PHDo ,InfoData)
                                            SELECT '" + a1[0] + " " + a1[1] + "','" + a0[0] + " " + a0[1] + "','" + Mindata + @"'                    
                                            WHERE (Select Changes() = 0);";
                UsingSQLComand(SqlZmiana, NewCobZak);
            }
            else
            {
                DataRowView item = ListRejonPH.Items.GetItemAt(ListRejonPH.SelectedIndex) as DataRowView;
                string sqwert = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("UPDATE BazaKl SET opiekun_klienta = '" + a0[0] + " " + a0[1] + "', OstAkt='" + Tim + "'  WHERE NIP like '%", item[2]), "%';"));
                Console.WriteLine("2 Aktual_ZAKUPY - " + sqwert);
                UsingSQLComand(sqwert, NewConKl);
                sqwert = "UPDATE BazaZKP SET Representative = '" + a0[0] + " " + a0[1] + "', OstAkt='" + Tim + "'  WHERE SoldTocustomer like '%" + a1[0] + "%' AND SoldTocustomer like '%" + a1[1] + "%';";
                Console.WriteLine("2 Aktual_All_PH_excel - " + sqwert);
                UsingSQLComand(sqwert, NewCobZak);
            }
            err = 0;
            strawdzzmiane = 1;
            return err.ToString();
        }

        private void C1_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SerchInsChange(C1.Text, C2.Text);
                ListRejonPH.SelectedItem = false;
                XSap.Text = "";
                XNazw.Text = "";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }


        private void RadioButton1_CheckedChanged(object sender, RoutedEventArgs e)
        {
        }

        private void LvListRejonPH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView item = ListRejonPH.Items.GetItemAt(ListRejonPH.SelectedIndex) as DataRowView;
                GridView itTab = ListRejonPH.View as GridView;
                XSap.Text = item[2].ToString();
                XNazw.Text = item[4].ToString() + " - " + item[5].ToString() + "" + item[6].ToString();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }


        private System.Data.DataTable ImportToExCelRejPH()
        {
            try
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                System.Windows.Forms.DialogResult Result;
                Result = dialog.ShowDialog();
                System.Data.DataTable dt = default;
                if (Result == System.Windows.Forms.DialogResult.OK)
                {
                    dt = AktualBazaKlient.Import_Z_Pliku_Excel_to_Datatable(dialog.FileName);
                line1:;
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName.Contains("Opiekun") == true)
                            column.ColumnName = "Opiekun_klienta";
                        if (column.ColumnName.Contains("NIP") == true)
                            column.ColumnName = "NIP";
                        if (column.ColumnName.Contains("Stan") == true)
                            column.ColumnName = "Stan";
                        if (column.ColumnName.Contains("konta") == true)
                            column.ColumnName = "Numer_konta";
                        if (column.ColumnName.Contains("Nazwa") & column.ColumnName.Contains("klienta") == true)
                            column.ColumnName = "Nazwa_klienta";
                        if (column.ColumnName.Contains("cd") == true)
                            column.ColumnName = "Nazwa_CD";
                        if (column.ColumnName.Contains("ulica") == true)
                            column.ColumnName = "Adres";
                        if (column.ColumnName.Contains("miasto") == true)
                            column.ColumnName = "Poczta";
                        if (column.ColumnName.Contains("kod") == true)
                            column.ColumnName = "Kod_poczta";
                        if (column.ColumnName.Contains("Telefon") == true)
                            column.ColumnName = "Tel";
                        if (column.ColumnName.Contains("Customer Group") == true)
                            column.ColumnName = "Branza";
                        if (column.ColumnName.Contains("Price") == true)
                            column.ColumnName = "PraceList";
                        if (column.ColumnName.Contains("modyfikować") == true)
                        {
                            dt.Columns.Remove(column);
                            goto line1;
                        }
                    }
                }
                else if (Result == System.Windows.Forms.DialogResult.Cancel)
                {
                    dialog.Dispose();
                }
                return dt;
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }





        private System.Data.DataTable Dtt = new System.Data.DataTable();
        private int ostR;
        private void ZmienPH_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (ZmienPH.Content.ToString() != "Wstaw Dane do Baza")
                {
                    ListRejonPH.View = (ViewBase)(object)null;

                    Dtt = ImportToExCelRejPH();
                    try
                    {
                        var grdView = new GridView();
                        foreach (DataColumn col in Dtt.Columns)
                        {
                            var bookColumn = new GridViewColumn() { DisplayMemberBinding = new System.Windows.Data.Binding(col.ColumnName), Header = col.ColumnName };
                            grdView.Columns.Add(bookColumn);
                        }
                        ListRejonPH.View = grdView;
                        var bind = new System.Windows.Data.Binding() { Source = Dtt.DefaultView };
                        ListRejonPH.SetBinding(System.Windows.Controls.ListView.ItemsSourceProperty, bind);
                    }

                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.ToString());
                    }
                    ZmienPH.Content = "Wstaw Dane do Baza";
                }
                else
                {
                    try
                    {
                        worker1.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private string Aktual_All_PH_excel(int s, string PH, string Nr_kL, SQLiteConnection zcon)
        {
            int err;
            string Tim = TimeAktual();
            if (zcon.State == ConnectionState.Closed)
                zcon.Open();
            PH = Strings.Replace(PH, "  ", " ");
            if (s == 0)
            {
                string sqwert = "UPDATE BazaKL SET Opiekun_klienta = '" + PH + "', OstAkt='" + Tim + "'  WHERE NIP like '%" + Nr_kL + "%';";
                Console.WriteLine("1 Aktual_All_PH_excel - " + sqwert);
                UsingSQLComand(sqwert, NewConKl);
            }
            else
            {
                string sqwert = "UPDATE BazaZKP SET Representative = '" + PH + "', OstAkt='" + Tim + "'  WHERE SoldTocustomer like '%" + Nr_kL + "%';";
                Console.WriteLine("2 Aktual_All_PH_excel - " + sqwert);
                UsingSQLComand(sqwert, NewCobZak);
            }

            err = 0;
            return err.ToString();
        }

        private void worker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int S = 0;
            int i = 1;
            ostR = Dtt.Rows.Count;
            foreach (DataRow row in Dtt.Rows)
            {
                S = 0;
                if (row["Opiekun_klienta"].ToString().Length > 2 & row["NIP"].ToString().Length > 2)
                {
                    Aktual_All_PH_excel(S, row["Opiekun_klienta"].ToString(), row["NIP"].ToString(), NewConKl);
                }
                S = 1;
                if (row["Opiekun_klienta"].ToString().Length > 2 & row["Numer_konta"].ToString().Length > 2)
                {
                    Aktual_All_PH_excel(S, row["Opiekun_klienta"].ToString(), row["Numer_konta"].ToString(), NewCobZak);
                }
                i = i + 1;
                worker1.ReportProgress(i);
            }
        }
        private void worker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ZmienPH.Content = "Zmień Listę PH";
                Wczytaj();
                progres.Width = 0;
                HomeProg.Content = "";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int _Stan = (e.ProgressPercentage / ostR) * 100;
                progres.Width = _Stan * 2; // (e.ProgressPercentage / ostR) * 200
                HomeProg.Content = _Stan + " %";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

    }
}
