using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeashooterHealth : MonoBehaviour
{
    [SerializeField] private int maxPeashooterHealth = 300;
    private int nowPeashooterHealth;
    void Start()
    {
        nowPeashooterHealth = maxPeashooterHealth;
    }


    /// <summary>
    /// 豌豆射手受到伤害
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        if (nowPeashooterHealth <= damage)
        {
            nowPeashooterHealth = 0;
        }
        else
        {
            nowPeashooterHealth -= damage;
        }
        if (nowPeashooterHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 死亡
    /// </summary>
    public void Die()
    {
        Debug.Log("豌豆射手死亡！");
        Destroy(gameObject);
    }

    /// <summary>
    /// 判断豌豆射手是否死亡
    /// </summary>
    /// <returns></returns>
    public bool IsPeashooterDead()
    {
        return nowPeashooterHealth <= 0;
    }
}