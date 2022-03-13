using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Data.Common;


namespace MultiClient
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        string currentMess = "";
        TcpListener listener;
        List<Socket> listOfClient;
        string info = "fi";

        string convertHTMLDatabase(string input, bool b)
        {
            string result = "";
            if (b)
            {
                string[] kh = input.Split('&');
                result = "\n" + "<Infor>";
                result += "\n" + "\t" + "<MAKH>" + kh[0] + "</MAKH>";
                result += "\n" + "\t" + "<HOTEN>" + kh[1] + "</HOTEN>";
                result += "\n" + "\t" + "<DCHI>" + kh[2] + "</DCHI>";
                result += "\n" + "\t" + "<SODT>" + kh[3] + "</SODT>";
                result += "\n" + "\t" + "<NGSINH>" + kh[4] + "</NGSINH>";
                result += "\n" + "\t" + "<NGDK>" + kh[5] + "</NGDK>";
                result += "\n" + "\t" + "<DOANHSO>" + kh[6] + "</DOANHSO>";
                result += "\n" + "</Infor>";
            }
            else
            {
                result = "<html>\n <body>";
                result += "\n" + "  <p>";
                result += "\n\t" + "404 Not found";
                result += "\n" + "  </p>\n </body>" + "\n</html>";
            }
            return result;
        }

            string createGet(bool kt1)
        {
            DateTime d = DateTime.Now;
            string rp = "";
            if (!kt1)
            {
                rp = "404 Not found";
            }
            else
            {
                rp = "200 OK";
            }
            return @"HTTP / 1.1 " + rp +
                @"\r\n" + "\r\n" +
@"Date:"+ d.ToString() + "\r\n" +
@"Host: Apache / 2.0.52(CentOS)\r\n" + "\r\n" +
@"Accept - Ranges: bytes\r\n" + "\r\n" +
@"Content - Length: 2652\r\n" + "\r\n" +
@"Keep - Alive: timeout = 10, max = 100\r\n" + "\r\n" +
@"Connection: Keep - Alive\r\n" + "\r\n" +
@"Content - Type: text / html; charset = ISO - 8859 - 1\r\n" + "\r\n" +
@"\r\n" + "\r\n";
        }

        string createPost(int length, bool c)
        {
            DateTime d = DateTime.Now;
            string rp = "";
            string content = "";
            if ((length != 7) || (!c))
            {
                rp = "500 Internal Server Error";
                content = "<html>\n<body>\n" +
                   "<h1>500 Internal Server Error! </h1>\n</body>\n</html>";
            }
            else
            {
                rp = "200 OK";
                content = "<html>\n<body>\n" +
                "<h1>Request Processed Successfully!</h1>\n</body>\n</html>";
            }
            return @"HTTP / 1.1 " + rp +
            @"\r\n" + "\r\n" +
@"Date: "+ d.ToString() + "\r\n" +
@"Host: Apache / 2.0.52(CentOS)\r\n" + "\r\n" +
@"Accept - Ranges: bytes\r\n" + "\r\n" +
@"Content - Length: 2652\r\n" + "\r\n" +
@"Keep - Alive: timeout = 10, max = 100\r\n" + "\r\n" +
@"Connection: Keep - Alive\r\n" + "\r\n" +
@"Content - Type: text / html; charset = ISO - 8859 - 1\r\n" + "\r\n" +
@"\r\n" + "\r\n" + content + "\n";
        }

        void responsePostDatabase()
        {
            ktG = true;
            string[] array = richTextBox1.Text.Split('\n');
            string[] kh = array[10].Split('&');
            string id = get.Substring(get.IndexOf("KH"));
            id = id.Substring(0, id.IndexOf("HTTP") - 1);
            string sqlConnect = @"Data Source=DESKTOP-B1JJEDA;Initial Catalog=QLBH_DoAn;Integrated Security=True";
            SqlConnection cn = new SqlConnection(sqlConnect);
            string command1 = "select * from KHACHHANG";
            cn.Open();
            SqlCommand cmd1 = new SqlCommand(command1, cn);
            using (SqlDataReader reader = cmd1.ExecuteReader())
            {

                if (reader.HasRows)
                {
                    ktG = true;
                    while (reader.Read())
                    {
                        if (id == reader.GetString(0).ToString())
                        { ktG = false; break; }
                    }
                }
            }
            if ((kh.Length == 7) && (ktG))
            {
                for (int i = 0; i < kh.Length; i++)
                {
                    kh[i] = kh[i].Split('=')[1];
                }     
                string command = "insert into KHACHHANG values('" + kh[0] + "','" + kh[1] + "','" + kh[2] + "','" + kh[3] + "','" + kh[4] + "','" + kh[5] + "'," + kh[6] + ")";
                SqlCommand cmd = new SqlCommand(command, cn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    ktG = false;
                }
                cn.Close();
            }
            currentMess = createPost(kh.Length, ktG) + "\n";
        }

        void responseGetDatabase()
        {
            ktG = true;
            get = get.Substring(get.IndexOf("?") + 4);
            string id = get.Substring(0, get.IndexOf("HTTP") - 1);
            string sqlConnect = @"Data Source=DESKTOP-B1JJEDA;Initial Catalog=QLBH_DoAn;Integrated Security=True";
            SqlConnection cn = new SqlConnection(sqlConnect);
            string command = "select * from KHACHHANG where MAKH='" + id + "'";
            cn.Open();
            SqlCommand cmd = new SqlCommand(command, cn);
            string data = "";
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    ktG = true;
                    while (reader.Read())
                    {
                        for (int i = 0; i < 4; i++)
                            data += reader.GetString(i).ToString() + "&";
                        data += reader.GetDateTime(4).ToString() + "&" + reader.GetDateTime(5).ToString() + "&";
                        data += reader.GetSqlMoney(6).ToString() + "\n";

                    }
                }
                else ktG = false;
            }
            currentMess = createGet(ktG) + convertHTMLDatabase(data, ktG) + "\n";         
        }
        string get = "";
        bool ktG = true;

        //Receive data from specific client
        void Receive(object obj)
        {
            Socket socketClient = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 50];
                    socketClient.Receive(data);
                    info = socketClient.RemoteEndPoint.ToString();
                    richTextBox1.Text = Encoding.UTF8.GetString(data);
                    richTextBox1.Text = DecryptString("1234567812345678", richTextBox1.Text); 
                    response();
                }
            }
            catch
            {
                listOfClient.Remove(socketClient);
                socketClient.Close();
            }
        }

        //Send data 
        void Send(Socket tcpClient)
        {
            currentMess = EncryptString("1234567812345678", currentMess);
            byte[] dataBroadcast = Encoding.UTF8.GetBytes(currentMess);          
            tcpClient.Send(dataBroadcast);
        }

        private static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        

        void responseGetDatabase_SP()
        {
            string day = "";
            string data = "";
            bool a = true;
            bool v = false;
            if (!richTextBox1.Text.Contains("HTTP/1.1"))
            {
                v = true;
                createGet_SP(ktG, a, v, day, data);
            }
            get = get.Substring(get.IndexOf("?") + 4);
            if(!richTextBox1.Text.Contains("If-modified-since"))
            {
                a = false;
            }  
            else
            {
                a = true;
                day = richTextBox1.Text.Split('\n')[1];
                day = day.Substring(day.IndexOf("since") + 6);
            }    
            string id = get.Substring(0, get.IndexOf("HTTP") - 1);
            string sqlConnect = @"Data Source=DESKTOP-B1JJEDA;Initial Catalog=QLBH_DoAn;Integrated Security=True";
            SqlConnection cn = new SqlConnection(sqlConnect);
            string command = "select * from SANPHAM where TENSP='" + id + "'";
            cn.Open();
            SqlCommand cmd = new SqlCommand(command, cn);
           
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    ktG = true;
                    while (reader.Read())
                    {
                        data += reader.GetString(0).ToString() + "&";
                        data += reader.GetSqlMoney(1).ToString() + "&";
                        data += reader.GetString(2).ToString() + "&" + reader.GetDateTime(3).ToString();
                    }
                }
                else ktG = false;
            }
            currentMess = createGet_SP(ktG, a, v, day, data) + convertHTMLDatabase_SP(data, day, ktG, a, v) + "\n";
        }

        string createGet_SP(bool kt1, bool a, bool v, string day, string input)
        {
            string rp = "";
            if (v)
            {
                rp = "505 HTTP Version Not Supported";
            }
            else if (!kt1)
            {
                rp = "404 Not found";
            }
            else if (!a)
            {
                string temp = input.Split('&')[3];
                rp = "200 OK";
                rp += @"\r\n" + "\r\n";
                rp += "Last-modified " + temp + @"\r\n" + "\r\n";
            }
            else
            {
              
                string temp = input.Split('&')[3];
                if (day == temp)
                {
                    rp = "304 Not-modified";
                }
                else
                {
                    rp = "200 OK";
                    rp += @"\r\n" + "\r\n";
                    rp += "Last-modified " + temp + @"\r\n" + "\r\n";
                }
            }
            DateTime d = DateTime.Now;
            return @"HTTP / 1.1 " + rp +
                @"\r\n" + "\r\n" +
@"Date: "+ d.ToString() + "\r\n" +
@"Host: Apache / 2.0.52(CentOS)\r\n" + "\r\n" +
@"Accept - Ranges: bytes\r\n" + "\r\n" +
@"Content - Length: 2652\r\n" + "\r\n" +
@"Keep - Alive: timeout = 10, max = 100\r\n" + "\r\n" +
@"Connection: Keep - Alive\r\n" + "\r\n" +
@"Content - Type: text / html; charset = ISO - 8859 - 1\r\n" + "\r\n" +
@"\r\n" + "\r\n";
        }

        string convertHTMLDatabase_SP(string input, string day, bool b, bool a, bool v)
        {

            string result = "";
            if (v)
            {
                result = "<html>\n <body>";
                result += "\n" + "  <p>";
                result += "\n\t" + "505 HTTP Version Not Supported";
                result += "\n" + "  </p>\n </body>" + "\n</html>";
                return result;
            }
            string[] kh = input.Split('&');

            if (a == false)
            {
                if (b)
                {
                    result = "\n" + "<Infor>";
                    result += "\n" + "\t" + "<TENSP>" + kh[0] + "</TENSP>";
                    result += "\n" + "\t" + "<GIA>" + kh[1] + "</GIA>";
                    result += "\n" + "\t" + "<NOISX>" + kh[2] + "</NOISX>";
                    result += "\n" + "\t" + "<DAY>" + kh[3] + "</DAY>";
                    result += "\n" + "</Infor>";
                }
                else
                {
                    result = "<html>\n <body>";
                    result += "\n" + "  <p>";
                    result += "\n\t" + "404 Not found";
                    result += "\n" + "  </p>\n </body>" + "\n</html>";
                }
            }
            else
            {
                if (day == kh[3])
                {
                    result = "<html>\n <body>";
                    result += "\n" + "  <p>";
                    result += "\n\t" + "304 Not-modified";
                    result += "\n" + "  </p>\n </body>" + "\n</html>";
                }
                else
                {
                    result = "\n" + "<Infor>";
                    result += "\n" + "\t" + "<TENSP>" + kh[0] + "</TENSP>";
                    result += "\n" + "\t" + "<GIA>" + kh[1] + "</GIA>";
                    result += "\n" + "\t" + "<NOISX>" + kh[2] + "</NOISX>";
                    result += "\n" + "\t" + "<DAY>" + kh[3] + "</DAY>";
                    result += "\n" + "</Infor>";
                }    
            }    
            return result;
        }

        void response()
        {
            get = richTextBox1.Text.Split('\n')[0];
            string pt = get.Substring(0, 4);
            switch (pt)
            {
                case "GET ":
                    if (get.Contains("KH"))
                        responseGetDatabase();
                    else
                        responseGetDatabase_SP();
                    break;
                case "POST":
                    responsePostDatabase();
                    break;
                default:
                    MessageBox.Show("error");
                    break;
            }

            foreach (Socket sk in listOfClient)
            {
                if (sk.RemoteEndPoint.ToString() == info)
                {
                    Send(sk);
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            get = richTextBox1.Text.Split('\n')[0];
            string pt = get.Substring(0, 4);
            switch (pt)
            {
                case "GET ":
                    if (get.Contains("KH"))
                        responseGetDatabase();
                    else
                        responseGetDatabase_SP();
                    break;
                case "POST":
                    responsePostDatabase();
                    break;
                default:
                    MessageBox.Show("error");
                    break;
            }

            foreach (Socket sk in listOfClient)
            {
                if (sk.RemoteEndPoint.ToString() == info)
                {
                    Send(sk);
                    break;
                }
            }
        }

        private void Server_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        void Disconnect()
        {
            listener.Stop();
        }

        void Connect()
        {
            listOfClient = new List<Socket>();
            listener = new TcpListener(8080);
            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        listener.Start();
                        Socket client = listener.AcceptSocket();
                        listOfClient.Add(client);
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    listener = new TcpListener(8080);
                }
            });
            listen.Start();
        }

        private void Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
        }
    }
}
