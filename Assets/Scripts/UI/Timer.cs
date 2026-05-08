using UnityEngine;
using TMPro;
using System;
using UnityEditor.Search;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;

    private float _runTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _runTime += Time.deltaTime; // Increment the time by seconds passed since last frame

        int minutes = Mathf.FloorToInt(_runTime / 60);
        int seconds = Mathf.FloorToInt(_runTime % 60);
        int miliseconds = Mathf.FloorToInt((_runTime) * 100) -  seconds * 100;

        timerText.text = String.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, miliseconds); ;
    }
}
