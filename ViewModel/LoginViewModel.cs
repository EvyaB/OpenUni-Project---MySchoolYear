using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using MySchoolYear.Model;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// ViewModel for the login process logic.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        #region Data Members
        private IMessageBoxService messageBoxService;
        #endregion

        #region Properties
        public string Username { get; set; }
        public RelayCommand LoginCommand 
        { 
            get;
            set;
        }

        #endregion

        #region Constructor/Destructors
        public LoginViewModel()
        {
            this.messageBoxService = Application.Current.Resources["MessageBoxService"] as IMessageBoxService;
            this.LoginCommand = new RelayCommand(param => AttemptLoginCommand(param));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Try to login with the input username and password.
        /// </summary>
        /// <param name="parameter">IHavePassword object that contains the SecureString input password</param>
        private void AttemptLoginCommand(object parameter)
        {
            SecureString password = (parameter as IHavePassword).SecurePassword;
            var validInput = CheckLoginInputValidity(Username, password);
            
            if (validInput.Valid)
            {
                // Unsecure the password to compare it to the DB - in a proper implementation, the hashed passwords would be compared, but for simplicity
                // we check it unsecured (and the DB holds passwords in plain text for the same reason).
                var unsecuredPassword = password.Unsecure();

                // Search for the user in the DB.
                schoolEntities mySchool = new schoolEntities();
                var myAccount = mySchool.Users.SingleOrDefault(user => user.username == Username && user.password == unsecuredPassword);

                // If the user is found, connect as it and open the application.
                if (myAccount != null && !myAccount.isDisabled)
                {
                    // TODO TODO TODO
                    this.messageBoxService.ShowMessage("Horray", "Yes!", MessageType.OK_MESSAGE);
                }
                // Report incorrect user credentials error.
                else
                {
                    this.messageBoxService.ShowMessage("שם המשתמש ו/או הסיסמא שגויים!", "Login Failed!", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
                }
            }
            // Report invalid credentials error.
            else
            {
                this.messageBoxService.ShowMessage(validInput.ErrorReport, "Login Failed!", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
            }
        }

        /// <summary>
        /// Assistant method to check the validity of the username and password.
        /// </summary>
        /// <param name="username">The Username to check</param>
        /// <param name="password">Secured version of the Password to check</param>
        /// <returns>The validity tests result</returns>
        private ValidityResult CheckLoginInputValidity(string username, SecureString password)
        {
            ValidityResult result = new ValidityResult();
            result.Valid = true;

            if (username == null || username.Length == 0)
            {
                result.ErrorReport = "אנא הכנס שם משתמש";
                result.Valid = false;
            }
            else if (username.Length < Globals.MINIMUM_USERNAME_LENGTH || username.Length > Globals.MAXIMUM_USERNAME_LENGTH)
            {
                result.ErrorReport = string.Format("שם משתמש לא תקין. אורך שם המשתמש חייב להיות בין {0} לבין {1} תווים",
                                                    Globals.MINIMUM_USERNAME_LENGTH, Globals.MAXIMUM_USERNAME_LENGTH);
                result.Valid = false;
            }
            else if (password == null || password.Length == 0)
            {
                result.ErrorReport = "אנא הכנס סיסמא";
                result.Valid = false;
            }
            else if (password.Length < Globals.MINIMUM_PASSWORD_LENGTH || password.Length > Globals.MAXIMUM_PASSWORD_LENGTH)
            {
                result.ErrorReport = string.Format("סיסמא לא תקינה. אורך הסיסמא חייב להיות בין {0} לבין {1} תווים",
                                                    Globals.MINIMUM_PASSWORD_LENGTH, Globals.MAXIMUM_PASSWORD_LENGTH);
                result.Valid = false;
            }

            return result;
        }
        #endregion
    }
}
