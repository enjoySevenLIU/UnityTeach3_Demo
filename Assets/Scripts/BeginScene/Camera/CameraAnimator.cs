using UnityEngine;
using UnityEngine.Events;

public class CameraAnimator : MonoBehaviour
{
    private Animator animator;
    private UnityAction overAction;     //用于记录动画播放完成后要做什么

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    /// <summary>
    /// 左转摄像机
    /// </summary>
    /// <param name="action">左转完成后要做什么</param>
    public void TurnLeft(UnityAction action)
    {
        animator.SetTrigger("Left");
        overAction = action;
    }

    /// <summary>
    /// 右转摄像机
    /// </summary>
    /// <param name="action">右转完成后要做什么</param>
    public void TurnRight(UnityAction action)
    {
        animator.SetTrigger("Right");
        overAction = action;
    }

    /// <summary>
    /// 当动画播放完后会调用外部传入的方法，执行完毕后清空
    /// </summary>
    public void PlayOver()
    {
        overAction?.Invoke();
        overAction = null;
    }

}
