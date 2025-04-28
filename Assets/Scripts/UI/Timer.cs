using UnityEngine;
using System;
using TMPro;

public class Timer : MonoBehaviour
{
    private TMP_Text timerText_;
    enum TimerType { Countdown, Stopwatch };
    [SerializeField] private TimerType timerType;
    [SerializeField] private float timeLimit = 60f; // Time limit in seconds for countdown

    private bool isRunning_ = false;
    private float currentTime_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerText_ = GetComponent<TMP_Text>();
        currentTime_ = timerType == TimerType.Countdown ? timeLimit : 0f;
        UpdateTimerDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning_)
        {
            if (timerType == TimerType.Countdown)
            {
                currentTime_ -= Time.deltaTime;
                if (currentTime_ <= 0f)
                {
                    currentTime_ = 0f;
                    isRunning_ = false;
                }
            }
            else if (timerType == TimerType.Stopwatch)
            {
                currentTime_ += Time.deltaTime;
            }

            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        isRunning_ = true;
    }

    public void StopTimer()
    {
        isRunning_ = false;
    }

    public void ResetTimer()
    {
        isRunning_ = false;
        currentTime_ = timerType == TimerType.Countdown ? timeLimit : 0f;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime_);
        timerText_.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }
}
