
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMove : MonoBehaviour
{
    [SerializeField] private float minMoveSpeed = 2.0f;
    [SerializeField] private float maxMoveSpeed = 3.0f;
    [SerializeField] private float downSpeed = 0.8f;
    [Tooltip("如果这个字段值为True，那么随机速度将会启用")]
    [SerializeField] private bool randomizeOnStart = true;

    private float moveSpeed = 2.2f;

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
    public void SpeedDown(bool isWillDie)
    {
        if (isWillDie)
        {
            moveSpeed *= downSpeed;
        }
    }

    /// <summary>
    /// 如果僵尸碰到Home，那就会报告日志且销毁僵尸
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Home"))
        {
            ZombieHealth zombieHealth = GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                if (!zombieHealth.IsZombieDead())
                {
                    Debug.LogWarning("僵尸到达了家！");
                    Destroy(gameObject);
                }
            }
        }
    }

    void Update()
    {
        ZombieAttack zombieAttack = GetComponent<ZombieAttack>();
        if (!(zombieAttack.IsAttacking()))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
    } 
}
