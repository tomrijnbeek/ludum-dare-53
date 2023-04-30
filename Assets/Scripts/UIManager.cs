using System.Collections;
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
    private bool restartQueued;

    // Update is called once per frame
    void Update()
    {
        if (restartQueued)
        {
            StartCoroutine(reloadScene());
            restartQueued = false;
            return;
        }

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

    private IEnumerator reloadScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    [UsedImplicitly]
    public void Restart()
    {
        restartQueued = true;
    }
}
