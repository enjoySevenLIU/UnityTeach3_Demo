using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 选择英雄面板
/// </summary>
public class ChooseHeroPanel : BasePanel
{
    public Button btnLeft;          //左切换键
    public Button btnRight;         //右切换键
    public Button btnUnLock;        //购买按钮
    public Text txtUnLock;          //购买按钮文本
    public Button btnStart;         //开始键
    public Button btnBack;          //返回键
    public Text txtMoney;           //左上角拥有的钱
    public Text txtName;            //角色信息
    private Transform heroPos;      //英雄预设体需要创建在的位置
    private GameObject heroObj;     //当前场景上显示的角色对象
    private RoleInfo nowRoleData;   //当前场景上显示的角色数据
    private int nowIndex;           //当前使用数据的索引

    public override void Init()
    {
        heroPos = GameObject.Find("HeroPos").transform;
        txtMoney.text = GameDataManager.Instance.playerData.haveMoney.ToString();   //更新左上角玩家拥有的钱
        btnLeft.onClick.AddListener(() =>
        {
            --nowIndex;
            if (nowIndex < 0)
                nowIndex = GameDataManager.Instance.roleInfoList.Count - 1;
            ChangeHero();   //模型的更新
        });
        btnRight.onClick.AddListener(() =>
        {
            ++nowIndex;
            if (nowIndex >= GameDataManager.Instance.roleInfoList.Count)
                nowIndex = 0;
            ChangeHero();   //模型的更新
        });

        btnUnLock.onClick.AddListener(() =>
        {
            PlayerData data = GameDataManager.Instance.playerData;                  //获取玩家数据
            if (data.haveMoney >= nowRoleData.lockMoney)                            //检查玩家数据的钱是否足够购买该角色，若足够，则进入购买逻辑
            {
                data.haveMoney -= nowRoleData.lockMoney;                            //玩家数据的钱减去对应购买角色的价格
                txtMoney.text = data.haveMoney.ToString();                          //更新界面显示
                data.buyHero.Add(nowRoleData.id);                                   //将购买的角色的id记录到玩家数据内
                GameDataManager.Instance.SavePlayerData();                          //保存购买逻辑执行结果
                UpdateLockBtn();                                                    //更新按钮显示
                UIManager.Instance.ShowPanel<TipPanel>().ChangeInfo("购买成功");    //提示面板 显示购买成功
                
            }
            else
            {
                UIManager.Instance.ShowPanel<TipPanel>().ChangeInfo("你的金钱不足");  //不够，则提示金钱不足
            }
        });

        btnStart.onClick.AddListener(() =>
        {
            GameDataManager.Instance.nowSelRole = nowRoleData;      //记录当前选择的角色
            UIManager.Instance.HidePanel<ChooseHeroPanel>();
            UIManager.Instance.ShowPanel<ChooseScenePanel>();       //切换为场景选择界面
        });
        btnBack.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ChooseHeroPanel>();
            Camera.main.GetComponent<CameraAnimator>().TurnRight(() =>
            {
                UIManager.Instance.ShowPanel<BeginPanel>();         //先隐藏自己再右转摄像头，完成后显示开始面板
            });
        });
        ChangeHero();   //更新默认的模型显示
    }

    public override void HideMe(UnityAction callBack)
    {
        base.HideMe(callBack);
        if (heroObj != null)
        {
            DestroyImmediate(heroObj);      //隐藏自己时，要先删除当前显示的3D模型对象
            heroObj = null;
        }
    }

    /// <summary>
    /// 更新场景上要显示的模型
    /// </summary>
    private void ChangeHero()
    {
        if (heroObj != null)
        {
            Destroy(heroObj);
            heroObj = null;
        }
        nowRoleData = GameDataManager.Instance.roleInfoList[nowIndex];          //根据索引值取出当前的角色数据
        heroObj = Instantiate(Resources.Load<GameObject>(nowRoleData.res),
                              heroPos.position,
                              heroPos.rotation);                                //根据角色数据在场景上实例化该对象并存储
        Destroy(heroObj.GetComponent<PlayerObject>());                          //在开始场景不需要操控角色，因此删除该对象上的控制脚本
        txtName.text = nowRoleData.tips;
        UpdateLockBtn();
    }

    /// <summary>
    /// 更新解锁按钮显示情况
    /// </summary>
    private void UpdateLockBtn()
    {
        //如果该角色 需要解锁 并且没有解锁的话，就应该显示解锁按钮
        if (nowRoleData.lockMoney > 0 && !GameDataManager.Instance.playerData.buyHero.Contains(nowRoleData.id))
        {
            btnUnLock.gameObject.SetActive(true);               //更新解锁按钮显示，并更新上面的钱
            txtUnLock.text = "￥" + nowRoleData.lockMoney;
            btnStart.gameObject.SetActive(false);               //隐藏开始按钮 因为该角色没有解锁
        }
        else
        {
            btnUnLock.gameObject.SetActive(false);              //隐藏解锁按钮
            btnStart.gameObject.SetActive(true);                //显示开始按钮，可以使用该角色开始游戏
        }
    }

}
