using MySchoolYear.Model;
using System.Linq;
using System.Collections.Generic;

namespace MySchoolYear.ViewModel
{
    public class SchoolInfoViewModel
    {
        public struct Secretary
        {
            public string Name;
            public string Phone;
        }

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

        /// <summary>
        /// Get all of the school info
        /// </summary>
        public SchoolInfoViewModel()
        {
            schoolEntities dbContext = new schoolEntities();
            
            // School basic information
            var schoolInfo = dbContext.SchoolInfoes;
            SchoolName = schoolInfo.Find("schoolName").value;
            SchoolDescription = schoolInfo.Find("schoolDescription").value;
            SchoolImage = schoolInfo.Find("schoolImage").value;

            // Calculate statistics
            NumberOfClasses = dbContext.Classes.Count();
            NumberOfStudents = dbContext.Students.Count();
            ClassAverageSize = NumberOfStudents / NumberOfClasses;
            ScoreAverage = CalcAverageScore(dbContext.Students.ToList());

            // Get the principal information
            var principal = dbContext.Persons.FirstOrDefault(person => person.isPrincipal);
            if (principal != null)
            {
                PrincipalName = principal.firstName + " " + principal.LastName;
                PrincipalEmail = principal.email;
            }

            // Get the secretaries information
            Secretaries = dbContext.Persons.Where(person => person.isSecretary)
                .Select(person => new Secretary { Name = person.firstName + " " + person.LastName, Phone = person.phoneNumber }).ToList();
            
        }

        // Calculate the school's average score by calculating each student's average and then the average of the averages.
        private int CalcAverageScore(List<Student> students)
        {
            // Calculate the average score of each student, then the sum of averages
            int scoresSum = 0;
            students.ForEach(student => scoresSum += student.Scores.Sum(x => x.score) / student.Scores.Count());
            
            // Calculate average
            return scoresSum / NumberOfStudents;
        }
    }
}