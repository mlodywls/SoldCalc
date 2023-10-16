using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SoldCalc.MainWindow;

namespace SoldCalc.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy GrupaProdukt.xaml
    /// </summary>
    //public partial class GrupaProdukt : UserControl
    //{
    //    public GrupaProdukt()
    //    {
    //        InitializeComponent();
    //    }
    //}
    public partial class GrupaProdukt : UserControl
    {
        // Property LiczOferta
        //private object _Dta;

        //private object Dta
        //{
        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    get
        //    {
        //        return _Dta;
        //    }

        //    [MethodImpl(MethodImplOptions.Synchronized)]
        //    set
        //    {
        //        _Dta = value;
        //    }
        //}
        public GrupaProdukt(TblOfr data) : this()
        {
            DataContext = data; // Me
                                // Dta = data;
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //   Name = Operators.ConcatenateObject("GrupaProdukt", Dta.SAP);
        }
        public GrupaProdukt()
        {
            // Dta = new object();


            InitializeComponent();
            //  DataContext = Dta; // LiczOfr.Tbl_Add_prodList

            if (Upr_User.CenaKO == false)
            {
                LKO.Visibility = Visibility.Collapsed;
                L6.Visibility = Visibility.Collapsed;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new System.Text.RegularExpressions.Regex("[^0-9,.]+");
            int St = Conversions.ToInteger(((TextBox)sender).SelectionStart);
            ((TextBox)sender).SelectionStart = St;
        }

        private void Clear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
            line1:
                ;

                foreach (var row in ListTblOfr.Tbl_Add_prodList)
                {
                    string TagNr1 = Conversions.ToString(row.Lpgrup);
                    if (TagNr1.ToString() == this.Tag.ToString())
                    {
                        ListTblOfr.Tbl_Add_prodList.Remove(row);
                        goto line1;
                    }
                }
                if (ListTblOfr.Tbl_Add_prodList.Count == 0)
                {
                    LiczOfr.Gr0.Height = GridLength.Auto;
                    LiczOfr.Gr1.Height = new GridLength(0.5d, GridUnitType.Star);
                    LiczOfr.DocListViev.Visibility = Visibility.Visible;
                    LiczOfr.xKryj.Content = "zwiń";
                }
                RemoveMe();
            }
            catch
            {
            }
        }

        private void RemoveMe()
        {
            try
            {
                StackPanel Par = (StackPanel)this.Parent;
                LiczOfr.Delete_row_LiczOfr_Tbl_selectedIndex(this.Tag.ToString(), "GrupaProdukt");
                Par.Children.Remove(this);
            }
            catch
            {
            }
        }
        public void TRabatReczny_TextChanged(object sender, EventArgs e) // Handles TRabatReczny.TextChanged
        {
            int St = Conversions.ToInteger(((TextBox)sender).SelectionStart);
            TextBox thisLabel = (TextBox)sender;
            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectNotEqual(((TextBox)sender).Text, "", false)))
                ((TextBox)sender).Text = Strings.Replace(Conversions.ToString(((TextBox)sender).Text), ".", ",").Trim();
            if (((TextBox)sender).Text.ToString().Contains(",,"))
                ((TextBox)sender).Text = Strings.Replace(Conversions.ToString(((TextBox)sender).Text), ",,", ",").Trim();
            if (Strings.Mid(((TextBox)sender).Text.ToString(), 1, 1) == ",")
                ((TextBox)sender).Text = Strings.Replace(Conversions.ToString(((TextBox)sender).Text), ",", "").Trim();
            string txt = thisLabel.Text.ToString();
            if (thisLabel.Text is null)
                txt = "";
            string myName = thisLabel.Tag + "|" + txt;
            Licz(myName);
            ((TextBox)sender).SelectionStart = St;
            // Catch : End Try
        }
        private void Licz(object myName)
        {
            // Try
            string[] MeValue = Strings.Split(Conversions.ToString(myName), "|");
            {
                //ref var withBlock = ref this;
                foreach (var ctr in ListTblOfr.Tbl_Add_prodList)
                {
                    if (ctr.Lpgrup == MeValue[0])
                    {
                        foreach (UserControl ctl in FlowLayoutPanel2.Children)
                        {
                            if (ctl.Tag.ToString() == ctr.SAP.ToString())
                            {
                                CtrProd SerchUC = (CtrProd)ctl;
                                // Console.WriteLine("MeValue(1) - {0}", MeValue(1))
                                if (string.IsNullOrEmpty(MeValue[1]) || string.IsNullOrEmpty(MeValue[1]))
                                {
                                    // ctr.CenaDoOFR = ctr.
                                    SerchUC.TRabatReczny.Text = null;
                                }
                                else
                                {
                                    ctr.CenaDoOFR = double.Parse(MeValue[1].ToString());
                                    SerchUC.TRabatReczny.Text = MeValue[1];
                                }
                            }
                        }
                    }
                }
            }
            // Catch : End Try
        }
        private void LiczLabel(object myName)
        {
            // Console.WriteLine("Grupa produkt - LiczLabel" & myName)
            try
            {
                string[] MeValue = Strings.Split(Conversions.ToString(myName), "|");
                var MyCena = default(double);
                {
                    foreach (var ctr in ListTblOfr.Tbl_Add_prodList)
                    {
                        if (ctr.Lpgrup == MeValue[0])
                        {
                            if (MeValue[1] == "CDM")
                                MyCena = double.Parse(ctr.CDM.ToString());
                            if (MeValue[1] == "LKO")
                                MyCena = double.Parse(ctr.KO.ToString());
                            if (MeValue[1] == "PH")
                                MyCena = double.Parse(ctr.PH.ToString());
                            if (MeValue[1] == "ZRP0")
                                MyCena = double.Parse(ctr.ZPR0.ToString());
                            MyCena = Math.Round(MyCena, 2, MidpointRounding.AwayFromZero);
                            foreach (UserControl ctl in FlowLayoutPanel2.Children)
                            {
                                if (ctl.Tag.ToString() == ctr.SAP)
                                {
                                    CtrProd SerchUC = (CtrProd)ctl;
                                    ctr.CenaDoOFR = MyCena;
                                    SerchUC.TRabatReczny.Text = MyCena.ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void Plus_minus(object sender, MouseButtonEventArgs e)
        {
            string myName = ((Label)sender).Tag.ToString() + "|" + ((Label)sender).Name.ToString();
            Plus_minusw(myName);
        }
        private void Plus_minusw(object myName)
        {
            try
            {
                string[] MeValue = Strings.Split(Conversions.ToString(myName), "|");
                {
                    // ref var withBlock = ref this;
                    foreach (var ctr in ListTblOfr.Tbl_Add_prodList)
                    {
                        if (ctr.Lpgrup == MeValue[0])
                        {
                            foreach (UserControl ctl in FlowLayoutPanel2.Children)
                            {
                                if (ctl.Tag.ToString() == ctr.SAP.ToString())
                                {
                                    CtrProd SerchUC = (CtrProd)ctl;
                                    if (MeValue[1] == "pmin")
                                    {
                                        ctr.CenaDoOFR -= 0.1;
                                        SerchUC.TRabatReczny.Text = ctr.CenaDoOFR.ToString();
                                    }
                                    if (MeValue[1] == "pplus")
                                    {
                                        ctr.CenaDoOFR += 0.1;
                                        SerchUC.TRabatReczny.Text = ctr.CenaDoOFR.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        private void KO_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Control thisLabel = (Control)sender;
                string myName = thisLabel.Tag + "|" + thisLabel.Name;
                LiczLabel(myName);
            }
            catch
            {
            }
        }


        private void KO_MouseLeave(object sender, MouseEventArgs e)
        {
            // Console.WriteLine("Grupa produkt - KO_MouseLeave")
            try
            {
                Control pan;
                pan = (Control)sender;
                if (pan.Name == "LKO")
                {
                    LKO.FontWeight = FontWeights.Normal;
                    LKO.Foreground = Brushes.Black;
                }
                if (pan.Name == "CDM")
                {
                    CDM.FontWeight = FontWeights.Normal;
                    CDM.Foreground = Brushes.Black;
                }
                if (pan.Name == "PH")
                {
                    PH.FontWeight = FontWeights.Normal;
                    PH.Foreground = Brushes.Black;
                }
                if (pan.Name == "ZRP0")
                {
                    ZRP0.FontWeight = FontWeights.Normal;
                    ZRP0.Foreground = Brushes.Black;
                }
            }
            catch
            {
            }
        }

        private void CDM_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                Label pan;
                pan = (Label)sender;
                pan.FontWeight = FontWeights.Bold;
                pan.Foreground = Brushes.Red;
            }
            catch
            {
            }
        }


        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Console.WriteLine("Grupa produkt - UserControl_SizeChanged")
            if (this.ActualHeight <= 100)
                RemoveMe();
        }

        private void Przenies_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserControl btn_to_move = this; // CType(e.GetData(DataFormats.Serializable), UserControl)
            int what_to_move = LiczOfr.FlowLayoutPanel1.Children.IndexOf(btn_to_move);
            int Last = LiczOfr.FlowLayoutPanel1.Children.Count; // 0 ' LiczOfr.Tbl_Add_prodList.Count - 1


            int ToMove = 0;
            if (((Image)sender).Name == "Upp")
                ToMove = what_to_move - 1;
            if (((Image)sender).Name == "Down")
                ToMove = what_to_move + 1;
            if (((Image)sender).Name == "ALLUpp")
                ToMove = 0; // what_to_move - 1
            if (((Image)sender).Name == "AllDown")
                ToMove = Last - 1;
            if (ToMove >= 0)
            {
                try
                {
                    if (ToMove <= Last - 1)
                    {
                        LiczOfr.FlowLayoutPanel1.Children.RemoveAt(what_to_move);
                        LiczOfr.FlowLayoutPanel1.Children.Insert(ToMove, btn_to_move);
                        ListTblOfr.Tbl_Add_prodList = (List<TblOfr>)LiczOfr.NewId_Tbl_prodList();
                    }
                }
                catch
                {
                }
            }
        }

        private void Lid_TextChanged(object sender, TextChangedEventArgs e)
        {
            int Last = LiczOfr.FlowLayoutPanel1.Children.Count - 1;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualHeight < 140)
                RemoveMe();
            // Console.WriteLine(Me.ActualHeight)
        }




    }
}
