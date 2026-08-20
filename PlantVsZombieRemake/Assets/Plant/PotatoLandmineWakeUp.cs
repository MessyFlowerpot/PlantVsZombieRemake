using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoLandmineWakeUp : MonoBehaviour
{
    [Header("准备时间设置")]
    [Tooltip("从准备到激活的时间")][SerializeField] private float readyTime = 15f;//从准备到激活的时间
    private bool isReady = false;//是否准备好
    private GameObject visualNotReady;//未准备好时的视觉效果
    private GameObject visualReady;//准备好时的视觉效果

    private AOEExplosion damageAOE;//AOE爆炸组件

    void Awake()
    {
        bool isWrong = false;
        visualNotReady = transform.Find("Visual_NotReady").gameObject;//获取未准备好时的视觉效果
        visualReady = transform.Find("Visual_Ready").gameObject;//获取准备好时的视觉效果
        damageAOE = GetComponent<AOEExplosion>();//获取AOE爆炸组件

        // 检查是否找到视觉效果对象
        if (visualNotReady == null) Debug.LogError($"{name}物体下未找到Visual_NotReady子对象"); isWrong = true;
        if (visualReady == null) Debug.LogError($"{name}物体下未找到Visual_Ready子对象"); isWrong = true;
        if (damageAOE == null) Debug.LogError($"{name}物体上未找到AOEExplosion组件"); isWrong = true;

        // 检查是否能找到子物体，如果找不到则返回以避免后续代码报错
        if (isWrong) return;
    }

    void Start()
    {
        // 初始化视觉效果
        if (visualReady != null && visualNotReady != null)
        {
            visualNotReady.SetActive(true);
            visualReady.SetActive(false);
        }

        StartCoroutine(ReadyProcess());//开始准备过程
    }

    /// <summary>
    /// 准备过程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReadyProcess()
    {
        yield return new WaitForSeconds(readyTime);

        ReadyFinish();
    }

    /// <summary>
    /// 准备完成
    /// </summary>
    private void ReadyFinish()
    {
        isReady = true;

        // 切换视觉效果
        if (visualReady != null && visualNotReady != null)
        {
            visualNotReady.SetActive(false);
            visualReady.SetActive(true);
        }

        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;//如果collider存在，则启用碰撞器

            // 立即检测是否已有僵尸在碰撞区内（防止启用碰撞器时不会触发 OnTriggerEnter2D）
            Collider2D[] results = new Collider2D[10];
            ContactFilter2D filter = new ContactFilter2D();
            filter.NoFilter();
            int overlapCount = col.OverlapCollider(filter, results);
            for (int i = 0; i < overlapCount; i++)
            {
                var hit = results[i];
                if (hit != null && hit.CompareTag("Enemy"))
                {
                    if (damageAOE != null)
                    {
                        damageAOE.TriggerExplosion();
                    }
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isReady) return;//如果未准备好，则不触发
        // 检查碰撞体是否是僵尸
        if (collision.CompareTag("Enemy"))
        {
            // 触发爆炸
            if (damageAOE != null)
            {
                damageAOE.TriggerExplosion();
            }
            // 销毁地雷对象
            Destroy(gameObject);
        }
    }
}