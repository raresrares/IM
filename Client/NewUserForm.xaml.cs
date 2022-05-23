using System;
using System.Windows;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace Client
{
    public partial class NewUserForm : Window
    {
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

        public NewUserForm()
        {
            InitializeComponent();
        }

        /**
         * When the user presses the Submit button (Registering a new user)
         */
        private async void Submit(object sender, RoutedEventArgs e)
        {
            ConnectClient();

            User user = new User()
            {
                UserName = this.newuser.Text,
                Password = this.newpassword.Password,
                isLoggedIn = 0
            };
            
            // Check if the User already exists
            FirebaseResponse r = await client.GetAsync("UserList/"+this.newuser.Text);

            User u = r.ResultAs<User>();

            if (u == null)
            {
                SetResponse response = await client.SetAsync("UserList/"+this.newuser.Text, user);
            }
            else
            {
                MessageBox.Show("The user with the username " + this.newuser.Text + " already exists!");
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) //Lets us drag the window with the mouse
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ClearUsername(object sender, RoutedEventArgs e) //Clears the username when clicked on
        {
            this.newuser.Text = "";
        }

        private void ClearPassword(object sender, MouseButtonEventArgs e) //Clears the password box when clicked on
        {
            this.newpassword.Password = "";
        }

        private void GoBackToLogin(object sender, RoutedEventArgs e) //Opens the MainWindow when user presses the back button
        {
            MainWindow goBack = new MainWindow();
            goBack.Show();
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) //Closes the application when user clicks the X button
        {
            Close();
        }

    }

}
