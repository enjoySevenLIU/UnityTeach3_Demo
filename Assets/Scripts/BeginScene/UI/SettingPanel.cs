using UnityEngine.UI;

/// <summary>
/// 设置面板
/// </summary>
public class SettingPanel : BasePanel
{
    public Button btnClose;
    public Toggle togMusic;
    public Toggle togSound;
    public Slider sliderMusic;
    public Slider sliderSound;

    public override void Init()
    {
        //初始化面板各控件显示内容，根据本地存储的设置数据来初始化
        MusicData data = GameDataManager.Instance.musicData;
        togMusic.isOn = data.musicOpen;
        togSound.isOn = data.soundOpen;
        sliderMusic.value = data.musicValue;
        sliderSound.value = data.soundValue;

        //初始化面板各控件方法
        btnClose.onClick.AddListener(() =>
        {
            GameDataManager.Instance.SaveMusicData();           //为了节约性能，只有当设置完成后 关闭面板时 才会去记录数据到硬盘上
            UIManager.Instance.HidePanel<SettingPanel>();       //隐藏设置面板
        });

        togMusic.onValueChanged.AddListener((b) =>
        {
            BKMusic.Instance.SetIsOpen(b);                      //开关背景音乐
            GameDataManager.Instance.musicData.musicOpen = b;   //记录开关的数据
        });
        togSound.onValueChanged.AddListener((b) =>
        {
            GameDataManager.Instance.musicData.musicOpen = b;   //记录开关的数据
        });

        sliderMusic.onValueChanged.AddListener((v) =>
        {
            BKMusic.Instance.ChangeValue(v);                    //设置音乐大小
            GameDataManager.Instance.musicData.musicValue = v;  //记录音乐大小的数据
        });
        sliderSound.onValueChanged.AddListener((v) =>
        {
            GameDataManager.Instance.musicData.soundValue = v;  //记录音效大小的数据
        });

    }
}
