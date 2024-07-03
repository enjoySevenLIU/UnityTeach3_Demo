using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform target;            //摄像机要看向的目标对象
    public Vector3 offsetPos;           //摄像机相对目标对象 在xyz上的偏移位置
    public float bodyHeight;            //看向位置的y偏移值
    public float moveSpeed;             //移动速度
    public float rotationSpeed;         //旋转速度
    private Vector3 targetPos;          //计算后摄像机应该在的位置
    private Quaternion targetRotation;  //计算后摄像机应该看向的方向

    public void Start()
    {
        //如果一开始有观察对象，则直接移到目标点，并看向目标
        if (target == null)
            return;
        targetPos = target.position + target.forward * offsetPos.z;      //向后偏移
        targetPos += Vector3.up * offsetPos.y;                           //向上偏移
        targetPos += target.right * offsetPos.x;                         //左右偏移
        this.transform.position = targetPos;
        targetRotation = Quaternion.LookRotation(target.position + Vector3.up * bodyHeight - this.transform.position);
        this.transform.rotation = targetRotation;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;
        targetPos = target.position + target.forward * offsetPos.z;      //向后偏移
        targetPos += Vector3.up * offsetPos.y;                           //向上偏移
        targetPos += target.right * offsetPos.x;                         //左右偏移
        //插值运算，使摄像机位置不停向目标点靠拢
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, moveSpeed * Time.deltaTime);
        //旋转的计算，得到最终要看向某个点时的四元数，并让摄像机不断的转过去
        targetRotation = Quaternion.LookRotation(target.position + Vector3.up * bodyHeight - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// 设置摄像机看向的目标对象
    /// </summary>
    /// <param name="player">要看向的玩家</param>
    public void SetTarget(Transform player)
    {
        target = player;
    }
}
