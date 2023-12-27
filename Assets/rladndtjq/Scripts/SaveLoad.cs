using UnityEngine;

public static class SaveLoad
{
    public static void Save(string key, object obj)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(obj));
        PlayerPrefs.Save();
    }

    public static T Load<T>(string key)
    {
        return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
    }

    public static void Load(string key, object overload)
    {
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), overload);
    }
}