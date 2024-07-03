using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 专门用于管理数据的类
/// </summary>
public class GameDataManager
{
    private static GameDataManager instance = new GameDataManager();
    public static GameDataManager Instance => instance;

    public MusicData musicData;                 //音效相关数据
    public List<RoleInfo> roleInfoList;         //角色数据列表
    public PlayerData playerData;               //玩家相关数据
    public RoleInfo nowSelRole;                 //记录选择的角色数据，用于之后在游戏场景中创建
    public List<SceneInfo> sceneInfoList;       //所有的场景数据
    public List<MonsterInfo> monsterInfoList;   //所有的怪物数据
    public List<TowerInfo> towerInfoList;       //所有的防御塔数据

    private GameDataManager() 
    {
        //初始化读取默认数据
        musicData = JsonManager.Instance.LoadData<MusicData>("MusicData");
        roleInfoList = JsonManager.Instance.LoadData<List<RoleInfo>>("RoleInfo");
        playerData = JsonManager.Instance.LoadData<PlayerData>("PlayerData");
        sceneInfoList = JsonManager.Instance.LoadData<List<SceneInfo>>("SceneInfo");
        monsterInfoList = JsonManager.Instance.LoadData<List<MonsterInfo>>("MonsterInfo");
        towerInfoList = JsonManager.Instance.LoadData<List<TowerInfo>>("TowerInfo");
    }

    /// <summary>
    /// 存储音效数据
    /// </summary>
    public void SaveMusicData()
    {
        JsonManager.Instance.SaveData(musicData, "MusicData");
    }

    /// <summary>
    /// 存储玩家数据
    /// </summary>
    public void SavePlayerData()
    {
        JsonManager.Instance.SaveData(playerData, "PlayerData");
    }

    /// <summary>
    /// 播放音效方法
    /// </summary>
    /// <param name="resName">播放音效的路径</param>
    public void PlaySound(string resName)
    {
        GameObject musicObj = new GameObject();
        AudioSource a = musicObj.AddComponent<AudioSource>();
        a.clip = Resources.Load<AudioClip>(resName);
        a.volume = musicData.soundValue;
        a.mute = !musicData.soundOpen;
        a.Play();
        GameObject.Destroy(musicObj, 1);
    }
}
