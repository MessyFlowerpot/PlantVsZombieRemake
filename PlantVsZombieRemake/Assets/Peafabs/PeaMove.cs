using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaMove : MonoBehaviour
{
    [SerializeField] private float PeaMoveSpeed = 1.0f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float destroyXPosition = 10f;
    private bool hasHit = false;

    /// <summary>
    /// 碰到僵尸后，子弹消失
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)//Unity内置方法
    {
        if (hasHit)
        {
            return;
        }
        if (collision.CompareTag("Enemy"))//判断碰到的对象是否是Enemy标签
        {
            ZombieHealth zombieHealth = collision.GetComponent<ZombieHealth>();

            if (zombieHealth != null)
            {
                hasHit = true;
                zombieHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (transform.position.x > destroyXPosition)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(Vector3.right * PeaMoveSpeed * Time.deltaTime);
        }
    }
}
