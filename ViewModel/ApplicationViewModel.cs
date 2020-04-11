using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MySchoolYear.Model;
using MySchoolYear.View;
using MySchoolYear.View.Utilities;
using MySchoolYear.ViewModel.Utilities;

namespace MySchoolYear.ViewModel
{
    /// <summary>
    /// The application main page's view model. 
    /// Responsible for navigating between the different screens of the application once the user logged-in
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Fields
        private ICommand _changeScreenCommand;
        private ICommand _updateScreensCommand;
        private ICommand _logoutCommand;

        private IMessageBoxService _messageBoxService;

        private IScreenViewModel _currentScreenViewModel;
        private List<IScreenViewModel> _screenViewModels;
        #endregion

        #region Properties / Commands
        public ICommand ChangeScreenCommand
        {
            get
            {
                if (_changeScreenCommand == null)
                {
                    _changeScreenCommand = new RelayCommand(
                        p => ChangeViewModel((IScreenViewModel)p),
                        p => p is IScreenViewModel);
                }

                return _changeScreenCommand;
            }
        }

        public ICommand UpdateScreensCommand
        {
            get
            {
                if (_updateScreensCommand == null)
                {
                    _updateScreensCommand = new RelayCommand(p => RefreshViewModels());
                }

                return _updateScreensCommand;
            }
        }

        public ICommand LogoutCommand
        { 
            get
            {
                if (_logoutCommand == null)
                {
                    _logoutCommand = new RelayCommand(
                        p => LogoutUser(p as IClosableScreen),
                        p => p is IClosableScreen);
                }

                return _logoutCommand;
            }
        }

        public List<IScreenViewModel> ScreensViewModels
        {
            get
            {
                if (_screenViewModels == null)
                    _screenViewModels = new List<IScreenViewModel>();

                return _screenViewModels;
            }
        }

        public IScreenViewModel CurrentScreenViewModel
        {
            get
            {
                return _currentScreenViewModel;
            }
            set
            {
                if (_currentScreenViewModel != value)
                {
                    _currentScreenViewModel = value;
                    OnPropertyChanged("CurrentScreenViewModel");
                }
            }
        }
        #endregion

        #region Constructors
        public ApplicationViewModel(Person connectedUser, IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;

            // Create a list of all possible screens
            List<IScreenViewModel> allScreens = new List<IScreenViewModel>();
            allScreens.Add(new SchoolInfoViewModel(connectedUser));
            allScreens.Add(new StudentGradesViewModel(connectedUser));
            allScreens.Add(new WeeklyScheduleViewModel(connectedUser));
            allScreens.Add(new CalenderViewModel(connectedUser));
            allScreens.Add(new MessagesDisplayViewModel(connectedUser));

            allScreens.Add(new CreateMessageViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new EventManagementViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new ClassManagementViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new RoomManagementViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new CourseManagementViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new LessonManagementViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new UserCreationViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new UserUpdateViewModel(connectedUser, UpdateScreensCommand, messageBoxService));

            allScreens.Add(new SchoolManagementViewModel(connectedUser, UpdateScreensCommand));

            // Use only the screens that are relevent to the current user
            foreach (IScreenViewModel screen in allScreens)
            {
                if (screen.HasRequiredPermissions)
                {
                    screen.Initialize();
                    ScreensViewModels.Add(screen);
                }
            }

            // Set starting page
            CurrentScreenViewModel = ScreensViewModels[0];
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the current view model to 'viewModel'
        /// </summary>
        /// <param name="viewModel">The new ViewModel</param>
        private void ChangeViewModel(IScreenViewModel viewModel)
        {
            if (!ScreensViewModels.Contains(viewModel))
                ScreensViewModels.Add(viewModel);

            CurrentScreenViewModel = ScreensViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        /// <summary>
        /// Refresh the view models following an update to the data
        /// </summary>
        private void RefreshViewModels()
        {
            foreach (IScreenViewModel viewModel in ScreensViewModels)
            {
                viewModel.Initialize();
            }
        }

        /// <summary>
        /// Closes the application and returns to the login menu
        /// </summary>
        /// <param name="thisApplicationScreen">ICloseableScreen object to close the application</param>
        private void LogoutUser(IClosableScreen thisApplicationScreen)
        {
            // Launch the login window
            LoginWindow appLoginWindow = new LoginWindow();
            LoginViewModel context = new LoginViewModel(_messageBoxService);
            appLoginWindow.DataContext = context;
            appLoginWindow.Show();

            // Close this Window 
            thisApplicationScreen.CloseScreen();
        }
        #endregion
    }
}
