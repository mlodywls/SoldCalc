using Microsoft.VisualBasic;
using SoldCalc.Supporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using ListView = System.Windows.Controls.ListView;

namespace SoldCalc
{
    public partial class PagePH : Page
    {
        private System.Data.DataTable TBLPH = new System.Data.DataTable();
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            DowBaza();
        }
        public PagePH()
        {

            InitializeComponent();

            try
            {
                DowBaza();
                PHList.ItemsSource = ListPH;
            }
            catch (Exception ex)
            {
                TextMessage(ex.ToString());
            }

            InfoSend.Content = "";
            InfoSend.Background = new SolidColorBrush(Colors.Transparent);

        }
        private void DowBaza()
        {
            URLstatus = FVerificaConnessioneInternet();
            if (URLstatus == true)
            {
                Pobierz_baze_DB_FTP(NEWFilePH, LocatiAktual, default);
            }
            else
            {
                Interaction.MsgBox("brak połaczenia z internetem" + Microsoft.VisualBasic.Constants.vbCrLf + " Sprawdz połączenie!");
                return;
            }
            WyswietlPH();
        }
        private void WyswietlPH()
        {
            TBLPH.Clear();
            if (PHDcon.State == ConnectionState.Closed)
                PHDcon.Open();
            TBLPH = SqlComandDatabase(StringComand.ReturnComndGeTLogUser(), PHDcon);
            GetDataPH(SqlComandDatabase(StringComand.ReturnComndGeTLogUser(), PHDcon));
            if (PHDcon.State == ConnectionState.Open)
                PHDcon.Close();
            PHList.ItemsSource = ListPH;
        }

        public List<ListaUser> ListPH { get; set; }
        public void GetDataPH(System.Data.DataTable Baza)
        {
            ListPH = new List<ListaUser>();
            foreach (DataRow row in Baza.Rows)
            {
                if (row["Rejon"].ToString() != "")
                {
                    RejPh = row["Rejon"].ToString();
                }
                ListPH.Add(new ListaUser()
                {
                    Id = row["Id"].ToString(),
                    Ranga = row["Ranga"].ToString(),
                    Rejon = RejPh,
                    ostLog = row["ostLog"].ToString(),
                    Imie = row["Imie"].ToString(),
                    Nazwisko = row["Nazwisko"].ToString(),
                    Telefon = row["Telefon"].ToString(),
                    Email = row["Email"].ToString(),
                    KO = row["KO"].ToString(),
                    CenaKO = bool.Parse(row["CenaKO"].ToString()),
                    WyślijInfoDoKO = bool.Parse(row["WyślijInfoDoKO"].ToString()),
                    MonitKO = bool.Parse(row["MonitKO"].ToString()),
                    Upr4 = bool.Parse(row["Upr4"].ToString()),
                    NrPh = row["NrPh"].ToString()
                });

            }
        }


        private void PHList_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            InfoSend.Content = "";
            var item = (ListaUser)((ListView)sender).SelectedItem;
            if (item is null)
                return;
            if (item.Ranga == "KO")
            {
                Ch1.IsChecked = true;
                Lab1.Content = item.Ranga;
            }
            else
            {
                Ch1.IsChecked = false;
                Lab1.Content = item.Ranga;
            }
            if (item.Ranga == "Blok")
            {
                Blokada.IsChecked = true;
                Upr.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Blokada.IsChecked = false;
                Upr.Background = new SolidColorBrush(Colors.Transparent);
            }

            Imie.Text = item.Imie; 
            Nazwisko.Text = item.Nazwisko; 
            Tel.Text = item.Telefon; 
            Email.Text = item.Email; 
            KO.Text = item.KO; 
            LabId.Content = item.Id;

