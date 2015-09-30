using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PlayUA.mini
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            InitSettings();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();
        }

        private void InitSettings()
        {
            // Initialize settings
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("HtmlFontSize"))
            {
                ApplicationData.Current.LocalSettings.Values.Add("HtmlFontSize", "DefaultHtmlFontSize");
            }

            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("BoolLoadImages"))
            {
                ApplicationData.Current.LocalSettings.Values.Add("BoolLoadImages", true);
            }

            //Setting controls
            var rb = this.FindName(ApplicationData.Current.LocalSettings.Values["HtmlFontSize"].ToString()) as RadioButton;
            rb.IsChecked = true;

            ImageLoad.IsOn = Boolean.Parse(ApplicationData.Current.LocalSettings.Values["BoolLoadImages"].ToString());
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("HtmlFontSize"))
            {
                var rb = sender as RadioButton;
                ApplicationData.Current.LocalSettings.Values["HtmlFontSize"] = rb.Name.ToString();
            }
        }

        private void ImageLoad_Toggled(object sender, RoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("BoolLoadImages"))
            {
                var ts = sender as ToggleSwitch;
                ApplicationData.Current.LocalSettings.Values["BoolLoadImages"] = ts.IsOn;
            }
        }
    }
}
