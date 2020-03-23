using System.Security;
using System.Windows;
using MySchoolYear.View.Utilities;
using MySchoolYear.ViewModel;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, IHavePassword
    {
        public LoginWindow()
        {
            InitializeComponent();
            //LoginViewModel VM = new LoginViewModel(new WPFMessageBoxService());
            //this.DataContext = VM;
        }

        /// <summary>
        /// The secure password for this login page
        /// </summary>
        public SecureString SecurePassword => PasswordText.SecurePassword;
    }
}
