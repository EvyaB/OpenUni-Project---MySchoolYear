using System.Collections.Generic;
using System.Linq;
using MySchoolYear.Model;
using MySchoolYear.Utilities;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// ViewModel for the login process logic.
    /// </summary>
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public RelayCommand LoginCommand 
        { 
            get;
            set;
        }

        private bool AttemptLoginCommand()
        {
            schoolEntities mySchool = new schoolEntities();
            var myAccount = mySchool.Users.SingleOrDefault(user => user.username == Username && user.password == Password);

            if (myAccount != null)
            {
                return true;
            }

            return false;
        }
    }
}
