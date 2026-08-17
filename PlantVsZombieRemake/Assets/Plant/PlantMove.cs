using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlantMove : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("植物的移动速度")]
    [SerializeField] private float moveSpeed = 5f; // 移动速度

    // 内部状态
    private Vector3 targetPosition; // 目标位置
    private bool isMoving = false; // 是否正在移动

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

        // 释放当前格位（立刻释放）
        if (CurrentCell != null)
        {
            CurrentCell.isHavingPlant = false;
            CurrentCell.plantOnCell = null;
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
            // 移动植物
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                transform.position = targetPosition; // 确保位置精确到目标位置

                // 到达后设置当前格子为移动目标格，并清理移动临时变量
                if (movingTargetCell != null)
                {
                    CurrentCell = movingTargetCell;
                    movingTargetCell = null;
                }
            }
        }
    }
}