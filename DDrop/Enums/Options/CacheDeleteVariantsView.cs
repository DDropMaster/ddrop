using System.ComponentModel;

namespace DDrop.Enums.Options
{
    public enum CacheDeleteVariantsView
    {
        [Description("Никогда")] Never,
        [Description("При запуске")] OnLaunch,
    }
}