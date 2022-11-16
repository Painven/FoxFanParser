using AutoMapper;
using FoxFanDownloader.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FoxFanDownloader;

public interface ISettingsStorage
{
    public Cartoon[] LoadCartoons();
    void SaveCartoon(Cartoon cartoon);
}

public class JsonSettingsStorage : ISettingsStorage
{
    private readonly string STORAGE_DIR = Path.GetDirectoryName(Application.Current.GetType().Assembly.Location) + "\\storage";
    private readonly JsonSerializerSettings settings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented
    };
    private readonly IMapper mapper;

    public JsonSettingsStorage(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public Cartoon[] LoadCartoons()
    {
        if (!Directory.Exists(STORAGE_DIR))
        {
            Directory.CreateDirectory(STORAGE_DIR);
        }

        var list = new List<Cartoon>();

        var files = Directory.GetFiles(STORAGE_DIR, "*.json");
        foreach (var json in files)
        {
            CartoonDto cartoonDto = JsonConvert.DeserializeObject<CartoonDto>(File.ReadAllText(json), settings);
            list.Add(mapper.Map<Cartoon>(cartoonDto));
        }

        return list.ToArray();
    }

    public void SaveCartoon(Cartoon cartoon)
    {
        if (!Directory.Exists(STORAGE_DIR))
        {
            Directory.CreateDirectory(STORAGE_DIR);
        }
        string localPath = Path.Combine(STORAGE_DIR, $"{cartoon.Name}.json");
        CartoonDto dto = mapper.Map<CartoonDto>(cartoon);
        string jsonObject = JsonConvert.SerializeObject(dto, settings);
        File.WriteAllText(localPath, jsonObject);
    }
}


