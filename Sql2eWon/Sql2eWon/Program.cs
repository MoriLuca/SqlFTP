using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sql2eWon
{
    class Program
    {
        private static  string connetionString = null;

        static void Main(string[] args)
        {
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            string connetionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            SqlDataReader dataReader;
            connetionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=EwonFtp;User ID=sa;Password=0000";
            sql = "Select top 1 * from EwonFtp.dbo.ewon order by Id desc";
            connection = new SqlConnection(connetionString);
            try
            {
                List<string> _textToShare = new List<string>();
                connection.Open();
                command = new SqlCommand(sql, connection);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        if (dataReader.GetName(i) == "Id") continue;
                        string nome = dataReader.GetName(i);
                        float valore = Convert.ToSingle(dataReader.GetValue(i));
                        _textToShare.Add("\"" + nome + "\";" + valore);
                        Console.WriteLine("\""+dataReader.GetName(i)+"\";" + dataReader.GetValue(i));
                    }
                    
                    System.IO.File.WriteAllLines(@"C:\Users\Public\Share\WriteLines.txt", _textToShare);
                    ftpTransfer("luca.txt");

                }
                dataReader.Close();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can not open connection ! ");
            }

            //Console.Read();
        }


        public static bool ftpTransfer(string fileName)
        {
            try
            {
                string ftpAddress = "192.168.250.100";
                string username = "adm";
                string password = "adm";

                using (StreamReader stream = new StreamReader(@"C:\Users\Public\Share\WriteLines.txt"))
                {
                    byte[] buffer = Encoding.Default.GetBytes(stream.ReadToEnd());

                    WebRequest request = WebRequest.Create("ftp://" + ftpAddress + "/" + "usr" + "/" + fileName);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(username, password);
                    Stream reqStream = request.GetRequestStream();
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }


    }
}
