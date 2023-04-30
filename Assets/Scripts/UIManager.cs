using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private GameObject gameOver;
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

        if (TurnState.Instance.GameOver && !gameOver.activeSelf)
        {
            gameOver.SetActive(true);
        }

        score.text = $"Turn: {TurnState.Instance.TurnNumber}\nScore: {DeliveryScheduler.Instance.Points}";
    }

    [UsedImplicitly]
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
