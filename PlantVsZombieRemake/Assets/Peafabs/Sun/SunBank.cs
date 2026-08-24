using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SunBank : MonoBehaviour
{
    public static SunBank Instance { get; private set; }
    [Header("全局阳光设置")]
    [Tooltip("初始阳光数")]
    [SerializeField] private int originalSun = 50;
    [Tooltip("阳光上限")]
    [SerializeField] private int MaxSun = 9999;
    private int currentSun;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(Instance);
    }
    private void Start()
    {
        currentSun = originalSun;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) Debug.Log($"目前阳光数:{currentSun}");
    }
    public void AddSun(int sunNum)
    {
        if (sunNum + currentSun > MaxSun) currentSun = MaxSun;
        else currentSun += sunNum;
    }

    // 新增：判断当前阳光是否足够（不修改 currentSun）
    public bool CanSpend(int sunNum)
    {
        return currentSun - sunNum >= 0;
    }

    public void SpendSun(int sunNum)
    {
        if (currentSun - sunNum < 0) Debug.Log("阳光不足");
        else currentSun -= sunNum;
    }
}
