using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxZombieHealth = 270;
    [SerializeField] private int diedZombieHealth = 70;
    private int nowZombieHealth;
    
    [SerializeField] private float willDieTakeDmageSpeed = 0.1f;
    [SerializeField] private int willDieTakeDmage;
    [SerializeField] private int maxWillDieTakeDmage = 10;
    [SerializeField] private TypeIArmourHealth typeIArmour = null;
    private bool isSpeedDown = false;

    private void Start()
    {
        nowZombieHealth = maxZombieHealth;
        willDieTakeDmage = Random.Range(1,maxWillDieTakeDmage);
    }

    private void Awake()
    {
        // 仅在子物体中搜索挂载了 TypeIArmourHealth 的对象（不包括自身）
        typeIArmour = GetComponentInChildren<TypeIArmourHealth>();
    }

    /// <summary>
    /// 僵尸受到伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="isPhysical"></param>
    public void TakeDamage(int damage,bool isPhysical)
    {
        if(isPhysical)
        {
            nowZombieHealth = (nowZombieHealth > damage ? nowZombieHealth - damage : 0);
        }
        else
        {
            if(typeIArmour == null || typeIArmour.IsArmourBroken())
            {
                nowZombieHealth = (nowZombieHealth > damage ? nowZombieHealth - damage : 0);
            }
            else
            {
                typeIArmour.ArmourTakeDamage(damage);
            }
        }

        if (nowZombieHealth <= diedZombieHealth)
        {
            StartCoroutine(WillDie());
        }
    }

    /// <summary>
    /// 濒死时，僵尸移动速度降低，并且血量持续下降，直到死亡
    /// </summary>
    public IEnumerator WillDie()
    {
        ZombieMove zombieMove = GetComponent<ZombieMove>();
        if(zombieMove != null)
        {
            zombieMove.SpeedDown(! isSpeedDown);
        }

        isSpeedDown = true;
   
        while (nowZombieHealth > 0)
        {
            nowZombieHealth -= willDieTakeDmage;
            willDieTakeDmage = Random.Range(1, maxWillDieTakeDmage);
            yield return new WaitForSeconds(willDieTakeDmageSpeed);
        }
        Die();
    }

    /// <summary>
    /// 血量为0时，僵尸死亡
    /// </summary>
    public void Die()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 判断僵尸是否死亡
    /// </summary>
    /// <returns></returns>
    public bool IsZombieDead()
    {
        return nowZombieHealth <= diedZombieHealth;
    }

    public void TypeIArmourBroke()
    {
        typeIArmour = null;
    }
}



