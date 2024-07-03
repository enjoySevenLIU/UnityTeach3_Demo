using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTowerObject : MonoBehaviour
{
    //能够被别人快速获取到位置
    private static MainTowerObject instance;
    public static MainTowerObject Instance => instance;

    private int hp;
    private int maxHp;      //血量相关
    private bool isDead;    //是否死亡

    private void Awake()
    {
        instance = this;
    }
    
    //更新血量
    public void UpdateHp(int hp, int maxHP)
    {
        this.hp = hp;
        this.maxHp = maxHP;
        UIManager.Instance.GetPanel<GamePanel>().UpdateTowerHp(hp, maxHP);
    }

    //自己受到伤害
    public void Wound(int dmg)
    {
        //如果保护区域已经被摧毁，就没有必要再减血
        if (isDead) return;
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();    //游戏结束
            panel.InitInfo(GameLevelManager.Instance.MonsterDeadNum, false);        //计算本局获得的金钱
        }
        UpdateHp(hp, maxHp);    //更新血量
    }

    private void OnDestroy()
    {
        instance = null;        //当过场景时，就销毁该单例，以保证安全
    }
}
