using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 怪物对象类
/// </summary>
public class MonsterObject : MonoBehaviour
{
    public bool isDead = false;         //怪物是否死亡
    
    private Animator animator;          //怪物动画
    private NavMeshAgent agent;         //位移相关
    private MonsterInfo monsterInfo;    //怪物的基础数据
    private int hp;                     //当前血量
    private float frontTime;            //上一次攻击的时间
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = gameObject.AddComponent<Animator>();
    }

    /// <summary>
    /// 初始化怪物数据
    /// </summary>
    /// <param name="info">怪物数据</param>
    public void InitInfo(MonsterInfo info)
    {
        monsterInfo = info;                                     //加载基础数据，动画机文件
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(info.animator);
        hp = info.hp;                                           //要变化的当前血量
        agent.speed = agent.acceleration = info.moveSpeed;      //速度与加速度赋值，为了使对象没有明显的加速运动，就让加速度等于速度
        agent.angularSpeed = info.roundSpeed;                   //旋转速度
    }

    /// <summary>
    /// 怪物受伤方法
    /// </summary>
    /// <param name="dmg">受到伤害数</param>
    public void Wound(int dmg)
    {
        if (isDead) return;             //如果已经死亡，就不执行该方法
        hp -= dmg;                      //减少血量
        animator.SetTrigger("Wound");   //播放受伤动画
        if (hp <= 0)
        {
            Dead();                     //死亡
        }
        else
        {
            GameDataManager.Instance.PlaySound("Music/Wound");  //播放音效
        }
    }

    /// <summary>
    /// 怪物死亡方法
    /// </summary>
    public void Dead()
    {
        isDead = true;
        agent.isStopped = true;                                 //停止移动
        agent.enabled = false;
        animator.SetBool("Dead", true);                         //播放死亡动画
        GameDataManager.Instance.PlaySound("Music/dead");       //播放死亡音效
        GameLevelManager.Instance.player.AddMoney(150);         //加玩家的钱
    }

    /// <summary>
    /// 死亡动画播放完毕后，会调用的事件方法
    /// </summary>
    public void DeadEvent()
    {
        GameLevelManager.Instance.RemoveMonster(this);          //从列表中清除该怪物
        GameLevelManager.Instance.MonsterDeadNum++;
        Destroy(this.gameObject);
        if (GameLevelManager.Instance.CheckOver())
        {
            GameOverPanel panel = UIManager.Instance.ShowPanel<GameOverPanel>();
            panel.InitInfo(GameLevelManager.Instance.MonsterDeadNum * 2, true);
        }
    }

    /// <summary>
    /// 出生过后再移动的方法
    /// </summary>
    public void BornOver()
    {
        //移动——寻路组件
        agent.SetDestination(MainTowerObject.Instance.transform.position);
        animator.SetBool("Run", true);
    }

    
    private void Update()
    {
        if (isDead) return;                                             //若已死亡则不执行下方逻辑
        animator.SetBool("Run", agent.velocity != Vector3.zero);        //根据速度来决定动画播放什么
        //检测和目标点达到攻击条件时，就攻击
        if (Vector3.Distance(this.transform.position, MainTowerObject.Instance.transform.position) < 5 &&
            Time.time - frontTime >= monsterInfo.atkOffset)
        {
            frontTime = Time.time;          //记录这次攻击时的时间
            animator.SetTrigger("Atk");
        }
    }

    /// <summary>
    /// 攻击——伤害检测的方法
    /// </summary>
    public void AtkEvent()
    {
        //范围检测进行伤害判断
        Collider[] colliders = Physics.OverlapSphere(this.transform.position + this.transform.forward + this.transform.up, 1,
                                                     1 << LayerMask.NameToLayer("MainTower"));
        GameDataManager.Instance.PlaySound("Music/Eat");       //播放死亡音效
        for (int i = 0; i < colliders.Length; i++)
        {
            if (MainTowerObject.Instance.gameObject == colliders[i].gameObject)
            {
                MainTowerObject.Instance.Wound(monsterInfo.atk);    //满足所有条件后让保护区域受到伤害
            }
        }
    }

}
