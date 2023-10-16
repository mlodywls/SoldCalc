using Microsoft.VisualBasic;
using SoldCalc.Supporting;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Controls
{
    public partial class ZapiszPDFvb
    {
        private BackgroundWorker WorkerSend = default;
        public DataTable UwagiDoPdf = new DataTable();
        private DataTable dtPDF = new DataTable();
        private readonly string strFilename = "";
        private readonly string PlikPath;
        private string SciezkaPliku_Do_DOWork_PDF = "";
        private string NazwaPliku_Do_DoWork_PDF = "";
        private int indexTxt;
        private int IndexTop;
        public string SendToEmail, TestEma;
        public ZapiszPDFvb()
        {
            // Console.WriteLine("new ZapiszPDFvb");
            InitializeComponent();
        }



        private void ZapiszPDFvb_Load(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("ZapiszPDFvb_Load");
                UwagiDoPdf.Columns.Add("Info");
                Mw.Info_Gif_Czekaj.Content = "Czekaj";

                ZapiszPDF = this;
                TxtFormPlac();
                indexTxt = 1;
                IndexTop = 30;

                LabInfo.Content = "";
                Label1.Content = Get_KlientDane.Nazwa_klienta;
                ComboPlac.Text = Get_KlientDane.Forma_plac;
                Textemail.Text = Get_KlientDane.E_mail;

                TextPDf.Text = NameFile(Get_KlientDane.Nazwa_klienta);
                SprDuplikat(Get_KlientDane.NIP);
                Serchtext();
                int LabKO = LiczPonCenaKO("KO", "CenaDoOFR", true);
                if (LabKO > 0)
                {
                    if (LabKO == 1)
                        LabInfoKO.Content = "";
                    if (LabKO == 1)
                        LabInfoKO.Content = "Wprowadzono " + LabKO + " produkt poniżej dopuszczalnej ceny !!! ";
                    if (LabKO >= 1 && LabKO <= 5)
                    {
                        LabInfoKO.Content = "Wprowadzono " + LabKO + " produkty poniżej dopuszczalnej ceny !!! ";
                    }
                    else
                    {
                        LabInfoKO.Content = "Wprowadzono " + LabKO + " produktów poniżej dopuszczalnej ceny !!! ";
                    } // & LabKO
                    LabInfoKO.Background = new SolidColorBrush(Colors.Red);
                    LabInfoKO.Opacity = 0.5d;
                }
                else
                {
                    LabInfoKO.Content = "";
                    LabInfoKO.Background = new SolidColorBrush(Colors.Transparent);
                }
                if (Textemail.Text == "")
                {
                    Textemail.Background = new SolidColorBrush(Colors.Red);
                    Textemail.Foreground = System.Windows.Media.Brushes.Black;
                    Label5.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            Serchtext();
            int AddSerch = Add_Kart_tds(AddTDS, "PlikTds", "TDS");
            if (AddSerch >= 1)
                AddTDS_Opis.Visibility = Visibility.Visible;
            else
                AddTDS_Opis.Visibility = Visibility.Collapsed;
            int AddSerchChar = Add_Kart_tds(AddChar, "PlikKch", "Char");
            if (AddSerchChar >= 1)
                Add_CharS_Opis.Visibility = Visibility.Visible;
            else
                Add_CharS_Opis.Visibility = Visibility.Collapsed;
        }
        private int Add_Kart_tds(WrapPanel ActivDocPanel, string RowPlikSerch, string O_Type)
        {
            int Add = 0;
            bool SerchPdf;
            int i = 0;
            foreach (var row in ListTblOfr.Tbl_Add_prodList)
            {
                SerchPdf = O_Type == "TDS" ? row.Plik_Tds_True : row.Plik_Kch_True;
                if (SerchPdf == true)
                {
                    string Sap;
                    string CtrTag;

                    Sap = row.SAP;
                    CtrTag = row.ID.ToString();
                    foreach (DataRow CenRow in BazaCennik.Rows)
                    {
                        if (Sap == CenRow["SAP"].ToString())
                        {

                            row.NazwaPdf = CenRow["NAZEWNICTWO"].ToString();
                            foreach (StackPanel AtdCtr in ActivDocPanel.Children)
                            {
                                foreach (object stP in AtdCtr.Children)
                                {
                                    if (stP is TextBlock)
                                    {
                                        TextBlock Txb = ((TextBlock)stP);
                                        // Console.WriteLine(stP.text)
                                        if (Txb.Text == CenRow["NAZEWNICTWO"].ToString())
                                        {
                                            goto LineNext;
                                        }

                                    }
                                }
                            }
                            // Console.WriteLine(CenRow("NAZEWNICTWO"))
                            var ST = new StackPanel() { Name = "stBaza" + O_Type + i, Opacity = 0.3d, MinWidth = 50, MaxWidth = 80, MinHeight = 100, Margin = new Thickness(10, 0, 0, 0), Tag = Sap };
                            var SP = new StackPanel() { Name = "stchack" + O_Type + i, Width = ST.Width, VerticalAlignment = VerticalAlignment.Bottom, Background = new SolidColorBrush(Colors.Red), Tag = Sap };
                            if (RowPlikSerch == "PlikTds")
                                SP.MouseDown += Stack_AddTDS_clik;
                            else
                                SP.MouseDown += Stack_AddChar_clik;
                            var CH = new CheckBox() { Name = "check" + O_Type + i, Width = 15, IsChecked = false, Tag = Sap };
                            if (RowPlikSerch == "PlikTds")
                                CH.Click += Stack_AddTDS_clik;
                            else
                                CH.Click += Stack_AddChar_clik;
                            SP.Children.Add(CH);
                            var TXB = new TextBlock() { Name = "tb" + O_Type + i, Text = CenRow["NAZEWNICTWO"].ToString(), FontSize = 10, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Center, Tag = Sap };
                            if (RowPlikSerch == "PlikTds")
                                TXB.MouseDown += Stack_AddTDS_clik;
                            else
                                TXB.MouseDown += Stack_AddChar_clik;
                            var img = new Image() { Name = "img" + O_Type + i, Width = 25, Height = 25, Source = new BitmapImage(new Uri("pack://application:,,,/images/PDF.png")), Tag = Sap };
                            if (RowPlikSerch == "PlikTds")
                                img.MouseDown += Stack_AddTDS_clik;
                            else
                                img.MouseDown += Stack_AddChar_clik;
                            var STctr = new System.Windows.Controls.Label() { Name = "stctr" + O_Type + i, Background = new SolidColorBrush(Colors.Yellow), Tag = Sap };


                            if (RowPlikSerch == "PlikTds")
                                STctr.MouseDown += Stack_AddTDS_clik;
                            else
                                STctr.MouseDown += Stack_AddChar_clik;
                            ST.Children.Add(SP);
                            ST.Children.Add(img);
                            ST.Children.Add(TXB);
                            ActivDocPanel.Children.Add(ST);
                            Add = 1;
                        }

                    LineNext:
                        ;


                    }
                }

                i += 1;
            }

            return Add;
        }

        private void Stack_AddTDS_clik(object sender, RoutedEventArgs e)
        {
            // Console.WriteLine("Final - " & sender.Name)
            string str = "";
            string Sname = "";
            if (sender is Button)
            {
                str = ((Button)sender).Tag.ToString();
                Sname = ((Button)sender).Name.ToString();
            }
            else if (sender is StackPanel)
            {
                str = ((StackPanel)sender).Tag.ToString();
                Sname = ((StackPanel)sender).Name.ToString();
            }
            else if (sender is TextBlock)
            {
                str = ((TextBlock)sender).Tag.ToString();
                Sname = ((TextBlock)sender).Name.ToString();
            }
            else if (sender is Image)
            {
                str = ((Image)sender).Tag.ToString();
                Sname = ((Image)sender).Name.ToString();
            }
            else if (sender is System.Windows.Controls.Label)
            {
                str = ((System.Windows.Controls.Label)sender).Tag.ToString();
                Sname = ((System.Windows.Controls.Label)sender).Name.ToString();
            }
            else if (sender is CheckBox)
            {
                str = ((CheckBox)sender).Tag.ToString();
                Sname = ((CheckBox)sender).Name.ToString();
            }




            foreach (StackPanel AtdCtr in AddTDS.Children)
            {
                if (AtdCtr.Tag.ToString() == str)
                {
                    if (AtdCtr is object)
                    {
                        foreach (object stP in AtdCtr.Children)
                        {
                            if (stP is StackPanel)
                            {
                                foreach (CheckBox ctr in ((StackPanel)stP).Children)
                                {
                                    if (Strings.Mid(Sname, 1, 4) == "stch")
                                    {
                                        if (ctr.IsChecked == true)
                                        {
                                            ctr.IsChecked = false;
                                            AtdCtr.Opacity = 0.3d;
                                        }
                                        else
                                        {
                                            ctr.IsChecked = true;
                                            AtdCtr.Opacity = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (ctr.IsChecked == true)
                                            AtdCtr.Opacity = 1;
                                        else
                                            AtdCtr.Opacity = 0.3d;
                                    }
                                    foreach (var item in ListTblOfr.Tbl_Add_prodList)
                                    {
                                        // Console.WriteLine(item.SAP & " " & str)
                                        if (item.SAP == str)
                                        {
                                            item.TDS_DO_OFR = ctr.IsChecked.Value;
                                            Console.WriteLine(item.SAP + " " + str + " " + item.TDS_DO_OFR);
                                            break;
                                        }
                                    }
                                    // Console.WriteLine("Final - " & stP.Name) ' & " / " & stP.IsChecked.ToString)
                                }
                            }
                            if (stP is Image)
                            {
                                // Console.WriteLine("Final - " & stP.Name) ' & " / " & stP.IsChecked.ToString)
                            }
                        }
                    }
                }
            }
        }
        private void Stack_AddChar_clik(object sender, RoutedEventArgs e)
        {
            // Console.WriteLine("Final - " & sender.Name)
            string str = "";
            string Sname = "";
            if (sender is Button)
            {
                str = ((Button)sender).Tag.ToString();
                Sname = ((Button)sender).Name.ToString();
            }
            else if (sender is StackPanel)
            {
                str = ((StackPanel)sender).Tag.ToString();
                Sname = ((StackPanel)sender).Name.ToString();
            }
            else if (sender is TextBlock)
            {
                str = ((TextBlock)sender).Tag.ToString();
                Sname = ((TextBlock)sender).Name.ToString();
            }
            else if (sender is Image)
            {
                str = ((Image)sender).Tag.ToString();
                Sname = ((Image)sender).Name.ToString();
            }
            else if (sender is System.Windows.Controls.Label)
            {
                str = ((System.Windows.Controls.Label)sender).Tag.ToString();
                Sname = ((System.Windows.Controls.Label)sender).Name.ToString();
            }
            else if (sender is CheckBox)
            {
                str = ((CheckBox)sender).Tag.ToString();
                Sname = ((CheckBox)sender).Name.ToString();
            }
            foreach (StackPanel AtdCtr in AddChar.Children)
            {
                if (AtdCtr.Tag.ToString() == str)
                {
                    if (AtdCtr is object)
                    {
                        foreach (object stP in AtdCtr.Children)
                        {
                            if (stP is StackPanel)
                            {
                                foreach (CheckBox ctr in ((StackPanel)stP).Children)
                                {
                                    if (Strings.Mid(Sname, 1, 4) == "stch")
                                    {
                                        if (ctr.IsChecked == true)
                                        {
                                            ctr.IsChecked = false;
                                            AtdCtr.Opacity = 0.3d;
                                        }
                                        else
                                        {
                                            ctr.IsChecked = true;
                                            AtdCtr.Opacity = 1;
                                        }
                                    }
                                    else
                                    {
                                        if (ctr.IsChecked == true)
                                            AtdCtr.Opacity = 1;
                                        else
                                            AtdCtr.Opacity = 0.3d;
                                    }
                                    foreach (var item in ListTblOfr.Tbl_Add_prodList)
                                    {
                                        if (item.SAP == str)
                                        {
                                            item.CHAR_DO_OFR = ctr.IsChecked.Value;
                                            break;
                                        }
                                    }
                                    // Console.WriteLine("Final - " & stP.Name) ' & " / " & stP.IsChecked.ToString)
                                }
                            }
                            if (stP is Image)
                            {
                                // Console.WriteLine("Final - " & stP.Name) ' & " / " & stP.IsChecked.ToString)
                            }
                        }
                    }
                }
            }
        }


        private Size Rozmiesc_Zastąpienie(Size finalSize)
        {
            double x = 0d;
            double y = 0d;
            double height = 0d;
            var children = AddTDS.Children.Cast<UIElement>().ToList();

            while (children.Count > 0)
            {
                var child = children.First();

                if (x >= 0d && x + child.DesiredSize.Width >= finalSize.Width)
                {
                    var fit = children.FirstOrDefault(c => x + c.DesiredSize.Width <= finalSize.Width);
                    child = fit ?? child;

                    if (x + child.DesiredSize.Width >= finalSize.Width)
                    {
                        x = 0d;
                        y = height;
                    }
                    Console.WriteLine(x.ToString());
                    break;
                }

                children.Remove(child);

                child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
                x += child.DesiredSize.Width;
                height = Math.Max(height, y + child.DesiredSize.Height);
            }

            return finalSize;
        }
        private object checktest(object sender)
        {
            return null;
        }


        public int LiczPonCenaKO(string KO, string CDN, bool Spr)
        {
            int licz = 0;
            int repl = 0;
            try
            {
                foreach (var item in ListTblOfr.Tbl_Add_prodList)
                {
                    if (Spr == true)
                    {
                        if (item.KO >= item.CenaDoOFR)
                        {
                            licz += 1;
                        }
                        repl = licz;
                    }
                    else
                    {
                        if (item.KO >= item.CenaDoOFR)
                        {
                            licz += 1;
                            string NazwProd = item.SAP.ToString() + " - " + item.NazwProd.ToString() + "ofertowana cena poniżej KO  - " + " - " + item.CenaDoOFR.ToString() + " -------- Cena KO produktu = " + item.KO.ToString();
                            repl = licz;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return repl;
        }


        private void TxtFormPlac()
        {
            try
            {
                ComboPlac.Items.Clear();
                ComboPlac.Items.Add("Przedpłata/ Gotówka kurier");
                ComboPlac.Items.Add("Gotówka Kurier");
                ComboPlac.Items.Add("Przedpłata");
                ComboPlac.Items.Add("Przelew 5 dni");
                ComboPlac.Items.Add("Przelew 7 dni");
                ComboPlac.Items.Add("Przelew 10 dni");
                ComboPlac.Items.Add("Przelew 14 dni");
                ComboPlac.Items.Add("Przelew 21 dni");
                ComboPlac.Items.Add("Przelew 28 dni");
                ComboPlac.Items.Add("Przelew 30 dni");
                ComboPlac.Items.Add("Przelew 40 dni");
                ComboPlac.Items.Add("Przelew 45 dni");
                ComboPlac.Items.Add("Przelew 50 dni");
                ComboPlac.Items.Add("Przelew 55 dni");
                ComboPlac.Items.Add("Przelew 60 dni");
                ComboPlac.Items.Add("Przelew 70 dni");
                ComboPlac.Items.Add("Przelew 75 dni");
                ComboPlac.Items.Add("Przelew 80 dni");
                ComboPlac.Items.Add("Przelew 90 dni");
                ComboPlac.Items.Add("Przelew 105 dni");
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }


        public void WorkerSend_DoWork(object sender, DoWorkEventArgs e)
        {
            WorkerSend.WorkerReportsProgress = true;
            BackgroundWorker bw = sender as BackgroundWorker;

            e.Result = GeneratePDF(NazwaPliku_Do_DoWork_PDF, SciezkaPliku_Do_DOWork_PDF, UwagiDoPdf);
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
        }
        private void WorkerSend_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // lblStatus.Text = "Working... (" & e.ProgressPercentage & "%)"
        }
        private int eResult = 0;
        private void WorkerSend_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            eResult += 1;
            // Console.WriteLine("eResultile {0} e.Result {1}", eResult, e.Result)
            try
            {
                if (e.Cancelled)
                {
                    Interaction.MsgBox("e.cncel" + e.Cancelled.ToString());
                }
                else
                {
                    SprDuplikat(Get_KlientDane.NIP);
                }
                Cennik_Add.WczytajZapisProd();
                LiczOfr.WczytajOfrDoCombo();
                Modul_Road.Wczytaj_Ofr_PDF();
                this.WorkerSend.ProgressChanged -= WorkerSend_ProgressChanged;
                this.WorkerSend.RunWorkerCompleted -= WorkerSend_RunWorkerCompleted;
                RemoveMe();
                Mw.VievPageVisibli(false, false, "");
                Mw.VievPage.Opacity = 1;
                foreach (string file_path in Directory.GetFiles(Scieżka_Pliku_AppData_FilesSC))
                {
                    try
                    {
                        if (file_path.Contains(".Pdf"))
                            File.Delete(file_path);
                    }
                    catch
                    {
           
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void GeneratePdf_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Serchtext();
            if (ListTblOfr.Tbl_Add_prodList.Count == 0)
            {
                Interaction.MsgBox("Do wygenerowania oferty konieczne jest wprowadzenie produktów");
                return;
            }
            if (ComboPlac.Text == "")
            {
                LabInfo.Content = "Wybierz formę płatnośći!";
                return;
            }
            NazwaPliku_Do_DoWork_PDF = EncodeString(TextPDf.Text) + ".Pdf";
            Console.WriteLine(Scieżka_Pliku_AppData_FilesSC + "/" + NazwaPliku_Do_DoWork_PDF);
            SciezkaPliku_Do_DOWork_PDF = System.IO.Path.Combine(Scieżka_Pliku_AppData_FilesSC, NazwaPliku_Do_DoWork_PDF);

            foreach (string file_path in Directory.GetDirectories(Scieżka_Pliku_AppData_FilesSC))
                Directory.Delete(file_path, true);
            if (System.IO.Directory.Exists(SciezkaPliku_Do_DOWork_PDF))
                //if (global::My.Computer.FileSystem.FileExists(SciezkaPliku_Do_DOWork_PDF))
                Interaction.MsgBox("Błąd plik nie może zostać nadpisany. zgłoś czynność. Aby wygenerować ofertę wylącz program i uruchom ponownie. Ofertę możesz zapisać przyciskiem zapisz");

            Mw.ComboPlacText = ComboPlac.Text;
            Mw.TIleDniText = TIleDni.Text;
            Mw.NameLab = ((System.Windows.Controls.Label)sender);
            Mw.NameLabString = ((Label)sender).Name;

            Mw.VievPageVisibli(true, false, "");

            SendToEmail = Textemail.Text;
            UwagiDoPdf.Clear();
            WorkerSend = new BackgroundWorker() { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            this.WorkerSend.DoWork += WorkerSend_DoWork;
            this.WorkerSend.ProgressChanged += WorkerSend_ProgressChanged;
            this.WorkerSend.RunWorkerCompleted += WorkerSend_RunWorkerCompleted;
            GetInfoTxtBoxAsOFRPdf();
            Update_FormPlac();
            WorkerSend.RunWorkerAsync();

        }

        private void Sprtext_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetInfoTxtBoxAsOFRPdf();
        }

        private object GetInfoTxtBoxAsOFRPdf()
        {
            // Console.WriteLine("GetInfoTxtBoxAsOFRPdf")
            try
            {
                if (AddTxtblock.ActualHeight > 5)
                {
                    foreach (TextBox CtlTxt in AddTxtblock.Children.OfType<TextBox>()) // Me.Controls
                    {
                        if (Strings.Mid(CtlTxt.Name, 1, 6) == "TxtInf")
                        {
                            string Txt = "UWAGI :";
                            UwagiDoPdf.Rows.Add(Txt);
                            break;
                        }
                    }
                    foreach (Control CtlTxt in AddTxtblock.Children.OfType<TextBox>()) // Me.Controls
                    {
                        if (CtlTxt is TextBox)
                        {
                            TextBox txtb = (TextBox)CtlTxt;
                            if (Strings.Mid(txtb.Name, 1, 6) == "TxtInf")
                            {
                                string Txt = txtb.Text;
                                UwagiDoPdf.Rows.Add(txtb.Text);
                                Console.WriteLine(txtb.Text);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            return null;
        }

        internal static ImageSource DoGetImageSourceFromResource(string psAssemblyName, string psResourceName)
        {
            try
            {
                var OUri = new Uri("pack://application:,,,/" + psAssemblyName + @"\" + psResourceName); // , UriKind.RelativeOrAbsolute)
                return BitmapFrame.Create(OUri);
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }
        internal int GeneratePDF(string NameFile, string MyPdfFilePath, DataTable TblUwagi)
        {
            int serch = 0;


            Console.WriteLine(" MyPdfFilePath " + MyPdfFilePath);
            byte[] PTds = File.ReadAllBytes(Modul_ItxPDF.GenerujOferta_tymczasowa_PDF(NameFile, MyPdfFilePath, TblUwagi, Mw.ComboPlacText));


            string eqNr = "";
            serch += 1;
            try
            {
                string serchUser = FTPConect.CreateFilePh_FTP();
            }
            catch (Exception ex)
            {
                eqNr += "CreateFilePh_FTP 1" + Constants.vbCrLf;
            }
            try
            {
                serch += SendtOFR_as_FTP(PTds, Get_KlientDane.NIP + "|" + NameFile);
            }
            catch (Exception ex)
            {
                eqNr += "SendtOFR_as_FTP 2" + Constants.vbCrLf;
            }

            try
            {
                serch += SawedPdf(Get_KlientDane.NIP, MyPdfFilePath, NameFile);
            }
            catch (Exception ex)
            {
                eqNr += "SawedPdf 3" + Constants.vbCrLf;
            }

            serch += OpenPdf_mail(MyPdfFilePath, NameFile);

            Cennik_Add.ZapiszRobocza();
            return serch;
        }

        public bool DirectoryExists(string directory)
        {
            try
            {
                var request = GetRequest(directory);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                return request.GetResponse() != null;
            }
            catch
            {
                return false;
            }
        }
        protected FtpWebRequest GetRequest(string filename = "")
        {
            FtpWebRequest request = WebRequest.Create(new Uri(Strim_URL + "/" + filename)) as FtpWebRequest;
            request.Credentials = new NetworkCredential(Uide, Pas);
            request.Proxy = null;
            request.KeepAlive = false;
            return request;
        }

        public int SendtOFR_as_FTP(byte[] data, string FileName)
        {
            string UserName = EncodeString(Upr_User.User_PH);
            int e = 0;
            do
            {
                try
                {
                    if (data is null)
                        break;

                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Strim_URL + "/BazaOfr/" + FileName);

                    request.Credentials = new NetworkCredential(Uide, Pas);
                    request.Method = WebRequestMethods.Ftp.AppendFile;
                    request.ContentLength = data.Length;
                    using (WebResponse myResponse = request.GetResponse())
                    {
                        try
                        {
                            using (Stream requestStream = request.GetRequestStream())
                            {
                                requestStream.Write(data, 0, data.Length);
                            }
                        }
                        catch (WebException webEx)
                        {
                            webEx.Response.Close();
                        }
                    }
                    request.Abort();

                    e = 0;
                }
                catch (Exception ex)
                {
                    TextMessage(Strim_URL + UserName + "/BazaOfr/" + FileName + "/ - błąd ftp" + ex.StackTrace.ToString());
                }
            }
            while (false);
            // Console.WriteLine("end SendtOFR_as_FTP")
            return e;
        }

        internal int OpenPdf_mail(string FilePdf, string NameFile)
        {
            try
            {
                int e = 0;
                int LabKO = LiczPonCenaKO("KO", "CenaDoOFR", true);
                string InfoProd = LiczPonCenaKO("KO", "CenaDoOFR", false).ToString();
                var o_Date = DateTime.Today;
                var xOutlookObj = new Microsoft.Office.Interop.Outlook.Application();
                var xEmailObj = (Microsoft.Office.Interop.Outlook.MailItem)(xOutlookObj.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem));

                if (Upr_User.WyslijInfoDoKO == true)
                {
                    if (LabKO > 0)
                    {
                        string SubEma = Upr_User.User_PH + "Poniżej KO - klient " + Get_KlientDane.Numer_konta;
                        string Body = Upr_User.User_PH + " aktualnie wysyła ofertę poniżej ceny KO " + Constants.vbCrLf + "Ilość ofertowanych produktów poniżej KO  -  " + LabKO + Constants.vbCrLf + Constants.vbCrLf + InfoProd;
                        TextMessageFileAndStart(Upr_User.KO_email, "", Get_KlientDane.BranzystaEmail.ToString() + ";" + TestEma + ";", SubEma, Body, FilePdf, NameFile, true);
                        //Console.WriteLine("Tu email do branz + KO");
                    }
                }

                {
                    ref var withBlock = ref xEmailObj;
                    var Obody = withBlock;
                    withBlock.Display();
                    withBlock.To = SendToEmail;
                    if ((Strings.Replace(Upr_User.KO_email, " ", "") ?? "") != (Strings.Replace(Get_KlientDane.BranzystaEmail, " ", "") ?? ""))
                        withBlock.CC = Upr_User.KO_email + ";" + Get_KlientDane.BranzystaEmail;
                    else
                        withBlock.CC = Upr_User.KO_email;
                    withBlock.Subject = NameFile;
                    withBlock.Attachments.Add(FilePdf);

                    foreach (var row in ListTblOfr.Tbl_Add_prodList)
                    {
                        if (row.TDS_DO_OFR.ToString() == "True")
                        {

                            string PlikPdf = Wstaw_Tds_do_Ema("TDS", (byte[])row.TDS, row.NazwaPdf.ToString() + ".Pdf");
                            //string PlikPdf = Wstaw_Tds_do_Ema("TDS", ObjectToByteArray(row.TDS), row.NazwaPdf.ToString() + ".Pdf");
                            withBlock.Attachments.Add(PlikPdf);
                            row.TDS_DO_OFR = false;
                        }
                    }
                    foreach (var row in ListTblOfr.Tbl_Add_prodList)
                    {
                        if (row.CHAR_DO_OFR.ToString() == "True")
                        {
                            string PlikPdf = Wstaw_Tds_do_Ema("CHAR", (byte[])row.KCH, row.NazwaPdf.ToString() + ".Pdf");
                           // string PlikPdf = Wstaw_Tds_do_Ema("CHAR", ObjectToByteArray(row.KCH), row.NazwaPdf.ToString() + ".Pdf");
                            withBlock.Attachments.Add(PlikPdf);
                            row.TDS_DO_OFR = false;
                        }
                    }
                    withBlock.Display();
                }
                if (xEmailObj == null)
                {
                    xEmailObj.Display();
                }
                xEmailObj = null;
                xOutlookObj = null;
                e = 1;
                return e;
            }
            catch
            { 
                return 0; 
            }
        }
        internal string Wstaw_Tds_do_Ema(string TypPLK, byte[] fileData, string NameFile)
        {
            string sTempFileName = null;

            sTempFileName = System.IO.Path.Combine(Scieżka_Pliku_AppData_FilesSC, TypPLK + "_" + NameFile);
            if (sTempFileName.Contains(".Pdf") == false)
                sTempFileName += ".Pdf";
            //using (var FS = new FileStream(sTempFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var FS = new FileStream(sTempFileName, FileMode.OpenOrCreate))
            {
                FS.Write(fileData, 0, fileData.Length);
                FS.Position = 0L;
                
                //FS.Close();
            }

            //sTempFileName = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, nameFile);
            //if (sTempFileName.Contains(".Pdf") == false)
            //    sTempFileName += ".Pdf";
            //using (var FS = new FileStream(sTempFileName, FileMode.Create)) // sTempFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write)
            //{
            //    FS.Write(fileData, 0, fileData.Length);
            //    FS.Position = 0L;
            //    FS.Close();
            //}





            Console.WriteLine(sTempFileName);
            return sTempFileName;
        }

        private int SawedPdf(string sSap, string sFilePath, string sFileName)
        {
            int e = 0;
            try
            {
                byte[] PTds;
                using (var fs = new FileStream(sFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var br = new BinaryReader(fs))
                    {
                        PTds = br.ReadBytes((int)fs.Length);
                    }
                }
                string Tim;
                Tim = TimeAktual();
                if (con.State == ConnectionState.Closed)
                    con.Open();
                cmd.CommandText = @" -- Try to update any existing row
                                    UPDATE TblPdf
                                    SET SAP =@SP ,NrOFR=@NO,PlkPdf=@PPdf,OstAkt=@OstAkt
                                    WHERE NrOFR like '%" + sFileName + @"%';  
                                -- If no update happened (i.e. the row didn't exist) then insert one
                                    INSERT INTO TblPdf  (SAP, NrOFR, PlkPdf,OstAkt)
                                    SELECT  @SP,@NO,@PPdf,@OstAkt
                                     WHERE (Select Changes() = 0);";
                cmd.Parameters.Add("@SP", (DbType)SqlDbType.VarChar).Value = sSap;
                cmd.Parameters.Add("@NO", (DbType)SqlDbType.VarChar).Value = sFileName;
                cmd.Parameters.Add("@PPdf", (DbType)SqlDbType.Binary).Value = PTds;
                cmd.Parameters.Add("@OstAkt", (DbType)SqlDbType.VarChar).Value = Tim;
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
                if (con.State == ConnectionState.Open)
                    con.Close();
                PTds = null;
                e = 1;
            }
            catch (Exception ex)
            {
                TextMessage(sFilePath + "/ - scieżka" + ex.StackTrace.ToString());
            }
            // Console.WriteLine("SawedPdf")
            return e;
        }

        private void ComboPlac_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboPlac.SelectedIndex >= -1)
                LabInfo.Content = "";
        }
        private void Update_FormPlac()
        {
            try
            {
                Get_KlientDane.E_mail = Textemail.Text.ToString();
                Get_KlientDane.Forma_plac = ComboPlac.Text.ToString();
                string StringSql = "update BazaKl set Forma_plac='" + ComboPlac.Text.ToString() + "',E_mail='" + Textemail.Text.ToString() + "' where NIP =" + Get_KlientDane.NIP + " ";
                if (con.State == ConnectionState.Closed)
                    con.Open();
                UsingSQLComand(StringSql, con);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            if (Textemail.Text == "")
                Textemail.Background = System.Windows.Media.Brushes.Red;
        }

        private void Button2_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var AddTxt = new TextBox()
                {
                    Name = "TxtInf" + indexTxt,
                    Tag = indexTxt,
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    FontStyle = FontStyles.Normal,
                    Text = "",
                    TextWrapping = TextWrapping.Wrap,
                    AcceptsReturn = true,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                    Width = 600,
                    Margin = new Thickness(0, 5, 5, 5)
                };
                var img = new Image() { Name = "imgInf" + indexTxt, Width = 15, Height = 15, Margin = new Thickness(640, -50, 0, 0), Source = new BitmapImage(new Uri("pack://application:,,,/images/Clear.png")), Tag = indexTxt };
                img.MouseDown += Clear_Uwagi;
                AddTxtblock.Children.Add(AddTxt);
                AddTxtblock.Children.Add(img);
                AddTxtblock.Visibility = Visibility.Visible;
                indexTxt += 1;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Clear_Uwagi(object sender, MouseButtonEventArgs e)
        {
            int tg = int.Parse(((Image)sender).Tag.ToString());
        line1:
            ;
            foreach (object ctr in AddTxtblock.Children)
            {
                if (int.Parse(((StackPanel)ctr).Tag.ToString()) == tg)
                {
                    AddTxtblock.Children.Remove((UIElement)ctr);
                    goto line1;
                }

            }
        }
        private void TextPDf_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int St = TextPDf.SelectionStart;
                TextPDf.Text = EncodeString(TextPDf.Text);
                Serchtext();
                TextPDf.SelectionStart = St;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void Serchtext()
        {
            try
            {
                string TxtStr = TextPDf.Text.ToString() + ".Pdf";
                string str = "Plik o takiej nazwie już istnieje." + Constants.vbCrLf + "Wysyłając ofertę nadpiszesz poprzednią ofertę.";
                if (System.IO.Directory.Exists(SciezkaPliku_Do_DOWork_PDF + @"\" + TxtStr))
                {
                    LabInfo.Content = "Nazwa chwilowo niedostępna. Zmień nazwę !!!!";
                    TextPDf.Foreground = System.Windows.Media.Brushes.Red;
                    LabInfo.Foreground = System.Windows.Media.Brushes.Red;
                    Button1.Visibility = Visibility.Collapsed;
                    return;
                }

                if (dtPDF.Columns.Count >= 0)
                {
                    for (int i = 0, loopTo = dtPDF.Rows.Count - 1; i <= loopTo; i++)
                    {
                        if ((dtPDF.Rows[i]["NrOFR"].ToString() ?? "") == (TxtStr ?? ""))
                        {
                            LabInfo.Content = str;
                            TextPDf.Foreground = System.Windows.Media.Brushes.Red;
                            LabInfo.Foreground = System.Windows.Media.Brushes.Red;
                            break;
                        }
                        else
                        {
                            LabInfo.Content = "";
                            TextPDf.Foreground = System.Windows.Media.Brushes.Black;
                            LabInfo.Foreground = System.Windows.Media.Brushes.Black;
                            Button1.Visibility = Visibility.Visible;
                        } // = True
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void SprDuplikat(string TxtPlik)
        {
            try
            {
                dtPDF.Clear();
                if (con.State == ConnectionState.Closed)
                    con.Open();
                string sqlstring = "SELECT SAP,NrOFR  From TblPdf Where  SAP Like'%" + TxtPlik + "%'";
                dtPDF = SqlComandDatabase(sqlstring, con);
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void UClear_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                RemoveMe();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void RemoveMe()
        {
            try
            {
                DockPanel Par = (DockPanel)this.Parent;
                Par.Visibility = Visibility.Collapsed;
                Par.Children.Remove(this);
                Mw.VievPageVisibli(false, false, "");
            }
            catch (Exception ex)
            {
                // TextMessage( ex.StackTrace.ToString)
            }
        }

        private void Textemail_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (((TextBox)sender).Text.ToString().Length >= 0)
                {
                    ((TextBox)sender).Background = new SolidColorBrush(Colors.White);
                    ((TextBox)sender).Foreground = System.Windows.Media.Brushes.Black;
                    Label5.Foreground = System.Windows.Media.Brushes.Black;
                }
                else
                {
                    ((TextBox)sender).Background = new SolidColorBrush(Colors.White);
                    ((TextBox)sender).Foreground = System.Windows.Media.Brushes.Red;
                    Label5.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private string NameFile(string name)
        {
            try
            {
                var o_Date = DateTime.Now;
                name = EncodeString(name);
                return name + "-" + o_Date.ToString("yy_MM_dd");
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }

    }
}
