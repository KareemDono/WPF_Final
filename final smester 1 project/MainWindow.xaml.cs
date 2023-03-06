using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace final_smester_1_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string regexPattern = @"^[a-zA-Z0-9\.\-_]+@[a-zA-Z]{2,15}(?:\.[a-zA-Z]+){1,2}$";

        public MainWindow()
        {
            InitializeComponent();
            DataLoadGrid();
        }

        //sql data base
        SqlConnection connection = new SqlConnection(@"Data Source=SNOOPDOGG\SQLEXPRESS;Initial Catalog=WPFDataBase;Integrated Security=True");

        //loading database table from sql
        public void DataLoadGrid()
        {
            SqlCommand cmd = new SqlCommand("select * from LoginTable", connection);
            DataTable dt = new DataTable();
            connection.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            connection.Close();
            datagrid.ItemsSource = dt.DefaultView;
        }

        //checking if the text boxes are not empty or does not match required input -- sign up
        public bool CheckInputSignUp()
        {
            if (firstNameSignUp.Text == String.Empty)
            {
                MessageBox.Show("First Name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(firstNameSignUp.Text, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("First Name should contain only letters", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (lastNameSignUp.Text == String.Empty)
            {
                MessageBox.Show("Last Name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(lastNameSignUp.Text, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("Last Name should contain only letters", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (emailSignUp.Text == String.Empty)
            {
                MessageBox.Show("Email is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(emailSignUp.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Email should contain 'example1-9@domaina.com'", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (passwordSignUp.Password == String.Empty)
            {
                MessageBox.Show("Password is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(passwordSignUp.Password, @"^.{1,16}$"))
            {
                MessageBox.Show("Password should contain 16 chars max", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        //email send function
        private void SendEmail(object sender, RoutedEventArgs e)
        {
            //tayeb cpehdgmxpignhodn
            //kareem ifuqkdvohbpreqcr

            string from = fromEmail.Text;
            string fromPassword = passwordEmail.Password;
            string to = toEmail.Text;
            string subject1 = subjectEmail.Text;
            string body1 = contentEmail.Text;

            try
            {
                if (!Regex.IsMatch(from, regexPattern) || !Regex.IsMatch(to, regexPattern))
                {
                    MessageBox.Show("Email is incorrect");
                    return;
                }

                MailMessage message = new MailMessage(from, to, subject1, body1);
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(from, fromPassword);
                smtp.Send(message);
                MessageBox.Show("Send successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //sign up function that insert the values of the params to the sql data
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckInputSignUp())
                {
                    SHA256 sha256 = SHA256.Create();
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordSignUp.Password));
                    string passwordHash = Convert.ToBase64String(hashBytes);
                    SqlCommand cmd = new SqlCommand("INSERT INTO LoginTable VALUES (@FirstName, @LastName, @Email, @Password)", connection);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@FirstName", firstNameSignUp.Text);
                    cmd.Parameters.AddWithValue("@LastName", lastNameSignUp.Text);
                    cmd.Parameters.AddWithValue("@Email", emailSignUp.Text);
                    cmd.Parameters.AddWithValue("@Password", passwordHash);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    DataLoadGrid();
                    MessageBox.Show("Registered Successfully", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //sign in function
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();

            SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordSignIn.Password));
            string passwordHash = Convert.ToBase64String(hashBytes);

            string query = $"SELECT * FROM LoginTable WHERE Email='{emailSignIn.Text}' AND Password='{passwordHash}'";

            SqlCommand sqlCommand = new SqlCommand(query, connection);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                MessageBox.Show("Signed In Successfully");
                RefresWindow();
                reader.Close();
            }
            else
            {
                MessageBox.Show("Signed In Failed");
            }

            connection.Close();
        }

        //refresh the window to reset the inputs
        public void RefresWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        //clear fields
        public void Clear()
        {
            firstNameSignUp.Clear();
            lastNameSignUp.Clear();
            emailSignUp.Clear();
            passwordSignUp.Clear();
        }

        //delte the stored valus from the sql data
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("delete from LoginTable", connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Table Has Been Deleted", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);

            }
            finally
            {
                connection.Close();
                Clear();
                DataLoadGrid();
            }
        }


        private void redirectButton_Click(object sender, RoutedEventArgs e)
        {
            StoredProcedure storedProcedure = new StoredProcedure();
            storedProcedure.Show();
        }

        private void firstNameSignUp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void lastNameSignUp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void emailSignUp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void passwordSignUp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void emailSignIn_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void passwordSignIn_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void fromEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void toEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void subjectEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void contentEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ClearDataGrid_Button(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
