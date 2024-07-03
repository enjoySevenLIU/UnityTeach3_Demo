using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 游戏界面UI
/// </summary>
public class GamePanel : BasePanel
{
    public Image imgHP;
    public Text txtHP;
    public Text txtWave;
    public Text txtMoney;
    public float hpW = 500;                                     //Hp的初始宽 可以在外面去控制它
    public Button btnQuit;
    public Transform botTrans;                                  //下方造塔组合控件的父对象，主要用于控制显隐
    public List<TowerBtn> towerBtns = new List<TowerBtn>();     //管理3个复合控件
    private TowerPoint nowSelTowerPoint;                        //当前进入的造塔点
    private bool checkInput;                                    //标识是否检测输入


    public override void Init()
    {
        btnQuit.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<GamePanel>();          //隐藏界面
            SceneManager.LoadScene("BeginScene");               //读取开始场景
        });

        botTrans.gameObject.SetActive(false);                   //一开始先隐藏下方的UI
        Cursor.lockState = CursorLockMode.Confined;             //锁定鼠标
    }

    /// <summary>
    /// 更新安全区域血量方法
    /// </summary>
    /// <param name="hp">当前血量</param>
    /// <param name="maxHP">最大血量</param>
    public void UpdateTowerHp(int hp, int maxHP)
    {
        txtHP.text = hp + "/" + maxHP;
        (imgHP.transform as RectTransform).sizeDelta = new Vector2((float)hp / maxHP * hpW, 25);    //更新血条长度和文字
    }

    /// <summary>
    /// 更新剩余波数
    /// </summary>
    /// <param name="nowNum">当前波数</param>
    /// <param name="maxNum">最大波数</param>
    public void UpdataWaveNum(int nowNum, int maxNum)
    {
        txtWave.text = nowNum + "/" + maxNum;
    }

    /// <summary>
    /// 更新金钱数量
    /// </summary>
    /// <param name="money">当前金钱数</param>
    public void UpdataMoney(int money)
    {
        txtMoney.text = money.ToString();
    }

    /// <summary>
    /// 当前选中造塔点界面的一些变化
    /// </summary>
    public void UpdateSelTower(TowerPoint point)
    {
        //根据造塔点的消息 决定 界面上的显示内容
        nowSelTowerPoint = point;
            
        //若为空，则不显示
        if (nowSelTowerPoint == null)
        {
            checkInput = false;
            botTrans.gameObject.SetActive(false);
        }
        //若不为空，则显示
        else
        {
            checkInput = true;
            botTrans.gameObject.SetActive(true);
            //如果造过塔，就显示可选择建造的塔
            if (nowSelTowerPoint.nowtowerInfo == null)
            {
                for (int i = 0; i < towerBtns.Count; i++)
                {
                    towerBtns[i].gameObject.SetActive(true);
                    towerBtns[i].InitInfo(nowSelTowerPoint.chooseIDs[i], "数字键" + (i + 1));
                }
            }
            //如果没造过塔，就显示下一级可以升级的塔
            else
            {
                for (int i = 0; i < towerBtns.Count; i++)
                {
                    towerBtns[i].gameObject.SetActive(false);
                }
                towerBtns[1].gameObject.SetActive(true);
                towerBtns[1].InitInfo(nowSelTowerPoint.nowtowerInfo.nextLev, "数字键1");
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        //如果没有显示选择建塔的UI,就不检测输入
        if (!checkInput) return;
        //如果没有造过塔 那么就检测数字键1，2，3按钮去建造塔
        if (nowSelTowerPoint.nowtowerInfo == null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                nowSelTowerPoint.CreateTower(nowSelTowerPoint.chooseIDs[0]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                nowSelTowerPoint.CreateTower(nowSelTowerPoint.chooseIDs[1]);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                nowSelTowerPoint.CreateTower(nowSelTowerPoint.chooseIDs[2]);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                nowSelTowerPoint.CreateTower(nowSelTowerPoint.nowtowerInfo.nextLev);
            }
        }
    }
}
