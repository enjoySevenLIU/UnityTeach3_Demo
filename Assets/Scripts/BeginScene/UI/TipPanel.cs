using UnityEngine.UI;

/// <summary>
/// 提示面板
/// </summary>
public class TipPanel : BasePanel
{
    public Text txtInfo;
    public Button btnSure;

    public override void Init()
    {
        btnSure.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<TipPanel>();
        });
    }

    /// <summary>
    /// 改变提示内容的方法
    /// </summary>
    /// <param name="str"></param>
    public void ChangeInfo(string str)
    {
        txtInfo.text = str;
    }
}
