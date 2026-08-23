using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCollectible : MonoBehaviour
{
    [Header("阳光属性")]
    [Tooltip("阳光点数")][SerializeField] private int sunPoint = 25;
    private SunBank bank;
    private void Start()
    {
        bank = GetComponent<SunBank>();
        if (bank == null) Debug.LogWarning("未找到管理阳光的组件"); return;
    }
    private void OnMouseDown()
    {
        if (bank != null) bank.AddSun(sunPoint);
    }
}
