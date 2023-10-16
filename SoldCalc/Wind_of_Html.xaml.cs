using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.Connect;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;
using Its = iTextSharp.text;

namespace SoldCalc
{
    public partial class Wind_of_Html : Window
    {
        private int errnambLab = 1;
        private DataTable DaneofrDoTbl = new DataTable();
        private DataTable dt0 = new DataTable();

        public Wind_of_Html()
        {
            InitializeComponent();
            if (Wind_of_Html_Add is null)
                Wind_of_Html_Add = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Width = Mw.ActualWidth / 2;
                Height = Mw.ActualHeight - 300;
                WindowState windowState = new WindowState(); //.Normal;
                windowState = WindowState.Normal;
                LayoutTransform = Upr_User.dpiTransform;

            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        internal string NumericTextAdd(string Numericname)
        {
            try
            {
                foreach (StackPanel ctr in PanelNaw.Children)
                {
                    if (ctr.Tag == Numericname)
                    {
                        if (Numericname.Length > 7)
                            Numericname = Strings.Mid(Numericname, 1, 7) + "-" + (Conversions.ToDouble(Strings.Mid(Numericname, 9, 11)) * 1d + 1d);
                        else
                            Numericname = Numericname + "-" + "1";
                    }
                }
                return Numericname;
            }
            catch (Exception ex)
            {

                TextMessage(ex.StackTrace.ToString());
                return null;
            }
        }

        public object Generuj_TDS_Tabela(byte[] fileData, string name)
        {

            //try
            //{
            if (Information.IsNumeric(name))
                name = NumericTextAdd(name);   // MsgBox(nameFile)
            string nameFile = name;
            string sTempFileName = null;
            sTempFileName = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, nameFile);
            if (sTempFileName.Contains(".Pdf") == false)
                sTempFileName += ".Pdf";
            using (var FS = new FileStream(sTempFileName, FileMode.Create)) // sTempFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write)
            {
                FS.Write(fileData, 0, fileData.Length);
                FS.Position = 0L;
                FS.Close();
            }

            foreach (StackPanel ctr in PanelNaw.Children)
            {
                foreach (Label ctr1 in ctr.Children)
                {
                    ctr1.Background = new SolidColorBrush(Colors.LightGray);
                }

            }
            var stPan = new StackPanel();
            var lab = new Label();
            var Lab1 = new Label();
            var toolT = new ToolTip();
            var WinPdf = new WebBrowser();
            toolT.Content = name;
            stPan.Orientation = Orientation.Horizontal;
            stPan.Margin = new Thickness(2, 0, 2, 0);
            stPan.Tag = name;
            WinPdf.Margin = new Thickness(0, 40, 0, 0);
            WinPdf.Tag = name;
            WinPdf.Navigate(sTempFileName);
            Lab1.Content = "X";
            Lab1.FontSize = 12;
            Lab1.FontWeight = FontWeights.Bold;
            Lab1.Foreground = new SolidColorBrush(Colors.Red); // Brushes.Red
            Lab1.Background = new SolidColorBrush(Colors.LightGreen);
            Lab1.Tag = name;
            Lab1.ToolTip = "Zamknij";
            Lab1.MouseDown += Lab_Clear_MouseDown;

            lab.Content = name;
            lab.Tag = name;
            lab.Height = 30;
            lab.HorizontalAlignment = HorizontalAlignment.Left;
            lab.Background = new SolidColorBrush(Colors.LightGreen);
            lab.ToolTip = toolT;
            lab.MouseDown += L1_MouseDown;
            stPan.Children.Add(lab);
            stPan.Children.Add(Lab1);
            PanelNaw.Children.Add(stPan);
            AddWeb.Children.Add(WinPdf);
            try
            {
                foreach (WebBrowser ctr in AddWeb.Children)
                {
                    if (ctr.Tag != name)
                    {
                        ctr.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // MessageBox.Show(ex.ToString())
            }
            //}
            //catch (Exception ex)
            //{

            //    TextMessage(ex.StackTrace.ToString());
            //    return null;
            //}
            return null;
        } // wyświetl wybierz Oferę do pokazania - Wyświetlenia

        internal bool IsFileLocked(string MyPdfFilePath) // , ByVal NameFile As String) As Boolean
        {
            bool Locked = false;
            if (!MyPdfFilePath.Contains(".Pdf"))
                MyPdfFilePath += ".Pdf";
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

        private Its.pdf.PdfPCell GetCell(string text, int alignment, Its.Font textfont, bool border, Its.BaseColor backcolor, float padtop, float padbottom, float padleft, float padright)
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

        private void Lab_Clear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string name = Conversions.ToString(((Label)sender).Tag);
                string PreviewName = null;
            Line:
                ;

                foreach (WebBrowser ctr in AddWeb.Children)
                {
                    if (ctr.Tag == name)
                    {
                        DeleteFile(ctr, name);
                        AddWeb.Children.Remove(ctr);
                        goto Line;
                    }
                }

            line1:
                ;

                foreach (StackPanel ctr in PanelNaw.Children)
                {
                    if (ctr.Tag == name)
                    {
                        PanelNaw.Children.Remove(ctr);
                        goto line1;
                    }
                    else
                        PreviewName = (string)ctr.Tag;
                }

                if (!string.IsNullOrEmpty(PreviewName))
                    Vie_existing_file(PreviewName);
                else
                {
                    Wind_of_Html_Add = null;
                    //this.Window.Close();
                    Interaction.MsgBox("Tu funkcja Close nie zrobiona");
                }
                Clear_label_Width();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private object DeleteFile(WebBrowser wb, string fileName)
        {
            wb.Navigate("about:blank");
            string filestr = System.IO.Path.Combine(Folder_Matka_Programu_FilesSC, fileName + ".Pdf");
            Usun_po_sprawdzeniu(filestr);
            return null;
        }

        private void L1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string name = Conversions.ToString(((Label)sender).Tag);
                Vie_existing_file(name);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Vie_existing_file(string Name)
        {
            try
            {
                foreach (StackPanel ctr in PanelNaw.Children)
                {
                    foreach (Label ctr1 in ctr.Children)
                    {
                        if (ctr1.Tag == Name)
                            ctr1.Background = new SolidColorBrush(Colors.LightGreen);
                        else
                            ctr1.Background = new SolidColorBrush(Colors.LightGray);
                    }
                }
                foreach (WebBrowser ctr in AddWeb.Children)
                {
                    if (ctr.Tag == Name)
                        ctr.Visibility = Visibility.Visible;
                    else
                        ctr.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

            foreach (WebBrowser ctr in AddWeb.Children)
                DeleteFile(ctr, ctr.Name);
            try
            {
                string[] FilePdf = Directory.GetFiles(Folder_Matka_Programu_FilesSC, "*.Pdf");
                foreach (string f in FilePdf)
                {
                    try
                    {
                        Usun_po_sprawdzeniu(f);
                        if (File.Exists(f))
                            File.Delete(f);
                        if (File.Exists(f))
                            FileSystem.Kill(f);
                    }
                    catch (Exception ex)
                    {
                        // TimeDelete()
                    }
                }

                Wind_of_Html_Add = null;
            }
            // Me.OnClosed(e)
            // Application.Current.Shutdown(e)
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void TimeDelete()
        {
            try
            {
                string[] FilePdf = Directory.GetFiles(Folder_Matka_Programu_FilesSC, "*.Pdf");
            // Kill(Directory.GetFiles(PDFFiles, "*.Pdf"))
            line1:
                ;

                foreach (string f in FilePdf)
                {
                    try
                    {
                        Interaction.MsgBox(f.ToString());
                        Console.WriteLine(f.ToString());
                        File.Delete(f);
                        ;
                        // if (global::My.Computer.FileSystem.FileExists(f))
                        if (System.IO.Directory.Exists(f))
                        {
                            // MsgBox("File found.")
                            File.Delete(f);
                        }
                        else
                        {
                            Interaction.MsgBox("File not found.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        goto line1;
                    }

                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }



        private void Clear_label_Width()
        {
            try
            {
                int SticSize = (int)this.ActualWidth;
                int CtrSize = default;
                int i = 1;
                foreach (StackPanel ctr in PanelNaw.Children)
                {
                    foreach (Label ctr1 in ctr.Children)
                    {
                        if (ctr1.Content != "X")
                        {
                            CtrSize += (int)ctr1.ActualWidth + 24;
                            i += 1;
                        }
                    }
                }
                if (CtrSize < SticSize / 3d)
                    return;
                int NewSizeCTR = (int)Math.Round(SticSize / (double)i - 24d);
                foreach (StackPanel ctr in PanelNaw.Children)
                {
                    foreach (Label ctr1 in ctr.Children)
                    {
                        if (ctr1.Content != "X")
                        {
                            ctr1.Width = NewSizeCTR; // : CtrSize = 0 : GoTo line1
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        private void PanelNaw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                NewwidthLabel();
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }
        private void NewwidthLabel()
        {
            try
            {
                int SticSize = (int)this.ActualWidth;
                int CtrSize = default;
                int i = 1;
                foreach (StackPanel ctr in PanelNaw.Children)
                {
                    foreach (Label ctr1 in ctr.Children)
                    {
                        if (ctr1.Content != "X")
                        {
                            CtrSize += (int)ctr1.ActualWidth + 24;
                            i += 1;
                        }
                    }
                }
                if (CtrSize >= SticSize)
                {
                    if (CtrSize < SticSize / 2d)
                        return;
                    int NewSizeCTR = (int)Math.Round(CtrSize / (double)(i + 1) - 24d);
                    foreach (StackPanel ctr in PanelNaw.Children)
                    {
                        foreach (Label ctr1 in ctr.Children)
                        {
                            if (ctr1.Content != "X")
                            {
                                ctr1.Width = NewSizeCTR;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

    }
}
