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
    /// UpdateServiceInfoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateServiceInfoWindow : Window
    {
        public string NewServiceTitle => TitleBox.Text;
        public string NewServiceUrl => UrlBox.Text;
        public UpdateServiceInfoWindow(string name,string password)
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
