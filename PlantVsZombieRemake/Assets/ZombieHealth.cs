using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxZombieHealth = 270;
    [SerializeField] private int diedZombieHealth = 70;
    private int nowZombieHealth;
    [SerializeField] private float downSpeed = 0.8f;
    [SerializeField] private float willDieTakeDmageSpeed = 0.1f;
    [SerializeField] private int willDieTakeDmage;
    [SerializeField] private int maxWillDieTakeDmage = 10;
    private bool isSpeedDown = false;

    private void Start()
    {
        nowZombieHealth = maxZombieHealth;
        willDieTakeDmage = Random.Range(1,maxWillDieTakeDmage);
    }

    /// <summary>
    /// 僵尸受到伤害
    /// </summary>
    /// <param name="damage"></param>
    public void takeDamage(int damage)
    {
        if(nowZombieHealth <= damage)
        {
            nowZombieHealth = 0;
        }
        else
        {
            nowZombieHealth -= damage;
        }

        if (nowZombieHealth <= diedZombieHealth)
        {
            StartCoroutine(willDie());
        }
    }

    /// <summary>
    /// 濒死时，僵尸移动速度降低，并且血量持续下降，直到死亡
    /// </summary>
    public IEnumerator willDie()
    {
        ZombieMove zombieMove = GetComponent<ZombieMove>();
        if(zombieMove != null)
        {
            zombieMove.SpeedDown(! isSpeedDown, downSpeed);
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
        Debug.Log("僵尸倒下了!");
        Destroy(gameObject);
    }
}



