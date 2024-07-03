using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button btnStart;
    public Button btnSetting;
    public Button btnAbout;
    public Button btnQuit;

    public override void Init()
    {
        btnStart.onClick.AddListener(() =>
        {
            Camera.main.GetComponent<CameraAnimator>().TurnLeft(() =>
            {
                UIManager.Instance.ShowPanel<ChooseHeroPanel>();
            });
            UIManager.Instance.HidePanel<BeginPanel>();
        });
        btnSetting.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<SettingPanel>();   //显示设置界面
        });
        btnAbout.onClick.AddListener(() =>
        {
            string about = "本作品为跟课实践作品\n仅用于学习交流\n本作品所有美术素材都来自互联网";
            UIManager.Instance.ShowPanel<TipPanel>().ChangeInfo(about);   //显示关于界面
        });
        btnQuit.onClick.AddListener(() =>
        {
            Application.Quit();         //退出游戏
        });
    }
}
