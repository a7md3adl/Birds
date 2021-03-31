using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Birds.Models;
using Birds.Views;
using Birds.ViewModels;
using System.Collections.ObjectModel;
using static Birds.DataBase.Database;
using System.IO;
using Xamarin.Essentials;
using Birds.Services;

namespace Birds.Views
{
  
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        public static ListView BirdsListView;
        public static ItemsViewModel viewModel;
        public static Item SelectedItem;
        List<ObservableGroupCollection<string, Bird>> groupedData = new List<ObservableGroupCollection<string, Bird>>();
        public static List<Bird> data = new List<Bird>();

        public ItemsPage()
        {
            InitializeComponent();
            BirdsListView = ItemsListView;
             //viewModel = new ItemsViewModel();
            BindingContext = viewModel = new ItemsViewModel(); 
        }

        protected override bool OnBackButtonPressed()
        {
            if (!ItemsSearchHolder.IsVisible) return base.OnBackButtonPressed();
            else
            {
                ItemsSearchHolder.IsVisible = false;
                ClearButton_Clicked(ClearButton, new EventArgs());
                return true;
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
          
        }

         void AddItem_Clicked(object sender, EventArgs e)
        {
            SearchPanel.IsVisible = !SearchPanel.IsVisible;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
            {
                viewModel.LoadItemsCommand.Execute(null);
                data = BirdsTable;
                ItemsListView.ItemsSource = BirdsGroups;
            }
        }

        private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchText.Text == "")
            {
                ClearButton_Clicked(ClearButton, new EventArgs());
                return;
            }
            if (!ItemsSearchHolder.IsVisible)
            {
                ItemsSearchHolder.IsVisible = true;
                ClearButton.Source = "clear";
                ClearButton.TabIndex = 99;
            }
            List<Bird> searchList = new List<Bird>();
            if (SearchText.Text != "")
            {
                ItemsSearchList.BatchBegin();
                if (SearchText.Text.IndexOf(" ") > 0)
                {
                    string[] pattern = new string[] { SearchText.Text };
                    if (SearchText.Text.IndexOf(" ") >= 0) pattern = SearchText.Text.Split(' ');
                    var source = data.Where(o => ListCheck.ContainsAll(o.Search.ToString().ToLower(), pattern)).ToList();
                    searchList = source;

                }
                else
                {
                    var source = data.Where(p => p.Search.ToLower().Contains(SearchText.Text.ToLower())).OrderBy(o => o.Text).ToList();
                    searchList = source;
                }
                ItemsSearchList.BatchCommit();
            }
            else
            {
                var source = data.OrderBy(p => p.Text).ToList();
                searchList = source;

            }
            groupedData = searchList
                        .GroupBy(p => p.Text.ToString())
                        .Select(p => new ObservableGroupCollection<string, Bird>(p))
                        .ToList();
            var ret = new ObservableCollection<ObservableGroupCollection<string, Bird>>(groupedData);
            ItemsSearchList
                .ItemsSource = groupedData;
        }

        private void ClearButton_Clicked(object sender, EventArgs e)
        {

            if (ClearButton.TabIndex == 99)
            {
                SearchText.Text = "";
                ItemsSearchHolder.IsVisible = false;
                ClearButton.TabIndex = 88;

            }
            else
            {
                ItemsSearchList.SelectedItem = null;
                ItemsSearchHolder.IsVisible = true;
                ClearButton.TabIndex = 99;
                var source = data.OrderBy(p => p.Text).ToList();
                groupedData = source
                       .GroupBy(p => p.Text.ToString())
                       .Select(p => new ObservableGroupCollection<string, Bird>(p))
                       .ToList();
                var ret = new ObservableCollection<ObservableGroupCollection<string, Bird>>(groupedData);
                ItemsSearchList
                    .ItemsSource = groupedData;

            }
        }

        private void ItemsSearchList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var bird = e.Item as Bird;
            ClearButton_Clicked(ClearButton, new EventArgs());
            SearchText.Placeholder = bird.Text;
            var item = viewModel.Items.ToList().Find(r => r.Bird.Id == bird.Id);
            if (item != null)
            {
                ItemsSearchList.BatchBegin();
                ItemsListView.ScrollTo(item, ScrollToPosition.Start, true);
                ItemsListView.SelectedItem = item;
                ItemsSearchList.BatchCommit();
                
            }
        }

        private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as Item;
            if (item == null)
                return;
            //Console.WriteLine(item.Bird.Age);
            SelectedItem = item;
            await Navigation.PushAsync(new NewItemPage(item, FormAction.Edit));
        }

        private async void LogoImage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage(new Item(), FormAction.Add));

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

        }

        private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage(new Item(), FormAction.Add));

        }

        [Obsolete]
        private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted) return;
            else
            {
                string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                //File.Copy(Connection.DatabasePath, Path.Combine(path, "Birds.db3"),true);
                Connection.Backup(Path.Combine(path, "Birds.bak"));
                await DisplayAlert("تم النسخ الاجتياطي", "تم اخذ نسخة احتياطية من البيانات و حفظها في مجلد التنزيلات (Downloas)", "Ok");
            }
        }

        [Obsolete]
        private async void ToolbarItem_Clicked_3(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted) return;
            else
            {
                //Connection.Close();
                string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                File.Copy(Path.Combine(path, "Birds.bak"), Connection.DatabasePath, true);
                //DependencyService.Get<MockDataStore>().LoadItems();
                //data = BirdsTable;
                //ItemsListView.ItemsSource = BirdsGroups;
                //Connection.Backup(Path.Combine(path, "Birds.bak"));
                await DisplayAlert("تم الاسترجاع بنجاح", "تم استرجاع النسخة الاحتياطية من البيانات المحفوظة في مجلد التنزيلات (Downloas)", "Ok");
                //Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                DependencyService.Get<IRestart>().Restart();

            }
        }

        [Obsolete]
        private async void ToolbarItem_Clicked_4(object sender, EventArgs e)
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted) return;
            else
            {

                var file = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                File.Copy(Path.Combine(file, "Birds.bak"), Connection.DatabasePath, true);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = Title,
                    File = new ShareFile(Path.Combine(file, "Birds.bak"))
                });
            }
        }

        [Obsolete]
        async Task PickAndShow()
        {
            try
            {
                var customFileType =
                new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "bak" } }, // or general UTType values
                    { DevicePlatform.Android, new[] { "bak" } },
                    { DevicePlatform.UWP, new[] { "bak" } },
                    { DevicePlatform.Tizen, new[] { "bak" } },
                    { DevicePlatform.macOS, new[] { "bak" } }, // or general UTType values
                });
                var options = new PickOptions
                {
                    PickerTitle = "Please select a backup file",
                    FileTypes = customFileType,
                   
                };
                
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    
                    if (result.FileName.EndsWith("bak", StringComparison.OrdinalIgnoreCase))
                    {
                        //var stream = await result.OpenReadAsync();
                        //Image = ImageSource.FromStream(() => stream);
                        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                        if (status != PermissionStatus.Granted) return;
                        else
                        {
                            string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                            File.Copy(result.FullPath, Connection.DatabasePath, true);
                            await DisplayAlert("تم الاسترجاع بنجاح", "تم استرجاع النسخة الاحتياطية من البيانات ", "Ok");
                            DependencyService.Get<IRestart>().Restart();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
                // The user canceled or something went wrong
            }
        }

        [Obsolete]
        private async void ToolbarItem_Clicked_5(object sender, EventArgs e)
        {
            await PickAndShow();
        }
    }


    public class ListCheck
    {
        public static bool ContainsAll(string combined, string[] pattern)
        {

            foreach (string needle in pattern)
            {
                if (!combined.ToLower().Contains(needle.ToLower()))
                    return false;
            }

            return true;
        }
    }
    public class ObservableGroupCollection<S, T> : ObservableCollection<T>
    {
        private readonly S _key;

        public ObservableGroupCollection(IGrouping<S, T> group)
            : base(group)
        {
            _key = group.Key;
        }

        public S Key
        {
            get { return _key; }
        }
    }
}