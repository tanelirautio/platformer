using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class LocalizationManager : MonoBehaviour
    {
        public enum Language
        {
            English,
            Finnish
        }

        //public static Language language = Language.English;
        public static Language language = Language.Finnish;

        private static Dictionary<string, string> localizedEN;
        private static Dictionary<string, string> localizedFI;

        public static bool isInit = false;

        static List<Dictionary<string, object>> data;

        public static void Init()
        {
            data = CSVReader.Read("localization");
            localizedEN = GetDictionaryValues("en");
            localizedFI = GetDictionaryValues("fi");

            isInit = true;
        }

        public static Dictionary<string, string> GetDictionaryValues(string attributeId)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            for (int i = 0; i < data.Count; i++)
            {
                var dict = data[i];
                string key = (string)dict["key"];
                string value = (string)dict[attributeId];
                dictionary.Add(key, value);
            }

            return dictionary;
        }


        public static string GetLocalizedValue(string key)
        {
            if (!isInit) { Init(); }

            string value = key;

            switch (language)
            {
                case Language.English:
                    localizedEN.TryGetValue(key, out value);
                    break;
                case Language.Finnish:
                    localizedFI.TryGetValue(key, out value);
                    break;
            }

            //print("returning value: " + value);
            return value;
        }
    }
}
