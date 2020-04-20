using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField]
    private Animator _titleImage;

    [SerializeField]
    private Animator _startText;

    [SerializeField]
    private Animator _gameImage;

    [SerializeField]
    private string _sceneToLoad;

    private void Start()
    {
        PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        if (Input.anyKey) {
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        var loadingGame = SceneManager.LoadSceneAsync(_sceneToLoad);

        while (!loadingGame.isDone) {
            Debug.Log("Loading...");
            yield return null;
        }
    }
}
