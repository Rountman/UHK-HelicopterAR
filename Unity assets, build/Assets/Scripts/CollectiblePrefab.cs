using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectiblePrefab : MonoBehaviour
{
    public GameObject cube;
    ScoreManager scoreManager;
    Timer timer;
    public AudioClip collectSound;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        timer = FindObjectOfType<Timer>();
    }

    void OnTriggerEnter(Collider other)
    {
        AudioSource.PlayClipAtPoint(collectSound, transform.position);
        scoreManager.Score++;
        timer.timeRemaining += 2;
        Destroy(cube);
    }

    // Update is called once per frame
    void Update()
    {

    }
}