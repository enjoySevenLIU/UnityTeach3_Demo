using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 面板基类，所有要显示的面板都要继承它，实现基础的显隐（包含淡入淡出效果），初始化方法
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;            //专门用于控制面板透明度的组件
    private float alphaSpeed = 10;              //淡入淡出的速度
    private UnityAction hideCallBack = null;    //当隐藏完成后要执行的委托

    public bool isShow = false;                 //当前是显示还是隐藏状态，用于在Update()内控制淡入淡出

    protected virtual void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();                 //一开始获取面板上挂载的组件
        if (canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();  //如果为空，就为其添加该组件
    }

    protected virtual void Start()
    {
        Init();
    }

    /// <summary>
    /// 注册控件事件的方法 所有的面板都要实现它来为面板上各个组件初始化其监听事件
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// 淡入显示自己，以及显示自己后要做什么的方法
    /// </summary>
    public virtual void ShowMe()
    {
        canvasGroup.alpha = 0;      //显示后先设置透明度为0，便于接下来透明度在Update()内增加
        isShow = true;              //使Update()内淡入逻辑开始执行
    }

    /// <summary>
    /// 淡出隐藏自己，以及隐藏自己后要做什么的方法
    /// </summary>
    /// <param name="callBack">淡出效果完成后要执行的方法</param>
    public virtual void HideMe(UnityAction callBack)
    {
        canvasGroup.alpha = 1;
        isShow = false;
        hideCallBack = callBack;    //记录完成淡出后要执行的委托
    }

    protected virtual void Update()
    {
        //淡入
        if (isShow && canvasGroup.alpha != 1)                   //当处于显示状态且透明度不为1时，就让透明度不断加到1，实现淡入效果
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
                canvasGroup.alpha = 1;
        }
        //淡出
        else if (!isShow && canvasGroup.alpha != 0)             //当处于隐藏状态且透明度不为0时，就让透明度不断减到0，实现淡出效果，并执行外部传入的委托
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                hideCallBack?.Invoke();                         //淡出效果完成后执行通过HideMe()外部传入的委托
            }
        }
    }
}
