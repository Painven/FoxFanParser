namespace FoxFanDownloaderCore;

public partial class FoxFanParser
{
    public class PlayerJsonRoot
    {
        public string id { get; set; }
        public string file { get; set; }
        public string comment { get; set; }
        public string poster { get; set; }
        public string subtitle { get; set; }
    }
}