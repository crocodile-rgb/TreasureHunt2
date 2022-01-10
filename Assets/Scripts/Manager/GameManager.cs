using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    [Header("元素预制体")]
    [Tooltip("透明背景")]
    public GameObject bgElement;
    [Tooltip("边界顺序:\n上，下，左，右，左上，右上，左下，右下")]
    public GameObject[] borderElements;
    public GameObject baseElement;
    public GameObject flagElement;
    public GameObject errorElement;

    [Header("特效预制体")]
    public GameObject smokeEffect;
    public GameObject uncoveredEffect;
    public GameObject goldEffect;

    [Header("图片资源")]
    public Sprite[] coverTileSprites;
    public Sprite[] trapSprites;
    public Sprite[] numberSprites;
    public Sprite[] toolSprites;
    public Sprite[] goldSprites;
    public Sprite[] bigwallSprites;
    public Sprite[] smallwallSprites;

    [Header("关卡设置")]
    public int w;
    public int h;
    public float minTrapProbability;
    public float maxTrapProbability;
    public float uncoveredProbability;

    //存放地图元素的数组
    public BaseElement[,] mapArray;
    private void Start()
    {
        CreateMap();
        ResetCamera();
        InitMap();
    }
    private void Awake()
    {
        _instance = this;
        mapArray = new BaseElement[w, h];
    }

    private void CreateMap()
    {
        Transform elementHolder = GameObject.Find("ElementHolder").transform;
        Transform bgHolder = GameObject.Find("ElementHolder/Background").transform;
        //创建元素
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                //背景方块
                Instantiate(bgElement, new Vector3(i, j, 0), Quaternion.identity, bgHolder);
                //可操作方块脚本并存于数组中
                mapArray[i, j] = Instantiate(baseElement, new Vector3(i, j, 0), Quaternion.identity, elementHolder).GetComponent<BaseElement>();
            }
        }
        //边界
        for (int i = 0; i < w; i++)
        {
            Instantiate(borderElements[0], new Vector3(i, h + 0.25f, 0), Quaternion.identity, bgHolder);
            Instantiate(borderElements[1], new Vector3(i, -1.25f, 0), Quaternion.identity, bgHolder);
        }
        for (int i = 0; i < h; i++)
        {
            Instantiate(borderElements[2], new Vector3(-1.25f, i, 0), Quaternion.identity, bgHolder);
            Instantiate(borderElements[3], new Vector3(w + 0.25f, i, 0), Quaternion.identity, bgHolder);
        }
        Instantiate(borderElements[4], new Vector3(-1.25f, h + 0.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderElements[5], new Vector3(w + 0.25f, h + 0.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderElements[6], new Vector3(-1.25f, -1.25f, 0), Quaternion.identity, bgHolder);
        Instantiate(borderElements[7], new Vector3(w + 0.25f, -1.25f, 0), Quaternion.identity, bgHolder);
    }

    private void ResetCamera()
    {
        Camera.main.orthographicSize = (h + 3) / 2f;//视口高度
        Camera.main.transform.position = new Vector3((w - 1) / 2f, (h - 1) / 2f, -10);
    }

    /// <summary>
    /// 初始化地图元素
    /// </summary>
    private void InitMap()
    {
        List<int> availableIndex = new List<int>();
        for (int i = 0; i < w * h; i++)
        {
            availableIndex.Add(i);
        }
        GenerateTrap(availableIndex);
        GenerateNumber(availableIndex);
    }
    /// <summary>
    /// 生成陷阱
    /// </summary>
    /// <param name="availableIndex"></param>
    private void GenerateTrap(List<int> availableIndex)
    {
        float trapProbability = Random.Range(minTrapProbability, maxTrapProbability);
        int trapNum = (int)(availableIndex.Count * trapProbability);
        for (int i = 0; i < trapNum; i++)
        {
            int tempIndex = availableIndex[Random.Range(0, availableIndex.Count)];
            int x, y;
            GetPosition(tempIndex, out x, out y);
            availableIndex.Remove(tempIndex);
            SetElement(tempIndex, ElementContent.Trap);
        }
    }

    /// <summary>
    /// 生成数字
    /// </summary>
    /// <param name="availableIndex"></param>
    private void GenerateNumber(List<int> availableIndex)
    {
        foreach (int i in availableIndex)
        {
            SetElement(i, ElementContent.Number);
        }
        availableIndex.Clear();
    }

    /// <summary>
    /// 设置元素脚本类型
    /// </summary>
    /// <param name="index">一维索引值</param>
    /// <param name="content">需要设置的类型</param>
    /// <returns>设置好新类型的组件</returns>
    private BaseElement SetElement(int index, ElementContent content)
    {
        int x, y;
        GetPosition(index, out x, out y);
        GameObject tempGo = mapArray[x, y].gameObject;
        Destroy(tempGo.GetComponent<BaseElement>());
        switch (content)
        {
            case ElementContent.Trap:
                mapArray[x, y] = tempGo.AddComponent<TrapElement>();
                break;
            case ElementContent.Number:
                mapArray[x, y] = tempGo.AddComponent<NumberElement>();
                break;
            case ElementContent.Tool:
                mapArray[x, y] = tempGo.AddComponent<ToolElement>();
                break;
            case ElementContent.Gold:
                mapArray[x, y] = tempGo.AddComponent<GoldElement>();
                break;
        }
        return mapArray[x, y];
    }

    /// <summary>
    /// 一维转二维
    /// </summary>
    /// <param name="index"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void GetPosition(int index, out int x, out int y)
    {
        y = index / w;
        x = index - y * w;
    }

    private int GetIndex(int x, int y)
    {
        return w * y + x;
    }
    /// <summary>
    /// 计算周围陷阱个数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int CountAdjacentTraps(int x, int y)
    {
        int count = 0;
        if (IsSameContent(x, y + 1, ElementContent.Trap)) count++;
        if (IsSameContent(x, y - 1, ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y, ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y, ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y + 1, ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y + 1, ElementContent.Trap)) count++;
        if (IsSameContent(x - 1, y - 1, ElementContent.Trap)) count++;
        if (IsSameContent(x + 1, y - 1, ElementContent.Trap)) count++;
        return count;
    }
    /// <summary>
    /// 判断某位置元素类型
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public bool IsSameContent(int x, int y, ElementContent content)
    {
        if (x > 0 && x < w && y > 0 && y < h)
        {
            return mapArray[x, y].elementContent == content;
        }
        return false;
    }

    public void FloodFillElement(int x, int y, bool[,] visited)
    {
        if (x >= 0 && x < w && y >= 0 && y < h)
        {
            if (visited[x, y]) return;
            if (mapArray[x, y].elementType != ElementType.CantCovered)
            {
                ((SingleCoveredElement)mapArray[x, y]).UncoveredElementSingle();
            }
            if (CountAdjacentTraps(x, y) > 0) return;
            if (mapArray[x, y].elementType == ElementType.CantCovered) return;
            visited[x, y] = true;
            FloodFillElement(x - 1, y, visited);
            FloodFillElement(x + 1, y, visited);
            FloodFillElement(x, y - 1, visited);
            FloodFillElement(x, y + 1, visited);
            FloodFillElement(x - 1, y - 1, visited);
            FloodFillElement(x + 1, y + 1, visited);
            FloodFillElement(x + 1, y - 1, visited);
            FloodFillElement(x - 1, y + 1, visited);
        }
    }
    /// <summary>
    /// kuaisutangchazhouweiquyu
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void UncoveredAdjacentElements(int x, int y)
    {
        int marked = 0;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < w && j >= 0 && j < h)
                {
                    if (mapArray[i, j].elementState == ElementState.Marked) marked++;
                    if (mapArray[i, j].elementState == ElementState.Uncovered && mapArray[i, j].elementContent == ElementContent.Trap) marked++;
                }
            }
        }
        if (CountAdjacentTraps(x, y) == marked)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i < w && j >= 0 && j < h)
                    {
                        if (mapArray[i, j].elementState != ElementState.Marked)
                        {
                            mapArray[i, j].OnPlayerStand();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// fan kai suo you xianjing
    /// </summary>
    public void DisplayAllTraps()
    {
        foreach(BaseElement element in mapArray)
        {
            if (element.elementContent == ElementContent.Trap)
            {
                ((TrapElement)element).UncoveredElementSingle();
            }
            if (element.elementContent != ElementContent.Trap && element.elementState == ElementState.Marked)
            {
                Instantiate(errorElement, element.transform);
            }
        }
    }
}
