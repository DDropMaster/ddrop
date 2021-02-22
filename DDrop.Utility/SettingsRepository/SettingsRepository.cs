namespace DDrop.Utility.SettingsRepository
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly ICommonApplicationSettingsProvider _commonApplicationSettingsProvider;

        public SettingsRepository(ICommonApplicationSettingsProvider commonApplicationSettingsProvider)
        {
            _commonApplicationSettingsProvider = commonApplicationSettingsProvider;
        }

        public string SubstanceCatalogUrl => _commonApplicationSettingsProvider.GetSettingByKey<string>("SubstanceCatalogUrl");
        public string SubstanceCatalogApiKey => _commonApplicationSettingsProvider.GetSettingByKey<string>("SubstanceCatalogApiKey");
    }
}