using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Media;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc
{
    public partial class App 
    {
       
    }

    public class SeitingList
    {
        public double ColNameProdWidth { get; set; }
    }

    public class UPR_Ranga : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(object propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName.ToString()));
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public string Ide { get; set; }
        private string _User_PH { get; set; }
        public string User_PH
        {
            get
            {
                return _User_PH;
            }
            set
            {
                if (_User_PH != value)
                {
                    _User_PH = value; OnPropertyChanged("User_PH");
                }
            }
        }

        private int _Id { get; set; }
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value; OnPropertyChanged("Id");
                }
            }
        }
        private string _Ranga { get; set; }
        public string Ranga
        {
            get
            {
                return _Ranga;
            }
            set
            {
                if (_Ranga != value)
                {
                    _Ranga = value; OnPropertyChanged("Ranga");
                }
            }
        }
        private string _Rejon { get; set; }
        public string Rejon
        {
            get
            {
                return _Rejon;
            }
            set
            {
                if (_Rejon != value)
                {
                    _Rejon = value; OnPropertyChanged("Rejon");
                }
            }
        }

        private string _Imie { get; set; }
        public string Imie
        {
            get
            {
                return _Imie;
            }
            set
            {
                if (_Imie != value)
                {
                    _Imie = value; OnPropertyChanged("Imie");
                }
            }
        }
        private string _Nazwisko { get; set; }
        public string Nazwisko
        {
            get
            {
                return _Nazwisko;
            }
            set
            {
                if (_Nazwisko != value)
                {
                    _Nazwisko = value; OnPropertyChanged("Nazwisko");
                }
            }
        }
        private bool _Admin { get; set; }
        public bool Admin
        {
            get
            {
                return _Admin;
            }
            set
            {
                if (_Admin != value)
                {
                    _Admin = value; OnPropertyChanged("Admin");
                }
            }
        }

        private bool _Viev_Admin { get; set; }
        public bool Viev_Admin
        {
            get
            {
                return _Viev_Admin;
            }
            set
            {
                if (_Viev_Admin != value)
                {
                    _Viev_Admin = value; OnPropertyChanged("Viev_Admin");
                }
            }
        }
        private string _Telefon { get; set; }
        public string Telefon
        {
            get
            {
                return _Telefon;
            }
            set
            {
                if (_Telefon != value)
                {
                    _Telefon = value; OnPropertyChanged("Telefon");
                }
            }
        }
        private bool _UprKO { get; set; }
        public bool UprKO
        {
            get
            {
                return _UprKO;
            }
            set
            {
                if (_UprKO != value)
                {
                    _UprKO = value; OnPropertyChanged("UprKO");
                }
            }
        }
        private string _Email { get; set; }
        public string User_Email
        {
            get
            {
                return _Email;
            }
            set
            {
                if (_Email != value)
                {
                    _Email = value; OnPropertyChanged("User_Email");
                }
            }
        }


        private string _KO_email { get; set; }
        public string KO_email
        {
            get
            {
                return _KO_email;
            }
            set
            {
                if (_KO_email != value)
                {
                    _KO_email = value; OnPropertyChanged("KO_email");
                }
            }
        }
        private bool _CenaKO { get; set; }
        public bool CenaKO
        {
            get
            {
                return _CenaKO;
            }
            set
            {
                if (_CenaKO != value)
                {
                    _CenaKO = value; OnPropertyChanged("CenaKO");
                }
            }
        }
        private bool _WyslijInfoDoKO { get; set; }
        public bool WyslijInfoDoKO
        {
            get
            {
                return _WyslijInfoDoKO;
            }
            set
            {
                if (_WyslijInfoDoKO != value)
                {
                    _WyslijInfoDoKO = value; OnPropertyChanged("WyslijInfoDoKO");
                }
            }
        }
        private bool _MonitKO { get; set; }
        public bool MonitKO
        {
            get
            {
                return _MonitKO;
            }
            set
            {
                if (_MonitKO != value)
                {
                    _MonitKO = value; OnPropertyChanged("MonitKO");
                }
            }
        }
        private bool _Upr4 { get; set; }
        public bool Upr4
        {
            get
            {
                return _Upr4;
            }
            set
            {
                if (_Upr4 != value)
                {
                    _Upr4 = value; OnPropertyChanged("Upr4");
                }
            }
        }
        private string _NrPh { get; set; }
        public string NrPh
        {
            get
            {
                return _NrPh;
            }
            set
            {
                if (_NrPh != value)
                {
                    _NrPh = value; OnPropertyChanged("NrPh");
                }
            }
        }
        private string _OstAkt { get; set; }
        public string OstAkt
        {
            get
            {
                return _OstAkt;
            }
            set
            {
                if (_OstAkt != value)
                {
                    _OstAkt = value; OnPropertyChanged("OstAkt");
                }
            }
        }
        private double _Scale { get; set; }
        public double Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                if (_Scale != value)
                {
                    _Scale = value; OnPropertyChanged("Scale");
                }
            }
        }
        private ScaleTransform _dpiTransform { get; set; }
        public ScaleTransform dpiTransform
        {
            get
            {
                return _dpiTransform;
            }
            set
            {
                _dpiTransform = value; OnPropertyChanged("dpiTransform");
            }
        }

        private string _MaxData { get; set; }
        public string MaxData
        {
            get
            {
                return _MaxData;
            }
            set
            {
                if (_MaxData != value)
                {
                    _MaxData = value; OnPropertyChanged("MaxData");
                }
            }
        }

        private int _MaxD { get; set; }
        public int MaxD
        {
            get
            {
                return _MaxD;
            }
            set
            {
                if (_MaxD != value)
                {
                    _MaxD = value; OnPropertyChanged("MaxD");
                }
            }
        }

        private string _MaxDataRok { get; set; }
        public string MaxDataRok
        {
            get
            {
                return _MaxDataRok;
            }
            set
            {
                if (_MaxDataRok != value)
                {
                    _MaxDataRok = value; OnPropertyChanged("MaxDataRok");
                }
            }
        }

        private string _MinData { get; set; }
        public string MinData
        {
            get
            {
                return _MinData;
            }
            set
            {
                if (_MinData != value)
                {
                    _MinData = value; OnPropertyChanged("MinData");
                }
            }
        }


        private int _MinD { get; set; }
        public int MinD
        {
            get
            {
                return _MinD;
            }
            set
            {
                if (_MinD != value)
                {
                    _MinD = value; OnPropertyChanged("MinD");
                }
            }
        }

        private int _O_Data { get; set; }
        public int O_Data
        {
            get
            {
                return _O_Data;
            }
            set
            {
                if (_O_Data != value)
                {
                    _O_Data = value; OnPropertyChanged("O_Data");
                }
            }
        }
        private bool _WyswCennikAdmin { get; set; }
        public bool WyswCennikAdmin
        {
            get
            {
                return _WyswCennikAdmin;
            }
            set
            {
                if (_WyswCennikAdmin != value)
                {
                    _WyswCennikAdmin = value; OnPropertyChanged("WyswCennikAdmin");
                }
            }
        }
        private string _SelectedDateMin { get; set; }
        public string SelectedDateMin
        {
            get
            {
                return _SelectedDateMin;
            }
            set
            {
                if (_SelectedDateMin != value)
                {
                    _SelectedDateMin = value; OnPropertyChanged("SelectedDateMin");
                }
            }
        }
        private string _SelectedDateMax { get; set; }
        public string SelectedDateMax
        {
            get
            {
                return _SelectedDateMax;
            }
            set
            {
                if (_SelectedDateMax != value)
                {
                    _SelectedDateMax = value; OnPropertyChanged("SelectedDateMax");
                }
            }
        }
        private bool _SendDB { get; set; }
        public bool SendDB 
        {
            get
            {
                return _SendDB;
            }
            set
            {
                if (_SendDB != value)
                {
                    _SendDB = value; OnPropertyChanged("SendDB");
                }
            }
        }
    }
    public class KlientData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private event PropertyChangedEventHandler INotifyPropertyChanged_PropertyChanged;

        public void NotifyPropertyChanged(object propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName.ToString()));
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private int _Id { get; set; }
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value; OnPropertyChanged("Id");
                }
            }
        }
        private string _Opiekun_klienta { get; set; }
        public string Opiekun_klienta
        {
            get
            {
                return _Opiekun_klienta;
            }
            set
            {
                if (_Opiekun_klienta != value)
                {
                    _Opiekun_klienta = value; OnPropertyChanged("Opiekun_klienta");
                }
            }
        }
        private string _NIP { get; set; }
        public string NIP
        {
            get
            {
                return _NIP;
            }
            set
            {
                if (_NIP != value)
                {
                    _NIP = value; OnPropertyChanged("NIP");
                }
            }
        }
        private string _Stan { get; set; }
        public string Stan
        {
            get
            {
                return _Stan;
            }
            set
            {
                if (_Stan != value)
                {
                    _Stan = value; OnPropertyChanged("Stan");
                }
            }
        }
        private string _Numer_konta { get; set; }
        public string Numer_konta
        {
            get
            {
                return _Numer_konta;
            }
            set
            {
                if (_Numer_konta != value)
                {
                    _Numer_konta = value; OnPropertyChanged("Numer_konta");
                }
            }
        }
        private string _Nazwa_klienta { get; set; }
        public string Nazwa_klienta
        {
            get
            {
                return _Nazwa_klienta;
            }
            set
            {
                if (_Nazwa_klienta != value)
                {
                    _Nazwa_klienta = value; OnPropertyChanged("Nazwa_klienta");
                }
            }
        }
        private string _Nazwa_CD { get; set; }
        public string Nazwa_CD
        {
            get
            {
                return _Nazwa_CD;
            }
            set
            {
                if (_Nazwa_CD != value)
                {
                    _Nazwa_CD = value; OnPropertyChanged("Nazwa_CD");
                }
            }
        }
        private string _Adres { get; set; }
        public string Adres
        {
            get
            {
                return _Adres;
            }
            set
            {
                if (_Adres != value)
                {
                    _Adres = value; OnPropertyChanged("Adres");
                }
            }
        }
        private string _Kod_Poczta { get; set; }
        public string Kod_Poczta
        {
            get
            {
                return _Kod_Poczta;
            }
            set
            {
                if (_Kod_Poczta != value)
                {
                    _Kod_Poczta = value; OnPropertyChanged("Kod_Poczta");
                }
            }
        }
        private string _Poczta { get; set; }
        public string Poczta
        {
            get
            {
                return _Poczta;
            }
            set
            {
                if (_Poczta != value)
                {
                    _Poczta = value; OnPropertyChanged("Poczta");
                }
            }
        }
        private string _Forma_plac { get; set; }
        public string Forma_plac
        {
            get
            {
                return _Forma_plac;
            }
            set
            {
                if (_Forma_plac != value)
                {
                    _Forma_plac = value; OnPropertyChanged("Forma_plac");
                }
            }
        }
        private string _PraceList { get; set; }
        public string PraceList
        {
            get
            {
                return _PraceList;
            }
            set
            {
                if (_PraceList != value)
                {
                    _PraceList = value; OnPropertyChanged("PraceList");
                }
            }
        }
        private string _Branza { get; set; }
        public string Branza
        {
            get
            {
                return _Branza;
            }
            set
            {
                if (_Branza != value)
                {
                    _Branza = value; OnPropertyChanged("Branza");
                }
            }
        }
        private string _Tel { get; set; }
        public string Tel
        {
            get
            {
                return _Tel;
            }
            set
            {
                if (_Tel != value)
                {
                    _Tel = value; OnPropertyChanged("Tel");
                }
            }
        }
        private string _E_mail { get; set; }
        public string E_mail
        {
            get
            {
                return _E_mail;
            }
            set
            {
                if (_E_mail != value)
                {
                    _E_mail = value; OnPropertyChanged("E_mail");
                }
            }
        }
        private string _Branzysta { get; set; }
        public string Branzysta
        {
            get
            {
                return _Branzysta;
            }
            set
            {
                if (_Branzysta != value)
                {
                    _Branzysta = value; OnPropertyChanged("Branzysta");
                }
            }
        }
        private string _BranzystaEmail { get; set; }
        public string BranzystaEmail
        {
            get
            {
                return _BranzystaEmail;
            }
            set
            {
                if (_BranzystaEmail != value)
                {
                    _BranzystaEmail = value; OnPropertyChanged("BranzystaEmail");
                }
            }
        }
        private string _TollTipInfo { get; set; }
        public string TollTipInfo
        {
            get
            {
                return _TollTipInfo;
            }
            set
            {
                if (_TollTipInfo != value)
                {
                    _TollTipInfo = value; OnPropertyChanged("TollTipInfo");
                }
            }
        }
        private double _Rabat { get; set; }
        public double Rabat_Double
        {
            get
            {
                return _Rabat;
            }
            set
            {
                if (_Rabat != value)
                {
                    _Rabat = value; OnPropertyChanged("Rabat_Double");
                }
            }
        }
    }

    public class CennikData
    {

        public List[] SelectIndexCennik { get; set; }
        public bool CbSelectRow { get; set; }
        public bool IsSelected { get; set; }
        public bool IsEnabled { get; set; }
        public int Id { get; set; }
        public string ProdKod { get; set; }
        public string Naglowek { get; set; }
        public string Lpgrup { get; set; }
        public string SAP { get; set; }
        public string NazwProd { get; set; }
        public string Kszt { get; set; }
        public string Pszt { get; set; }
        public string Poj { get; set; }
        public string Miara { get; set; }
        public string Kolor { get; set; }
        public double CDM { get; set; }
        public double CK { get; set; }
        public double PH { get; set; }
        public double ZPR0 { get; set; }
        public string GRUPA { get; set; }
        public string KATEGORIA { get; set; }
        public string NAZEWNICTWO { get; set; }
        public string BrakPrace { get; set; }
        public object Img { get; set; } // As Object
        public object Tds { get; set; } // As Object
        public object KartaCHAR { get; set; } // As Object
        public bool Tds_Ok_True { get; set; }
        public bool Kchar_Ok_True { get; set; }
        public string TruePath { get; set; }
        public string FalsePath { get; set; }
        public string ND1 { get; set; }
        public string CenaZPrace { get; set; }
        public bool CenaKOCennik { get; set; }
    }

    public class OFRData
    {
        public string Id { get; set; }
        public string SAPnr { get; set; }
        public string NazwOFR { get; set; }
        public string NazwKlient { get; set; }
        public string Opiekun { get; set; }
        public object OFR { get; set; }
    }

    public class DaneKlient : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private event PropertyChangedEventHandler INotifyPropertyChanged_PropertyChanged;

        public void NotifyPropertyChanged(object propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName.ToString()));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int _Id { get; set; }
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value; OnPropertyChanged("Id");
                }
            }
        }
        private string _Opiekun_klienta { get; set; }
        public string Opiekun_klienta
        {
            get
            {
                return _Opiekun_klienta;
            }
            set
            {
                if (_Opiekun_klienta != value)
                {
                    _Opiekun_klienta = value; OnPropertyChanged("Opiekun_klienta");
                }
            }
        }
        private string _NIP { get; set; }
        public string NIP
        {
            get
            {
                return _NIP;
            }
            set
            {
                if (_NIP != value)
                {
                    _NIP = value; OnPropertyChanged("NIP");
                }
            }
        }
        private string _Stan { get; set; }
        public string Stan
        {
            get
            {
                return _Stan;
            }
            set
            {
                if (_Stan != value)
                {
                    _Stan = value; OnPropertyChanged("Stan");
                }
            }
        }
        private string _Numer_konta { get; set; }
        public string Numer_konta
        {
            get
            {
                return _Numer_konta;
            }
            set
            {
                if (_Numer_konta != value)
                {
                    _Numer_konta = value; OnPropertyChanged("Numer_konta");
                }
            }
        }
        private string _Nazwa_klienta { get; set; }
        public string Nazwa_klienta
        {
            get
            {
                return _Nazwa_klienta;
            }
            set
            {
                if (_Nazwa_klienta != value)
                {
                    _Nazwa_klienta = value; OnPropertyChanged("Nazwa_klienta");
                }
            }
        }
        private string _Nazwa_CD { get; set; }
        public string Nazwa_CD
        {
            get
            {
                return _Nazwa_CD;
            }
            set
            {
                if (_Nazwa_CD != value)
                {
                    _Nazwa_CD = value; OnPropertyChanged("Nazwa_CD");
                }
            }
        }
        private string _Adres { get; set; }
        public string Adres
        {
            get
            {
                return _Adres;
            }
            set
            {
                if (_Adres != value)
                {
                    _Adres = value; OnPropertyChanged("Adres");
                }
            }
        }
        private string _Kod_Poczta { get; set; }
        public string Kod_Poczta
        {
            get
            {
                return _Kod_Poczta;
            }
            set
            {
                if (_Kod_Poczta != value)
                {
                    _Kod_Poczta = value; OnPropertyChanged("Kod_Poczta");
                }
            }
        }
        private string _Poczta { get; set; }
        public string Poczta
        {
            get
            {
                return _Poczta;
            }
            set
            {
                if (_Poczta != value)
                {
                    _Poczta = value; OnPropertyChanged("Poczta");
                }
            }
        }
        private string _Forma_plac { get; set; }
        public string Forma_plac
        {
            get
            {
                return _Forma_plac;
            }
            set
            {
                if (_Forma_plac != value)
                {
                    _Forma_plac = value; OnPropertyChanged("Forma_plac");
                }
            }
        }
        private string _PraceList { get; set; }
        public string PraceList
        {
            get
            {
                return _PraceList;
            }
            set
            {
                if (_PraceList != value)
                {
                    _PraceList = value; OnPropertyChanged("PraceList");
                }
            }
        }
        private string _Branza { get; set; }
        public string Branza
        {
            get
            {
                return _Branza;
            }
            set
            {
                if (_Branza != value)
                {
                    _Branza = value; OnPropertyChanged("Branza");
                }
            }
        }
        private string _Tel { get; set; }
        public string Tel
        {
            get
            {
                return _Tel;
            }
            set
            {
                if (_Tel != value)
                {
                    _Tel = value; OnPropertyChanged("Tel");
                }
            }
        }
        private string _E_mail { get; set; }
        public string E_mail
        {
            get
            {
                return _E_mail;
            }
            set
            {
                if (_E_mail != value)
                {
                    _E_mail = value; OnPropertyChanged("E_mail");
                }
            }
        }
        private string _Branzysta { get; set; }
        public string Branzysta
        {
            get
            {
                return _Branzysta;
            }
            set
            {
                if (_Branzysta != value)
                {
                    _Branzysta = value; OnPropertyChanged("Branzysta");
                }
            }
        }
        private string _BranzystaEmail { get; set; }
        public string BranzystaEmail
        {
            get
            {
                return _BranzystaEmail;
            }
            set
            {
                if (_BranzystaEmail != value)
                {
                    _BranzystaEmail = value; OnPropertyChanged("BranzystaEmail");
                }
            }
        }
        private string _TollTipInfo { get; set; }
        public string TollTipInfo
        {
            get
            {
                return _TollTipInfo;
            }
            set
            {
                if (_TollTipInfo != value)
                {
                    _TollTipInfo = value; OnPropertyChanged("TollTipInfo");
                }
            }
        }
        private double _Rabat { get; set; }
        public double Rabat_Double
        {
            get
            {
                return _Rabat;
            }
            set
            {
                if (_Rabat != value)
                {
                    _Rabat = value; OnPropertyChanged("Rabat_Double");
                }
            }
        }
    }

    public class ListTblOfr : ObservableCollection<TblOfr>
    {

        private static List<TblOfr> _TblList { get; set; } = new List<TblOfr>();

        public static List<TblOfr> Tbl_Add_prodList
        {
            get
            {
                return _TblList;
            }
            set
            {
                _TblList = value;
            }
        }
    }

    public class TblOfr : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private event PropertyChangedEventHandler INotifyPropertyChanged_PropertyChanged;
        public void NotifyPropertyChanged(object propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName.ToString()));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public string Null_As_String(string Value)
        {
            try
            {
                if (Value == null || Information.IsDBNull(Value))
                    return "";
                else
                    return Value.ToString(); 
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private string return_ComandUpdate(string Szukane, string Wstawiane, string Set)
        {
            Tim = TimeAktual();
            Wstawiane = Null_As_String(Wstawiane); 
            string set_Val = Set + "='"; 
            string Nip = Get_KlientDane.NIP;
            string Sqwert = @" -- Try to update any existing row 
                                        UPDATE BazaOfr_robocze
                                        SET NIP= '" + Nip + "', " + set_Val + Wstawiane.ToString() + "',OstAkt='" + Tim + @"'
                                        where SAP=" + Szukane + "  And NIP=" + Nip + @" ;

                                    -- If no update happened (i.e. the row didn't exist) then insert one                                        
                                        INSERT INTO BazaOfr_robocze  (NIP,'" + Set + @"',OstAkt)
                                        SELECT '" + Nip + "','" + Wstawiane.ToString() + "','" + Tim + @"'
                                        WHERE (Select Changes() = 0);";
            return Sqwert;
        }
        public string TblOfr_Return_Marza(double NowaCena, double CDM)
        {
            double Marza = default(Double);
            try
            {
                if (NowaCena > 0)
                    Marza = Math.Round(((NowaCena - CDM) / CDM) * 100, 2);
                else
                    Marza = default(Double);
            }
            catch
            {
            }

            return Marza.ToString();
        }
        public double TblOfr_Return_ZK(double CDO, double CZP)
        {
            double ZK = default(Double);
            try
            {

                if (((CDO != 0) && (CZP != 0)) || ((CDO != default) && (CZP != default)))
                {
                    ZK = double.Parse(Strings.Replace(Math.Round((CDO / CZP * 100) - 100, 2, MidpointRounding.AwayFromZero).ToString(), "-", ""));
                }
                if (ZK == 100)
                {
                    ZK = 0;
                }

            }
            catch
            {
                ZK = 0;
            }
            return ZK;
        }

        public TblOfr() { }
        public TblOfr(string sap, string nD, int iD, string naglowek, string lpgrup, string nazwProd, string poj, double cDM, double kO, double pH, double zPR0, string gRUPA,
            string kATEGORIA, string nAZEWNICTWO, string brakPrace, double cenaZPrace, double cenaDoOFR, double cenaBrutto, double cenaDoOFR2,
            double cenaBrutto2, double cenaDoOFR3, double cenaBrutto3, string marza, string marza2, string marza3, double zK11A1, double zK11A2, double zK11A3,
            string szt_1, string szt_2, string szt_3, double cena_zapis_do_OFR, string opis_Cena_zapis_do_OFR, string opis_Cena_zapis_do_OFR2, string opis_Cena_zapis_do_OFR3,
            object img, object tDS, object kCH, bool plik_Tds_True, bool plik_Kch_True, string fileName, bool tDS_DO_OFR, bool cHAR_DO_OFR, bool pDF_DO_OFR, string nazwaPdf)
        {
            SAP = sap; ND = nD; ID = iD; Naglowek = naglowek; Lpgrup = lpgrup; NazwProd = nazwProd; Poj = poj; CDM = cDM; KO = kO; PH = pH; ZPR0 = zPR0;
            GRUPA = gRUPA; KATEGORIA = kATEGORIA; NAZEWNICTWO = nAZEWNICTWO;
            BrakPrace = brakPrace;
            CenaZPrace = cenaZPrace; CenaDoOFR = cenaDoOFR; CenaBrutto = cenaBrutto; CenaDoOFR2 = cenaDoOFR2; CenaBrutto2 = cenaBrutto2; CenaDoOFR3 = cenaDoOFR3; CenaBrutto3 = cenaBrutto3;
            Marza = marza; Marza2 = marza2; Marza3 = marza3;
            ZK11A1 = zK11A1; ZK11A2 = zK11A2; ZK11A3 = zK11A3;
            szt1 = szt_1; szt2 = szt_2; szt3 = szt_3;
            Cena_zapis_do_OFR = cena_zapis_do_OFR;
            Opis_Cena_zapis_do_OFR = opis_Cena_zapis_do_OFR; Opis_Cena_zapis_do_OFR2 = opis_Cena_zapis_do_OFR2; Opis_Cena_zapis_do_OFR3 = opis_Cena_zapis_do_OFR3;
            Img = img; TDS = tDS; KCH = kCH; Plik_Tds_True = plik_Tds_True; Plik_Kch_True = plik_Kch_True;
            FileName = fileName; TDS_DO_OFR = tDS_DO_OFR; CHAR_DO_OFR = cHAR_DO_OFR; PDF_DO_OFR = pDF_DO_OFR; NazwaPdf = nazwaPdf;
        }



        private double VAT = 1.23;
        private string _SAP { get; set; }
        public string SAP
        {
            get
            {
                return _SAP;
            }
            set
            {
                if (_SAP != value)
                {
                    _SAP = value; OnPropertyChanged("SAP"); UsingSQLComand(return_ComandUpdate(SAP, _SAP, "SAP"), con);
                }
            }
        }
        private string _ND { get; set; }
        public string ND
        {
            get
            {
                return _ND;
            }
            set
            {
                if (_ND != value)
                {
                    _ND = value; OnPropertyChanged("ND");
                }
            }
        }
        private int _ID { get; set; }
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    _ID = value; OnPropertyChanged("ID");
                } 
            }
        }
        private string _Naglowek { get; set; }
        public string Naglowek
        {
            get
            {
                return _Naglowek;
            }
            set
            {
                if (_Naglowek != value)
                {
                    _Naglowek = value; OnPropertyChanged("Naglowek"); UsingSQLComand(return_ComandUpdate(SAP, _Naglowek, "Naglowek"), con);
                }
            }
        }
        private string _Lpgrup { get; set; }
        public string Lpgrup
        {
            get
            {
                return _Lpgrup;
            }
            set
            {
                if (_Lpgrup != value)
                {
                    _Lpgrup = value; OnPropertyChanged("Lpgrup"); UsingSQLComand(return_ComandUpdate(SAP, _Lpgrup, "Lpgrup"), con);
                }
            }
        }

        private string _NazwProd { get; set; }
        public string NazwProd
        {
            get
            {
                return _NazwProd;
            }
            set
            {
                if (_NazwProd != value)
                {
                    _NazwProd = value; OnPropertyChanged("NazwProd"); UsingSQLComand(return_ComandUpdate(SAP, _NazwProd, "NazwProd"), con);
                }
            }
        }
        private string _Kszt { get; set; }
        public string Kszt
        {
            get
            {
                return _Kszt;
            }
            set
            {
                if (_Kszt != value)
                {
                    _Kszt = value; OnPropertyChanged("Kszt"); UsingSQLComand(return_ComandUpdate(SAP, _Kszt, "Kszt"), con);
                }
            }
        }
        private string _Poj { get; set; }
        public string Poj
        {
            get
            {
                return _Poj;
            }
            set
            {
                if (_Poj != value)
                {
                    _Poj = value; OnPropertyChanged("Poj"); UsingSQLComand(return_ComandUpdate(SAP, _Poj, "Poj"), con);
                }
            }
        }
        private double _CDM { get; set; }
        public double CDM
        {
            get
            {
                return _CDM;
            }
            set
            {
                if (_CDM != value)
                {
                    _CDM = value; OnPropertyChanged("CDM"); UsingSQLComand(return_ComandUpdate(SAP, _CDM.ToString(), "CDM"), con);
                }
            }
        }
        private double _KO { get; set; }
        public double KO
        {
            get
            {
                return _KO;
            }
            set
            {
                if (_KO != value)
                {
                    _KO = value; OnPropertyChanged("KO"); UsingSQLComand(return_ComandUpdate(SAP, _KO.ToString(), "KO"), con);
                }
            }
        }
        private double _PH { get; set; }
        public double PH
        {
            get
            {
                return _PH;
            }
            set
            {
                if (_PH != value)
                {
                    _PH = value; OnPropertyChanged("PH"); UsingSQLComand(return_ComandUpdate(SAP, _PH.ToString(), "PH"), con);
                }
            }
        }
        private double _ZPR0 { get; set; }
        public double ZPR0
        {
            get
            {
                return _ZPR0;
            }
            set
            {
                if (_ZPR0 != value)
                {
                    _ZPR0 = value; OnPropertyChanged("ZPR0"); UsingSQLComand(return_ComandUpdate(SAP, _ZPR0.ToString(), "ZPR0"), con);
                }
            }
        }
        private string _GRUPA { get; set; }
        public string GRUPA
        {
            get
            {
                return _GRUPA;
            }
            set
            {
                if (_GRUPA != value)
                {
                    _GRUPA = value; OnPropertyChanged("GRUPA"); UsingSQLComand(return_ComandUpdate(SAP, _GRUPA, "GRUPA"), con);
                }
            }
        }
        private string _KATEGORIA { get; set; }
        public string KATEGORIA
        {
            get
            {
                return _KATEGORIA;
            }
            set
            {
                if (_KATEGORIA != value)
                {
                    _KATEGORIA = value; OnPropertyChanged("KATEGORIA"); UsingSQLComand(return_ComandUpdate(SAP, _KATEGORIA, "KATEGORIA"), con);
                }
            }
        }
        private string _NAZEWNICTWO { get; set; }
        public string NAZEWNICTWO
        {
            get
            {
                return _NAZEWNICTWO;
            }
            set
            {
                if (_NAZEWNICTWO != value)
                {
                    _NAZEWNICTWO = value; OnPropertyChanged("NAZEWNICTWO"); UsingSQLComand(return_ComandUpdate(SAP, _NAZEWNICTWO, "NAZEWNICTWO"), con);
                }
            }
        }
        private string _BrakPrace { get; set; }
        public string BrakPrace
        {
            get
            {
                return _BrakPrace;
            }
            set
            {
                if (_BrakPrace != value)
                {
                    _BrakPrace = value; OnPropertyChanged("BrakPrace"); UsingSQLComand(return_ComandUpdate(SAP, _BrakPrace, "BrakPrace"), con);
                }
            }
        }
        private double _CenaZPrace { get; set; }
        public double CenaZPrace
        {
            get
            {
                return _CenaZPrace;
            }
            set
            {
                if (_CenaZPrace != value)
                {
                    _CenaZPrace = value; OnPropertyChanged("CenaZPrace"); UsingSQLComand(return_ComandUpdate(SAP, _CenaZPrace.ToString(), "CenaZPrace"), con);
                }
            }
        }
        private double _CenaDoOFR { get; set; }

        public double CenaDoOFR
        {
            get
            {
                return _CenaDoOFR;
            }
            set
            {
                if (_CenaDoOFR != value)
                {
                    _CenaDoOFR = value;
                    OnPropertyChanged("CenaDoOFR");
                    UsingSQLComand(return_ComandUpdate(SAP, _CenaDoOFR.ToString(), "CenaDoOFR"), con);
                    CenaBrutto = Math.Round((_CenaDoOFR * VAT), 2);
                    Marza = TblOfr_Return_Marza(_CenaDoOFR, CDM); 
                    ZK11A1 = TblOfr_Return_ZK(_CenaDoOFR, CenaZPrace);
                }
            }
        }

        private double _CenaBrutto { get; set; }
        public double CenaBrutto
        {
            get
            {
                return _CenaBrutto; 
            }
            set
            {
                if (_CenaBrutto != value)
                {
                    _CenaBrutto = Math.Round(value, 2); OnPropertyChanged("CenaBrutto");
                } 
            }
        }
        private double _CenaDoOFR2 { get; set; }
        public double CenaDoOFR2
        {
            get
            {
                return _CenaDoOFR2;
            }
            set
            {
                if (_CenaDoOFR2 != value)
                {
                    _CenaDoOFR2 = value;
                    OnPropertyChanged("CenaDoOFR2");
                    UsingSQLComand(return_ComandUpdate(SAP, _CenaDoOFR2.ToString(), "CenaDoOFR2"), con);
                    CenaBrutto2 = Math.Round(_CenaDoOFR2 * 1.23, 2);
                    Marza2 = TblOfr_Return_Marza(_CenaDoOFR2, CDM); 
                    ZK11A2 = TblOfr_Return_ZK(_CenaDoOFR2, CenaZPrace);
                }
            }
        }// 13
        private double _CenaBrutto2 { get; set; }
        public double CenaBrutto2
        {
            get
            {
                return _CenaBrutto2; 
            }
            set
            {

                if (_CenaBrutto2 != value)
                {
                    _CenaBrutto2 = Math.Round(value, 2); OnPropertyChanged("CenaBrutto2");
                }
            }
        }
        private double _CenaDoOFR3 { get; set; }
        public double CenaDoOFR3
        {
            get
            {
                return _CenaDoOFR3;
            }
            set
            {
                if (_CenaDoOFR3 != value)
                {
                    _CenaDoOFR3 = value;
                    OnPropertyChanged("CenaDoOFR3");
                    UsingSQLComand(return_ComandUpdate(SAP, _CenaDoOFR3.ToString(), "CenaDoOFR3"), con);
                    CenaBrutto3 = Math.Round(_CenaDoOFR3 * 1.23, 2);
                    Marza3 = TblOfr_Return_Marza(_CenaDoOFR3, CDM); 
                    ZK11A3 = TblOfr_Return_ZK(_CenaDoOFR3, CenaZPrace);
                }
            }
        }
        private double _CenaBrutto3 { get; set; }
        public double CenaBrutto3
        {
            get
            {
                return _CenaBrutto3;
            }
            set
            {
                if (_CenaBrutto3 != value)
                {
                    _CenaBrutto3 = Math.Round(value, 2); OnPropertyChanged("CenaBrutto3");
                } 
            }
        }
        private string _Marza { get; set; }
        public string Marza
        {
            get
            {
                return _Marza;
            }
            set
            {
                if (_Marza != value)
                {
                    _Marza = value; OnPropertyChanged("Marza"); UsingSQLComand(return_ComandUpdate(SAP, _Marza, "Marza"), con);
                }
            }
        }
        private string _Marza2 { get; set; }
        public string Marza2
        {
            get
            {
                return _Marza2;
            }
            set
            {
                if (_Marza2 != value)
                {
                    _Marza2 = value; OnPropertyChanged("Marza2"); UsingSQLComand(return_ComandUpdate(SAP, _Marza2, "Marza2"), con);
                }
            }
        }
        private string _Marza3 { get; set; }
        public string Marza3
        {
            get
            {
                return _Marza3;
            }
            set
            {
                if (_Marza3 != value)
                {
                    _Marza3 = value; OnPropertyChanged("Marza3"); UsingSQLComand(return_ComandUpdate(SAP, _Marza3, "Marza3"), con);
                }
            }
        }
        private double _ZK11A1 { get; set; } 
        public double ZK11A1
        {
            get
            {
                return _ZK11A1;
            }
            set
            {
                if (_ZK11A1 != value)
                {
                    _ZK11A1 = value;
                    OnPropertyChanged("ZK11A1"); UsingSQLComand(return_ComandUpdate(SAP, _ZK11A1.ToString(), "ZK11A1"), con);
                }
            }
        }
        private double _ZK11A2 { get; set; }
        public double ZK11A2
        {
            get
            {
                return _ZK11A2;
            }
            set
            {
                if (_ZK11A2 != value)
                {
                    _ZK11A2 = value; OnPropertyChanged("ZK11A2"); UsingSQLComand(return_ComandUpdate(SAP, _ZK11A2.ToString(), "ZK11A2"), con);
                }
            }
        }
        private double _ZK11A3 { get; set; }
        public double ZK11A3
        {
            get
            {
                return _ZK11A3;
            }
            set
            {
                if (_ZK11A3 != value)
                {
                    _ZK11A3 = value; OnPropertyChanged("ZK11A3"); UsingSQLComand(return_ComandUpdate(SAP, _ZK11A3.ToString(), "ZK11A3"), con);
                }
            }
        }
        private string _szt1 { get; set; }
        public string szt1
        {
            get
            {
                return _szt1;
            }
            set
            {
                if (_szt1 != value)
                {
                    _szt1 = value; OnPropertyChanged("szt1"); UsingSQLComand(return_ComandUpdate(SAP, _szt1, "szt1"), con);
                }
            }
        }
        private string _szt2 { get; set; }
        public string szt2
        {
            get
            {
                return _szt2;
            }
            set
            {
                if (_szt2 != value)
                {
                    _szt2 = value; OnPropertyChanged("szt2"); UsingSQLComand(return_ComandUpdate(SAP, _szt2, "szt2"), con);
                }
            }
        }
        private string _szt3 { get; set; }
        public string szt3
        {
            get
            {
                return _szt3;
            }
            set
            {
                if (_szt3 != value)
                {
                    _szt3 = value; OnPropertyChanged("szt3"); UsingSQLComand(return_ComandUpdate(SAP, _szt3, "szt3"), con);
                }
            }
        }
        private double _Cena_zapis_do_OFR { get; set; }
        public double Cena_zapis_do_OFR
        {
            get
            {
                return _Cena_zapis_do_OFR;
            }
            set
            {
                if (_Cena_zapis_do_OFR != value)
                {
                    _Cena_zapis_do_OFR = value; OnPropertyChanged("Cena_zapis_do_OFR"); UsingSQLComand(return_ComandUpdate(SAP, _Cena_zapis_do_OFR.ToString(), "Cena_zapis_do_OFR"), con);
                }
            }
        }
        private string _Opis_Cena_zapis_do_OFR { get; set; }
        public string Opis_Cena_zapis_do_OFR
        {
            get
            {
                return _Opis_Cena_zapis_do_OFR;
            }
            set
            {
                if (_Opis_Cena_zapis_do_OFR != value)
                {
                    _Opis_Cena_zapis_do_OFR = value; OnPropertyChanged("Opis_Cena_zapis_do_OFR"); UsingSQLComand(return_ComandUpdate(SAP, _Opis_Cena_zapis_do_OFR, "Opis_Cena_zapis_do_OFR"), con);
                }
            }
        }
        private string _Opis_Cena_zapis_do_OFR2 { get; set; }
        public string Opis_Cena_zapis_do_OFR2
        {
            get
            {
                return _Opis_Cena_zapis_do_OFR2;
            }
            set
            {
                if (_Opis_Cena_zapis_do_OFR2 != value)
                {
                    _Opis_Cena_zapis_do_OFR2 = value; OnPropertyChanged("Opis_Cena_zapis_do_OFR2"); UsingSQLComand(return_ComandUpdate(SAP, _Opis_Cena_zapis_do_OFR2, "Opis_Cena_zapis_do_OFR2"), con);
                }
            }
        }
        private string _Opis_Cena_zapis_do_OFR3 { get; set; }
        public string Opis_Cena_zapis_do_OFR3
        {
            get
            {
                return _Opis_Cena_zapis_do_OFR3;
            }
            set
            {
                if (_Opis_Cena_zapis_do_OFR3 != value)
                {
                    _Opis_Cena_zapis_do_OFR3 = value; OnPropertyChanged("Opis_Cena_zapis_do_OFR3"); UsingSQLComand(return_ComandUpdate(SAP, _Opis_Cena_zapis_do_OFR3, "Opis_Cena_zapis_do_OFR3"), con);
                }
            }
        }

        public string Wstaw_wrsByte_Baza(string Szukane, object Wstawiane, string _Set)
        {
            int err;
            try
            {
                Tim = TimeAktual();
                string set_Val = _Set + "="; 
                string Nip = Get_KlientDane.NIP;

                string Sqwery = @" -- Try to update any existing row
                                        UPDATE BazaOfr_robocze
                                        SET NIP=@NIP," + set_Val + "@" + _Set + @",OstAkt=@OstAkt
                                        where SAP=" + Szukane + "  And NIP=" + Nip + @" ;

                                    -- If no update happened (i.e. the row didn't exist) then insert one                                        
                                        INSERT INTO BazaOfr_robocze  (NIP," + _Set + @",OstAkt)
                                        SELECT @NIP," + "@" + _Set + @",@OstAkt
                                        WHERE (Select Changes() = 0);";
                using (SQLiteCommand cmd = new SQLiteCommand(Sqwery, con))
                {
                    cmd.Parameters.Add("@NIP", (DbType)SqlDbType.VarChar).Value = Nip;
                    cmd.Parameters.Add("@SAP", (DbType)SqlDbType.VarChar).Value = Szukane; 
                    cmd.Parameters.Add("@" + _Set, (DbType)SqlDbType.Binary).Value = Wstawiane;
                    cmd.Parameters.Add("@OstAkt", (DbType)SqlDbType.Binary).Value = Tim;
                    cmd.Connection = con;
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    cmd.ExecuteNonQuery();
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
                err = 0;

                return err.ToString();
            }
            catch
            {
                return 1.ToString();
            }
        } 
        private object _Img { get; set; }
        public object Img
        {
            get
            {
                return _Img;
            }
            set
            {

                _Img = value; OnPropertyChanged("Img"); Wstaw_wrsByte_Baza(SAP, _Img, "Img");
            }
        }
        private object _TDS { get; set; }
        public object TDS
        {
            get
            {
                return _TDS;
            }
            set
            {

                _TDS = value; OnPropertyChanged("TDS"); Wstaw_wrsByte_Baza(SAP, _TDS, "TDS");
            }
        }
        private object _KCH { get; set; }
        public object KCH
        {
            get
            {
                return _KCH;
            }
            set
            {
                _KCH = value; OnPropertyChanged("KCH"); Wstaw_wrsByte_Baza(SAP, _KCH, "KCH");
            }
        }
        private bool _Plik_Tds_True { get; set; }
        public bool Plik_Tds_True
        {
            get
            {
                return _Plik_Tds_True;
            }
            set
            {
                if (_Plik_Tds_True != value)
                {
                    _Plik_Tds_True = value; OnPropertyChanged("Plik_Tds_True"); UsingSQLComand(return_ComandUpdate(SAP, _Plik_Tds_True.ToString(), "Plik_Tds_True"), con);
                }
            }
        }
        private bool _Plik_Kch_True { get; set; }
        public bool Plik_Kch_True
        {
            get
            {
                return _Plik_Kch_True;
            }
            set
            {
                if (_Plik_Kch_True != value)
                {
                    _Plik_Kch_True = value; OnPropertyChanged("Plik_Kch_True"); UsingSQLComand(return_ComandUpdate(SAP, _Plik_Kch_True.ToString(), "Plik_Kch_True"), con);
                }
            }
        }
        private string _FileName { get; set; }
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value; OnPropertyChanged("FileName"); UsingSQLComand(return_ComandUpdate(SAP, _FileName, "FileName"), con);
                }
            }
        }
        private bool _TDS_DO_OFR { get; set; }
        public bool TDS_DO_OFR
        {
            get
            {
                return _TDS_DO_OFR;
            }
            set
            {
                if (_TDS_DO_OFR != value)
                {
                    _TDS_DO_OFR = value; OnPropertyChanged("TDS_DO_OFR"); UsingSQLComand(return_ComandUpdate(SAP, _TDS_DO_OFR.ToString(), "TDS_DO_OFR"), con);
                }
            }
        }
        private bool _CHAR_DO_OFR { get; set; }
        public bool CHAR_DO_OFR
        {
            get
            {
                return _CHAR_DO_OFR;
            }
            set
            {
                if (_CHAR_DO_OFR != value)
                {
                    _CHAR_DO_OFR = value; OnPropertyChanged("CHAR_DO_OFR"); UsingSQLComand(return_ComandUpdate(SAP, _CHAR_DO_OFR.ToString(), "CHAR_DO_OFR"), con);
                }
            }
        }
        private bool _PDF_DO_OFR { get; set; }
        public bool PDF_DO_OFR
        {
            get
            {
                return _PDF_DO_OFR;
            }
            set
            {
                if (_PDF_DO_OFR != value)
                {
                    _PDF_DO_OFR = value; OnPropertyChanged("PDF_DO_OFR"); UsingSQLComand(return_ComandUpdate(SAP, _PDF_DO_OFR.ToString(), "PDF_DO_OFR"), con);
                }
            }
        }
        private string _NazwaPdf { get; set; } 
        public string NazwaPdf
        {
            get
            {
                return _NazwaPdf;
            }
            set
            {
                if (_NazwaPdf != value)
                {
                    _NazwaPdf = value; OnPropertyChanged("NazwaPdf"); UsingSQLComand(return_ComandUpdate(SAP, _NazwaPdf, "NazwaPdf"), con);
                }
            }
        }
    }

    public class ZK11Data : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private event PropertyChangedEventHandler INotifyPropertyChanged_PropertyChanged;

        public void NotifyPropertyChanged(object propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName.ToString()));
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _NIP { get; set; }
        public string NIP
        {
            get
            {
                return _NIP;
            }
            set
            {
                if (_NIP != value)
                {
                    _NIP = value; OnPropertyChanged("NIP");
                }
            }
        }
        private string _NrSAP { get; set; }
        public string NrSAP
        {
            get
            {
                return _NrSAP;
            }
            set
            {
                if (_NrSAP != value)
                {
                    _NrSAP = value; OnPropertyChanged("NrSAP");
                }
            }
        }
        private string _NazwProd { get; set; }
        public string NazwProd
        {
            get
            {
                return _NazwProd;
            }
            set
            {
                if (_NazwProd != value)
                {
                    _NazwProd = value; OnPropertyChanged("NazwProd");
                }
            }
        }
        private string _ZK1 { get; set; }
        public string ZK1
        {
            get
            {
                return _ZK1;
            }
            set
            {
                if (_ZK1 != value)
                {
                    _ZK1 = value; OnPropertyChanged("ZK1");
                }
            }
        }
        private string _ZK2 { get; set; }
        public string ZK2
        {
            get
            {
                return _ZK2;
            }
            set
            {
                if (_ZK2 != value)
                {
                    _ZK2 = value; OnPropertyChanged("ZK2");
                }
            }
        }
        private string _ZK3 { get; set; }
        public string ZK3
        {
            get
            {
                return _ZK3;
            }
            set
            {
                if (_ZK3 != value)
                {
                    _ZK3 = value; OnPropertyChanged("ZK3");
                }
            }
        }
        private string _ZK1Info { get; set; }
        public string ZK1Info
        {
            get
            {
                return _ZK1Info;
            }
            set
            {
                if (_ZK1Info != value)
                {
                    _ZK1Info = value; OnPropertyChanged("ZK1Info");
                }
            }
        }
        private string _ZK2Info { get; set; }
        public string ZK2Info
        {
            get
            {
                return _ZK2Info;
            }
            set
            {
                if (_ZK2Info != value)
                {
                    _ZK2Info = value; OnPropertyChanged("ZK2Info");
                }
            }
        }
        private string _ZK3Info { get; set; }
        public string ZK3Info
        {
            get
            {
                return _ZK3Info;
            }
            set
            {
                if (_ZK3Info != value)
                {
                    _ZK3Info = value; OnPropertyChanged("ZK3Info");
                }
            }
        }
        private string _Representative { get; set; }
        public string Representative
        {
            get
            {
                return _Representative;
            }
            set
            {
                if (_Representative != value)
                {
                    _Representative = value; OnPropertyChanged("Representative");
                }
            }
        }
        private string _OstAkt { get; set; }
        public string OstAkt
        {
            get
            {
                return _OstAkt;
            }
            set
            {
                if (_OstAkt != value)
                {
                    _OstAkt = value; OnPropertyChanged("OstAkt");
                }
            }
        }
        private string _ND { get; set; }
        public string ND
        {
            get
            {
                return _ND;
            }
            set
            {
                if (_ND != value)
                {
                    _ND = value; OnPropertyChanged("ND");
                }
            }
        }
    }

    public class AnalizaBranzaData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private event PropertyChangedEventHandler INotifyPropertyChanged_PropertyChanged;
        public void NotifyPropertyChanged(object propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName.ToString()));
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private string _NrSAP { get; set; }
        public string A_NrSAP
        {
            get
            {
                return _NrSAP;
            }
            set
            {
                if (_NrSAP != value)
                {
                    _NrSAP = value; OnPropertyChanged("NrSAP");
                }
            }
        }
        private string _NazwProd { get; set; }
        public string A_NazwProd
        {
            get
            {
                return _NazwProd;
            }
            set
            {
                if (_NazwProd != value)
                {
                    _NazwProd = value; OnPropertyChanged("NazwProd");
                }
            }
        }
        private string _ALLQ { get; set; }
        public string ALLQ
        {
            get
            {
                return _ALLQ;
            }
            set
            {
                if (_ALLQ != value)
                {
                    _ALLQ = value; OnPropertyChanged("ALLQ");
                }
            }
        }
        private string _ALLQuantity { get; set; }
        public string ALLQuantity
        {
            get
            {
                return _ALLQuantity;
            }
            set
            {
                if (_ALLQuantity != value)
                {
                    _ALLQuantity = value; OnPropertyChanged("ALLQuantity");
                }
            }
        }
        private string _Szr1 { get; set; }
        public string Sztt1
        {
            get
            {
                return _Szr1;
            }
            set
            {
                if (_Szr1 != value)
                {
                    _Szr1 = value; OnPropertyChanged("Sztt1");
                }
            }
        }
        private string _Sztt2 { get; set; }
        public string Sztt2
        {
            get
            {
                return _Sztt2;
            }
            set
            {
                if (_Sztt2 != value)
                {
                    _Sztt2 = value; OnPropertyChanged("Sztt2");
                }
            }
        }
        private string _Sztt3 { get; set; }
        public string Sztt3
        {
            get
            {
                return _Sztt3;
            }
            set
            {
                if (_Sztt3 != value)
                {
                    _Sztt3 = value; OnPropertyChanged("Sztt3");
                }
            }
        }
        private string _Sztt4 { get; set; }
        public string Sztt4
        {
            get
            {
                return _Sztt4;
            }
            set
            {
                if (_Sztt4 != value)
                {
                    _Sztt4 = value; OnPropertyChanged("Sztt4");
                }
            }
        }
        private int _Szt_r1 { get; set; }
        public int Szt_r1
        {
            get
            {
                return _Szt_r1;
            }
            set
            {
                if (_Szt_r1 != value)
                {
                    _Szt_r1 = value; OnPropertyChanged("Szt_r1");
                }
            }
        }
        private int _Szt_r1_pon_10 { get; set; }
        public int Szt_r1_pon_10
        {
            get
            {
                return _Szt_r1_pon_10;
            }
            set
            {
                if (_Szt_r1_pon_10 != value)
                {
                    _Szt_r1_pon_10 = value; OnPropertyChanged("Szt_r1_pon_10");
                }
            }
        }
        private int _Szt_r1_pow_10 { get; set; }
        public int Szt_r1_pow_10
        {
            get
            {
                return _Szt_r1_pow_10;
            }
            set
            {
                if (_Szt_r1_pow_10 != value)
                {
                    _Szt_r1_pow_10 = value; OnPropertyChanged("Szt_r1_pow_10");
                }
            }
        }
        private int _Szt_r1_polPal { get; set; }
        public int Szt_r1_polPal
        {
            get
            {
                return _Szt_r1_polPal;
            }
            set
            {
                if (_Szt_r1_polPal != value)
                {
                    _Szt_r1_polPal = value; OnPropertyChanged("Szt_r1_polPal");
                }
            }
        }
        private int _Szt_r1_Pal { get; set; }
        public int Szt_r1_Pal
        {
            get
            {
                return _Szt_r1_Pal;
            }
            set
            {
                if (_Szt_r1_Pal != value)
                {
                    _Szt_r1_Pal = value; OnPropertyChanged("Szt_r1_Pal");
                }
            }
        }
        private int _Szt_r2 { get; set; }
        public int Szt_r2
        {
            get
            {
                return _Szt_r2;
            }
            set
            {
                if (_Szt_r2 != value)
                {
                    _Szt_r2 = value; OnPropertyChanged("Szt_r2");
                }
            }
        }
        private int _Szt_r2_pon_10 { get; set; }
        public int Szt_r2_pon_10
        {
            get
            {
                return _Szt_r2_pon_10;
            }
            set
            {
                if (_Szt_r2_pon_10 != value)
                {
                    _Szt_r2_pon_10 = value; OnPropertyChanged("Szt_r2_pon_10");
                }
            }
        }
        private int _Szt_r2_pow_10 { get; set; }
        public int Szt_r2_pow_10
        {
            get
            {
                return _Szt_r2_pow_10;
            }
            set
            {
                if (_Szt_r2_pow_10 != value)
                {
                    _Szt_r2_pow_10 = value; OnPropertyChanged("Szt_r2_pow_10");
                }
            }
        }
        private int _Szt_r2_polPal { get; set; }
        public int Szt_r2_polPal
        {
            get
            {
                return _Szt_r2_polPal;
            }
            set
            {
                if (_Szt_r2_polPal != value)
                {
                    _Szt_r2_polPal = value; OnPropertyChanged("Szt_r2_polPal");
                }
            }
        }
        private int _Szt_r2_Pal { get; set; }
        public int Szt_r2_Pal
        {
            get
            {
                return _Szt_r2_Pal;
            }
            set
            {
                if (_Szt_r2_Pal != value)
                {
                    _Szt_r2_Pal = value; OnPropertyChanged("Szt_r2_Pal");
                }
            }
        }
        private int _Szt_r3 { get; set; }
        public int Szt_r3
        {
            get
            {
                return _Szt_r3;
            }
            set
            {
                if (_Szt_r3 != value)
                {
                    _Szt_r3 = value; OnPropertyChanged("Szt_r3");
                }
            }
        }
        private int _Szt_r3_pon_10 { get; set; }
        public int Szt_r3_pon_10
        {
            get
            {
                return _Szt_r3_pon_10;
            }
            set
            {
                if (_Szt_r3_pon_10 != value)
                {
                    _Szt_r3_pon_10 = value; OnPropertyChanged("Szt_r3_pon_10");
                }
            }
        }
        private int _Szt_r3_pow_10 { get; set; }
        public int Szt_r3_pow_10
        {
            get
            {
                return _Szt_r3_pow_10;
            }
            set
            {
                if (_Szt_r3_pow_10 != value)
                {
                    _Szt_r3_pow_10 = value; OnPropertyChanged("Szt_r3_pow_10");
                }
            }
        }
        private int _Szt_r3_polPal { get; set; }
        public int Szt_r3_polPal
        {
            get
            {
                return _Szt_r3_polPal;
            }
            set
            {
                if (_Szt_r3_polPal != value)
                {
                    _Szt_r3_polPal = value; OnPropertyChanged("Szt_r3_polPal");
                }
            }
        }
        private int _Szt_r3_Pal { get; set; }
        public int Szt_r3_Pal
        {
            get
            {
                return _Szt_r3_Pal;
            }
            set
            {
                if (_Szt_r3_Pal != value)
                {
                    _Szt_r3_Pal = value; OnPropertyChanged("Szt_r3_Pal");
                }
            }
        }
        private int _Szt_r4 { get; set; }
        public int Szt_r4
        {
            get
            {
                return _Szt_r4;
            }
            set
            {
                if (_Szt_r4 != value)
                {
                    _Szt_r4 = value; OnPropertyChanged("Szt_r4");
                }
            }
        }
        private int _Szt_r4_pon_10 { get; set; }
        public int Szt_r4_pon_10
        {
            get
            {
                return _Szt_r4_pon_10;
            }
            set
            {
                if (_Szt_r4_pon_10 != value)
                {
                    _Szt_r4_pon_10 = value; OnPropertyChanged("Szt_r4_pon_10");
                }
            }
        }
        private int _Szt_r4_pow_10 { get; set; }
        public int Szt_r4_pow_10
        {
            get
            {
                return _Szt_r4_pow_10;
            }
            set
            {
                if (_Szt_r4_pow_10 != value)
                {
                    _Szt_r4_pow_10 = value; OnPropertyChanged("Szt_r4_pow_10");
                }
            }
        }
        private int _Szt_r4_polPal { get; set; }
        public int Szt_r4_polPal
        {
            get
            {
                return _Szt_r4_polPal;
            }
            set
            {
                if (_Szt_r4_polPal != value)
                {
                    _Szt_r4_polPal = value; OnPropertyChanged("Szt_r4_polPal");
                }
            }
        }
        private int _Szt_r4_Pal { get; set; }
        public int Szt_r4_Pal
        {
            get
            {
                return _Szt_r4_Pal;
            }
            set
            {
                if (_Szt_r4_Pal != value)
                {
                    _Szt_r4_Pal = value; OnPropertyChanged("Szt_r4_Pal");
                }
            }
        }
        private string _R1 { get; set; }
        public string R1
        {
            get
            {
                return _R1;
            }
            set
            {
                if (_R1 != value)
                {
                    _R1 = value; OnPropertyChanged("R1");
                }
            }
        }
        private string _R2 { get; set; }
        public string R2
        {
            get
            {
                return _R2; 
            }
            set
            {
                if (_R2 != value)
                {
                    _R2 = value; OnPropertyChanged("R2");
                }
            }
        }
        private string _R3 { get; set; }
        public string R3
        {
            get
            {
                return _R3; 
            }
            set
            {
                if (_R3 != value)
                {
                    _R3 = value; OnPropertyChanged("R3");
                }
            }
        }
        private string _R4 { get; set; }
        public string R4
        {
            get
            {
                return _R4; 
            }
            set
            {
                if (_R4 != value)
                {
                    _R4 = value; OnPropertyChanged("R4");
                }
            }
        }
        private object _SelSap { get; set; }
        public object SelSap
        {
            get
            {
                return _SelSap; 
            }
            set
            {
                if (_SelSap != value)
                {
                    _SelSap = value; OnPropertyChanged("SelSap");
                }
            }
        }
    }



    public class HistOfSeals : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private event PropertyChangedEventHandler INotifyPropertyChanged_PropertyChanged;
        public void NotifyPropertyChanged(object propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName.ToString()));
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //private string _NrSAP { get; set; }
        //public string A_NrSAP
        //{
        //    get
        //    {
        //        return _NrSAP;
        //    }
        //    set
        //    {
        //        if (_NrSAP != value)
        //        {
        //            _NrSAP = value; OnPropertyChanged("NrSAP");
        //        }
        //    }
        //}

  
        private string _PH { get; set; }
        public string PH
        {
            get
            {
                return _PH;
            }
            set
            {
                if (_PH != value)
                {
                    _PH = value; OnPropertyChanged("PH");
                }
            }
        }
        private string _Branza { get; set; }
        public string Branza
        {
            get
            {
                return _Branza;
            }
            set
            {
                if (_Branza != value)
                {
                    _Branza = value; OnPropertyChanged("Branza");
                }
            }
        }
        private string _KO { get; set; }
        public string KO
        {
            get
            {
                return _KO;
            }
            set
            {
                if (_KO != value)
                {
                    _KO = value; OnPropertyChanged("KO");
                }
            }
        }
        private string _Klient { get; set; }
        public string Klient
        {
            get
            {
                return _Klient;
            }
            set
            {
                if (_Klient != value)
                {
                    _Klient = value; OnPropertyChanged("Klient");
                }
            }
        }
        private string _Produkt { get; set; }
        public string Produkt
        {
            get
            {
                return _Produkt;
            }
            set
            {
                if (_Produkt != value)
                {
                    _Produkt = value; OnPropertyChanged("Produkt");
                }
            }
        }


        private string _Selsr1 { get; set; }
        public string Selsr1
        {
            get
            {
                return _Selsr1;
            }
            set
            {
                if (_Selsr1 != value)
                {
                    _Selsr1 = value; OnPropertyChanged("Selsr1");
                }
            }
        }
        private string _Selsr2 { get; set; }
        public string Selsr2
        {
            get
            {
                return _Selsr2;
            }
            set
            {
                if (_Selsr2 != value)
                {
                    _Selsr2 = value; OnPropertyChanged("Selsr2");
                }
            }
        }
        private string _Selsr3 { get; set; }
        public string Selsr3
        {
            get
            {
                return _Selsr3;
            }
            set
            {
                if (_Selsr3 != value)
                {
                    _Selsr3 = value; OnPropertyChanged("Selsr3");
                }
            }
        }
        private string _Selsr4 { get; set; }
        public string Selsr4
        {
            get
            {
                return _Selsr4;
            }
            set
            {
                if (_Selsr4 != value)
                {
                    _Selsr4 = value; OnPropertyChanged("Selsr4");
                }
            }
        }

        private string _SelsSztt1 { get; set; }
        public string SelsSztt1
        {
            get
            {
                return _SelsSztt1;
            }
            set
            {
                if (_SelsSztt1 != value)
                {
                    _SelsSztt1 = value; OnPropertyChanged("SelsSztt1");
                }
            }
        }
        private string _SelsSztt2 { get; set; }
        public string SelsSztt2
        {
            get
            {
                return _SelsSztt2;
            }
            set
            {
                if (_SelsSztt2 != value)
                {
                    _SelsSztt2 = value; OnPropertyChanged("SelsSztt2");
                }
            }
        }
        private string _SelsSztt3 { get; set; }
        public string SelsSztt3
        {
            get
            {
                return _SelsSztt3;
            }
            set
            {
                if (_SelsSztt3 != value)
                {
                    _SelsSztt3 = value; OnPropertyChanged("SelsSztt3");
                }
            }
        }
        private string _SelsSztt4 { get; set; }
        public string SelsSztt4
        {
            get
            {
                return _SelsSztt4;
            }
            set
            {
                if (_SelsSztt4 != value)
                {
                    _SelsSztt4 = value; OnPropertyChanged("SelsSztt4");
                }
            }
        }
       
        
        
        
        
        
        
        
        
        
        
        
    }

}
