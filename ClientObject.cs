using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Server
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();

                string message = GetMessage();
                userName = message;

                message = userName + " присоединился к серверу\n";

                server.BroadcastMessage(message, this.Id);

                Form1.richTextBox1.Invoke((MethodInvoker)delegate
                {
                    Form1.richTextBox1.Text += message;
                });

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format($"{message}");

                        Form1.richTextBox1.Invoke((MethodInvoker)delegate
                        {
                            Form1.richTextBox1.Text += message;
                        });

                        server.BroadcastMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format($"{userName}: покинул сервер\n");

                        Form1.richTextBox1.Invoke((MethodInvoker)delegate
                        {
                            Form1.richTextBox1.Text += message;
                        });

                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Form1.richTextBox1.Invoke((MethodInvoker)delegate
                {
                    Form1.richTextBox1.Text += $"{e.Message}\n";
                });
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}