using System;
using UnityEngine;

public class MapChip : MonoBehaviour
{
    public Action onOpen;
    public Action onClose;

    public enum Status
    {
        None,
        Open,
        Closed
    }

    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] GameObject _open;
    [SerializeField] GameObject _closed;

    Vector2Int _nodeId;
    float _cost;
    float _hcost;
    int _nodeType;
    Status _currentStatus;
    MapChip _parentNode;

    public Action<float, float, float> onChangeParam;
    public Action<Status> onChangeStatus;

    public Vector2Int NodeId => _nodeId;
    public int NodeType => _nodeType;
    public float Cost 
    { 
        get => _cost;
        set
        {
            _cost = value;
            onChangeParam?.Invoke(_cost, _hcost, Score);
        } 
    }
    public float HCost => _hcost;
    public float Score => _cost + _hcost;
    public Status CurrentStatus => _currentStatus;
    public MapChip ParentNode { get => _parentNode; set => _parentNode = value; }
    public Vector3 WorldPosition => transform.position;
    public Vector3 LocalPosition => transform.localPosition;

#if UNITY_EDITOR
    void Reset()
    {
        int childLength = transform.childCount;
        for(int i = 0; i < childLength; i++)
        {
            Transform child = transform.GetChild(i);
            switch (child.name)
            {
                case "Sprite": _spriteRenderer = child.GetComponent<SpriteRenderer>(); break;
                case "Sprite_Open": _open = child.gameObject; break;
                case "Sprite_Closed": _closed = child.gameObject; break;
                default: break;
            }
        }
    }
#endif

    public void Init(int x, int y, int nodeType)
    {
        _nodeId = new Vector2Int(x, y);
        _nodeType = nodeType;
        _currentStatus = Status.None;
        _parentNode = null;

        if (_open != null)
        {
            _open.SetActive(false);
        }
        if (_closed != null)
        {
            _closed.SetActive(false);
        }
    }

    public void UpdateNode(Vector2Int startNodeId, Vector2Int targetNodeId, bool isDiagonal)
    {
        int dx, dy;
        dx = Mathf.Abs(_nodeId.x - startNodeId.x);
        dy = Mathf.Abs(_nodeId.y - startNodeId.y);
        _cost = isDiagonal ? Mathf.Max(dx, dy) : dx + dy;

        dx = Mathf.Abs(targetNodeId.x - _nodeId.x);
        dy = Mathf.Abs(targetNodeId.y - _nodeId.y);
        _hcost = isDiagonal ? Mathf.Max(dx, dy) : dx + dy;

        onChangeParam?.Invoke(_cost, _hcost, Score);
    }

    public void ResetNode()
    {
        _currentStatus = Status.None;
        _parentNode = null;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, 0);
    }

    public void SetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }

    public void SetColor(float r, float g, float b, float a = 1)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.color = new Color(r, g, b, a);
    }

    public void SetColor(Color color)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.color = color;
    }

    public void SetAlpha(float value)
    {
        if (_spriteRenderer == null) return;
        Color color = _spriteRenderer.color;
        _spriteRenderer.color = new Color(color.a, color.g, color.b, value);
    }

    public void Open()
    {
        SetStatus(Status.Open);
        //_open.SetActive(true);
        //_closed.SetActive(false);
    }

    public void Close()
    {
        SetStatus(Status.Closed);
        //_open.SetActive(false);
        //_closed.SetActive(true);
    }

    public void SetEnable(bool value)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.enabled = value;
    }

    void SetStatus(Status value)
    {
        _currentStatus = value;
        //onChangeStatus?.Invoke(_currentStatus);
    }
}
