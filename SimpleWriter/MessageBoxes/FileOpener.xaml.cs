using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ComplexWriter.Properties;

namespace ComplexWriter.MessageBoxes
{
    /// <summary>
    /// Interaction logic for QuestionBox.xaml
    /// </summary>
    public partial class FileOpener
    {
        public FileOpener()
        {
            InitializeComponent();
        }

      
        public IEnumerable<string> Filenames {get { return FileView.SelectedItems.Cast<string>(); } }

        private void MoveWindow(object sender, DragDeltaEventArgs e)
        {
            var win = ((Thumb)sender).Tag as Window;
            win.Left = win.Left + e.HorizontalChange;
            win.Top = win.Top + e.VerticalChange;
        }

      

        private void CloseWithTag(object sender, RoutedEventArgs e)
        {
            Result = (MessageBoxResult)((Button) sender).Tag;
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var hasExpandedSomething = BuildSpecialFolder(false, Environment.SpecialFolder.DesktopDirectory, "Desktop");

            hasExpandedSomething = BuildSpecialFolder(hasExpandedSomething, Environment.SpecialFolder.MyDocuments, "Mein Dokumente");
            hasExpandedSomething = BuildSpecialFolder(hasExpandedSomething, Environment.SpecialFolder.UserProfile, folderPath.Substring(folderPath.LastIndexOf("\\") + 1));

            foreach (string s in Directory.GetLogicalDrives())
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = s;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                foldersItem.Items.Add(item);
                item.Expanded += folder_Expanded;

                if (!hasExpandedSomething && s.Equals(_pathForEditing.FirstOrDefault()))
                {
                    hasExpandedSomething = true;
                    _pathForEditing = _pathForEditing.Skip(1).ToArray();

                    item.IsExpanded = true;
                }
            }
           
        }

        private bool BuildSpecialFolder(bool hasExpandedSomething, Environment.SpecialFolder folder,string caption)
        {
            var s = Environment.GetFolderPath(folder);
            
                TreeViewItem item = new TreeViewItem();
                item.Header = caption;
                item.Tag = s;
                item.FontWeight = FontWeights.Normal;
                item.Items.Add(dummyNode);
                foldersItem.Items.Add(item);
                item.Expanded += folder_Expanded;

                if (!hasExpandedSomething && s.Equals(_pathForEditing.FirstOrDefault()))
                {
                    
                    _pathForEditing = _pathForEditing.Skip(1).ToArray();

                    item.IsExpanded = true;
                    return true;
                }
            return false;
        }

        private object dummyNode = null;

        /// <summary>
        /// DependencyProperty for 'SelectedImagePath'
        /// </summary>
        public static readonly DependencyProperty SelectedPathProperty =
        DependencyProperty.Register("SelectedPath", typeof(string), typeof(FileOpener), new UIPropertyMetadata("", UpdateFileList));

