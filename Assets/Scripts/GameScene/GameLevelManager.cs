using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏关卡管理器
/// </summary>
public class GameLevelManager
{
    private static GameLevelManager instance = new GameLevelManager();
    public static GameLevelManager Instance => instance;

    public PlayerObject player;                                     //当前场景上的玩家
    private List<MonsterPoint> points = new List<MonsterPoint>();   //所有的出怪点
    private int nowWaveNum = 0;                                     //记录当前 还有多少波怪物
    private int maxWaveNum = 0;                                     //记录 一共有多少波怪物
    //private int nowMonsterNum = 0;                                  //记录 当前场景上有多少个怪物
    public int MonsterDeadNum = 0;                                  //记录 当前场景上有多少怪物死亡
    private List<MonsterObject> monsterList = new();                //记录场景上怪物的列表

    private GameLevelManager()
    {

    }

    public void InitInfo(SceneInfo info)
    {
        //显示游戏界面
        UIManager.Instance.ShowPanel<GamePanel>();
        //切换到游戏场景时 我们需要动态的创建玩家，并让摄像机看向玩家
        RoleInfo roleInfo = GameDataManager.Instance.nowSelRole;
        Transform heroPos = GameObject.Find("HeroBornPos").transform;
        GameObject heroObj = GameObject.Instantiate(Resources.Load<GameObject>(roleInfo.res), heroPos.position, heroPos.rotation);
        player = heroObj.GetComponent<PlayerObject>();
        player.InitPlayerInfo(roleInfo.atk, info.money);        //初始化玩家的基础属性
        Camera.main.GetComponent<CameraMove>().SetTarget(heroObj.transform);
        MainTowerObject.Instance.UpdateHp(info.towerHp, info.towerHp);
    }

    //我们需要通过游戏管理器 来判断 游戏是否胜利
    //需要知道场景中是否还有怪物没有出 以及 场景中 是否还有没有死亡的怪物

    /// <summary>
    /// 用于记录出怪点的方法
    /// </summary>
    /// <param name="point">要记录的出怪点</param>
    public void AddMonsterPoint(MonsterPoint point)
    {
        points.Add(point);
    }

    /// <summary>
    /// 更新一共有多少波怪
    /// </summary>
    /// <param name="num">所有波数</param>
    public void UpdateMaxNum(int num)
    {
        maxWaveNum += num;
        nowWaveNum = maxWaveNum;
        UIManager.Instance.GetPanel<GamePanel>().UpdataWaveNum(nowWaveNum, maxWaveNum);
    }

    /// <summary>
    /// 更新当前一共有多少波怪
    /// </summary>
    /// <param name="num">剩余波数</param>
    public void ChangeNowWaveNum(int num)
    {
        nowWaveNum -= num;
        UIManager.Instance.GetPanel<GamePanel>().UpdataWaveNum(nowWaveNum, maxWaveNum);
    }

    /// <summary>
    /// 检测是否胜利，条件是所有出怪点是否出完怪以及所有怪物是否死亡
    /// </summary>
    /// <returns>所有出怪点是否出完怪以及所有怪物是否死亡</returns>
    public bool CheckOver()
    {
        for (int i = 0; i < points.Count; i++)
        {
            if (!points[i].CheckOver()) return false;
        }
        if (monsterList.Count > 0) return false;
        return true;
    }

    /// <summary>
    /// 记录向场景上添加的怪物
    /// </summary>
    /// <param name="obj">怪物</param>
    public void AddMonster(MonsterObject obj)
    {
        monsterList.Add(obj);
    }

    /// <summary>
    /// 从列表清除该怪物
    /// </summary>
    /// <param name="obj">怪物</param>
    public void RemoveMonster(MonsterObject obj)
    {
        monsterList.Remove(obj);
    }

    /// <summary>
    /// 遍历当前场景上的怪物列表的位置，计算与传入的位置的距离，若距离小于传入的距离范围，则返回该怪物
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="range">距离范围</param>
    /// <returns>在特定范围内的怪物，没有就返回null</returns>
    public MonsterObject FindMonster(Vector3 pos, int range)
    {
        for (int i = 0; i < monsterList.Count; i++)
        {
            if (monsterList[i] == null)
            {
                monsterList.RemoveAt(i);
                continue;
            }
            if (Vector3.Distance(pos, monsterList[i].transform.position) <= range &&
                !monsterList[i].isDead)
            {
                return monsterList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 遍历当前场景上的怪物列表的位置，计算与传入的位置的距离，返回所有距离小于传入的距离范围的怪物列表
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="range">距离范围</param>
    /// <returns>在特定范围内的所有怪物的列表</returns>
    public List<MonsterObject> FindMonsters(Vector3 pos, int range)
    {
        List<MonsterObject> list = new List<MonsterObject>();
        for (int i = 0; i < monsterList.Count; i++)
        {
            if (Vector3.Distance(pos, monsterList[i].transform.position) <= range &&
                !monsterList[i].isDead)
            {
                list.Add(monsterList[i]);
            }
        }
        return list;
    }

    /// <summary>
    /// 清空当前关卡记录的数据 避免影响下一次切换关卡
    /// </summary>
    public void ClearInfo()
    {
        points.Clear();
        monsterList.Clear();
        maxWaveNum = MonsterDeadNum = 0;
    }
}
