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
    bool isSlowed = false; // 是否被减速

    private float moveSpeed = 2.2f;

    // 下面用于管理延长减速时间（避免叠加减速效果）
    private Coroutine slowCoroutine = null;
    private float originalSpeedForSlow = 0f;
    private float slowEndTime = 0f;

    // 缓存攻击组件以便同步攻速
    private ZombieAttack cachedZombieAttack = null;

    // 用于外观染色
    private SpriteRenderer spriteRenderer = null;
    private Color originalColor = Color.white;

    private void Start()
    {
        if (randomizeOnStart)
        {
            moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        }
        cachedZombieAttack = GetComponent<ZombieAttack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
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
        var za = cachedZombieAttack != null ? cachedZombieAttack : GetComponent<ZombieAttack>();
        if (za == null || !za.IsAttacking())
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 触发减速：如果当前未减速则应用减速并启动计时；
    /// 如果已经在减速中，则只延长减速时间（不叠加速度效果）。
    /// 同时同步调整攻击间隔（首次触发时）并改变外观为蓝色（恢复时还原）。
    /// </summary>
    /// <param name="slowFactor">减速系数（例如0.5f）</param>
    /// <param name="duration">本次触发希望延长的时间（秒）</param>
    public void SlowDown(float slowFactor, float duration)
    {
        if (!isSlowed)
        {
            isSlowed = true;
            originalSpeedForSlow = moveSpeed;
            moveSpeed = originalSpeedForSlow * slowFactor;
            slowEndTime = Time.time + duration;

            if (cachedZombieAttack == null) cachedZombieAttack = GetComponent<ZombieAttack>();
            if (cachedZombieAttack != null)
            {
                cachedZombieAttack.ApplyAttackSlow(slowFactor);
            }

            // 应用蓝色视觉效果（保留原始纹理）
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null) originalColor = spriteRenderer.color;
            }
            if (spriteRenderer != null)
            {
                // 混合原色和蓝色，避免完全覆盖亮度/透明度
                spriteRenderer.color = Color.Lerp(originalColor, Color.blue, 0.8f);
            }

            slowCoroutine = StartCoroutine(SlowDownCoroutine());
        }
        else
        {
            // 已在减速中：只延长减速时间，不改变当前速度或再次修改攻击间隔/外观
            slowEndTime += duration;
        }
    }

    private IEnumerator SlowDownCoroutine()
    {
        // 等待直到达到延迟结束时间
        while (Time.time < slowEndTime)
        {
            yield return null;
        }

        // 恢复速度
        moveSpeed = originalSpeedForSlow;
        isSlowed = false;
        slowCoroutine = null;

        // 恢复攻击间隔
        if (cachedZombieAttack == null) cachedZombieAttack = GetComponent<ZombieAttack>();
        if (cachedZombieAttack != null)
        {
            cachedZombieAttack.RemoveAttackSlow();
        }

        // 恢复原始颜色
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}