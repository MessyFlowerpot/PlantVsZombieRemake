using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEExplosion : MonoBehaviour
{
    [Header("AOE爆炸设置")]
    [Tooltip("爆炸半径(单位:米)")][SerializeField]private float explosionRadius = 1.5f; // 爆炸半径
    [Tooltip("爆炸伤害")] [SerializeField]private int explosionDamage = 1800; // 爆炸伤害
    [Tooltip("是否为物理伤害")] [SerializeField]private bool isPhysical = true; // 是否为物理伤害

    public void TriggerExplosion()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);// 获取爆炸范围内的所有碰撞体

        foreach (var hit in hitColliders) 
        {
            if(hit == null) continue; // 如果碰撞体为空，则跳过

            // 检查碰撞体是否是僵尸
            if (hit.CompareTag("Enemy")){
                ZombieHealth zombie= hit.GetComponent<ZombieHealth>();
                if (zombie != null)
                {
                    zombie.TakeDamage(explosionDamage, isPhysical); // 对僵尸造成伤害
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // 在场景视图中绘制爆炸范围的可视化圆圈
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
