using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Controls
{

    public partial class CtrProd //: UserControl
    {
        public double OpaImg { get; set; }
        public double Rabat1;
        private TblOfr ctrlist;

        public CtrProd()
        {
            InitializeComponent();

            if (Upr_User.CenaKO == false)
                T_KO.Visibility = Visibility.Collapsed;

            UpDown.DataContext = Opacity == 0.3d;
            PInf.Visibility = Visibility.Collapsed;
            Row1_a.Visibility = Visibility.Collapsed;
            Row2_a.Visibility = Visibility.Collapsed;

        }
        public CtrProd(TblOfr data) : this()
        {

            ctrlist = data;
            this.DataContext = ctrlist; // ListTblOfr.Tbl_Add_prodList.ElementAt(data.ID);

            // Console.WriteLine("2 - CtrProd {0}", ctrlist.SAP);
            bool Nd;
            if (Strings.Mid(ctrlist.NazwProd, 1, 3) == "N/D" || Strings.Mid(ctrlist.NazwProd, 1, 5) == "(N/D)" || Strings.Mid(ctrlist.NazwProd, 1, 2) == "ND")
                Nd = true;
            else
                Nd = false;
            if (Nd == true)
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 198, 198));
                LabND.Content = "Produkt niedostępny - sprawdz!!";
            }
            LabImg.Tag = ctrlist.SAP;
            LabTds.Tag = ctrlist.SAP;
            LabKCH.Tag = ctrlist.SAP;
            CtrImage.Source = LiczOfr.ToImage(ctrlist.Img as byte[]);
            if (ctrlist.Plik_Tds_True == true)
                LabTds.Visibility = Visibility.Visible;
            else
                LabTds.Visibility = Visibility.Collapsed;
            if (ctrlist.Plik_Kch_True == true)
                LabKCH.Visibility = Visibility.Visible;
            else
                LabKCH.Visibility = Visibility.Collapsed;
            TRabatReczny.Background = new SolidColorBrush(Colors.White);
            TRabatReczny.Foreground = new SolidColorBrush(Colors.Black);
        }
        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            // Console.WriteLine("CtrProd - FindParent")
            try
            {
                var parent = VisualTreeHelper.GetParent(dependencyObject);
                if (parent is null)
                    return (T)null;
                T parentT = parent as T;
                return parentT ?? FindParent<T>(parent);
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return (T)null;
            }
        }
        public void RemoveMe()
        {
            try
            {
                var conditionUserControl = FindParent<GrupaProdukt>(this);
                if (conditionUserControl != null)
                {
                    var sp = FindParent<StackPanel>(conditionUserControl);
                    if (sp != null)
                    {
                        foreach (GrupaProdukt ctr in sp.Children)
                        {
                            foreach (CtrProd CtrPr in ctr.FlowLayoutPanel2.Children)
                            {
                                if (CtrPr.Tag == this.Tag)
                                {
                                    ctr.FlowLayoutPanel2.Children.Remove(this);
                                    LiczOfr.Delete_row_LiczOfr_Tbl_selectedIndex(Tag.ToString(), "CtrProd");

                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            try
            {
                LiczOfr.List_Add_prodList.ItemsSource = ListTblOfr.Tbl_Add_prodList;
                LiczOfr.List_Add_prodList.Items.Refresh();
            }
            catch
            {
            }
        }
        private void UClear_MouseDown(object sender, MouseButtonEventArgs e) // Handles UClear.MouseDown
        {
            RemoveMe();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new System.Text.RegularExpressions.Regex("[^0-9,.]+");
            int St = Conversions.ToInteger(((TextBox)sender).SelectionStart);
            ((TextBox)sender).SelectionStart = St;
        }
       
       

        private void Pmin_Pplus_TouchDown(object sender, MouseButtonEventArgs e)
        {
            if ((((Label)sender).Name.ToString() == "pplus") || (((Label)sender).Name.ToString() == "pmin"))
            {
                if (TRabatReczny.Text.Length <= 0)
                    TRabatReczny.Text = ctrlist.CenaDoOFR.ToString();
                double cpr = ctrlist.CenaDoOFR;
                if (((Label)sender).Name.ToString() == "pmin") 
                    cpr -= 0.1;
                if (((Label)sender).Name.ToString() == "pplus")
                    cpr += 0.1;
                if (ctrlist.CenaDoOFR != cpr)           
                    TRabatReczny.Text = cpr.ToString();
                
                return;
            }
            if ((((Label)sender).Name.ToString() == "pplus2") || (((Label)sender).Name.ToString() == "pmin2"))
            {
                if (TRabatReczny2.Text.Length <= 0) //&& TRabatReczny.Text.Length >= 0
                    TRabatReczny2.Text = (ctrlist.CenaDoOFR - 0.1).ToString();
                else
                {
                    double cpr = double.Parse(TRabatReczny2.Text.ToString());
                    if (((Label)sender).Name.ToString() == "pmin2")
                    {
                        if ((cpr -= 0.1) > 0.1)
                            cpr -= 0.1;
                    }

                    if (((Label)sender).Name.ToString() == "pplus2")
                    {
                        if ((cpr += 0.1) < ctrlist.CenaDoOFR)
                            cpr += 0.1;
                    }
                    if (ctrlist.CenaDoOFR2 != cpr && cpr < ctrlist.CenaDoOFR && cpr > 0.1)
                        TRabatReczny2.Text = cpr.ToString();
                   
                }
                return;
            }
            if ((((Label)sender).Name.ToString() == "pplus3") || (((Label)sender).Name.ToString() == "pmin3"))
            {
                if (ctrlist.CenaDoOFR2 >= 0)
                {
                    if (TRabatReczny3.Text.Length <= 0) //&& TRabatReczny2.Text.Length >= 0
                        TRabatReczny3.Text = (ctrlist.CenaDoOFR2 - 0.1).ToString();
                    else
                    {
                        double cpr = double.Parse(TRabatReczny3.Text.ToString());
                        if (((Label)sender).Name.ToString() == "pmin3")
                        {
                            if ((cpr -= 0.1) > 0.1)
                            cpr -= 0.1;
                        }
                            
                        if (((Label)sender).Name.ToString() == "pplus3")
                        {
                          if ((cpr += 0.1) < ctrlist.CenaDoOFR2)
                                cpr += 0.1;
                        }
                        if (ctrlist.CenaDoOFR3 != cpr && cpr < ctrlist.CenaDoOFR2 && cpr > 0.1)
                            TRabatReczny3.Text = cpr.ToString();
                
                    }
                }
                return;
            }
        }




        private void TRabatReczny_TextChanged(object sender, TextChangedEventArgs e)
        {

            WriteTxtCena(sender); ;
        }
        public void WriteTxtCena(object sender)
        {
            int St = ((TextBox)sender).SelectionStart;
            string myName = ((TextBox)sender).Name.ToString();
            if ((((TextBox)sender).Name == "TRabatReczny2") || (((TextBox)sender).Name == "TRabatReczny3"))
            {
                if (((TextBox)sender).Text.ToString() == "")
                    ClearKskadaZK(sender);
            }
            if ((((TextBox)sender).Text != ""))
            {
                ((TextBox)sender).Text = Strings.Replace(((TextBox)sender).Text, ".", ",").Trim();
            }
            if (((TextBox)sender).Text.ToString().Contains(",,"))
            {
                ((TextBox)sender).Text = Strings.Replace(((TextBox)sender).Text, ",,", ",").Trim();
            }
            if (Strings.Mid(((TextBox)sender).Text.ToString(), 1, 1) == ",")
            {
                ((TextBox)sender).Text = Strings.Replace(((TextBox)sender).Text, ",", "").Trim();
            }

            if (myName == "TRabatReczny")
            {
                if (((TextBox)sender).Text is null || (((TextBox)sender).Text == ""))
                {
                    ctrlist.CenaDoOFR = ctrlist.CenaZPrace;
                    ctrlist.ZK11A1 = default;
                    TxtInfoCena.Content = "PraceList";
                }
                try
                {
                    if (((TextBox)sender).Text.Length >= 0)
                    {
                        ctrlist.CenaDoOFR = double.Parse(((TextBox)sender).Text.ToString());
                        if (ctrlist.CenaDoOFR >= ctrlist.CenaZPrace)
                        {
                            ctrlist.CenaDoOFR = ctrlist.CenaZPrace;
                            ((TextBox)sender).Text = default;
                            ctrlist.ZK11A1 = default;
                            TxtInfoCena.Content = "PraceList";
                        }

                        if (((TextBox)sender).Text is null)
                        {
                            TRabatReczny2.Text = default;
                            TRabatReczny3.Text = default;
                        }
                        if (ctrlist.CenaDoOFR <= ctrlist.KO && ctrlist.CenaDoOFR >= 0)
                        {
                            ctrlist.ND = "True";
                            TxtInfoCena.Content = " Za nisko !";
                            TxtInfoCena.Foreground = Brushes.Red;
                            TRabatReczny.Background = new SolidColorBrush(Colors.Red);
                            TRabatReczny.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            ctrlist.ND = "False";
                            TxtInfoCena.Content = "";
                            TxtInfoCena.Foreground = Brushes.Black;
                            TRabatReczny.Background = new SolidColorBrush(Colors.White);
                            TRabatReczny.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                    else
                    {
                        Row1_a.Visibility = Visibility.Collapsed;
                        Row2_a.Visibility = Visibility.Collapsed;
                    }
                }
                catch
                {
                    return;
                }
                return;
            }
            string[] szt = Strings.Split(ctrlist.Kszt, "/");
            if (myName == "TRabatReczny2")
            {
                try
                {
                    if ((((TextBox)sender).Text.Length >= 0))
                    {
                        if (ctrlist.szt2 == "0" || ctrlist.szt2 is null)
                        {
                            ZK2Ile.Text = (double.Parse(szt[0].ToString()) * 10).ToString();
                        }
                        ctrlist.CenaDoOFR2 = double.Parse(((TextBox)sender).Text.ToString());
                        // Dim Zk As Decimal = Replace(Math.Round((ctrlist.CenaDoOFR2 / ctrlist.CenaZPrace * 100) - 100, 2, MidpointRounding.AwayFromZero), "-", "") : If Zk = 100 Then Zk = 0
                        if (ctrlist.CenaDoOFR2 <= ctrlist.KO)
                        {
                            TxtInfoCena2.Content = " Za nisko !";
                            TxtInfoCena2.Foreground = Brushes.Red;
                        }
                        else
                        {
                            TxtInfoCena.Foreground = Brushes.Black;
                        }
                        if (ctrlist.CenaDoOFR2 >= ctrlist.CenaDoOFR)
                        {
                            ((TextBox)sender).Text = ctrlist.CenaDoOFR.ToString();
                        }
                        if ((((TextBox)sender).Text.Length >= 1))
                        {
                            if (ctrlist.CenaDoOFR <= double.Parse(((TextBox)sender).Text.ToString()) || ctrlist.CenaDoOFR2 == 0)
                            {
                                ((TextBox)sender).Text = default;
                                TRabatReczny3.Text = default;
                            }
                        }
                        else
                        {
                            TRabatReczny3.Text = default;
                        }
                        T_Zk11_2.Text = ctrlist.ZK11A2.ToString();
                        Marza_2.Text = ctrlist.Marza2;
                        if (ctrlist.CenaDoOFR2 == 0)
                        {
                            ((TextBox)sender).Text = default;
                        }
                        if (((TextBox)sender).Text is null)
                        {
                            TRabatReczny3.Text = default;
                        }
                        if (ctrlist.CenaDoOFR2 <= ctrlist.KO & ctrlist.CenaDoOFR2 >= 0)
                        {
                            TxtInfoCena2.Content = " Za nisko !";
                            TxtInfoCena2.Foreground = Brushes.Red;
                            TRabatReczny2.Background = new SolidColorBrush(Colors.Red);
                            TRabatReczny2.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            TxtInfoCena2.Content = "";
                            TxtInfoCena2.Foreground = Brushes.Black;
                            TRabatReczny2.Background = new SolidColorBrush(Colors.White);
                            TRabatReczny2.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                    else
                    {

                        ctrlist.CenaDoOFR2 = default;
                        TRabatReczny3.Text = default;
                        ZK2Ile.Text = default;
                        Lsz2.Content = default;
                        Marza_2.Text = default;
                        TxtInfoCena2.Content = default;
                        InfodoZK2.Content = default;
                    }
                }
                catch
                {
                    return;
                }
                return;
            }


            if (myName == "TRabatReczny3")
            {
                try
                {
                    if ((((TextBox)sender).Text.Length >= 0))
                    {
                        if (ctrlist.szt3 == "0" || ctrlist.szt3 is null)
                        {
                            //ZK3Ile.Text = (double.Parse(szt[0].ToString()) * 10).ToString();
                            if (ctrlist.szt2 == null)
                                return;
                            ZK3Ile.Text = (double.Parse(ctrlist.szt2.ToString()) + (double.Parse(szt[0].ToString()) * 10)).ToString();// + (double.Parse(szt[0].ToString()) * 10)));
                        }
                        ctrlist.CenaDoOFR3 = double.Parse(((TextBox)sender).Text.ToString());
                        // Dim Zk As Decimal = Replace(Math.Round((ctrlist.CenaDoOFR2 / ctrlist.CenaZPrace * 100) - 100, 2, MidpointRounding.AwayFromZero), "-", "") : If Zk = 100 Then Zk = 0
                        if (ctrlist.CenaDoOFR3 <= ctrlist.KO)
                        {
                            TxtInfoCena3.Content = " Za nisko !";
                            TxtInfoCena3.Foreground = Brushes.Red;
                        }
                        else
                        {
                            TxtInfoCena3.Foreground = Brushes.Black;
                        }
                        if (ctrlist.CenaDoOFR3 >= ctrlist.CenaDoOFR)
                        {
                            ((TextBox)sender).Text = ctrlist.CenaDoOFR.ToString();
                        }
                        if ((((TextBox)sender).Text.Length >= 1))
                        {
                            if (ctrlist.CenaDoOFR <= double.Parse(((TextBox)sender).Text.ToString()) || ctrlist.CenaDoOFR3 == 0)
                            {
                                ((TextBox)sender).Text = default;
                                //TRabatReczny3.Text = default;
                            }
                        }
                        else
                        {
                            //TRabatReczny3.Text = default;
                        }
                        T_Zk11_3.Text = ctrlist.ZK11A3.ToString();
                        Marza_3.Text = ctrlist.Marza3;
                        if (ctrlist.CenaDoOFR3 == 0)
                        {
                            ((TextBox)sender).Text = default;
                        }
                        if (((TextBox)sender).Text is null)
                        {
                           // TRabatReczny3.Text = default;
                        }
                        if (ctrlist.CenaDoOFR3 <= ctrlist.KO & ctrlist.CenaDoOFR3 >= 0)
                        {
                            TxtInfoCena3.Content = " Za nisko !";
                            TxtInfoCena3.Foreground = Brushes.Red;
                            TRabatReczny3.Background = new SolidColorBrush(Colors.Red);
                            TRabatReczny3.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            TxtInfoCena3.Content = "";
                            TxtInfoCena3.Foreground = Brushes.Black;
                            TRabatReczny3.Background = new SolidColorBrush(Colors.White);
                            TRabatReczny3.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                    else
                    {

                        ctrlist.CenaDoOFR3 = default;
                        TRabatReczny3.Text = default;
                        ZK3Ile.Text = default;
                        Lsz3.Content = default;
                        Marza_3.Text = default;
                        TxtInfoCena3.Content = default;
                        InfodoZK3.Content = default;
                    }
                }
                catch
                {
                    return;
                }
                return;
            }






            if (myName == "TRabatReczny322")
            {
                try
                {
                    if ((((TextBox)sender).Text.Length >= 0))
                    {


                        if (ctrlist.szt3 == "0" || ctrlist.szt3 is null)
                        {
                            ZK2Ile.Text = (double.Parse(ZK2Ile.ToString()) + double.Parse(szt[0].ToString()) * 10).ToString();
                        }
                        ctrlist.CenaDoOFR3 = double.Parse(((TextBox)sender).Text.ToString());
                        if (ctrlist.CenaDoOFR3 >= ctrlist.CenaDoOFR2)
                        {
                            ((TextBox)sender).Text = ctrlist.CenaDoOFR2.ToString();
                        }
                        if ((((TextBox)sender).Text.Length >= 1))
                        {
                            if (ctrlist.CenaDoOFR2 <= double.Parse(((TextBox)sender).Text.ToString()) || ctrlist.CenaDoOFR3 == 0)
                            {
                                ((TextBox)sender).Text = default;
                            }
                        }
                        T_Zk11_3.Text = ctrlist.ZK11A3.ToString();
                        Marza_3.Text = ctrlist.Marza3;
                        if (ctrlist.CenaDoOFR3 == 0)
                            ((TextBox)sender).Text = default;
                        if (ctrlist.CenaDoOFR3 <= ctrlist.KO && ctrlist.CenaDoOFR3 >= 0)
                        {
                            TxtInfoCena3.Content = " Za nisko !";
                            TxtInfoCena3.Foreground = Brushes.Red;
                            TRabatReczny3.Background = new SolidColorBrush(Colors.Red);
                            TRabatReczny3.Foreground = new SolidColorBrush(Colors.White);
                        }
                        else
                        {
                            TxtInfoCena3.Content = "";
                            TxtInfoCena3.Foreground = Brushes.Black;
                            TRabatReczny3.Background = new SolidColorBrush(Colors.White);
                            TRabatReczny3.Foreground = new SolidColorBrush(Colors.Black);
                        }
                    }
                    else
                    {
                        // ctrlist.ZK11A3 = 0 : ctrlist.Marza3 = 0
                        // T_Zk11_3.Text = ctrlist.ZK11A3 : Marza_3.Text = ctrlist.Marza3
                        ctrlist.CenaDoOFR3 = default;
                        ZK3Ile.Text = default;
                        Lsz3.Content = default;
                        Marza_3.Text = default;
                        TxtInfoCena3.Content = default;
                        InfodoZK3.Content = default;
                    }
                }
                catch
                {
                    return;
                }
                return;
            }
            ((TextBox)sender).SelectionStart = St;

        }
        private void UpDown_TouchDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (UpDown.Opacity == 0.3d)
                    return;
                int meTag = int.Parse(Tag.ToString());
                if (Row1_a.Visibility == Visibility.Collapsed)
                {
                    Row1_a.Visibility = Visibility.Visible;
                    Row2_a.Visibility = Visibility.Visible;
                }
                else
                {
                    Row1_a.Visibility = Visibility.Collapsed;
                    Row2_a.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void CzyscLin2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearKskadaZK(sender);
        }
        private object ClearKskadaZK(object sender)
        {
            // Console.WriteLine("CtrProd - ClearKskadaZK")
            // Exit Function
            string Namesender;
            if (sender is Label)
            {
                Namesender = ((Label)sender).Name;
            }
            else
            {
                Namesender = ((TextBox)sender).Name;
            }



            if ((Namesender == "CzyscLin2") || (Namesender == "TRabatReczny2"))
            {
                TRabatReczny2.Text = "";
                Lsz2.Content = "";
                T_Zk11_2.Text = "";
                Marza_2.Text = "";
                ZK2Ile.Text = "";
                TxtInfoCena2.Content = "";
                foreach (var ctr in ListTblOfr.Tbl_Add_prodList) // dtTableOfert.Rows
                {
                    if (Operators.ConditionalCompareObjectEqual(ctr.SAP, this.Tag, false))
                    {
                        ctr.szt2 = null;
                        ctr.ZK11A2 = default;
                    }
                }
            }
            if ((Namesender == "CzyscLin3") || (Namesender == "TRabatReczny3"))
            {
                TRabatReczny3.Text = "";
                Lsz3.Content = "";
                T_Zk11_3.Text = "";
                Marza_3.Text = "";
                ZK3Ile.Text = "";
                TxtInfoCena3.Content = "";
                foreach (var ctr in ListTblOfr.Tbl_Add_prodList)
                {
                    if (Operators.ConditionalCompareObjectEqual(ctr.SAP, this.Tag, false))
                    {
                        ctr.szt3 = null;
                        ctr.ZK11A3 = default;
                    }
                }
            }
            return null;
        }
        private void LabTds_MouseDown(object sender, MouseButtonEventArgs e) // Handles LabTds.MouseDown
        {
            if (((Label)sender).Name == "LabTds")
                Cennik_Add.PdfView(sender, "Tds");
            if (((Label)sender).Name == "LabKCH")
                Cennik_Add.PdfView(sender, "KC");
        }
        private void T_Zk11_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ctrlist.ZK11A1 == 0 || T_Zk11_1.Text.Length < 1)
            {
                T_Zk11_1.Foreground = Brushes.Transparent;
                T9Prc.Content = "";
                Marza_1.Foreground = Brushes.Transparent;
                TxtInfoCena.Content = "PraceList";
            }
            else
            {
                T_Zk11_1.Foreground = Brushes.Red;
                T9Prc.Content = "%";
                TxtInfoCena.Content = "";
                Marza_1.Foreground = Brushes.Black;
            }
            if (ctrlist.ZK11A2 == 0)
            {
                T_Zk11_2.Foreground = Brushes.Transparent;
                TZK2Prc.Content = "";
                Marza_2.Foreground = Brushes.Transparent;
            }
            else
            {
                T_Zk11_2.Foreground = Brushes.Red;
                TZK2Prc.Content = "%";
                Marza_2.Foreground = Brushes.Black;
            }
            if (ctrlist.ZK11A3 == 0)
            {
                T_Zk11_3.Foreground = Brushes.Transparent;
                TZK3Prc.Content = "";
                Marza_3.Foreground = Brushes.Transparent;
            }
            else
            {
                T_Zk11_3.Foreground = Brushes.Red;
                TZK3Prc.Content = "%";
                Marza_3.Foreground = Brushes.Black;
            }
            // If sender.text >= ctrlist.CenaZPrace Then sender.Foreground = Brushes.Transparent : Exit Sub Else sender.Foreground = Brushes.Black
        }
        private void UpDown_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((Label)sender).Name, "UpDown_TouchDown", false)))
                {
                    if (ActualHeight <= 100)
                    {
                        Label sender1 = ((Label)sender);
                        // toltip1.SetValue(sender1, "Rozwiń");
                    }
                    else
                    {
                        // toltip1.SetValue(sender, "Zwiń");
                    }

                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void ZK2Ile_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string[] IleszK = Strings.Split(T_Szt.Content.ToString(), "/");
                Lsz2.Content = Math.Round(double.Parse(ZK2Ile.Text.ToString()) / double.Parse(IleszK[0].ToString()), 1) + " kart";
                foreach (var row in ListTblOfr.Tbl_Add_prodList)
                {
                    if (Operators.ConditionalCompareObjectEqual(Tag, row.SAP, false))
                    {
                        row.szt2 = ZK2Ile.Text;
                    }
                }
            }
            catch
            {
                Lsz2.Content = "Wprowadz !";
            }


        }
        private void ZK3Ile_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Console.WriteLine("CtrProd - ZK3Ile_TextChanged");
            try
            {
                string[] IleszK = Strings.Split(T_Szt.Content.ToString(), "/");
                Lsz3.Content = Math.Round(double.Parse(ZK3Ile.Text.ToString()) / double.Parse(IleszK[0].ToString()), 1) + " kart";
                foreach (var row in ListTblOfr.Tbl_Add_prodList)
                {
                    if (Operators.ConditionalCompareObjectEqual(Tag, row.SAP, false))
                    {
                        row.szt3 = ZK3Ile.Text;
                    }
                }
            }
            catch
            {
                Lsz3.Content = "Wprowadz !";
            }

        }
        private void LabImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Cennik_Add.ImgView(sender);
        }
        private void T_CenaPraceList_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Console.WriteLine("T_CenaPraceList_TextChanged -  {0} /  sender {1}", ctrlist.CenaDoOFR, sender.text)
            if (ctrlist.ZK11A1 == 0)
                TRabatReczny.Text = ctrlist.CenaZPrace.ToString();
            else
                TRabatReczny.Text = Math.Round(ctrlist.CenaZPrace - ctrlist.CenaZPrace / (1 / ctrlist.ZK11A1) / 100, 2).ToString();
        }

        private void T9_TextChanged(object sender, EventArgs e) // Handles T9.coChanged
        {
            Console.WriteLine("CtrProd - T9_TextChanged");

            try
            {
                if (T_Zk11_1.Text != "")
                {
                    if (TxtInfoCena.Content.ToString() == "PraceList" & (T_Zk11_1.Text == "0" || T_Zk11_1.Text == default))
                        PInf.Visibility = Visibility.Visible;
                    else
                        PInf.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // UpDown.Opacity = 0.3
                    PInf.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void UpdateZK11(int intRow)
        {
            //  Console.WriteLine("CtrProd - UpdateZK11");
            try
            {
                var Dgv = new DataTable();
                string a1;
                string a3, a4, a5;
                a1 = Cennik_Add.T3.Text;
                if (con.State == ConnectionState.Closed)
                    con.Close();
                if (ctrlist.ZK11A1.ToString() != "")
                {
                    if (ctrlist.ZK11A1 == default || ctrlist.ZK11A1.ToString().Trim() == "")
                        a3 = 0.ToString();
                    else
                        a3 = Strings.Replace(ctrlist.ZK11A1.ToString(), "-", "");
                    if (ctrlist.ZK11A2 == default || ctrlist.ZK11A2.ToString().Trim() == "")
                        a4 = 0.ToString();
                    else
                        a4 = Strings.Replace(ctrlist.ZK11A2.ToString(), "-", "");
                    if (ctrlist.ZK11A3 == default || ctrlist.ZK11A3.ToString().Trim() == "")
                        a5 = 0.ToString();
                    else
                        a5 = Strings.Replace(ctrlist.ZK11A3.ToString(), "-", "");

                    string searchQuery = "Select * from TabZK WHERE NIP Like '%" + a1 + "%'  And NrSAP like '%" + ctrlist.SAP + "%'";
                    Dgv = SqlComandDatabase(searchQuery, con);
                    if (Dgv.Rows.Count == Conversions.ToInteger(false))
                    {
                        string SqlString = "Insert into TabZK values('" + a1 + "','" + ctrlist.SAP + "','" + a3 + "','" + a4 + "','" + a5 + "')";
                        UsingSQLComand(SqlString, con);
                    }
                    else
                    {
                        for (int v = 0, loopTo = Dgv.Rows.Count - 1; v <= loopTo; v++)
                        {
                            ctrlist.ID = int.Parse(Dgv.Rows[v][0].ToString());
                            if (Dgv.Rows[v][2].ToString() == ctrlist.SAP)
                            {
                                string SqlString = "update TabZK set NIP='" + a1 + "',NrSAP='" + ctrlist.SAP + "',ZK1='" + a3 + "',ZK2='" + a4 + "',ZK3='" + a5 + "' where Id =" + ctrlist.ID + " ";
                                UsingSQLComand(SqlString, con);
                            }
                        }
                    }

                }
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        } // generuje ZK po wprowadzeniu reczni 



    }
}
