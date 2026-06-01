using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace PasswordManager
{
    /// <summary>
    /// CreateUserWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CreateUserWindow : Window
    {
        public string UserName => NameBox.Text;
        public string Password => PasswordBox.Password;
        public string ConfirmPassword => ConfirmPasswordBox.Password;
        public CreateUserWindow()
        {
            InitializeComponent();
        }

        public void SetPreviousData(
            string userName,
            string password
            )
        {
            NameBox.Text = userName;
            PasswordBox.Password = password;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
