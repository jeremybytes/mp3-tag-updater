using Id3;
using Id3.Frames;

namespace AudioProcessorLibrary;

public class Tagger
{
    public static TrackInfo GetFileTags(string fileName)
    {
        using var mp3 = new Mp3(fileName, Mp3Permissions.Read);
        Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
        bool hasCoverArt = tag.Pictures.Count > 0;
        TrackInfo info = new(tag.Artists.Value.FirstOrDefault(),
            tag.Band, tag.Album, tag.Year.Value, tag.Title, tag.Track,
            hasCoverArt, null);
        return info;
    }

    public static string TagFile(string fileName, TrackInfo info)
    {
        using Mp3 mp3 = new (fileName, Mp3Permissions.ReadWrite);
        Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
        tag.Title = info.Title;
        tag.Track = info.TrackNumber;
        tag.Album = info.Album;
        tag.Artists.Value.Add(info.Artist);
        tag.Band = info.AlbumArtist;
        tag.Year = info.Year;
        if (File.Exists(info.CoverArtPath))
        {
            tag.Pictures.Clear();
            PictureFrame cover = new();
            cover.LoadImage(info.CoverArtPath);
            tag.Pictures.Add(cover);
        }
        mp3.WriteTag(tag, WriteConflictAction.Replace);
        return fileName;
    }
}
