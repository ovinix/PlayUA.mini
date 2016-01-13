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
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace PlayUA.mini
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PostPage : Page, INotifyPropertyChanged
    {
        private const String COMMENTS_REQUEST_URL = "http://playua.net/?json=get_post&include=comments";
        private Post Post;

        public ObservableCollection<Comment> AllComments { get; set; }

        private bool bLoadImages
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

        public double BigFontSize
        {
            get { return PostFontSize + 4; }
        }

        public double PostFontSize
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

        public double SmallFontSize
        {
            get { return PostFontSize - 2; }
        }

        public PostPage()
        {
            this.InitializeComponent();

            AllComments = new ObservableCollection<Comment>();
            //PostParagraphs = new ObservableCollection<String>();

            //hwPostContent.FontSize = PostFontSize;

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

        //public ObservableCollection<String> PostParagraphs { get; set; }
        //MyToolkit.Controls.HtmlView hwPostContent = new MyToolkit.Controls.HtmlView();

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private String _PostTitle;
        public String PostTitle
        {
            get { return _PostTitle; }
            set
            {
                _PostTitle = value;
                NotifyPropertyChanged("PostTitle");
            }
        }
        private String _PostAuthor;
        public String PostAuthor
        {
            get { return _PostAuthor; }
            set
            {
                _PostAuthor = value;
                NotifyPropertyChanged("PostAuthor");
            }
        }
        private String _PostDate;
        public String PostDate
        {
            get { return _PostDate; }
            set
            {
                _PostDate = value;
                NotifyPropertyChanged("PostDate");
            }
        }

        public async void ShowPost(Post post)
        {
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "";
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

            hwPostContent.FontSize = PostFontSize;

            PostTitle = post.Title_Plain;
            PostAuthor = "Автор " + post.Author.Name;
            PostDate = post.Date;

            String html = post.Content;

            String output = Regex.Replace(html, "\\<div[^\\>]*\\>", "<div>");
            output = Regex.Replace(output, "\\<iframe[^\\>]*\\>", "<iframe>");
            output = Regex.Replace(output, "\\<button[^\\>]*\\>", "<button>");
            //output = Regex.Replace(output, "\\<a[^\\>]*\\>", "<a>");
            //Repairing links with img inside
            output = Regex.Replace(output, "\\<a[^\\>]*\\><img", "<img");
            output = Regex.Replace(output, "/></a>", "/>");
            //Removing img width and height (HtmlView improvement for poor connection)
            //output = Regex.Replace(output, "\\width=\"[0-9]*\\\"", " ");
            output = Regex.Replace(output, "height=\"[0-9]*\\\"", "");
            output = Regex.Replace(output, "width=\"[0-9]*\\\"", "");

            // <ol> tag not showing in MyToolkit
            output = output.Replace("<ol>", "<ul>");
            output = output.Replace("</ol>", "</ul>");

            // Repearing <strong> tag
            output = output.Replace("<strong>\u00a0", " <strong>");
            output = output.Replace("\u00a0</strong>", "</strong> ");
            output = output.Replace("<strong> ", " <strong>");
            output = output.Replace(" </strong>", "</strong> ");
            // Remove video 'x' button
            output = output.Replace("<button>x</button>", "");
            //output = output.Replace("&amp;", "");

            if (!bLoadImages)
            {
                output = Regex.Replace(output, "\\<img[^\\>]*\\>", "");
            }
            // output = Regex.Replace(output, "\\<div\\>*\\<iframe\\>*\\</button\\>\\</div\\>", "</div>");

            hwPostContent.Html = output;

            //foreach (var prop in hwPostContent.ScrollViewer.GetType().GetRuntimeProperties())
            //{
            //    Debug.WriteLine("{0}={1}", prop.Name, prop.GetValue(hwPostContent.ScrollViewer, null));
            //}

            ///********* LET IT BEGIN *************/

            ////List<String> groupCollection = new List<String>();

            //////System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(output, @"<(p|div)>\s*(.+?)\s*</(p|div)>");
            //////System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(output, @"<.*[^>]>\s*(.+?)\s*</.*[^>]>");
            ////System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(output, @"<(p|div|h[0-9])>.*?</(p|div||h[0-9])>");

            ////while (m.Success)
            ////{
            ////    groupCollection.Add(m.Value);
            ////    m = m.NextMatch();
            ////}
            ////ObservableCollection<String> paragraphs = new ObservableCollection<String>();
            ////if (groupCollection.Count > 0)
            ////{
            ////    foreach (object item in groupCollection)
            ////    {
            ////        paragraphs.Add(item.ToString());
            ////    }
            ////}

            ////string[] stringSeparators = new string[] { "</p>" };
            //string[] stringSeparators = new string[] { "<img" };
            //var s = output.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            //List<String> list = new List<String>(s);

            //bool first = false;
            //foreach (var pa in list)
            //{
            //    String paragraph = pa.Replace("\n", "");
            //    if (!String.IsNullOrEmpty(paragraph.Replace("<p>", "")))
            //    {
            //        //paragraph = /*"&nbsp;\n" + */paragraph + "</p>";
            //        if (first)
            //        {
            //            paragraph = "<img " + paragraph;
            //        }                    
            //        PostParagraphs.Add(paragraph);
            //    }
            //    first = true;
            //}

            ///******** END ******************/

            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }

        public async void LoadCommentsAsync(String URL)
        {
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "Завантаження";
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

            try
            {
                // Create the http request
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

                // Get the response asynchronously
                String ResponseString = await httpClient.GetStringAsync(new Uri(URL));

                var json = JsonConvert.DeserializeObject<PostResponse>(ResponseString);

                foreach (var comment in json.Post.Comments)
                {
                    if (!AllComments.Any(c => c.Id == comment.Id))
                    {
                        // Checking for parent comment
                        if (!comment.Parent.Equals("0"))
                        {
                            var parent = AllComments.Single(c => c.Id == comment.Parent);
                            int parentIndex = AllComments.IndexOf(parent);
                            if (parentIndex < AllComments.Count)
                            {
                                comment.CommentMargin = new Thickness(18, 6, 0, 6);
                                AllComments.Insert(parentIndex + 1, comment);
                                continue;
                            }
                        }
                        AllComments.Add(comment);
                    }
                }

                //Removing ListView Header
                if (AllComments.Count > 0)
                {
                    lvComments.Header = null;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            Frame.GoBack();//.Navigate(typeof(MainPage));
        }

        private void hwPostContent_Loaded(object sender, RoutedEventArgs e)
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

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    Debug.WriteLine("First pivot page is actived");
                    break;
                case 1:
                    Debug.WriteLine("Second pivot page is actived");
                    String CommentsURL = COMMENTS_REQUEST_URL + "&slug=" + this.Post.Slug;
                    LoadCommentsAsync(CommentsURL);
                    break;
            }
        }
    }

    public class PostResponse
    {
        public Post Post { get; set; }
    }   
}    