using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewSunOnSky : MonoBehaviour
{
     static public CreateNewSunOnSky Instance {  get; private set; }
    [Header("生成设置")]
    [Tooltip("阳光预制体")][SerializeField] private GameObject sunPrefab;
    [Tooltip("生成间隔")][SerializeField] private float newSunTime = 10f;
    [Tooltip("X坐标范围")][SerializeField] private Vector2 xRange = new Vector2(-8f, 8f);
    [Tooltip("Y坐标")][SerializeField] private float spawnY = 6f;
    private Coroutine dropRoutine;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(Instance);
    }
    private void Start()
    {
        StartDropping();
    }
    private void StartDropping()
    {
        if(dropRoutine != null) StopCoroutine(dropRoutine);
        dropRoutine = StartCoroutine(DropLoop());
    }

    private void StopDropping()
    {
        if(dropRoutine != null)
        {
            StopCoroutine(dropRoutine);
            dropRoutine = null; 
        }
    }

    private IEnumerator DropLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(newSunTime);
            SpawnSun();
        }
    }

    private void SpawnSun()
    {
        if (sunPrefab == null) { Debug.LogWarning("未找到阳光预制体"); return; }
        float spawnX = Random.Range(xRange.x, xRange.y);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);
        Instantiate(sunPrefab, spawnPos, Quaternion.identity);
    }
}
