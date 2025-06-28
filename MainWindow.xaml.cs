using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUIpad4
{
    public sealed partial class MainWindow : Window
    {
        public List<Document> docs = new List<Document>();
        
        public MainWindow()
        {
            InitializeComponent();

            Document d = new();
            docs.Add(d);

            // Custom title bar
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;

            CreateNewTab();
        }

        private void CreateNewTab()
        {
            docs[MyTabs.SelectedIndex].ResetDocument();

            // Set DataContext to the Document instance
            var tabViewItem = new TabViewItem();
            tabViewItem.DataContext = docs[MyTabs.SelectedIndex];

            // Get the converter from resources
            var converter = (IValueConverter)App.Current.Resources["ConvertFileName"];

            // Create the binding for the Header property
            var binding = new Binding
            {
                Path = new PropertyPath("FileName"),
                Converter = converter,
                Mode = BindingMode.OneWay
            };

            // Apply the binding to the Header property
            tabViewItem.SetBinding(TabViewItem.HeaderProperty, binding);

            // Add the TabViewItem to your TabView
            MyTabs.TabItems.Add(tabViewItem);
        }

        private async void NewMenu_Click(object sender, RoutedEventArgs e)
        {
            FileOperations fo = new FileOperations();
            if (await fo.NeedsToBeSavedAsync(this, docs[MyTabs.SelectedIndex]))
            {
                docs[MyTabs.SelectedIndex].ResetDocument();
                TextBox1.Focus(FocusState.Programmatic);

                docs[MyTabs.SelectedIndex].DocumentIsSaved = false;
                docs[MyTabs.SelectedIndex].TextHasChanged = false;
            }
            else return;
        }

        private async void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            FileOperations fo = new FileOperations();
            if (await fo.NeedsToBeSavedAsync(this, docs[MyTabs.SelectedIndex]))
            {
                if (docs[MyTabs.SelectedIndex] != null) 
                    fo.OpenFile(this, docs[MyTabs.SelectedIndex]);
            }
            TextBox1.Focus(FocusState.Programmatic);
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            // https://github.com/microsoft/Windows-universal-samples/tree/main/Samples/FilePicker
            // https://learn.microsoft.com/en-us/windows/apps/develop/files/dotnet-files
            FileOperations fo = new FileOperations(); 
            if (docs[MyTabs.SelectedIndex] != null)
                fo.SaveDocument(this, docs[MyTabs.SelectedIndex]);
        }

        private void AutoSaveMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            FileOperations fo = new FileOperations();
            if (await fo.NeedsToBeSavedAsync(this, docs[MyTabs.SelectedIndex]))
            {
                App.Current.Exit();
            }
        }

        private void TextBox1_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            docs[MyTabs.SelectedIndex].TextHasChanged = true;
        }

        private void MyTabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            // May require more code per
            // https://learn.microsoft.com/en-us/windows/apps/design/controls/tab-view

            sender.TabItems.Remove(args.Tab);

            if (sender.TabItems.Count == 0)
            {
                App.Window.Close();
            }
        }

        private void MyTabs_AddTabButtonClick(TabView sender, object args)
        {

        }
    }

    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
            {
                return "";
            }
            else
            {
                if (value is string formattedValue)
                {
                    try
                    {
                        formattedValue = Path.GetFileNameWithoutExtension(formattedValue);
                        return formattedValue;
                    }
                    catch
                    {
                        return value;
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool booleanValue)
            {
                return booleanValue ? "True" : "False"; // Or customize strings as needed
            }
            return value; // Return original value if it's not a boolean
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // This method is used for two-way binding. 
            // If you don't need it, you can leave it unimplemented or return null.
            throw new NotImplementedException();
        }
    }
}


