using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

[System.Serializable]
public struct PlantCoolDownEntry
{
    public GameObject plantPrefab;
    public float cooldownTime;
    public bool isStartWithCoolDown;
    public int sunCost;
}

public class PlantCardControl : MonoBehaviour
{
    public static PlantCardControl Instance { get; private set; }

    // 核心字典：记录所有植物的【当前】冷却时间
    private Dictionary<GameObject, float> cooldownDict = new Dictionary<GameObject, float>();

    // 新增：记录每种植物的基础冷却（配置中的值），用于外部调用时自动查找冷却时长
    private Dictionary<GameObject, float> baseCooldownDict = new Dictionary<GameObject, float>();

    // 新增：记录每种植物的阳光消耗
    private Dictionary<GameObject, int> baseSunCostDict = new Dictionary<GameObject, int>();

    [Header("开局冷却配置(结构体列表)")]
    [Tooltip("格式：(植物预制体, 开局冷却秒数，是否开局冷却)")]
    [SerializeField] private List<PlantCoolDownEntry> initialCooldowns = new List<PlantCoolDownEntry>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 【关键写入】：遍历元组列表，把每个植物专属的冷却时间写进字典
        foreach (var config in initialCooldowns)
        {
            if (config.plantPrefab != null)
            {
                // 记录基础冷却时长，供 TryPlant(GameObject) 查找使用
                baseCooldownDict[config.plantPrefab] = config.cooldownTime;

                // 记录阳光消耗
                baseSunCostDict[config.plantPrefab] = config.sunCost;

                if (config.isStartWithCoolDown)
                {
                    // 用专属的秒数初始化字典
                    cooldownDict[config.plantPrefab] = config.cooldownTime;
                }
                else
                {
                    cooldownDict[config.plantPrefab] = 0f;
                }
            }
        }
    }

    void Update()
    {
        // 每帧自动扣减所有植物的冷却时间
        // 使用 ToArray() 防止遍历字典时修改字典导致报错
        foreach (var kvp in new Dictionary<GameObject, float>(cooldownDict))
        {
            if (kvp.Value > 0)
            {
                cooldownDict[kvp.Key] = kvp.Value - Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 【核心接口】尝试种植某种植物（直接传入要设置的冷却时长）
    /// 修改：在种植前检测阳光是否足够，如果不足则返回 false；如果足够则调用 SunBank.SpendSun 并继续种植逻辑。
    /// </summary>
    public bool TryPlant(GameObject plantPrefab, float cooldownTime)
    {
        // 【安全读取】：检查字典里有没有这个 Key，且时间大于 0
        if (cooldownDict.ContainsKey(plantPrefab) && cooldownDict[plantPrefab] > 0)
        {
            Debug.Log($"[{plantPrefab.name}] 还在冷却中！剩余 {cooldownDict[plantPrefab]:F1} 秒");
            return false;
        }

        // 检查阳光消耗（若未配置则视为 0）
        int sunCost = 0;
        baseSunCostDict.TryGetValue(plantPrefab, out sunCost);

        if (sunCost > 0)
        {
            if (SunBank.Instance != null)
            {
                if (!SunBank.Instance.CanSpend(sunCost))
                {
                    Debug.Log($"[{plantPrefab.name}] 阳光不足，无法种植（需要 {sunCost}）");
                    return false;
                }
                // 足够则扣除阳光
                SunBank.Instance.SpendSun(sunCost);
            }
            else
            {
                Debug.LogWarning("SunBank.Instance 为 null，跳过阳光检测并继续种植（请确保场景中有 SunBank 实例）");
            }
        }

        // 【安全写入】：无论之前有没有这个 Key，直接赋值
        cooldownDict[plantPrefab] = cooldownTime;
        Debug.Log($"[{plantPrefab.name}] 种植成功！进入 {cooldownTime} 秒冷却。");
        return true;
    }

    /// <summary>
    /// 便利重载：只传入预制体，内部从配置中查找冷却时长（若未配置则使用0）
    /// </summary>
    public bool TryPlant(GameObject plantPrefab)
    {
        if (plantPrefab == null)
        {
            Debug.LogWarning("TryPlant 收到 null 预制体");
            return false;
        }

        float cooldownTime = 0f;
        if (!baseCooldownDict.TryGetValue(plantPrefab, out cooldownTime))
        {
            Debug.LogWarning($"[{plantPrefab.name}] 未在初始化配置中找到基础冷却时间，使用 0 秒作为默认值。");
            cooldownTime = 0f;
        }

        return TryPlant(plantPrefab, cooldownTime);
    }
}