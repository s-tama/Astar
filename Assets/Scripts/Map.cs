using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum MapChipType
    {
        Normal,
        Obstacle
    }

    public const int kMapWidth = 8;
    public const int kMapHeight = 8;

    //public static readonly int[,] kMapData = new int[kMapHeight, kMapWidth]
    //{
    //    {1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    //    {1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
    //    {1,1,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1},
    //    {1,1,1,1,1,1,1,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
    //    {1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
    //    {1,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
    //    {1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,1,1,1,0,0,1,1,1,1,1,1},
    //    {1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,1,1,1,0,0,1,1,1,1,1,1},
    //    {1,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
    //    {1,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
    //    {1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,1,1,1,0,0,1,1,1,1,0,1,0,0,1},
    //    {1,1,1,1,1,1,0,0,0,0,0,0,1,1,1,0,0,1,1,1,0,0,1,1,1,1,0,1,0,0,1},
    //    {1,1,1,1,1,1,1,0,1,1,0,0,1,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,1},
    //    {1,1,1,1,1,1,1,0,1,1,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,1,0,0,1},
    //    {1,1,1,1,1,0,0,0,0,1,0,0,0,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,0,0,1},
    //    {1,1,1,1,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,0,0,1,1,1,1,1,1,0,0,1},
    //    {1,1,1,1,1,0,0,0,0,1,0,0,1,1,1,1,1,0,1,1,0,0,0,0,0,0,0,0,0,0,1},
    //    {1,1,1,1,1,0,0,0,0,1,0,0,1,1,1,1,1,0,1,1,0,0,0,0,0,0,0,0,0,0,1},
    //    {1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1},
    //    {1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
    //    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
    //};

    public static readonly int[,] kMapData = new int[kMapHeight, kMapWidth]
    {
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 1, 1, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 1, 0 },
        { 1, 1, 1, 0, 0, 0, 1, 0 },
        { 1, 0, 0, 0, 0, 1, 1, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0 }
    };

    [SerializeField] MapChip _mapChipPrefab;
    [SerializeField] bool _isDiagonal = false;

#if !MAP_3D
    // DEBUG
    // ターゲットノードの可視化用
    [SerializeField] GameObject _target = null;
    // ~DEBUG
#endif

#if MAP_3D
    [SerializeField] GameObject _goalFlag = null;
