using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI面板管理器，单例
/// </summary>
public class UIManager
{
    //单例模式基建
    private static UIManager instance = new UIManager();
    public static UIManager Instance => instance;
    //存储当前正在显示的面板的字典，每显示一个面板，都会将该面板添加到字典，隐藏面板时，通过该面板名直接获取该面板
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    private Transform canvasTrans;                          //场景中的Canvas对象 用于设置为面板的父对象

    private UIManager() 
    {
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvas.transform;             //通过Resources读取并实例化来获得Canvas的Transform
        GameObject.DontDestroyOnLoad(canvas);       //通过过场景不移除该对象 保证整个游戏过程有且仅有一个Canvas
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">要显示面板的类型</typeparam>
    /// <returns>显示的面板</returns>
    public T ShowPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;                  //我们需要保证泛型T的类型和面板预设体名字一样，就可以方便的使用
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;                //判断字典是否已经存在改变面板，若存在则直接返回该面板
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));
        panelObj.transform.SetParent(canvasTrans, false);   //把这个对象放到场景中的Canvas下面
        T panel = panelObj.GetComponent<T>();
        panelDic.Add(panelName, panel);                     //获取该panel对象的脚本将其放入字典内，再执行它的ShowMe方法，最后返回它
        panel.ShowMe();
        return panel;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">要隐藏面板的类型</typeparam>
    /// <param name="isFade">是否让面板淡出隐藏，默认淡出</param>
    public void HidePanel<T>(bool isFade = true) where T : BasePanel
    {
        string panelName = typeof(T).Name;                  //我们需要保证泛型T的类型和面板预设体名字一样，就可以方便的使用
        if (panelDic.ContainsKey(panelName))                //判断该面板是否正在显示
        {
            if (isFade)                                     //通过外部参数决定是否淡出隐藏
            {
                panelDic[panelName].HideMe(() =>            //淡出隐藏完毕后再删除该对象，并将其从显示面板的字典
                {
                    GameObject.Destroy(panelDic[panelName].gameObject);
                    panelDic.Remove(panelName);
                });
            }
            else                                            //不选择淡出而直接删除该对象，并将其从显示面板的字典上移除
            {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }            
        }
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">要获取的面板的类型</typeparam>
    /// <returns>要获取的面板，若该面板不存在返回null</returns>
    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))                //检查显示面板的字典里有没有该面板
            return panelDic[panelName] as T;
        return null;                                        //该面板没有显示，直接返回null
    }
}
