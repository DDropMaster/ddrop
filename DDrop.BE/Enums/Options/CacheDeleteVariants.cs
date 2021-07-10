using System.ComponentModel;

namespace DDrop.BE.Enums.Options
{
    public enum CacheDeleteVariants
    {
        [Description("Никогда")] Never,
        [Description("При запуске")] OnLaunch,
    }
}