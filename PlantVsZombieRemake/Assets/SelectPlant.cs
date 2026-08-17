using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPlantController : MonoBehaviour
{
    public static SelectPlantController instance { get; private set; } //单例模式

    private PlantMove currentPlant = null; //当前选中的植物

    /// <summary>
    /// 给instance赋值，如果已经有了instance就销毁当前对象
    /// </summary>
    public void Awake()
    {
        // 修复单例赋值逻辑：若没有实例则设为当前，否则销毁当前对象
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SelectPlant(PlantMove plant)
    {
        if (currentPlant == plant)
        {
            // 二次点击同一植物 -> 取消选中
            DeselectPlant();
            return;
        }

        // 取消之前的选中（若有）
        if (currentPlant != null)
        {
            currentPlant.SetSelected(false);
        }

        // 设置新的选中
        currentPlant = plant;
        if (currentPlant != null)
        {
            currentPlant.SetSelected(true);
        }
    }

    public void DeselectPlant()
    {
        if (currentPlant != null)
        {
            currentPlant.SetSelected(false);
            currentPlant = null;
        }
    }

    public PlantMove GetSelectPlant()
    {
        return currentPlant;
    }
}