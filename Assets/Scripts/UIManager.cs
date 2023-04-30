using TMPro;
using UnityEngine;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI[] tutorials;
    private bool firstClickDetected;

    // Update is called once per frame
    void Update()
    {
        if (!firstClickDetected && TurnState.Instance.FirstMovementDone)
        {
            foreach (var gui in tutorials)
            {
                Destroy(gui);
            }
        }

        score.text = $"Turn: {TurnState.Instance.TurnNumber}\nScore: {DeliveryScheduler.Instance.Points}";
    }
}
