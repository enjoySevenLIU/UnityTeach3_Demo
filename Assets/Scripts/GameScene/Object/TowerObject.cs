using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    public Transform head;                      //炮台头部 用于旋转 头部目标
    public Transform gunPoint;                  //用于释放攻击特效的位置
    private float roundSpeed = 40;              //炮台头部旋转速度
    private TowerInfo info;                     //炮台关联的数据
    private MonsterObject targetObj;            //当前要攻击的目标，用于单体攻击
    private List<MonsterObject> targetObjs;     //当前要攻击的目标群体，用于范围攻击
    private float nowTime;                      //用于计时的 用来判断攻击间隔时间
    private Vector3 monsterPos;                 //用于记录瞄准的怪物位置

    /// <summary>
    /// 初始化炮台相关数据
    /// </summary>
    /// <param name="info"></param>
    public void InitInfo(TowerInfo info)
    {
        this.info = info;
    }

    void Update()
    {
        switch (info.atkType)
        {
            //单体攻击逻辑
            case 1:
                //当不存在目标怪物，或目标怪物死亡，或超出距离就寻找进入范围的怪物
                if (targetObj == null ||
                    targetObj.isDead ||
                    Vector3.Distance(this.transform.position, targetObj.transform.position) > info.atkRange)
                {
                    targetObj = GameLevelManager.Instance.FindMonster(this.transform.position, info.atkRange);
                }
                if (targetObj == null)
                    return;                                             //若没有目标怪物，炮台就不应该旋转
                monsterPos = targetObj.transform.position;
                monsterPos.y = head.position.y;                         //使防御塔平视怪物，而不会盯着塔看
                head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(monsterPos - head.position), roundSpeed * Time.deltaTime);
                //判断 两对象之间的夹角 小于一定范围时 才能让目标受伤 并且攻击间隔条件要满足
                if (Vector3.Angle(head.forward, monsterPos - head.position) < 5 &&
                    Time.time - nowTime >= info.offsetTime)
                {
                    targetObj.Wound(info.atk);                          //让目标受伤
                    GameDataManager.Instance.PlaySound("Music/Tower");  //播放音效
                    GameObject effObj = Instantiate(Resources.Load<GameObject>(info.eff), gunPoint.position, gunPoint.rotation);
                    Destroy(effObj, 0.2f);                              //播放特效后延迟移除特效
                    nowTime = Time.time;
                }
                break;
            //群体攻击逻辑
            case 2:
                targetObjs = GameLevelManager.Instance.FindMonsters(this.transform.position, info.atkRange);
                //当有怪物且攻击间隔条件满足时，释放攻击
                if (targetObjs.Count > 0 &&
                    Time.time - nowTime >= info.offsetTime)
                {
                    GameObject effObj = Instantiate(Resources.Load<GameObject>(info.eff), head.position, head.rotation);
                    Destroy(effObj, 0.2f);                              //播放特效后延迟移除特效
                    //让目标们受伤
                    for (int i = 0; i < targetObjs.Count; i++)
                    {
                        targetObjs[i].Wound(info.atk);
                    }
                    nowTime = Time.time;
                }
                break;
        }
    }
}
