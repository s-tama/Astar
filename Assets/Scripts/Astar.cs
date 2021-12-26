using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    public class Node
    {
        Vector2Int _nodeId;
        float _cost;
        float _hcost;
        float _score;
        int _nodeType;
        Node _parentNode;
        Vector2 _worldPosition;

        public Action<float, float, float> onChangeParam;

        public Vector2Int Id { get => _nodeId; set => _nodeId = value; }
        public float Cost => _cost;
        public float HCost => _hcost;
        public float Score => _score;
        public Vector2 WorldPosition => _worldPosition;

        public void Init()
        {
            _cost = -1;
            _hcost = -1;
            _score = -1;
            _parentNode = null;
        }

        public void Update(Node startNode, Node targetNode, bool isDiagonal = false)
        {
            float cost = CalcCost(startNode, isDiagonal);
            float hcost = CalcHCost(targetNode, isDiagonal);
            ChangeParam(cost, hcost);
        }

        public void ChangeParam(float cost, float hcost)
        {
            _cost = cost;
            _hcost = hcost;
            _score = _cost + _hcost;

            onChangeParam?.Invoke(_cost, _hcost, _score);
        }

        public void ChangeParam(float cost)
        {
            ChangeParam(cost, _hcost);
        }

        public void Reset()
        {
            _parentNode = null;
        }

        float CalcCost(Node startNode, bool isDiagonal)
        {
            float dx = Mathf.Abs(_worldPosition.x - startNode.WorldPosition.x);
            float dy = Mathf.Abs(_worldPosition.y - startNode.WorldPosition.y);
            return isDiagonal ? Mathf.Sqrt(dx * dx + dy * dy) : dx + dy; 
        }

        float CalcHCost(Node targetNode, bool isDiagonal)
        {
            float dx = Mathf.Abs(targetNode.WorldPosition.x - _worldPosition.y);
            float dy = Mathf.Abs(targetNode.WorldPosition.y - _worldPosition.y);
            return isDiagonal ? Mathf.Sqrt(dx * dx + dy * dy) : dx + dy;
        }
    }

    List<Node> _nodeList;
    List<Node> _openList;
    List<Node> _closedList;
    Vector2Int _size;

    public Astar()
    {
        
    }

    public void Init(Vector2Int size)
    {
        _nodeList = new List<Node>();
        _openList = new List<Node>();
        _closedList = new List<Node>();
        _size = size;
    }

    public void CreateNode(Vector2Int id, Vector2 pos)
    {
        Node node = new Node();
        node.Id = id;
    }

    public bool SearchRoute(Vector2Int startNodeId, Vector2Int targetNodeId, List<Vector2> routeList)
    {
        return SearchRoute(FindNode(startNodeId), FindNode(targetNodeId), routeList);
    }

    public bool SearchRoute(Node startNode, Node targetNode, List<Vector2> routeList)
    {
        // 初期位置判定
        if (startNode.Id == targetNode.Id)
        {
            Debug.Log("開始地点と終了地点が同じです。");
            return false;
        }

        // ノードデータ更新
        foreach (var node in _nodeList)
        {
            node.Reset();
            node.Update(startNode, targetNode);
        }

        // 開始ノードをオープンリストに登録
        _openList.Add(startNode);

        // ルート検索
        while (true)
        {
            Node bestNode = FindBestNode();
            CloseNode(bestNode);

        }

        return true;
    }

    public Node FindBestNode()
    {
        Node result = null;
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

    public void OpenNode(Node bestNode)
    {
        //for (int x = -1; x < 2; x++)
        //{
        //    for (int y = -1; y < 2; y++)
        //    {
        //        if (x == 0 && y == 0)
        //        {
        //            continue;
        //        }

        //        int cx = bestNode.Id.x + x;
        //        int cy = bestNode.Id.y + y;

        //        if (cx < 0
        //        || cy < 0
        //        || cx >= _size.x
        //        || cy >= _size.y)
        //        {
        //            continue;
        //        }

        //        Node node = FindNode(new Vector2Int(cx, cy));
        //        if (node == null)
        //        {
        //            Debug.LogError("NodeがNULLです");
        //        }
        //        if (node.NodeType == 1 || node.CurrentStatus != MapChip.Status.None)
        //        {
        //            continue;
        //        }

        //        node.Open();
        //        node.Cost = GetNode(bestNodeId).Cost + 1;
        //        node.ParentNode = GetNode(bestNodeId);
        //        _openList.Add(node);
        //    }
        //}
    }

    public void CloseNode(Node node)
    {
        _openList.Remove(node);
        _closedList.Add(node);
    }

    public Node FindNode(Vector2Int nodeId)
    {
        return _nodeList.Find((node) => node.Id == nodeId);
    }
}
