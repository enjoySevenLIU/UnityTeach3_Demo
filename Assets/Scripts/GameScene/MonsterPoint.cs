using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 出怪点类
/// </summary>
public class MonsterPoint : MonoBehaviour
{
    public int maxWave;                 //怪物有多少波
    public int monsterNumOneWave;       //每波怪物有多少只
    private int nowNum;                 //记录还有多少怪物没有出
    public List<int> monsterIDList;     //怪物ID 允许有多个 这样就可以随机创建不同的怪物 更具有多样性
    private int nowID;                  //记录当前波要创建什么ID的怪物
    public float createOffsetTime;      //单只怪物创建的间隔时间
    public float delayTime;             //波与波之间的间隔时间
    public float firstDelayTime;        //第一波怪物创建的间隔时间

    // Start is called before the first frame update
    void Start()
    {
        Invoke("CreateWave", firstDelayTime);
        GameLevelManager.Instance.AddMonsterPoint(this);
        GameLevelManager.Instance.UpdateMaxNum(maxWave);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 开始创建一波的怪物
    /// </summary>
    private void CreateWave()
    {
        nowID = monsterIDList[Random.Range(0, monsterIDList.Count)];    //得到当前波怪物的ID是什么
        nowNum = monsterNumOneWave;                                     //当前波怪物有多少只
        CreateMonster();                                                //创建怪物
        --maxWave;                                                      //减少波数
        GameLevelManager.Instance.ChangeNowWaveNum(1);
    }

    /// <summary>
    /// 创建怪物
    /// </summary>
    private void CreateMonster()
    {
        //根据本波要出现的怪物ID，取出对应怪物数据，并实例化该怪物
        MonsterInfo info = GameDataManager.Instance.monsterInfoList[nowID - 1];
        GameObject obj = Instantiate(Resources.Load<GameObject>(info.res), this.transform.position, Quaternion.identity);
        MonsterObject monsterObj = obj.AddComponent<MonsterObject>();
        monsterObj.InitInfo(info);                              //初始化该怪物的信息
        GameLevelManager.Instance.AddMonster(monsterObj);       //向管理器记录该怪物
        --nowNum;                                               //当前波剩余怪物减一
        if (nowNum == 0)
        {
            //当本波怪物出完，而还有剩余波数时
            if (maxWave > 0)
                Invoke("CreateWave", delayTime);                //当还有怪物波数时，就按照设定好的时间执行创建一波的函数
        }
        else
        {
            Invoke("CreateMonster", createOffsetTime);          //如果当前波还有怪物，则延迟设定好的时间再执行一次本函数
        }
    }

    /// <summary>
    /// 检测出怪点怪物是否出完
    /// </summary>
    /// <returns>是否出完</returns>
    public bool CheckOver()
    {
        return nowNum == 0 && maxWave == 0;
    }

}
