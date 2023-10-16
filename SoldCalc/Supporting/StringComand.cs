using Microsoft.VisualBasic;
using System;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Supporting
{
    public class StringComand
    {

        public static String ReturnOptymalizeSql()
        {
            return "PRAGMA writable_schema=ON; VACUUM;";
            //   return Prag;
        }
        public static string GetNameColumnTableSql(string Namecolumn)
        {
            return "SELECT name FROM PRAGMA_TABLE_INFO('" + Namecolumn + "')";
        }
        public static string ReturnComandPdtFile()
        {
            string SqlOFR = @"Select si.Id,si.SAP,si.NrOFR,si.PlkPdf,md.Nazwa_klienta || ' ' || md.Nazwa_CD as Nazwa_klienta, md.Opiekun_klienta
                                        from TblPdf si 
                                        LEFT JOIN  BazaKL md ON md.NIP = si.SAP
                                        WHERE si.PlkPdf IS NOT NULL AND si.SAP NOT like '%DEL-%'
										group by si.NrOFR 
                                    ORDER BY si.OstAkt DESC";
            return SqlOFR;
        }
        public static string returnComandUpdateZK11(string aa1, string aa2, string aa3, string aa4, string aa5, string aa6, string aa7, string aa8)
        {
            Connect.Tim = SupportingFunctions.TimeAktual();
            string Sqwert = @" -- Try to update any existing row
                                    UPDATE TabZK
                                    SET   NIP='" + aa1 + "',NrSAP='" + aa2 + "',ZK1='" + aa3 + "',ZK2='" + aa4 + "',ZK3='" + aa5 + "' ,ZK1Info='" + aa6 + "',ZK2Info='" + aa7 + "',ZK3Info='" + aa8 + "',Representative='" + Upr_User.User_PH + "',OstAkt='" + Connect.Tim + @"'
                                    where NIP =" + aa1 + " And NrSAP=" + aa2 + @" ;

                                -- If no update happened (i.e. the row didn't exist) then insert one                                        
                                    INSERT INTO TabZK  (NIP,NrSAP,ZK1,ZK2,ZK3,ZK1Info,ZK2Info,ZK3Info,Representative,OstAkt)
                                    SELECT '" + aa1 + "','" + aa2 + "','" + aa3 + "','" + aa4 + "','" + aa5 + "','" + aa6 + "','" + aa7 + "','" + aa8 + "','" + Upr_User.User_PH + "','" + Connect.Tim + @"'
                                    WHERE (Select Changes() = 0);";
            return Sqwert;
        }
        public static string ReturnComndCennik()
        {
            string sqlcomand = @"Select si.Id, si.Naglowek, si.Lpgrup, si.SAP, si.NazwProd, si.Kszt || ' / ' || si.Pszt as Kszt , si.Poj || ' ' || si.Miara as Poj, si.CDM, si.CK, si.PH, si.ZPR0,
                            si.GRUPA, si.KATEGORIA, si.NAZEWNICTWO, si.BrakPrace, md.Img , md.Tds, md.KC , md.PDF2, si.OstAkt, 
							(SELECT CASE WHEN md.Tds IS NOT NULL and md.Tds not like ''  THEN 'True' ELSE 'False' END FROM Baza_PDF) as TdsOk,
							(SELECT CASE WHEN md.KC IS NOT NULL and md.KC not like '' THEN 'True' ELSE 'False' END FROM Baza_PDF ) as KArtcOK,
							(SELECT CASE WHEN md.PDF2 IS NOT NULL and md.PDF2 not like '' THEN 'True' ELSE 'False' END FROM Baza_PDF  ) as Pdf2OK
                            from Cennik si
                            LEFT JOIN  Baza_PDF md ON md.SAP = si.SAP";
            // ' ORDER BY si.Lpgrup ASC" ' DESC" 'ASC"
            return sqlcomand;
        }
        public static string ReturnComandBazaKlient()
        {
            string Sqwery = @"Select si.id, si.Opiekun_klienta ,si.NIP, si.Stan, si.Numer_konta, si.Nazwa_klienta || ' ' || si.Nazwa_CD as Nazwa_klienta, si.Adres, si.Kod_Poczta, si.Poczta, si.Forma_plac, si.PraceList, si.Branza, si.Tel, si.E_mail, si.OstAkt ,md.KO,md.Email as BrEma
                              from BazaKl si
                              LEFT JOIN  DaneKO md ON substr(md.Branza,1,2) = substr(si.Branza ,1,2) ";
            return Sqwery;
        }
        public static string ReturnComndBazaUser_PH()
        {
            Connect.Tim = SupportingFunctions.TimeAktual();
            string SqlString1 = "";
            if (Upr_User.NrPh != null)
            {
                string[] Str_NrPh = Upr_User.NrPh.Split('/');
                try
                {
                    Str_NrPh = Upr_User.NrPh.Split('/');
                    if (Str_NrPh[1] == null)
                        Str_NrPh[1] = GetUserName().ToString();
                }
                catch
                {
                }
                // Console.WriteLine(Str_NrPh[0] + "  "  + Str_NrPh[1]);
                SqlString1 = @" -- Try to update any existing row
                                                        UPDATE TblUser
                                                        SET Ranga = '" + Upr_User.Ranga + "',Imie='" + Upr_User.Imie + "',Nazwisko='" + Upr_User.Nazwisko + "',Telefon='" + Upr_User.Telefon + "',Email='" + Upr_User.User_Email + "',KO='" + Upr_User.KO_email + "',CenaKO='" + Upr_User.CenaKO + "',WyślijInfoDoKO='" + Upr_User.WyslijInfoDoKO + "',MonitKO='" + Upr_User.MonitKO + "',Upr4='" + Upr_User.Upr4 + "',NrPh ='" + Upr_User.NrPh + "',  OstAkt='" + Connect.Tim + @"' 
                                                        WHERE Email like '%" + Upr_User.User_Email + "%' and NrPh like '%" + Str_NrPh[1] + @"%';                                                                                       
                                                        -- If no update happened (i.e. the row didn't exist) then insert one
                                                        INSERT INTO TblUser   (Ranga ,Imie ,Nazwisko ,Telefon  ,Email  ,KO  , CenaKO , WyślijInfoDoKO  ,MonitKO , Upr4, NrPh ,OstAkt)   SELECT '" + Upr_User.Ranga + "','" + Upr_User.Imie + "','" + Upr_User.Nazwisko + "','" + Upr_User.Telefon + "','" + Upr_User.User_Email + "','" + Upr_User.KO_email + "','" + Upr_User.CenaKO + "','" + Upr_User.WyslijInfoDoKO + "','" + Upr_User.MonitKO + "','" + Upr_User.Upr4 + "','" + Upr_User.NrPh + "','" + Connect.Tim + @"'
                                                        WHERE (Select Changes() = 0);";
                //  Console.WriteLine(SqlString1);
            }
            return SqlString1;
        }
        public static string ReturnComndBazaZakupy()
        {
            string sqlcomand = @"Select 'False' AS Chc, si.SoldTocustomer , IFNULL(mc.SAP || ' - ' || mc.NazwProd, si.Material) as Material , si.Yearbilling, si.SalesP, si.Quantity, si.Datebilling, si.Representative, si.Turnover 
                         From BazaZKP si  
						LEFT JOIN Cennik mc ON mc.SAP = substr(si.Material, 1, 6) 	
                        ORDER BY si.Yearbilling DESC";
            // ' ORDER BY si.Lpgrup ASC" ' DESC" 'ASC"
            return sqlcomand;
        }
        public static string ReturnComandMaxData()
        {
            string Sqwery = @"Select 
                                    SUBSTR(Datebilling, -4, 1)|| 
                                    SUBSTR(Datebilling, -3, 1)||
                                    SUBSTR(Datebilling, -2, 1)||
                                    SUBSTR(Datebilling, -1, 1)||'-'||
                                    SUBSTR(Datebilling, -7, 1)||
	                                SUBSTR(Datebilling, -6, 1)||'-'||
	                                IIF(length(Datebilling)=10,SUBSTR(Datebilling, -10, 1) ,0)||
	                                SUBSTR(Datebilling, -9, 1) As	'reversed' 
                            From BazaZKP Order By reversed desc limit 1;";
            return Sqwery;
        }
        public static string ReturnComandMinData()
        {
            string Sqwery = @"Select 
                                SUBSTR(Datebilling, -4, 1)|| 
                                    SUBSTR(Datebilling, -3, 1)||
                                    SUBSTR(Datebilling, -2, 1)||
                                    SUBSTR(Datebilling, -1, 1)||'-'||
                                    SUBSTR(Datebilling, -7, 1)||
	                                SUBSTR(Datebilling, -6, 1)||'-'||
	                                IIF(length(Datebilling)=10,SUBSTR(Datebilling, -10, 1) ,0)||
	                                SUBSTR(Datebilling, -9, 1)    As	'reversed' 
                            From BazaZKP WHERE Datebilling not like '' Order By reversed ASC LIMIT 1 ";
            return Sqwery;
        }
        public static string ReturnComndBazaPH()
        {
            string sqlcomand = "Select * From TblUser WHERE Email Not Like ''";
            // ' ORDER BY si.Lpgrup ASC" ' DESC" 'ASC"
            return sqlcomand;
        }
        public static string ReturnComndGeTLogUser()
        {
            string sqlcomand = @"Select Id,Ranga,Rejon,Imie,Nazwisko, SUBSTR(OstAkt, 1, 2)|| '-'||
                                SUBSTR(OstAkt, 3, 2)||'-'||
                                SUBSTR(OstAkt, 5, 2)||' / '||
                                SUBSTR(OstAkt, 7, 2)||':'||
                                SUBSTR(OstAkt, 9, 2)    As	'ostLog' ,Telefon,Email,KO,CenaKO , WyślijInfoDoKO,MonitKO,Upr4,NrPh  
								From TblUser 
								WHERE NrPh IS NOT NULL
								order by OstAkt DESC";
            // ' ORDER BY si.Lpgrup ASC" ' DESC" 'ASC"
            return sqlcomand;
        }
        public static string Suma_Yearbilling_SQL(bool Szt, string odata)
        {
            try
            {
                string str = "";
                int MaxDta = int.Parse(Strings.Mid(Upr_User.MaxData, 1, 4));
                int MinDta = int.Parse(Strings.Mid(Upr_User.MinData, 1, 4));
                for (int i = MaxDta, loopTo = int.Parse(odata); i >= loopTo; i -= 1)
                {
                    if (i > MaxDta - 4)
                    {
                        if (Szt == true)
                        {
                            str += Microsoft.VisualBasic.Constants.vbCrLf + "round(sum(printf('%d.02%',(si.Turnover*1))) filter(where si.Yearbilling = '" + i.ToString() + "'),2) as '" + i.ToString() + @"',
                                              sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '" + i.ToString() + "') as '" + "szt " + i.ToString() + "',";
                        }
                        else
                        {
                            str += Microsoft.VisualBasic.Constants.vbCrLf + "round(sum(printf('%d.02%',(si.Turnover*1))) filter(where si.Yearbilling = '" + i.ToString() + "'),2) as '" + i.ToString() + "',";
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                str = Strings.Mid(str, 1, str.Length - 1);
                // Console.WriteLine(str)
                return str;
            }
            catch (Exception ex)
            {

                Message.TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }
        public static string Suma_YearbillingSUM_SQL(bool Sum, string odata)
        {
            try
            {
                string str = "";
                int MaxDta = int.Parse(Strings.Mid(Upr_User.MaxData, 1, 4));
                for (int i = MaxDta; i >= int.Parse(odata); i += -1)
                {
                    if (i > MaxDta - 4)
                    {
                        if (Sum == true)
                            str += Microsoft.VisualBasic.Constants.vbCrLf + "sum(((replace(mc.CK,',','.')) * si.Quantity) *1) filter(where si.Yearbilling = '" + i + "')as '" + i + @"',                 
                                                sum( si.Quantity) filter(where si.Yearbilling = '" + i + "') as '" + "szt " + i + "',";
                        else
                            str += Microsoft.VisualBasic.Constants.vbCrLf + "sum(((replace(mc.CK,',','.')) * si.Quantity) *1) filter(where si.Yearbilling = '" + i + "') as '" + i + "',";
                    }
                    else
                        break;
                }
                str = Strings.Mid(str, 1, str.Length - 1);
                // Console.WriteLine("Suma_YearbillingSUM_SQL - " & str)
                return str;
            }
            catch (Exception ex)
            {

                Message.TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }
        public static string GetComandSql(bool serh, string ControlName)
        {
            try
            {
                // Console.WriteLine(" GetComandSql " + ControlName);
                string Sqlstring = "";
                if (serh == true)
                {
                    if (ControlName == "Material")
                        Sqlstring = "GROUP BY si.Representative, si.SoldTocustomer ,si.Material"; // "GROUP BY si.Representative, md.Branza "
                    if (ControlName == "ProduktNagl")
                        Sqlstring = "IFNULL(( mc.SAP || ' - ' || mc.NazwProd), 'wycofany -' || si.Material) as Produkt,";
                    if (ControlName == "UprKO")
                        Sqlstring = "";
                    if (ControlName == "WyswietlPH")
                        Sqlstring = "si.Representative as PH,";
                    if (ControlName == "ComboBoxKO_PH")
                        Sqlstring = "GROUP BY si.Representative, md.Branza Order By md.Branza ASC ;";
                    if (ControlName == "TextBoxSzukKlientTrue")
                        Sqlstring = " GROUP BY si.Representative , si.SoldTocustomer  Order By md.Branza ASC ;";
                    if (ControlName == "TextBoxSzukKlientFalse")
                        Sqlstring = " GROUP BY si.Representative , si.Material  Order By md.Branza ASC ;";
                    if (ControlName == "TextBoxProdukt")
                        Sqlstring = " GROUP BY si.Representative , si.Material   Order By md.Branza ASC ;";
                }
                else
                {
                    if (ControlName == "Material")
                        Sqlstring = "  GROUP BY si.Representative, si.SoldTocustomer ,si.Material"; // "  GROUP BY si.Representative, md.Branza "
                    if (ControlName == "ProduktNagl")
                        Sqlstring = "";

                    if (ControlName == "UprKO")
                        Sqlstring = "AND si.Representative Like '%" + Upr_User.Imie + "%' and si.Representative like '%" + Upr_User.Nazwisko + "%' ";
                    if (ControlName == "WyswietlPH")
                        Sqlstring = "";
                    if (ControlName == "ComboBoxKO_PH")
                        Sqlstring = "GROUP BY si.Representative, md.Branza Order By md.Branza ASC ;";
                    if (ControlName == "TextBoxSzukKlientTrue")
                        Sqlstring = " GROUP BY si.Representative , si.SoldTocustomer  Order By md.Branza ASC ;";
                    if (ControlName == "TextBoxSzukKlientFalse")
                        Sqlstring = " GROUP BY si.Representative , si.Material  Order By md.Branza ASC ;";
                    if (ControlName == "TextBoxProdukt")
                        Sqlstring = " GROUP BY si.Representative , si.Material   Order By md.Branza ASC ;";
                }
                return Sqlstring;
            }
            catch (Exception ex)
            {

                Message.TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }
        public static string ComandBranzaSQL(bool KO, bool Szt, string Dat_A)
        {
            //Console.WriteLine("   public static string ComandBranzaSQL");
            try
            {
                string DateString = Suma_Yearbilling_SQL(Szt, Dat_A);
                string StringFormat = "";
                string WyswietlProdukty = GetComandSql(false, "Material");  // "GROUP BY si.Representative, si.SoldTocustomer,si.Material"
                string WyswietlProduktNagl = GetComandSql(false, "ProduktNagl");  // " si.Material as Produkt,"

                string UprawnieniaKO = GetComandSql(Upr_User.UprKO, "UprKO"); // "AND si.Representative Like '%" & DtUser.Rows(0).Item("Imie") & "%' and si.Representative like '%" & DtUser.Rows(0).Item("Nazwisko") & "%' "
                string WHEREUprKO = GetComandSql(Upr_User.UprKO, "WyswietlPH");
                if (Upr_User.UprKO == true)
                    WHEREUprKO = "WHERE" + WHEREUprKO;
                else
                    WHEREUprKO = "";
                {
                    //if (KO == false)
                    //    StringFormat = @" Select   
                    //              ifnull(md.Branza, 'N/D') as Branza,
                    //               " + DateString + @"
                    //         From BazaZKP si
                    //               LEFT JOIN  BazaKL md ON md.Numer_konta = substr(si.SoldTocustomer, 1, 7)
                    //               LEFT JOIN  DaneKO mKO ON substr(mKO.Branza,1,2) = substr(md.Branza ,1,2) 
                    //               LEFT JOIN  Cennik mc ON  mc.SAP = substr(si.Material, 1, 6) 
                    //                " + WHEREUprKO + @"
                    //               GROUP BY si.Representative , md.Branza 
                    //                    ;";
                    //else
                    //    StringFormat = @" Select   
                    //               si.Representative,
                    //                ifnull(md.Branza, 'N/D') as Branza,
                    //               " + DateString + @"
                    //         From BazaZKP si
                    //               LEFT JOIN  BazaKL md ON md.Numer_konta = substr(si.SoldTocustomer, 1, 7)
                    //               LEFT JOIN  DaneKO mKO ON substr(mKO.Branza,1,2) = substr(md.Branza ,1,2) 
                    //               LEFT JOIN  Cennik mc ON  mc.SAP = substr(si.Material, 1, 6)  
                    //               GROUP BY si.Representative not like '%%', md.Branza 
                    //
                    //  ;";
                }

                if (KO == false)
                    StringFormat = @" Select   
                        si.Representative as PH, md.Branza, mKO.KO, md.Nazwa_klienta || ' ' || md.Nazwa_CD  as Klient,IFNULL(( mc.SAP || ' - ' || mc.NazwProd), 'wycofany -' || si.Material) as Produkt,
                                            " + DateString + @"
                                    From BazaZKP si
                                        LEFT JOIN  BazaKL md ON md.Numer_konta = substr(si.SoldTocustomer, 1, 7)
                                        LEFT JOIN  DaneKO mKO ON substr(mKO.Branza,1,2) = substr(md.Branza ,1,2) 
                                        LEFT JOIN  Cennik mc ON  mc.SAP = substr(si.Material, 1, 6) 
                                            " + WHEREUprKO + @"
                                    GROUP BY si.Representative , si.Material
                                    Order By md.Branza ASC 
                                ;";
                else
                    StringFormat = @" Select
                        si.Representative as PH, md.Branza, mKO.KO, md.Nazwa_klienta || ' ' || md.Nazwa_CD  as Klient,IFNULL(( mc.SAP || ' - ' || mc.NazwProd), 'wycofany -' || si.Material) as Produkt,
                                            " + DateString + @"
                                    From BazaZKP si
                                        LEFT JOIN  BazaKL md ON md.Numer_konta = substr(si.SoldTocustomer, 1, 7)
                                        LEFT JOIN  DaneKO mKO ON substr(mKO.Branza,1,2) = substr(md.Branza ,1,2) 
                                        LEFT JOIN  Cennik mc ON  mc.SAP = substr(si.Material, 1, 6)  
                                    GROUP BY si.Representative not like '%%', si.Material
                                    Order By md.Branza ASC
                               ;";
                {
                    //" Select si.Representative as PH, md.Branza, mKO.KO, md.Nazwa_klienta || ' ' || md.Nazwa_CD  as Klient,IFNULL(( mc.SAP || ' - ' || mc.NazwProd), 'wycofany -' || si.Material) as Produkt," +
                    //    "round(sum(printf('%d.02%',(si.Turnover*1))) filter(where si.Yearbilling = '2023'),2) as '2023'," +
                    //    "sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '2023') as 'szt 2023'," +
                    //    "round(sum(printf('%d.02%',(si.Turnover*1))) filter(where si.Yearbilling = '2022'),2) as '2022'," +
                    //    "sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '2022') as 'szt 2022'," +
                    //    "round(sum(printf('%d.02%',(si.Turnover*1))) filter(where si.Yearbilling = '2021'),2) as '2021'," +
                    //    "sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '2021') as 'szt 2021'," +
                    //    "round(sum(printf('%d.02%',(si.Turnover*1))) filter(where si.Yearbilling = '2020'),2) as '2020'," +
                    //    "sum(printf('%d',si.Quantity *1 )) filter(where  si.Yearbilling = '2020') as 'szt 2020'" +
                    //    "From BazaZKP si" +
                    //    "LEFT JOIN  BazaKL md ON md.Numer_konta = substr(si.SoldTocustomer, 1, 7)" +
                    //    "LEFT JOIN  DaneKO mKO ON substr(mKO.Branza,1,2) = substr(md.Branza ,1,2) " +
                    //    "LEFT JOIN  Cennik mc ON  mc.SAP = substr(si.Material, 1, 6) " +
                    //    "WHERE " +
                    //    "((replace(UPPER(md.Nazwa_klienta), '-', '')  || ' ' || replace(UPPER(md.Nazwa_CD), '-', ''))  like '%%'" +
                    //    " OR UPPER(si.SoldTocustomer) like '%  %' ) " +
                    //    " AND (replace(si.Material, '-', '') like '%%'" +
                    //    " OR (replace(mc.SAP, '-', '') like '%%' ) " +
                    //    "OR (mc.SAP || ' ' || mc.NazwProd)  like '%%'" +
                    //    "OR (replace(mc.NazwProd, '-', '') like '%%'))" +
                    //    " AND (md.Branza like '%%') " +
                    //    "AND (si.Representative like '%%')" +
                    //    "AND (mKO.KO like '%%') " +
                    //    "GROUP BY si.Representative , si.Material   Order By md.Branza ASC ; ;"
                }


                //Console.WriteLine(StringFormat);
                return StringFormat;
            }
            catch (Exception ex)
            {
                Message.TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }





    }

}
