using NAudio.Wave;

namespace AudioProcessorLibrary;

public class MP3Converter
{
    public static string ConvertFile(string inputFilePath)
    {
        string inputPath = Path.GetDirectoryName(inputFilePath)!;
        string inputFileName = Path.GetFileNameWithoutExtension(inputFilePath);
        string outputFilePath = Path.Combine(inputPath, $"""{inputFileName}.mp3""");

        using (var reader = new WaveFileReader(inputFilePath))
        {
            MediaFoundationEncoder.EncodeToMp3(reader,
                    outputFilePath, 48000);
        }

        return outputFilePath;
    }
}
