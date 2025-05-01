using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FileManager
{
    public partial class MainWindow : Window
    {
        string Path = "Компьютер";

        public MainWindow()
        {
            InitializeComponent();

            ShowDrivers();
            tbPath.Text = Path;
        }

        private void OpenDirectory(object sender, MouseEventArgs e)
        {
            DeleteContent();
            Path = (sender as UIContent).Path;
            ShowContent();
            butBack.IsEnabled = true;
            tbPath.Text = Manager.GetPathTextWithArrow(Path);
        }

        private void ShowDrivers()
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                UIContent dir = new UIContent(drive);
                dir.MouseDown += OpenDirectory;
                dir.MouseEnter += Content_MouseEnter;
                dir.MouseLeave += Content_MouseLeave;
                dir.Add(ContentGrid);
            }
        }

        private void ShowContent()
        {
            var directories = Manager.GetDirectories(Path);
            var files = Manager.GetFiles(Path);

            foreach (var directory in directories)
            {
                UIContent dirTB = new UIContent(directory, OpenProperties);
                dirTB.MouseDown += OpenDirectory;
                dirTB.MouseEnter += Content_MouseEnter;
                dirTB.MouseLeave += Content_MouseLeave;
                dirTB.Add(ContentGrid);
            }
            foreach (var file in files)
            {
                UIContent fileTB = new UIContent(file);
                fileTB.MouseEnter += Content_MouseEnter;
                fileTB.MouseLeave += Content_MouseLeave;
                fileTB.Add(ContentGrid);
            }
        }

        private void DeleteContent()
        {
            for (int i = ContentGrid.Children.Count - 1; i >= 0; i--)
                if (ContentGrid.Children[i] is UIContent)
                    ContentGrid.Children.Remove(ContentGrid.Children[i]);
        }

        private void OpenProperties(object sender, MouseEventArgs e)
        {
            object grid = (sender as TextBlock).Parent;
            MessageBox.Show(Manager.GetDirectorySizeInfo((grid as UIContent).Path));
        }

        private void Content_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as UIContent).Background = Brushes.LightSkyBlue;
        }

        private void Content_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as UIContent).Background = Brushes.White;
        }

        private void GetParentDirectories(object sender, RoutedEventArgs e)
        {
            DeleteContent();
            Path = Manager.GetParentPath(Path);
            if (Path == "Компьютер")
            {
                butBack.IsEnabled = false;
                tbPath.Text = "Компьютер";
                ShowDrivers();
            }
            else
            {
                ShowContent();
                tbPath.Text = Manager.GetPathTextWithArrow(Path);
            }
        }
    }
}
