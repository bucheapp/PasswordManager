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
    public partial class CreateAccountInfoWindow : Window
    {
        public string AccountName => NameBox.Text;
        public string AccountPassword => PasswordBox.Password;
        public string ConfirmPassword => ConfirmPasswordBox.Password;
        public CreateAccountInfoWindow(
            string title
            )
        {
            InitializeComponent();
            Title = $"Create Entry - {title}";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }
    }
}