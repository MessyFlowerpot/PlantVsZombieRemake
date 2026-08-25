using UnityEngine;
using System.Collections;

public class Sunflower : MonoBehaviour
{
    [Header("设置")]
    public GameObject sunPrefab;      // 拖入你的阳光预制体
    public float productionInterval = 1f; // 多少秒生产一次阳光 (原版大概是24秒)

    [Header("生成位置微调")]
    public float spawnHeightOffset = 1.5f; // 从向日葵中心向上偏移多少 (Y轴)
    public float spawnRandomRange = 0.8f;  // X轴的随机左右摆动范围

    private void Start()
    {
        // 启动生产循环
        StartCoroutine(ProduceSunRoutine());
    }

    IEnumerator ProduceSunRoutine()
    {
        while (true) // 只要花活着，就无限循环
        {
            // 1. 等待指定的时间
            yield return new WaitForSeconds(productionInterval);

            // 2. 计算生成位置
            // 基础位置：向日葵自己的位置
            Vector3 basePos = transform.position;

            // X轴：在基础位置左边或右边随机一点点 (-0.8 到 0.8)
            float randomX = Random.Range(-spawnRandomRange, spawnRandomRange);

            // Y轴：基础位置 + 头顶高度
            float targetY = basePos.y + spawnHeightOffset;

            // 最终位置
            Vector3 spawnPos = new Vector3(basePos.x + randomX, targetY, basePos.z);

            // 3. 生成阳光！
            Instantiate(sunPrefab, spawnPos, Quaternion.identity);
        }
    }
}
