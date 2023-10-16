using System;
using System.IO;
using System.Net;
using static SoldCalc.Supporting.Message;
using static SoldCalc.Supporting.SupportingFunctions;

namespace SoldCalc.Supporting
{
    internal class UnUsedFuncton
    {
        public void SendtFileTxt_as_FTP()
        {
            try
            {
                string text = "Hello World!| test kolejny plik";
                byte[] data = System.Text.Encoding.Default.GetBytes(text);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Strim_URL + "BazaKL/plik.txt");
                request.Credentials = new NetworkCredential(Uide, Pas);
                request.Method = WebRequestMethods.Ftp.AppendFile;
                request.ContentLength = data.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                TextMessage(ex.StackTrace.ToString());
            }
            // return null;
        }



    }
}
