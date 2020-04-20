using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private string _endingScene;

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

    public bool IsGameRunning { get; private set; }
    public int CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance == null) {
            Instance = GetComponent<GameManager>();
        }
    }

    private IEnumerator Start()
    {
        CurrentLevel = PlayerPrefs.GetInt("Level", 1);

        _currentLife = PlayerPrefs.GetFloat("Life", 1f);
        _currentScore = PlayerPrefs.GetInt("Score", 0);

        _startupDisplay.GetComponentInChildren<TMP_Text>().text = string.Format("LEVEL {0:D0}", CurrentLevel);
        _startupDisplay.SetActive(true);

        yield return new WaitForSeconds(_startupDelayInSeconds);
        
        _lifeView.CurrentLife = _currentLife;
        _scoreView.CurrentScore = _currentScore;
        
        PlayerPrefs.DeleteAll();

        var backgroundMusic = Camera.main.GetComponent<AudioSource>();
        backgroundMusic.Play();
        
        _startupDisplay.SetActive(false);
        IsGameRunning = true;
    }

    private void Update()
    {
        if (!IsGameRunning) return;

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

            StopAllCoroutines();
            GameOver();
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
            StopAllCoroutines();
            GameOver();
        }

        _timeView.CurrentTime = _currentTime;
    }

    private void GameOver()
    {
        IsGameRunning = false;

        PlayerPrefs.SetFloat("Life", _currentLife);
        PlayerPrefs.SetFloat("Time", _currentTime);
        PlayerPrefs.SetInt("Score", _currentScore);

        SceneManager.LoadScene(_endingScene);
    }

    public void GameOverByContent()
    {
        IsGameRunning = false;
        SceneManager.LoadScene(_endingScene);
    }

    public IEnumerator LevelComplete(float secondsToLoad = 1f)
    {
        IsGameRunning = false;

        var activeScene = SceneManager.GetActiveScene();
        var loadingNextLevel = SceneManager.LoadSceneAsync(activeScene.name);
        loadingNextLevel.allowSceneActivation = false;

        PlayerPrefs.SetFloat("Life", _currentLife);
        PlayerPrefs.SetInt("Score", _currentScore);

        while (!loadingNextLevel.isDone) {
            if (loadingNextLevel.progress >= 0.9f) {
                Debug.Log("Next Level...");
                
                PlayerPrefs.SetInt("Level", CurrentLevel + 1);

                yield return new WaitForSeconds(secondsToLoad);
                loadingNextLevel.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
