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
using PictureMemoryTraining.Views.Models;

namespace PictureMemoryTraining.Views
{
    /// <summary>
    /// EditUserProfileView.xaml 的交互逻辑
    /// </summary>
    public partial class EditUserProfileView : UserControl
    {
        public EditUserProfileView()
        {
            InitializeComponent();
        }

        public event EventHandler<UserInfoMode> UserInfoInputed;
        private void ComfirmButton_Onclick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TestCodeTextBox.Text) ||
                string.IsNullOrWhiteSpace(UserNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AgeTextBox.Text) ||
                string.IsNullOrWhiteSpace(TestDateTextBox.Text))
            {
                ComfirmButton.BorderBrush = Brushes.Red;
                return;
            }
            ComfirmButton.BorderBrush = Brushes.Gray;
            UserInfoInputed?.Invoke(this, new UserInfoMode()
            {
                TestCode = TestCodeTextBox.Text.Trim(),
                UserName = UserNameTextBox.Text.Trim(),
                Age = int.Parse(AgeTextBox.Text.Trim()),
                TestDate = DateTime.Parse(TestDateTextBox.Text.Trim()),
            });
        }
    }
}
