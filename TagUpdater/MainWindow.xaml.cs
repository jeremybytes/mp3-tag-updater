using AudioProcessorLibrary;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace TagUpdater;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void FolderName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Configure open file dialog box
        var dialog = new Microsoft.Win32.OpenFolderDialog();
        dialog.InitialDirectory = """C:\Recordings""";

        // Show open file dialog box
        bool? result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open document
            string folderName = dialog.FolderName;
            FolderName.Text = folderName;
            UpdateData(folderName);
        }
    }

    private void UpdateData(string folderName)
    {
        var files = Directory.GetFiles(folderName, "*.mp3");

        var firstTags = Tagger.GetFileTags(files.First());
        Artist.Text = firstTags.Artist;
        Album.Text = firstTags.Album;
        Year.Text = firstTags.Year.ToString();
        AlbumArtPath.Text = "";
        TrackNames.Text = $"{firstTags.Title}";

        foreach (var file in files.Skip(1))
        {
            var tag = Tagger.GetFileTags(file);
            TrackNames.Text += $"\n{tag.Title}";
        }
    }

    private void AlbumArtPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Configure open file dialog box
        var dialog = new Microsoft.Win32.OpenFileDialog();

        if (!string.IsNullOrEmpty(FolderName.Text))
            dialog.InitialDirectory = FolderName.Text;
        else
            dialog.InitialDirectory = """C:\Recordings""";

        dialog.FileName = ""; // Default file name

        // Show open file dialog box
        bool? result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            // Open document
            string fileName = dialog.FileName;
            AlbumArtPath.Text = fileName;
        }
    }

    private void Update_Click(object sender, RoutedEventArgs e)
    {
        var folderName = FolderName.Text;
        var files = Directory.GetFiles(folderName, "*.mp3");

        foreach (var file in files.Order())
        {
            TrackInfo info = Tagger.GetFileTags(file);
            info = info with {
                Artist = Artist.Text,
                AlbumArtist = Artist.Text,
                Album = Album.Text,
                Year = Int32.Parse(Year.Text),
                CoverArtPath = AlbumArtPath.Text,
            };
            Tagger.TagFile(file, info);
        }
        MessageBox.Show("Done");
    }

}