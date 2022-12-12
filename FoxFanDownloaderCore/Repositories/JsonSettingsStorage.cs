using AutoMapper;
using Newtonsoft.Json;

namespace FoxFanDownloaderCore;

public class JsonSettingsStorage : ISettingsStorage
{
    private readonly string STORAGE_DIR;
    private readonly JsonSerializerSettings settings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented
    };

    public JsonSettingsStorage(string folder)
    {
        this.STORAGE_DIR = folder;
    }

    public CartoonModel[] LoadCartoons()
    {
        if (!Directory.Exists(STORAGE_DIR))
        {
            Directory.CreateDirectory(STORAGE_DIR);
        }

        var list = new List<CartoonModel>();

        var files = Directory.GetFiles(STORAGE_DIR, "*.json");
        foreach (var json in files)
        {
            CartoonModel cartoon = JsonConvert.DeserializeObject<CartoonModel>(File.ReadAllText(json), settings);
            list.Add(cartoon);
        }

        return list.ToArray();
    }

    public void SaveCartoon(CartoonModel cartoon)
    {
        if (!Directory.Exists(STORAGE_DIR))
        {
            Directory.CreateDirectory(STORAGE_DIR);
        }
        string localPath = Path.Combine(STORAGE_DIR, $"{cartoon.Name}.json");
        string jsonObject = JsonConvert.SerializeObject(cartoon, settings);
        File.WriteAllText(localPath, jsonObject);
    }
}