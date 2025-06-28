using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUIpad4
{
    public partial class FileOperations
    {
        public FileOperations()
        {
           
        }

        private async Task<ContentDialogResult> DisplayConfirmationDialog(MainWindow mw, String filename)
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = mw.Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "WinUIpad",
                Content = "Do you want to save changes to " + filename,
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Don't save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };
            
            return await dialog.ShowAsync();
        }

        public async void OpenFile(MainWindow mw, Document d)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(".txt");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mw);
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            StorageFile file = await filePicker.PickSingleFileAsync();
            if (file != null)
            {
                d.FileName = file.Path;
                d.Contents = await Windows.Storage.FileIO.ReadTextAsync(file);
                d.TextHasChanged = false;
                d.DocumentIsSaved = true;
            }
        }

        public async Task<bool> NeedsToBeSavedAsync(MainWindow mw, Document d)
        {
            // true: Continue
            // false: Cancel
            if (d.TextHasChanged)
            {
                // The document needs to be saved
                if (d.FileName != null)
                {
                    ContentDialogResult result = await DisplayConfirmationDialog(mw, d.FileName);
                    switch(result)
                    {
                        // Save
                        case ContentDialogResult.Primary:
                            if (SaveDocument(mw, d))
                                return true;
                            break;
                        // Don't save
                        case ContentDialogResult.Secondary:
                            return true;
                        default:
                            return false;
                    }
                }
            }
            // If nothing needs to be done, just continue normally
            return true;
        }

        public bool SaveDocument(MainWindow mw, Document d)
        {
            // true: Success, continue
            // false: Cancel

            if (d.DocumentIsSaved)
            {
                // Save existing file
                if (d.FileName != null)
                {
                    if (File.Exists(Path.GetFullPath(d.FileName)))
                    {
                        File.WriteAllText(d.FileName, d.Contents);
                        d.TextHasChanged = false;
                        d.DocumentIsSaved = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return SaveAs(mw, d).Result;
            }
        }

        public async Task<bool> SaveAs(MainWindow mw, Document d)
        {
            // Save as a new file
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("Text document", new List<string>() { ".txt" });

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mw);
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            StorageFile file = await filePicker.PickSaveFileAsync();
            if (file != null)
            {
                File.WriteAllText(file.Path, d.Contents);
                d.FileName = file.Path;
                d.TextHasChanged = false;
                d.DocumentIsSaved = true;
                return true;
            }
            else
            {
                // User cancelled the save operation
                return false;
            }
        }
    }
}
