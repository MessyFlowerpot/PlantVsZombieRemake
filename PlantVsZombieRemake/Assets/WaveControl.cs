using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveControl : MonoBehaviour
{
    // 事件：通知订阅者每个有效刷怪点被分配到的点数
    public static event Action<List<int>, ZombieSpawner[]> OnPointsAllocated;

    [Header("点数设置")]
    [Tooltip("初始点数")]
    [SerializeField] private int initialPoints = 5;

    [Tooltip("如果此字段为True，则开启指数级增长")]
    [SerializeField] private bool isExponential = false;

    [Tooltip("指数级增长的指数底数")]
    [SerializeField] private float exponentialBase = 1.15f;

    [Tooltip("指数增长每 N 波增加底数的阈值")]
    [SerializeField] private int exponentialGrowthThreshold = 5;

    [Tooltip("指数增长底数每次增加的量")]
    [SerializeField] private float exponentialGrowthAmount = 0.05f;

    [Tooltip("指数增长底数上限")]
    [SerializeField] private float exponentialBaseMax = 2.0f;

    [Tooltip("若未开启指数级增长，每波则线性增长，此字段为线性增长的增量")]
    [SerializeField] private int linearGrowth = 2;

    [Tooltip("线性增长增量增加需要的波次次数")]
    [SerializeField] private int linearGrowthThreshold = 3;

    [Tooltip("线性增长增量增加的幅度")]
    [SerializeField] private int linearGrowthAmount = 2;

    [Tooltip("线性增长增量上限")]
    [SerializeField] private int linearGrowthMax = 10;

    [Header("间隔设置")]
    [Tooltip("波次间隔")]
    [SerializeField] private float waveInterval = 40f;

    [Tooltip("波次间隔时间缩短需要的波次次数")]
    [SerializeField] private int intervalReductionThreshold = 5;

    [Tooltip("波次间隔时间缩短的幅度")]
    [SerializeField] private float intervalReductionAmount = 1f;

    [Tooltip("波次间隔时间下限")]
    [SerializeField] private float waveIntervalMin = 30f;

    [Header("分配与保留设置")]
    [Tooltip("尽量保证至少有多少个 spawner 能拿到其最小生成消耗（若点数不够则尽量保证更多）")]
    [SerializeField] private int guaranteedSpawners = 2;

    [Tooltip("是否保留上波未用尽的点数到下一波（true 保留，false 每波清零）")]
    [SerializeField] private bool isKeeping = true;

    [Tooltip("每个 spawner 最多保留的点数上限（仅在 isKeeping=true 时生效）")]
    [SerializeField] private int maxKeepingPoints = 3;

    [Tooltip("波次点数上限")]
    [SerializeField] private int maxWavePoints = 999;

    private float timer = 0f;
    private int currentWavePoints;
    private int waveCount = 0;

    private void Start()
    {
        // 开局直接触发第一波
        currentWavePoints = initialPoints;
        timer = 0f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            NewWave();
            timer += waveInterval; // 用 += 保留超时量，避免节奏漂移

            PointsUp(); // 新波次增加点数
        }
    }

    /// <summary>
    /// 增加点数
    /// </summary>
    void PointsUp()
    {
        if (isExponential)
        {
            currentWavePoints = Mathf.RoundToInt(initialPoints * Mathf.Pow(exponentialBase, waveCount));
            if (exponentialGrowthThreshold > 0 && waveCount % exponentialGrowthThreshold == 0)
            {
                exponentialBase += exponentialGrowthAmount;
                exponentialBase = Mathf.Min(exponentialBase, exponentialBaseMax);
            }
        }
        else
        {
            currentWavePoints += linearGrowth;
            if (linearGrowthThreshold > 0 && waveCount % linearGrowthThreshold == 0)
            {
                linearGrowth += linearGrowthAmount;
                linearGrowth = Mathf.Min(linearGrowth, linearGrowthMax);
            }
        }

        currentWavePoints = Mathf.Min(currentWavePoints, maxWavePoints);

        if (intervalReductionThreshold > 0)
        {
            if (waveCount % intervalReductionThreshold == 0)
            {
                waveInterval -= intervalReductionAmount;
                waveInterval = Mathf.Max(waveInterval, waveIntervalMin);
            }
        }
    }

    void NewWave()
    {
        waveCount++;

        ZombieSpawner[] validSpawners = FindObjectsOfType<ZombieSpawner>(false)
            .Where(s => s.IsActive())
            .ToArray();

        if (validSpawners.Length == 0)
        {
            Debug.LogWarning("当前没有激活的刷怪点！");
            return;
        }

        // 根据保留设置，决定是否清除或限制每个 spawner 的 pendingPoints
        if (!isKeeping)
        {
            foreach (var s in validSpawners) s.ClearPendingPoints();
        }
        else
        {
            foreach (var s in validSpawners) s.CapPendingPoints(Mathf.Max(0, maxKeepingPoints));
        }

        int N = validSpawners.Length;
        int totalPoints = Mathf.Clamp(currentWavePoints, 0, maxWavePoints);

        // 先尝试保证最多 guaranteedSpawners 个 spawner 各自至少得到其最小生成消耗（以随机选择候选）
        var allocation = Enumerable.Repeat(0, N).ToList();
        var minCosts = new int[N];
        for (int i = 0; i < N; i++) minCosts[i] = validSpawners[i].GetMinCost();

        // 候选索引（有有效 minCost）
        var candidates = Enumerable.Range(0, N).Where(i => minCosts[i] != int.MaxValue).ToList();

        // 随机打乱候选顺序
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            int tmp = candidates[i];
            candidates[i] = candidates[j];
            candidates[j] = tmp;
        }

        int pointsRemaining = totalPoints;
        int guaranteeTarget = Math.Min(guaranteedSpawners, candidates.Count);
        int guaranteedCount = 0;

        // 按打乱后的候选顺序尝试分配最小消耗，直到满足 guaranteeTarget 或无足够点数
        foreach (var idx in candidates)
        {
            if (guaranteedCount >= guaranteeTarget) break;
            int cost = minCosts[idx];
            if (cost <= pointsRemaining)
            {
                allocation[idx] += cost;
                pointsRemaining -= cost;
                guaranteedCount++;
            }
        }

        // 剩余点数逐点随机分配
        if (pointsRemaining > 0)
        {
            for (int p = 0; p < pointsRemaining; p++)
            {
                int idx = UnityEngine.Random.Range(0, N);
                allocation[idx]++;
            }
        }

        // 触发事件，让订阅者根据分配点数开始刷怪并自行减少点数
        OnPointsAllocated?.Invoke(allocation, validSpawners);
    }
}