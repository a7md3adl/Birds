using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Birds.Models;
using Android.Graphics;
using System.IO;
using System.Reflection;
using FFImageLoading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Birds.ComboBox;
using Xamarin.Forms.ComboBox;
using Birds.Services;
using System.Diagnostics;
using static Birds.DataBase.Database;
using SQLite;
using System.Linq;

namespace Birds.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    //[DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class NewItemPage : ContentPage
    {
        
        public static NewItemPage Instance {get;set;}
        public static CarouselView CVIEW { get; set; }
        public Item Item { get; set; }
        public List<Images> Banners { set; get; }

        public NewItemPage(Item item, FormAction action)
        {
            Instance = this;
            InitializeComponent();
            if (action == FormAction.Add) 
            { 
            DelMenu.IsEnabled = false; 
            DelMenu.Text = "";
            DelMenu.IconImageSource = null;
            item.BirthDate = DateTime.Now; 
            item.Bird.BirthDate = DateTime.Now;
            if(Connection.Table<Bird>().Count() > 0) item.Number = Connection.Table<Bird>().Max(o=>o.Number) + 1; 
            item.Bird.Number = item.Number; }
            Item = item;
            var VM = new ComboBoxViewModel();
           
            //IndicatorView iv = new IndicatorView();
            //iv.IndicatorsShape = IndicatorShape.Circle;
            //iv.IsVisible = true;
            //iv.IsEnabled = true;
            //iv.IndicatorColor = Color.Red;
            //iv.ItemsSource = cvBanners.ItemsSource;
            //iv.VerticalOptions = LayoutOptions.End;
            //iv.HorizontalOptions = LayoutOptions.FillAndExpand;
            //iv.WidthRequest = 100;
            //iv.HeightRequest = 50;
            //Item.Banners = GetBanners();
            //_comboBox1.SetBinding(Xamarin.Forms.ComboBox.ComboBox.ItemsSourceProperty, "List",BindingMode.OneWay);

            //_comboBox1.BindingContext = VM;

            //_comboBox1.ItemsSource = VM.List;
            BindingContext = Item;

            CVIEW = cvBanners;

        }
        public ComboBoxViewModel VM { get; private set; }

        public static string DefaultBitmapString { get => ConvertBitmapToString(BitmapFactory.DecodeStream(ImageService.Instance.LoadCompiledResource("ImageTemp.png").AsPNGStreamAsync().Result)); }
        public static async Task<Bitmap> loadResource(string resourceID)
        {
            Bitmap bid;
            using (var stream = await ImageService.Instance.LoadCompiledResource(resourceID).AsPNGStreamAsync())
            {
                //var t = SKBitmap.Decode(stream);
                
                bid =   BitmapFactory.DecodeStream(stream);

               
            }
            return bid;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            if (Item.Bird.No == 0)
            {

                MessagingCenter.Send(this, "AddItem", Item);
            }
            else
            {

                MessagingCenter.Send(this, "UpdateItem", Item);

            }
            await Navigation.PopAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            var results = await NewItemPage.Instance.DisplayAlert("مسح الطائر", "هل تريد مسح بينات الطائر الحالي؟", "موافق", "الغاء");
            if (results)
            {
                DataBase.Database.Connection.Query<Images>("DELETE FROM Images WHERE Id LIKE '" + Item.Id + "'");
                MessagingCenter.Send(this, "DeleteItem", Item);
                ItemsPage.viewModel.LoadItemsCommand.Execute(null);
                await Navigation.PopAsync();
            }
        }
        

        public  List<Images> GetBanners()
        {
          
                //Banners.Clear();
            return DataBase.Database.Connection.Query<Images>("SELECT * FROM Images WHERE BirdID LIKE '"+Item.Bird.Id+"' ");



        }
        public byte[] ImageDataFromResource(string r)
        {
            // Ensure "this" is an object that is part of your implementation within your Xamarin forms project
            var assembly = this.GetType().GetTypeInfo().Assembly;
            byte[] buffer = null;

            using (System.IO.Stream s = assembly.GetManifestResourceStream(r))
            {
                if (s != null)
                {
                    long length = s.Length;
                    buffer = new byte[length];
                    s.Read(buffer, 0, (int)length);
                }
            }

            return buffer;
        }
        public static string ConvertBitmapToString(Bitmap theBitmap)
        {
            string strImage = string.Empty;
            using (var stream = new System.IO.MemoryStream())
            {
                theBitmap.Compress(Bitmap.CompressFormat.Png, 25, stream);
                var bytes = stream.ToArray();
                strImage = Convert.ToBase64String(bytes);
            }

            return strImage;
        }

        public static Bitmap ConvertStringToBitmap(string theBitmap)
        {
            Bitmap img = null;

            if (theBitmap != null)
            {
                byte[] decodedByte = Android.Util.Base64.Decode(theBitmap, 0);
                img = BitmapFactory.DecodeByteArray(decodedByte, 0, decodedByte.Length);

            }

            return img;
        }
        List<Images> imageList = new List<Images>();

        [Obsolete]
        private async void LogoImage_Clicked(object sender, EventArgs e)
        {
            ImageButton_Clicked(sender, e);
            return;
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted) return;
            else
            {

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    
                    SaveToAlbum = false,
                    CompressionQuality = 50,
                    CustomPhotoSize = 50,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    MaxWidthHeight = 2000,
                    DefaultCamera = CameraDevice.Rear
                });
                if (file != null)
                {
                    cvBanners.ItemsSource = null;
                    var img = new Images
                    {
                        //Id = Guid.NewGuid().ToString(),
                        BirdID = Item.Bird.Id,
                        Image = ConvertBitmapToString(BitmapFactory.DecodeStream(File.Open(file.Path,FileMode.Open)))
                        //ImageDate = DateTime.Now,

                    };
                    DataBase.Database.Connection.Insert(img);
                    Item.Banners.Add(img);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Comparison<Images> comparison = delegate (Images x, Images y)
                        {
                            ////if (x.PartName == null && y.PartName == null) return 0;
                            ////else if (x.PartName == null) return -1;
                            ////else if (y.PartName == null) return 1;
                            //else 
                            if (x.ImageDate > y.ImageDate) return -1;
                            else return 1;
                            //return x.ImageDate.CompareTo(y.ImageDate);
                        };
                        Item.Banners.Sort(comparison);
                        cvBanners.ItemsSource = Item.Banners;

                        //try
                        //{
                        //    var enu = cvBanners.ItemsSource.GetEnumerator();
                        //    int pos = 0;
                        //    while (enu.MoveNext()) { cvBanners.Position = pos; pos++; Console.WriteLine(pos.ToString()); }
                        //    cvBanners.Position = pos - 1;


                        //    }
                        //catch (Exception)
                        //{

                        //}
                    });

                }
            }
        }

        private void Combo1_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        {

        }

        private void Editor_Completed(object sender, EventArgs e)
        {

        }

        [Obsolete]
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {

                Plugin.Media.Abstractions.MediaFile file = null;
                string action = await DisplayActionSheet("اختار مصدر الصورة", "الغاء", null, "الكاميرا", "المعرض");
                if (action != null && action != "" && action != string.Empty)
                {
                    if (action == "المعرض")
                    {
                        if (!CrossMedia.Current.IsPickPhotoSupported)
                        {
                            await DisplayAlert("Not Supported", "Picking a photo is not supported", "ok");
                            return;
                        }

                        file = await CrossMedia.Current.PickPhotoAsync();

                        if (file == null)
                            return;
                    }
                    if (action == "الكاميرا")
                    {
                        var status = await Permissions.RequestAsync<Permissions.Camera>();
                        if (status != PermissionStatus.Granted) return;

                        if (!CrossMedia.Current.IsPickPhotoSupported)
                        {
                            await DisplayAlert("Not Supported", "Taking pictures is not supported", "ok");
                            return;
                        }

                        file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());

                        if (file == null)
                            return;

                    }
                    if (file != null)
                    {
                        cvBanners.ItemsSource = null;
                        var img = new Images
                        {
                            BirdID = Item.Bird.Id,
                            Image = ConvertBitmapToString(BitmapFactory.DecodeStream(File.Open(file.Path, FileMode.Open)))

                        };
                        DataBase.Database.Connection.Insert(img);
                        Item.Banners.Add(img);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Comparison<Images> comparison = delegate (Images x, Images y)
                            {
                                if (x.ImageDate > y.ImageDate) return -1;
                                else return 1;
                            };
                            Item.Banners.Sort(comparison);
                            cvBanners.ItemsSource = Item.Banners;
                        });

                    }
                }
            }
            catch (Exception)
            {

                // throw;
            }

        }

        private void SavedImageButtonClicked(object sender, EventArgs e)
        {
            // var file = await CrossFilePicker.Current.PickFile();
            var b = sender as ImageButton;
            
           // DependencyService.Get<IPlatformSpecificAndroid>().DisplayInGallery(note);

        }
    }

    public enum FormAction
    {
        Add,Edit
    }
    [SQLite.Table("Images")]
    public class Images
    {
        [Obsolete]
        public Images()
        {
            DisplayImage = new Command(async () => await ExecuteLoadItemsCommand());
            DeleteImage = new Command(async () => await ExecuteDeleteImage());

        }

        async Task ExecuteDeleteImage()
        {
            try
            {
                if (IsBusy)
                    return;

                IsBusy = true;
                await Task.Delay(1);
               
                    var results = await NewItemPage.Instance.DisplayAlert("مسح الصورة", "هل تريد مسح الصورة الحالية؟", "موافق", "الغاء");
                    if (results)
                    {
                     Device.BeginInvokeOnMainThread(async () =>
                    {
                        NewItemPage.CVIEW.ItemsSource = null;
                        NewItemPage.Instance.Item.Banners.Remove(NewItemPage.Instance.Item.Banners.Find(o=>o.Id == Id));
                        DataBase.Database.Connection.Table<Images>().Delete((Images i) => i.Id == Id);
                        await Task.Delay(1);
                        //NewItemPage.Instance.Item.Banners = DataBase.Database.Connection.Query<Images>("SELECT * FROM Images WHERE BirdID LIKE '" + Id + "' ");
                       // NewItemPage.CVIEW.ItemsSource = NewItemPage.Instance.Item.Banners;
                        Comparison<Images> comparison = delegate (Images x, Images y)
                        {
                            if (x.ImageDate > y.ImageDate) return -1;
                            else return 1;
                        };
                        NewItemPage.Instance.Item.Banners.Sort(comparison);
                        NewItemPage.CVIEW.ItemsSource = NewItemPage.Instance.Item.Banners;
                    });

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [SQLite.Ignore]
        public Command DisplayImage { get; set; }
        [SQLite.Ignore]
        public Command DeleteImage { get; set; }

        [Obsolete]
        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                MemoryStream ms = new MemoryStream(ImageByteArray);
                string fileName = Guid.NewGuid().ToString().Replace("-", "") + ".png";
                var documentsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath; //System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                documentsPath = System.IO.Path.Combine(documentsPath);
                Directory.CreateDirectory(documentsPath);

                string filePath = System.IO.Path.Combine(documentsPath, fileName);
                App.TempImagePath = filePath;
                DependencyService.Get<IFileService>().SavePicture(fileName, ms);
                await Task.Delay(1);
                 DependencyService.Get<IPlatformSpecificAndroid>().DisplayInGallery(filePath);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [SQLite.PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BirdID { get; set; }
        public DateTime ImageDate { get; set; } = DateTime.Now;
        public string Image { get; set; }

        public string DateString { get => ImageDate.ToString("yyyy-MM-dd"); }
        [SQLite.Ignore]
        public ImageSource Source { get {
                if (_Source != null) return _Source;
                _Source = ImageSource.FromStream(() => new MemoryStream(ImageByteArray, 0, ImageByteArray.Length));
                return _Source;
            } }


        [SQLite.Ignore]
        public byte[] ImageByteArray
        {
            get
            {
                {
                    if (_ImageByteArray != null) return _ImageByteArray;
                    string Img ="";
                    if (Image == null || Image == "") Img = NewItemPage.DefaultBitmapString;// NewItemPage.ConvertBitmapToString(NewItemPage.loadResource("ImageTemp.png").Result);
                    else Img = Image;
                    //byte[] bitmapData;
                    using (var stream = new MemoryStream())
                    {
                       NewItemPage.ConvertStringToBitmap(Img).Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                        _ImageByteArray = stream.ToArray();
                    }
                    return _ImageByteArray;
                }
            }
        }
        [SQLite.Ignore]
        private ImageSource _Source { set; get; }

        [SQLite.Ignore]
        private byte[] _ImageByteArray { get; set; }
        [SQLite.Ignore]
        public bool IsBusy { get; private set; }
    }
}