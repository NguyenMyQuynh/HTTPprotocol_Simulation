using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Collections;

namespace MultiClient
{
    public partial class DangKy : Form
    {
        public DangKy()
        {
            InitializeComponent();
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            string pass = GetStringSha256Hash(txbPass.Text);          
            string sqlConnect = @"Data Source=DESKTOP-B1JJEDA;Initial Catalog=QLBH_DoAn;Integrated Security=True";
            SqlConnection conn = new SqlConnection(sqlConnect);
            string sqlInsert = "insert into HASH_PASS values('" + txbUserName.Text + "','" + pass + "')";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sqlInsert, conn);
            try
            {
                cmd.ExecuteNonQuery();              
            }
            catch
            {
                MessageBox.Show("Tài khoản đã tồn tại !");
                return;
            }
            
            
            this.Hide();
            Client client = new Client(txbUserName.Text,txbAddress.Text,txbBirthday.Text,txbRegistday.Text,txbPNumber.Text,txbFullName.Text);
            client.Show();
            txbFullName.Text = "";
            txbPass.Text = "";
            txbPNumber.Text = "";
            txbUserName.Text = "";
            txbUserName.Focus();
            
        }
        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
