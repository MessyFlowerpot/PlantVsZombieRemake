using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [Header("UI颜色提示")]
    [Tooltip("体力充足时的颜色")][SerializeField] private Color fullColor = Color.green; // 体力充足时的颜色
    [Tooltip("体力低时的颜色")][SerializeField] private Color lowColor = Color.yellow; // 体力低时的颜色
    [Tooltip("疲劳时的颜色")][SerializeField] private Color tiredColor = Color.red; // 疲劳时的颜色

    [Header("体力阈值")]
    [Tooltip("体力低时的阈值")][SerializeField] private float lowThreshold = 0.3f; // 体力低时的阈值
    [Tooltip("疲劳时的阈值")][SerializeField] private float tiredThreshold = 0.15f; // 疲劳时的阈值

    [Header("淡出与显示设置")]
    [Tooltip("淡出延迟时间")][SerializeField] private float hideDelay = 1.5f; // 淡出延迟时间
    [Tooltip("淡出速度")][SerializeField] private float fadeSpeed = 1f; // 淡出速度

    [Header("边框设置")]
    [SerializeField] private Color borderColor = Color.gray;    // 边框颜色
    [SerializeField] private float borderWidth = 2f;             // 边框粗细

    //内部设置
    private Slider slider;// 体力条 Slider 组件引用
    private Image fillImage;// 体力条填充图像
    private CanvasGroup canvasGroup;// CanvasGroup 组件引用，用于控制 UI 的透明度
    private float currentPercentage = 1f;// 当前体力百分比
    private float hideTimer = 0f;// 淡出计时器
    private bool isPlantMoving = false;// 植物是否正在移动
    private RectTransform borderRect;// 边框的 RectTransform 引用

    void Awake()
    {
        slider = GetComponent<Slider>();// 尝试获取 Slider 组件
        if(slider != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();// 尝试从 Slider 的 fillRect 获取
        }

        canvasGroup = GetComponent<CanvasGroup>();// 尝试获取 CanvasGroup 组件
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();// 如果没有 CanvasGroup 组件，则添加一个
        }

        CreateBorder();// 创建边框
    }

    private void Update()
    {
        if(currentPercentage > 0.999f && !isPlantMoving)
        {
            hideTimer += Time.deltaTime;
            if(hideTimer > hideDelay)
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
            }
        }
        else
        {
            hideTimer = 0f;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
        }
    }
    /// <summary>
    /// 更新体力条的显示
    /// </summary>
    /// <param name="currentStamina"></param>
    /// <param name="maxStamina"></param>
    public void UpdateStamina(float currentStamina, float maxStamina,bool moving)
    {
        if (slider == null || maxStamina <= 0) return; // 避免除以零或未初始化的 Slider

        isPlantMoving = moving; // 更新植物移动状态

        float percentage = currentStamina / maxStamina;
        currentPercentage = percentage;
        slider.value = percentage; // 更新 Slider 的值

        UpdateColor(percentage);// 根据体力百分比更新体力条的颜色
    }

    /// <summary>
    /// 根据体力百分比更新体力条的颜色
    /// </summary>
    /// <param name="percentage"></param>
    public void UpdateColor(float percentage)
    {
        if (fillImage == null) return;

        if (percentage > lowThreshold)
        {
            fillImage.color = fullColor;
        }
        else if (percentage > tiredThreshold)
        {
            fillImage.color = lowColor;
        }
        else
        {
            fillImage.color = tiredColor;
        }
    }

    private void CreateBorder()
    {
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(this.transform, false); // 设为 Slider 的子物体，且不继承缩放

        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor; // 设置边框颜色
        borderRect = borderObj.GetComponent<RectTransform>();

        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-borderWidth, -borderWidth); // 向左下扩展
        borderRect.offsetMax = new Vector2(borderWidth, borderWidth);   // 向右上扩展

        borderObj.transform.SetAsFirstSibling();
    }
}
