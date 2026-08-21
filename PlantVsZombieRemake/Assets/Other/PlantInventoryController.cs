using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlantInventoryController : MonoBehaviour
{
    [Header("植物预制体列表")]
    [SerializeField] private List<GameObject> allPlants = new List<GameObject>();// 存储所有植物预制体的列表
    private List<GameObject> currentPlant = new List<GameObject>();// 存储当前卡槽中的植物

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TogglePlant(0);//按下数字键1，添加至第一个植物卡槽

        if (Input.GetKeyDown(KeyCode.Alpha2)) TogglePlant(1);//按下数字键2，添加至第二个植物卡槽

        if (Input.GetKeyDown(KeyCode.Alpha3)) TogglePlant(2);//按下数字键3，添加至第三个植物卡槽

        if (Input.GetKeyDown(KeyCode.Alpha4)) TogglePlant(3);//按下数字键4，添加至第四个植物卡槽

        if (Input.GetKeyDown(KeyCode.Alpha5)) TogglePlant(4);//按下数字键5，添加至第五个植物卡槽

        if (Input.GetKeyDown(KeyCode.O)) OutSlot();//按下字母键O，输出当前卡槽植物
    }

    /// <summary>
    /// 对植物卡槽进行操作
    /// </summary>
    /// <param name="index"></param>
    public void TogglePlant(int index)
    {
        //防止输入的索引越界
        if (index < 0 || index >= allPlants.Count)
        {
            Debug.LogWarning($"{index}索引越界！");
            return;
        }

        GameObject plant = allPlants[index];

        //防止添加空的植物预制体到当前植物列表中
        if (plant == null)
        {
            Debug.LogWarning($"植物库第{index + 1}项为空!");
            return;
        }

        //对卡槽进行添加或移除操作
        if (!currentPlant.Contains(plant))
        {
            currentPlant.Add(plant);
            Debug.Log($"已将{plant.name}添加到当前植物列表中 | 目前卡槽长度: {currentPlant.Count}");

        }
        else
        {
            currentPlant.Remove(plant);
            Debug.Log($"已将{plant.name}从当前植物列表中移除 | 目前卡槽长度: {currentPlant.Count}");
        }
    }

    /// <summary>
    /// 开发者：输出当前卡槽植物
    /// </summary>
    public void OutSlot()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=============当前卡槽============");
        if(currentPlant.Count == 0)
        {
            sb.AppendLine("这里啥都没有~快去装备植物吧");
        }
        else
        {
            for(int i = 0; i < currentPlant.Count; i++)
            {
                sb.AppendLine($"槽位{i + 1} : {currentPlant[i].name}");
            }
            sb.AppendLine($"当前共{currentPlant.Count}株植物被选中");
        }
        sb.AppendLine("=================================");

        Debug.Log(sb.ToString());
    }

    public GameObject GetPlantByIndex(int index)
    {
        if (index >= 0 && index < currentPlant.Count)
        {
            return currentPlant[index];
        }
        return null;
    }
}
