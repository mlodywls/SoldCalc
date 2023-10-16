using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.FTPConect;
using static SoldCalc.Supporting.Message;

namespace SoldCalc
{
    /// <summary>
    /// Logika interakcji dla klasy EmailShow.xaml
    /// </summary>
    //public partial class EmailShow : Page
    //{
    //    public EmailShow()
    //    {
    //        InitializeComponent();
    //    }
    //}
    public partial class EmailShow //: UserControl
    {
        private DataTable BrGrup = new DataTable();
        public List<GrupBranz> BrList { get; set; }
        public EmailShow()
        {

            try
            {
                InitializeComponent();
                string Sqwery2 = "SELECT distinct KO, Branza FROM DaneKO WHERE Branza <>''";
                WczytajBaza(Sqwery2);
                GetData(BrGrup);
                ListView1.ItemsSource = BrList;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void UClear_Click(object sender, MouseButtonEventArgs e)
        {
            RemoveMe();
        }
        private void RemoveMe()
        {
            try
            {
                // DockPanel Par = (DockPanel)this.Parent;
                //  Par.Visibility = Visibility.Collapsed;
                /// Par.Children.Remove(this);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public void WczytajBaza(string serch)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                using (var cmd = new SQLiteCommand(serch))
                {
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                    var daBaza = new SQLiteDataAdapter(cmd);
                    daBaza.Fill(BrGrup);
                }
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        public void GetData(DataTable Baza)
        {
            try
            {
                BrList = new List<GrupBranz>();
                foreach (DataRow row in Baza.Rows)
                    BrList.Add(new GrupBranz()
                    {
                        Branza = row["Branza"].ToString(),
                        BrTag = Strings.Mid(row["Branza"].ToString(), 1, 2)
                    });
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void ES_chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkSelectAll.IsChecked.Value == true)
                {
                    if (ListView1.Visibility == Visibility.Visible)
                        ListView1.SelectAll();
                }
                else
                {
                    if (ListView1.Visibility == Visibility.Visible)
                        ListView1.UnselectAll();
                    RichTextBox1.Text = "";
                    ZaznaczBr.Clear();
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private DataTable ZaznaczBr = new DataTable();
        private ListView Zaznaczon = new ListView();
        private readonly DockPanel Parent;








        private void chkWspSelect_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                string SerhTag = Conversions.ToString(((CheckBox)sender).Tag);
                try
                {
                    foreach (var itm in BrList)
                    {
                        if ((itm.Branza ?? "") == (SerhTag ?? ""))
                            ADDProdList(itm);
                    }
                }
                catch (Exception ex)
                {
                    TextMessage(ex.StackTrace.ToString());
                }
                wstawEmail();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void chkWspSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                string SerhTag = Conversions.ToString(((CheckBox)sender).Tag);
                // MsgBox(sender.tag)
                foreach (DataRow itm in ZaznaczBr.Rows)
                {
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(itm["Branza"], SerhTag, false)))
                    {
                        ZaznaczBr.Rows.Remove(itm);
                        break;
                    }
                }
                wstawEmail();
            }
            catch
            {
            }
        }
        public object ADDProdList(GrupBranz obj)
        {
            try
            {
                if (ZaznaczBr != null == true)
                    UstawPanels();
                int Nr = ZaznaczBr.Rows.Count; // - 1
                string Cc2 = obj.Branza; // "Naglowek" naglówek produktu
                for (int j = 0, loopTo = ZaznaczBr.Rows.Count - 1; j <= loopTo; j++)
                {
                    if ((ZaznaczBr.Rows[j][1].ToString() ?? "") == (Cc2 ?? ""))
                        goto line1;
                }
                ZaznaczBr.Rows.Add(Nr, Cc2);
            line1:
                ;

                return null;
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        } // Utwórz tabelę do przechowawania wstawionych danych do oferta

        public object wstawEmail()
        {

            string SqlStr = "";
            string TxtBr = null;
            RichTextBox1.Text = "";
            foreach (DataRow row in ZaznaczBr.Rows)
            {
                if (!string.IsNullOrEmpty(SqlStr))
                    SqlStr += " or si.Branza like'%" + Strings.Mid(row["Branza"].ToString(), 1, 2) + "%'";
                else
                    SqlStr = " si.Branza like'%" + Strings.Mid(row["Branza"].ToString(), 1, 2) + "%'";
            }
            string sql = @"SELECT distinct si.KO, si.Branza, md.E_mail FROM DaneKO si  LEFT JOIN  BazaKL md ON substr(si.Branza,1,2) = substr(md.Branza ,1,2) 
                                where " + SqlStr;
            DataTable db = SqlComandDatabase_NewBaza(sql, con);
            foreach (DataRow row in db.Rows)
            {
                if (!string.IsNullOrEmpty(row["E_mail"].ToString()))
                {
                    string rpl = Strings.Replace(row["E_mail"].ToString(), ";", ";" + Constants.vbCrLf);
                    TxtBr += rpl + ";" + Constants.vbCrLf;
                }
            }
            if (db.Rows.Count > 0)
                Label1.Content = "Wprowadzono " + db.Rows.Count + " adresów e_mail";
            else
                Label1.Content = "";
            RichTextBox1.Text = TxtBr;
            return null;
        }

        private void UstawPanels()
        {
            var dc1 = new DataColumn("ID", typeof(int));
            var dc2 = new DataColumn("Branza", typeof(string)); // 1
            try
            {
                ZaznaczBr.Columns.Add(dc1);
                ZaznaczBr.Columns.Add(dc2);
            }
            catch
            {
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Sendt_mail(RichTextBox1.Text);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        internal void Sendt_mail(string TextEmail)
        {
            try
            {

                var objOutlook = new Microsoft.Office.Interop.Outlook.Application();
                objOutlook = (Microsoft.Office.Interop.Outlook.Application)GenerateEmail("", "", TextEmail, "", "", "");

                //var Outl = Interaction.CreateObject("Outlook.Application");
                //var xOutlookObj = Interaction.CreateObject("Outlook.Application");
                //var xEmailObj = xOutlookObj.CreateItem((object)0);
                //var Obody = xEmailObj.body;
                //xEmailObj.Display();  // .To = "" '.CC = adresat_2
                //xEmailObj.BCC = TextEmail; // .Subject = ""
                //xEmailObj.HtmlBody = xEmailObj.HtmlBody;
                //if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(xEmailObj.Display, false, false)))
                //    xEmailObj.Display();   // .Send ' wyślij bez podglądu
                //object OutApp = null;


            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Mw.VievPageVisibli(false, false, "");
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }



        private void Button1_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Sendt_mail(RichTextBox1.Text);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
    }

    public partial class GrupBranz
    {
        public string Branza { get; set; }
        public string BrTag { get; set; }
    }
}
