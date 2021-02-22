namespace DDrop.Utility.SettingsRepository
{
    /// <summary>
    /// Провайдер общих настроек для приложения (интерфейс к appSettings)
    /// </summary>
    public interface ICommonApplicationSettingsProvider
    {
        /// <summary>
        /// Получает значение из настроек приложения и преобразует в требуемый тип
        /// </summary>
        /// <param name="settingKey"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetSettingByKey<T>(string settingKey, T defaultValue = default(T));
    }
}