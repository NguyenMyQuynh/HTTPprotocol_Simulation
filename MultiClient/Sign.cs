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
    public partial class Sign : Form
    {
        public Sign()
        {
            InitializeComponent();         
        }
        Hashtable hash = new Hashtable();
        private void DangNhap_Click(object sender, EventArgs e)
        {                  
            string sqlConnect = @"Data Source=DESKTOP-B1JJEDA;Initial Catalog=QLBH_DoAn;Integrated Security=True";
            SqlConnection conn = new SqlConnection(sqlConnect);
            string sqlSelect = "Select* from HASH_PASS where TaiKhoan = '" + txbTaiKhoan.Text + "' and MatKhau = '" + GetStringSha256Hash(txbMatKhau.Text) + "'";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sqlSelect, conn);
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read() == true)
            {
                this.Hide();
                Client client = new  Client(txbTaiKhoan.Text);
                client.Show();
            }
            else
            {
                MessageBox.Show("Đăng nhập không thành công, sai tài khoản hoặc mật khẩu !");
                txbMatKhau.Text = "";
                txbTaiKhoan.Text = "";
                txbTaiKhoan.Focus();
            }
        }

        private void DangKy_Click(object sender, EventArgs e)
        {
            this.Hide();
            DangKy dk = new DangKy();
            dk.Show();
        }

        private void Thoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkViewHide_CheckedChanged(object sender, EventArgs e)
        {
            if (chkViewHide.Checked)
            {
                txbMatKhau.UseSystemPasswordChar = true;
                var checkBox = (CheckBox)sender;
                checkBox.Text = "View";
            }    
            else
            {
                txbMatKhau.UseSystemPasswordChar = false;
                var checkBox = (CheckBox)sender;
                checkBox.Text = "Hide";
            }    
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
