using System.Collections.Generic;

/// <summary>
/// 玩家数据类
/// </summary>
public class PlayerData
{
    public int haveMoney = 300;                       //当前所拥有的游戏币
    public List<int> buyHero = new List<int>();     //当前解锁了哪些角色
}
