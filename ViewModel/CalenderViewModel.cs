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
    /// Shows the user a calender with his upcoming events
    /// </summary>
    public class CalenderViewModel : BaseViewModel, IScreenViewModel
    {
        #region SubStructs
        #endregion

        #region Fields
        private SchoolEntities _schoolData;

        private ICommand _updateCalenderCommand;
        private ICommand _updateSelectedDayCommand;
        private List<string> _months;
        private string _selectedMonth;
        #endregion

        #region Properties / Commands
        // Base Properties
        public Person ConnectedUser { get; }
        public bool HasRequiredPermissions { get; }
        public string ScreenName { get { return "לוח שנה"; } }

        // Business Logic Properties
        public List<string> Months
        {
            get
            {
                if (_months == null)
                {
                    _months = new List<string>();
                }

                return _months;
            }
        }

        public string SelectedMonth
        {
            get
            {
                return _selectedMonth;
            }
            set
            {
                if (_selectedMonth != value && Months.Contains(value))
                {
                    _selectedMonth = value;
                    OnPropertyChanged("SelectedMonth");
                }
            }
        }

        /// <summary>
        /// Create a new school class with the current data
        /// </summary>
        public ICommand UpdateCalenderCommand
        {
            get
            {
                if (_updateCalenderCommand == null)
                {
                    _updateCalenderCommand = new RelayCommand(p => UpdateCalender((Jarloo.Calendar.Calendar)p),
                                                              p => p is Jarloo.Calendar.Calendar);
                }
                return _updateCalenderCommand;
            }
        }
        /// <summary>
        /// Create a new school class with the current data
        /// </summary>
        public ICommand UpdateSelectedDayCommand
        {
            get
            {
                if (_updateSelectedDayCommand == null)
                {
                    _updateSelectedDayCommand = new RelayCommand(p => UpdateCalender2());
                }
                return _updateSelectedDayCommand;
            }
        }

        #endregion

        #region Constructors
        public CalenderViewModel(Person connectedUser)
        {
            ConnectedUser = connectedUser;
            HasRequiredPermissions = true;

            if (HasRequiredPermissions)
            {
                _schoolData = new SchoolEntities();
                _months = new List<string> 
                    { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            // Use current month as the default. (Note that DateTime.Now.Month indexes from 1 to 12 and not 0-11)
            SelectedMonth = _months[DateTime.Now.Month - 1];
        }

        /// <summary>
        /// Update the calender to show data for the selected month
        /// </summary>
        /// <param name="calender">The visual calender</param>
        private void UpdateCalender(Jarloo.Calendar.Calendar calender)
        {
            DateTime targetDate = new DateTime(DateTime.Now.Year, Months.IndexOf(SelectedMonth) + 1, 1);

            calender.BuildCalendar(targetDate);
        }

        /// <summary>
        /// Update the calender to show data for the selected month
        /// </summary>
        /// <param name="calender">The visual calender</param>
        private void UpdateCalender2()
        {
            //
        }
        #endregion
    }
}