            Ch2.IsChecked = item.CenaKO;
            Lab2.Content = item.CenaKO; 
            Ch3.IsChecked = item.WyślijInfoDoKO;
            Lab3.Content = item.WyślijInfoDoKO; 
            Ch4.IsChecked = item.MonitKO;
            Lab4.Content = item.MonitKO; 
            Ch5.IsChecked = item.Upr4;
            Lab5.Content = item.Upr4; 
            Rejon.Text = item.Rejon;
        }

        private void Usun_Click(object sender, RoutedEventArgs e)
        {
            string searchQueryP;
            if (PHDcon.State == ConnectionState.Closed)
                PHDcon.Open();
            searchQueryP = "delete from TblUser where Id LIKE " + LabId.Content.ToString() + "";
            Console.WriteLine(searchQueryP);
            UsingSQLComand(searchQueryP, PHDcon);
            if (PHDcon.State == ConnectionState.Open)
                PHDcon.Close();
            ConClose();
            SendDownload(NEWFilePH, DownloadPHh);
            DowBaza();
            Interaction.MsgBox("wysłano");
        }

        private void Zapis_Click(object sender, RoutedEventArgs e)
        {
            if (LabId.Content.ToString() != "")
            {
                string searchQueryP;
                if (PHDcon.State == ConnectionState.Closed)
                    PHDcon.Open();
                if (Blokada.IsChecked == true)
                    Lab1.Content = "Blok";
                else
                {
                    if (Ch1.IsChecked == true)
                        Lab1.Content = "KO";
                    else
                        Lab1.Content = "PH";
                }
                searchQueryP = @" UPDATE TblUser
                                    SET Ranga = '" + Lab1.Content + "',Imie='" + Imie.Text + "',Nazwisko='" + Nazwisko.Text + "',Telefon='" + Tel.Text + "',Email='" + Email.Text + "',KO='" + KO.Text + @"'
                                    ,CenaKO='" + Lab2.Content + "',WyślijInfoDoKO='" + Lab3.Content + "',MonitKO='" + Lab4.Content + "',Upr4='" + Lab5.Content + "',Rejon ='" + Rejon.Text + @"'                                                                     
                                    where Id LIKE " + LabId.Content + "";
                // Console.WriteLine(searchQueryP)
                UsingSQLComand(searchQueryP, PHDcon);
                if (PHDcon.State == ConnectionState.Open)
                    PHDcon.Close();
                ConClose();
                SendDownload(NEWFilePH, DownloadPHh);
                DowBaza();
            }
            else
            {
                Console.WriteLine("PHList.SelectedIndex = null");
            }
        }



        private void Check()
        {
            if (Blokada.IsChecked == true)
            {
                if (Ch1.IsChecked == true)
                    Ch1.IsChecked = false;
                Lab1.Content = "Blok";
                Upr.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Lab1.Content = "";
                Upr.Background = new SolidColorBrush(Colors.LightBlue);
            }
            if (Lab1.Content.ToString() != "Blok")
            {
                if (Ch1.IsChecked == true)
                {
                    Lab1.Content = "KO";
                    Blokada.IsChecked = false;
                }
                else
                {
                    Lab1.Content = "PH";
                    if (Blokada.IsChecked == true)
                        Blokada.IsChecked = false;
                }
            }
            else
            {
          
            }
            if (Lab1.Content.ToString() == "")
                Upr.Background = new SolidColorBrush(Colors.LightBlue);

            Lab2.Content = Ch2.IsChecked.ToString();
            Lab3.Content = Ch3.IsChecked.ToString();
            Lab4.Content = Ch4.IsChecked.ToString();
            Lab5.Content = Ch5.IsChecked.ToString();
        }

        private void Ch1_Checked(object sender, RoutedEventArgs e)
        {
            Check();
        }

        private void Ch1_Unchecked(object sender, RoutedEventArgs e)
        {
            Check();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete(AktualFullPath);
            }
            catch
            {

            }
            try
            {
                File.Delete(DownloadFullPath);
            }
            catch
            {

            }
        }

        private void lvw_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {

        }

        private void KopiaZapas_Click(object sender, RoutedEventArgs e)
        {
            Transfer_FTP_file();
        }

    }

    public partial class ListaUser
    {
        public string Id { get; set; }
        public string Ranga { get; set; }
        public string Rejon { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string KO { get; set; }
        public bool CenaKO { get; set; }
        public bool WyślijInfoDoKO { get; set; }
        public bool MonitKO { get; set; }
        public bool Upr4 { get; set; }
        public string NrPh { get; set; }
        public string ostLog { get; set; }

    }
}
