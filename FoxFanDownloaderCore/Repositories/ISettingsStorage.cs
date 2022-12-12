using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxFanDownloaderCore;

public interface ISettingsStorage
{
    public CartoonModel[] LoadCartoons();
    void SaveCartoon(CartoonModel cartoon);
}
