using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    List<Map> _mapList;

    // Debug
    bool _isFirst;
    // ~Debug

    public void AddMap(Map map)
    {
        if (_mapList == null)
        {
            _mapList = new List<Map>();
        }
        _mapList.Add(map);
    }

    public bool SearchRoute(int mapId, Vector2Int startNodeId, Vector2Int targetNodeId, List<Vector2Int> routeList)
    {
        if (IsOutOfRangeMapId(mapId) == false)
        {
            return false;
        }
        return _mapList[mapId].SearchRoute(startNodeId, targetNodeId, routeList);
    }

    public bool SearchRouteRandom(int mapId, Vector2Int startNodeId, List<Vector2Int> routeList)
    {
        if (IsOutOfRangeMapId(mapId) == false)
        {
            return false;
        }

        Vector2Int nodeId = Vector2Int.zero;
        int debugCount = 0;
        while (true)
        {
            int x = Random.Range(0, Map.kMapWidth);
            int y = Random.Range(0, Map.kMapHeight);
            nodeId = new Vector2Int(y, x);

            if (Map.kMapData[y, x] == 0
            && startNodeId != nodeId)
            {
                break;
            }

            debugCount++;
            if (debugCount >= 1000)
            {
                break;
            }
        }

        if (_isFirst == false)
        {
            nodeId = new Vector2Int(6, 1);
            _isFirst = true;
        }

        return _mapList[mapId].SearchRoute(startNodeId, nodeId, routeList);
    }

    public bool SearchRouteRandomAsync(int mapId, Vector2Int startNodeId, List<Vector2Int> routeList)
    {
        if (IsOutOfRangeMapId(mapId) == false)
        {
            return false;
        }

        Vector2Int nodeId = Vector2Int.zero;
        int debugCount = 0;
        while (true)
        {
            int x = Random.Range(0, Map.kMapWidth);
            int y = Random.Range(0, Map.kMapHeight);
            nodeId = new Vector2Int(y, x);

            if (Map.kMapData[y, x] == 0
            && startNodeId != nodeId)
            {
                break;
            }

            debugCount++;
            if (debugCount >= 1000)
            {
                break;
            }
        }

        if (_isFirst == false)
        {
            nodeId = new Vector2Int(6, 1);
            _isFirst = true;
        }

        _mapList[mapId].SearchRouteAsync(startNodeId, nodeId, routeList);
        return true;
    }

    public MapChip GetNode(int mapId, Vector2Int nodeId)
    {
        if (IsOutOfRangeMapId(mapId) == false)
        {
            return null;
        }

        return _mapList[mapId].GetNode(nodeId);
    }

    bool IsOutOfRangeMapId(int id)
    {
        if (id < 0 || id >= _mapList.Count)
        {
            Debug.LogError($"範囲外のマップIDです。mapId={id}");
            return false;
        }
        return true;
    }
}
