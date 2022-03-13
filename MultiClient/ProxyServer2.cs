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
    
   
    public partial class ProxyServer2 : Form
    {
        public ProxyServer2()
        {
            InitializeComponent();
        }
     
        TcpClient Cl = new TcpClient();
        NetworkStream ns = default(NetworkStream);
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

        string currentMess = "";
        TcpListener listener;
        List<Socket> listOfClient;
        string info = "fi";

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
        void receiveFromServer()
        {
            try
            {
                while (true)
                {
                    ns = Cl.GetStream();
                    byte[] data = new byte[20000];
                    ns.Read(data, 0, 1024);
                    string returndata = Encoding.UTF8.GetString(data);
                    string tam = "";
                    for (int i = 0; i < returndata.Length; i++)
                        if (Convert.ToInt32(returndata[i]) != 0)
                            tam += returndata[i];
                        else break;
                    string result = DecryptString("1234567812345678", tam);
                    richTextBox2.Text += result;
                    response2();
                }
            }
            catch
            {
                Cl.Close();
                ns.Close();
            }

        }
        void Connect()
        {
            // Connect to server.
            try
            {
                //kết nối với server có IPEndpoint("127.0.0.1"), 8080);
                Cl.Connect(IPAddress.Parse("127.0.0.1"), 8080);
                CheckForIllegalCrossThreadCalls = false;
                Thread Thread = new Thread(new ThreadStart(receiveFromServer));
                Thread.Start();
            }
            catch
            {
                MessageBox.Show(" Kết nối tới server thất bại ");
            }


            listOfClient = new List<Socket>();
            listener = new TcpListener(8082);
            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        listener.Start();
                        Socket client = listener.AcceptSocket();
                        listOfClient.Add(client);
                        if (listOfClient.Count > 2)
                        {
                            currentMess = "Enough";
                            Send(client);
                            client.Close();
                            listOfClient.Remove(client);
                            continue;
                        }
                        else
                        {
                            currentMess = "Hello";
                            Send(client);
                        }
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    listener = new TcpListener(8082);
                }
            });
            //listen.IsBackground = true;
            listen.Start();
        }

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
                    if (richTextBox1.Text == "")
                    {
                        listOfClient.Remove(socketClient);
                        socketClient.Close();
                        break;
                    }
                    response();
                }
            }
            catch
            {
                listOfClient.Remove(socketClient);
                socketClient.Close();
            }
        }

        void Send(Socket tcpClient)
        {
            currentMess = EncryptString("1234567812345678", currentMess);
            byte[] dataBroadcast = Encoding.UTF8.GetBytes(currentMess);
            tcpClient.Send(dataBroadcast);
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
@"Date: "+d.ToString() + "\r\n" +
@"Host: Apache / 2.0.52(CentOS)\r\n" + "\r\n" +
@"Accept - Ranges: bytes\r\n" + "\r\n" +
@"Content - Length: 2652\r\n" + "\r\n" +
@"Keep - Alive: timeout = 10, max = 100\r\n" + "\r\n" +
@"Connection: Keep - Alive\r\n" + "\r\n" +
@"Content - Type: text / html; charset = ISO - 8859 - 1\r\n" + "\r\n" +
@"\r\n" + "\r\n";
        }

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

        void responseGetDatabase()
        {
            get = get.Substring(get.IndexOf("?") + 4);
            string id = get.Substring(0, get.IndexOf("HTTP") - 1);
            string sqlConnect = @"Data Source=DESKTOP-B1JJEDA;Initial Catalog=ProxyServer_LTMCB2;Integrated Security=True";
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
                    currentMess = createGet(ktG) + convertHTMLDatabase(data, ktG) + "\n";
                }
                else
                {
                    currentMess = "";
                    string ciphertext = EncryptString("1234567812345678", richTextBox1.Text);
                    byte[] dataBytes = Encoding.UTF8.GetBytes(ciphertext);
                    ns.Write(dataBytes, 0, dataBytes.Length);
                    ns.Flush();
                }
            }
        }

        void responsePostDatabase()
        {
            currentMess = "";
            string ciphertext = EncryptString("1234567812345678", richTextBox1.Text);
            byte[] dataBytes = Encoding.UTF8.GetBytes(ciphertext);
            ns.Write(dataBytes, 0, dataBytes.Length);
            ns.Flush();
        }

        bool ktG = true;
        string get;


        private void ProxyServer2_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        void response()
        {
            currentMess = "";
            richTextBox2.Text = "";
            get = richTextBox1.Text.Split('\n')[0];
            string pt = get.Substring(0, 4);

            switch (pt)
            {
                case "GET ":
                    responseGetDatabase();
                    break;
                case "POST":
                    responsePostDatabase();
                    break;
                default:
                    MessageBox.Show("error");
                    break;
            }
            if (currentMess != "")
            foreach (Socket sk in listOfClient)
            {
                if (sk.RemoteEndPoint.ToString() == info)
                {
                    Send(sk);
                    break;
                }
            }
        }

        
        void Disconnect()
        {
            listener.Stop();
        }

        private void ProxyServer2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
        }

        string ExtractData(string Line)
        {
            string result = "";
            int start = Line.IndexOf(">");
            int end = Line.LastIndexOf("</");
            if (end == -1 || start == -1)
            {
                MessageBox.Show("Line doesnt have enough html tag pair");
                return result;
            }
            result = Line.Substring(start + 1, end - start - 1);
            return result;
        }

        void UpdateDatabase_KH()
        {
            get = richTextBox1.Text.Split('\n')[0];
            if (!richTextBox2.Text.Contains("200 OK"))
                return;
            string pt = get.Substring(0, 4);
            string[] temp = richTextBox2.Text.Split('\n');
            string[] i4 = new string[7];
            if (pt == "GET ")
            {
                i4[0] = ExtractData(temp[11]);      // MAKH
                                                 

                i4[1] = ExtractData(temp[12]);      // HOTEN
                                                 

                i4[2] = ExtractData(temp[13]);      // DCHI


                i4[3] = ExtractData(temp[14]);      // SODT
                                                    

                i4[4] = ExtractData(temp[15]);      // NGSINH
          

                i4[5] = ExtractData(temp[16]);      // NGDK
           

                i4[6] = richTextBox2.Text.Substring(richTextBox2.Text.IndexOf("<DOANHSO>") + 9, richTextBox2.Text.IndexOf("</DOANHSO>") - richTextBox2.Text.IndexOf("<DOANHSO>") - 9);      // DOANHSO
        

                string sqlConnect = @"Data Source=DESKTOP-B1JJEDA;Initial Catalog=ProxyServer_LTMCB2;Integrated Security=True";
                SqlConnection cn = new SqlConnection(sqlConnect);
                cn.Open();
                string command = "insert into KHACHHANG values('" + i4[0] + "','" + i4[1] + "','" + i4[2] + "','" + i4[3] + "','" + i4[4] + "','" + i4[5] + "'," + i4[6] + ")";
                SqlCommand cmd = new SqlCommand(command, cn);

                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        void response2()
        {
            if (richTextBox1.Text.Contains("id=KH"))
                UpdateDatabase_KH();
            currentMess = richTextBox2.Text;
            foreach (Socket sk in listOfClient)
            {
                if (sk.RemoteEndPoint.ToString() == info)
                {
                    Send(sk);
                    break;
                }
            }
        }

        
    }
}
