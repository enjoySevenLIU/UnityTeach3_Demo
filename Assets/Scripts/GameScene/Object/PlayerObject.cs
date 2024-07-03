using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    private Animator animator;
    //玩家属性的初始化
    private int atk;                    //玩家攻击力
    public int money;                   //玩家金钱数
    private float roundSpeed = 180;     //旋转速度
    private bool isCrouch = false;      //是否蹲下
    private float nowWeight = 0f;       //当前蹲下层权重
    public Transform gunPoint;          //持枪对象才有的开火点

    LineRenderer line;                  //枪发出的射线
    public float width = 0.02f;         //射线宽度
    public float effectSeconds = 0.03f; //射线持续时间
    private bool isEffectDisplayed;     //射线是否在显示着
    private float nowTimes = 0f;        //射线显示时间

    void Start()
    {
        animator = this.GetComponent<Animator>();
        line = this.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.loop = false;
        }
    }

    /// <summary>
    /// 初始化玩家基础属性
    /// </summary>
    /// <param name="atk">攻击力</param>
    /// <param name="money">金钱数</param>
    public void InitPlayerInfo(int atk, int money)
    {
        this.atk = atk;         
        this.money = money;
        UpdateMoney();          //更新界面上钱的数量
    }

    //移动变化 动作变化
    void Update()
    {
        //移动动作的变化
        animator.SetFloat("VSpeed", Input.GetAxis("Vertical"));
        animator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));
        this.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * roundSpeed * Time.deltaTime);
        
        //滚动
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Roll");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouch = !isCrouch;   //切换是否蹲下
        }

        //开火
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Fire");
        }
        
        //蹲下动作的变化
        if (isCrouch)
        {
            if (nowWeight < 1f)
            {
                nowWeight = Mathf.Lerp(nowWeight, 1f, Time.deltaTime * 10);
                animator.SetLayerWeight(2, nowWeight);
            }
            else if (nowWeight > 1f)
                nowWeight = 1f;
        }
        else
        {
            if (nowWeight > 0f)
            {
                nowWeight = Mathf.Lerp(nowWeight, 0f, Time.deltaTime * 10);
                animator.SetLayerWeight(2, nowWeight);
            }
            else if (nowWeight < 1f)
                nowWeight = 0f;
        }

        if (isEffectDisplayed)
        {
            nowTimes -= Time.deltaTime;
            if (nowTimes <= 0f)
            {
                isEffectDisplayed = false;
                nowTimes = 0f;
                EffectDisabled();
            }
        }
    }

    //攻击动作的不同处理
    /// <summary>
    /// 专门用于处理到武器攻击动作的伤害检测事件
    /// </summary>
    public void KnifeEvent()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position + this.transform.forward + this.transform.up, 1,
                                                     1 << LayerMask.NameToLayer("Monster"));
        GameDataManager.Instance.PlaySound("Music/Knife");      //播放音效
        for (int i = 0; i < colliders.Length; i++)
        {
            //得到碰撞到的对象的怪物脚本，让其受伤
            MonsterObject monster = colliders[i].gameObject.GetComponent<MonsterObject>();
            if (monster != null && !monster.isDead)
                monster.Wound(this.atk);
        }
    }

    /// <summary>
    /// 专门用于处理到武器攻击动作的伤害检测事件
    /// </summary>
    public void ShootEvent()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Ray(gunPoint.position, this.transform.forward), 1000,
                                               1 << LayerMask.NameToLayer("Monster"));
        TrailingEffect(gunPoint.position, this.transform.forward, 1000);
        GameDataManager.Instance.PlaySound("Music/Gun");      //播放音效
        for (int i = 0; i < hits.Length; i++)
        {
            //得到对象上的怪物脚本，让其受伤
            MonsterObject monster = hits[i].collider.gameObject.GetComponent<MonsterObject>();
            if (monster != null)
            {
                GameObject effObj = Instantiate(Resources.Load<GameObject>(GameDataManager.Instance.nowSelRole.hitEff));
                effObj.transform.position = hits[i].point;
                effObj.transform.rotation = Quaternion.LookRotation(hits[i].normal);
                Destroy(effObj, 1);
                monster.Wound(this.atk);
            }  
        }
    }

    /// <summary>
    /// 封装更新钱数方法，减少代码量
    /// </summary>
    public void UpdateMoney()
    {
        //间接的更新界面上，钱的数量
        UIManager.Instance.GetPanel<GamePanel>().UpdataMoney(money);
    }

    /// <summary>
    /// 提供给外部加钱的方法
    /// </summary>
    /// <param name="money">加多少钱</param>
    public void AddMoney(int money)
    {
        this.money += money;
        UpdateMoney();
    }

    public void TrailingEffect(Vector3 startPos, Vector3 direction, float distance)
    {
        isEffectDisplayed = true;
        nowTimes = effectSeconds;
        line.startWidth = width;
        line.endWidth = width;
        line.startColor = Color.yellow;
        line.endColor = Color.red;
        line.positionCount = 2;
        line.SetPositions(new Vector3[]
        {
            startPos,
            startPos + direction * distance,
        });
    }

    public void EffectDisabled()
    {
        line.positionCount = 0;
    }
}
