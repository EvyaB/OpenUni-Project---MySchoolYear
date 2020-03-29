using MySchoolYear.Model;
using System.Linq;
using System.Collections.Generic;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.ViewModel
{
    public class UserViewModel : BaseViewModel, IScreenViewModel
    {
        public List<User> myUsers
        {
            get;
            set;
        }

        public string ScreenName { get { return "זמני"; } }

        public Person ConnectedUser { get; }

        public bool HasRequiredPermissions { get { return true; } }

        public void Initialize()
        {
        }

        public void LoadStudents()
        {
            schoolEntities dbContext = new schoolEntities();
            myUsers = dbContext.Users.ToList();
        }
    }
}