using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using DataTable = System.Data.DataTable;

namespace SoldCalc
{

    public partial class Aktualizuj_Cennik //: Page
    {

        private DataTable DataCennik = new DataTable(); private DataTable DataCennikPDF = new DataTable(); private DataTable DTCombo = new DataTable();
        public ToolTip Tt;
        private const int Panel1MaxWidth = 1100; private readonly int Split1Panel1MaxHeight = 280; private int E_rowSelect;
        private BackgroundWorker worker1 = default;
        private BitmapImage myBitmapImage = new BitmapImage(); private BitmapImage myBitmapImageCopy = default;
        private string NazwaPlik; private string NewFile; private string NewFileKatrChar; private string NazwaBaza; private string Tim;
        private byte[] newFileData2 = null; private byte[] newFileData3 = null;
        private string[] files;


        public Aktualizuj_Cennik()
        {
            InitializeComponent();
            Tt = new ToolTip();
        }
        private void Aktualizuj_KO_Load(object sender, RoutedEventArgs e)
        {
            if (Dcon.State == ConnectionState.Open)
                Dcon.Close();
            Dcon.ConnectionString = ConectString("DB_Cennik", Dcon);
            NazwaBaza = "DB_Cennik.db";
            try
            {
                File.Copy(FullPath, AktualFullPath, true);
                ConClose();
                URLstatus = FVerificaConnessioneInternet();
                if (URLstatus == true)
                {
                    WczytajBaza_CENNK_DoDGV();
                    WczytajCombo();
                }
                T18.Items.Add("BrakPrace");
                T9.Items.Add("ml");
                T9.Items.Add("gr");
                T9.Items.Add("kg");
                T9.Items.Add("l");
                T9.Items.Add("m");
                T9.Items.Add("mb");
                T9.Items.Add("szt");
                T9.Text = "";
                T15.Text = "";
                Mw.Dock_Aktual_Progres.Visibility = Visibility.Collapsed;
                worker1 = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
                this.worker1.DoWork += worker1_DoWork;
                this.worker1.RunWorkerCompleted += worker1_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        public void WczytajBaza_CENNK_DoDGV()
        {
            try
            {
                DataCennik.Clear();
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                string sqlstring = "Select * from Cennik  ORDER BY Lpgrup ASC";
                DataCennik = SqlComandDatabase(sqlstring, Dcon);
                try
                {
                    var grdView = new GridView();
                    foreach (DataColumn col in DataCennik.Columns)
                    {
                        var bookColumn = new GridViewColumn() { DisplayMemberBinding = new Binding(col.ColumnName), Header = col.ColumnName };
                        if (col.ColumnName == "NazwProd")
                            bookColumn.Width = 330;
                        if (col.ColumnName == "Id")
                            bookColumn.Width = 0;
                        if (col.ColumnName == "ProdKod")
                            bookColumn.Width = 0;
                        if (col.ColumnName == "OstAkt")
                            bookColumn.Width = 0;
                        grdView.Columns.Add(bookColumn);
                    }
                    ListCennikAdd.DataContext = grdView;
                    var bind = new Binding() { Source = DataCennik.DefaultView };
                    ListCennikAdd.SetBinding(ListView.ItemsSourceProperty, bind);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                if (Dcon.State == ConnectionState.Open)
                    Dcon.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
            Ilewrs.Content = DataCennik.DefaultView.Count + " wierszy";
        }


        private void WczytajCombo()
        {
            try
            {
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                var da = new SQLiteDataAdapter("SELECT distinct  GRUPA, KATEGORIA, NAZEWNICTWO FROM Cennik  GROUP BY  GRUPA ", Dcon);
                var dt = new DataTable();
                int i = da.Fill(dt);
                DTCombo = dt.Copy();
                if (i > 0)
                {
                    var row = dt.NewRow();
                    dt.Rows.InsertAt(row, 0);
                    ComboBox1.ItemsSource = dt.DefaultView;
                    ComboBox1.DisplayMemberPath = "GRUPA";
                    T15.ItemsSource = DTCombo.DefaultView;
                    T15.DisplayMemberPath = "GRUPA";
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
            ComboBox3.Text = "";
        }

        private void PictureBox3_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                foreach (StackPanel siic in SticPan1.Children)
                {
                    if (siic is StackPanel)
                    {
                        foreach (StackPanel siic1 in siic.Children)
                        {
                            if (siic1 is StackPanel)
                            {
                                try
                                {
                                    foreach (Control txtBox in siic1.Children)
                                    {
                                        if (txtBox.GetType() == typeof(System.Windows.Controls.TextBox))
                                            ((System.Windows.Controls.TextBox)txtBox).Text = string.Empty;
                                        if (txtBox.GetType() == typeof(ComboBox))
                                            ((ComboBox)txtBox).Text = string.Empty;
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                                try
                                {
                                    foreach (StackPanel siic2 in siic1.Children)
                                    {
                                        if (siic2 is StackPanel)
                                        {
                                            try
                                            {
                                                foreach (Control txtBox in siic2.Children)
                                                {
                                                    if (txtBox.GetType() == typeof(System.Windows.Controls.TextBox))
                                                        ((System.Windows.Controls.TextBox)txtBox).Text = string.Empty;
                                                    if (txtBox.GetType() == typeof(ComboBox))
                                                        ((ComboBox)txtBox).Text = string.Empty;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
            Tszukaj.Text = "";
            ComboBox1.Text = "";
            ComboBox2.Text = "";
            ComboBox3.Text = "";
            WczytajBaza_CENNK_DoDGV();
        }


        private void Button5_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                OdktyjControl();
                SercgData(Tszukaj.Text);
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void OdktyjControl()
        {
            int idx = default;

            try
            {
                Tim = TimeAktual();
                string SqweryDelete = @" -- Try to update any existing row
                                    UPDATE Cennik
                                    SET ProdKod = '" + T1.Text + "',Naglowek='" + T2.Text + "',Lpgrup='" + T3.Text + "', NazwProd = '" + T5.Text + "',Kszt='" + T6.Text + "',Pszt='" + T7.Text + "',Poj='" + T8.Text + "',Miara='" + T9.Text + "', Kolor='" + T10.Text + @"' ,
                                    CDM='" + T11.Text + "',CK='" + T12.Text + "',PH='" + T13.Text + "',ZPR0='" + T14.Text + "',GRUPA='" + T15.Text + "',KATEGORIA='" + T16.Text + "',NAZEWNICTWO='" + T17.Text + "',BrakPrace='" + T18.Text + "',OstAkt='" + Tim + @"' 
                                    where SAP like '%" + T4.Text + @"%';
                                -- If no update happened (i.e. the row didn't exist) then insert one                                         
                                    INSERT INTO Cennik  (ProdKod,Naglowek,Lpgrup,SAP,NazwProd,Kszt,Pszt,Poj,Miara,Kolor,CDM,CK,PH,ZPR0,GRUPA,KATEGORIA,NAZEWNICTWO,BrakPrace,OstAkt)
                                    SELECT '" + T1.Text + "','" + T2.Text + "','" + T3.Text + "','" + T4.Text + "','" + T5.Text + "','" + T6.Text + "','" + T7.Text + "','" + T8.Text + "','" + T9.Text + "','" + T10.Text + "','" + T11.Text + "','" + T12.Text + "','" + T13.Text + "','" + T14.Text + "','" + T15.Text + "','" + T16.Text + "','" + T17.Text + "','" + T18.Text + "','" + Tim + @"'
                                    WHERE (Select Changes() = 0);";
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                string Mesage = UsingSQLComand(SqweryDelete, Dcon).ToString();
                if (Dcon.State == ConnectionState.Open)
                    Dcon.Close();
                WczytajBaza_CENNK_DoDGV();
                if (T0.Text != "")
                    ListCennikAdd.SelectedIndex = int.Parse(T0.Text);
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        private void Tszukaj_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (((System.Windows.Controls.TextBox)sender).Name == "Tszukaj")
                {
                    SercgData(Conversions.ToString(((System.Windows.Controls.TextBox)sender).Text)); // , sender.Name)
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void ComboBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SercgData(ComboBox1.Text);
                string serchqwery = "SELECT distinct  KATEGORIA FROM Cennik WHERE GRUPA like '%" + ComboBox1.Text + "'";
                ComboBox2 = CombGrData(serchqwery, Dcon);
                ComboBox2.Visibility = Visibility.Visible;
                ComboBox3.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        public static ComboBox CombGrData(string qwery, SQLiteConnection conection)
        {
            ComboBox Ctr = null;
            Ctr.ItemsSource = null;
            Ctr.ItemsSource = (System.Collections.IEnumerable)SqlComandDatabase(qwery, conection);
            return Ctr;
        }
        private void ComboBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                SercgData(ComboBox2.Text);
                string serchqwery = "SELECT distinct NAZEWNICTWO FROM Cennik WHERE KATEGORIA like '%" + ComboBox2.Text + "'";
                ComboBox3 = CombGrData(serchqwery, Dcon);
                ComboBox3.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void ComboBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SercgData(ComboBox3.Text);
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        private void SercgData(string valueToSearch) // , Cont As String)
        {
            try
            {
                string T1 = "";
                string T2 = "";
                string T3 = "";
                string T4 = "";
                string T5 = "";
                string T6 = "";
                string[] splittext = valueToSearch.Split(' '); // As String()
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
                    DataCennik.DefaultView.RowFilter = string.Format(@"NazwProd LIKE '%{0}%' and NazwProd Like '%{1}%' and NazwProd Like '%{2}%' and NazwProd Like '%{3}%' and NazwProd Like '%{4}%' and NazwProd Like '%{5}%'
                                                                                OR SAP LIKE '%{0}%' and SAP Like '%{1}%' and SAP Like '%{2}%' and SAP Like '%{3}%' and SAP Like '%{4}%' and SAP Like '%{5}%'
                                                                                OR GRUPA LIKE '%{0}%' and GRUPA Like '%{1}%' and GRUPA Like '%{2}%' and GRUPA Like '%{3}%' and GRUPA Like '%{4}%' and GRUPA Like '%{5}%'
                                                                                OR KATEGORIA LIKE '%{0}%' and KATEGORIA Like '%{1}%' and KATEGORIA Like '%{2}%' and KATEGORIA Like '%{3}%' and KATEGORIA Like '%{4}%' and KATEGORIA Like '%{5}%'
                                                                                OR NAZEWNICTWO LIKE '%{0}%' and NAZEWNICTWO Like '%{1}%' and NAZEWNICTWO Like '%{2}%' and NAZEWNICTWO Like '%{3}%' and NAZEWNICTWO Like '%{4}%' and NAZEWNICTWO Like '%{5}%'
                                                                                 ", T1, T2, T3, T4, T5, T6);
                    if (string.IsNullOrEmpty(valueToSearch))
                        DataCennik.DefaultView.RowFilter = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        private void SearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Information.IsNumeric(T4.Text))
                {
                    if (T4.Text.Length == 6)
                        T1.Text = T4.Text + " - " + T5.Text;
                }
                else
                {
                    T4.Text = "";
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (T5.IsReadOnly == false)
                {
                    if (Dcon.State == ConnectionState.Closed)
                        Dcon.Open();
                    string SqweryDelete = "delete from Cennik WHERE SAP Like '%" + T4.Text + "%' ";
                    string SqweryDelete2 = "delete from Cennik WHERE SAP Like '' ";
                    string Mesage = UsingSQLComand(SqweryDelete, Dcon).ToString();
                    string Mesage2 = UsingSQLComand(SqweryDelete2, Dcon).ToString();
                    if (Dcon.State == ConnectionState.Open)
                        Dcon.Close();
                    WczytajBaza_CENNK_DoDGV();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        public object GetFileName(string path)
        {
            try
            {
                return System.IO.Path.GetFileNameWithoutExtension(path);
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
                return null;
            }
        }

        private void ListCennikAdd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FTPInage.Source = (ImageSource)(object)null;
                listFiles.Items.Clear();
                pdfWebViewer.Navigate(new Uri("about:blank"));
                pdfWebViewerChar.Navigate(new Uri("about:blank"));
                int SComp;
                SComp = ListCennikAdd.SelectedIndex;
                if (SComp >= 0)
                {
                    DataRowView item = ListCennikAdd.Items.GetItemAt(ListCennikAdd.SelectedIndex) as DataRowView;
                    if (item is null)
                        return;
                    GridView itTab = ListCennikAdd.DataContext as GridView;
                    T0.Text = item[0].ToString();
                    T1.Text = item[1].ToString();
                    T2.Text = item[2].ToString();
                    T3.Text = item[3].ToString();
                    T4.Text = item[4].ToString();
                    T5.Text = item[5].ToString();
                    T6.Text = item[6].ToString();
                    T7.Text = item[7].ToString();
                    T8.Text = item[8].ToString();
                    T9.Text = item[9].ToString();
                    T10.Text = item[10].ToString();
                    T11.Text = item[11].ToString();
                    T12.Text = item[12].ToString();
                    T13.Text = item[13].ToString();
                    T14.Text = item[14].ToString();
                    T15.Text = item[15].ToString();
                    T16.Text = item[16].ToString();
                    T17.Text = item[17].ToString();
                    T18.Text = item[18].ToString();
                    T19.Text = item[19].ToString();
                    NazwaPlik = Conversions.ToString(item[17]);
                    do
                    {
                        try
                        {
                            var request1 = new WebClient();
                            string url = Strim_URL + "Img/" + item[17].ToString() + ".jpg";
                            request1.Credentials = new NetworkCredential(Uide, Pas);
                            byte[] newFileData = request1.DownloadData(url);
                            if (newFileData.Length == default)
                            {
                                FTPInage.Source = (ImageSource)(object)null;
                                listFiles.Items.Clear();
                                break;
                            }
                            var myBitmapImage = new BitmapImage();
                            myBitmapImage.BeginInit();
                            myBitmapImage.UriSource = new Uri(url);
                            myBitmapImage.DecodePixelWidth = 200;
                            myBitmapImage.EndInit();
                            FTPInage.Source = myBitmapImage;
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    while (false);


                    try
                    {
                        worker1.CancelAsync();
                    }
                    catch (Exception ex)
                    {
                    }
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
                TextMessage(ex.ToString());
            }
        }
        private void WyslijPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] PTds = File.ReadAllBytes(listFiles.Items[0].ToString());
                SendtOFR_as_FTP(PTds, "Pdf/", T17.Text, ".pdf");
                Interaction.MsgBox("wysłano - pdf");
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void WyslijChar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] PTds = File.ReadAllBytes(listFiles.Items[0].ToString());
                SendtOFR_as_FTP(PTds, "Kart_Char/", T17.Text, ".pdf");
                Interaction.MsgBox("wysłano - Kart_Char");
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void WyslijJpg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] bits = null;
                UpLoadImage(bits, "Img/", T17.Text, ".jpg");
                Interaction.MsgBox("wysłano - jpg");
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        public void UpLoadImage(byte[] data, string Folder, string FileName, string type)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(Strim_URL + Folder + FileName + type);
                req.UseBinary = true;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.Credentials = new NetworkCredential(Uide, Pas);
                byte[] fileData = File.ReadAllBytes(listFiles.Items[0].ToString());

                req.ContentLength = fileData.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(fileData, 0, fileData.Length);
                    reqStream.Close();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        public object SendtOFR_as_FTP(byte[] data, string Folder, string FileName, string type)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Strim_URL + Encode(Folder) + Encode(FileName) + type);
                request.Credentials = new NetworkCredential(Uide, Pas);
                request.Method = WebRequestMethods.Ftp.AppendFile;
                request.ContentLength = data.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }
                return null;
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
                return null;
            }
        }
        private void worker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (!worker1.CancellationPending)
                {
                    newFileData2 = null;
                    NewFile = null;
                    NewFileKatrChar = null;
                    bool exit = false;
                    if (exit == true)
                        break;
                    try
                    {
                        var request12 = new WebClient();
                        string url2 = Strim_URL + "Pdf/" + NazwaPlik + ".pdf";
                        request12.Credentials = new NetworkCredential(Uide, Pas);
                        newFileData2 = request12.DownloadData(url2);
                        string sFileName = NazwaPlik; // "test.pdf"
                        string sTempFileName = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, sFileName + ".pdf");
                        using (var FS = new FileStream(sTempFileName, FileMode.Create))
                        {
                            FS.Write(newFileData2, 0, newFileData2.Length);
                        }
                        NewFile = sTempFileName;
                    }
                    catch (Exception ex) { }
                    try
                    {
                        var request12 = new WebClient();
                        string url3 = Strim_URL + "Kart_Char/" + NazwaPlik + ".pdf";
                        request12.Credentials = new NetworkCredential(Uide, Pas);
                        newFileData3 = request12.DownloadData(url3);
                        string sFileName = NazwaPlik; // "test.pdf"
                        string sTempFileName = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, sFileName + "Char.pdf");
                        using (var FS = new FileStream(sTempFileName, FileMode.Create))
                        {
                            FS.Write(newFileData3, 0, newFileData3.Length);
                        }
                        NewFileKatrChar = sTempFileName;
                    }
                    catch (Exception ex)
                    {
                    }
                    if (exit == true)
                    {
                        break;
                    }
                }
                e.Result = 42;
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void worker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                }
                else
                {
                    try
                    {
                        pdfWebViewer.Navigate(NewFile);
                    }
                    catch (Exception ex)
                    {
                    }
                    try
                    {
                        pdfWebViewerChar.Navigate(NewFileKatrChar);
                    }

                    catch (Exception ex)
                    {

                    }
                    RunUkryjColDGCenn();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void UkryjColDGCenn(object sender, RoutedEventArgs e)
        {
            RunUkryjColDGCenn();
        }
        private void RunUkryjColDGCenn()
        {
            if (ListCennikAdd is null)
                return;
            foreach (var col in ListCennikAdd.Columns)
            {
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(col.Header, "Id", false)))
                    col.Visibility = Visibility.Collapsed;
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(col.Header, "ProdKod", false)))
                {
                    if (ukryj_ProdKod.IsChecked == true)
                        col.Visibility = Visibility.Visible;
                    else
                        col.Visibility = Visibility.Collapsed;
                }

            }
        }
        private void T16_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                var da = new SQLiteDataAdapter("SELECT distinct Lpgrup,Naglowek,  GRUPA, KATEGORIA, NAZEWNICTWO FROM Cennik    WHERE KATEGORIA like '%" + T16.Text + "%' GROUP BY  NAZEWNICTWO ", Dcon);
                var dt = new DataTable();
                var dt2 = new DataTable();
                int i = da.Fill(dt);
                dt2 = dt.Copy();

                if (i > 0)
                {
                    var row = dt.NewRow();
                    dt.Rows.InsertAt(row, 0);
                    T17.ItemsSource = dt2.DefaultView;
                    T17.DisplayMemberPath = "NAZEWNICTWO";

                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void T17_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                T3.Text = T17.SelectedValue.ToString();
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                var da = new SQLiteDataAdapter("SELECT distinct Lpgrup, GRUPA, KATEGORIA, NAZEWNICTWO FROM Cennik    WHERE GRUPA like '%" + ComboBox1.Text + "%' GROUP BY  KATEGORIA ", Dcon);
                var dt = new DataTable();

                int i = da.Fill(dt);
                if (i > 0)
                {
                    var row = dt.NewRow();
                    dt.Rows.InsertAt(row, 0);
                    ComboBox2.ItemsSource = dt.DefaultView;
                    ComboBox2.DisplayMemberPath = "KATEGORIA";
                }
                ComboBox2.Text = "";
                ComboBox3.Text = "";
                SercgData(Conversions.ToString(((ComboBox)sender).Text));
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Dcon.State == ConnectionState.Closed)
                    Dcon.Open();
                var da = new SQLiteDataAdapter("SELECT distinct Lpgrup, GRUPA, KATEGORIA, NAZEWNICTWO FROM Cennik   WHERE KATEGORIA like '%" + ComboBox2.Text + "%' GROUP BY  NAZEWNICTWO ", Dcon);
                var dt = new DataTable();
                int i = da.Fill(dt);
                if (i > 0)
                {
                    var row = dt.NewRow();
                    ComboBox3.ItemsSource = dt.DefaultView;
                    ComboBox3.DisplayMemberPath = "NAZEWNICTWO";
                }
                ComboBox3.Text = "";
                SercgData(Conversions.ToString(((ComboBox)sender).Text));
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SercgData(Conversions.ToString(((ComboBox)sender).Text));
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }

        private void SendToServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConClose();
                Wyslij_Pobraną_baze_DB__StartSerwer(NazwaBaza, LocatiAktual + @"\" + NazwaBaza, null);
                Interaction.MsgBox("Wysłano");
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }
        private void Window_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] droppedFiles = null;
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    droppedFiles = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                }
                if (droppedFiles is null || !droppedFiles.Any())
                {
                    return;
                }
                listFiles.Items.Clear();
                var myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();

                foreach (string s in droppedFiles)
                {
                    try
                    {
                        listFiles.Items.Add(s);
                        myBitmapImage.UriSource = new Uri(s);
                        myBitmapImage.DecodePixelHeight = 150;
                        myBitmapImage.EndInit();
                        FTPInage.Source = myBitmapImage;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }
        }


    }
}
