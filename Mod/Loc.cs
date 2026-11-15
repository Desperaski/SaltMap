using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProjectTower.config;

namespace SaltMap
{
    internal static class Loc
    {
        private static readonly Dictionary<string, string> Strings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static string currentLang = "en";
        private static string modDirCache;

        public static void Init(string modDir)
        {
            modDirCache = modDir;
            string modLang = Settings.Get("modLang", "").Trim().ToLowerInvariant();

            if (!string.IsNullOrEmpty(modLang))
            {
                LoadLanguage(modLang);
            }
            else
            {
                LoadLanguage("en"); // Временный дефолт до чтения конфига
            }
        }

        public static void Reload()
        {
            // Если в SaltMap.ini жестко задан язык, не переопределяем его
            string modLang = Settings.Get("modLang", "").Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(modLang)) return;

            // Читаем ID языка напрямую из конфига игры (0-12)
            int langId = ConfigMgr.language;

            // Log.Info("loc: reload detected game language ID = " + langId);

            // Маппинг ID игры в имена наших файлов
            switch (langId)
            {
                case 1: LoadLanguage("fr"); break;
                case 2: LoadLanguage("de"); break;
                case 3: LoadLanguage("es"); break;
                case 4: LoadLanguage("it"); break;
                case 5: LoadLanguage("ja"); break;
                case 6: LoadLanguage("ko"); break;
                case 7: LoadLanguage("pt"); break;
                case 8: LoadLanguage("ru"); break;
                case 9: LoadLanguage("ru"); break;
                case 10: LoadLanguage("zh"); break;
                default: LoadLanguage("en"); break;
            }
        }

        private static void LoadLanguage(string langCode)
        {
            if (currentLang == langCode && Strings.Count > 0) return; // Уже загружен

            currentLang = langCode;
            Strings.Clear();
            string path = Path.Combine(modDirCache, "lang", currentLang + ".txt");
            if (!File.Exists(path))
            {
                path = Path.Combine(modDirCache, "lang", "en.txt");
            }

            if (File.Exists(path))
            {
                foreach (string line in File.ReadAllLines(path, Encoding.UTF8))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                    int eq = line.IndexOf('=');
                    if (eq > 0)
                    {
                        string key = line.Substring(0, eq).Trim();
                        string val = line.Substring(eq + 1).TrimStart().Replace("\\n", "\n");
                        Strings[key] = val;
                    }
                }
                Log.Info("loc: loaded " + currentLang + " from " + path);
            }
        }

        public static string Get(string key)
        {
            if (Strings.TryGetValue(key, out var val)) return val;
            if (key.StartsWith("REGION_")) return key.Substring(7);
            return key;
        }
    }
}