using UnityEngine;
using System;
using TMPro;

public class TimeView : MonoBehaviour
{
    private float _currentTime;
    private TMP_Text _timeText;

    public float CurrentTime
    {
        get => _currentTime;
        set {
            _currentTime = value;
            UpdateUI();
        }
    }

    private void Awake()
    {
        _timeText = GetComponent<TMP_Text>();
    }

    private void UpdateUI()
    {
        var currentTime = TimeSpan.FromSeconds(_currentTime);
        _timeText.text = string.Format("TIME: {0}", currentTime.ToString(@"mm\:ss"));
    }
}