        /// <summary>
        /// 
        /// </summary>
        public string SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }

        private static void UpdateFileList(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dObj = d as FileOpener;
            if (dObj == null) return;

            var test = (string) e.NewValue;
            dObj.FilesForPath = Utilities.HasFolderWritePermission(test)
                ? new ObservableCollection<string>(
                    Directory.GetFiles(test).Where(elem => elem.EndsWith(".etf") || elem.EndsWith(".rtxt")).ToList())
                : new ObservableCollection<string>();
        }

        /// <summary>
        /// DependencyProperty for 'FilesForPath'
        /// </summary>
        public static readonly DependencyProperty FilesForPathProperty =
        DependencyProperty.Register("FilesForPath", typeof(ObservableCollection<string>), typeof(FileOpener), new UIPropertyMetadata(new ObservableCollection<string>()));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<string> FilesForPath
        {
            get { return (ObservableCollection<string>)GetValue(FilesForPathProperty); }
            set { SetValue(FilesForPathProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'InitialDirectory'
        /// </summary>
        public static readonly DependencyProperty InitialDirectoryProperty =
        DependencyProperty.Register("InitialDirectory", typeof(string), typeof(FileOpener), new UIPropertyMetadata("", UpdatePathForSelection));

        /// <summary>
        /// 
        /// </summary>
        public string InitialDirectory
        {
            get { return (string)GetValue(InitialDirectoryProperty); }
            set { SetValue(InitialDirectoryProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'CharacterList'
        /// </summary>
        public static readonly DependencyProperty CharacterListProperty =
        DependencyProperty.Register("CharacterList", typeof(IEnumerable<Character>), typeof(FileOpener), new UIPropertyMetadata(new List<Character>()));

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Character> CharacterList
        {
            get { return (IEnumerable<Character>)GetValue(CharacterListProperty); }
            set { SetValue(CharacterListProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'Tags'
        /// </summary>
        public static readonly DependencyProperty TagsProperty =
        DependencyProperty.Register("Tags", typeof(IEnumerable<string>), typeof(FileOpener), new UIPropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Tags
        {
            get { return (IEnumerable<string>)GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for 'Showtags'
        /// </summary>
        public static readonly DependencyProperty ShowtagsProperty =
        DependencyProperty.Register("Showtags", typeof(Visibility), typeof(FileOpener), new UIPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility Showtags
        {
            get { return (Visibility)GetValue(ShowtagsProperty); }
            set { SetValue(ShowtagsProperty, value); }
        }


        /// <summary>
        /// DependencyProperty for 'CharacterlistVisibility'
        /// </summary>
        public static readonly DependencyProperty CharacterlistVisibilityProperty =
        DependencyProperty.Register("CharacterlistVisibility", typeof(Visibility), typeof(FileOpener), new UIPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// 
        /// </summary>
        public Visibility CharacterlistVisibility
        {
            get { return (Visibility)GetValue(CharacterlistVisibilityProperty); }
            set { SetValue(CharacterlistVisibilityProperty, value); }
        }




        private static void UpdatePathForSelection(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dObj = d as FileOpener;
            if (dObj == null) return;

            if(!string.IsNullOrEmpty(e.NewValue as string))
            {
                var pathForEditing = ((string) e.NewValue).Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                pathForEditing[0] += "\\";

                dObj._pathForEditing = pathForEditing;
            }
            else
            {
                dObj._pathForEditing = new[] {""};
            }
        }


        private IEnumerable<string> _pathForEditing = new[] { "" };
        private bool _somethingSelected = false;

        void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    var hasExpandedSomething=false;
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        var headerString = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Header = headerString;
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        item.Items.Add(subitem);

                        var hasFolderWritePermission = Utilities.HasFolderWritePermission(s);
                        if (hasFolderWritePermission)
                        {
                            var test = Directory.GetDirectories(s);
                            if(test.Any())
                                subitem.Items.Add(dummyNode);

                        }
                        subitem.Expanded += folder_Expanded;

                        if (!hasExpandedSomething && headerString.Equals(_pathForEditing.FirstOrDefault()))
                        {
                            hasExpandedSomething = true;
                            _pathForEditing = _pathForEditing.Skip(1).ToArray();

                            subitem.IsExpanded = true;
                            if(!_somethingSelected)
                            {
                                subitem.IsSelected = true;
                                _somethingSelected = true;
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
        }

        

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tree = (TreeView)sender;
            TreeViewItem temp = ((TreeViewItem)tree.SelectedItem);
            if (temp == null)
                return;
            
            SelectedPath = (string)temp.Tag;
            //show user selected path
        }

        private void FileIsSelected(object sender, SelectionChangedEventArgs e)
        {
            var add = e.AddedItems.OfType<string>();
            if (!add.Any()) return;


            var file = TextFile.Load(add.Last());

            Content.Document = file.Document;
            CharacterList = file.Characters?.OrderByDescending(GetOrderNumber).Take(5) ?? Enumerable.Empty<Character>();
            Tags = file.Tags ?? Enumerable.Empty<string>();
            Showtags = Tags.Any() ? Visibility.Visible : Visibility.Collapsed;
            CharacterlistVisibility = CharacterList.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        private static string GetOrderNumber(Character elem)
        {
            return string.Format("{0}{1}", (int) elem.Type, elem.IsMain ? "Z" : "A");
        }

        private void OpenFile(object sender, MouseButtonEventArgs e)
        {
            Result = MessageBoxResult.OK;
        }
    }
}
