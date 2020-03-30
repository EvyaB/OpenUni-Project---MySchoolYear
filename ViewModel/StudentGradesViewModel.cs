using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MySchoolYear.Model;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// Student's grades summary page.
    /// </summary>
    public class StudentGradesViewModel : BaseViewModel, IScreenViewModel
    {
        #region Sub-Structs
        public struct Grade
        {
            public string CourseName { get; set; }
            public int Score { get; set; }
            public string TeacherNotes { get; set; }
        }
        #endregion

        #region Fields
        private Student _currentStudent;
        private List<Grade> _grades;
        private double _averageGrade;
        private List<Student> _students;
        private ICommand _changeStudentCommand;
        #endregion

        #region Properties / Commands
        // Base Properties
        public Person ConnectedUser { get; }
        public bool HasRequiredPermissions { get; }
        public string ScreenName { get { return "סיכום לימודים"; } }
        #endregion

        // Business Logic Properties / Commands
        /// <summary>
        /// The grades of the student
        /// </summary>
        public List<Grade> Grades
        {
            get
            {
                return _grades;
            }
            set
            {
                if (_grades != value)
                {
                    _grades = value;
                    OnPropertyChanged("Grades");

                    // Update the average grade
                    if (_grades != null && _grades.Count > 0)
                    {
                        AverageGrade =  Math.Round(_grades.Average(x => x.Score), 1);
                    } 
                }
            }
        }
        /// <summary>
        /// The average of the student's grades
        /// </summary>
        public double AverageGrade
        {
            get
            {
                return _averageGrade;
            }
            set
            {
                if (_averageGrade != value)
                {
                    _averageGrade = value;
                    OnPropertyChanged("AverageGrade");
                }
            }
        }
        /// <summary>
        /// The student whose grades are viewed currently
        /// </summary>
        public Student CurrentStudent
        {
            get
            {
                return _currentStudent;
            }
            set
            {
                if (_currentStudent != value)
                {
                    _currentStudent = value;
                    OnPropertyChanged("CurrentStudent");

                    // Show this student's grades
                    Grades = _currentStudent.Scores.Select(score =>
                    new Grade() { CourseName = score.Course.courseName, Score = score.score, TeacherNotes = score.notes })
                    .ToList();
                }
            }
        }
        /// <summary>
        /// All students that the current user can view
        /// </summary>
        public List<Student> Students
        {
            get
            {
                return _students;
            }
            set
            {
                if (_students != value)
                {
                    _students = value;
                    OnPropertyChanged("Students");
                }
            }
        }
        public bool CanViewDifferentStudents { get; private set; }

        /// <summary>
        /// Changes which student's grades are viewed
        /// </summary>
        public ICommand ChangeStudentCommand
        {
            get
            {
                if (_changeStudentCommand == null)
                {
                    _changeStudentCommand = new RelayCommand(
                        p => ChangeStudent((Student)p),
                        p => p is Student);
                }

                return _changeStudentCommand;
            }
        }

        #region Constructor / Destructor
        public StudentGradesViewModel(Person currentUser)
        {
            this.ConnectedUser = currentUser;

            // Check if this page is relevent to the user - is a student, parent or homeroom teacher
            if (currentUser.isStudent || currentUser.isParent || currentUser.isTeacher && currentUser.Teacher.classID != null)
            {
                HasRequiredPermissions = true;
            }

            Students = new List<Student>();
            Grades = new List<Grade>();
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            if (HasRequiredPermissions)
            {
                // Initialize the lists according to the user type
                if (ConnectedUser.isStudent)
                {
                    // A student can only see his own grades
                    Students.Add(ConnectedUser.Student);
                    CanViewDifferentStudents = false;
                }
                else if (ConnectedUser.isTeacher && ConnectedUser.Teacher.classID != null)
                {
                    // An homeroom teacher can see the grades of all of his students
                    Students.AddRange(ConnectedUser.Teacher.Class.Students);
                    CanViewDifferentStudents = true;
                }
                else if (ConnectedUser.isParent)
                {
                    // A parent can see the grades of all of his children
                    Students.AddRange(ConnectedUser.ChildrenStudents);
                    CanViewDifferentStudents = true;
                }

                CurrentStudent = Students.First();
            }
        }

        /// <summary>
        /// Changes which student's grades are viewed
        /// </summary>
        /// <param name="newStudent">The new student whose grades should be shown</param>
        private void ChangeStudent(Student newStudent)
        {
            // Make sure newStudent is one of the students that can be shown (is part of Students list)
            if (Students.Contains(newStudent))
            {
                CurrentStudent = newStudent;
            }
        }
        #endregion
    }
}
