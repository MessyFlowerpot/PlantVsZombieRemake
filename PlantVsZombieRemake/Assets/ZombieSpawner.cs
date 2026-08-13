using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 单个刷怪点的配置数据
/// </summary>
[Serializable]
public class SpawnOption
{
    [Tooltip("程序内ID，留空或 <=0 将自动分配")]
    public int id;

    [Tooltip("僵尸预制体")]
    public GameObject prefab;

    [Tooltip("生成该僵尸消耗的点数")]
    public int cost = 1;
}

public class ZombieSpawner : MonoBehaviour
{
    [Header("基础设置")]
    [Tooltip("是否激活")]
    [SerializeField] private bool isActive = true;

    [Header("刷怪选项")]
    [Tooltip("可选的僵尸类型列表（包含ID、预制体、消耗点数）")]
    [SerializeField] private List<SpawnOption> spawnOptions = new List<SpawnOption>();

    [Tooltip("生成间隔（秒）")]
    [SerializeField] private float spawnInterval = 0.5f;

    // 当前分配到但尚未用于生成的点数
    private int pendingPoints = 0;
    private Coroutine spawningCoroutine;

    // 用于自动分配 ID 的静态计数器
    private static int s_nextAutoId = 1;

    #region 公共接口

    public void SetActiveState(bool state)
    {
        isActive = state;
        Debug.Log(isActive ? $"[{gameObject.name}] 僵尸生成器已激活" : $"[{gameObject.name}] 僵尸生成器已停用");
    }

    public bool IsActive() => isActive;
    public int GetPendingPoints() => pendingPoints;
    public IEnumerable<SpawnOption> GetOptions() => spawnOptions;
    public void ClearPendingPoints() => pendingPoints = 0;

    /// <summary>
    /// 限制保留的 pendingPoints 上限（用于 WaveControl 的保留逻辑）
    /// </summary>
    public void CapPendingPoints(int max)
    {
        if (max < 0) return;
        pendingPoints = Math.Min(pendingPoints, max);
    }

    #endregion

    #region Unity 生命周期与事件

    private void OnEnable()
    {
        WaveControl.OnPointsAllocated += HandlePointsAllocated;
    }

    private void OnDisable()
    {
        WaveControl.OnPointsAllocated -= HandlePointsAllocated;
    }

    private void Awake()
    {
        if (spawnOptions != null)
        {
            foreach (var opt in spawnOptions)
            {
                if (opt == null) continue;
                if (opt.id <= 0) opt.id = s_nextAutoId++;
            }
        }
    }

    private void OnValidate()
    {
        if (spawnOptions == null) return;
        foreach (var opt in spawnOptions)
        {
            if (opt == null) continue;
            if (opt.id <= 0) opt.id = s_nextAutoId++;
            if (opt.cost < 1) opt.cost = 1;
        }
    }

    #endregion

    #region 核心逻辑

    private void HandlePointsAllocated(List<int> allocation, ZombieSpawner[] spawners)
    {
        if (!isActive) return;

        int idx = Array.IndexOf(spawners, this);
        if (idx < 0 || idx >= allocation.Count) return;

        int assigned = allocation[idx];
        if (assigned <= 0) return;

        pendingPoints += assigned;
        Debug.Log($"[{gameObject.name}] 接收到分配点数: {assigned}，当前待用点数: {pendingPoints}");

        if (spawningCoroutine == null) spawningCoroutine = StartCoroutine(SpawnRoutine());
    }

    public int GetMinCost()
    {
        if (spawnOptions == null || spawnOptions.Count == 0) return int.MaxValue;
        var valid = spawnOptions.Where(o => o != null && o.prefab != null).ToList();
        if (valid.Count == 0) return int.MaxValue;
        return valid.Min(o => Math.Max(1, o.cost));
    }

    private SpawnOption SelectOptionForCurrentPoints()
    {
        if (spawnOptions == null || spawnOptions.Count == 0) return null;
        var candidates = spawnOptions
            .Where(o => o != null && o.prefab != null && o.cost <= pendingPoints)
            .OrderByDescending(o => o.cost)
            .ToList();
        return candidates.FirstOrDefault();
    }

    public bool TrySpawnById(int id)
    {
        var opt = spawnOptions.FirstOrDefault(o => o != null && o.id == id);
        if (opt == null || opt.prefab == null) return false;
        if (pendingPoints < opt.cost) return false;
        Instantiate(opt.prefab, transform.position, Quaternion.identity);
        pendingPoints -= opt.cost;
        return true;
    }

    private IEnumerator SpawnRoutine()
    {
        while (isActive)
        {
            var option = SelectOptionForCurrentPoints();
            if (option == null) break;
            Instantiate(option.prefab, transform.position, Quaternion.identity);
            pendingPoints -= option.cost;
            yield return new WaitForSeconds(spawnInterval);
        }
        spawningCoroutine = null;
    }

    #endregion
}