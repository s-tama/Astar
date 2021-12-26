using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walk
    }

    Vector3 _targetPos;
    Vector3 _defaultPos;
    State _currentState;
    List<Vector2Int> _routeList;
    float _moveTime;

#if MAP_3D
    [SerializeField] Animator _aniamtor = null;
#endif

    void Awake()
    {
        _targetPos = transform.position;
        _currentState = State.Idle;
        _routeList = new List<Vector2Int>();
        _defaultPos = transform.position;
        _moveTime = 0;
    }

#if MAP_3D
    void Update()
    {
        if (_currentState != State.Walk)
        {
            _aniamtor.SetBool("walk", false);
            //Raycast();
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("MapChip") == false)
                    {
                        return;
                    }
                    Raycast(hit.collider.GetComponent<MapChip>());
                }
            }
        }

        if (_currentState == State.Walk)
        {
            _aniamtor.SetBool("walk", true);

            if (_routeList.Count != 0)
            {
                _moveTime += Time.deltaTime * 2;
                Vector2Int targetId = _routeList[_routeList.Count - 1];
                Vector3 targetPos = MapManager.Instance.GetNode(0, targetId).WorldPosition;
                transform.position = Vector3.Lerp(_defaultPos, targetPos, _moveTime);
                //float dist = Vector2.Distance(transform.position, targetPos);

                transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
                
                if (_moveTime >= 1)
                {
                    _routeList.RemoveAt(_routeList.Count - 1);
                    _defaultPos = transform.position;
                    _moveTime = 0;
                }
            }
            else
            {
                _currentState = State.Idle;
            }
        }
    }
#else
    void Update()
    {
        if (_currentState == State.Walk)
        {
            if (_routeList.Count != 0)
            {
                _moveTime += Time.deltaTime;
                Vector2Int targetId = _routeList[_routeList.Count - 1];
                Vector3 targetPos = MapManager.Instance.GetNode(0, targetId).WorldPosition;
                transform.position = Vector3.Lerp(_defaultPos, targetPos, _moveTime);
                
                if (_moveTime >= 1)
                {
                    _routeList.RemoveAt(_routeList.Count - 1);
                    _defaultPos = transform.position;
                    _moveTime = 0;
                }
            }
            else
            {
                //_currentState = State.Idle;
            }
        }
    }
#endif

    void Raycast(MapChip target)
    {
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(0, -1, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("MapChip") == false)
            {
                return;
            }

            _defaultPos = transform.position;
            _moveTime = 0;

            MapChip node = hit.collider.GetComponent<MapChip>();
            if (MapManager.Instance.SearchRoute(0, node.NodeId, target.NodeId, _routeList) == true)
            {
                string result = string.Empty;
                for (int i = 0; i < _routeList.Count; i++)
                {
                    Vector2Int route = _routeList[i];
                    result += $"({route.x}, {route.y})";
                    if (i != _routeList.Count - 1)
                    {
                        result += " -> ";
                    }
                }
                Debug.Log(result);
            }

            _currentState = State.Walk;
            Debug.Log("íTçıäJén");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("MapChip") == false
        ||  _currentState == State.Walk)
        {
            return;
        }

        _defaultPos = transform.position;
        _moveTime = 0;

        MapChip node = other.GetComponent<MapChip>();
        if (MapManager.Instance.SearchRouteRandom(0, node.NodeId, _routeList) == true)
        {
            string result = string.Empty;
            for (int i = 0; i < _routeList.Count; i++)
            {
                Vector2Int route = _routeList[i];
                result += $"({route.x}, {route.y})";
                if (i != _routeList.Count - 1)
                {
                    result += " -> ";
                }
            }
            Debug.Log(result);
        }

        _currentState = State.Walk;
        Debug.Log("íTçıäJén");
    }
}
