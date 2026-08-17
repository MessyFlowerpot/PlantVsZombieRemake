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

    public void CreateNewPrefab(GameObject prefab,Vector3 position)
    {
        if(prefab == null)
        {
            Debug.LogWarning($"{nameof(NewPrefabOnCell)}: 传入的 prefab 为 null，无法创建新实例");
            return;
        }

        Instantiate(prefab,position, Quaternion.identity);
        Debug.Log($"在{position}处创建了新的预制体{prefab.name}");
    }
}
