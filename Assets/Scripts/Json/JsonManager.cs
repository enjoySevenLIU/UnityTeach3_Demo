using System.IO;
using UnityEngine;
using LitJson;
using System;

public enum JsonType
{
    JsonUtlity,
    LitJson,
}

/// <summary>
/// Json数据管理类 主要用于进行Json的序列化和反序列化 存储到硬盘或者从硬盘中读取
/// </summary>
public class JsonManager
{
    private static JsonManager instance = new JsonManager();
    public static JsonManager Instance => instance;
    private JsonManager() { }

    /// <summary>
    /// 保存数据到Json文件中
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <param name="fileName">文件名</param>
    /// <param name="type">读取模式</param>
    public void SaveData(object data, string fileName, JsonType type = JsonType.LitJson)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        string jsonStr = "";
        switch (type)
        {
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);                
                break;
            case JsonType.JsonUtlity:
                jsonStr = JsonUtility.ToJson(data);
                break;
        }
        File.WriteAllText(path, jsonStr);
    }

    /// <summary>
    /// 从Json文件中读取数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="fileName">文件名</param>
    /// <param name="type">读取模式</param>
    /// <returns>数据对象</returns>
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new()
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        if (!File.Exists(path))
            path = Application.streamingAssetsPath + "/" + fileName + ".json";
        if (!File.Exists(path))
            return new T();
        string jsonStr = File.ReadAllText(path);
        T data = default;
        switch (type)
        {
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(jsonStr);
                break;
            case JsonType.JsonUtlity:
                data = JsonUtility.FromJson<T>(jsonStr);
                break;
        }
        return data;
    }
}
