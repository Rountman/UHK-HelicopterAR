using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float timeRemaining;
    public bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI gameOverText;
    MovingScript movingScript;
    ScoreManager scoreManager;
    public Button retryButton;
    public Button startButton;

    void Start()
    {
        // Starts the timer automatically
        timerIsRunning = false;
        movingScript = FindObjectOfType<MovingScript>();
        scoreManager = FindObjectOfType<ScoreManager>();
        timeRemaining = 20;
        retryButton.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                GameObject[] colObjects = GameObject.FindGameObjectsWithTag("CollectiblePrefab");
                foreach (GameObject obj in colObjects)
                {
                    Destroy(obj);
                }

                timeRemaining = 0;
                timerIsRunning = false;
                movingScript.pauseGame = true;
                movingScript.DestroyCollectibleObjects();
                retryButton.gameObject.SetActive(true);
                gameOverText.gameObject.SetActive(true);
                timeText.gameObject.SetActive(false);
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartGame()
    {
        timeRemaining = 20;
        timerIsRunning = true;
        movingScript.pauseGame = false;
        scoreManager.Score = 0;
        retryButton.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(true);
        movingScript.counter = 0;
    }

    public void DeleteStartButton()
    {
        startButton.gameObject.SetActive(false);
    }
}