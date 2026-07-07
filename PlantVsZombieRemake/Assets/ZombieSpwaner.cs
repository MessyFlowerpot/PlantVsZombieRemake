using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieSpwaner : MonoBehaviour
{
    [Header("生成规则")]
    [Tooltip("如果此字段值为True，那么指数级增长僵尸将会开启")]
    [SerializeField] private bool isExponentialGrowth = false;

    [Tooltip("如果指数级增长开启，此字段就为每波僵尸点数增长的倍率")]
    [SerializeField] private float exponentialGrowthRate = 1.5f;

    [Tooltip("此字段为初始的僵尸点数")]
    [SerializeField] private int initialZombiePoints = 3;

    [Tooltip("此字段为每波增长的僵尸点数，如果开启指数级增长则此字段不使用")]
    [SerializeField] private int zombiePointsPerWave = 2;

    [Tooltip("如果此字段为True，那么则为开启僵尸上限值保护")]
    [SerializeField] private bool isZombieLimitProtection = true;

    [Tooltip("如果僵尸上限值保护开启，此字段就为僵尸点数上限值")]
    [SerializeField] private int zombiePointsLimit = 500;

    [Header("波次设置")]
    [Tooltip("生成波次的间隔")]
    [SerializeField] private float waveInterval = 30.0f;

    private Coroutine spawnRoutine;
    private bool isSpawning = false;

    /// <summary>
    /// 僵尸类型类，包含僵尸预制体和僵尸点数
    /// </summary>
    [System.Serializable]
    public class ZombieType
    {
        [Tooltip("拖入僵尸的预制体")]
        public GameObject zombiePrefab;

        [Tooltip("本僵尸占用的点数")]
        public int zombiePointCost = 1;
    }

    [Tooltip("此列表内含所有类型僵尸")]
    public List<ZombieType> zombieTypes = new List<ZombieType>();

    private int currentZombiePoints;
    private int currentWave = 0;

    /// <summary>
    /// 初始设置当前波次和当前僵尸点数
    /// </summary>
    void Set()
    {
        currentZombiePoints = initialZombiePoints;
        currentWave = 0;
        waveInterval = 30.0f;
    }

    /// <summary>
    /// 新的一波僵尸生成，计算当前波次和当前僵尸点数，如果开启了指数级增长，则计算当前波次的僵尸点数，如果开启了僵尸上限值保护，则判断当前波次的僵尸点数是否超过上限值，如果超过则将当前波次的僵尸点数设置为上限值
    /// </summary>
    public void CurrentNewWave()
    {
        currentWave = currentWave + 1;
        if (isExponentialGrowth)
        {
            currentZombiePoints = Mathf.RoundToInt(initialZombiePoints * Mathf.Pow(exponentialGrowthRate, currentWave - 1));
            if (isZombieLimitProtection)
            {
                if(currentZombiePoints > zombiePointsLimit) 
                {
                    currentZombiePoints = zombiePointsLimit;
                }
                else
                {
                    Debug.LogWarning("点数已达到上限");
                }
            }

        }
        else
        {
            currentZombiePoints += zombiePointsPerWave;
        }
    }

    /// <summary>
    /// 生成僵尸，计算当前波次的僵尸点数，随机生成僵尸类型，直到当前波次的僵尸点数为0，如果没有僵尸类型可以生成，则跳出循环
    /// </summary>
    public void SpwanZombies()
    {
        CurrentNewWave();
        int thisWaveZombiePonits = currentZombiePoints;
        if (zombieTypes == null || zombieTypes.Count == 0)
        {
            Debug.LogError("没有僵尸类型，请在ZombieSpwaner中添加僵尸类型");
            return;
        }
        while (thisWaveZombiePonits > 0)
        {
            ZombieType newZombie = zombieTypes[Random.Range(0, zombieTypes.Count)];
            if (newZombie.zombiePointCost < thisWaveZombiePonits)
            {
                thisWaveZombiePonits -= newZombie.zombiePointCost;
                Instantiate(newZombie.zombiePrefab, transform.position, Quaternion.identity);
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 如果按下了开始生成僵尸的按钮，则开启协程，开始生成僵尸
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isSpawning)
            {
                spawnRoutine = StartCoroutine(SpawnZombiesRoutine());
                Debug.Log("开始生成僵尸");
                isSpawning = true;
            }
            else
            {
                StopCoroutine(spawnRoutine);
                Debug.Log("停止生成僵尸");
                Set();
                isSpawning = false;
            }
        }
    }

    /// <summary>
    /// 生成僵尸的协程，每波生成僵尸后等待waveInterval秒后再生成下一波僵尸，如果当前波次为偶数波，则增加5秒的间隔
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnZombiesRoutine()
    {
        while (true)
        {
            SpwanZombies();
            if(currentWave % 2 == 0)
            { 
                waveInterval += 5.0f; // 每两波增加5秒的间隔
            }
            Debug.LogWarning($"第{currentWave}波次将在{waveInterval}秒开始");
            yield return new WaitForSeconds(waveInterval);
        }
    }
}