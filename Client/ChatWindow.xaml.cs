using SimpleTCP;
using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;
using System.Threading.Tasks;

namespace Client
{
    public partial class ChatWindow : Window
    {
        private string username;
        SimpleTcpClient client;

        IFirebaseConfig fCon = new FirebaseConfig()
        {
            AuthSecret = "3WCno5lT6Er5C3MMpM9GvhvO2F7xitnH1McV1ttK",
            BasePath = "https://instant-messenger-7ba8c-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        IFirebaseClient FBclient;

        private void ConnectClient()
        {
            FBclient = new FireSharp.FirebaseClient(fCon);
        }

        public ChatWindow(string usr)
        {
            InitializeComponent();

            ConnectClient();
            username = usr;
        }

        private void ChatWindowLoaded(object sender, EventArgs e)
        {
            client = new SimpleTcpClient();
            client.StringEncoder = Encoding.UTF8;
            client.DataReceived += Client_DataReceived;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            client.Connect("127.0.0.1", 1111);
            client.Write(">>> " + username + " just joined! <<<\n");
            btnConnect.IsEnabled = false;
        }

        private void Client_DataReceived(object sender, SimpleTCP.Message e)
        {
            if (e.MessageString.Length > 4 && e.MessageString.Contains("list"))
            {
                String msg = e.MessageString.Substring(4, e.MessageString.Length - 4);
                string[] listClients = new string[msg.Split(':').Length - 2];
                int aux = 0;

                for (int i = 0; i < msg.Split(':').Length; i++)
                {
                    if (msg.Split(':')[i] != "" && msg.Split(':')[i] != this.username)
                    {
                        listClients[aux] = msg.Split(':')[i];
                        aux++;
                    }
                }

                listbox.Items.Dispatcher.Invoke((Action)delegate ()
                {
                    listbox.ItemsSource = listClients;
                });
            }
            else
            {
                txtStatus.Dispatcher.Invoke((Action)delegate ()
                {
                    if (e.MessageString.Contains("just joined") && !e.MessageString.Contains(username))
                    {
                        txtStatus.Text += e.MessageString;
                    }
                    else if (e.MessageString.Contains("just joined") && e.MessageString.Contains(username))
                    {
                        txtStatus.Text += "";
                    }
                    else
                    {
                        txtStatus.Text += e.MessageString;
                    }
                });

            }

            if (e.MessageString.Equals("All"))
            {
                txtStatus.Dispatcher.Invoke((Action)delegate ()
                {
                    txtStatus.Text += "";
                });
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (btnConnect.IsEnabled)
            {
                System.Windows.MessageBox.Show("Please connect to the local server!");
            }
            else if (listbox.SelectedIndex != -1)
            {
                if (txtMessage.Text.Length > 0)
                {
                    client.WriteLine(listbox.SelectedItem.ToString() + "#" + username + ": " + txtMessage.Text);
                }
                else
                {
                    System.Windows.MessageBox.Show("Please type something!");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Please select someone to send the message to!");
            }

            // Clear the text message box
            txtMessage.Text = "";
        }

        /**
         * When we press the message text box
         */
        private void txtMessage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) 
        {
            // If the user is connected to a server, writes "list" to the server
            if (!btnConnect.IsEnabled)
                client.WriteLine("list");
            else
                System.Windows.MessageBox.Show("Please connect to the local server!");
        }

        /**
         * Logs off the user when window is closed
         */
        private void Window_Closed(object sender, EventArgs e)
        {
            // If button is not pressed and if we we are connected
            if (!btnConnect.IsEnabled)
            {
                client.Write(username + ":closed");
                client.TcpClient.Close();
            }

            FirebaseResponse response = FBclient.Get("UserList/" + this.username);

            User user = response.ResultAs<User>();
            user.isLoggedIn = 0;

            response = FBclient.Set("UserList/" + this.username, user);

            Close();
        }

        /**
         * Clears the chat log.
         */
        private void Click_clear(object sender, EventArgs e)
        {
            txtStatus.Text = "";
        }

        /**
         * Let's the user drag window with mouse.
         */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /**
         * Closes the app when user clicks the X button, also logs out the user.
         */
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            FirebaseResponse response = FBclient.Get("UserList/" + this.username);

            User user = response.ResultAs<User>();
            user.isLoggedIn = 0;

            response = FBclient.Set("UserList/" + this.username, user);

            Close();
        }
    }
}
