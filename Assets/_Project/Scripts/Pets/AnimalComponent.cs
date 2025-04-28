using UnityEngine;

public class AnimalComponent : MonoBehaviour
{
    public AnimalData animalData; // Assign after spawn!

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera mainCamera;

    private enum AnimalState { Idle, Walking, Sleeping }
    private AnimalState currentState = AnimalState.Idle;

    private float stateTimer = 0f;
    private float walkDirection = 1f;

    private float uiUpdateTimer = 0f;
    private const float uiUpdateInterval = 0.5f; // Update every 0.5 seconds

    private float sleepNeed = 100f; // Start full
    private bool isNapping = false;
    private bool recentlyForcedAwake = false;
    private float forcedAwakeTimer = 0f;
    private float sleepDrainRatePerSecond;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        if (animalData != null && sr != null)
        {
            sr.sprite = animalData.idleSprite;
        }

        if (animalData != null)
        {
            sleepDrainRatePerSecond = 100f / (animalData.hoursToFullSleep * 3600f);
        }
    }

    private void Update()
    {
        HandleAI();
        HandleSleepNeeds();
    }

    private void HandleSleepNeeds()
    {
        // Handle player-forced awake timer
        if (recentlyForcedAwake)
        {
            forcedAwakeTimer -= Time.deltaTime;
            if (forcedAwakeTimer <= 0f)
            {
                recentlyForcedAwake = false;
            }
        }

        // Calculate drain/fill multiplier from NeedsUIManager
        float effectiveMultiplier = 1f;
        if (NeedsUIManager.Instance != null)
        {
            effectiveMultiplier = NeedsUIManager.Instance.sleepDrainTimeMultiplier;
        }

        // 🛌 Handle SleepNeed draining/filling
        if (isNapping)
        {
            // 💤 FILL SleepNeed while sleeping
            sleepNeed += sleepDrainRatePerSecond * Time.deltaTime * effectiveMultiplier;
        }
        else
        {
            // 😴 DRAIN SleepNeed while awake
            sleepNeed -= sleepDrainRatePerSecond * Time.deltaTime * effectiveMultiplier;
        }

        sleepNeed = Mathf.Clamp(sleepNeed, 0f, 100f);

        // 🖥️ Update Sleep UI every 0.5 seconds
        uiUpdateTimer -= Time.deltaTime;
        if (uiUpdateTimer <= 0f)
        {
            if (NeedsUIManager.Instance != null)
            {
                NeedsUIManager.Instance.UpdateSleepNeed(sleepNeed);
            }
            uiUpdateTimer = uiUpdateInterval; // Reset UI timer
        }

        // 🌙 Decide whether to start a nap
        if (!isNapping && !recentlyForcedAwake)
        {
            if (sleepNeed <= animalData.sleepDesireThreshold)
            {
                // Calculate nap chance growing as sleepNeed drops
                float sleepinessPercent = 1f - (sleepNeed / animalData.sleepDesireThreshold); // 0% sleepy to 100% sleepy
                float napChance = sleepinessPercent * 100f; // 0 to 100 chance

                if (Random.Range(0f, 100f) <= napChance * Time.deltaTime)
                {
                    StartNap();
                }
            }
        }

        // 🌞 Stop napping if recovered enough
        if (isNapping && sleepNeed >= animalData.sleepDesireThreshold + animalData.napFillAmount)
        {
            StopNap();
        }
    }


    private void StartNap()
    {
        isNapping = true;
        currentState = AnimalState.Sleeping;
        sr.sprite = animalData.sleepSprite;
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"{animalData.animalName} is napping...");
    }

    private void StopNap()
    {
        isNapping = false;
        ChooseNewState();
        Debug.Log($"{animalData.animalName} woke up from nap.");
    }

    public void PlayerWakeAnimal()
    {
        if (isNapping)
        {
            StopNap();

            // 🎁 Tiny reward for partial nap
            float refillBonus = animalData.wakeUpRefillBonus; // new value we'll add to AnimalData
            sleepNeed = Mathf.Clamp(sleepNeed + refillBonus, 0f, 100f);

            recentlyForcedAwake = true;
            forcedAwakeTimer = animalData.forcedAwakeTimeMinutes * 60f;
            Debug.Log($"{animalData.animalName} was woken up by player! SleepNeed boosted by {refillBonus}%");
        }
    }

    private void HandleAI()
    {
        float currentMaxSpeed = animalData.maxWalkSpeed;

        // 💤 Penalty if pet is exhausted
        if (sleepNeed <= animalData.fatigueThreshold)
        {
            currentMaxSpeed *= animalData.fatigueSpeedMultiplier; // slower when tired
        }

        // Clamp velocity using currentMaxSpeed now:
        if (Mathf.Abs(rb.linearVelocity.x) > currentMaxSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * currentMaxSpeed, rb.linearVelocity.y);
        }

        // 🛌 Sleep always overrides random behavior
        if (isNapping)
        {
            currentState = AnimalState.Sleeping;
            sr.sprite = animalData.sleepSprite;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return; // Don't allow AI randomness to happen while napping
        }

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            ChooseNewState();
        }

        switch (currentState)
        {
            case AnimalState.Idle:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                break;

            case AnimalState.Walking:
                rb.AddForce(new Vector2(walkDirection * animalData.walkSpeed * 10f, 0f));

                if (mainCamera != null)
                {
                    Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

                    if (viewportPos.x <= 0.05f)
                    {
                        walkDirection = 1f;
                        sr.flipX = false;
                        rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
                    }
                    else if (viewportPos.x >= 0.95f)
                    {
                        walkDirection = -1f;
                        sr.flipX = true;
                        rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
                    }
                }

                if (animalData != null && Mathf.Abs(rb.linearVelocity.x) > animalData.maxWalkSpeed)
                {
                    rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * animalData.maxWalkSpeed, rb.linearVelocity.y);
                }
                break;
        }
    }

    private void ChooseNewState()
    {
        int random = Random.Range(0, 2); // Only idle and walking now

        switch (random)
        {
            case 0:
                currentState = AnimalState.Idle;
                sr.sprite = animalData.idleSprite;
                stateTimer = Random.Range(2f, 4f);
                break;

            case 1:
                currentState = AnimalState.Walking;
                sr.sprite = animalData.walkSprite;
                stateTimer = Random.Range(3f, 5f);
                walkDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
                sr.flipX = walkDirection == -1f;
                break;
        }
    }

    // 🐾 NEW! SetupAnimal method
    public void SetupAnimal(AnimalData data)
    {
        animalData = data;

        if (sr != null && animalData != null)
        {
            sr.sprite = animalData.idleSprite;
        }
    }
}
