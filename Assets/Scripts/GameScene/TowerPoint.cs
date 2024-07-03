using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPoint : MonoBehaviour
{
    private GameObject towerObj = null;         //造塔点关联的塔对象
    public TowerInfo nowtowerInfo = null;       //造塔点关联的塔的数据
    public List<int> chooseIDs;                 //可以建造的三个塔的ID的列表
    
    /// <summary>
    /// 建造塔的方法
    /// </summary>
    /// <param name="id">塔的id</param>
    public void CreateTower(int id)
    {
        TowerInfo info = GameDataManager.Instance.towerInfoList[id - 1];
        //如果钱不够，就不造塔
        if (info.money > GameLevelManager.Instance.player.money)
            return;
        //扣钱并造塔，若之前已经存在塔就删除之前的塔
        GameLevelManager.Instance.player.AddMoney(-info.money);
        if (towerObj != null)
        {
            Destroy(towerObj);
            towerObj = null;
        }
        //记录当前塔的数据，并更新游戏界面
        towerObj = Instantiate(Resources.Load<GameObject>(info.res), this.transform.position, Quaternion.identity);
        towerObj.GetComponent<TowerObject>().InitInfo(info);
        nowtowerInfo = info;
        if (nowtowerInfo.nextLev != 0)
        {
            UIManager.Instance.GetPanel<GamePanel>().UpdateSelTower(this);
        }
        else
        {
            UIManager.Instance.GetPanel<GamePanel>().UpdateSelTower(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //如果现在已经有塔了 就没赢必要再显示升级页面 或者造塔页面了
        if (nowtowerInfo != null && nowtowerInfo.nextLev == 0)
            return;
        UIManager.Instance.GetPanel<GamePanel>().UpdateSelTower(this);
    }

    private void OnTriggerExit(Collider other)
    {
        //如果不希望游戏界面下方的造塔界面显示，就直接传空
        UIManager.Instance.GetPanel<GamePanel>().UpdateSelTower(null);
    }


}
