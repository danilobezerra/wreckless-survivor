using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private string _endingScene;
    [SerializeField] private string _nextLevelScene;

    [SerializeField] private GameObject _startupDisplay;
    [SerializeField] private GameObject _headUpDisplay;
    [SerializeField] private GameObject _pauseDisplay;

    [SerializeField] private ScoreView _scoreView;
    [SerializeField] private LifeView _lifeView;
    [SerializeField] private TimeView _timeView;

    [SerializeField] private float _startupDelayInSeconds = 1.5f;

    private int _currentScore = 0;
    private float _currentLife = 1f;
    private float _currentTime = 60;

    private void Awake()
    {
        if (Instance == null) {
            Instance = GetComponent<GameManager>();
        }
    }

    private IEnumerator Start()
    {
        _currentLife = PlayerPrefs.GetFloat("Life", 1f);
        _lifeView.CurrentLife = _currentLife;

        _currentScore = PlayerPrefs.GetInt("Score", 0);
        _scoreView.CurrentScore = _currentScore;

        PlayerPrefs.DeleteAll();

        yield return new WaitForSeconds(_startupDelayInSeconds);
        _startupDisplay.SetActive(false);
    }

    private void Update()
    {
        if (_startupDisplay.activeSelf) return;

        if (Input.GetButtonDown("Cancel")) {
            if (Time.timeScale > 0f) {
                _headUpDisplay.SetActive(false);
                _pauseDisplay.SetActive(true);

                Time.timeScale = 0;
            } else {
                Time.timeScale = 1f;

                _pauseDisplay.SetActive(false);
                _headUpDisplay.SetActive(true);
            }
        } else {
            CountdownTime(Time.deltaTime);
        }
    }

    public void ReplenishLife(float amount)
    {
        if (_currentLife + amount > 1) {
            _currentLife = 1;
        } else {
            _currentLife += amount;
        }

        _lifeView.CurrentLife = _currentLife;
    }

    public void DepleteLife(float amount)
    {
        if (_currentLife - amount < 0.01f) {
            _currentLife = 0;
            StartCoroutine(GameOver());
        } else {
            _currentLife -= amount;
        }

        _lifeView.CurrentLife = _currentLife;
    }

    public void ImproveScore(int amount)
    {
        if (_currentScore + amount > 9999) {
            _currentScore = 9999;
        } else {
            _currentScore += amount;
        }

        _scoreView.CurrentScore = _currentScore;
    }

    public void DropScore(int amount)
    {
        if (_currentScore - amount < 0) {
            _currentScore = 0;
        } else {
            _currentScore -= amount;
        }

        _scoreView.CurrentScore = _currentScore;
    }

    private void CountdownTime(float seconds)
    {
        _currentTime -= seconds;

        if (_currentTime < 1) {
            StartCoroutine(GameOver());
        }

        _timeView.CurrentTime = _currentTime;
    }

    private IEnumerator GameOver()
    {
        var loadingEnding = SceneManager.LoadSceneAsync(_endingScene);
        PlayerPrefs.SetFloat("Life", _currentLife);
        PlayerPrefs.SetInt("Score", _currentScore);

        while (!loadingEnding.isDone) {
            Debug.Log("Game Over...");
            yield return null;
        }
    }

    public IEnumerator LevelComplete()
    {
        var loadingNextLevel = SceneManager.LoadSceneAsync(_nextLevelScene);
        PlayerPrefs.SetFloat("Life", _currentLife);
        PlayerPrefs.SetInt("Score", _currentScore);

        while (!loadingNextLevel.isDone) {
            Debug.Log("Next Level...");
            yield return null;
        }
    }
}
