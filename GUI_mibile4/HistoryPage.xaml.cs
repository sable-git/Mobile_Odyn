using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.DataGrid;

namespace GUI_mobile4
{
    public class Converters: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Color.Transparent;

            var val = (string)value;
            if (val.Contains("["))
            {
                return Color.FromHex("#FFFF00");
            }
            else
            {
                var tokens = val.Split(' ');
                if (tokens.Length == 2)
                    if (tokens[0] == "")
                        return Color.FromHex("#00FF00");
                    else
                        return Color.FromHex("#0000FF");
            }


            return Color.Transparent;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<HistoryRecord> historyList;
        readonly DataGrid dataGrid;
        private HistoryRecord _selectedRecord;
        private bool _isRefreshing;
        double _fontSize;

        public double FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        public ObservableCollection<HistoryRecord> HistoryList
        {
            get
            {
                return historyList;
            }
            set
            {
                historyList = value;
                OnPropertyChanged(nameof(HistoryList));
                //DataGrid.ScrollIntoView(historyList[historyList.Count - 1]);
            }
        }
        public HistoryRecord SelectedRecord
        {
            get
            {
                return _selectedRecord;
            }
            set
            {
                _selectedRecord = value;
                OnPropertyChanged(nameof(SelectedRecord));
            }
        }

        public bool IsRefreshing
        {
           get
            {
                return _isRefreshing;
            }
            set
            {
                _isRefreshing = value;                
                OnPropertyChanged(nameof(IsRefreshing));
                
            }

        }

        public ICommand RefreshCommand { get; set; }
        public HistoryViewModel()
        {

        }
        public HistoryViewModel(ObservableCollection<HistoryRecord> h,DataGrid g)
        {
            HistoryList = h;
            dataGrid = g;
            FontSize = StaticElements.fontSize-4;
           
            RefreshCommand = new Command(CmdRefresh);
        }

        public async void CmdRefresh()
        {
            IsRefreshing = true;
            await Task.Delay(3000);
            IsRefreshing = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
   // [DesignTimeVisible(false)]
    public partial class HistoryPage : ContentPage,IReset
    {
        readonly ObservableCollection<HistoryRecord> historyList;
        double _fontSize;

        public double FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        public HistoryPage(ObservableCollection<HistoryRecord> h)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            FontSize = StaticElements.fontSize;
            historyList = h;
            var it = new HistoryViewModel(h, dgX);           
            InitializeComponent();
            BindingContext = it;

            h.CollectionChanged += DgX_Refreshing;
        }
        void IReset.Reset()
        {
            historyList.Clear();
        }
        private void DgX_Refreshing(object sender, EventArgs e)
        {
            if (historyList.Count > 0)
            {
                dgX.SelectedItem = historyList[historyList.Count - 1];
                dgX.ScrollTo(historyList[historyList.Count - 1], ScrollToPosition.End);
            }
        }
    }
}
