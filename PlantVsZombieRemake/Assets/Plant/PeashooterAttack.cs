using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeashooterAttack : MonoBehaviour
{
    [Header("射击设置")]
    [Tooltip("子弹对象")][SerializeField] private GameObject bulletObject;//子弹对象
    [Tooltip("发射间隔")][SerializeField] private float fireTime = 2.5f;//发射间隔
    [Tooltip("下一次发射时间")][SerializeField] private float nextFireTime = 2.5f;//下一次发射时间
    [Tooltip("单次射击次数")][SerializeField] private int shotCount = 1;//单次射击次数
    [Tooltip("高频射击间隔")][SerializeField] private float highFrequencyInterval = 0.05f;//高频射击间隔

    /// <summary>
    /// 发射子弹协程：立即发射一次，然后在次级射击间隔内继续发射直到达到 shotCount
    /// </summary>
    private IEnumerator AttackCoroutine()
    {
        for (int i = 0; i < shotCount; i++)
        {
            Instantiate(bulletObject, transform.position, Quaternion.identity);
            if (i < shotCount - 1)
                yield return new WaitForSeconds(highFrequencyInterval);
        }
    }

    void Start()
    {
        nextFireTime += Random.Range(-0.3f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        PeashooterHealth ph = GetComponent<PeashooterHealth>();
        if (ph != null)
        {
            if (!ph.IsPeashooterDead())
            {
                if (Time.time >= nextFireTime)
                {
                    nextFireTime += (fireTime + Random.Range(-0.2f, 0.2f));
                    StartCoroutine(AttackCoroutine());
                }
            }
        }
    }
}