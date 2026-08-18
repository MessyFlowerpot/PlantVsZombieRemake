using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlantMove : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("植物的移动速度")]
    [SerializeField] private float originalSpeed = 5f; // 原始移动速度

    [Tooltip("疲劳时降低的速度倍率")]
    [SerializeField] private float tiredSpeedMultiplier = 0.7f; // 疲劳时的移动速度倍率
    private float currentSpeed; // 移动速度

    // 内部状态
    private Vector3 targetPosition; // 目标位置
    private float strength;//植物体力
    private float maxStrength = 100f;//植物最大体力
    private float TiredThreshold = 0.15f; // 疲劳阈值
    private float strengthDecreaseRate = 5f; // 每秒体力消耗量
    private float normalStrengthRecoveryRate = 2f; // 正常每秒体力恢复量
    private float slowlyStrengthRecoveryRate = 1.5f; // 缓慢每秒体力恢复量
    private bool isMoving = false; // 是否正在移动
    private bool isTired = false; // 是否疲劳（体力低于阈值）

    // 恢复调试相关
    private bool isRecovering = false;
    private float recoveryStartStrength = 0f;
    private bool recoveryStartIsTired = false;
    private Coroutine recoveryDebugCoroutine = null;

    // 选中视觉相关
    private SpriteRenderer sr;
    private Color originalColor;
    private bool isSelected = false;

    // 当前所在格子引用
    public CellHighLight CurrentCell { get; private set; }

    // 移动目标格子（在移动过程中记录）
    private CellHighLight movingTargetCell = null;

    void Start()
    {
        strength = maxStrength;// 初始化体力为最大值
        currentSpeed = originalSpeed;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }
    }


    /// <summary>
    /// 将该植物与格子关联（用于在生成时设置格子占用）
    /// </summary>
    public void AssignCell(CellHighLight cell)
    {
        if (cell == null) return;

        // 如果之前有格子，先清除旧格子的占用与引用
        if (CurrentCell != null && CurrentCell != cell)
        {
            CurrentCell.isHavingPlant = false;
            CurrentCell.plantOnCell = null;
        }

        CurrentCell = cell;
        CurrentCell.isHavingPlant = true;
        CurrentCell.plantOnCell = this;
    }

    /// <summary>
    /// 尝试移动到目标格子（会在移动前检测目标格子是否可用，并在移动期间预留该格位）
    /// 返回 true 表示移动已发起
    /// </summary>
    public bool TryMoveToCell(CellHighLight targetCell)
    {
        if (targetCell == null)
        {
            Debug.LogWarning("目标格子为空，不能移动。");
            return false;
        }

        // 如果目标格子就是当前格子，不进行移动
        if (targetCell == CurrentCell)
        {
            return false;
        }

        // 检测目标格子是否已有植物（若已被占用则不能移动）
        if (targetCell.isHavingPlant)
        {
            Debug.LogWarning("目标格子已被占用，不能移动。");
            return false;
        }

        // 预占目标格位并设置引用，避免在移动过程中被其它操作占用
        targetCell.isHavingPlant = true;
        targetCell.plantOnCell = this;

        // 释放当前格位（立刻释放），并清除当前引用以保持一致性
        if (CurrentCell != null)
        {
            CurrentCell.isHavingPlant = false;
            CurrentCell.plantOnCell = null;
            CurrentCell = null;
        }

        // 记录移动目标格子
        movingTargetCell = targetCell;
        MoveTo(targetCell.transform.position);
        return true;
    }

    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
        // 初始化速度为原速，后续在 Update 中根据体力调整
        currentSpeed = originalSpeed;
        // 停止恢复状态（如果之前在恢复）
        isRecovering = false;
    }

    /// <summary>
    /// 设置选中状态（视觉反馈）
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (sr != null)
        {
            sr.color = isSelected ? Color.yellow : originalColor;
        }
    }

    /// <summary>
    /// 点击植物则选中 / 取消选中（由 SelectPlantController 处理逻辑）
    /// </summary>
    void OnMouseDown()
    {
        if (SelectPlantController.instance != null)
        {
            // 选中植物时再额外检测一次：若当前所在格子不一致或被误标记则修正
            if (CurrentCell != null && !CurrentCell.isHavingPlant)
            {
                CurrentCell.isHavingPlant = true;
                CurrentCell.plantOnCell = this;
            }

            SelectPlantController.instance.SelectPlant(this);
        }
    }

    /// <summary>
    /// 每帧调用一次处理植物的移动逻辑
    /// </summary>
    void Update()
    {
        if (isMoving)
        {
            // 判断植物是否疲劳（体力低于阈值）
            isTired = strength <= maxStrength * TiredThreshold;
            currentSpeed = isTired ? originalSpeed * tiredSpeedMultiplier : originalSpeed;

            if (strength > 0f)
            {
                // 如果植物有体力，则按当前速度移动并消耗体力
                strength -= strengthDecreaseRate * Time.deltaTime;
                strength = Mathf.Max(0f, strength);
            }
            else
            {
                Debug.Log($"{name}体力耗尽了!");
            }

            // 移动植物
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                transform.position = targetPosition; // 确保位置精确到目标位置

                // 到达后设置当前格子为移动目标格，并清理移动临时变量
                if (movingTargetCell != null)
                {
                    CurrentCell = movingTargetCell;
                    // 确保格子引用一致
                    CurrentCell.isHavingPlant = true;
                    CurrentCell.plantOnCell = this;
                    movingTargetCell = null;
                }
            }

            // 移动时不处于恢复状态
            isRecovering = false;
        }
        else
        {
            if (strength < maxStrength)
            {
                // 恢复开始的转变检测：只有从非恢复到恢复时触发一次初始调试日志和计时协程
                if (!isRecovering)
                {
                    isRecovering = true;
                    recoveryStartStrength = strength;
                    recoveryStartIsTired = isTired;
                    Debug.Log($"[恢复开始] 初始体力: {recoveryStartStrength:F2}, 是否疲劳: {recoveryStartIsTired}");
                    if (recoveryDebugCoroutine == null)
                    {
                        recoveryDebugCoroutine = StartCoroutine(RecoveryDebugCoroutine(20f));
                    }
                }

                ResumeStrength(isTired);
            }
            else
            {
                // 已恢复到满值，结束恢复状态
                isRecovering = false;
            }
        }
    }

    /// <summary>
    /// 恢复体力（根据是否疲劳调整恢复速度）
    /// </summary>
    /// <param name="tiredFlag"></param>
    public void ResumeStrength(bool tiredFlag)
    {
        float recoveryRate = tiredFlag ? slowlyStrengthRecoveryRate : normalStrengthRecoveryRate;
        strength += recoveryRate * Time.deltaTime;
        // 确保体力不会超过最大值
        strength = Mathf.Min(maxStrength, strength);
        // 若体力恢复到高于疲劳阈值，更新疲劳状态
        if (strength > maxStrength * TiredThreshold)
        {
            isTired = false;
        }
    }

    // 调试协程：在指定秒数后打印当前体力（不重复启动）
    private IEnumerator RecoveryDebugCoroutine(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        Debug.Log($"[恢复报告] {name} 在{waitSeconds}秒后当前体力: {strength:F2} (开始时: {recoveryStartStrength:F2}, 开始时疲劳: {recoveryStartIsTired})");
        recoveryDebugCoroutine = null;
    }
}