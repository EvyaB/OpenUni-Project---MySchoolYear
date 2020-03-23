using System;
using System.ComponentModel;
using System.Windows;

namespace MySchoolYear.View.Utilities
{
    /// <summary>
    /// ViewModelLocator automates the process of getting a ViewModel hooked up to a View DURING RUNTIME.
    /// It requires attaching a property to the view to request this hookup.
    /// </summary>
    public class ViewModelLocator
    {
        public static bool GetAutoHookedUpViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoHookedUpViewModelProperty);
        }

        public static void SetAutoHookedUpViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoHookedUpViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoHookedUpViewModel. 
        //This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoHookedUpViewModelProperty =
           DependencyProperty.RegisterAttached("AutoHookedUpViewModel",
           typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false,
           AutoHookedUpViewModelChanged));

        /// <summary>
        /// Update the View with its ViewModel
        /// </summary>
        /// <param name="d">The View</param>
        /// <param name="e">Event arguements for this hookup event</param>
        private static void AutoHookedUpViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // The ViewModelLocator requires Reflection and runtime functionality - it cannot attach the ViewModel when we work through the designer. 
            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            // Get the full pathname of the view.
            var viewType = d.GetType();
            string str = viewType.FullName;

            // Standarize all possible views 
            str = str.Replace("Page", "View");
            str = str.Replace("Window", "View");

            // Switch folder from Views folder to ViewModel folder
            str = str.Replace(".View.", ".ViewModel.");
            
            // Add 'Model' to the end of the filename and search for the corresponding viewmodel
            var viewTypeName = str;
            var viewModelTypeName = viewTypeName + "Model";
            var viewModelType = Type.GetType(viewModelTypeName);

            // Check if any VM was found, and if so set it as the data context.
            if (viewModelType != null)
            {
                var viewModel = Activator.CreateInstance(viewModelType);
                ((FrameworkElement)d).DataContext = viewModel;
            }
        }
    }
}
