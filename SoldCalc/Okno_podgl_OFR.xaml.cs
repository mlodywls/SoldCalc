using SoldCalc.Supporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.SupportingFunctions;
using DataTable = System.Data.DataTable;
using ListView = System.Windows.Controls.ListView;

namespace SoldCalc
{

    public partial class Okno_podgl_OFR //: Page
    {
        public Okno_podgl_OFR()
        {
            InitializeComponent();
            Mw.AddKlient.Visibility = Visibility.Collapsed;
            if (Upr_User.UprKO == true)
            {
                LPH.Text = "100";
                LKlient.Text = "200";
                Comb_PH.Visibility = Visibility.Visible;
            }
            else
            {
                LPH.Text = "0";
                LKlient.Text = "0";
                Comb_PH.Visibility = Visibility.Collapsed;
            }

            Lsta_OFR.ItemsSource = Mw.ListOFR; // items
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Lsta_OFR.ItemsSource);
            view.Filter = UserFilter;
            SerhPH_as_combo();
            Comb_Klient.ItemsSource = Serhklient_as_combo("").DefaultView;

        }

        private void SerhPH_as_combo()
        {
            string sqlserch = @"Select md.Opiekun_klienta
                                        from TblPdf si 
                                        LEFT JOIN  BazaKL md ON md.NIP = si.SAP
                                        WHERE si.PlkPdf IS NOT NULL 
										group by  replace(md.Opiekun_klienta, '  ', ' ')";
            Comb_PH.ItemsSource = SqlComandDatabase(sqlserch, con).DefaultView;
        }
        private System.Data.DataTable Serhklient_as_combo(string txt)
        {

            string sqlserch = @"Select md.Nazwa_klienta || ' ' || md.Nazwa_CD as Nazwa_klienta
                                        from TblPdf si 
                                        LEFT JOIN  BazaKL md ON md.NIP = si.SAP
                                        WHERE si.PlkPdf IS NOT NULL AND
                                        md.Opiekun_klienta like '%" + txt + @"%'
										group by  md.NIP ;";

            return SqlComandDatabase(sqlserch, con); // dt
        }




        private void Lsta_OFR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = (OFRData)((ListView)sender).SelectedItem;
                if (item is null)
                    return;
                string DbnullSerh = item.OFR.ToString();

                if (!string.IsNullOrEmpty(DbnullSerh))
                {
                    Labofr.Content = item.NazwOFR;
                    string Name = item.NazwOFR;
                    if (item.OFR != null)
                    {
                        byte[] fileData = (byte[])item.OFR;
                        string sTempFileName = null;
                        sTempFileName = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, item.NazwOFR);
                        if (sTempFileName.Contains(".Pdf") == false)
                            sTempFileName += ".Pdf";
                        try
                        {
                            using (var FS = new FileStream(sTempFileName, FileMode.Create)) // sTempFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write)
                            {
                                FS.Write(fileData, 0, fileData.Length);
                                FS.Close();
                                FS.Dispose();
                            }
                            showOFR.Navigate(sTempFileName);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                {
                    showOFR.Navigate(new Uri("about:blank"));
                    Labofr.Content = " Błąd Pliku !!!";
                }
            }
            catch (Exception ex)
            {

            }
            Lsta_OFR.Focus();
        }


        private int ActivObject = 1;
        private bool UserFilter(object item)
        {
            switch (ActivObject)
            {
                case 1:
                    if (string.IsNullOrEmpty(OfrSerch.Text))
                        return true;
                    else
                        return (item as OFRData).NazwOFR.IndexOf(OfrSerch.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                case 2:
                    if (string.IsNullOrEmpty(Comb_PH.Text))
                        return true;
                    else
                        return (item as OFRData).Opiekun.IndexOf(Comb_PH.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                case 3:
                    if (string.IsNullOrEmpty(Comb_Klient.Text))
                        return true;
                    else
                        return (item as OFRData).NazwKlient.IndexOf(Comb_Klient.Text, StringComparison.OrdinalIgnoreCase) >= 0;

                default: //wstaw wyjatek - info " błędny wybór"
                    break;
            }
            ActivObject = 1;
            return default;
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ActivObject = 1;
            CollectionViewSource.GetDefaultView(Lsta_OFR.ItemsSource).Refresh();
        }

        private void OfrSerch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActivObject = 1;
            CollectionViewSource.GetDefaultView(Lsta_OFR.ItemsSource).Refresh();
        }
        private void Comb_PH_SelectionChanged(object sender, TextChangedEventArgs e)
        {
            if (((System.Windows.Controls.ComboBox)sender).Name == "Comb_PH")
            {
                ActivObject = 2;
                Comb_Klient.ItemsSource = Serhklient_as_combo(Comb_PH.Text).DefaultView;
            }
            else
            {
                ActivObject = 3;
            }
            CollectionViewSource.GetDefaultView(Lsta_OFR.ItemsSource).Refresh();

        }

        private void ListViewItem_clear(object sender, MouseButtonEventArgs e)
        {
            var item = (OFRData)((ListView)Lsta_OFR).SelectedItem;
            if (item is null)
                return;
            string stringqwerty = "update TblPdf set SAP='" + "DEL-" + (item.SAPnr + "' where Id =") + item.Id + " ";
            UsingSQLComand(stringqwerty, con);
            FTPConect.RenameFileName(item.SAPnr + "|" + item.NazwOFR, "BazaOfr/", "DEL-" + item.SAPnr + "|" + item.NazwOFR);

            BazaOFR = SqlComandDatabase_NewBaza(StringComand.ReturnComandPdtFile(), con);
            GetDataOFR(BazaOFR);
            Lsta_OFR.ItemsSource = Mw.ListOFR; // items
            showOFR.Navigate(new Uri("about:blank"));
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Lsta_OFR.ItemsSource);
            view.Filter = UserFilter;
        }

        public static int GetDataOFR(DataTable Baza)
        {
            try
            {
                Mw.ListOFR = new List<OFRData>();
                string PH, NazwOFR, Klient, sap;
                foreach (DataRow row in Baza.Rows)
                {
                    PH = row["Opiekun_klienta"].ToString();
                    NazwOFR = row["NrOFR"].ToString();
                    Klient = row["Nazwa_klienta"].ToString();
                    sap = row["SAP"].ToString();
                    Mw.ListOFR.Add(new OFRData()
                    {
                        Id = row["Id"].ToString(),
                        SAPnr = sap,
                        NazwOFR = NazwOFR,
                        OFR = row["PlkPdf"],
                        NazwKlient = Klient,
                        Opiekun = PH
                    });
                }
                return Baza.Rows.Count;
            }
            catch
            {
                return 0;
            }
        }

    }
}
