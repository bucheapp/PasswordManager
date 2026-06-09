using PasswordManager.Models;
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
using System.Windows.Shapes;

namespace PasswordManager
{
    /// <summary>
    /// UpdateAccountInfoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateAccountInfoWindow : Window
    {
        public string NewAccountName => NameBox.Text;
        public string NewAccountPassword => PasswordBox.Password;
        public string ConfirmPassword => ConfirmPasswordBox.Password;
        public UpdateAccountInfoWindow(string name,string password)
        {
            InitializeComponent();
            NameBox.Text = name;
            PasswordBox.Password = password;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
