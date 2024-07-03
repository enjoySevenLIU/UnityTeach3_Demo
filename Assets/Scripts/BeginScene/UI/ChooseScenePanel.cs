using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChooseScenePanel : BasePanel
{
    public Button btnLeft;              //左切换按钮
    public Button btnRight;             //右切换按钮
    public Button btnStart;             //开始游戏
    public Button btnBack;              //返回上一页面
    public Text txtInfo;                //场景的文字描述
    public Image imgScene;              //场景的缩略图
    private int nowIndex;               //当前的数据索引
    private SceneInfo nowSceneInfo;     //记录当前选择的数据

    public override void Init()
    {
        btnLeft.onClick.AddListener(() =>
        {
            --nowIndex;
            if (nowIndex < 0)
                nowIndex = GameDataManager.Instance.sceneInfoList.Count - 1;
            ChangeScene();
        });
        btnRight.onClick.AddListener(() =>
        {
            ++nowIndex;
            if (nowIndex >= GameDataManager.Instance.sceneInfoList.Count)
                nowIndex = 0;
            ChangeScene();
        });

        btnStart.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ChooseScenePanel>();                           //隐藏当前面板
            AsyncOperation ao = SceneManager.LoadSceneAsync(nowSceneInfo.sceneName);    //异步切换场景，将关卡初始化交给彻底完成场景切换后再执行
            ao.completed += (obj) =>
            {
                GameLevelManager.Instance.InitInfo(nowSceneInfo);       //进行关卡初始化
            };
        });
        btnBack.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ChooseScenePanel>();
            UIManager.Instance.ShowPanel<ChooseHeroPanel>();    //切换到选角面板
        });
        ChangeScene();                                          //一打开面板 初始化时 就显示
    }

    /// <summary>
    /// 切换界面上显示的场景信息
    /// </summary>
    public void ChangeScene()
    {
        nowSceneInfo = GameDataManager.Instance.sceneInfoList[nowIndex];
        imgScene.sprite = Resources.Load<Sprite>(nowSceneInfo.imgRes);
        txtInfo.text = "名称：\n" + nowSceneInfo.name + "\n描述：\n" + nowSceneInfo.tips;
    }
}
