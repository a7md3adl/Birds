using Birds.Models;
using Birds.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Birds.DataBase.Database;

namespace Birds.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command DeleteItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Birds";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            //DeleteItemsCommand = new Command(async () => await ExecuteDeleteItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "DeleteItem", async (obj, item) =>
            {
                var newItem = item as Item;
                
                await DataStore.DeleteItemAsync(newItem.Id);
            });
            MessagingCenter.Subscribe<NewItemPage, Item>(this, "UpdateItem", async (obj, item) =>
            {
                var newItem = item as Item;

                await DataStore.UpdateItemAsync(newItem);
            });

        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                ItemsPage.BirdsListView.ItemsSource = BirdsGroups;
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

        //async Task ExecuteDeleteItemsCommand()
        //{
        //    if (IsBusy)
        //        return;

        //    IsBusy = true;

        //    try
        //    {
        //        //DataBase.Database.Connection.Delete<Bird>(item);
        //        //var items = await DataStore.DeleteItemAsync((item as Bird).Id);
               
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }
        //}
    }
}