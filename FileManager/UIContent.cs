using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileManager
{
    class UIContent : Grid
    {
        public string Path { get; }
        Image icoImg;
        TextBlock nameTB;
        TextBlock sizeMemoryTB;

        public UIContent(DriveInfo drive)
        {
            icoImg = new Image { Source = Manager.Images[Ico.Folder], StretchDirection = StretchDirection.Both };
            nameTB = new TextBlock { Text = drive.Name, TextAlignment = TextAlignment.Left };
            sizeMemoryTB = new TextBlock { Text = drive.IsReady ? Manager.GetDriveSpaceInfo(drive.AvailableFreeSpace, drive.TotalSize) : "Диск не готов", TextAlignment = TextAlignment.Right, };
            Path = drive.Name;
        }

        public UIContent(DirectoryInfo directory, MouseButtonEventHandler e)
        {
            icoImg = new Image { Source = Manager.Images[Ico.Folder], StretchDirection = StretchDirection.Both };
            nameTB = new TextBlock { Text = directory.Name.Length > 12 ? directory.Name.Remove(12) + "..." : directory.Name, TextAlignment = TextAlignment.Left };
            sizeMemoryTB = new TextBlock { Text = "Свойства", TextAlignment = TextAlignment.Right};
            sizeMemoryTB.MouseDown += e;
            Path = directory.FullName;
        }

        public UIContent(FileInfo file)
        {
            icoImg = new Image { Source = Manager.Images[Ico.File], StretchDirection = StretchDirection.Both };
            nameTB = new TextBlock { TextAlignment = TextAlignment.Left };

            if (file.Name.Length - file.Extension.Length > 12 && file.Extension.Length > 0)
                nameTB.Text = file.Name.Remove(12) + $"...({file.Extension})";
            else if (file.Extension.Length > 0)
                nameTB.Text = file.Name.Remove(file.Name.Length - file.Extension.Length) + $"({file.Extension})";
            else
                nameTB.Text = file.Name;

            sizeMemoryTB = new TextBlock { Text = Manager.GetFileSizeInfo(file.Length), TextAlignment = TextAlignment.Right };
            Path = file.FullName;
        }

        public void Add(Grid MainGrid)
        {
            VerticalAlignment = VerticalAlignment.Top;
            Height = 18;

            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(15) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(430) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(190) });
            Children.Add(icoImg);
            Children.Add(nameTB);
            Children.Add(sizeMemoryTB);
            SetColumn(nameTB, 1);
            SetColumn(sizeMemoryTB, 2);

            if (MainGrid.Children.Count > 0)
            {
                Thickness marg = (MainGrid.Children[MainGrid.Children.Count - 1] as Grid).Margin;
                marg.Top = marg.Top + 18;
                Margin = marg;
            }
            MainGrid.Children.Add(this);
        }
    }
}
