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
    /// CreateServiceInfoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CreateServiceInfoWindow : Window
    {
        public string ServiceTitle => TitleBox.Text;
        public string ServiceUrl => UrlBox.Text;
        public string AccountName => NameBox.Text;
        public string AccountPassword => PasswordBox.Password;
        public string ConfirmPassword => ConfirmPasswordBox.Password;
        public CreateServiceInfoWindow(string prevTitle,string prevUrl,string prevName,string prevPassword)
        {
            InitializeComponent();
            TitleBox.Text = prevTitle;
            UrlBox.Text = prevUrl;
            NameBox.Text = prevName;
            PasswordBox.Password = prevPassword;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}