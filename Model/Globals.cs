using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySchoolYear.Model
{
    /// <summary>
    /// This contains global static variables and information
    /// </summary>
    public static class Globals
    {
        public const int MINIMUM_USERNAME_LENGTH = 3;
        public const int MINIMUM_PASSWORD_LENGTH = 4;
        public const int MAXIMUM_USERNAME_LENGTH = 16;
        public const int MAXIMUM_PASSWORD_LENGTH = 16;

        public const string USER_TYPE_STUDENT = "תלמידים";
        public const string USER_TYPE_PARENTS = "הורים";
        public const string USER_TYPE_TEACHERS = "מורים";
        public const string USER_TYPE_SECRETARIES = "מזכירות";
        public const string USER_TYPE_PRINCIPAL = "מנהלים";
    }
}
