using System;
using System.Collections.Generic;
using System.Configuration;

namespace WeeklyRobot.Service
{
    public static class Config
    {
        public static T Get<T>(string key)
        {
            var value = default(T);
            if (TryGet(key, out value))
            {
                return value;
            }
            Set(key, nameof(KeyNotFoundException));

            throw new KeyNotFoundException(key);
        }

        public static T TryGet<T>(string key, T defaultValue)
        {
            var value = default(T);
            if (TryGet(key, out value))
            {
                return value;
            }

            Set(key, defaultValue);

            return defaultValue;
        }

        public static bool TryGet<T>(string key, out T value)
        {
            value = default(T);
            try
            {
                if (ConfigurationManager.AppSettings[key] == null)
                    return false;
                var valueString = ConfigurationManager.AppSettings[key].ToString().Trim();
                if (!typeof(T).Name.Equals("string", StringComparison.OrdinalIgnoreCase) && typeof(T).IsClass)
                {
                    value = valueString.Trim().DeserializeObject<T>();
                }
                else
                {
                    value = (T)Convert.ChangeType(valueString.Trim(), typeof(T));
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"get\t{key}\t{e.Message.ToString()}");
                return false;
            }
        }

        public static void Set<T>(string key, T value)
        {
            var valueString = string.Empty;
            if (!typeof(T).Name.Equals("string", StringComparison.OrdinalIgnoreCase) && typeof(T).IsClass)
            {
                valueString = value.SerializeObject();
            }
            else
            {
                valueString = value.ToString();
            }


            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, valueString);
            }
            else
            {
                settings[key].Value = valueString;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
