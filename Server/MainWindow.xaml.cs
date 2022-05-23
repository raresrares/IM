using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Input;

namespace Server
{
    public partial class MainWindow : Window
    {        
        SimpleTcpServer server;
        List<ClientDetails> listOfClients = new List<ClientDetails>();

        class ClientDetails
        {
            String username;
            Socket userSocket;
            public ClientDetails(string username, Socket userSocket)
            {
                this.username = username;
                this.userSocket = userSocket;
            }
            public String getUsername()
            {
                return this.username;
            }
            public Socket getSocket()
            {
                return this.userSocket;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerLoaded(object sender, RoutedEventArgs e)
        {
            server = new SimpleTcpServer();
            server.Delimiter = 0x13; // enter 
            server.StringEncoder = Encoding.UTF8;
            server.DataReceived += Server_DataReceived;
        }

        private void Server_DataReceived(object sender, Message e)
        {
            string listToBeSent = "list:All:";

            if (e.MessageString.Contains("just joined"))
            {
                String user = e.MessageString.Substring(4, e.MessageString.Length - 4 - 18);
                Socket socket = e.TcpClient.Client;

                listOfClients.Add(new ClientDetails(user, socket));

                server.Broadcast(e.MessageString);
                txtStatus.Dispatcher.Invoke((Action)delegate ()
                {
                    txtStatus.Text += e.MessageString;
                });
            }

            if (e.MessageString.Contains("list"))
            {
                foreach (ClientDetails username in listOfClients)
                {
                    listToBeSent += username.getUsername() + ":";
                }

                List<byte> vs = new List<byte>();

                vs.AddRange(Encoding.UTF8.GetBytes(listToBeSent));

                e.TcpClient.Client.Send(vs.ToArray());
            }

            if (e.MessageString.Contains("#"))
            {
                if (!e.MessageString.Substring(0, 3).Equals("All"))
                {
                    int posOfDel = e.MessageString.IndexOf('#');
                    string user = e.MessageString.Substring(0, posOfDel);
                    
                    string msg = "[Private] " + e.MessageString.Substring(posOfDel + 1, e.MessageString.Length - posOfDel - 1) + "\n";
                    Socket replySock = findSocket(user, listOfClients);

                    if (replySock != null)
                    {
                        List<byte> vs = new List<byte>();
                        vs.AddRange(Encoding.UTF8.GetBytes(msg));
                        replySock.Send(vs.ToArray());
                        e.TcpClient.Client.Send(vs.ToArray());
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(user + " has logged out!");
                    }
                }
            }

            // Remove the user from the list when he leaves the server.
            if (e.MessageString.Contains("closed"))
            {
                string username = e.MessageString.ToString().Split(':')[0];

                for (int i = 0; i < listOfClients.Count(); i++)
                {
                    if (listOfClients[i].getUsername() == username)
                        listOfClients.RemoveAt(i);
                }

                // Chat log
                server.Broadcast(">>> " + username + " just left! <<<\n");

                txtStatus.Dispatcher.Invoke((Action)delegate ()
                {
                    // Server log
                    txtStatus.Text += ">>> " + username + " just left! <<<\n";
                });

            }
        }

        /**
         *  Start the server disable the start button.
         */
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            server.Start(ip, 1111);
            btnStart.IsEnabled = false;
            txtStatus.Text = "Server is up and running!\n";
        }

        /**
         * If the server closes.
         */
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (server.IsStarted)
            {
                server.Broadcast(">>>The server has crashed! Please close this window and try again later!<<<");
                server.Stop();
                btnStart.IsEnabled = true;
                txtStatus.Text = "Server stopped.\n";
            }
        }

        private Socket findSocket(string user, List<ClientDetails> list)
        {
            Socket userSocket;
            int counter = 0;

            foreach (ClientDetails details in list)
            {
                if (details.getUsername().Equals(user)) break;
                counter++;
            }

            ClientDetails resSock = list.ElementAt(counter);
            userSocket = resSock.getSocket();

            return userSocket;
        }

        private string findUsername(Socket socket, List<ClientDetails> list)
        {
            string user;
            int counter = 0;

            foreach (ClientDetails details in list)
            {
                if (details.getSocket().Equals(socket)) break;
                counter++;
            }

            ClientDetails resUser = list.ElementAt(counter);
            user = resUser.getUsername();

            return user;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}
