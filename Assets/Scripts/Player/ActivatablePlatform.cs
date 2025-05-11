using UnityEngine;

public class ActivatablePlatform : MonoBehaviour
{
    [Header("Click Sequence Settings")]
    [SerializeField] private int sequenceLength = 3;
    [SerializeField] private int maxRepeats = 2;
    [SerializeField] private float timeoutDuration = 2f;
    [SerializeField] private float activeDuration = 5f;

    [Header("Colliders")]
    [SerializeField] private BoxCollider2D mouseCollider;
    [SerializeField] private BoxCollider2D platformCollider;
    private int[] clickSequence;
    private int currentIndex = 0;
    private float timer;
    private bool isTimerActive = false;
    private bool isPlatformActive = false;

    private LayerMask groundLayer;
    private LayerMask mouseColliderLayer; 

    void Start()
    {
        // Ensure correct setup
        if (!mouseCollider || !platformCollider)
        {
            Debug.LogError("Assign both mouseCollider and platformCollider in the inspector.");
            return;
        }

        groundLayer = LayerMask.NameToLayer("Ground");
        mouseColliderLayer = LayerMask.NameToLayer("MouseCollider");

        mouseCollider.enabled = true;
        mouseCollider.isTrigger = true;

        platformCollider.enabled = false;

        GenerateNewSequence();
    }

    void Update()
    {
        if (isTimerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Debug.Log("Timeout. Resetting sequence.");
                ResetSequence();
            }
        }

        if (isPlatformActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                DeactivatePlatform();
                GenerateNewSequence();
            }
        }
    }

    void OnMouseOver()
    {
        int clickType = -1;
        if (Input.GetMouseButtonDown(0)) clickType = 0;
        if (Input.GetMouseButtonDown(1)) clickType = 1;
        if (clickType != -1)
        {
            if (!isTimerActive) StartTimer();
            HandleClick(clickType);
        }
    }

    void HandleClick(int clickType)
    {
        if (clickSequence[currentIndex] == clickType)
        {
            currentIndex++;
            ResetTimer();

            if (currentIndex >= clickSequence.Length)
            {
                Debug.Log("Sequence completed. Activating platform.");
                ActivatePlatform();
            }
        }
        else
        {
            ResetSequence();
            GenerateNewSequence();
        }
    }

    void StartTimer()
    {
        timer = timeoutDuration;
        isTimerActive = true;
    }

    void ResetTimer()
    {
        timer = timeoutDuration;
    }

    void ResetSequence()
    {
        currentIndex = 0;
        isTimerActive = false;
    }

    void ActivatePlatform()
    {
        platformCollider.enabled = true;
        isPlatformActive = true;
        timer = activeDuration;

        gameObject.layer = groundLayer;

        mouseCollider.enabled = false;
    }

    void DeactivatePlatform()
    {
        platformCollider.enabled = false;
        isPlatformActive = false;
        ResetSequence();

        gameObject.layer = mouseColliderLayer;

        mouseCollider.enabled = true;
    }

    void GenerateNewSequence()
    {
        clickSequence = ClickSequenceGenerator.GenerateSequence(sequenceLength, maxRepeats);
        Debug.Log($"{gameObject.name} sequence: {string.Join(" ", clickSequence)}");
    }
}
