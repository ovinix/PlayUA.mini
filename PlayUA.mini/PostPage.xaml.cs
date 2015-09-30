using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PlayUA.mini
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostPage : Page
    {
        private Post Post;

        public double CurrentFontSize
        {
            get
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("HtmlFontSize"))
                {
                    return Double.Parse(loader.GetString(ApplicationData.Current.LocalSettings.Values["HtmlFontSize"].ToString()));
                }
               
                return Double.Parse(loader.GetString("DefaultHtmlFontSize"));
            }
        }

        public bool bLoadImages
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("BoolLoadImages"))
                {
                    return Boolean.Parse(ApplicationData.Current.LocalSettings.Values["BoolLoadImages"].ToString());
                }

                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                return Boolean.Parse(loader.GetString("LoadImages"));
            }
        }

        public PostPage()
        {
            this.InitializeComponent();

            //StatusBar.GetForCurrentView().BackgroundOpacity = 1;
            //StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            //StatusBar.GetForCurrentView().BackgroundColor = ((SolidColorBrush)this.Resources["PlayUABlueBrush"]).Color;

            DataContext = this;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            this.Post = (Post)e.Parameter;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        public async void ShowPost(Post post)
        {
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

            PostTitle.Text = post.Title_Plain;

            String html = post.Content;

            String output = Regex.Replace(html, "\\<div[^\\>]*\\>", "<div>");
            output = Regex.Replace(output, "\\<iframe[^\\>]*\\>", "<iframe>");
            output = Regex.Replace(output, "\\<a[^\\>]*\\>", "<a>");
            // Repearing <strong> tag
            output = output.Replace("<strong>\u00a0", " <strong>");
            output = output.Replace("\u00a0</strong>", "</strong> ");
            output = output.Replace("<strong> ", " <strong>");
            output = output.Replace(" </strong>", "</strong> ");
            //output = output.Replace("&amp;", "");
            if (!bLoadImages)
            {
                output = Regex.Replace(output, "\\<img[^\\>]*\\>", "");
            }
            // output = Regex.Replace(output, "\\<div\\>*\\<iframe\\>*\\</button\\>\\</div\\>", "</div>");

            SomeHtmlView.Html = output;

            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();//.Navigate(typeof(MainPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowPost(this.Post);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private async void OpenInBrowser_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(this.Post.Url));
        }
    }

    //    public async void GetAString(String URL)
    //    {
    //        try
    //        {
    //            // Create the http request
    //            var httpClient = new HttpClient();
    //            httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

    //            // Get the response asynchronously
    //            String ResponseString = await httpClient.GetStringAsync(new Uri(URL));

    //            Debug.WriteLine(ResponseString);

    //            var json = JsonConvert.DeserializeObject<PostResponse>(ResponseString);
    //            var post = json.Post;

    //            PostTitle.Text = post.Title_Plain;

    //            Debug.WriteLine("Saved:" + post.Title_Plain);

    //            String html = post.Content;

    //            String output = Regex.Replace(html, "\\<div[^\\>]*\\>", "<div>");
    //            output = Regex.Replace(output, "\\<iframe[^\\>]*\\>", "<iframe>");
    //            output = Regex.Replace(output, "\\<a[^\\>]*\\>", "<a>");
    //            //output = Regex.Replace(output, "\\<img[^\\>]*\\>", "");
    //            // output = Regex.Replace(output, "\\<div\\>*\\<iframe\\>*\\</button\\>\\</div\\>", "</div>");

    //            Debug.WriteLine("Output:" + output);

    //            Debug.WriteLine("Html:" + post.Content);

    //            SomeHtmlView.Html = output;

    //        }
    //        catch (Exception e)
    //        {
    //            SomeHtmlView.Html += "<div>Exceprion 1:" + e.ToString() + "</div>";
    //        }

    //    }
    //}

    //class PostResponse
    //{
    //    public Post Post { get; set; }
    //}
}
