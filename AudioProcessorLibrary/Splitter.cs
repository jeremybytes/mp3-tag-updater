using NAudio.Wave;

namespace AudioProcessorLibrary;

public class Splitter
{
    public static string ExtractWAVTrack(string filePath, TimeSpan start, TimeSpan end, 
        TrackInfo info)
    {
        string inputPath = Path.GetDirectoryName(filePath)!;
        string outputFileName = $"""{info.TrackNumber:D2} {info.Title}.wav""";
        string outputFilePath = Path.Combine(inputPath,
            ReplaceForbiddenCharacters(info.Artist),
            ReplaceForbiddenCharacters(info.Album),
            ReplaceForbiddenCharacters(outputFileName));

        if (!Directory.Exists(Path.GetDirectoryName(outputFilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);

        TrimWavFile(filePath, outputFilePath, start, end);

        return outputFilePath;
    }

    private static string ReplaceForbiddenCharacters(string input)
    {
        // This does 9 "Replaces" on each string
        // (artist, album, track). There may be
        // a more efficient way of doing this.
        foreach (char c in """<>:"/\|?*""")
            input = input.Replace(c, '_');
        return input;
    }

    private static void TrimWavFile(string inPath, string outPath, TimeSpan startPoint, TimeSpan endPoint)
    {
        using WaveFileReader reader = new(inPath);
        using WaveFileWriter writer = new(outPath, reader.WaveFormat);
        double bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000d;

        int startPos = (int)(startPoint.TotalMilliseconds * bytesPerMillisecond);
        startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

        int endPos = (int)(endPoint.TotalMilliseconds * bytesPerMillisecond);
        endPos = endPos - endPos % reader.WaveFormat.BlockAlign;

        TrimWavFile(reader, writer, startPos, endPos);
    }

    private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
    {
        reader.Position = startPos;
        byte[] buffer = new byte[1024];
        while (reader.Position < endPos)
        {
            int bytesRequired = (int)(endPos - reader.Position);
            if (bytesRequired > 0)
            {
                int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                int bytesRead = reader.Read(buffer, 0, bytesToRead);
                if (bytesRead > 0)
                {
                    writer?.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
