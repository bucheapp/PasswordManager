using PasswordManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// SelectUserWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectUserWindow : Window
    {
        private readonly List<User> _users;
        public string Password => PasswordBox.Password;
        public User SelectedUser { get; set; }
        public SelectUserWindow(List<User> users,User defaultUser,User? prevSelectedUser)
        {
            InitializeComponent();
            _users = users;

            UserComboBox.ItemsSource = _users;
            SelectedUser = prevSelectedUser ?? defaultUser;

            if (_users.Count > 0)
            {
                var targetId = prevSelectedUser?.Id ?? defaultUser.Id;
                UserComboBox.SelectedItem = _users.FirstOrDefault(u => u.Id == targetId);
            } else
            {
                UserComboBox.SelectedIndex = 0;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var user = UserComboBox.SelectedItem as User;

            if (user == null)
            {
                MessageBox.Show("Please select a user.");
                return;
            }

            SelectedUser = user;
            DialogResult = true;
            Close();
        }
    }
}
