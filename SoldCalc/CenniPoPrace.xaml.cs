using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc
{
    /// <summary>
    /// Logika interakcji dla klasy CenniPoPrace.xaml
    /// </summary>


    public partial class CenniPoPrace : Page
    {
        public List<My_Class> LCennik { get; set; }
        // Private GVDataCennik As New DataTable
        public CenniPoPrace()
        {
            try
            {
                InitializeComponent();

                if (BazaCennik is null)
                    goto line1;
                GVDataCennik = (DataTable)(object)null;
                if (GVDataCennik is null)
                    GVDataCennik = BazaCennik.Copy();
                GVDataCennik.Columns.Add("CenaZPrace");
                GVDataCennik.Columns.Add("Check");
                GVDataCennik = LiczBazaRabat(GVDataCennik, "0");
                AddCheck();

                GVCennik.ItemsSource = GVDataCennik.DefaultView;
                FormPlacAdd();
            line1:
                ;
            }

            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString())
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private object AddCheck()
        {
            bool serchCheck = false;
            foreach (DataColumn col in GVDataCennik.Columns)
            {
                if (col.ColumnName == "Check")
                    serchCheck = true;
            }
            if (serchCheck == false)
            {
                GVDataCennik.Columns.Add("Check");
                foreach (DataRow row in GVDataCennik.Rows)
                    row["Check"] = "False";
            }
            try
            {
                LCennik = new List<My_Class>();
                string NazwProd, sap;
                foreach (DataRow row in GVDataCennik.Rows)
                {
                    NazwProd = row["NazwProd"].ToString();
                    sap = row["SAP"].ToString();
                    LCennik.Add(new My_Class()
                    {
                        ID = Conversions.ToInteger(row["Id"]),
                        SAP = Conversions.ToInteger(sap),
                        Name = NazwProd,
                        CenaZPrace = "",
                        Kszt = "",
                        Poj = "",
                        CDM = "",
                        CK = "",
                        PH = "",
                        ZPR0 = "",
                        BrakPrace = "",
                        IsChecked = Conversions.ToBoolean(row["Check"].ToString())
                    });

                }
            }
            catch
            {

            }
            return null;
        }



        private void FormPlacAdd()
        {
            try
            {
                PraceL.Items.Clear();
                var WczytajCB = new StreamReader("TxtPraceList1.txt");

                while (WczytajCB.EndOfStream != true)
                {
                    string Wind = WczytajCB.ReadLine();
                    PraceL.Items.Add(Wind); // : col.Add(Wind).ToString()
                }
                WczytajCB.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                double remainingSpace = ActualWidth;
                GVCennik.Width = remainingSpace - 25;
                if (remainingSpace > 1000)
                {

                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SercgData(TxtAdd.Text);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private bool blokujcheck = false;
        private void SercgData(string valueToSearch)
        {
            try
            {
                blokujcheck = true;
                if (string.IsNullOrEmpty(valueToSearch))
                {
                    BazaCennik.DefaultView.RowFilter = null;
                    return;
                }
                string T1 = "";
                string T2 = "";
                string T3 = "";
                string T4 = "";
                string T5 = "";
                string T6 = "";
                string[] splittext = valueToSearch.Split(' '); // As String()
                if (splittext.Count() == splittext.Count())
                {
                    // On Error Resume Next
                    try
                    {
                        if (splittext[0].Length > 0)
                        {
                            T1 = splittext[0].ToString();
                            valueToSearch = splittext[0].ToString();
                        }
                        else
                            T1 = ""; // : txtAddProd.Text = "" 'splittext(0).ToString
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

                    catch
                    {

                    }
                }
                try
                {
                    GVDataCennik.DefaultView.RowFilter = string.Format(@"Check Like 'True' or NazwProd LIKE '%{0}%' and NazwProd Like '%{1}%' and NazwProd Like '%{2}%' and NazwProd Like '%{3}%' and NazwProd Like '%{4}%' and NazwProd Like '%{5}%'
                                                                                     Or SAP LIKE '%{0}%' and SAP Like '%{1}%' and SAP Like '%{2}%' and SAP Like '%{3}%' and SAP Like '%{4}%' and SAP Like '%{5}%'
                                                                                    Or GRUPA LIKE '%{0}%' and GRUPA Like '%{1}%' and GRUPA Like '%{2}%' and GRUPA Like '%{3}%' and GRUPA Like '%{4}%' and GRUPA Like '%{5}%'
                                                                                      Or KATEGORIA LIKE '%{0}%' and KATEGORIA Like '%{1}%' and KATEGORIA Like '%{2}%' and KATEGORIA Like '%{3}%' and KATEGORIA Like '%{4}%' and KATEGORIA Like '%{5}%'
                                                                                      Or NAZEWNICTWO LIKE '%{0}%' and NAZEWNICTWO Like '%{1}%' and NAZEWNICTWO Like '%{2}%' and NAZEWNICTWO Like '%{3}%' and NAZEWNICTWO Like '%{4}%' and NAZEWNICTWO Like '%{5}%'
                                                                                               ", T1, T2, T3, T4, T5, T6);
                }
                catch
                {
                }
            }
            // For i As Integer = 0 To GVDataCennik.Rows.Count - 1
            // If GVDataCennik.Rows(i)("Check") = "True" Then GVCennik.Items(i)(0) = True '  GVDataCennik.Rows(i)("Check")
            // Next

            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }

            if (string.IsNullOrEmpty(valueToSearch))
                BazaCennik.DefaultView.RowFilter = null;
            blokujcheck = false;
        } // szukaj text w DATAGRID VIEW _STRING FORMAT

        private void CPP_CombBR_Selectiontext(object sender, TextChangedEventArgs e)
        {
            RunCombBR_Selectiontext();
        }
        private void RunCombBR_Selectiontext()
        {
            try
            {
                // If GVDataCennik Is Nothing Then GVDataCennik = BazaCennik.Copy()
                string text = PraceL.Text;
                string Rbt = "";
                bool min = false;
                for (int i = 3, loopTo = text.Length - 1; i <= loopTo; i++)
                {
                    char c = text[i];
                    if (Conversions.ToString(c) == "-")
                        min = true;
                    if (min == true)
                    {
                        if (Information.IsNumeric(c) | Conversions.ToString(c) == ",")
                            Rbt += Conversions.ToString(c);
                    }
                }
                if (Rbt is null | string.IsNullOrEmpty(Rbt))
                    Rbt = 0.ToString();
                Console.WriteLine(Rbt);
                GVDataCennik = LiczBazaRabat(GVDataCennik, Rbt);
                GVCennik.ItemsSource = GVDataCennik.DefaultView;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private DataTable LiczBazaRabat(DataTable Db, string Rbt)
        {
            try
            {
                foreach (DataRow row in Db.Rows)
                {
                    if (Information.IsNumeric(row["ZPR0"].ToString()))
                    {
                        if (string.IsNullOrEmpty(row["BrakPrace"].ToString()))
                        {
                            double Val = Conversions.ToDouble(Roundd(Conversions.ToDecimal(Operators.MultiplyObject(row["ZPR0"], 1d - Conversions.ToDouble(Rbt) / 100d))));
                            row["CenaZPrace"] = Strings.Format(Val, "# ### ##0.00") + " zł";
                        }
                        else
                        {
                            row["CenaZPrace"] = Roundd(Conversions.ToDecimal(row["ZPR0"]));
                        } // 'Nothing
                    }
                    else
                    {
                        row["CenaZPrace"] = null;
                    }
                }
                return Db;
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }

        public object Roundd(decimal dec)
        {
            try
            {
                decimal d = dec;
                decimal r = Math.Truncate(d * 100) / 100;
                return r;
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            GVDataCennik.Rows[GVCennik.SelectedIndex]["Check"] = ((System.Windows.Controls.CheckBox)sender).IsChecked;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) // Handles TestCheckBox.Checked
        {
            if (blokujcheck == true)
                return;
            System.Windows.Controls.CheckBox chk = (System.Windows.Controls.CheckBox)sender;
            bool checkVal = chk.IsChecked.Value;
            Console.Write(chk.IsChecked.Value);
            try
            {
                foreach (DataRow row in GVDataCennik.Rows)
                {
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(((System.Windows.Controls.CheckBox)sender).Tag, row["SAP"].ToString(), false)))
                        row["Check"] = checkVal; // chk.IsChecked.ToString
                }
            }
            catch
            {

            }


        }


        private void Cennik_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double remainingSpace;
            try
            {
                remainingSpace = GVCennik.ActualWidth - 170;
                GridView colH = GVCennik.View as GridView;
                int i = 30;
                for (int c = 0, loopTo = colH.Columns.Count - 1; c <= loopTo; c++)
                {
                    if (c != 2)
                        i += (int)colH.Columns[c].ActualWidth;
                    // If CenaKO = False Then Lloc.Text = "0" Else Lloc.Text = "80"
                } (GVCennik.View as GridView).Columns[2].Width = Math.Ceiling(remainingSpace - i); // (i + 115)) '770
            }
            catch
            {
                // a &= a & 1
            }
        }

        private void CombBrSelectionText(object sender, TextChangedEventArgs e)
        {

        }
    }

    public partial class My_Class
    {
        public int ID { get; set; }
        public int SAP { get; set; }
        public string NazwProd { get; set; }
        public string CenaZPrace { get; set; }
        public string Kszt { get; set; }
        public string Poj { get; set; }
        public string CDM { get; set; }
        public string CK { get; set; }
        public string PH { get; set; }
        public string ZPR0 { get; set; }
        public string BrakPrace { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public bool IsChecked { get; set; }
    }
}

