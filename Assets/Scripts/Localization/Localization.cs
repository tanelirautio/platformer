using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization
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

    public static void Init()
    {
        CSVLoader csvLoader = new CSVLoader();
        bool ok = csvLoader.LoadCSV();

        localizedEN = csvLoader.GetDictionaryValues("en");
        localizedFI = csvLoader.GetDictionaryValues("fi");

        isInit = true;
    }

    public static string GetLocalizedValue(string key)
    {
        if(!isInit) { Init(); }

        string value = key;

        switch(language)
        {
            case Language.English:
                localizedEN.TryGetValue(key, out value);
                break;
            case Language.Finnish:
                localizedFI.TryGetValue(key, out value);
                break;
        }

        return value;
    }
}
