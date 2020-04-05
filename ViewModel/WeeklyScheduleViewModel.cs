﻿using MySchoolYear.Model;
using MySchoolYear.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// Display the weekly schedule of the relevant class
    /// </summary>
    public class WeeklyScheduleViewModel : BaseViewModel, IScreenViewModel
    {
        #region SubStructs
        public class LessonData
        {
            public string CourseName { get; set; }
            public string TeacherName { get; set; }
            public string RoomName { get; set; }
            public string ClassName { get; set; }

            public LessonData(Lesson lesson)
            {
                CourseName = lesson.Course.courseName;
                TeacherName = lesson.Teacher.Person.firstName + " " + lesson.Teacher.Person.lastName;
                ClassName = "כיתה " + lesson.Class.className;

                // A room name is optional
                if (lesson.Room != null)
                {
                    RoomName = "חדר " + lesson.Room.roomName;
                }
                else
                {
                    RoomName = String.Empty;
                }
            }
        }
        public class ScheduleData
        {
            public bool IsPersonSchedule { get; set; }
            public bool IsClassSchedule { get; set; }
            public int ID { get; set; }
            public string Name { get; set; }
            public LessonData[,] ActualSchedule { get; set; }
        }
        #endregion

        #region Fields
        private LessonData[,] _schedule;
        private ScheduleData _selectedSchedule;
        private List<ScheduleData> _availableSchedules;
        #endregion

        #region Properties / Commands
        // Base Properties
        public Person ConnectedUser { get; }
        public bool HasRequiredPermissions { get; }
        public string ScreenName { get { return "לוח שעות"; } }

        // Business Logic Properties
        public LessonData[,] Schedule 
        { 
            get
            {
                return _schedule;
            }
            set
            {
                if (_schedule != value)
                {
                    _schedule = value;
                    OnPropertyChanged("Schedule");
                }
            }
        }
        public bool CanViewDifferentSchedules { get; private set; }
        public ScheduleData SelectedSchedule 
        { 
            get
            {
                return _selectedSchedule;
            }
            set
            {
                // Check if the input and _selectedSchedule are different
                if  (_selectedSchedule == null && value != null || _selectedSchedule.ID != value.ID ||
                    _selectedSchedule.IsClassSchedule != value.IsClassSchedule || _selectedSchedule.IsPersonSchedule != value.IsPersonSchedule)
                {
                    _selectedSchedule = value;
                    Schedule = _selectedSchedule.ActualSchedule;
                    OnPropertyChanged("SelectedSchedule");
                }
            }
        }
        public List<ScheduleData> AvailableSchedules
        { 
            get
            {
                return _availableSchedules;
            }
            set
            {
                if (_availableSchedules != value)
                {
                    _availableSchedules = value;
                    OnPropertyChanged("AvailableSchedules");
                }
            }
        }
        #endregion

        #region Constructors
        public WeeklyScheduleViewModel(Person connectedUser)
        {
            HasRequiredPermissions = true;
            ConnectedUser = connectedUser;
            AvailableSchedules = new List<ScheduleData>();
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            // Reset data
            Schedule = new LessonData[Globals.DAY_NAMES.Length, Globals.HOUR_NAMES.Length];
            AvailableSchedules.Clear();
            
            // Find the currently displayed class, as well as fill the available classes list depending on the connected user
            if (ConnectedUser.isStudent)
            {
                // A student schedule is his class's schedule
                CanViewDifferentSchedules = false;
                Class studentClass = ConnectedUser.Student.Class;

                // Make sure the student is assigned to a class (otherwise he doesn't have a schedule
                if (studentClass != null)
                {
                    ScheduleData schedule = CreateClassSchedule(studentClass);

                    // Add the schedule to the schedules list
                    AvailableSchedules.Add(schedule);
                }
            }
            else if (ConnectedUser.isParent)
            {
                // A parent can see his children's schedules
                CanViewDifferentSchedules = true;
                foreach (Student student in ConnectedUser.ChildrenStudents)
                {
                    if (!student.Person.User.isDisabled && student.classID != null)
                    {
                        ScheduleData schedule = CreateClassSchedule(student.Class);

                        // Change schedule to use children's name
                        schedule.Name = student.Person.firstName + " " + student.Person.lastName;

                        // Add the schedule to the schedules list
                        AvailableSchedules.Add(schedule);
                    }
                }
            }
            else if (ConnectedUser.isTeacher)
            {
                // A teacher can just see his own schedule, as well as his classroom's schedule (if he/she has one)
                CanViewDifferentSchedules = true;

                // Add teacher's own schedule
                ScheduleData teacherOwnSchedule = CreateTeacherSchedule(ConnectedUser.Teacher);
                AvailableSchedules.Add(teacherOwnSchedule);

                // Add an homeroom teacher's class schedule
                if (ConnectedUser.Teacher.classID != null)
                {
                    ScheduleData homeroomTeacherClassSchedule = CreateClassSchedule(ConnectedUser.Teacher.Class);
                    AvailableSchedules.Add(homeroomTeacherClassSchedule);
                }
            }
            else if (ConnectedUser.isPrincipal || ConnectedUser.isSecretary)
            {
                // Secretaries and the principal can watch every class's, and every teacher's schedule
                CanViewDifferentSchedules = true;

                // Add every class's schedule
                SchoolEntities schoolData = new SchoolEntities();
                foreach (Class schoolClass in schoolData.Classes)
                {
                    ScheduleData schedule = CreateClassSchedule(schoolClass);
                    AvailableSchedules.Add(schedule);
                }

                // Add every teacher's schedule
                foreach (Teacher teacher in schoolData.Teachers)
                {
                    ScheduleData schedule = CreateTeacherSchedule(teacher);
                    AvailableSchedules.Add(schedule);
                }
            }

            SelectedSchedule = AvailableSchedules.First();
        }

        /// <summary>
        /// Assistant method that creates the weekly schedule information for a specific class
        /// </summary>
        /// <param name="classData">The class to build from</param>
        /// <returns>The schedule information</returns>
        private ScheduleData CreateClassSchedule(Class classData)
        {
            ScheduleData studentSchedule = new ScheduleData()
            {
                ID = classData.classID,
                Name = "כיתה " + classData.className,
                IsClassSchedule = true,
                IsPersonSchedule = false,
                ActualSchedule = new LessonData[Globals.DAY_NAMES.Length, Globals.HOUR_NAMES.Length]
            };
            // Add the class's lessons to the schedule
            classData.Lessons.ToList().ForEach(lesson => AddLessonToSchedule(lesson, ref studentSchedule));
            return studentSchedule;
        }

        /// <summary>
        /// Assistant method that creates the weekly schedule information for a specific teacher
        /// </summary>
        /// <param name="teacher">The teacher to build from</param>
        /// <returns>The schedule information</returns>
        private ScheduleData CreateTeacherSchedule(Teacher teacher)
        {
            ScheduleData teacherSchedule = new ScheduleData()
            {
                ID = teacher.teacherID,
                Name = teacher.Person.firstName + " " + teacher.Person.lastName,
                IsClassSchedule = false,
                IsPersonSchedule = true,
                ActualSchedule = new LessonData[Globals.DAY_NAMES.Length, Globals.HOUR_NAMES.Length]
            };
            // Add the class's lessons to the schedule
            teacher.Lessons.ToList().ForEach(lesson => AddLessonToSchedule(lesson, ref teacherSchedule));
            return teacherSchedule;
        }

        /// <summary>
        /// Assistant method that adds a lesson to 
        /// </summary>
        /// <param name="lesson"></param>
        /// <param name="schedule"></param>
        private void AddLessonToSchedule(Lesson lesson, ref ScheduleData schedule)
        {
            // Each lesson has upto 4 meetings in the week - add each one, but note that only the first is assured (the others are nullable)
            if (isValidLesson(lesson.firstLessonHour, lesson.firstLessonDay))
            {
                schedule.ActualSchedule[lesson.firstLessonDay, lesson.firstLessonHour] = new LessonData(lesson);
            }
            if(isValidLesson(lesson.secondLessonHour, lesson.secondLessonDay))
            {
                schedule.ActualSchedule[lesson.secondLessonDay.Value, lesson.secondLessonHour.Value] = new LessonData(lesson);
            }
            if (isValidLesson(lesson.thirdLessonHour, lesson.thirdLessonDay))
            {
                schedule.ActualSchedule[lesson.thirdLessonDay.Value, lesson.thirdLessonHour.Value] = new LessonData(lesson);
            }
            if (isValidLesson(lesson.fourthLessonHour, lesson.fourthLessonDay))
            {
                schedule.ActualSchedule[lesson.fourthLessonDay.Value, lesson.fourthLessonHour.Value] = new LessonData(lesson);
            }
        }

        /// <summary>
        /// Assistant method that checks if a lesson's day and hour values are valid
        /// </summary>
        /// <param name="lessonHour">The hour of the lesson (expected 0-Globals.HOUR_NAMES.length)</param>
        /// <param name="lessonDay">The day of the lesson (expected 0-Globals.DAY_NAMES.length)</param>
        /// <returns></returns>
        private bool isValidLesson(Nullable<int> lessonHour, Nullable<int> lessonDay)
        {
            if ((lessonHour != null && lessonDay != null) &&
                (lessonDay >= 0 && lessonDay < Globals.DAY_NAMES.Length) &&
                (lessonHour >= 0 && lessonHour < Globals.HOUR_NAMES.Length))
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
