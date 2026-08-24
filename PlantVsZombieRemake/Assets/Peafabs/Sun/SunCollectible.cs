using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCollectible : MonoBehaviour
{
    [Header("阳光属性")]
    [Tooltip("阳光点数")][SerializeField] private int sunPoint = 25;
    private void OnMouseDown()
    {
        SunBank.Instance.AddSun(sunPoint);
        Destroy(gameObject);
    }
}
