using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SunFall : MonoBehaviour
{
    [Header("下落设置")]
    [Tooltip("下落速度")][SerializeField] private float fallSpeed = 1f;
    [Tooltip("落点Y坐标范围")][SerializeField] private Vector2 yRange = new Vector2(-4f, 1.5f);

    private float targetY;
    private bool isFalling = true;
    private void Awake()
    {
        targetY = Random.Range(yRange.x, yRange.y);
    }
    private void Update()
    {
        if(isFalling && transform.position.y > targetY)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        else
        {
            isFalling = false;
            float newY = targetY + Mathf.Sin(Time.time * 3f) * 0.1f;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
