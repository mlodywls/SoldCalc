using SoldCalc.Controls;
using SoldCalc.Login;
using SoldCalc.Supporting;
using SoldCalc.UpdateWorker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc
{
    public partial class MainWindow : Window
    {
        public static UPR_Ranga Upr_User { get; set; }

        public string MWTitle;
        public static LiczOferta LiczOfr;
        public static Panel PnStart;
        public bool startKlient = false;
        public static DockPanel Dock_Aktual_Progre;
        public static DockPanel Dock_VievPage;

        public static StackPanel WyslProcent;
        public static StackPanel InfoStackPanelkryj;
        public static Label lblTime;
        public static ProgressBar Dock_Aktual_LabProgr;
        public static ProgressBar AktualBazaProgre;
        public static ProgressBar SendtBazaProgre;

        public static StackPanel StAktua;
        public static Label LabProgre;
        public static Label LabNazawaDzialani;
        public static Label LabNazawaAktua;
        public static Label labelProgres;
        public static Label SendtLabProgre;
        public static Label InfoLabelKryj;
        public static Label LabInfoSen;
        public static Image PodgladHis;
        public static Image ShowOF;
        public static Image ShowCenni;
        public static Label LabIleZ;
        public static string dowsise;

        public bool SerchCennik = false;
        public string ErrNamber = null;

        public List<DaneKlient> ListKlient { get; set; }
        public List<CennikData> ListCennik { get; set; }
        public List<HistOfSeals> ListHistSels { get; set; }
        public List<OFRData> ListOFR { get; set; }

        public DaneKlient EditKlientDane = new DaneKlient();

        public string NameImgstring = "";

        public int licz = 0;
        public int LDLicz = 0;

        public Label NameLab = default;
        public string NameLabString = null;
        public Image NameIMG = default;
        public string NameLabel;
        public string ComboPlacText = null;
        public string TIleDniText;

        public int DwMath = 0;
        public MainWindow()
        {
            InitializeComponent();

            Uprawnienia_Obiect();
            BlokujProgram.Visibility = Visibility.Visible;
            new Connect();
            StPH.DataContext = Upr_User;
            slider.DataContext = Upr_User;
            MainWindowBorder.DataContext = Upr_User;
            parentContainer.DataContext = Upr_User;
            St_TestScale.DataContext = Upr_User;
            if (Upr_User.User_PH != "")
            {
                Ukryj_Panele();
                BlokClose = true;
            }
            new UpdateApp();
        }
        public void VisibilityBlockApp()
        {
            if (Upr_User.Ranga != "Blok")
                Mw.BlokujProgram.Visibility = Visibility.Collapsed;
            else
                Mw.BlokujProgram.Visibility = Visibility.Visible;
        }
        public void StartApp()
        {
            if (Upr_User.Ranga == "Blok")
                BlokujProg();
            else
            {
                if (Upr_User.User_PH == default || Upr_User.User_PH == "")
                {
                    AktualNwwBaza_PH.StartUpdatePH();
                    Mw.Panel_Pierwsze_Logowanie.Margin = new Thickness(0, 0, 0, 0);
                    Mw.Panel_Pierwsze_Logowanie.Children.Add(new UserLog());
                    Mw.Panel_Pierwsze_Logowanie.Visibility = Visibility.Visible;
                }
                else
                {

                    Panel_Pierwsze_Logowanie.Margin = new Thickness(0, 0, 0, 0);
                    Panel_Pierwsze_Logowanie.Children.Add(new LoadingData());
                    Panel_Pierwsze_Logowanie.Visibility = Visibility.Visible;

                }
            }
            if (PHcon.State == ConnectionState.Open)
                PHcon.Close();
            if (PHDcon.State == ConnectionState.Open)
                PHDcon.Close();
        }

        private void Uprawnienia_Obiect()
        {
            var MainW = this;
            Mw = MainW;
            MWTitle = this.Title;
            PnStart = Panel_Pierwsze_Logowanie;
            WyslProcent = WyswietlProcent;
            Dock_Aktual_Progre = Dock_Aktual_Progres;
            Dock_VievPage = VievPage;
            InfoStackPanelkryj = Infokryj;
            lblTime = lblTim;
            LabIleZ = LabIle;
            Dock_Aktual_LabProgr = Progres;
            LabInfoSen = LabInfoSend;

            AktualBazaProgre = AktualBazaProgres;
            SendtBazaProgre = SendtBazaProgres;
            LabNazawaAktua = LabNazawaAktual;

            LabNazawaDzialani = LabNazawaDzialanie;
            SendtLabProgre = SendtLabProgres;
            PodgladHis = PodgladHist;
            PodgladHis.Visibility = Visibility.Collapsed;
            ShowOF = ShowOFR;
            ShowOF.Visibility = Visibility.Collapsed;
            ShowCenni = ShowCennik;
            ShowCenni.Visibility = Visibility.Collapsed;
            StAktua = StAktual;
            LabProgre = LabProgres;
            InfoLabelKryj = Info_Gif_Czekaj;
            labelProgres = LblProgres;
            StAktua.Width = 35;
        }
        private void Ukryj_Panele()
        {
            WyswietlProcent.Visibility = Visibility.Collapsed;
            UpdateMe.Visibility = Visibility.Collapsed;
            VievPage.Visibility = Visibility.Collapsed;
            PageClear.Visibility = Visibility.Collapsed;
            if (Upr_User.UprKO == false)
                ukrujKO = 0;
            else
                ukrujKO = 80;
        }
        private void BlokujProg()
        {
            try
            {
                BlokujProgram.Width = 5000;
                BlokujProgram.Height = 5000;
                BlokujProgram.Visibility = Visibility.Visible;
                BlokujProgram.Opacity = 0.9d;
                LabBlok.Content = "Wyłączono uprawnienia programu !!!!!!";
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        internal static void UkryjPanel()
        {
            try
            {
                PnStart.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void Reflash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Reflash_App();
        }

        public void Reflash_App()
        {
            Upr_User = ConnectUser.LoadUpr_ranga();
            StPH.DataContext = Upr_User;

            //MessageBox.Show(Connect.Upr_User.User_PH);
            int a = int.Parse(Rmany.Content.ToString());
            a += 1;
            Rmany.Content = a;
            Console.WriteLine("Reflash_MouseDown");
            StartApp();
        }

        public void VievPageVisibli(bool Aktual_DocPanel, bool ViexPage, string ukryj)
        {
            Dispatcher.Invoke(() =>
            {
                try
            {
                if (Aktual_DocPanel == true) // - aktualizacja z znacznikiem Progrsbar
                {
                    Dock_Aktual_Progre.Visibility = Visibility.Visible;
                    Dock_Aktual_Progre.Opacity = 0.8d; // Dock_Aktual_Progres
                    WaitImage.Visibility = Visibility.Visible;  // dock panel Dock_Aktual_Progres - gif czekaj
                    Info_Gif_Czekaj.Visibility = Visibility.Visible; // label Content=" Czekaj " w Image_wait / Dock_Aktual_Progres
                    if (NameImgstring == "PiZK11")
                    {
                        PageCleardock.Visibility = Visibility.Collapsed; // label Content=" Czekaj " w Image_wait / Dock_Aktual_Progres
                        Infokryj.Visibility = Visibility.Collapsed; // Pasek progres do aktualizacji z plików excel
                    }
                    else
                    {
                        PageCleardock.Visibility = Visibility.Visible; // label Content=" Czekaj " w Image_wait / Dock_Aktual_Progres
                        Infokryj.Visibility = Visibility.Visible;
                    } // Pasek progres do aktualizacji z plików excel
                }
                else
                {
                    Dock_Aktual_Progre.Visibility = Visibility.Collapsed;
                    Dock_Aktual_Progre.Opacity = 1; // Dock_Aktual_Progres
                    WaitImage.Visibility = Visibility.Collapsed;   // dock panel Dock_Aktual_Progres - gif czekaj
                    Info_Gif_Czekaj.Visibility = Visibility.Collapsed; // label Content=" Czekaj " w Image_wait / Dock_Aktual_Progres
                    PageCleardock.Visibility = Visibility.Collapsed;
                }

                if (ViexPage == true)
                {
                    VievPage.Visibility = Visibility.Visible;
                    PageCleardock.Visibility = Visibility.Collapsed;
                    if (ukryj == "1")
                    {
                        InfoLab.Content = "Czekaj, wgrywam dane !!!";
                    }

                }

                else
                {
                    VievPage.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                // MsgBox(ex.StackTrace.ToString)
                TextMessage(ex.StackTrace.ToString());
            }
            if (ukryj == "kryj")
                Image_wait.Visibility = Visibility.Collapsed;
            else
                Image_wait.Visibility = Visibility.Visible;
            });
        }

        internal void ClearImg()
        {
        line1:
            ;
            foreach (var ctr in VievPage.Children)
            {
                if (ctr is Image)
                {
                    Image img1 = (Image)ctr;
                    if (img1.Name.ToString() != "PageClear")
                    {
                        VievPage.Children.Remove(img1);
                        goto line1;
                    }
                }
            }
        }























        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Upr_User.Scale = Math.Round(this.ActualWidth / 2000, 1);
            Mw.slider.Value = Upr_User.Scale;
        }

        private void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Handled = true;
        }

        private void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            System.Windows.Shapes.Rectangle rectToMove = e.OriginalSource as System.Windows.Shapes.Rectangle;
            Matrix rectsMatrix = ((MatrixTransform)rectToMove.RenderTransform).Matrix;
            rectsMatrix.RotateAt(e.DeltaManipulation.Rotation, e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
            rectsMatrix.ScaleAt(e.DeltaManipulation.Scale.X, e.DeltaManipulation.Scale.X, e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
            rectsMatrix.Translate(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
            rectToMove.RenderTransform = new MatrixTransform(rectsMatrix);
            var containingRect = new Rect(((FrameworkElement)e.ManipulationContainer).RenderSize);
            Rect shapeBounds = rectToMove.RenderTransform.TransformBounds(new Rect(rectToMove.RenderSize));
            if (e.IsInertial && !containingRect.Contains(shapeBounds))
            {
                e.Complete();
            }
            e.Handled = true;
        }

        private void Window_InertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 10.0d * 96.0d / (1000.0d * 1000.0d);
            e.ExpansionBehavior.DesiredDeceleration = 0.1d * 96d / (1000.0d * 1000.0d);
            e.RotationBehavior.DesiredDeceleration = 720d / (1000.0d * 1000.0d);
            e.Handled = true;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ConClose();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWindowHeight = e.NewSize.Height;
            double newWindowWidth = e.NewSize.Width;
            double prevWindowHeight = e.PreviousSize.Height;
            double prevWindowWidth = e.PreviousSize.Width;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

            if (WindowState == WindowState.Maximized)
            {
                MainWindowBorder.BorderThickness = new Thickness(8);
                RestoreButton.Visibility = Visibility.Visible;
                MaximizeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainWindowBorder.BorderThickness = new Thickness(0);
                RestoreButton.Visibility = Visibility.Collapsed;
                MaximizeButton.Visibility = Visibility.Visible;
            }


        }


        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }
        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }
        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void User_ph_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (StPH.Visibility == Visibility.Visible)
                StPH.Visibility = Visibility.Collapsed;
            else
                StPH.Visibility = Visibility.Visible;
        }

        private void User_ph_MouseEnter(object sender, MouseEventArgs e)
        {
            StPH.Visibility = Visibility.Visible;
        }

        private void Client_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Main.Content = new Klient();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void AddClient_MouseDown(object sender, MouseButtonEventArgs e)
        {
            {
                try
                {

                    ClearImg();
                    var DocPan = new DockPanel();
                    DocPan.HorizontalAlignment = HorizontalAlignment.Center;
                    DocPan.VerticalAlignment = VerticalAlignment.Center;
                    DocPan.Children.Add(new EdytujZmienDane(false));
                    DocPan.Background = new SolidColorBrush(Colors.LightGray);
                    VievPage.Children.Add(DocPan);
                    VievPageVisibli(true, true, "kryj");
                }
                catch (Exception ex)
                {
                    TextMessage(ex.StackTrace.ToString());
                }
            }

        }

        private void PodgladHist_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = new HistZak();
        }
        private void WyslijEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearImg();
            Main.Content = new EmailShow();
        }
        private void ShowCennik_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = new CenniPoPrace();
        }

        private void ShowOFR_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Main.Content = new Okno_podgl_OFR();
        }





        private void MWCzysc_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LiczOfr.Czysc_OFR();
            LiczOfr.ZwinLab("rozwiń");
            string sqldel = "delete From BazaOfr_robocze WHERE NIP like '%" + Get_KlientDane.NIP + "%';";
            LiczOfr.Delete_row_LiczOfr_Tbl_selectedIndex(Get_KlientDane.NIP, ((Image)sender).Name);

            UsingSQLComand(sqldel, con);
        }

        private void SaweOfr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ListTblOfr.Tbl_Add_prodList.Count >= 0)
                {
                    LiczOfr.ZapiszRobocza();
                    LiczOfr.WczytajZapisProd();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void WyswietlOfr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VievWind_of_Html();
        }
        [STAThread]
        internal void VievWind_of_Html()
        {
            string Filesave;
            string nameFile = EncodeString(Get_KlientDane.Nazwa_klienta + " " + Get_KlientDane.Nazwa_CD + ".Pdf");
            string Filestring = System.IO.Path.Combine(Scieżka_Pliku_AppData_FilesSC, nameFile);
            if (Wind_of_Html_Add is null)
            {
                Window window = new Wind_of_Html();
                window.Show();

                Filesave = Modul_ItxPDF.GenerujOferta_tymczasowa_PDF(nameFile, Filestring, default, default);
                byte[] PTds = File.ReadAllBytes(Filesave);
                Wind_of_Html_Add.Generuj_TDS_Tabela(PTds, Get_KlientDane.Numer_konta); // Mw_nrkonta.Text)
            }
            else
            {
                Filesave = Modul_ItxPDF.GenerujOferta_tymczasowa_PDF(nameFile, Filestring, default, default);
                byte[] PTds = File.ReadAllBytes(Filesave);
                Wind_of_Html_Add.Generuj_TDS_Tabela(PTds, Get_KlientDane.Numer_konta); // Mw_nrkonta.Text)
                Wind_of_Html_Add.Focus();
            }
        }

        private void WyswietlRobocze_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LiczOfr.Add_list_to_robocze();
        }

        private void Menu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Upr_User.Admin == true)
                Main.Content = new PagePH();
        }


        private void PmessagePdf_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ListTblOfr.Tbl_Add_prodList.Count >= 0)
            {
                ClearImg();
                var DocPan = new DockPanel();
                DocPan.HorizontalAlignment = HorizontalAlignment.Center;
                DocPan.VerticalAlignment = VerticalAlignment.Center;
                DocPan.Children.Add(new ZapiszPDFvb());
                DocPan.Background = new SolidColorBrush(Colors.LightGray);
                VievPage.Children.Add(DocPan);
                VievPageVisibli(true, true, "kryj");
            }
        }

        private void PiZK11_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                NameLab = (Label)sender;
            }
            catch (Exception ex)
            {
                NameIMG = (Image)sender;
                NameImgstring = ((Image)sender).Name;
            }
            Mw.DwMath = 0;
            Info_Gif_Czekaj.Content = "Czekaj";
            VievPageVisibli(true, false, "");

            new MoreFunctionMW();

        }
        private void DowCen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Upr_User.Ranga == "KO" || Upr_User.Rejon == "Admin")
            {
                NameLabel = ((Image)sender).Name;
                VievPageVisibli(true, false, "");
                try
                {
                    new MoreFunctionMW();
                }
                catch
                {
                    VievPageVisibli(true, true, "");
                }
            }
        }

        private void Aktual_Baza_PH_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NameLabel = ((Label)sender).Name.ToString();
            Mw.DwMath = 0;
            VievPageVisibli(true, false, "");
            new MoreFunctionMW();

        }

        private void Addwindows1_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }




        // Update DataBase con/DOwnload Con FTP ------- >>>   Start


        private void Update_App_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string StartFile = Folder_Matka_Programu_FilesSC + @"\Update\";
                var objProcess = new Process();
                objProcess.StartInfo.FileName = Folder_Matka_Programu + @"Update\UpdateWinCalc.exe";
                objProcess.Start();
                Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Labkryj_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (StAktual.Width == 35)
            {
                StAktual.Width = 280;
                Labkryj.Content = "<";
            }
            else
            {
                StAktual.Width = 35;
                Labkryj.Content = ">";
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((CheckBox)sender).Name.ToString() == "AktImg")
                    AktIMGStart = bool.Parse(((CheckBox)sender).IsChecked.ToString());
                if (((CheckBox)sender).Name == "AktTDS")
                    AktTDSStart = bool.Parse(((CheckBox)sender).IsChecked.ToString());
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Aktualizuj_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                UruchomAktual.Visibility = Visibility.Visible;
                Main.Opacity = 0.3d;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Aktualizuj_Baza_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                UruchomAktual.Visibility = Visibility.Collapsed;
                Main.Opacity = 1;
                NEW_UpdateURL();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Cancel_Aktuaalizuj_baza_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                UruchomAktual.Visibility = Visibility.Collapsed;
                Main.Opacity = 1;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void PageClear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VievPageClearDocpane();
        }

        // Update DataBase con/DOwnload Con FTP ------- >>>   End





        public void VievPageClearDocpane()
        {
            ClearImg();
            if (Dock_Aktual_Progres.Visibility == Visibility.Visible)
            {
                Dock_Aktual_Progres.Visibility = Visibility.Collapsed;
            }
            if (VievPage.Visibility == Visibility.Visible)
            {
                VievPage.Visibility = Visibility.Collapsed;
            }
        }


        private void User_ph_MouseLeave(object sender, MouseEventArgs e)
        {
            StPH.Visibility = Visibility.Collapsed;
        }



        private void Label_Minus(object sender, MouseButtonEventArgs e)
        {
            Mw.slider.Value = Math.Round(Mw.slider.Value - 0.1d, 2, MidpointRounding.AwayFromZero);
            Upr_User.Scale = Math.Round(Upr_User.Scale - 0.1d, 2, MidpointRounding.AwayFromZero);
        }

        private void Label_Plus(object sender, MouseButtonEventArgs e)
        {
            Mw.slider.Value = Math.Round(Mw.slider.Value + 0.1d, 2, MidpointRounding.AwayFromZero);
            Upr_User.Scale = Math.Round(Upr_User.Scale + 0.1d, 2, MidpointRounding.AwayFromZero);
        }

        private void WyswietlCen_Checked(object sender, RoutedEventArgs e)
        {
            //Upr_User.WyswCennikAdmin = WyswietlCen.IsChecked.Value;
            //Lab_Checked.Content = Upr_User.WyswCennikAdmin;
        }

        private void WyswietlCen_Unloaded(object sender, RoutedEventArgs e)
        {
            //Upr_User.WyswCennikAdmin = WyswietlCen.IsChecked.Value;
            //Lab_Checked.Content = Upr_User.WyswCennikAdmin;

        }
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        private void ConsoleOpen_Click(object sender, RoutedEventArgs e)
        {
            AllocConsole();
            Console.WriteLine("test");
        }
    }
}
