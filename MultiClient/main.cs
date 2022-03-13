using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiClient
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server server = new Server();
            server.Show();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {          
            Sign sign = new Sign();
            sign.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProxyServer proxy = new ProxyServer();
            proxy.Show();
        }

        private void btn_Pro2_Click(object sender, EventArgs e)
        {
            ProxyServer2 pr = new ProxyServer2();
            pr.Show();
        }
    }
}
