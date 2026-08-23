using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SunBank : MonoBehaviour
{
    [Header("全局阳光设置")]
    [Tooltip("初始阳光数")]
    [SerializeField] private int originalSun = 50;
    [Tooltip("阳光上限")]
    [SerializeField] private int MaxSun = 9999;
    private int currentSun;
    private void Start()
    {
        currentSun = originalSun;
    }
    public void AddSun(int sunNum)
    {
        if (sunNum + currentSun > MaxSun) currentSun = MaxSun;
        else currentSun += sunNum;
    }
    public void SpendSun(int sunNum)
    {
        if (currentSun - sunNum < 0) Debug.Log("阳光不足");
        else currentSun -= sunNum;
    }
}
