using System.Configuration;

namespace DDrop.Utility.SettingsRepository
{
    public class CommonApplicationSettingsProvider : ICommonApplicationSettingsProvider
    {
        /// <summary>
        /// Получает значение из настроек приложения и преобразует в требуемый тип
        /// </summary>
        /// <param name="settingKey"></param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSettingByKey<T>(string settingKey, T defaultValue = default(T))
        {
            return StringConvertationHelper.Convert(ConfigurationManager.AppSettings[settingKey], defaultValue);
        }
    }
}