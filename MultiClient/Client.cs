using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MultiClient
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }
        TcpClient Cl = new TcpClient();
        TcpClient Cl2 = new TcpClient();
        NetworkStream ns = default(NetworkStream);
        NetworkStream ns1 = default(NetworkStream);
        IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        int number = 0;
        string rq = "";
        string Username = "";
        string fullname = "";
        string birthday = "";
        string registday = "";
        string phonenumber = "";
        string address = "";
        string rtb2 = "";

        void Disconnect()
        {
        
            if (Cl != null)
                Cl.Close();
            if (Cl2 != null)
                Cl2.Close();
        }

        private void Client_Load(object sender, EventArgs e)
        {
    

            richTextBox1.Text = "";
            richTextBox2.Text = "";
            Cl.Connect(IPAddress.Parse("127.0.0.1"), 8081);
            number = 1;
            ns1 = Cl.GetStream();
            byte[] data1 = new byte[200000];
            ns1.Read(data1, 0, 1024);
            string returndata = Encoding.UTF8.GetString(data1);
            string tam = "";
            for (int i = 0; i < returndata.Length; i++)
                if (Convert.ToInt32(returndata[i]) != 0)
                    tam += returndata[i];
                else break;
            string result = DecryptString("1234567812345678", tam);
            if (result == "Enough")
            {
                Cl2.Connect(IPAddress.Parse("127.0.0.1"), 8082);
                number = 2;
            }
            CheckForIllegalCrossThreadCalls = false;
            Thread Thread = new Thread(new ThreadStart(receive));
            Thread.Start();
            if (fullname != "")
            {
                richTextBox1.Text = PhuongThuc2("POST");
                string ciphertext = EncryptString("1234567812345678", richTextBox1.Text);
                if (number == 1)
                    ns = Cl.GetStream();
                else if (number == 2)
                    ns = Cl2.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(ciphertext);
                ns.Write(data, 0, data.Length);
                ns.Flush();
                richTextBox2.Text = "";
            }
        }

        string Xuly(string result)
        {
            rtb2 = "";        
            if (result.IndexOf("<MAKH>")==-1)
            {
                if (result.IndexOf("200 OK") != -1)
                    rtb2 += "Dang ky thanh cong!";
                else if (result.IndexOf("500 Internal Server Error") != -1)
                    rtb2 += "Server dont know how to handle";
            }
            else
            {
                rtb2 += result.Substring(result.IndexOf("<MAKH>") + 6, result.IndexOf("</MAKH>") - result.IndexOf("<MAKH>") - 6) + "\n";
                rtb2 += result.Substring(result.IndexOf("<HOTEN>") + 7, result.IndexOf("</HOTEN>") - result.IndexOf("<HOTEN>") - 7) + "\n";
                rtb2 += result.Substring(result.IndexOf("<DCHI>") + 6, result.IndexOf("</DCHI>") - result.IndexOf("<DCHI>") - 6) + "\n";
                rtb2 += result.Substring(result.IndexOf("<SODT>") + 6, result.IndexOf("</SODT>") - result.IndexOf("<SODT>") - 6) + "\n";
                rtb2 += result.Substring(result.IndexOf("<NGSINH>") + 8, result.IndexOf("</NGSINH>") - result.IndexOf("<NGSINH>") - 8) + "\n";
                rtb2 += result.Substring(result.IndexOf("<NGDK>") + 6, result.IndexOf("</NGDK>") - result.IndexOf("<NGDK>") - 6) + "\n";
                rtb2 += result.Substring(result.IndexOf("<DOANHSO>") + 9, result.IndexOf("</DOANHSO>") - result.IndexOf("<DOANHSO>") - 9) + "\n";
            }           
            return rtb2;
        }


        string Process_Product(string result)
        {
            rtb2 = "";
           
            if(result.Contains("304 Not-modified"))
            {
                string tmp1 = "";
                
                switch (tenSP)
                {
                    case "VAI":
                        tmp1 = temp;
                        break;
                    case "CAM":
                        tmp1 = pro_orange;
                        break;
                    case "NHO":
                        tmp1 = pro_nho;
                        break;
                }
                string[] i4_pro = tmp1.Split('&');
                rtb2 += "TEN SAN PHAM: " + i4_pro[0] + "\n";
                rtb2 += "GIA: " + i4_pro[1] + "\n";
                rtb2 += "XUAT SU: " + i4_pro[2] + "\n";
            }
            else if(result.Contains("404 Not found"))
            {
                switch (tenSP)
                {
                    case "VAI":
                        temp = "";
                        break;
                    case "CAM":
                        pro_orange = "";
                        break;
                    case "NHO":
                        pro_nho = "";
                        break;
                }
                rtb2 += "Server not found any info about product!!!" + "\n";
           
            }
            else if(result.Contains("505 HTTP Version Not Supported"))
            {
                switch (tenSP)
                {
                    case "VAI":
                        temp = "";
                        break;
                    case "CAM":
                        pro_orange = "";
                        break;
                    case "NHO":
                        pro_nho = "";
                        break;
                }
                rtb2 += "HTTP Version Not Supported";
                
            }
            else if(result.Contains("200 OK"))
            {
                switch (tenSP)
                {
                    case "VAI":
                        temp = "";
                        break;
                    case "CAM":
                        pro_orange = "";
                        break;
                    case "NHO":
                        pro_nho = "";
                        break;
                }
                rtb2 += "TEN SAN PHAM: " + result.Substring(result.IndexOf("<TENSP>") + 7, result.IndexOf("</TENSP>") - result.IndexOf("<TENSP>") - 7) + "\n";
                rtb2 += "GIA: " + result.Substring(result.IndexOf("<GIA>") + 5, result.IndexOf("</GIA>") - result.IndexOf("<GIA>") - 5) + "\n";
                rtb2 += "XUAT SU: " + result.Substring(result.IndexOf("<NOISX>") + 7, result.IndexOf("</NOISX>") - result.IndexOf("<NOISX>") - 7) + "\n";
                if (rtb2.Contains("VAI"))
                {
                    temp += result.Substring(result.IndexOf("<TENSP>") + 7, result.IndexOf("</TENSP>") - result.IndexOf("<TENSP>") - 7) + "&";
                    temp += result.Substring(result.IndexOf("<GIA>") + 5, result.IndexOf("</GIA>") - result.IndexOf("<GIA>") - 5) + "&";
                    temp += result.Substring(result.IndexOf("<NOISX>") + 7, result.IndexOf("</NOISX>") - result.IndexOf("<NOISX>") - 7) + "&";
                    temp += result.Substring(result.IndexOf("<DAY>") + 5, result.IndexOf("</DAY>") - result.IndexOf("<DAY>") - 5);
                }
                else if(rtb2.Contains("CAM"))
                {
                    pro_orange += result.Substring(result.IndexOf("<TENSP>") + 7, result.IndexOf("</TENSP>") - result.IndexOf("<TENSP>") - 7) + "&";
                    pro_orange += result.Substring(result.IndexOf("<GIA>") + 5, result.IndexOf("</GIA>") - result.IndexOf("<GIA>") - 5) + "&";
                    pro_orange += result.Substring(result.IndexOf("<NOISX>") + 7, result.IndexOf("</NOISX>") - result.IndexOf("<NOISX>") - 7) + "&";
                    pro_orange += result.Substring(result.IndexOf("<DAY>") + 5, result.IndexOf("</DAY>") - result.IndexOf("<DAY>") - 5);
                }
                else if(rtb2.Contains("NHO"))
                {
                    pro_nho += result.Substring(result.IndexOf("<TENSP>") + 7, result.IndexOf("</TENSP>") - result.IndexOf("<TENSP>") - 7) + "&";
                    pro_nho += result.Substring(result.IndexOf("<GIA>") + 5, result.IndexOf("</GIA>") - result.IndexOf("<GIA>") - 5) + "&";
                    pro_nho += result.Substring(result.IndexOf("<NOISX>") + 7, result.IndexOf("</NOISX>") - result.IndexOf("<NOISX>") - 7) + "&";
                    pro_nho += result.Substring(result.IndexOf("<DAY>") + 5, result.IndexOf("</DAY>") - result.IndexOf("<DAY>") - 5);
                }
            }
            return rtb2;
        }

        void receive()
        {
            try
            {
                while (true)
                {
                    if (number == 1)
                        ns = Cl.GetStream();
                    else if (number == 2)
                        ns = Cl2.GetStream();
                    byte[] data = new byte[200000];
                    ns.Read(data, 0, 1024);
                    string returndata = Encoding.UTF8.GetString(data);
                    string tam = "";
                    for (int i = 0; i < returndata.Length; i++)
                        if (Convert.ToInt32(returndata[i]) != 0)
                            tam += returndata[i];
                        else break;
                    string result = DecryptString("1234567812345678", tam);
                    if (infoOfRequest == "Product")
                        result = Process_Product(result);
                    else
                        result = Xuly(result);
                    richTextBox2.Text += result;
                    if (richTextBox2.Text == "Server dont know how to handle")
                        this.Close();
                }
            }
            catch
            {
                Cl.Close();
                if (ns != null)
                    ns.Close();
            }

        }
        
        
        string createGet1(string url)
        {
            string get = @"GET /?id=" + url + @" HTTP/1.1\r\n";
            string urlHost = "Host:" + "UIT" + @"\r\n";
            string s = get + "\n" + urlHost + "\n" +
@"User - Agent: Firefox / 3.6.10\r\n" + "\n" +
@"Accept: text / html,application / xhtml + xml\r\n" + "\n" +
@"Accept - Language: en - us,en; q = 0.5\r\n" + "\n" +
@"Accept - Encoding: gzip,deflate\r\n" + "\n" +
@"Accept - Charset: ISO - 8859 - 1,utf - 8; q = 0.7\r\n" + "\n" +
@"Keep - Alive: 115\r\n" + "\n" +
@"Connection: keep - alive\r\n" + "\n" +
@"\r\n" + "\n";
            return s;
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

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
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

       
        string createPost1(string url)
        {

            string get = @"POST /?id=" + url + @" HTTP/1.1\r\n";
            string urlHost = "Host:" + "UIT" + @"\r\n";
            string s = get + "\n" + urlHost + "\n" +
@"User - Agent: Firefox / 3.6.10\r\n" + "\n" +
@"Accept: text / html,application / xhtml + xml\r\n" + "\n" +
@"Accept - Language: en - us,en; q = 0.5\r\n" + "\n" +
@"Accept - Encoding: gzip,deflate\r\n" + "\n" +
@"Accept - Charset: ISO - 8859 - 1,utf - 8; q = 0.7\r\n" + "\n" +
@"Keep - Alive: 115\r\n" + "\n" +
@"Connection: keep - alive\r\n" + "\n" +
@"\r\n" + "\n" + "MAKH="+Username +"&HOTEN="+ fullname+"&DCHI="+address+"&SODT="+phonenumber+"&NGSINH="+birthday+"&NGDK="+registday+"&DOANHSO=NULL";
            return s;
        }

        
        string PhuongThuc1(string a)
        {
            switch (a)
            {
                case "GET":
                    return createGet1(Username);
             
                default:
                    MessageBox.Show("error ");
                    string s = "error";
                    return s;
            }
        }
        string PhuongThuc2(string a)
        {
            switch (a)
            {
                case "POST":
                    return createPost1(Username);
                default:
                    MessageBox.Show("error ");
                    string s = "error";
                    return s;
            }
        }
        

        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Disconnect();
        }

        private void btninfo_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            richTextBox1.Text = PhuongThuc1("GET");
            rq = PhuongThuc1("GET");
            infoOfRequest = "Customer";
            string ciphertext = EncryptString("1234567812345678", richTextBox1.Text);
            byte[] data = Encoding.UTF8.GetBytes(ciphertext);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }
        public Client(string content) : this()
        {
            Username = content;
        }
        public Client(string usrname,string Address,string Birthday,string Registday,string Phonenumber,string Fullname) : this()
        {
            Username = usrname;
            address = Address;
            birthday = Birthday;
            registday = Registday;
            phonenumber = Phonenumber;
            fullname = Fullname;
  
        }

        /* TENSP&GIA&NOISX&DAY */
        string temp = "";

        string pro_orange = "";
        string pro_nho = "";

        string createGet_SP(string ten)
        {
            
            string get = @"GET /?id=" + ten + @" HTTP/1.1\r\n";
            string urlHost = "Host:" + "UIT" + @"\r\n";
            string t ="";
            string tmp1="";
            switch (ten)
            {
                case "VAI":
                    tmp1 = temp;
                    break;
                case "CAM":
                    tmp1 = pro_orange;
                    break;
                case "NHO":
                    tmp1 = pro_nho;
                    break;
            }
            if (tmp1 != "")
            {
                t = "If-modified-since " + tmp1.Split('&')[3];
                get += "\n" + t;
            }
            string s = get + "\n" + urlHost + "\n" +
@"User - Agent: Firefox / 3.6.10\r\n" + "\n" +
@"Accept: text / html,application / xhtml + xml\r\n" + "\n" +
@"Accept - Language: en - us,en; q = 0.5\r\n" + "\n" +
@"Accept - Encoding: gzip,deflate\r\n" + "\n" +
@"Accept - Charset: ISO - 8859 - 1,utf - 8; q = 0.7\r\n" + "\n" +
@"Keep - Alive: 115\r\n" + "\n" +
@"Connection: keep - alive\r\n" + "\n" +
@"\r\n" + "\n";
            return s;
        }

        string tenSP = "";

        string PhuongThuc_SanPham(string a)
        {
            switch (a)
            {
                case "GET":
                    return createGet_SP(tenSP);
                default:
                    MessageBox.Show("error ");
                    string s = "error";
                    return s;
            }
        }

        string infoOfRequest = "";
                       
        
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            tenSP = "VAI";
            richTextBox2.Text = "";
            richTextBox1.Text = PhuongThuc_SanPham("GET");
            rq = PhuongThuc_SanPham("GET");
            infoOfRequest = "Product";
            string ciphertext = EncryptString("1234567812345678", richTextBox1.Text);
            byte[] data = Encoding.UTF8.GetBytes(ciphertext);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
          
            tenSP = "NHO";
            richTextBox2.Text = "";
            richTextBox1.Text = PhuongThuc_SanPham("GET").Replace("HTTP/1.1", "HTTP/1.0");
            rq = PhuongThuc_SanPham("GET");
            infoOfRequest = "Product";
            string ciphertext = EncryptString("1234567812345678", richTextBox1.Text);
            byte[] data = Encoding.UTF8.GetBytes(ciphertext);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            tenSP = "CAM";
            richTextBox2.Text = "";
            richTextBox1.Text = PhuongThuc_SanPham("GET");
            rq = PhuongThuc_SanPham("GET");
            infoOfRequest = "Product";
            string ciphertext = EncryptString("1234567812345678", richTextBox1.Text);
            byte[] data = Encoding.UTF8.GetBytes(ciphertext);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }
    }   
}
