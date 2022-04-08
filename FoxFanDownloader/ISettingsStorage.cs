using Newtonsoft.Json;
using System.IO;

namespace FoxFanDownloader;

public interface ISettingsStorage
{
    SettingsRoot GetSettings();
    void SaveSettings(SettingsRoot settings);
}

public class JsonSettingsStorage : ISettingsStorage
{
    private readonly string fileName;

    public JsonSettingsStorage(string fileName)
    {
        this.fileName = fileName;
    }
    public SettingsRoot GetSettings()
    {
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            SettingsRoot settings = JsonConvert.DeserializeObject<SettingsRoot>(json);
            return settings;
        }
        return new SettingsRoot();
    }

    public void SaveSettings(SettingsRoot settings)
    {
        string json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(fileName, json);
    }
}

public class SettingsRoot
{
    public string SelectedMultfilmName { get; set; }
    public string SelectedSeasonNumber { get; set; }
    public bool IsOneLineSubtitles { get; set; }
}

