using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PeaMove))] //确保该脚本所在的游戏对象上有PeaMove组件以防止在运行时出现漏洞
public class IceEffect : MonoBehaviour
{
    [Header("寒冰效果附加设置")]
    [Tooltip("寒冰效果的减速幅度(单位:100%)")][SerializeField] private float slowFactor = 0.5f;//寒冰效果的减速幅度(具体的值:100% * slowFactor)
    [Tooltip("寒冰效果的持续时间(单位:秒)")][SerializeField] private float duration = 5f;//寒冰效果的持续时间

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other == null) return;
        if (other.CompareTag("Enemy"))
        {
            var zombie = other.GetComponent<ZombieMove>();
            zombie.SlowDown(slowFactor, duration);
        }
    }
}
