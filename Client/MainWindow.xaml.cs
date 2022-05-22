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

        private async void CheckLogin(string username, string password) //Checks the login info provided
        {
            ConnectClient();
            string checkUser = null;
            string checkPassword = null;
            int isLogged = 0;

            //To create a connection to the local database
            //String connectionString = "datasource = localhost; username = root; password = 1234; database = loginnames";
            //MySqlConnection connection = new MySqlConnection(connectionString);
            //MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE username = '" + username + "';", connection);
            //MySqlDataReader reader;

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
                } else
                {
                    u.isLoggedIn = 1;

                    //TODO CHANGE LOGIN STATUS IN FIREBASE

                    if (this.passwordBox.Password == u.Password)
                    {
                        OpenChatWindow();
                    } else
                    {
                        MessageBox.Show("The password " + this.passwordBox.Password + " is wrong!");
                    }
                }
            }
            else
            {
                MessageBox.Show("The user " + this.usernameBox.Text + " does not exist! Register first!");
            }

            //try
            //{
            //    connection.Open();
            //    reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        checkUser = reader[0].ToString();
            //        checkPassword = reader[1].ToString();
            //        isLogged = (reader[2].ToString() == "1" ? 1 : 0);
            //    }

            //    reader.Close();

            //    if (checkUser != null && checkUser.Equals(username) && checkPassword.Equals(password) && isLogged == 0)
            //    {
            //        MySqlCommand upd = new MySqlCommand("UPDATE users SET isLogged='1' WHERE username='" + username + "';", connection);
            //        upd.ExecuteNonQuery();
            //        OpenChatWindow();
            //    }
            //    else
            //    {
            //        if (isLogged == 1) MessageBox.Show("User already logged in!");
            //        else
            //        {
            //            MessageBox.Show("Incorrect user or pass!");
            //            this.usernameBox.Text = ""; //Changes username box to blank
            //            this.passwordBox.Password = ""; //Changes password box to blank
            //        }
            //    }

            //    connection.Close();

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void Login (object sender, RoutedEventArgs e) //Sends the username and password information to check
        {
            String user = this.usernameBox.Text;
            String pass = this.passwordBox.Password;

            CheckLogin(user, pass);
        }

        private void ClearUsername(object sender, RoutedEventArgs e) //Clears the username box when clicked on
        {
            this.usernameBox.Text = ""; 
        }

        private void ClearPassword(object sender, MouseButtonEventArgs e) //Clears the password box when clicked on
        {
            this.passwordBox.Password = "";
        }

        private void UserForm(object sender, RoutedEventArgs e) //Takes the user to the NewUserForm
        {
            NewUserForm newUser = new NewUserForm();
            newUser.Show();
            Close();
        }

        private void OpenChatWindow() //On successful login takes the user to the ChatWindow
        {
            ChatWindow chatWindow = new ChatWindow(this.usernameBox.Text);
            chatWindow.Show();
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) //Lets us drag the window with the mouse
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) //Closes the application when we click the X button
        {
            Close();
        }

    }
}