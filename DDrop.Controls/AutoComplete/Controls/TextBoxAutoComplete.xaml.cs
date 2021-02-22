using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DDrop.Controls.AutoComplete.Controls
{
    /// <summary>
    /// Interaction logic for TextBoxAutoCompleteProvider.xaml
    /// </summary>
    public partial class TextBoxAutoComplete : INotifyPropertyChanged
    {
        #region Dependancy Properties

        #region TargetControl
        /// <summary>
        /// The target control is the textbox which this extender wil be supporting.
        /// </summary>
        public TextBox TargetControl
        {
            get { return (TextBox)GetValue(TargetControlProperty); }
            set { SetValue(TargetControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetControl.
        public static readonly DependencyProperty TargetControlProperty =
            DependencyProperty.Register("TargetControl",
                                        typeof(TextBox),
                                        typeof(TextBoxAutoComplete),
                                        new UIPropertyMetadata(null,
                                                    TargetControl_Changed),
                                                    TargetControl_Validate
                                        );

        /// <summary>
        /// Validate that the target is a textbox control
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True if TargetControl is a textbox</returns>
        private static bool TargetControl_Validate(object value)
        {
            TextBox newv = value as TextBox;
            if (newv == null && value != null) return false;
            return true;
        }

        /// <summary>
        /// When we assign the target control we set up event handlers.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void TargetControl_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                TextBoxAutoComplete me = d as TextBoxAutoComplete;
                TextBox oldv = e.OldValue as TextBox;
                TextBox newv = e.NewValue as TextBox;
                if (oldv != null)
                {
                    if (me != null)
                    {
                        oldv.LostFocus -= me.TargetControl_LostFocus;
                        oldv.GotFocus -= me.TargetControl_GotFocus;
                        oldv.KeyUp -= me.TargetControl_KeyUp;
                        oldv.PreviewKeyUp -= me.TargetControl_PreviewKeyUp;
                        oldv.PreviewKeyDown -= me.TargetControl_PreviewKeyDown;
                    }
                }
                if (newv != null)
                {
                    if (me != null)
                    {
                        me.popup.PlacementTarget = newv;
                        newv.LostFocus += me.TargetControl_LostFocus;
                        newv.GotFocus += me.TargetControl_GotFocus;
                        newv.KeyUp += me.TargetControl_KeyUp;
                        newv.PreviewKeyUp += me.TargetControl_PreviewKeyUp;
                        newv.PreviewKeyDown += me.TargetControl_PreviewKeyDown;
                    }
                }
            }
        }

        private void TargetControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!popup.IsKeyboardFocusWithin)
                popup.IsOpen = false;
            IsBusy = false;

            if (SelectedListBoxItem == null)
                TargetControl.Text = "";
        }

        private void TargetControl_GotFocus(object sender, RoutedEventArgs e)
        {
            
            if (SelTextOnFocus) txtSearch.SelectAll();
            if (_itemsSelected)
                _itemsSelected = false;
        }

        private void TargetControl_KeyUp(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Up)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex--;
                }
            }
            if (e.Key == Key.Down)
            {
                if (listBox.SelectedIndex < listBox.Items.Count - 1)
                {
                    listBox.SelectedIndex++;
                }
            }
            if (e.Key == Key.Enter)
            {
                if (popup.IsOpen && listBox.Items.Count > 0)
                    SetTextAndHide();
                else if (SearchText.Length < _parialSearchTextLength)
                    TextBoxEnterCommand.Execute(null);
            }
        }

        private void TargetControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                popup.IsOpen = false;
                listBox.SelectedItem = null;
                e.Handled = true;
            }
            if (IsTextChangingKey(e.Key))
            {
                Suggest();                
            }
        }

        private void TargetControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && listBox.SelectedItem != null)
            {
                popup.IsOpen = false;
                TargetControl.Text = String.IsNullOrEmpty(DisplayMemberPath) ?
                                     listBox.SelectedItem.ToString() :
                                     listBox.SelectedItem.GetType().GetProperty(
                                     DisplayMemberPath).GetValue(listBox.SelectedItem, null).ToString();
                if (MovesFocus)
                    TargetControl.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
        }

        private bool IsTextChangingKey(Key key)
        {
            if (TargetControl.Text.Length < 3)
            {
                listBox.SelectedItem = null;
                popup.IsOpen = false;
                return false;
            }

            if (key == Key.Back || key == Key.Delete)
            {
                if (TargetControl.Text == "")
                {
                    listBox.SelectedItem = null;
                    popup.IsOpen = false;
                }
                return true;
            }

            KeyConverter conv = new KeyConverter();
            string keyString = (string)conv.ConvertTo(key, typeof(string));                

            return keyString != null && keyString.Length == 1;
        }

        private void Suggest()
        {
                SearchText = TargetControl.Text;
                if (_odp != null) IsBusy = true;
        }       

        #endregion

        #region MovesFocus
        /// <summary>
        /// Do we move focus to the next control, when we 
        /// have selected an item from the dropdown list?
        /// </summary>
        public bool MovesFocus
        {
            get { return (bool)GetValue(MovesFocusProperty); }
            set { SetValue(MovesFocusProperty, value); }
        }

        public static readonly DependencyProperty MovesFocusProperty =
            DependencyProperty.Register("MovesFocus", typeof(bool), typeof(TextBoxAutoComplete), new UIPropertyMetadata(true));

        #endregion

        #region SelTextOnFocus
        /// <summary>
        /// Seleccionar todo el texto cuando el control recibe el foco        
        /// </summary>
        public bool SelTextOnFocus
        {
            get { return (bool)GetValue(SelTextOnFocusProperty); }
            set { SetValue(SelTextOnFocusProperty, value); }
        }

        public static readonly DependencyProperty SelTextOnFocusProperty =
            DependencyProperty.Register("SelTextOnFocus", typeof(bool), typeof(TextBoxAutoComplete), new UIPropertyMetadata(true));

        #endregion

        #region IsBusy
        /// <summary>
        /// Indica que el control esta en el ciclo de busqueda de datos
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(TextBoxAutoComplete), new UIPropertyMetadata(false));

        #endregion

        #region WatermarkText
        /// <summary>
        /// Indica que texto a mostrar cuando no se ingreso nada en el texto del control
        /// </summary>
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
        }

        public static readonly DependencyProperty WatermarkTextProperty  =
            DependencyProperty.Register("WatermarkText", typeof(String), typeof(TextBoxAutoComplete), new UIPropertyMetadata(""));

        #endregion

        #region Mode

        public bool IsEdited
        {
            get { return (bool) GetValue(IsEditedProperty); }
            set { SetValue(IsEditedProperty, value); NotifyPropertyChanged("IsEdited"); }
        }

        public static readonly DependencyProperty IsEditedProperty =
            DependencyProperty.Register("IsEdited", typeof(bool), typeof(TextBoxAutoComplete), new UIPropertyMetadata(false));

        #endregion

        #region SavedValue

        public object SavedValue
        {
            get => GetValue(SavedValueProperty);
            set { SetValue(SavedValueProperty, value); NotifyPropertyChanged("SavedValue"); }
        }

        public static readonly DependencyProperty SavedValueProperty =
            DependencyProperty.Register("SavedValue", typeof(object), typeof(TextBoxAutoComplete),
                new UIPropertyMetadata(null));

        #endregion

        #region ExistingValue

        public string ExistingValue
        {
            get => (string)GetValue(ExistingValueProperty);
            set { SetValue(ExistingValueProperty, value); NotifyPropertyChanged("ExistingValue"); }
        }

        public static readonly DependencyProperty ExistingValueProperty =
            DependencyProperty.Register("ExistingValue", typeof(string), typeof(TextBoxAutoComplete),
                new UIPropertyMetadata(null));

        #endregion

        #region DeleteIsEnabled

        public bool IsDeleteEnabled
        {
            get { return (bool)GetValue(IsDeleteEnabledProperty); }
            set { SetValue(IsDeleteEnabledProperty, value); NotifyPropertyChanged("IsDeleteEnabled"); }
        }

        public static readonly DependencyProperty IsDeleteEnabledProperty =
            DependencyProperty.Register("IsDeleteEnabled", typeof(bool), typeof(TextBoxAutoComplete), new UIPropertyMetadata(false));

        #endregion

        #endregion

        private static Int32 _parialSearchTextLength = 2;
        
        public TextBoxAutoComplete()
        {
            InitializeComponent();
            grid.DataContext = this;

            TargetControl = txtSearch;

            // ListBox inside the Popup
            listBox.SelectionChanged += listBox_SelectionChanged;
            listBox.SelectionMode = SelectionMode.Single;
            _itemsSelected = false;            

            // Setup the command for the enter key on the textbox
            _textBoxEnterCommand = new ReactiveRelayCommand(obj => { });

            // Listen to all property change events on SearchText
            var searchTextChanged = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                ev => PropertyChanged += ev,
                ev => PropertyChanged -= ev
                )
                .Where(ev => ev.EventArgs.PropertyName == "SearchText");

            // Transform the event stream into a stream of strings (the input values)
            var input = searchTextChanged
                .Where(ev => SearchText == null || SearchText.Length < _parialSearchTextLength)
                .Throttle(TimeSpan.FromSeconds(2))
                .Merge(searchTextChanged
                    .Where(ev => SearchText != null && SearchText.Length >= _parialSearchTextLength)
                    .Throttle(TimeSpan.FromMilliseconds(400)))
                .Select(args => SearchText)
                .Merge(
                    _textBoxEnterCommand.Executed.Select(e => SearchText))
                .DistinctUntilChanged();

            // Setup an Observer for the search operation
            var search = Observable.ToAsync<string, SearchResult>(DoSearch);

            // Chain the input event stream and the search stream, cancelling searches when input is received
            var results = from searchTerm in input
                          from result in search(searchTerm).TakeUntil(input)
                          select result;

            // Log the search result and add the results to the results collection
            results.ObserveOn(DispatcherScheduler.Current)
                .Subscribe(
                    result =>
                    {                        
                        SearchResults.Clear();
                        
                        if (result.Results == null) return;
                        result.Results.ToList().ForEach(item => SearchResults.Add(item));

                        popup.VerticalOffset = TargetControl.ActualHeight;

                        if (result.Results?.Count > 0)
                        {
                            popup.IsOpen = true;
                        }
                        
                        IsBusy = false;
                        SelectedListBoxValue = null;
                    },
                    ex => {
                        string msg = string.Format("Exception {0} in OnError handler\nException.Message : {1}", ex.GetType(), ex.Message);
                        MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
                    }
                );
        }

        #region Control Properties

        public string DisplayMemberPath
        {
            get { return listBox.DisplayMemberPath; }
            set { listBox.DisplayMemberPath = value; }
        }

        public string SelectedValuePath
        {
            get { return listBox.SelectedValuePath; }
            set { listBox.SelectedValuePath = value; }
        }

        public DataTemplate ItemTemplate
        {
            get { return listBox.ItemTemplate; }
            set { listBox.ItemTemplate = value; }
        }

        public ItemsPanelTemplate ItemsPanel
        {
            get { return listBox.ItemsPanel; }
            set { listBox.ItemsPanel = value; }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return listBox.ItemTemplateSelector; }
            set { listBox.ItemTemplateSelector = value; }
        }

        #region SelectedListBoxIndex
        public Int32 SelectedListBoxIndex
        {
            get { return (Int32)GetValue(SelectedListBoxIndexProperty); }
            set { SetValue(SelectedListBoxIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedListBoxIndexProperty = DependencyProperty.Register("SelectedListBoxIndex", typeof(Int32), typeof(TextBoxAutoComplete), new UIPropertyMetadata(0));
        #endregion

        #region SelectedListBoxItem
        public object SelectedListBoxItem
        {
            get { return GetValue(SelectedListBoxItemProperty); }
            set { SetValue(SelectedListBoxItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedListBoxItemProperty = 
            DependencyProperty.Register("SelectedListBoxItem", typeof(object), typeof(TextBoxAutoComplete), new UIPropertyMetadata(null));                
        #endregion


        #region SelectedListBoxValue
        public object SelectedListBoxValue
        {
            get { return GetValue(SelectedListBoxValueProperty); }
            set { SetValue(SelectedListBoxValueProperty, value); }
        }

        public static readonly DependencyProperty SelectedListBoxValueProperty = 
            DependencyProperty.Register("SelectedListBoxValue", typeof(object), typeof(TextBoxAutoComplete), new UIPropertyMetadata(null, SelectedListBoxValue_Changed));

        private static void SelectedListBoxValue_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxAutoComplete me = d as TextBoxAutoComplete;            
            if (e.NewValue != null)
            {
                if (me != null && !me.popup.IsOpen)
                {                 
                    me.SetTextAndHide();
                }
                else
                {
                    if (me != null)
                    {
                        me.TargetControl.Text = String.IsNullOrEmpty(me.DisplayMemberPath)
                            ? me.listBox.SelectedItem.ToString()
                            : me.listBox.SelectedItem.GetType().GetProperty(
                                me.DisplayMemberPath).GetValue(me.listBox.SelectedItem, null).ToString();

                        me.popup.IsOpen = false;
                    }
                }
                
            }
        }

        #endregion


        #region PopupWidth
        /// <summary>
        /// Set the width of the Popup
        /// </summary>
        public Int32 PopupWidth
        {
            get { return (Int32)GetValue(PopupWidthProperty); }
            set { SetValue(PopupWidthProperty, value); }
        }

        public static readonly DependencyProperty PopupWidthProperty = DependencyProperty.Register("PopupWidth", typeof(Int32), typeof(TextBoxAutoComplete), new UIPropertyMetadata(0));
        #endregion

        public PopupAnimation PopupAnimation
        {
            get { return popup.PopupAnimation; }
            set { popup.PopupAnimation = value; }
        }

        #endregion

        #region SearchText
        private string _searchText;
        /// <summary>
        /// The search text is populated from the filtered
        /// keystrokes observed in the target control.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyPropertyChanged("SearchText");
            }
        }
        #endregion

        #region TextBoxEnterCommand
        private ReactiveRelayCommand _textBoxEnterCommand;
        /// <summary>
        /// If the operator presses the enter key
        /// we immediately do a search.
        /// </summary>
        public ReactiveRelayCommand TextBoxEnterCommand
        {
            get { return _textBoxEnterCommand; }
            set { _textBoxEnterCommand = value; }
        }
        #endregion

        #region Listbox Selection

        private bool _itemsSelected;

        void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NotifyPropertyChanged("SelectedItem");
            NotifyPropertyChanged("SelectedValue");
            RaiseSelectionChangedEvent();

            if (popup.IsKeyboardFocusWithin)
            {
                SetTextAndHide();
            }
        }

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBoxAutoComplete));

        // Provide CLR accessors for the event
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        private void RaiseSelectionChangedEvent()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(SelectionChangedEvent);
            RaiseEvent(newEventArgs);
        }

        private void SetTextAndHide()
        {

        }

        #endregion

        private ISearchDataProvider _odp;

        protected SearchResult DoSearch(string searchTerm)
        {
            if (_odp != null & !string.IsNullOrEmpty(searchTerm))
            {
                _ = new SearchResult();
                SearchResult sr;
                if (int.TryParse(searchTerm, out var nId))
                {
                    sr = _odp.SearchByKey(nId);
                }
                else
                {
                    sr = _odp.DoSearch(searchTerm);
                }

                return sr;
            }

            return new SearchResult();
        }

        #region SearchResults

        public ObservableCollection<KeyValuePair<object, string>> SearchResults { get; } = new ObservableCollection<KeyValuePair<object, string>>();

        #endregion

        #region SearchDataProvider
        public ISearchDataProvider SearchDataProvider
        {
            get { return (ISearchDataProvider)GetValue(SearchDataProviderProperty); }
            set { SetValue(SearchDataProviderProperty, value); }
        }

        public static readonly DependencyProperty SearchDataProviderProperty =
            DependencyProperty.Register("SearchDataProvider", typeof(ISearchDataProvider), typeof(TextBoxAutoComplete), new UIPropertyMetadata(null, SearchDataProvider_Changed));


        private static void SearchDataProvider_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxAutoComplete me = d as TextBoxAutoComplete;
            if (me != null) me._odp = e.NewValue as ISearchDataProvider;
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private string _originalValue;
        private void EditModeButton_OnClick(object sender, RoutedEventArgs e)
        {
            IsEdited = true;
            _originalValue = txtSearch.Text;
            txtSearch.Text = "";
        }

        public event RoutedEventHandler SaveClick
        {
            add => SaveButton.Click += value;
            remove => SaveButton.Click -= value;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SavedValue = listBox.SelectedItem;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = _originalValue;
            IsEdited = false;
        }

        public event RoutedEventHandler DeleteClick
        {
            add => DeleteSubstanceButton.Click += value;
            remove => DeleteSubstanceButton.Click -= value;
        }
    }
}