#endif

    List<MapChip> _nodeList;
    List<MapChip> _openList;
    List<MapChip> _closedList;

    MapChip _targetNode;

    public MapChip CurrentTargetNode => _targetNode;

    void Awake()
    {
        MapManager.Instance.AddMap(this);
        Init();
    }

    void Start()
    {
        // DEBUG
#if !MAP_3D
        _target.SetActive(false);
#endif
        // ~DEBUG

        _nodeList = new List<MapChip>();
        _openList = new List<MapChip>();
        _closedList = new List<MapChip>();

        // 行インデックス
        for (int row = 0; row < kMapHeight; row++)
        {
            // 列インデックス
            for (int column = 0; column < kMapWidth; column++)
            {
                int chipId = kMapData[row, column];

                MapChip chip = Instantiate(_mapChipPrefab, transform);
                chip.Init(row, column, chipId);
#if MAP_3D
                chip.SetPosition(column - kMapWidth / 2 + 0.5f, 0, (row - kMapHeight / 2) * -1 - 0.5f);
                switch (chipId)
                {
                    case 0: chip.SetActive(true); break;
                    case 1: chip.SetActive(false); break;
                    default: Debug.LogError($"マップチップIDが正しくありません。id={chipId}"); break;
                }
#else
                chip.SetPosition(column - kMapWidth / 2 + 0.5f, (row - kMapHeight / 2) * -1 - 0.5f);
                SetNodeColor(chip);
#endif
                _nodeList.Add(chip);
            }
        }
    }

    void Update()
    {
        /*
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider)
        {
            if (!hit.collider.CompareTag("MapChip"))
            {
                return;
            }

            MapChip node = hit.collider.GetComponent<MapChip>();
            Debug.Log($"cost={node.Cost}, hcost={node.HCost}, score={node.Score}");

            List<Vector2Int> routeList = new List<Vector2Int>();
            SearchRoute(new Vector2Int(4, 1), node.NodeId, routeList);
            string result = string.Empty;
            for (int i = 0; i < routeList.Count; i++)
            {
                Vector2Int route = routeList[i];
                result += $"({route.x}, {route.y})";
                if (i != routeList.Count - 1)
                {
                    result += " -> ";
                }
            }
            Debug.Log(result);
        }
        */
    }

    public virtual void Init()
    {
#if MAP_3D
        _goalFlag.SetActive(false);
#endif
    }

    public bool SearchRoute(Vector2Int startNodeId, Vector2Int targetNodeId, List<Vector2Int> routeList)
    {
        ResetNode();
        routeList.Clear();

        // 位置判定
        if(startNodeId == targetNodeId)
        {
            Debug.Log("開始位置と終了位置が同じです。");
            return false;
        }

        // ノードデータ更新
        int length = _nodeList.Count;
        for (int i = 0; i < length; i++)
        {
            SetNodeColor(_nodeList[i]);
            _nodeList[i].ResetNode();
            _nodeList[i].UpdateNode(startNodeId, targetNodeId, _isDiagonal);
        }

        // ターゲットノード設定
        MapChip targetNode = GetNode(targetNodeId);
        targetNode.SetColor(Color.blue);
        _targetNode = targetNode;

#if MAP_3D
        _goalFlag.transform.position = new Vector3(_targetNode.WorldPosition.x, _goalFlag.transform.position.y, _targetNode.WorldPosition.z);
        _goalFlag.SetActive(true);
#endif

        // 開始ノード初期化
        MapChip node = GetNode(startNodeId);
        _openList.Add(node);

        int count = 0;
        while (true)
        {
            MapChip bestNode = GetBestNode();
            CloseNode(bestNode);
            OpenNode(bestNode.NodeId, targetNodeId);
            if(bestNode.NodeId == targetNodeId)
            {
                break;
            }

            // デバッグ用
            count++;
            if (count >= 10000)
            {
                Debug.LogError("タイムアウト");
                break;
            }
        }

        // ルート取得
        CreateRoute(targetNode, routeList, 30);

        return true;
    }

    void CreateRoute(MapChip targetNode, List<Vector2Int> routeList, int depth)
    {
        if (depth > 0)
        {
            depth--;
            if (depth <= 0)
            {
                Debug.LogError("タイムアウト");
                return;
            }
        }

        if (targetNode.ParentNode == null)
        {
            return;
        }

        if (routeList == null)
        {
            routeList = new List<Vector2Int>();
        }

        routeList.Add(targetNode.NodeId);
        CreateRoute(targetNode.ParentNode, routeList, depth);
    }

    MapChip GetBestNode()
    {
        MapChip result = null;
        float min = float.MaxValue;
        foreach (var node in _openList)
        {
            if (min > node.Score)
            {
                min = node.Score;
                result = node;
            }
        }
        return result;
    }

    void OpenNode(Vector2Int bestNodeId, Vector2Int targetNodeId)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                if (_isDiagonal == false)
                {
                    if (x == -1 && y == -1
                    || x == 1 && y == 1
                    || x == 1 && y == -1
                    || x == -1 && y == 1)
                    {
                        continue;
                    }
                }

                int cx = bestNodeId.x + x;
                int cy = bestNodeId.y + y;

                if (cx < 0 
                ||  cy < 0
                ||  cx >= kMapHeight 
                ||  cy >= kMapWidth)
                {
                    continue;
                }

                MapChip node = GetNode(new Vector2Int(cx, cy));
                if(node == null)
                {
                    Debug.LogError("NodeがNULLです");
                }
                if (node.NodeType == 1 || node.CurrentStatus != MapChip.Status.None)
                {
                    continue;
                }
                    
                node.Open();
                node.Cost = GetNode(bestNodeId).Cost + 1;
                node.ParentNode = GetNode(bestNodeId);
                _openList.Add(node);
            }
        }
    }

    public MapChip GetNode(Vector2Int nodeId)
    {
        foreach(var node in _nodeList)
        {
            if(node.NodeId == nodeId)
            {
                return node;
            }
        }
        return null;
    }

    void CloseNode(MapChip node)
    {
        node.Close();
        _openList.Remove(node);
        _closedList.Add(node);
    }

    void ResetNode()
    {
        _openList.Clear();
        _closedList.Clear();
    }

    public void SearchRouteAsync(Vector2Int startNodeId, Vector2Int targetNodeId, List<Vector2Int> routeList)
    {
        StartCoroutine(SearchRouteCoroutine(startNodeId, targetNodeId, routeList));
    }

    IEnumerator SearchRouteCoroutine(Vector2Int startNodeId, Vector2Int targetNodeId, List<Vector2Int> routeList)
    {
        ResetNode();
        routeList.Clear();

        // 位置判定
        if (startNodeId == targetNodeId)
        {
            Debug.Log("開始位置と終了位置が同じです。");
            yield break;
        }

        // ノードデータ更新
        int length = _nodeList.Count;
        for (int i = 0; i < length; i++)
        {
            SetNodeColor(_nodeList[i]);
            _nodeList[i].ResetNode();
            _nodeList[i].UpdateNode(startNodeId, targetNodeId, _isDiagonal);
        }

        // ターゲットノード設定
        MapChip targetNode = GetNode(targetNodeId);
#if !MAP_3D
        // DEBUG
        _target.transform.position = targetNode.WorldPosition;
        _target.SetActive(true);
        // ~DEBUG
#endif
        _targetNode = targetNode;

        // 開始ノード初期化
        MapChip node = GetNode(startNodeId);
        _openList.Add(node);

        int count = 0;
        while (true)
        {
            MapChip bestNode = GetBestNode();
            bestNode.SetColor(0.8f, 0.8f, 0.8f);
            yield return new WaitForSeconds(0.5f);
            CloseNode(bestNode);
            OpenNode(bestNode.NodeId, targetNodeId);
            if (bestNode.NodeId == targetNodeId)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);

            // デバッグ用
            count++;
            if (count >= 10000)
            {
                Debug.LogError("タイムアウト");
                break;
            }
        }

        // ルート取得
        CreateRoute(targetNode, routeList, 30);

        // DEBUG
        // ターゲットまでのルートを色付け
        foreach(var nodeId in routeList)
        {
            MapChip routeNode = GetNode(nodeId);
            routeNode.SetColor(new Color(1, 0.8f, 0.8f, 1));
        }
        // ~DEBUG
    }

    void SetNodeColor(MapChip node)
    {
        int nodeType = node.NodeType;
        switch (nodeType)
        {
            case 0: node.SetColor(Color.green); break;
            case 1: node.SetColor(Color.gray); break;
            default: Debug.LogError($"マップチップIDが正しくありません。nodeType={nodeType}"); break;
        }
    }

    IEnumerator OpenNodeCoroutine(Vector2Int bestNodeId, Vector2Int targetNodeId)
    {
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                if (_isDiagonal == false)
                {
                    if (x == -1 && y == -1
                    || x == 1 && y == 1
                    || x == 1 && y == -1
                    || x == -1 && y == 1)
                    {
                        continue;
                    }
                }

                int cx = bestNodeId.x + x;
                int cy = bestNodeId.y + y;

                if (cx < 0
                || cy < 0
                || cx >= kMapHeight
                || cy >= kMapWidth)
                {
                    continue;
                }

                MapChip node = GetNode(new Vector2Int(cx, cy));
                if (node == null)
                {
                    Debug.LogError("NodeがNULLです");
                }
                if (node.NodeType == 1 || node.CurrentStatus != MapChip.Status.None)
                {
                    continue;
                }

                node.Open();
                node.Cost = GetNode(bestNodeId).Cost + 1;
                node.ParentNode = GetNode(bestNodeId);
                _openList.Add(node);

                // DEBUG
                yield return new WaitForSeconds(1);
                // ~DEBUG
            }
        }
    }
}
