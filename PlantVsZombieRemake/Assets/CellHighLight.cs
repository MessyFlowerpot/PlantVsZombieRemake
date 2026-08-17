using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellHighLight : MonoBehaviour
{
    private SpriteRenderer sr;// 记录格子精灵渲染器组件
    private Color originalColor;// 记录格子原始颜色
    private bool isMouseEnter = false;// 记录鼠标是否进入格子

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

        SetAlpha(0f);// 设置初始透明度为0.2
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
    /// 当鼠标点击格子时，输出格子位置
    /// </summary>
    void OnMouseDown()
    {
        Debug.Log($"已点击格子：{transform.position}");

        if (isMouseEnter && sr != null)
        {
            PlantInventoryController inventory = FindObjectOfType<PlantInventoryController>();// 使用 FindObjectOfType 来获取 PlantInventoryController 实例
            if (inventory == null)
            {
                Debug.LogWarning("未找到 PlantInventoryController 实例");
                return;
            }

            GameObject plant = inventory.GetPlantByIndex(0); // TODO:增加随机种植的方法

            if(plant == null)
            {
                Debug.LogWarning("当前植物列表为空，请检查自己的卡槽");
                return;
            }
            else
            {
                NewPrefabOnCell.Instance.CreateNewPrefab(plant, transform.position);
                Debug.Log($"成功在格子种植了：{plant.name}");
            }
                    
        }
    }
}
