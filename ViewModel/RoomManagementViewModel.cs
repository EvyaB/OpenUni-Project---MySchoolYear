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
    public class RoomManagementViewModel : BaseViewModel, IScreenViewModel
    {
        #region SubStructs
        public class RoomData
        {
            public int ID { get; set; }
            public string Name { get; set; }
            
            public string HomeroomClassName { get; set; }
            public Nullable<int> HomeroomClassID { get; set; }
            
            public List<int> LessonsIDs { get; set; }
        }
        #endregion

        #region Fields
        private ICommand _refreshDataCommand;

        private RoomData _selectedRow;
        private string _selectedRoomName;
        #endregion

        #region Properties / Commands
        // Base Properties
        public Person ConnectedUser { get; }
        public bool HasRequiredPermissions { get; }
        public string ScreenName { get { return "ניהול חדרים"; } }

        // Buisness Logic Properties
        public ObservableCollection<RoomData> TableData { get; set; }
        public RoomData SelectedRow 
        { 
            get
            {
                return _selectedRow;
            }
            set
            {
                if (_selectedRow != value)
                {
                    _selectedRow = value;
                    UseSelectedRoom(_selectedRow);
                    OnPropertyChanged("SelectedRow");
                }
            }
        }
        public string RoomName 
        { 
            get
            {
                return _selectedRoomName;
            }
            set
            {
                if (_selectedRoomName != value)
                {
                    _selectedRoomName = value;
                    OnPropertyChanged("RoomName");
                }
            }
        }

        #endregion

        #region Constructors
        public RoomManagementViewModel(Person connectedUser, ICommand refreshDataCommand)
        {
            ConnectedUser = connectedUser;
            HasRequiredPermissions = connectedUser.isSecretary || connectedUser.isPrincipal;
            _refreshDataCommand = refreshDataCommand;
            TableData = new ObservableCollection<RoomData>();
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            // TODO
            TableData.Add(new RoomData() { Name = "חדר", ID = 1 });
            TableData.Add(new RoomData() { Name = "שלי", ID = 3 });
        }

        /// <summary>
        /// Choose a specific room and prepare to edit it.
        /// </summary>
        /// <param name="selectedRoom">The room's data</param>
        private void UseSelectedRoom(RoomData selectedRoom)
        {
            RoomName = selectedRoom.Name;
        }
        #endregion


    }
}
