using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Birds.Models;
using Birds.Views;
using static Birds.DataBase.Database;

namespace Birds.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public MockDataStore()
        {

            items = new List<Item>();
            LoadItems();


        }

        public void LoadItems()
        {
            foreach (Bird bird in BirdsTable)
            {
                items.Add(new Item { Id = bird.Id, Text = bird.Text, BirthDate = bird.BirthDate, Description = bird.Description, Number = bird.Number, Bird = bird });
            }
            if (ItemsPage.BirdsListView != null) ItemsPage.BirdsListView.ItemsSource = BirdsGroups;
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);
            if (item.Bird != null && item.Bird.No == 0)
            {
                try
                {
                    Connection.Insert(item.Bird);

                }
                catch (Exception)
                {

                }            
            }
            if (ItemsPage.BirdsListView != null) ItemsPage.BirdsListView.ItemsSource = BirdsGroups;

            //else
            //{
            //    if (item.Bird == null) item.Bird = new Bird { Id = item.Id, Text = item.Text, Description = item.Description, BirthDate = item.BirthDate };
            //}
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);
            try
            {

                Connection.Update(item.Bird);
                //Console.WriteLine("MockDataStroe Bird Loaded" + "   " + item.Bird.Number.ToString());

            }
            catch (Exception)
            {

            }
            if (ItemsPage.BirdsListView != null) ItemsPage.BirdsListView.ItemsSource = BirdsGroups;

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            try
            {
                Connection.Delete(oldItem.Bird);

            }
            catch (Exception)
            {

            }
            items.Remove(oldItem);
            if (ItemsPage.BirdsListView != null) ItemsPage.BirdsListView.ItemsSource = BirdsGroups;

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}