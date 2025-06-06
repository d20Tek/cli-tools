using System.Text;

namespace D20Tek.Tools.CreateGuid.Commands;

internal static class FileSaver
{
    public static void WriteText(string outputFile, string data)
    {
        var directoryName = Path.GetDirectoryName(outputFile);
        if (directoryName != null)
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(outputFile, data, Encoding.ASCII);
    }
}
