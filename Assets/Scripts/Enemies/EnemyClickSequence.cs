using DG.Tweening;
using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class EnemyClickSequence : NetworkBehaviour
{
    public EnemyManager enemyManager;
    public ulong playerId;

    
    private SpriteRenderer spriteRenderer;
    public int[] clickSequence;
    private int currentIndex = 0;

    [SerializeField] private int enemyClickSequenceLength = 1;
    [SerializeField] private float timeoutDuration = 5.0f;
    [SerializeField] private int enemySeqRepeatAllow = 3;

    private float timer = 0f;
    private bool isTimerActive = false;

    [SerializeField] private Transform circleLarge;
    [SerializeField] private Transform circleMid;
    [SerializeField] private Transform circleSmall;

    [SerializeField] private SpriteRenderer circleLargeRenderer;
    [SerializeField] private SpriteRenderer circleMidRenderer;
    [SerializeField] private SpriteRenderer circleSmallRenderer;

    private readonly Vector3 scaleLarge = Vector3.one * 1.5f;
    private readonly Vector3 scaleMid = Vector3.one * 1.25f;
    private readonly Vector3 scaleSmall = Vector3.one * 1.1f;

    public AudioClip CoinEnemyHitsSFX;
    public AudioClip CoinEnemyDefeatSFX;
    public AudioClip wrongSequenceSFX;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        clickSequence = ClickSequenceGenerator.GenerateSequence(enemyClickSequenceLength, enemySeqRepeatAllow);
        ClickSequenceHolder.Instance.SetClickSequence(clickSequence);
        UpdateClickIndicators();
        Debug.Log("Click Sequence for this enemy: " + string.Join(" ", clickSequence));

        if (IsOwner)
        {
            playerId = NetworkManager.Singleton.LocalClientId;
        }
        else
        {
            playerId = NetworkObject.OwnerClientId;
        }

        // Start coroutine to wait for EnemyManager to spawn before registering
        StartCoroutine(WaitForEnemyManagerAndRegister());
    }


    void Update()
    {
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
        if (Input.GetMouseButtonDown(0)) clickType = 0;
        if (Input.GetMouseButtonDown(1)) clickType = 1;

        if (clickType != -1)
        {
            if (!isTimerActive) StartTimer();
            CheckClick(clickType);
        }
    }

    void CheckClick(int clickType)
    {
        if (clickSequence[currentIndex] == clickType)
        {
            currentIndex++;
            StartCoroutine(AnimateClickTransition());
            ResetTimer();

            if (currentIndex >= clickSequence.Length)
            {
                spriteRenderer.color = Color.black;
                Debug.Log("Sequence Completed! Enemy changed color.");
                ClickSequenceHolder.Instance.PopClickSequence(clickSequence);
                ResetSequence();
                AudioManager.Instance.PlaySFX(CoinEnemyHitsSFX);
                enemyManager.PlayerFinishedSequenceServerRpc(NetworkManager.Singleton.LocalClientId, gameObject);
                // enemyManager?.PlayerFinishedSequence(playerId);
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
        spriteRenderer.color = Color.white;
        currentIndex = 0;
        isTimerActive = false;
        UpdateClickIndicators();
    }

    void UpdateClickIndicators()
    {
        int stepsLeft = clickSequence.Length - currentIndex;

        if (stepsLeft > 0)
        {
            int click0 = clickSequence[currentIndex];
            circleLarge.gameObject.SetActive(true);
            circleLarge.localScale = scaleLarge;
            circleLargeRenderer.color = GetColorForClick(click0);
        }
        else
        {
            circleLarge.gameObject.SetActive(false);
        }

        if (stepsLeft > 1)
        {
            int click1 = clickSequence[currentIndex + 1];
            circleMid.gameObject.SetActive(true);
            circleMid.localScale = scaleMid;
            circleMidRenderer.color = GetColorForClick(click1);
        }
        else
        {
            circleMid.gameObject.SetActive(false);
        }

        if (stepsLeft > 2)
        {
            int click2 = clickSequence[currentIndex + 2];
            circleSmall.gameObject.SetActive(true);
            circleSmall.localScale = scaleSmall;
            circleSmallRenderer.color = GetColorForClick(click2);
        }
        else
        {
            circleSmall.gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimateClickTransition()
    {
        float duration = 0.5f;
        float half = duration / 2f;

        // Step 1: Scale up + fade out large circle
        circleLarge.DOScale(scaleLarge * 1.1f, half).SetEase(Ease.OutQuad);
        circleMid.DOScale(scaleMid * 1.1f, half).SetEase(Ease.OutQuad);
        circleSmall.DOScale(scaleSmall * 1.1f, half).SetEase(Ease.OutQuad);
        circleLargeRenderer.DOFade(0f, half).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(half);

        // Step 2: Update indicators and reset alpha to 0 temporarily
        UpdateClickIndicators();
        circleLargeRenderer.color = new Color(circleLargeRenderer.color.r, circleLargeRenderer.color.g, circleLargeRenderer.color.b, 0f);

        // Step 3: Fade in + scale to original
        circleLarge.DOScale(scaleLarge, half).SetEase(Ease.OutBack);
        circleMid.DOScale(scaleMid, half).SetEase(Ease.OutBack);
        circleSmall.DOScale(scaleSmall, half).SetEase(Ease.OutBack);
        circleLargeRenderer.DOFade(1f, half).SetEase(Ease.OutQuad);
    }

    public void Defeat()
    {
        Debug.Log("Enemy defeated by both players!");
        // Disable enemy visuals, play death animation, etc.
        gameObject.SetActive(false);
    }
    
    private IEnumerator WaitForEnemyManagerAndRegister()
    {
        // Wait until the network session is active
        yield return new WaitUntil(() => NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost);

        // Wait for the EnemyManager to be found and spawned
        while (enemyManager == null || !enemyManager.IsSpawned)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
            yield return null;
        }
        // Register this enemy
        enemyManager.RegisterEnemyServerRpc(gameObject);
    }


    private Color GetColorForClick(int clickType)
    {
        return clickType == 0 ? Color.red : Color.blue;
    }
}
