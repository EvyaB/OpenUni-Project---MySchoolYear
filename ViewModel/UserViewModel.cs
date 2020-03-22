using MySchoolYear.Model;
using System.Linq;
using System.Collections.Generic;

namespace MySchoolYear.ViewModel
{
    public class UserViewModel
    {
        public List<User> myUsers
        {
            get;
            set;
        }

        public void LoadStudents()
        {
            schoolEntities dbContext = new schoolEntities();
            myUsers = dbContext.Users.ToList();
        }
    }
}