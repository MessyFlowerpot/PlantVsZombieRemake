using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CellHighLight : MonoBehaviour
{
    private SpriteRenderer sr;// 记录格子精灵渲染器组件
    private Color originalColor;// 记录格子原始颜色
    private bool isMouseEnter = false;// 记录鼠标是否进入格子

    // 表示格子是否被植物占用（由外部设置/查询）
    public bool isHavingPlant = false;

    // 记录当前格子上的植物引用，种植时/移动结束时由 PlantMove 设置
    public PlantMove plantOnCell = null;

    void Start()
    {
        // 获取格子精灵渲染器组件并记录原始颜色
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning($"{nameof(CellHighLight)}: 未找到 SpriteRenderer，脚本将被禁用。对象：{gameObject.name}");
            enabled = false;
            return;
        }

        originalColor = sr.color;

        SetAlpha(0f);// 设置初始透明度为0
    }

    /// <summary>
    /// 设置格子透明度
    /// </summary>
    /// <param name="alpha"></param>
    void SetAlpha(float alpha)
    {
        if (sr == null) return;

        Color newColor = originalColor;
        newColor.a = alpha;
        sr.color = newColor;
    }

    /// <summary>
    /// 当鼠标进入格子时，恢复原始颜色
    /// </summary>
    void OnMouseEnter()
    {
        if (sr == null) return;
        sr.color = originalColor;
        isMouseEnter = true;
    }

    /// <summary>
    /// 当鼠标离开格子时，设置透明度为0
    /// </summary>
    void OnMouseExit()
    {
        if (sr == null) return;
        SetAlpha(0f);
        isMouseEnter = false;
    }

    /// <summary>
    /// 当鼠标点击格子时：
    /// - 若有已选植物：优先尝试移动该植物至此格（移动成功则不再种植）
    /// - 若无已选植物且 isHavingPlant == false：执行种植
    /// - 若无已选植物且 isHavingPlant == true：选中格子上的植物（使用 plantOnCell 引用）
    /// </summary>
    void OnMouseDown()
    {

        if (!isMouseEnter || sr == null) return;

        // 优先：如果存在全局已选植物，先尝试移动（无论格子是否为空）
        if (SelectPlantController.instance != null)
        {
            PlantMove selected = SelectPlantController.instance.GetSelectPlant();
            if (selected != null)
            {
                bool moved = selected.TryMoveToCell(this);
                if (moved)
                {
                    // 移动发起成功，取消选中并返回（不执行种植）
                    SelectPlantController.instance.DeselectPlant();
                    return;
                }
                else
                {
                    // 移动失败时（例如目标被占用或为同格），如果格子有植物则选中该格子的植物
                    if (isHavingPlant)
                    {
                        if (plantOnCell == null)
                        {
                            Debug.LogWarning("格子标记为已被占用，但 plantOnCell 为 null（数据不一致）");
                            return;
                        }
                        SelectPlantController.instance.SelectPlant(plantOnCell);
                        return;
                    }
                    // 若移动失败且格子为空，则继续执行后续种植逻辑（降级为种植）
                }
            }
        }

        // 若没有已选植物或移动降级到种植：按 isHavingPlant 决定
        if (!isHavingPlant)
        {
            PlantInventoryController inventory = FindObjectOfType<PlantInventoryController>();
            if (inventory == null)
            {
                Debug.LogWarning("未找到 PlantInventoryController 实例");
                return;
            }

            GameObject plant = inventory.GetPlantByIndex(0); // TODO:增加随机或选择种植的方法

            if (plant == null)
            {
                Debug.LogWarning("当前植物列表为空，请检查自己的卡槽");
                return;
            }
            else
            {
                // 在实例化前，先使用 PlantCardControl 做冷却校验
                if (PlantCardControl.Instance == null)
                {
                    Debug.LogWarning("未找到 PlantCardControl 实例，取消种植以避免不一致。");
                    return;
                }

                if (!PlantCardControl.Instance.TryPlant(plant))
                {
                    Debug.Log($"{plant.name} 还在冷却中，无法种植。");
                    return;
                }

                // 使用 CreateNewPrefab 并传入当前格子引用，以便建立关联（NewPrefabOnCell 会在创建后设置 plantOnCell）
                GameObject created = NewPrefabOnCell.Instance.CreateNewPrefab(plant, transform.position, this);
                if (created != null)
                {
                    isHavingPlant = true; // 标记该格已被占用
                }
            }

            return;
        }

        // 若走到这里，说明格子被占用且没有已选植物 -> 选中格子上的植物
        if (plantOnCell == null)
        {
            return;
        }

        if (SelectPlantController.instance != null)
        {
            SelectPlantController.instance.SelectPlant(plantOnCell);
        }
    }
}
