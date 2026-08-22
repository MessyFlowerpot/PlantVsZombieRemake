using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeIArmourHealth : MonoBehaviour
{
    [Header("一类防具设置")]
    [Tooltip("满耐久防具耐久")][SerializeField] private int maxArmourDurability = 370; //满耐久防具耐久
    private bool isArmourBroken;//防具是否破损
    private int armourDurability;//当前防具耐久

    private void Awake()
    {
        armourDurability = maxArmourDurability; //初始化防具耐久
        isArmourBroken = false; //初始化防具状态为未破损
    }

    public void ArmourTakeDamage(int damage)
    {
        if (isArmourBroken) return; //如果防具已经破损，直接返回
        if (armourDurability < damage) armourDurability = 0;
        else armourDurability -= damage;//减少防具耐久
        if (armourDurability <= 0)
        {
            ArmourBroke();
        }
    }

    private void ArmourBroke()
    {
        isArmourBroken = true;
        ZombieHealth zombie = GetComponentInParent<ZombieHealth>();
        zombie.TypeIArmourBroke(); //通知僵尸防具已破损
        Destroy(gameObject); //销毁防具预制体
    }

    public bool IsArmourBroken()
    {
        return isArmourBroken;
    }
}
