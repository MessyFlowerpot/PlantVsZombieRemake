using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDown : MonoBehaviour
{
    [Header("冷却设置")]
    [Tooltip("冷却时长")][SerializeField] private float cooldownTime = 3.5f;
    [Tooltip("开局是否需要冷却")][SerializeField] private bool isStartWithCooldown = false;
    private bool isReady;//是否准备好
    private float currentCooldownTime;//当前冷却时间

    // 新增：标记是否已完成基于配置的初始化（防止在 prefab 上直接调用时未初始化）
    private bool initialized = false;

    private void Start()
    {
        EnsureInitialized();
    }

    private void Update()
    {
        EnsureInitialized();

        if (!isReady)
        {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime <= 0f)
            {
                Debug.Log($"{name}准备好了");
                isReady = true;
                currentCooldownTime = 0f;
            }
        }
    }

    // 新增：确保根据 isStartWithCooldown 完成初始化（幂等）
    private void EnsureInitialized()
    {
        if (initialized) return;

        Debug.Log($"{name}被初始化过了");

        if (isStartWithCooldown)
        {
            isReady = false;
            currentCooldownTime = cooldownTime;
        }
        else
        {
            isReady = true;
            currentCooldownTime = 0f;
        }

        initialized = true;
    }

    public bool TryPlant()
    {
        // 在尝试种植前确保已初始化（处理在 prefab 上直接调用的情况）
        EnsureInitialized();

        if (isReady)
        {
            StartCoolDown();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartCoolDown()
    {
        isReady = false;
        currentCooldownTime = cooldownTime;
    }

    public float Cd()
    {
        return currentCooldownTime;
    }
}