using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TextScoreUI;

    private int _scr = 0;

    public int Score
    {
        get { return _scr; }
        set
        {
            _scr = value;
            TextScoreUI.text = Score.ToString();
            PlayerPrefs.SetInt("Score", Score);
        }
    }
}