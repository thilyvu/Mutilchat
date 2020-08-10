using SimpleTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Connect();

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show("loaded");       
        }
        IPEndPoint IP;
        Socket server;
        List<Socket> clientlist;
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);// phan manh ap dung 2 cai o tren 
            return stream.ToArray();
        }
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            // Close1();
        }

        void Connect()
        {
            clientlist = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Any, 9099);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server.Bind(IP);
            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientlist.Add(client);
                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);

                    }
                }
                catch (Exception e)
                {
                    /*IP = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);*/
                    MessageBox.Show(e.Message);
                }
            });
            listen.IsBackground = true;
            listen.Start();

        }
        void Send(Socket Client)
        {
            if (Client != null && tbMessage.Text != string.Empty)
            {
                Client.Send(Serialize(tbMessage.Text));
            }
        }
        void close()
        {
            server.Close();
        }
        void Receive(object obj)
        {
            Socket Client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    Client.Receive(data);
                    string message = (string)Deserialize(data);
                    foreach (Socket item in clientlist)
                    {
                        if (item != null && item != Client)
                        {
                            item.Send(Serialize(message));
                        }

                    }
                    AddMessage(message);
                }
            }
            catch
            {
                clientlist.Remove(Client);
                //Client.Close();
            }
        }
        void AddMessage(string s)
        {
            this.Dispatcher.Invoke(() => {
                TBwall.Text += s;
                TBwall.Text += "\n";
            });

        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
           
            foreach (Socket item in clientlist)
            {
                
                    if (item == (Socket)(CBClient.SelectedItem))
                    {
                        Send(item);
                        tbMessage.Text = "";
                        return;
                    }
                    else
                    {
                        Send(item);
                    }

               
            }
            AddMessage(tbMessage.Text);
            tbMessage.Text = "";
        }
        

        private void CBClient_MouseMove(object sender, MouseEventArgs e)
        {
           
            CBClient.ItemsSource = clientlist;
           
            
        }

       
    }
}
