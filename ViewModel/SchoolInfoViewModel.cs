using MySchoolYear.Model;
using System.Linq;
using System.Collections.Generic;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// The school's about page - display lots of different data about the school
    /// </summary>
    public class SchoolInfoViewModel : BaseViewModel, IScreenViewModel
    {
        #region Sub-Structs
        public struct Secretary
        {
            public string Name { get; set; }
            public string Phone { get; set; }
        }
        #endregion

        #region Properties
        // Base Properites
        public string ScreenName { get { return "אודות"; } }
        public User Session { get; }
        public bool HasRequiredPermissions { get; private set; }

        // Business Logic Properties
        public string SchoolName { get; private set; }
        public string SchoolDescription { get; private set; }
        public string SchoolImage { get; private set; }
        public int NumberOfStudents { get; private set; }
        public int NumberOfClasses { get; private set; }
        public float ClassAverageSize { get; private set; }
        public int ScoreAverage { get; private set; }
        public string PrincipalName { get; private set; }
        public string PrincipalEmail { get; private set; }
        public List<Secretary> Secretaries { get; set; }

        #endregion

        #region Constructors/Destructor
        public SchoolInfoViewModel(User session)
        {
            HasRequiredPermissions = true;
            Session = session;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Calculate the school's average score by calculating each student's average and then the average of the averages.
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        private int CalcAverageScore(List<Student> students)
        {
            // Calculate the average score of each student, then the sum of averages
            int scoresSum = 0;
            students.ForEach(student => scoresSum += student.Scores.Sum(x => x.score) / student.Scores.Count());
            
            // Calculate average
            return scoresSum / NumberOfStudents;
        }

        public void Initialize()
        {
            schoolEntities dbContext = new schoolEntities();

            // School basic information
            var schoolInfo = dbContext.SchoolInfoes;
            this.SchoolName = schoolInfo.Find("schoolName").value;
            this.SchoolDescription = schoolInfo.Find("schoolDescription").value;
            this.SchoolImage = schoolInfo.Find("schoolImage").value;

            // Calculate statistics
            this.NumberOfClasses = dbContext.Classes.Count();
            this.NumberOfStudents = dbContext.Students.Count();
            this.ClassAverageSize = NumberOfStudents / NumberOfClasses;
            this.ScoreAverage = CalcAverageScore(dbContext.Students.ToList());

            // Get the principal information
            var principal = dbContext.Persons.FirstOrDefault(person => person.isPrincipal);
            if (principal != null)
            {
                this.PrincipalName = principal.firstName + " " + principal.LastName;
                this.PrincipalEmail = principal.email;
            }

            // Get the secretaries information
            this.Secretaries = dbContext.Persons.Where(person => person.isSecretary).ToList()
                .Select(person => new Secretary() { Name = person.firstName + " " + person.LastName, Phone = person.phoneNumber })
                .ToList();
        }
        #endregion
    }
}