using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;

namespace NoteKeeper
{
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        private static DelayTextBox pageContent;
        private static DelayTextBox pageTitle;
        private static string filePath = "notes.nk";
        //private static List<Note> noteList;
        private static Note currentNote;
        public ObservableCollection<Note> noteList;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            noteList = new ObservableCollection<Note>();
            NoteListView.ItemsSource = NoteList;

            pageContent = new DelayTextBox();
            pageContent.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            pageContent.SetValue(TextBox.AcceptsReturnProperty, true);
            pageContent.SetValue(TextBox.AcceptsTabProperty, true);
            pageContent.SetValue(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            pageContent.SetValue(TextBox.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            pageContent.SetValue(Grid.RowProperty, 2);
            pageContent.SetValue(Grid.ColumnProperty, 2);
            pageContent.Background = (System.Windows.Media.Brush)(new System.Windows.Media.BrushConverter().ConvertFromString("Black"));
            pageContent.Foreground = (System.Windows.Media.Brush)(new System.Windows.Media.BrushConverter().ConvertFromString("White"));
            MainGrid.Children.Add(pageContent);

            pageTitle = new DelayTextBox();
            pageTitle.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            pageTitle.SetValue(TextBox.AcceptsReturnProperty, false);
            pageTitle.SetValue(TextBox.AcceptsTabProperty, false);
            pageTitle.SetValue(Grid.RowProperty, 0);
            pageTitle.SetValue(Grid.ColumnProperty, 2);
            pageTitle.Background = (System.Windows.Media.Brush)(new System.Windows.Media.BrushConverter().ConvertFromString("Black"));
            pageTitle.Foreground = (System.Windows.Media.Brush)(new System.Windows.Media.BrushConverter().ConvertFromString("White"));
            MainGrid.Children.Add(pageTitle);

            ReadData();
            if (noteList.Count == 0)
            {
                currentNote = NewNote();
            }
            else
            {
                currentNote = noteList[0];
            }
            pageTitle.Text = currentNote.PageTitle;
            pageContent.Text = currentNote.PageContent;
            //<ListView Grid.Row="1" Grid.Column="0" Name="PageList" ScrollViewer.CanContentScroll="True" ScrollViewer.VerticalScrollBarVisibility="Auto" Background="Black" Foreground="White" PreviewMouseLeftButtonUp="OnPageSelected">
            //</ ListView >
        }

        public ObservableCollection<Note> NoteList
        {
            get
            {
                return noteList;
            }
        }

        private Note NewNote()
        {
            //create new Note object
            Note n = new Note();
            //set page file descriptor
            n.PageFileDescriptor = Guid.NewGuid().ToString();
            n.PageContent = "";
            n.PageTitle = "";
            //add Note to list
            noteList.Add(n);
            return n;
        }

        private void SwitchCurrentNote(Note n)
        {
            SaveData();
            currentNote = n;
            pageTitle.Text = currentNote.PageTitle;
            pageContent.Text = currentNote.PageContent;
        }

        private void OnNewPage(object sender, RoutedEventArgs e)
        {
            SwitchCurrentNote(NewNote());
        }

        public void SaveData()
        {
            try
            {
                currentNote.PageContent = pageContent.Text;
                currentNote.PageTitle = pageTitle.Text;
                NoteListView.Items.Refresh();
                //open save file, write data, close file
                string s = noteList.Count + ";;";
                foreach(Note n in noteList)
                {
                    s += n.PageFileDescriptor + ";;" + n.PageTitle + ";;" + n.PageContent + ";;";
                }
                File.WriteAllBytes(filePath, Encoding.ASCII.GetBytes(s));
            }
            catch (Exception e)
            {
                //fuuuu
            }
        }

        public void ReadData()
        {
            try
            {
                //open file, read data, close file
                FileStream f = File.OpenRead(filePath);
                int availableBytes = (int)f.Length;
                byte[] b;
                if (!(availableBytes > 0))
                {
                    f.Close();
                    return;
                }
                b = new byte[availableBytes];
                f.Read(b, 0, availableBytes);
                f.Close();
                //read number of notes, create that many, and then read in each set of data to each note, and update the UI with the list
                string s = Encoding.ASCII.GetString(b);
                int index1 = s.IndexOf(";;");
                int index2 = index1;
                int noteCount = int.Parse(s.Substring(0, index1));
                index1 += 2;
                for(int i = 0; i < noteCount; ++i)
                {
                    Note n = new Note();
                    index1 = index2 + 2;
                    index2 = s.IndexOf(";;", index1);
                    n.PageFileDescriptor = s.Substring(index1, index2-index1);
                    index1 = index2 + 2;
                    index2 = s.IndexOf(";;", index1);
                    n.PageTitle = s.Substring(index1, index2-index1);
                    index1 = index2 + 2;
                    index2 = s.IndexOf(";;", index1);
                    n.PageContent = s.Substring(index1, index2-index1);
                    noteList.Add(n);
                }
            }
            catch(Exception e)
            {
                //fuuuu
            }
        }

        private void OnPageSelected(object sender, RoutedEventArgs e)
        {
            Note n = (Note)NoteListView.SelectedItem;
            if (n == null)
            {
                return;
            }
            SwitchCurrentNote(n);
        }
    }
}
