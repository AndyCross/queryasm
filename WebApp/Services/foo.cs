using System;
using System.Configuration;

namespace TodoListWebApp.Services
{

    public static class CustomConfigurationManager
    {
        private static CustomAppSettings _appSettings = new CustomAppSettings();
        public static CustomAppSettings AppSettings { get { return _appSettings; } }
    }
    public class CustomAppSettings
    {
        const string BasePrefix = "APPSETTING_";
        private string _appPrefix;
        public string AppPrefix
        {
            get
            {
                return _appPrefix;
            }
            set
            {
                _appPrefix = value;
                if (!string.IsNullOrEmpty(_appPrefix) && !_appPrefix.EndsWith("_"))
                {
                    _appPrefix += "_";
                }
            }
        }

        public string this[string key]
        {
            get
            {
                string prefixedKey = string.IsNullOrEmpty(AppPrefix)
                                        ? key
                                        : AppPrefix + key;
                string value = Environment.GetEnvironmentVariable(BasePrefix + prefixedKey)
                                ?? Environment.GetEnvironmentVariable(prefixedKey)
                                ?? ConfigurationManager.AppSettings[prefixedKey]
                                ?? ConfigurationManager.AppSettings[key];
                return value;
            }
        }
    }
}