using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private string _titleScene;
    [SerializeField] private ScoreView _scoreView;

    private void Start()
    {
        var finalScore = PlayerPrefs.GetInt("Score", 0);
        _scoreView.CurrentScore = finalScore;
    }

    private void Update()
    {
        if (Input.anyKey) {
            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator RestartGame()
    {
        var loadingTitle = SceneManager.LoadSceneAsync(_titleScene);

        while (!loadingTitle.isDone) {
            Debug.Log("Restart game...");
            yield return null;
        }
    }
}
