using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Birds.Services
{
    public interface IDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
    public interface IRestart
    {
        void Restart();
    }
    public interface IPlatformSpecificAndroid
    {
        void DisplayInGallery(string filePath);
    }

    public interface IFileService
    {
        void SavePicture(string name, Stream data, string location = "temp");
    }

    public interface IAppHandler
    {
        Task<bool> LaunchApp(string packageName);
        bool IsAppInstalled(string packageName);
    }
}
