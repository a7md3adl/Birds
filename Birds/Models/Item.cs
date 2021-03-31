using Birds.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using static Birds.DataBase.Database;

namespace Birds.Models
{
    public class Item : INotifyPropertyChanged
    {
        private string _Id;
        private string _Text;
        private string _Description;
        private DateTime _BirthDate;
        private Bird _Bird;
        private List<Images> _Banners;
        private int _Number;

        public Item()
        {
            _Id = Guid.NewGuid().ToString();
        }
        public string Id
        {
            get { return _Id; }
            set { SetProperty(ref _Id, value); }
        }
        public string Text
        {
            get { return _Text; }
            set { SetProperty(ref _Text, value); }
        }
        public int Number
        {
            get { return _Number; }
            set { SetProperty(ref _Number, value); }
        }

        public string Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, value); }
        }
        public DateTime BirthDate
        {
            get { return _BirthDate; }
            set { SetProperty(ref _BirthDate, value); }
        }
        public Bird Bird
        {
            get { return _Bird; }
            set { SetProperty(ref _Bird, value); }
        }

        public List<Images> Banners
        {
            get 
            {
                if(_Banners == null) _Banners = Connection.Query<Images>("SELECT * FROM Images WHERE BirdID LIKE '" + Id + "' ");
                return _Banners; 
            }
            set { SetProperty(ref _Banners, value); }
        }

        public ImageSource Source
        {
            get
            {
                if (Bird != null && Bird.Id != null)
                {
                    var list = Connection.Table<Images>().Where(y => y.BirdID != null && y.BirdID == Bird.Id).ToList();
                    if ( list != null && list.Count > 0)
                    {
                        var image = list.FindLast(p=>p.BirdID == Bird.Id);
                        if (image != null) return image.Source;
                    }
                }
                return null;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
           
            

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        protected bool SetProperty<T>(ref T backingStore, T value,
           [CallerMemberName]string propertyName = "",
           Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            if (propertyName == "Id")
            {
                if (this.Bird == null)
                {

                    this.Bird = Connection.Query<Bird>("SELECT * FROM  Birds WHERE Id LIKE '" + Id + "'").FindLast(o => o.Id == Id);
                }
            }
            backingStore = value;
            if (this.Bird != null)
            {
                this.Bird.Id = this.Id;
                this.Bird.Text = this.Text;
                this.Bird.Description = this.Description;
                this.Bird.BirthDate = this.BirthDate;
                this.Bird.Number = this.Number;

            }
            else
            {
                
                this.Bird = new Bird { Id = this.Id, Text = this.Text, Description = this.Description, BirthDate = this.BirthDate  };
            }
             //_Description =  _BirthDate.ToString("yyyy MM dd");
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
    }
   
}