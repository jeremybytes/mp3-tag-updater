using System.Drawing;

namespace AudioProcessorLibrary;

public record TrackInfo(string? Artist, string? AlbumArtist, 
    string? Album, int? Year, string? Title, int? TrackNumber,
    bool HasCoverArt, Bitmap? CoverArt);
