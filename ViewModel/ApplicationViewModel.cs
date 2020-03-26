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
        private ICommand _changePageCommand;

        private IScreenViewModel _currentScreenViewModel;
        private List<IScreenViewModel> _screenViewModels;
        #endregion

        #region Constructors/Destructors
        public ApplicationViewModel(User connectedUser)
        {
            // Add available pages
            ScreensViewModels.Add(new SchoolInfoViewModel());
            ScreensViewModels.Add(new UserViewModel());

            // Set starting page
            CurrentScreenViewModel = ScreensViewModels[0];
        }
        #endregion

        #region Properties / Commands
        public ICommand ChangeScreenCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IScreenViewModel)p),
                        p => p is IScreenViewModel);
                }

                return _changePageCommand;
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

        #endregion
    }
}
