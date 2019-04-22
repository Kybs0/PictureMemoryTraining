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
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }
        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartTrainingButton.Visibility = Visibility.Collapsed;
        }

        public static readonly DependencyProperty UserInfoProperty = DependencyProperty.Register(
            "UserInfo", typeof(UserInfoMode), typeof(MainView), new PropertyMetadata(default(UserInfoMode)));

        public UserInfoMode UserInfo
        {
            get { return (UserInfoMode)GetValue(UserInfoProperty); }
            set { SetValue(UserInfoProperty, value); }
        }
        private void EditUserProfileView_OnUserInfoInputed(object sender, UserInfoMode userInfoMode)
        {
            UserInfo = userInfoMode;
            userInfoMode.StartRecord();
            UserStartingGrid.Visibility = Visibility.Collapsed;
        }
    }
}
