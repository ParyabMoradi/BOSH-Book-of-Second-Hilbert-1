using UnityEngine;
using UnityEngine.UI;

public class EnemyClickSequence : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Image timerCircleUI;
    public int[] clickSequence;
    private int currentIndex = 0;
    [SerializeField] private int enemyClickSequenceLength = 1;
    [SerializeField] private float timeoutDuration = 5.0f;
    [SerializeField] private int enemySeqRepeatAllow = 3;

    private float timer = 0f;
    private bool isTimerActive = false;
    
    private AudioManager _audioManager;
    
    private Color[] colorOptions = { Color.red, Color.green, Color.blue };
    private Color assignedColor;
    
    public AudioClip CoinEnemyHitsSFX;
    public AudioClip CoinEnemyDefeatSFX;
    public AudioClip wrongSequenceSFX;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        clickSequence = ClickSequenceGenerator.GenerateSequence(enemyClickSequenceLength,enemySeqRepeatAllow);

        ClickSequenceHolder.Instance.SetClickSequence(clickSequence);
        
        assignedColor = colorOptions[Random.Range(0, colorOptions.Length)];
        spriteRenderer.color = assignedColor;

        Debug.Log("Click Sequence for this enemy: " + string.Join(" ", clickSequence));
    }

    void Update()
    {
        if (isTimerActive)
        {
            timer -= Time.deltaTime;

            // Update the UI circle
            if (timerCircleUI != null)
                timerCircleUI.fillAmount = timer / timeoutDuration;

            if (timer <= 0)
            {
                ResetSequence();
                Debug.Log("Time ran out! Restarting sequence.");
                AudioManager.Instance.PlaySFX(wrongSequenceSFX);
            }
        }
    }

    void OnMouseOver()
    {
        int clickType = -1; 
        if (Input.GetMouseButtonDown(0))
            clickType = 0;
        if (Input.GetMouseButtonDown(1))
            clickType = 1;

        if (clickType != -1)
        {
            if (!isTimerActive)
            {
                StartTimer();
            }
            CheckClick(clickType);
        }
    }

    void CheckClick(int clickType)
    {
        if (clickSequence[currentIndex] == clickType)
        {
            currentIndex++;
            ResetTimer();
            if (currentIndex >= clickSequence.Length)
            {
                spriteRenderer.color = Color.black;
                Debug.Log("Sequence Completed! Enemy changed color.");
                ClickSequenceHolder.Instance.PopClickSequence(clickSequence);
                ResetSequence();
                AudioManager.Instance.PlaySFX(CoinEnemyHitsSFX);
            }
            else
            {
                AudioManager.Instance.PlaySFX(CoinEnemyDefeatSFX);

            }
        }
        else
        {
            Debug.Log("Wrong Click! Restarting sequence.");
            ResetSequence();
            AudioManager.Instance.PlaySFX(wrongSequenceSFX);
        }
    }

    void ResetTimer()
    {
        timer = timeoutDuration;
    }

    void StartTimer()
    {
        timer = timeoutDuration;
        isTimerActive = true;

        if (timerCircleUI != null)
            timerCircleUI.fillAmount = 1f;
    }

    void ResetSequence()
    {
        currentIndex = 0;
        isTimerActive = false;

        if (timerCircleUI != null)
            timerCircleUI.fillAmount = 1f;
    }
}
