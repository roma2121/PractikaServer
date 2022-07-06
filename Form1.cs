using System;
using System.Windows.Forms;

using System.Threading;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ActiveControl = stop_button;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (statusServer == 200)
                stop_button_Click(sender, e);
        }

        static ServerObject server;
        static Thread listenThread;

        private byte statusServer = 0;

        private void start_button_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = null;
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();

                start_button.Enabled = false;
                stop_button.Enabled = true;

                statusServer = 200;
            }
            catch (Exception ex)
            {
                server.Disconnect();
                richTextBox1.Text += ex.Message;
            }
        }

        private void stop_button_Click(object sender, EventArgs e)
        {
            server.Disconnect();

            stop_button.Enabled = false;
            start_button.Enabled = true;


            richTextBox1.Text += "Сервер отключен...\n";
        }

        public string FirstName
        {
            get { return richTextBox1.Text; }
            set { richTextBox1.Text = value; }
        }
    }
}
