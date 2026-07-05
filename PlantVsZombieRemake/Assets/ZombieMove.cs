
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMove : MonoBehaviour
{
    [SerializeField] private float minMoveSpeed = 1.5f;
    [SerializeField] private float maxMoveSpeed = 2.5f;
    [Tooltip("如果这个字段值为True，那么随机速度将会启用")]
    [SerializeField] private bool randomizeOnStart = true;

    private float moveSpeed = 2.0f;

    private void Start()
    {
        if (randomizeOnStart)
        {
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    /// <summary>
    /// 初始化僵尸移动速度
    /// </summary>
    /// <param name="speed"></param>
    public void Initialize(float speed)
    {
        moveSpeed = speed;
        randomizeOnStart = false;
    }

    /// <summary>
    /// 濒死状态下的僵尸移动速度降低
    /// </summary>
    /// <param name="isWillDie"></param>
    /// <param name="downSpeed"></param>
    public void SpeedDown(bool isWillDie,float downSpeed)
    {
        if (isWillDie)
        {
            moveSpeed *= downSpeed;
            Debug.Log($"僵尸濒死了！目前速度{moveSpeed}");
        }
    }


    void Update()
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }
}
