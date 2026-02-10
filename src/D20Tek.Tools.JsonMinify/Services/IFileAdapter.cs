namespace D20Tek.Tools.JsonMinify.Services;

internal interface IFileAdapter
{
    bool Exists(string path);
    
    string ReadAllText(string path);

    void WriteAllText(string path, string contents);
}
