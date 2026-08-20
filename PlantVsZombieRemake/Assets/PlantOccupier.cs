using UnityEngine;

public class PlantOccupier : MonoBehaviour
{
    // 持有的格子引用
    public CellHighLight OccupiedCell { get; private set; }

    // 将该植物与格子关联（用于非移动型植物）
    public void AssignCell(CellHighLight cell)
    {
        if (cell == null) return;

        // 如果之前占用了其他格子，先释放旧格子（仅在旧格子仍指向自己或未指向其他植物时释放）
        if (OccupiedCell != null && OccupiedCell != cell)
        {
            if (OccupiedCell.plantOnCell == null)
            {
                OccupiedCell.isHavingPlant = false;
            }
            else
            {
                // 如果旧格子记录的是移动植物的引用且等于自己上的 PlantMove，则清理
                PlantMove oldPm = GetComponent<PlantMove>();
                if (oldPm != null && OccupiedCell.plantOnCell == oldPm)
                {
                    OccupiedCell.plantOnCell = null;
                    OccupiedCell.isHavingPlant = false;
                }
            }
        }

        OccupiedCell = cell;

        // 只有在格子当前没有 plantOnCell（即非移动植物占位）时，才设置占用标记，避免覆盖移动植物引用
        if (OccupiedCell.plantOnCell == null)
        {
            OccupiedCell.isHavingPlant = true;
            // 对于非移动植物，plantOnCell 保持 null
        }
    }

    void OnDestroy()
    {
        if (OccupiedCell == null) return;

        // 如果格子的 plantOnCell 为 null（即非移动植物占位），则释放占位
        if (OccupiedCell.plantOnCell == null)
        {
            OccupiedCell.isHavingPlant = false;
        }
        else
        {
            // 若该 GameObject 本身挂有 PlantMove 并且格子指向它，则清理（保险措施）
            PlantMove pm = GetComponent<PlantMove>();
            if (pm != null && OccupiedCell.plantOnCell == pm)
            {
                OccupiedCell.plantOnCell = null;
                OccupiedCell.isHavingPlant = false;
            }
        }

        OccupiedCell = null;
    }
}