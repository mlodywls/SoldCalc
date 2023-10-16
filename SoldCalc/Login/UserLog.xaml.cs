using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SoldCalc.Supporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Login
{
    public partial class UserLog
    {
        private static string KodAktywacja;
        public UserLog()
        {
            Upr_User = new UPR_Ranga();
            Upr_User.Ide = "UserLog";
            InitializeComponent();
            Mw.VisibilityBlockApp();
            //Console.WriteLine("UserLog / UserLog()");
            WyswietlDaneUser();
        }
        public void WyswietlDaneUser()
        {
            PanelDopiszUser.Visibility = Visibility.Collapsed;
            CBRegionalManager.Items.Add("wkonieczny@soudal.pl");
            CBRegionalManager.Items.Add("rgrzeda@soudal.pl");
            CBRegionalManager.Items.Add("jzielinski@soudal.pl");
            Label5.Content = "";
            Label7.Content = "1";
            LoadUserlog = 1;
            Label7.Content = "3";
            try
            {
                sprCennik = ListAktualPH[0][0].ToString() + " " + ListAktualPH[0][1].ToString() != ListAktualKO[0][0].ToString() + " " + ListAktualKO[0][1].ToString();
                sprZakupy = ListAktualPH[1][0].ToString() + " " + ListAktualPH[1][1].ToString() != ListAktualKO[1][0].ToString() + " " + ListAktualKO[1][1].ToString();
            }
            catch (Exception ex)
            {
                // TextMessage(Upr_User.User_PH, ex.StackTrace.ToString)
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtName.Text != "" & TxtLastName.Text != "" & TxtTel.Text != "" & TxtEmail.Text != "" & CBRegionalManager.Text != "")
                {
                    Imie.Content = TxtName.Text;
                    Nazwisko.Content = TxtLastName.Text;
                    email.Content = TxtEmail.Text;
                    DtUser = SqlComandDatabase(StringComand.ReturnComndBazaPH(), PHcon);
                    ControlInfo_Load(DtUser);
                }
                else
                {
                    Label7.Content = "Wprowadz wszystkie dane!!";
                }
            }
            catch (Exception ex)
            {
                //TextMessage(ex.StackTrace.ToString());
            }
        }
        private object ControlInfo_Load(DataTable dt)
        {
            try
            {
                LabInfo.Content = "";
                if (Upr_User == null)
                {
                    Upr_User = new UPR_Ranga();
                    Upr_User.Ide = "ControlInfo_Load(DataTable dt) ";
                }
                for (int i = 0, loopTo = dt.Rows.Count - 1; i <= loopTo; i++)
                {
                    if (Operators.ConditionalCompareObjectEqual(dt.Rows[i]["Email"], TxtEmail.Text, false))
                    {
                        PanelWpisznowy.Visibility = Visibility.Collapsed;
                        PanelDopiszUser.Visibility = Visibility.Visible;
                        LabInfo.Content = "Na twój adres " + Constants.vbCrLf + email.Content + Constants.vbCrLf + " został wysłany kod weryfikacyjny" + Constants.vbCrLf + " Sprawdz pocztę i skopiuj kod, wklejając go poniżej do weryfikacji" + Constants.vbCrLf + "Jeżeli Nie otrzymałeś kodu" + Constants.vbCrLf + "sprawdz folder - wiadomości sieci, lub uruchom program ponwnie";
                        KodSerch.Visibility = Visibility.Visible;
                        ConfirmCode.Visibility = Visibility.Visible;
                        {
                            Upr_User.Imie = TxtName.Text;
                            Upr_User.Nazwisko = TxtLastName.Text;
                            Upr_User.Telefon = TxtTel.Text;
                            Upr_User.User_Email = TxtEmail.Text;
                            Upr_User.KO_email = CBRegionalManager.Text;
                            Upr_User.User_PH = Upr_User.Imie + " " + Upr_User.Nazwisko;
                        }
                        SendEmail();
                        return null;
                    }
                }
                if (TxtEmail.Text.Contains(EmailAdmin) || TxtEmail.Text.Contains(CompanyDomain1) || TxtEmail.Text.Contains(CompanyDomain2))
                {
                    string Tim = TimeAktual();
                    {
                        Upr_User.Imie = TxtName.Text;
                        Upr_User.Nazwisko = TxtLastName.Text;
                        Upr_User.Telefon = TxtTel.Text;
                        Upr_User.User_Email = TxtEmail.Text;
                        Upr_User.KO_email = CBRegionalManager.Text;
                        Upr_User.Ranga = "PH";
                        Upr_User.CenaKO = false;
                        Upr_User.UprKO = false;
                        Upr_User.WyslijInfoDoKO = true;
                        Upr_User.MonitKO = false;
                        Upr_User.Upr4 = false;
                        Upr_User.NrPh = Tim + Strings.Mid(TxtName.Text, 1, 1) + Strings.Mid(TxtLastName.Text, 1, 1) + "/" + GetUserName().ToString();
                        Upr_User.User_PH = Upr_User.Imie + " " + Upr_User.Nazwisko;
                    }
                    PanelWpisznowy.Visibility = Visibility.Collapsed;
                    PanelDopiszUser.Visibility = Visibility.Visible;
                    LabInfo.Content = "Na twój adres " + Constants.vbCrLf + email.Content + Constants.vbCrLf + " został wysłany kod weryfikacyjny" + Constants.vbCrLf + " Sprawdz pocztę i skopiuj kod, wklejając go poniżej do weryfikacji" + Constants.vbCrLf + "Jeżeli Nie otrzymałeś kodu" + Constants.vbCrLf + "sprawdz folder - wiadomości sieci, lub uruchom program ponwnie";
                    KodSerch.Visibility = Visibility.Visible;
                    ConfirmCode.Visibility = Visibility.Visible;
                    SendEmail();
                }
                else
                {
                    Label5.Content = "Niewłasciwy adres e_mail !!!";
                    TxtEmail.Background = Brushes.Red;
                }
            }

            catch (Exception ex)
            {
                //TextMessage(ex.StackTrace.ToString());
            }
            return null;
        }
        private void SendEmail()
        {
            try
            {
                var Letters = new List<int>();
                for (int i = 48; i <= 57; i++)
                    Letters.Add(i);
                for (int i = 97; i <= 122; i++)
                    Letters.Add(i);
                for (int i = 65; i <= 90; i++)
                    Letters.Add(i);
                var Rnd = new Random();
                var SB = new StringBuilder();
                int Temp;
                for (int count = 0; count <= 4; count++)
                {
                    Temp = Rnd.Next(0, Letters.Count);
                    SB.Append(Strings.Chr(Letters[Temp]));
                    KodAktywacja = SB.ToString();
                }
                Console.WriteLine("Kod aktywacyjny UserLog - " + KodAktywacja);
                string subiect = "SoldCalc - Kod aktywacyjny";
                string Body = "Witaj !" + Constants.vbCrLf + Upr_User.Imie + " " + Upr_User.Nazwisko + Constants.vbCrLf + "Wprowadz poniższy kod, aby aktywować produkt" + Constants.vbCrLf + KodAktywacja;
                TxySpamHtml.Text += " Sprawdz również skrzynkę Spam pod adresem, gdzie kod mógł trafić " + Constants.vbCrLf + htmlSpam;
                Message.TextMessageFileAndStart(email.Content.ToString(), TxtEmail2.Text.ToString(), EmailAdmin, subiect, Body, "", "", false);

            }
            catch (Exception ex)
            {
                //TextMessage(ex.StackTrace.ToString());
            }
        }
        private void ConfirmCode_Click(object sender, RoutedEventArgs e)
        {
            string Tim = TimeAktual();
            if (KodSerch.Text == KodAktywacja)
            {
                if (Upr_User.Ranga == "KO")
                    Upr_User.UprKO = true;
                else
                    Upr_User.UprKO = false;
                if (Upr_User.Admin == true)
                    Upr_User.UprKO = true;
                Console.WriteLine("ActualPH 3 ");
                Console.WriteLine("UserLog / ConfirmCode_Click - " + UsingSQLComand(StringComand.ReturnComndBazaUser_PH(), PHcon));
                AktualNwwBaza_PH.StartUpdatePH();
                AktIMGStart = true;
                AktTDSStart = true;
                AktTDSStart = true;
                Mw.Reflash_App();
                RemoveMe();
            }
            else
            {
                LabInfo.Content = "Niewłasciwy kod." + Constants.vbCrLf + "Wysłano ponownie!";
                ConfirmCode.Content = "Potwierdz ponownie";
                KodSerch.Text = "";
            }
            if (ConfirmCode.Content.ToString() == "Potwierdz ponownie")
            {
                SendEmail();
            }
        }

        public void RemoveMe()
        {
            try
            {
                MainWindow.UkryjPanel();
                DockPanel Par = (DockPanel)this.Parent;
                Par.Children.Remove(this);
            }
            catch (Exception ex)
            {
                //TextMessage(ex.StackTrace.ToString());
            }
        }

        private void StackPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter | e.Key == Key.Down)
                {
                    TextBox s = e.Source as TextBox;
                    if (s != null)
                        s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
                if (e.Key == Key.Up)
                {
                    TextBox s = e.Source as TextBox;
                    if (s != null)
                        s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                //TextMessage(ex.StackTrace.ToString());
            }
        }
        private void TextBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtEmail.Background = Brushes.White;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Mw.Panel_Pierwsze_Logowanie.Visibility = Visibility.Collapsed;
        }

    }
}

