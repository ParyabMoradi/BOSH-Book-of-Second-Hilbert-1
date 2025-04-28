using UnityEngine.Events;

public static class UIHandler
{
    // To stop the timer
    public static event UnityAction StopTimer;

    // To start the timer
    public static event UnityAction StartTimer;

    // To update the timer
    public static event UnityAction<float> UpdateTimer;

    public static void OnTimerStart() => StartTimer?.Invoke();
    public static void OnTimerStop() => StopTimer?.Invoke();

    public static void OnTimerUpdate(float time) => UpdateTimer?.Invoke(time);

}
