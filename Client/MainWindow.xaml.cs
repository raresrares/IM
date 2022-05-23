using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using System.Net.Sockets;
using SimpleTCP;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        IFirebaseConfig fCon = new FirebaseConfig()
        {
            AuthSecret = "3WCno5lT6Er5C3MMpM9GvhvO2F7xitnH1McV1ttK",
            BasePath = "https://instant-messenger-7ba8c-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        IFirebaseClient client;

        private void ConnectClient()
        {
            client = new FireSharp.FirebaseClient(fCon);
        }

        /**
         * Checks the login info provided.
         */
        private async void CheckLogin(string username, string password) 
        {
            ConnectClient();

            User user = new User()
            {
                UserName = this.usernameBox.Text,
                Password = this.passwordBox.Password,
                isLoggedIn = 1
            };

            // Check if the User exists
            FirebaseResponse r = await client.GetAsync("UserList/" + this.usernameBox.Text);

            User u = r.ResultAs<User>();

            if (u != null)
            {
                if (u.isLoggedIn == 1)
                {
                    MessageBox.Show("The user " + this.usernameBox.Text + " is already logged in!");
                } 
                else
                {
                    if (this.passwordBox.Password == u.Password)
                    {
                        u.isLoggedIn = 1;

                        SetResponse response = await client.SetAsync("UserList/" + u.UserName, u);
                        
                        OpenChatWindow();
                    }
                    else
                    {
                        MessageBox.Show("The password is incorrect!");
                    }
                }
            }
            else
            {
                MessageBox.Show("The user " + this.usernameBox.Text + " does not exist! Register first!");
            }
        }

        private void Login (object sender, RoutedEventArgs e)
        {
            String user = this.usernameBox.Text;
            String pass = this.passwordBox.Password;

            CheckLogin(user, pass);
        }

        private void ClearUsername(object sender, RoutedEventArgs e)
        {
            this.usernameBox.Text = ""; 
        }

        private void ClearPassword(object sender, MouseButtonEventArgs e)
        {
            this.passwordBox.Password = "";
        }

        private void UserForm(object sender, RoutedEventArgs e)
        {
            NewUserForm newUser = new NewUserForm();
            newUser.Show();
            Close();
        }

        private void OpenChatWindow()
        {
            ChatWindow chatWindow = new ChatWindow(this.usernameBox.Text);
            chatWindow.Show();
            Close();
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