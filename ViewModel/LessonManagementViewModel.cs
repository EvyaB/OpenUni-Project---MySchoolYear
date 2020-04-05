using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MySchoolYear.Model;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// Manages the school's Rooms - creating, editing and deleting rooms
    /// </summary>
    public class LessonManagementViewModel : BaseViewModel, IScreenViewModel
    {
        #region SubStructs
        public class LessonData
        {
            public int ID { get; set; }
            public string ClassName { get; set; }
            public string CourseName { get; set; }
            public string TeacherName { get; set; }
            public int ClassID { get; set; }
            public int CourseID { get; set; }
            public int TeacherID { get; set; }

            public List<LessonTimes> LessonMeetingTimes { get; set; }
        }

        public class LessonTimes
        {
            public string DayFirstLesson { get; set; }
            public string HourFirstLesson { get; set; }
            public string DaySecondLesson { get; set; }
            public string HourSecondLesson { get; set; }
            public string DayThirdLesson { get; set; }
            public string HourThirdLesson { get; set; }
            public string DayFourthLesson { get; set; }
            public string HourFourthLesson { get; set; }
        }
        #endregion

        #region Fields
        private ICommand _refreshDataCommand;
        private ICommand _deleteLessonCommand;
        private ICommand _updateLessonCommand;
        private ICommand _createLessonCommand;

        private SchoolEntities _schoolData;
        private IMessageBoxService _messageBoxService;

        private LessonData _selectedLesson;
        private bool _searchingByClass;
        private bool _searchingByCourse;
        private bool _searchingByTeacher;
        private int _selectedClassID;
        private int _selectedCourseID;
        private int _selectedTeacherID;
        private int _selectedSearchChoiceID;

        public ObservableCollection<LessonData> _lessonsTableData;
        private ObservableDictionary<int, string> _availableClasses;
        private ObservableDictionary<int, string> _availableCourses;
        private ObservableDictionary<int, string> _availableTeachers;
        private ObservableDictionary<int, string> _availableSearchChoices;

        private const int NO_ASSIGNED = -1;
        #endregion

        #region Properties / Commands
        // Base Properties
        public Person ConnectedUser { get; }
        public bool HasRequiredPermissions { get; }
        public string ScreenName { get { return "ניהול שיעורים"; } }

        // Business Logic Properties
        public ObservableCollection<LessonData> LessonsTableData 
        { 
            get
            {
                return _lessonsTableData;
            }
            set
            {
                if (_lessonsTableData != value)
                {
                    _lessonsTableData = value;
                    OnPropertyChanged("LessonsTableData");
                }
            }
        }
        public LessonData SelectedLesson 
        { 
            get
            {
                return _selectedLesson;
            }
            set
            {
                if (_selectedLesson != value)
                {
                    _selectedLesson = value;
                    UseSelectedLesson(_selectedLesson);
                    OnPropertyChanged("SelectedLesson");
                }
            }
        }

        public bool SearchingByClass
        { 
            get
            {
                return _searchingByClass;
            }
            set
            {
                if (_searchingByClass != value)
                {
                    _searchingByClass = value;
                    OnPropertyChanged("SearchingByClass");
                }
            }
        }
        public bool SearchingByCourse
        {
            get
            {
                return _searchingByCourse;
            }
            set
            {
                if (_searchingByCourse != value)
                {
                    _searchingByCourse = value;
                    OnPropertyChanged("SearchingByCourse");
                }
            }
        }
        public bool SearchingByTeacher
        {
            get
            {
                return _searchingByTeacher;
            }
            set
            {
                if (_searchingByTeacher != value)
                {
                    _searchingByTeacher = value;
                    OnPropertyChanged("SearchingByTeacher");
                }
            }
        }

        public ObservableDictionary<int, string> AvailableClasses
        {
            get
            {
                return _availableClasses;
            }
            set
            {
                if (_availableClasses != value)
                {
                    _availableClasses = value;
                    OnPropertyChanged("AvailableClasses");
                }
            }
        }
        public int SelectedClass
        {
            get
            {
                return _selectedClassID;
            }
            set
            {
                if (_selectedClassID != value)
                {
                    _selectedClassID = value;
                    OnPropertyChanged("SelectedClass");
                }
            }
        }

        public ObservableDictionary<int, string> AvailableCourses
        {
            get
            {
                return _availableCourses;
            }
            set
            {
                if (_availableCourses != value)
                {
                    _availableCourses = value;
                    OnPropertyChanged("AvailableCourses");
                }
            }
        }
        public int SelectedCourse
        {
            get
            {
                return _selectedCourseID;
            }
            set
            {
                if (_selectedCourseID != value)
                {
                    _selectedCourseID = value;
                    OnPropertyChanged("SelectedCourse");
                }
            }
        }

        public ObservableDictionary<int, string> AvailableTeachers
        {
            get
            {
                return _availableTeachers;
            }
            set
            {
                if (_availableTeachers != value)
                {
                    _availableTeachers = value;
                    OnPropertyChanged("AvailableTeachers");
                }
            }
        }
        public int SelectedTeacher
        {
            get
            {
                return _selectedTeacherID;
            }
            set
            {
                if (_selectedTeacherID != value)
                {
                    _selectedTeacherID = value;
                    OnPropertyChanged("SelectedTeacher");
                }
            }
        }

        public ObservableDictionary<int, string> AvailableSearchChoices
        {
            get
            {
                return _availableSearchChoices;
            }
            set
            {
                if (_availableSearchChoices != value)
                {
                    _availableSearchChoices = value;
                    OnPropertyChanged("AvailableSearchChoices");
                }
            }
        }
        public int SelectedSearchChoice
        {
            get
            {
                return _selectedSearchChoiceID;
            }
            set
            {
                if (_selectedSearchChoiceID != value)
                {
                    _selectedSearchChoiceID = value;
                    OnPropertyChanged("SelectedSearchChoice");
                }
            }
        }

        /// <summary>
        /// Delete the currently selected lesson
        /// </summary>
        public ICommand DeleteLessonCommand
        {
            get
            {
                if (_deleteLessonCommand == null)
                {
                    _deleteLessonCommand = new RelayCommand(p => DeleteSelectedLesson());
                }
                return _deleteLessonCommand;
            }
        }

        /// <summary>
        /// Update the currently selected lesson
        /// </summary>
        public ICommand UpdateLessonCommand
        {
            get
            {
                if (_updateLessonCommand == null)
                {
                    _updateLessonCommand = new RelayCommand(p => UpdateSelectedLesson());
                }
                return _updateLessonCommand;
            }
        }

        /// <summary>
        /// Create a new lesson with the current data
        /// </summary>
        public ICommand CreateNewLessonCommand
        {
            get
            {
                if (_createLessonCommand == null)
                {
                    _createLessonCommand = new RelayCommand(p => CreateNewLesson());
                }
                return _createLessonCommand;
            }
        }

        #endregion

        #region Constructors
        public LessonManagementViewModel(Person connectedUser, ICommand refreshDataCommand, IMessageBoxService messageBoxService)
        {
            ConnectedUser = connectedUser;
            HasRequiredPermissions = connectedUser.isSecretary || connectedUser.isPrincipal;
            _refreshDataCommand = refreshDataCommand;
            _messageBoxService = messageBoxService;

            if (HasRequiredPermissions)
            {
                _schoolData = new SchoolEntities();
                AvailableClasses = new ObservableDictionary<int, string>();
            }

        }
        #endregion

        #region Methods
        public void Initialize()
        {
            // Get the list of existing rooms
            LessonsTableData = new ObservableCollection<LessonData>(_schoolData.Lessons.AsEnumerable().Select(lesson => ModelLessonToLessonData(lesson)).ToList());

            //// Create the basic list of available classes
            //AvailableClasses.Clear();

            //// Add a 'No class' option, as not all rooms are assigned to a specific class
            //AvailableClasses.Add(NO_ASSIGNED, "אין כיתה משויכת");

            //// Create the list of classes that don't have an homeroom already
            //_schoolData.Classes.Where(schoolClass => schoolClass.roomID == null).ToList()
            //    .ForEach(schoolClass => AvailableClasses.Add(schoolClass.classID, schoolClass.className));

            //SelectedClass = NO_ASSIGNED;

            //// For some reason, after re-initializing this view, the SelectedClass is not updated properly in the view unless called again
            //OnPropertyChanged("SelectedClass"); 
        }

        /// <summary>
        /// Converts the Model's lesson class into the local LessonData class
        /// </summary>
        /// <param name="lesson">The Model's lesson</param>
        /// <returns>Corresponding LessonData version of the lesson</returns>
        private LessonData ModelLessonToLessonData(Lesson lesson)
        {
            LessonData lessonData = new LessonData();
            lessonData.ID = lesson.lessonID;
            //lessonData.ClassName = lesson.roomName;

            //// Check if the room has an homeroom class
            //if (lesson.Classes != null && lesson.Classes.Count > 0)
            //{
            //    lessonData.HomeroomClassID = lesson.Classes.Single().classID;
            //    lessonData.TeacherName = lesson.Classes.Single().className;
            //}

            //// Check if the room has lessons associated with it
            //if (lesson.Lessons != null && lesson.Lessons.Count > 0)
            //{
            //    lessonData.LessonMeetingTimes = lesson.Lessons.Select(lesson => new LessonTimes()
            //        {
            //            LessonID = lesson.lessonID,
            //            ClassID = lesson.classID,
            //            ClassName = lesson.Class.className,
            //            CourseName = lesson.Course.courseName,
            //            DayFirstLesson = Globals.ConvertDayNumberToName(lesson.firstLessonDay),
            //            DaySecondLesson = Globals.ConvertDayNumberToName(lesson.secondLessonDay),
            //            DayThirdLesson = Globals.ConvertDayNumberToName(lesson.thirdLessonDay),
            //            DayFourthLesson = Globals.ConvertDayNumberToName(lesson.fourthLessonDay),
            //            HourFirstLesson = Globals.ConvertHourNumberToName(lesson.firstLessonHour),
            //            HourSecondLesson = Globals.ConvertHourNumberToName(lesson.secondLessonHour),
            //            HourThirdLesson = Globals.ConvertHourNumberToName(lesson.thirdLessonHour),
            //            HourFourthLesson = Globals.ConvertHourNumberToName(lesson.fourthLessonHour)
            //        }).ToList();
            //}

            return lessonData;
        }

        /// <summary>
        /// Choose a specific lesson and view its information.
        /// </summary>
        /// <param name="selectedLesson">The lesson's data</param>
        private void UseSelectedLesson(LessonData selectedLesson)
        {
            //// Cleanup previous selections
            //SelectedClass = NO_ASSIGNED;
            //RoomName = string.Empty;
            //LessonsInSelectedRoom = new ObservableCollection<LessonTimes>();

            //// Remove the previous room choice's class from the available classes list (as it is already assigned to that room)
            //if (_previousLessonClass != null)
            //{
            //    AvailableClasses.Remove(_previousLessonClass.Value);
            //}

            //// Update the properties per the selected room
            //if (selectedLesson != null)
            //{
            //    RoomName = selectedLesson.ClassName;

            //    // If the room has an homeroom, add it first to the available classes list
            //    if (selectedLesson.HomeroomClassID != null)
            //    {
            //        AvailableClasses.Add(selectedLesson.HomeroomClassID.Value, selectedLesson.TeacherName);
            //        SelectedClass = selectedLesson.HomeroomClassID.Value;
            //    }

            //    // Save this room class ID so it can be removed from the available classes when we select another room
            //    _previousLessonClass = selectedLesson.HomeroomClassID;

            //    // Create the list of lessons in the current room
            //    if (selectedLesson.LessonMeetingTimes != null)
            //    {
            //        LessonsInSelectedRoom = new ObservableCollection<LessonTimes>(selectedLesson.LessonMeetingTimes);
            //    }
            //}
        }

        /// <summary>
        /// Delete the currently selected lesson
        /// </summary>
        private void DeleteSelectedLesson()
        {
            //// Check that a room was selected
            //if (SelectedRoom == null)
            //{
            //    _messageBoxService.ShowMessage("אנא בחר חדר קודם כל.",
            //                                   "נכשל במחיקת חדר", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
            //}
            //else
            //{
            //    // Get the room that is going to be edited and the related class (if any)
            //    Room selectedRoom = _schoolData.Rooms.Find(SelectedRoom.ID);
                

            //    // As this is a serious action, request a confirmation from the user
            //    bool confirmation = _messageBoxService.ShowMessage("האם אתה בטוח שברצונך למחוק את חדר " + selectedRoom.roomName + "?",
            //                                                        "מחיקת חדר!", MessageType.ACCEPT_CANCEL_MESSAGE, MessagePurpose.INFORMATION);
            //    if (confirmation == true)
            //    {
            //        // Remove the room from any associated class
            //        Class previousClass = _schoolData.Classes.Where(schoolClass => schoolClass.roomID == selectedRoom.roomID).FirstOrDefault();
            //        if (previousClass != null)
            //        {
            //            previousClass.roomID = null;
            //        }
                    
            //        // Clear the previous room property (as it was removed anyway)
            //        this._previousLessonClass = null;

            //        // Remove the room from any associated lessons
            //        _schoolData.Lessons.Where(lesson => lesson.roomID == selectedRoom.roomID).ToList().ForEach(lesson => lesson.roomID = null);

            //        // Delete the room itself
            //        _schoolData.Rooms.Remove(selectedRoom);

            //        // Save and report changes
            //        _schoolData.SaveChanges();
            //        _messageBoxService.ShowMessage("החדר " + selectedRoom.roomName + " נמחק בהצלחה!",
            //                "מחיקת חדר!", MessageType.OK_MESSAGE, MessagePurpose.INFORMATION);
            //        _refreshDataCommand.Execute(null);
            //    }
            //}
        }

        /// <summary>
        /// Update the currently selected lesson
        /// </summary>
        private void UpdateSelectedLesson()
        {
            //// Check that a room was selected
            //if (SelectedRoom == null)
            //{
            //    _messageBoxService.ShowMessage("אנא בחר חדר קודם כל.",
            //                                   "נכשל בעדכון חדר", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
            //}
            //else
            //{
            //    // Get the room that is going to be edited and the related class (if any)
            //    Room selectedRoom = _schoolData.Rooms.Find(SelectedRoom.ID);
            //    Class previousClass = _schoolData.Classes.Where(schoolClass=>schoolClass.roomID == selectedRoom.roomID).FirstOrDefault();
            //    Class selectedClass = _schoolData.Classes.Find(SelectedClass);

            //    // Check that the room's new name is unique
            //    if (_schoolData.Rooms.Where(room => room.roomID != selectedRoom.roomID && room.roomName == RoomName).Any())
            //    {
            //        _messageBoxService.ShowMessage("שם החדר תפוס כבר! אנא תן שם חדש", "נכשל בעדכון חדר", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
            //    }
            //    // Check that the room selected class doesn't have another room already
            //    else if (SelectedClass != NO_ASSIGNED && selectedClass.roomID != null && selectedClass.roomID != selectedRoom.roomID)
            //    {
            //        _messageBoxService.ShowMessage("לכיתה שנבחרה יש כבר חדר אם. בטל את הבחירה או עדכן את הכיתה קודם.",
            //                                       "נכשל בעדכון חדר", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
            //    }
            //    else
            //    {
            //        // Update the room's data
            //        selectedRoom.roomName = RoomName;

            //        // Remove the room from its previous class (assuming it has changed)
            //        if (previousClass != null && previousClass.roomID != SelectedClass)
            //        {
            //            previousClass.roomID = null;

            //            // Clear the previous room property (as it was removed anyway)
            //            this._previousLessonClass = null;
            //        }

            //        // Add the room to the selected class
            //        if (SelectedClass != NO_ASSIGNED)
            //        {
            //            // Update class to use this room's ID
            //            selectedClass.roomID = selectedRoom.roomID; 
            //        }

            //        // Update the model
            //        _schoolData.SaveChanges();

            //        // Report action success
            //        _messageBoxService.ShowMessage("החדר " + RoomName + " עודכן בהצלחה!", "עודכן חדר", MessageType.OK_MESSAGE, MessagePurpose.INFORMATION);

            //        // Update data in all screens
            //        _refreshDataCommand.Execute(null);
            //    }
            //}
        }
        
        /// <summary>
        /// Create a new lesson with current data
        /// </summary>
        private void CreateNewLesson()
        {
            //// Check that the room's name is unique
            //if (_schoolData.Rooms.Where(room => room.roomName == RoomName).Any())
            //{
            //    _messageBoxService.ShowMessage("שם החדר תפוס כבר! אנא תן שם חדש", "נכשל ביצירת חדר", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
            //}
            //// Check that the room selected class doesn't have another room already
            //else if (SelectedClass != NO_ASSIGNED && _schoolData.Classes.Find(SelectedClass).roomID != null)
            //{
            //    _messageBoxService.ShowMessage("לכיתה שנבחרה יש כבר חדר אם. בטל את הבחירה או עדכן את הכיתה קודם.",
            //                                   "נכשל ביצירת חדר", MessageType.OK_MESSAGE, MessagePurpose.ERROR);
            //}
            //else
            //{
            //    // Create a new room
            //    Room newRoom = new Room() { roomName = RoomName };
            //    _schoolData.Rooms.Add(newRoom);
            //    _schoolData.SaveChanges();

            //    // If a class was selected, update it too
            //    if (SelectedClass != NO_ASSIGNED)
            //    {
            //        _schoolData.Classes.Find(SelectedClass).roomID = newRoom.roomID;
            //        _schoolData.SaveChanges();
            //    }

            //    // Report action success
            //    _messageBoxService.ShowMessage("החדר " + RoomName + " נוצר בהצלחה!", "נוצר חדר", MessageType.OK_MESSAGE, MessagePurpose.INFORMATION);

            //    // Update data in all screens
            //    _refreshDataCommand.Execute(null);
            //}
        }

        #endregion
    }
}
