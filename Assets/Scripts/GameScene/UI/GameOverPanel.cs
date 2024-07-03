using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    public Text txtTitle;
    public Text txtInfo;
    public Text txtMoney;
    public Button btnSure;

    public override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            //隐藏两个面板
            UIManager.Instance.HidePanel<GameOverPanel>();
            UIManager.Instance.HidePanel<GamePanel>();
            //清空当前关卡的数据
            GameLevelManager.Instance.ClearInfo();
            //切换场景
            SceneManager.LoadScene("BeginScene");
        });
    }

    public void InitInfo(int money, bool isWin)
    {
        txtTitle.text = isWin ? "胜利" : "失败";
        txtInfo.text = isWin ? "获得成功奖励" : "获得失败奖励";
        txtMoney.text = "￥" + money;
        //根据奖励 改变玩家数据
        GameDataManager.Instance.playerData.haveMoney += money;
        GameDataManager.Instance.SavePlayerData();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        Cursor.lockState = CursorLockMode.None;
    }
}
