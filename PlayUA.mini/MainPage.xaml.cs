using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Html;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PlayUA.mini
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const String SITE_URL = "http://playua.net/?json=get_recent_posts&count=12&page=1&thumbnail_size=n-fulls";
        private const String REQUEST_URL = "http://playua.net/?json=get_recent_posts&thumbnail_size=n-fulls";
        // private const String SITE_URL = "http://hangglidemiami.com/?json=get_recent_posts";

        private const int ITEMS_TO_LOAD = 6;

        public ObservableCollection<Post> Items { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            StatusBar.GetForCurrentView().BackgroundOpacity = 1;
            StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
            StatusBar.GetForCurrentView().BackgroundColor = ((SolidColorBrush)this.Resources["PlayUABlueBrush"]).Color;

            Items = new ObservableCollection<Post>();

            DataContext = this;

            //GetAString();
            LoadMoreAsync(REQUEST_URL);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.

            //SomeText.Text = "PlayUA, я вже тут =)";

            //GetAString();

            //string json = @"{
            //  'Name': 'Bad Boys',
            //  'ReleaseDate': '1995-4-7T00:00:00',
            //  'Genres': [
            //    'Action',
            //    'Comedy'
            //  ]
            //}";

            //var m = JsonConvert.DeserializeObject<Test>(json);

            //SomeText.Text += ("\n" + m.Name);
        }

        public async void LoadMoreAsync(String RequestURL)
        {
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "Завантаження...";
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

            try
            {
                // Create the http request
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

                // Get the response asynchronously
                String ResponseString = await httpClient.GetStringAsync(new Uri(RequestURL));

                var json = JsonConvert.DeserializeObject<PostsResponse>(ResponseString);

                foreach (var post in json.Posts)
                {
                    if (!Items.Any(p => p.Url == post.Url))
                    {
                        Items.Add(post);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            Debug.WriteLine("LoadMoreAsync()");
            isLoadMore = true;
        }

        public async void LoadUpdatesAsync(String RequestURL)
        {
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "Оновлення...";
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

            try
            {
                // Create the http request
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

                // Get the response asynchronously
                String ResponseString = await httpClient.GetStringAsync(new Uri(RequestURL));

                var json = JsonConvert.DeserializeObject<PostsResponse>(ResponseString);
                json.Posts.Reverse();

                foreach (var post in json.Posts)
                {
                    if (!Items.Any(p => p.Url == post.Url))
                    {
                        Items.Insert(0, post);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }

        public async void GetAString()
        {
            try
            {
                // Create the http request
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

                // Get the response asynchronously
                String ResponseString = await httpClient.GetStringAsync(new Uri(SITE_URL));

                //SomeText.Text += ("\n" + ResponseString);
                
                Debug.WriteLine(ResponseString);

                var json = JsonConvert.DeserializeObject<PostsResponse>(ResponseString);

                Items.Clear();

                foreach (var post in json.Posts)
                {
                    Debug.WriteLine("Saved:" + post.Title_Plain);

                    //SomeText.Text += ("\n\n" + post.Title_Plain);
                    //var paragraph = new Paragraph();
                    //paragraph.Inlines.Add(new Run { Text = post.Title_Plain });
                    //SomeText.Blocks.Add(paragraph);

                    //SomeHtmlView.Html += ("<div>" + post.Title_Plain + "</div>");

                    if (!Items.Contains(post))
                    {
                        Items.Add(post);
                    }

                }

                Debug.WriteLine("Items:" + PostsList.Items.Count);

            }
            catch (Exception e)
            {
                //SomeText.Text += ("\n" + "Exceprion 1:" + e.ToString());

                //var paragraph = new Paragraph();
                //paragraph.Inlines.Add(new Run { Text = "\n" + "Exceprion 1:" + e.ToString() });
                //SomeText.Blocks.Add(paragraph);

                //SomeHtmlView.Html += "<div>Exceprion 1:" + e.ToString() + "</div>";

                Debug.WriteLine(e.ToString());
            }

        }

        public async void GetAString(String URL)
        {
            try
            {
                // Create the http request
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

                // Get the response asynchronously
                String ResponseString = await httpClient.GetStringAsync(new Uri(URL));

                //SomeText.Text = ("");
                //SomeText.Blocks.Clear();

                Debug.WriteLine(ResponseString);

                var json = JsonConvert.DeserializeObject<PostsResponse>(ResponseString);

                foreach (var post in json.Posts)
                {
                    //Debug.WriteLine("Saved:" + post.Title_Plain);

                    ////SomeText.Text += ("\n\n" + post.Title_Plain
                    ////    + "\n" + HtmlUtilities.ConvertToText(post.Content));

                    ////var paragraph = new Paragraph();
                    ////paragraph.Inlines.Add(new Run { Text = post.Title_Plain, FontWeight = FontWeights.Bold });
                    ////paragraph.Inlines.Add(new Run { Text = "\n\n" });
                    //////SomeText.Blocks.Add(paragraph);
                    //////paragraph = new Paragraph();
                    ////paragraph.Inlines.Add(new Run { Text = HtmlUtilities.ConvertToText(post.Content) });
                    ////SomeText.Blocks.Add(paragraph);

                    //String html = post.Content;

                    //String output = Regex.Replace(html, "\\<div[^\\>]*\\>", "<div>");
                    //output = Regex.Replace(output, "\\<iframe[^\\>]*\\>", "<iframe>");
                    //output = Regex.Replace(output, "\\<a[^\\>]*\\>", "<a>");
                    ////output = Regex.Replace(output, "\\<img[^\\>]*\\>", "");
                    //// output = Regex.Replace(output, "\\<div\\>*\\<iframe\\>*\\</button\\>\\</div\\>", "</div>");

                    //Debug.WriteLine("output:" + output);

                    ////PlayUA.mini.Common.Properties.SetHtml(SomeText, output);

                    //Debug.WriteLine("Html:" + post.Content);

                    ////HtmlView.NavigateToString(post.Content);


                    //SomeHtmlView.Html = output;

                    if (!Items.Contains(post))
                    {
                        Items.Add(post);
                    }                    
                }
            }
            catch (Exception e)
            {
                // Debug.WriteLine(e.ToString());
                //SomeText.Text += ("\n" + "Exceprion 1:" + e.ToString());

                //var paragraph = new Paragraph();
                //paragraph.Inlines.Add(new Run { Text = "\n" + "Exceprion 1:" + e.ToString() });
                //SomeText.Blocks.Add(paragraph);

                //SomeHtmlView.Html += "<div>Exceprion 1:" + e.ToString() + "</div>";
            }

            isLoadMore = true;
            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();

        }

        private void PostImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var Item = (sender as Image).DataContext;
            Frame.Navigate(typeof(PostPage), (Post)Item);
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var Item = (sender as TextBlock).DataContext;
            Frame.Navigate(typeof(PostPage),(Post)Item);
        }

        private void RemoteImage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var Item = sender as Image;
            ((Image)Item).Visibility = Visibility.Visible;
        }

#region LoadMore
        private bool isLoadMore = true;

        // Method to pull out a ScrollViewer
        public static ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) return depObj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer viewer = GetScrollViewer(this.PostsList);
            viewer.ViewChanged += ListView_ViewChanged;
        }


        private async void ListView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var view =sender as ScrollViewer;
            double progress = view.VerticalOffset / view.ScrollableHeight;
            Debug.WriteLine(progress);
            if (progress > 0.9 && isLoadMore && (Items.Count < 120))
            {
                isLoadMore = false;
                String requestURL = REQUEST_URL + "&count=" + ITEMS_TO_LOAD + "&page=" + ((Items.Count / ITEMS_TO_LOAD) + 1);
                LoadMoreAsync(requestURL);
                //GetAString("http://playua.net/?json=get_recent_posts&count=12&thumbnail_size=n-fulls&page=" + ++CurrentPage);
            }
        }
#endregion

#region Pull_To_Refresh
        bool _isPullRefresh = false;

        private void scrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer.ChangeView(null, 60.0, null);
        }

        private void scrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PostsList.Width = e.NewSize.Width;
            PostsList.Height = e.NewSize.Height;
            scrollViewer.ChangeView(null, 60.0, null);
        }
                
        private async void scrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ScrollViewer = sender as ScrollViewer;

            // Text Change
            //textBlock2.Opacity = ScrollViewer.VerticalOffset / 100.0;
            if (ScrollViewer.VerticalOffset == 0.0)
                textBlock1.Opacity = 0.7;
            else
                textBlock1.Opacity = 0.3;

            if (ScrollViewer.VerticalOffset != 0.0)
                _isPullRefresh = true;

            if (!e.IsIntermediate)
            {
                if (ScrollViewer.VerticalOffset == 0.0 && _isPullRefresh)
                {
                    Debug.WriteLine("_isPullRefresh");
                    //StatusBar.GetForCurrentView().ProgressIndicator.Text = "Оновлення...";
                    //await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
                    //GetAString();
                    //await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
                    LoadUpdatesAsync(REQUEST_URL);
                }
                _isPullRefresh = false;
                ScrollViewer.ChangeView(null, 60.0, null);
            }
        }
        #endregion

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
    }

    public class PostsResponse
    {
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        private string _Title_Plain;
        public String Title_Plain {
            get
            {
                return HtmlUtilities.ConvertToText(this._Title_Plain);
            }

            set
            {
                this._Title_Plain = value;
            }
        }
        public String Excerpt { get; set; }
        public String Content { get; set; }
        public String Url { get; set; }
        public String Slug { get; set; }
        public String Thumbnail { get; set; }
    }

    public class PaginatedObservableCollection<Object> : ObservableCollection<Object>, ISupportIncrementalLoading
    {
        private int LastPost = 1;
        public bool HasMoreItems
        {
            get
            {
                if (LastPost >= 100)
                {
                    return false;
                }

                return true;
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            //ProgressBar progressBar = ((Window.Current.Content as Frame).Content as MainPage).ProgressBar;

            CoreDispatcher coreDispatcher = Window.Current.Dispatcher;

            return Task.Run<LoadMoreItemsResult>(async () =>
            {
                Debug.WriteLine("LoadMoreItemsAsync");
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        //progressBar.IsIndeterminate = true;
                        //progressBar.Visibility = Visibility.Visible;
                    });
                LastPost += 10;
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        //progressBar.Visibility = Visibility.Collapsed;
                        //progressBar.IsIndeterminate = false;
                        
                    });

                return new LoadMoreItemsResult() { Count = count };
            }).AsAsyncOperation<LoadMoreItemsResult>();
        }
    }
}
