using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPrefabOnCell : MonoBehaviour
{
    static public NewPrefabOnCell Instance { get; private set; } // 引用 NewPrefabOnCell 脚本

    private void Awake()
    {
        // 确保 Instance 只被赋值一次
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 如果已经存在一个实例，销毁当前实例并输出警告信息
            Debug.LogWarning($"{nameof(NewPrefabOnCell)}: 已经存在一个实例，当前实例将被销毁。对象：{gameObject.name}");
            Destroy(gameObject);
            return;
        }
    }

    // 修改：返回创建的实例并接收格子引用，以便立即将植物与格子关联
    public GameObject CreateNewPrefab(GameObject prefab, Vector3 position, CellHighLight cell)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"{nameof(NewPrefabOnCell)}: 传入的 prefab 为 null，无法创建新实例");
            return null;
        }

        GameObject go = Instantiate(prefab, position, Quaternion.identity);

        // 将新实例与格子关联（如果植物上挂有 PlantMove）
        if (go != null && cell != null)
        {
            PlantMove pm = go.GetComponent<PlantMove>();
            if (pm != null)
            {
                // 通过 AssignCell 建立关联，并显式保证 cell.plantOnCell 被赋值
                pm.AssignCell(cell);
                // 额外保证（防止 AssignCell 未被正确执行的极端情况）
                cell.plantOnCell = pm;
                cell.isHavingPlant = true;
            }
            else
            {
                // 对于没有 PlantMove 的植物，使用 PlantOccupier 负责占位与销毁时清理
                PlantOccupier occupier = go.GetComponent<PlantOccupier>();
                if (occupier == null)
                {
                    occupier = go.AddComponent<PlantOccupier>();
                }
                occupier.AssignCell(cell);
            }
        }

        return go;
    }

    // 向后兼容的重载：不传 cell 时行为与之前保持一致（仅实例化）
    public GameObject CreateNewPrefab(GameObject prefab, Vector3 position)
    {
        return CreateNewPrefab(prefab, position, null);
    }
}