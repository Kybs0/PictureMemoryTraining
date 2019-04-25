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
using PictureMemoryTraining.Business.Excel;
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
            IsVisibleChanged += EditUserProfileView_IsVisibleChanged;
        }

        private void EditUserProfileView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool visible && visible)
            {
                TestDateTextBox.Text = DateTime.Now.ToString();
            }
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
                Age = double.Parse(AgeTextBox.Text.Trim()),
                TestDate = DateTime.Parse(TestDateTextBox.Text.Trim()),
            });
            TestCodeTextBox.Text = string.Empty;
            UserNameTextBox.Text = string.Empty;
            AgeTextBox.Text = string.Empty;
            TestDateTextBox.Text = string.Empty;
        }
    }
}
