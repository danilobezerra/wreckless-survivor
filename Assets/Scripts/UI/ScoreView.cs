using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    private int _currentScore;
    private TMP_Text _scoreText;

    public int CurrentScore
    {
        get => _currentScore;
        set {
            _currentScore = value;
            UpdateUI();
        }
    }

    private void Awake()
    {
        _scoreText = GetComponent<TMP_Text>();
    }

    private void UpdateUI()
    {
        _scoreText.text = string.Format("SCORE: {0:D4}", _currentScore);
    }
}
