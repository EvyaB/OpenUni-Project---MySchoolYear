using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MySchoolYear.Model;
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
            // Create a list of all possible screens
            List<IScreenViewModel> allScreens = new List<IScreenViewModel>();
            allScreens.Add(new SchoolInfoViewModel(connectedUser));
            allScreens.Add(new StudentGradesViewModel(connectedUser));
            allScreens.Add(new SchoolManagementViewModel(connectedUser, UpdateScreensCommand));
            allScreens.Add(new UserCreationViewModel(connectedUser, UpdateScreensCommand, messageBoxService));
            allScreens.Add(new UserUpdateViewModel(connectedUser, UpdateScreensCommand, messageBoxService));

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
        #endregion
    }
}
