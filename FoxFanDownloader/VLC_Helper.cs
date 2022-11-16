using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static FoxFanDownloader.FoxFanParser;

namespace FoxFanDownloader;

public static class VLC_Helper
{
    

    public static async Task OpenVideo(PlayerJsonRoot data, bool transformSubtitlesToOneLine)
    {
        if (data == null) { return; }
        
        string exePath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
        string subtitleFileName = Path.GetFileName(data.subtitle);
        string workDirectory = Path.GetDirectoryName(App.Current.GetType().Assembly.Location) + "\\subtitles";
        string localSutitlesFile = Path.Combine(workDirectory, subtitleFileName);
        string command = $"-vvv \"{data.file}\" --sub-file \"{subtitleFileName}\"";

        await LoadSubtitles(workDirectory, data.subtitle, localSutitlesFile, transformSubtitlesToOneLine);

        var process = new ProcessStartInfo(exePath, command);
        process.WorkingDirectory = workDirectory;
        Process.Start(process);      
    }

    private static async Task LoadSubtitles(string dir, string remoteUri, string localPath, bool transformSubtitlesToOneLine)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        await (new WebClient()).DownloadFileTaskAsync(remoteUri, localPath);

        if (transformSubtitlesToOneLine)
        {        
            var sourceLines = File.ReadAllLines(localPath);
            var newLines = new List<string>();

            bool isPreviousLineHasData = false;
            foreach(var line in sourceLines)
            {
                if(line.Contains(" --> "))
                {
                    if (isPreviousLineHasData)
                    {
                        newLines.Add(string.Empty);
                    }
                    newLines.Add(line);
                    isPreviousLineHasData = false;
                }
                else if (string.IsNullOrEmpty(line))
                {
                    newLines.Add(string.Empty);
                    isPreviousLineHasData = false;
                }
                else
                {
                    if (isPreviousLineHasData)
                    {
                        newLines[newLines.Count - 1] = $"{newLines[newLines.Count - 1]} {line}";
                    }
                    else
                    {
                        newLines.Add(line);
                        isPreviousLineHasData = true;
                    }                   
                }               
            }

            await File.WriteAllLinesAsync(localPath, newLines);
        }
    }
}
