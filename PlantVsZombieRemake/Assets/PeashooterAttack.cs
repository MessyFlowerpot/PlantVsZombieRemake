using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeashooterAttack : MonoBehaviour
{
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private float fireTime = 2.5f;
    [SerializeField] private float nextFireTime = 2.5f;

    /// <summary>
    /// 发射子弹
    /// </summary>
    void Attack()
    {
        //参数1为克隆对象，参数2为克隆位置，参数3为旋转角度
        //transform.position为获取当前位置
        //Quaternion.identity为将旋转角度设为0
        Instantiate(bulletObject, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime += fireTime;
            Attack();
        }   
    }
}
