﻿using MySchoolYear.Model;
using System.Linq;
using System.Collections.Generic;
using MySchoolYear.ViewModel.Utilities;
using System;

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
        // Base Properties
        public string ScreenName { get { return "אודות"; } }
        public Person ConnectedPerson { get; private set; }
        public bool HasRequiredPermissions { get; private set; }

        // Business Logic Properties
        public string SchoolName { get; private set; }
        public string SchoolDescription { get; private set; }
        public string SchoolImage { get; private set; }
        public int NumberOfStudents { get; private set; }
        public int NumberOfClasses { get; private set; }
        public float ClassAverageSize { get; private set; }
        public double ScoreAverage { get; private set; }
        public string PrincipalName { get; private set; }
        public string PrincipalEmail { get; private set; }
        public List<Secretary> Secretaries { get; set; }

        #endregion

        #region Constructors
        public SchoolInfoViewModel(Person connectedPerson)
        {
            HasRequiredPermissions = true;
        }
        #endregion

        #region Methods
        public void Initialize(Person connectedPerson)
        {
            ConnectedPerson = connectedPerson;
            SchoolEntities dbContext = new SchoolEntities();

            // School basic information
            var schoolInfo = dbContext.SchoolInfo;
            SchoolName = schoolInfo.Find("schoolName").value;
            SchoolDescription = schoolInfo.Find("schoolDescription").value;

            // Get the full path to the school logo image
            string imageSrc = schoolInfo.Find("schoolImage").value;
            SchoolImage = imageSrc.Contains(":\\") ? imageSrc : "/MySchoolYear;component/Images/" + imageSrc;

            // Calculate statistics
            var relevantStudentsQuery = dbContext.Students.Where(student => !student.Person.User.isDisabled);
            NumberOfClasses = dbContext.Classes.Count();
            NumberOfStudents = relevantStudentsQuery.Count();
            ClassAverageSize = NumberOfStudents / NumberOfClasses;
            ScoreAverage = CalcAverageScore(relevantStudentsQuery.ToList());

            // Get the principal information
            var principal = dbContext.Persons.FirstOrDefault(person => person.isPrincipal && !person.User.isDisabled);
            if (principal != null)
            {
                PrincipalName = principal.firstName + " " + principal.lastName;
                PrincipalEmail = principal.email;
            }

            // Get the secretaries information
            Secretaries = dbContext.Persons.Where(person => person.isSecretary && !person.User.isDisabled).ToList()
                .Select(person => new Secretary() { Name = person.firstName + " " + person.lastName, Phone = person.phoneNumber })
                .ToList();
        }

        /// <summary>
        /// Calculate the school's average score by calculating each student's average and then the average of the averages.
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        private double CalcAverageScore(List<Student> students)
        {
            // Calculate the average score of each student, then the sum of averages
            double scoresSum = 0;
            int releventStudentsNumber = 0;
            foreach (Student student in students)
            {
                // The student has any scores
                if (student.Scores != null && student.Scores.Count > 0)
                {
                    scoresSum += Math.Round(student.Scores.Average(x => x.score), 1);
                    releventStudentsNumber++;
                }
            }

            // Calculate average
            return Math.Round(scoresSum / releventStudentsNumber, 1);
        }

        #endregion
    }
}