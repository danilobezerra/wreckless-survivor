using UnityEngine;
using TMPro;

public class LifeView : MonoBehaviour
{
    private float _currentLife;
    private TMP_Text _lifeText;

    public float CurrentLife
    {
        get => _currentLife;
        set {
            _currentLife = value;
            UpdateUI();
        }
    }
    private void Awake()
    {
        _lifeText = GetComponent<TMP_Text>();
    }

    private void UpdateUI()
    {
        _lifeText.text = string.Format("LIFE: {0:P0}", _currentLife);
    }
}
