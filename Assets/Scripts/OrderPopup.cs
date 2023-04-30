using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public sealed class OrderPopup : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    [CanBeNull] private DeliveryScheduler.Order order;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (order is null)
        {
            return;
        }

        var turnsLeft = order.ExpiryTurn - TurnState.Instance.TurnNumber;
        textMesh.text = $"{turnsLeft}";
        textMesh.color = turnsLeft > 2 ? Color.white : Color.red;
    }

    public void SetOrder(DeliveryScheduler.Order order)
    {
        this.order = order;
    }
}
