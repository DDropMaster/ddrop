namespace DDrop.Utility.SettingsRepository
{
    public interface ISettingsRepository
    {
        string SubstanceCatalogUrl { get; }

        string SubstanceCatalogApiKey { get; }
    }
}