using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 组合控件 主要方便我们控制 造塔相关 UI的更新逻辑
/// </summary>
public class TowerBtn : MonoBehaviour
{
    public Image imgPic;
    public Text txtTip;
    public Text txtMoney;

    /// <summary>
    /// 初始化组合控件上的消息
    /// </summary>
    /// <param name="id">防御塔id</param>
    /// <param name="inputStr">操作键说明</param>
    public void InitInfo(int id, string inputStr)
    {
        TowerInfo info = GameDataManager.Instance.towerInfoList[id - 1];
        imgPic.sprite = Resources.Load<Sprite>(info.imgRes);
        txtMoney.text = "￥" + info.money;
        txtTip.text = inputStr;
        if (info.money > GameLevelManager.Instance.player.money)        //如果钱不够就显示当前金钱不足
            txtMoney.text = "金钱不足";
    }
}
