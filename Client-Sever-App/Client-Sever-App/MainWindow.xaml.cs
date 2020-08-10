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

namespace Client_Sever_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //Connect();
        }
        IPEndPoint IP;
        Socket Client;
        void Connect(string ip, int port)
        {
            IP = new IPEndPoint(IPAddress.Parse(ip), port);
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                Client.Connect(IP);
                MessageBox.Show("Connect successful", "Information", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch
            {
                MessageBox.Show("cant connect to the sever", "error", MessageBoxButton.OK);
                //return;
            }
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }
        /*void Close1()
        {
            Client.Close();
        }*/
        void Send()
        {
            if (tbMessage.Text != string.Empty)
            {
                Client.Send(Serialize(tbMessage.Text));
                AddMessage(tbMessage.Text);
            }
        }
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    Client.Receive(data);
                    string message = (string)Deserialize(data);
                    AddMessage(message);
                }
            }
            catch
            {
               // Close1();
            }
        }
        void AddMessage(string s)
        {
            this.Dispatcher.Invoke(() => {
                TBwall.Text += s;
                TBwall.Text += "\n";
                tbMessage.Text = "";
            });
        }
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream,obj);// phan manh ap dung 2 cai o tren 
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

        private void btSend_Click(object sender, RoutedEventArgs e)
        {
            Send();
            //AddMessage(tbMessage.Text);
        }

        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
           string ip = TBhost.Text;
           int port = Convert.ToInt32(TBport.Text);
           Connect(ip,port);
          
        }
    }

   

}
