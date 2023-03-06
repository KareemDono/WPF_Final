using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace final_smester_1_project
{
    /// <summary>
    /// Interaction logic for StoredProcedure.xaml
    /// </summary>
    public partial class StoredProcedure : Window
    {
        private SqlConnection connection;
        private SqlCommand cmd;

        public StoredProcedure()
        {
            InitializeComponent();

            connection = new SqlConnection(@"Data Source=SNOOPDOGG\SQLEXPRESS;Initial Catalog=WPFDataBase;Integrated Security=True");
            cmd = new SqlCommand();
            cmd.Connection = connection;

            LoadData();
        }

        
        //display the values of the params by the id that the user insert
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            int employeeID;
            if (int.TryParse(txtEmployeeID.Text, out employeeID))
            {
                cmd.CommandText = "SELECT * FROM Employee WHERE EmployeeID = @EmployeeID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dt);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

                if (dt.Rows.Count > 0)
                {
                    txtEmployeeName.Text = dt.Rows[0]["EmployeeName"].ToString();
                    txtEmployeeEmail.Text = dt.Rows[0]["EmployeeEmail"].ToString();
                    txtEmployeeAge.Text = dt.Rows[0]["EmployeeAge"].ToString();
                    txtEmployeeCity.Text = dt.Rows[0]["EmployeeCity"].ToString();
                }
                else
                {
                    MessageBox.Show("Employee not found.");
                }
            }
            else
            {
                MessageBox.Show("Employee ID must be an integer.");
            }
        }

        //delete the values of the params from the sql data
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int employeeID;
            if (int.TryParse(txtEmployeeID.Text, out employeeID))
            {
                cmd.CommandText = "DELETE FROM Employee WHERE EmployeeID = @EmployeeID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

                LoadData();
            }
            else
            {
                MessageBox.Show("Employee ID must be an integer.");
            }
        }

        //insert the user inputs to the sql data
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            int employeeID;
            int employeeAge;
            if (!int.TryParse(txtEmployeeID.Text, out employeeID))
            {
                MessageBox.Show("ID is required and should only contain numbers", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(txtEmployeeAge.Text, out employeeAge))
            {
                MessageBox.Show("Age is required and should only contain numbers", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CheckUserInput())
            {
                string employeeName = txtEmployeeName.Text;
                string employeeEmail = txtEmployeeEmail.Text;
                string employeeCity = txtEmployeeCity.Text;

                cmd.CommandText = "dbo.Employee_Insert";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                cmd.Parameters.AddWithValue("@EmployeeEmail", employeeEmail);
                cmd.Parameters.AddWithValue("@EmployeeAge", employeeAge);
                cmd.Parameters.AddWithValue("@EmployeeCity", employeeCity);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

                LoadData();
            }
        }

        //update the user inputs to the sql data
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int employeeID;
            int employeeAge;
            if (!int.TryParse(txtEmployeeID.Text, out employeeID))
            {
                MessageBox.Show("ID is required and should only contain numbers", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(txtEmployeeAge.Text, out employeeAge))
            {
                MessageBox.Show("Age is required and should only contain numbers", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CheckUserInput())
            {
                string employeeName = txtEmployeeName.Text;
                string employeeEmail = txtEmployeeEmail.Text;
                string employeeCity = txtEmployeeCity.Text;

                cmd.CommandText = "UPDATE Employee SET EmployeeName = @EmployeeName, EmployeeEmail = @EmployeeEmail, EmployeeAge = @EmployeeAge, EmployeeCity = @EmployeeCity WHERE EmployeeID = @EmployeeID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@EmployeeName", employeeName);
                cmd.Parameters.AddWithValue("@EmployeeEmail", employeeEmail);
                cmd.Parameters.AddWithValue("@EmployeeAge", employeeAge);
                cmd.Parameters.AddWithValue("@EmployeeCity", employeeCity);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }

                LoadData();
            }
        }

        //load function
        private void LoadData()
        {
            cmd.CommandText = "SELECT * FROM Employee";
            cmd.CommandType = CommandType.Text;

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            try
            {
                connection.Open();
                adapter.Fill(dt);
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            dgEmployees.ItemsSource = dt.DefaultView;
        }

        //checking if the text boxes are not empty or does not match required input
        public bool CheckUserInput()
        {
            if (txtEmployeeName.Text == String.Empty)
            {
                MessageBox.Show("Name is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(txtEmployeeName.Text, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("Name should contain only letters", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (txtEmployeeEmail.Text == String.Empty)
            {
                MessageBox.Show("Email is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(txtEmployeeEmail.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show("Email should contain 'example1-9@domaina.com'", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (txtEmployeeCity.Text == String.Empty)
            {
                MessageBox.Show("City is required", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(txtEmployeeCity.Text, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("City should contain only letters", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        //redirect the main window
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void txtEmployeeEmail_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtEmployeeName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtEmployeeID_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void dgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txtEmployeeAge_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtEmployeeCity_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
       
    }
}
