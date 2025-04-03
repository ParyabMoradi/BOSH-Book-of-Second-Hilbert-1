using UnityEngine;

public class EnemyClickSequence : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private int[] clickSequence;
    private int currentIndex = 0;
    [SerializeField] private int enemyClickSequenceLength = 1;
    [SerializeField] private float timeoutDuration = 2.0f;

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
        clickSequence = ClickSequenceGenerator.GenerateSequence(enemyClickSequenceLength);
        assignedColor = colorOptions[Random.Range(0, colorOptions.Length)];
        spriteRenderer.color = assignedColor;

        Debug.Log("Click Sequence for this enemy: " + string.Join(" ", clickSequence));
    }

    void Update()
    {
        // If the timer is active, update it
        if (isTimerActive)
        {
            timer -= Time.deltaTime;
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
    }

    void ResetSequence()
    {
        currentIndex = 0;
        isTimerActive = false;
    }
}
