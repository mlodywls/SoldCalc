using iTextSharp.text.pdf;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using Its = iTextSharp.text;

namespace SoldCalc
{
    //internal class Modul_ItxPDF
    //{
    //}
    internal static partial class Modul_ItxPDF
    {


        public static DataTable DaneofrDoTbl = new DataTable();
        public static DataTable dt0 = new DataTable();
        private readonly static Color white = Color.Black;
        private readonly static Color black = Color.White;
        private readonly static Color red = Color.Red;

        public static string Encode_ND(string NameString)
        {
            if (string.IsNullOrEmpty(NameString))
            {
                return null;
            }

            NameString = Strings.Replace(NameString, "N/D - ", "").Trim();
            NameString = Strings.Replace(NameString, "(N/D)", "").Trim();
            NameString = Strings.Replace(NameString, "N/D", "").Trim();
            NameString = Strings.Replace(NameString, "ND", "").Trim();
            return NameString;
        }

        internal static bool Czy_plik_jest_zablokowany(string MyPdfFilePath) // , ByVal NameFile As String) As Boolean
        {
            bool Locked = false;
            try
            {
                var fs = File.Open(MyPdfFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                Locked = true;
                TextMessage(ex.StackTrace.ToString());
            }

            return Locked;
        }

        internal static string GenerujOferta_tymczasowa_PDF(string NameFile, string Pdf_FilePath, DataTable TblUwagi, string sposPłac)
        {
            // Try
            var pSize = new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4);

            // Dim b As Boolean = Czy_plik_jest_zablokowany(Pdf_FilePath)
            // If b = False Then Usun_po_sprawdzeniu(Pdf_FilePath)
            // MsgBox(SprawdzCzyIstniejePlik(Pdf_FilePath) & vbCrLf & Pdf_FilePath & vbCrLf & NameFile)
            // IsFileLocked(Pdf_FilePath) ', NameFile)

            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
            BaseFont bf = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var font8B = new iTextSharp.text.Font(bf, 6, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var font8N = new iTextSharp.text.Font(bf, 6, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            var font10N = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            var font10B = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var font4 = new iTextSharp.text.Font(bf, 4);
            var tblfont = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

            if (ListTblOfr.Tbl_Add_prodList.Count == 0)
            {
                Interaction.MsgBox("Do wygenerowania oferty konieczne jest wprowadzenie produktów");
                return "";
            }

            var o_Date = DateTime.Today;
            {
                ref var withBlock = ref DaneofrDoTbl;
                withBlock.Clear();
                if (withBlock.Columns.Count < 1)
                {
                    withBlock.Columns.Add("1");
                    withBlock.Columns.Add("2");
                    withBlock.Columns.Add("3");
                    withBlock.Columns.Add("4");
                }
            }
            string OldNR = 0.ToString();
            // For i As Integer = 0 To dtTableOfert.Rows.Count - 1
            foreach (var item in ListTblOfr.Tbl_Add_prodList)
            {
                // MsgBox(dtTableOfert.Rows(i)("CenaDoOFR"))
                string NRgrup = Conversions.ToString(item.Lpgrup);
                string SerhOld = "";
                string TextGrup = Conversions.ToString(item.Naglowek);
                int a = 0;
                int rowsCount = ListTblOfr.Tbl_Add_prodList.Count - 1;
                foreach (var row in ListTblOfr.Tbl_Add_prodList)
                {
                    // For j As Integer = 0 To rowsCount
                    string NrGrupSerch = Conversions.ToString(row.Lpgrup);
                    string TxtSap = Conversions.ToString(row.SAP);
                    string Txtprod = Conversions.ToString(row.NazwProd);
                    string TxtOpak = Conversions.ToString(row.Kszt);
                    string[] strArr = TxtOpak.Split('/');
                    TxtOpak = Conversions.ToString(Operators.ConcatenateObject(strArr[0] + " x ", row.Poj));
                    string Txtmiara = Conversions.ToString(row.Poj);
                    string[] strArropak = Txtmiara.Split(' ');
                    string Jmra;
                    if (Strings.Mid(strArropak[1], 1, 2) == "l.")
                        Jmra = "l.";
                    else
                        Jmra = "szt.";
                    TxtOpak = strArr[0] + Jmra + " x " + Txtmiara;
                    double TxtCena;
                    if (!string.IsNullOrEmpty(row.CenaDoOFR.ToString()))
                        TxtCena = Conversions.ToDouble(row.CenaDoOFR.ToString());
                    else
                        TxtCena = Conversions.ToDouble(row.CenaZPrace.ToString());
                    TxtCena = Math.Round(TxtCena, 2, MidpointRounding.AwayFromZero);
                    if ((NRgrup ?? "") == (NrGrupSerch ?? ""))
                    {
                        IList<string> list = new List<string>(SerhOld.Split(new string[] { "," }, StringSplitOptions.None));
                        foreach (string element in list)
                        {
                            if (!string.IsNullOrEmpty(element))
                            {
                                if ((element ?? "") == (NRgrup ?? ""))
                                    goto Line1;
                            }
                        }
                        if (a == 0)
                        {
                            DaneofrDoTbl.Rows.Add(new object[] { "", TextGrup, "", "" });
                            rowsCount += 1;
                            a = 1;
                        }

                    Line1:
                        ;

                        if ((NRgrup ?? "") == (NrGrupSerch ?? ""))
                            DaneofrDoTbl.Rows.Add(new object[] { TxtSap, Txtprod, TxtOpak, TxtCena }); // : rowsCount += 1
                        SerhOld = SerhOld + "," + NRgrup;
                    }
                    var uniqueCols = DaneofrDoTbl.DefaultView.ToTable(true, "1", "2", "3", "4");
                    DaneofrDoTbl = uniqueCols;
                    OldNR = NrGrupSerch;
                }
            }


            {
                ref var withBlock1 = ref dt0;
                withBlock1.Clear();
                if (withBlock1.Columns.Count < 1)
                {
                    withBlock1.Columns.Add("1");
                    withBlock1.Columns.Add("2");
                }
                withBlock1.Rows.Add(new object[] { "Ofertę przygotował:", Upr_User.User_PH }); // User}) ' 
                withBlock1.Rows.Add(new object[] { "Warunki handlowe:", "" });
                withBlock1.Rows.Add(new object[] { "1. Sposób płatności:", sposPłac }); // ComboPlac.Text})
                withBlock1.Rows.Add(new object[] { "2. Dostawa:", "Na koszt dostawcy" });
                withBlock1.Rows.Add(new object[] { "3. Minimum Logistyczne: ", "300 zł" });
                withBlock1.Rows.Add(new object[] { "4. Oferta ważna: ", "30 dni" }); // TIleDni.Text & " dni"})
            }

            var PdfImage = new iTextSharp.text.Jpeg(new Uri(Folder_Matka_Programu + @"Resources\Logo_Soudal.jpg"));
            //Nie można odnaleźć części ścieżki „D:\WinCalcWindows\SoldCalc C#\SoldCalc 26-06-2023\SoldCalc\bin\Debug\Resources\Logo_Soudal.jpg”.”
            PdfImage.Alignment = iTextSharp.text.Jpeg.ALIGN_LEFT;
            PdfImage.ScaleAbsoluteHeight(30.0f);
            PdfImage.ScaleAbsoluteWidth(90.0f);

            var hTable = new iTextSharp.text.pdf.PdfPTable(1);
            hTable.AddCell(GetCell("Czosnów Mazowiecki " + Conversions.ToString(o_Date), iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, iTextSharp.text.FontFactory.GetFont("Calibri", 5, iTextSharp.text.BaseColor.BLACK), false, iTextSharp.text.BaseColor.WHITE, -80, 0, 0, -30)); // Naglówek
            var kTable = new iTextSharp.text.pdf.PdfPTable(1);
            kTable.AddCell(GetCell(" Nr klienta / Odbiorca " + Get_KlientDane.Numer_konta, iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, iTextSharp.text.FontFactory.GetFont("Calibri", 4, iTextSharp.text.BaseColor.BLACK), false, iTextSharp.text.BaseColor.WHITE, 20, 2, 0, -20)); // dane klienta 
            var klTable = new iTextSharp.text.pdf.PdfPTable(1);
            klTable.AddCell(GetCell(Get_KlientDane.Nazwa_klienta, iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, font8B, false, iTextSharp.text.BaseColor.WHITE, 0, 2, 0, -20)); // dane klienta
            klTable.AddCell(GetCell(Get_KlientDane.Adres, iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, font8B, false, iTextSharp.text.BaseColor.WHITE, 0, 2, 0, -20));
            klTable.AddCell(GetCell(Get_KlientDane.Kod_Poczta + " " + Get_KlientDane.Poczta, iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, font8B, false, iTextSharp.text.BaseColor.WHITE, 0, 2, 0, -20));
            var pTable = new iTextSharp.text.pdf.PdfPTable(dt0.Columns.Count);
            pTable.HorizontalAlignment = iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT; // Dane handlowe oferty
            pTable.WidthPercentage = 35.0f;
            pTable.DefaultCell.MinimumHeight = 10.0f;
            pTable.SpacingAfter = 15.0f;
            foreach (DataColumn col in dt0.Columns)
                pTable.AddCell(GetCell(col.ColumnName, iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, iTextSharp.text.FontFactory.GetFont("Calibri", 6, iTextSharp.text.BaseColor.WHITE), false, iTextSharp.text.BaseColor.WHITE, -30, 0, 0, 0));
            foreach (DataRow row in dt0.Rows)
            {
                pTable.AddCell(GetCell(row.Field<string>("1"), iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT, font8N, false, iTextSharp.text.BaseColor.WHITE, 2, 2, 5, 0));
                pTable.AddCell(GetCell(row.Field<string>("2"), iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT, font8N, false, iTextSharp.text.BaseColor.WHITE, 2, 2, 5, 0));
            }
            var Htbl = new DataTable();
            Htbl.Columns.Add("Nr produktu");
            Htbl.Columns.Add("Nazwa produktu");
            Htbl.Columns.Add("Opakowanie zbiorcze.");
            Htbl.Columns.Add("Cena za szt. netto");
            var NTable = new PdfPTable(4);
            NTable.SetWidths(new int[] { 1, 5, 1, 1 });
            NTable.TotalWidth = 540.0f;
            NTable.LockedWidth = true;
            NTable.HorizontalAlignment = 0;
            NTable.SpacingBefore = 15.0f;
            NTable.SpacingAfter = 10.0f;
            foreach (DataColumn col in Htbl.Columns)
                NTable.AddCell(GetCell(col.ColumnName, iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, iTextSharp.text.FontFactory.GetFont("Calibri", 7, iTextSharp.text.BaseColor.BLACK), true, iTextSharp.text.BaseColor.LIGHT_GRAY, 2, 2, 0, 0));
            //    line2:
            ;

            var cTable = new iTextSharp.text.pdf.PdfPTable(4);
            cTable.SetWidths(new int[] { 1, 5, 1, 1 });

            cTable.DefaultCell.Border = 0;
            cTable.TotalWidth = 540.0f;
            cTable.LockedWidth = true;
            cTable.HorizontalAlignment = 0;
            // .SpacingBefore = 15.0F
            cTable.SpacingAfter = 10.0f;
            foreach (DataRow row in DaneofrDoTbl.Rows)
            {
                // MsgBox(row.Field(Of String)("1").ToString)
                if (string.IsNullOrEmpty(row.Field<string>("1")))
                {
                    cTable.AddCell(GetCell(row.Field<string>("1"), iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, font8B, false, iTextSharp.text.BaseColor.LIGHT_GRAY, 3, 4, 5, 0));
                    cTable.AddCell(GetCell(row.Field<string>("2"), iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, font8B, false, iTextSharp.text.BaseColor.LIGHT_GRAY, 3, 4, 5, 0));
                    cTable.AddCell(GetCell(row.Field<string>("3"), iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, font8B, false, iTextSharp.text.BaseColor.LIGHT_GRAY, 3, 4, 5, 0));
                    cTable.AddCell(GetCell(row.Field<string>("4"), iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, font8B, false, iTextSharp.text.BaseColor.LIGHT_GRAY, 3, 4, 5, 0));
                }
                else
                {
                    cTable.AddCell(GetCell(row.Field<string>("1"), iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, font8N, true, iTextSharp.text.BaseColor.WHITE, 3, 4, 5, 0));
                    cTable.AddCell(GetCell(row.Field<string>("2"), iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT, font8N, true, iTextSharp.text.BaseColor.WHITE, 3, 4, 5, 0));
                    cTable.AddCell(GetCell(row.Field<string>("3"), iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, font8N, true, iTextSharp.text.BaseColor.WHITE, 3, 4, 5, 0));
                    cTable.AddCell(GetCell(row.Field<string>("4") + " zł", iTextSharp.text.pdf.PdfPCell.ALIGN_CENTER, font8N, true, iTextSharp.text.BaseColor.WHITE, 3, 4, 5, 0));
                }
            }
            var footerTable = new PdfPTable(1);
            footerTable.SpacingBefore = 15.0f;
            footerTable.AddCell(GetCell(Upr_User.User_PH, iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, font8N, false, iTextSharp.text.BaseColor.WHITE, 5, 0, 0, -20));
            footerTable.AddCell(GetCell(Upr_User.Telefon, iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, font8N, false, iTextSharp.text.BaseColor.WHITE, 5, 0, 0, -20));
            footerTable.AddCell(GetCell(Upr_User.User_Email, iTextSharp.text.pdf.PdfPCell.ALIGN_RIGHT, font8N, false, iTextSharp.text.BaseColor.WHITE, 5, 0, 0, -20));
            // Dim pdfWrite As Its.pdf.PdfWriter
            // Using fs As New FileStream(Pdf_FilePath, FileMode.Create, FileAccess.Write, FileShare.None)
            using (var fs = new FileStream(Pdf_FilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var PdfDoc = new iTextSharp.text.Document(pSize, 20, 20, 20, 20))
                {
                    using (iTextSharp.text.pdf.PdfWriter pdfWrite = iTextSharp.text.pdf.PdfWriter.GetInstance(PdfDoc, fs))
                    {
                        // https://stackoverflow.com/questions/64147520/null-reference-exception-when-calling-itext7-pdfacroform-getacroform-in-net-c
                        PdfDoc.Open();
                        PdfDoc.Add(PdfImage);
                        PdfDoc.Add(kTable);
                        PdfDoc.Add(klTable);
                        PdfDoc.Add(hTable);
                        PdfDoc.Add(pTable);
                        PdfDoc.Add(NTable);
                        PdfDoc.Add(cTable);
                        var TTable = new iTextSharp.text.pdf.PdfPTable(1);
                        // ''''''''''''''''''''''''''''''''''''''''''''''Dodaj uwaga w oferta 

                        if (TblUwagi != null)
                        {
                            foreach (DataRow row in TblUwagi.Rows)
                            {
                                if (row[0].ToString() == "UWAGI :")
                                {
                                    TTable.AddCell(GetCell("UWAGI :", iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT, font10B, false, iTextSharp.text.BaseColor.WHITE, 5, 15, -10, 0));
                                }
                                else
                                {
                                    TTable.AddCell(GetCell(row[0].ToString(), iTextSharp.text.pdf.PdfPCell.ALIGN_LEFT, font10N, false, iTextSharp.text.BaseColor.WHITE, 5, 10, -10, 0));
                                }
                            }
                        }
                        PdfDoc.Add(TTable);
                        PdfDoc.Add(footerTable);
                        PdfDoc.Close();
                        PdfDoc.Dispose();

                        // :pdfWrite.Close(): pdfWrite.Dispose()
                    }
                    // fs.Close() : fs.Dispose()
                }
                // pdfWrite.Close()
                // pdfWrite.Dispose()
            }

            // Console.WriteLine("end generate PDF")
            return Pdf_FilePath;
        }




        /// <summary>Creates a new cell for the table.</summary>
        /// <param name="text">The text string for the cell.</param>
        /// <param name="alignment">Alighnment for the text string.</param>
        /// <param name="textfont">The font used for the text string.</param>
        /// <param name="border">True to show the cell border. False to hide the cell border.</param>
        /// <param name="backcolor">The background color of the cell.</param>
        /// <param name="padtop">The amount of padding on the top of the text string.</param>
        /// <param name="padbottom">The amount of padding on the bottom of the text string.</param>
        /// <param name="padleft">The amount of padding on the left of the text string.</param>
        /// <param name="padright">The amount of padding on the right of the text string.</param>
        public static Its.pdf.PdfPCell GetCell(string text, int alignment, Its.Font textfont, bool border, Its.BaseColor backcolor, float padtop, float padbottom, float padleft, float padright)
        {
            try
            {
                var cell = new iTextSharp.text.pdf.PdfPCell(new iTextSharp.text.Phrase(text, textfont)) { BackgroundColor = backcolor, PaddingLeft = padleft, PaddingRight = padright, PaddingTop = padtop, PaddingBottom = padbottom, HorizontalAlignment = alignment };
                if (!border)
                    cell.Border = iTextSharp.text.pdf.PdfPCell.NO_BORDER;
                return cell;
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return default;
            }
        }

    }
}
