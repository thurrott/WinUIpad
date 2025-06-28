using Microsoft.UI.Xaml;
using System.ComponentModel;

namespace WinUIpad4
{
    public partial class Document() : DependencyObject, INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string? fileName;
        public string? FileName
        {
            get => fileName;
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        private string? contents;
        public string? Contents
        {
            get => contents;
            set
            {
                if (contents != value)
                {
                    contents = value;
                    OnPropertyChanged(nameof(Contents));
                }
            }
        }

        public static readonly DependencyProperty TextHasChangedProperty = DependencyProperty.Register("TextHasChanged", typeof(bool), typeof(Document), new PropertyMetadata(false));

        public bool TextHasChanged
        {
            get
            {
                return (bool)GetValue(TextHasChangedProperty);
            }
            set
            {
                SetValue(TextHasChangedProperty, value);
            }
        }

        public static readonly DependencyProperty DocumentIsSavedProperty = DependencyProperty.Register("DocumentIsSaved", typeof(bool), typeof(Document), new PropertyMetadata(false));

        public bool DocumentIsSaved
        {
            get
            {
                return (bool)GetValue(DocumentIsSavedProperty);
            }
            set
            {
                SetValue(DocumentIsSavedProperty, value);
            }
        }

        public void ResetDocument()
        {
            FileName = "Untitled.txt";
            Contents = "";
            TextHasChanged = false;
            DocumentIsSaved = false;
        }
    }
}