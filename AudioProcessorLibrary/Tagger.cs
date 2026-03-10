using Id3;
using Id3.Frames;
using System.Drawing;
using System.Drawing.Imaging;

namespace AudioProcessorLibrary;

public class Tagger
{
    public static TrackInfo? GetFileTags(string fileName)
    {
        using var mp3 = new Mp3(fileName, Mp3Permissions.Read);
        Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
        if (tag is null) return null;
        bool hasCoverArt = tag.Pictures.Count > 0;
        Bitmap? coverArt = null;
        if (hasCoverArt)
        {
            using MemoryStream stream = new();
            tag.Pictures.First().SaveImage(stream);
            using Bitmap temp = new Bitmap(stream);
            coverArt = new Bitmap(temp);
        }
        TrackInfo info = new(tag.Artists.Value.FirstOrDefault(),
            tag.Band, tag.Album, tag.Year.Value, tag.Title, tag.Track,
            hasCoverArt, coverArt);
        return info;
    }

    public static string TagFile(string fileName, TrackInfo info)
    {
        using Mp3 mp3 = new (fileName, Mp3Permissions.ReadWrite);
        Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);
        if (tag is null)
        {
            tag = new Id3Tag();
        }
        tag.Title = info.Title;
        tag.Track = info.TrackNumber;
        tag.Album = info.Album;
        tag.Artists.Value.Add(info.Artist);
        tag.Band = info.AlbumArtist;
        tag.Year = info.Year;
        if (info.CoverArt is not null)
        {
            tag.Pictures.Clear();
            PictureFrame cover = new();
            using MemoryStream stream = new();
            info.CoverArt.Save(stream, ImageFormat.Jpeg);
            cover.LoadImage(stream);
            tag.Pictures.Add(cover);
        }
        mp3.WriteTag(tag, Id3Version.V23, WriteConflictAction.Replace);
        return fileName;
    }
}
