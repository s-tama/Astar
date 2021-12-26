using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    [SerializeField] MapChip _owner;
    [SerializeField] Text _cost;
    [SerializeField] Text _hcost;
    [SerializeField] Text _score;

    bool _isEnabled;

    void Awake()
    {
        gameObject.SetActive(false);
        _isEnabled = false;

        _owner.onChangeParam = (cost, hcost, score) =>
        {
            gameObject.SetActive(_isEnabled);
            SetData((int)cost, (int)hcost);
        };

        _owner.onChangeStatus = (status) =>
        {
            _isEnabled = status != MapChip.Status.None;
        };
    }

    public void SetData(int cost, int hcost)
    {
        _cost.text = $"c={cost}";
        _hcost.text = $"h={hcost}";
        _score.text = $"s={cost + hcost}";
    }
}
