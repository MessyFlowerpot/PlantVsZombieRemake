using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaking : MonoBehaviour
{
    [Header("格子数量（列 x 行）")]
    public int columns = 9;
    public int rows = 5;

    [Header("地图世界尺寸（单位：世界坐标）")]
    [Tooltip("地图在 X 轴的总宽度")]
    public float worldWidth = 18f;
    [Tooltip("地图在 Y 轴的总高度")]
    public float worldHeight = 5f;

    [Header("格子预制体与父对象")]
    public GameObject tilePrefab;
    public Transform gridParent;

    [Header("生成设置")]
    public bool generateOnStart = true;

    // 运行时格子引用与数据
    private GameObject[,] tiles;
    public MapData mapData;

    private void Start()
    {
        if (generateOnStart)
            GenerateGrid(columns, rows);
    }

    // 对外调用：根据行列生成格子并适配大小
    public void GenerateGrid(int cols, int rws)
    {
        if (cols <= 0 || rws <= 0)
        {
            Debug.LogWarning("列或行必须大于 0");
            return;
        }

        columns = cols;
        rows = rws;
        mapData = new MapData(columns, rows);

        ClearGrid();

        tiles = new GameObject[columns, rows];

        float cellW = worldWidth / columns;
        float cellH = worldHeight / rows;

        // 以本物体位置为地图中心，计算左下角起点
        Vector3 center = transform.position;
        float startX = center.x - worldWidth / 2f + cellW / 2f;
        float startY = center.y - worldHeight / 2f + cellH / 2f;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = new Vector3(startX + x * cellW, startY + y * cellH, 0f);
                GameObject tile = null;
                if (tilePrefab != null)
                {
                    tile = Instantiate(tilePrefab, pos, Quaternion.identity, gridParent != null ? gridParent : this.transform);
                    tile.name = $"Tile_{x}_{y}";

                    // 尝试根据 Sprite 大小缩放以填充格子（如果存在 SpriteRenderer）
                    var sr = tile.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null && sr.sprite != null)
                    {
                        Vector2 spriteSize = sr.sprite.bounds.size;
                        if (spriteSize.x > 0.0001f && spriteSize.y > 0.0001f)
                        {
                            Vector3 localScale = tile.transform.localScale;
                            localScale.x = (cellW / spriteSize.x);
                            localScale.y = (cellH / spriteSize.y);
                            tile.transform.localScale = localScale;
                        }
                    }
                }
                else
                {
                    // 如果没有预制体，用空物体占位
                    tile = new GameObject($"Tile_{x}_{y}");
                    tile.transform.position = pos;
                    tile.transform.parent = gridParent != null ? gridParent : this.transform;
                }

                tiles[x, y] = tile;
                // 初始化数据数组（示例：0 为空地）
                mapData.mapArray[x, y] = 0;
            }
        }
    }

    // 清除已有格子（编辑器/运行时都支持）
    public void ClearGrid()
    {
        if (tiles != null)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
                for (int j = 0; j < tiles.GetLength(1); j++)
                    if (tiles[i, j] != null)
                        DestroyImmediate(tiles[i, j]);
        }
        // 若使用 Instantiate 而非编辑器下创建，运行时应用 Destroy 而非 DestroyImmediate
        // 这里简单重置引用
        tiles = null;
    }
}

public class MapData
{
    public int width;
    public int height;
    public int[,] mapArray;
    public MapData(int width, int height)
    {
        this.width = width;
        this.height = height;
        mapArray = new int[width, height];
    }
}