using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using static SoldCalc.MainWindow;
using static SoldCalc.Supporting.SupportingFunctions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace SoldCalc.Supporting
{
    internal class Message
    {
        public static void TextMessageFileAndStart(string SendTo, string bcc, string sendToKO, string Subiect, string Body, string File, string NameFile, bool SendToKo)
        {
            var SmtpServer = new SmtpClient() { Credentials = new System.Net.NetworkCredential(SupportingFunctions.EmailSerwer, SupportingFunctions.Pas), Port = 587, Host = SupportingFunctions.HostName, EnableSsl = false };
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(SupportingFunctions.EmailSerwer);
                mail.To.Add(SendTo);
                if (bcc != "")
                    mail.Bcc.Add(bcc);
                System.Net.Mail.Attachment data = null;
                string[] strArr;
                int count;
                strArr = sendToKO.Split(';');
                for (count = 0; count <= sendToKO.Split(';').Length; count++)
                {
                    try
                    {
                        mail.CC.Add(strArr[count].ToString());
                    }
                    catch { }
                }
                mail.Subject = Subiect;
                mail.Body = Body;
                if (NameFile != "")
                {
                    data = new System.Net.Mail.Attachment(File)
                    {
                        Name = NameFile
                    };
                    mail.Attachments.Add(data);
                }
                SmtpServer.Send(mail);

                if (data != null)
                {
                    data.Dispose();
                }
            }
        }

        public static void TextMessage(string EX_err)
        {
            DateTime DataEx = DateTime.Today;
            try
            {
                string stringex = "Insert into BazaErr (PH, Err, data) VALUES('" + Upr_User.User_PH + "','" + EX_err + "','" + Connect.Mw.MWTitle.ToString() + " - " + DataEx + "') ;";
                Connect.UsingSQLComand(stringex, Connect.con);
            }
            catch
            {
            }
        }

        public static void SendEmEX(string PH, string EX_err)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient() { Credentials = new System.Net.NetworkCredential(EmailSerwer, Pas), Port = 587, Host = HostName, EnableSsl = false };
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(EmailSerwer);
                    mail.To.Add(EmailSerwer);
                    mail.Subject = "Błąd - " + PH;
                    mail.Body = EX_err;
                    SmtpServer.Send(mail);
                }
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
        }

        public static object GenerateEmail(string emailTo, string ccTo, string bccTo, string file, string subject, string body)
        {
            {
                try
                {


                    Microsoft.Office.Interop.Outlook.Application objOutlook = new Microsoft.Office.Interop.Outlook.Application();

                    Microsoft.Office.Interop.Outlook.MailItem mailItem = objOutlook.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                    mailItem.Display();
                    mailItem.To = emailTo;
                    mailItem.CC = ccTo;
                    mailItem.BCC = bccTo;
                    mailItem.Subject = subject;
                    mailItem.Attachments.Add(file);
                    mailItem.HTMLBody = body + mailItem.HTMLBody;

                    mailItem.Display(mailItem);
                    return mailItem;
                }

                catch (Exception ex)
                {

                    Console.WriteLine(ex.StackTrace.ToString());
                   // Interaction.MsgBox(ex.StackTrace.ToString());
                    TextMessage(ex.StackTrace.ToString());
                    return null;
                }
            }
        }

    }
}
