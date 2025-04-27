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

    public float maxWalkSpeed = 2.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        if (animalData != null && sr != null)
        {
            sr.sprite = animalData.idleSprite;
        }
    }

    private void Update()
    {
        HandleAI();
    }

    private void HandleAI()
    {
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

                if (Mathf.Abs(rb.linearVelocity.x) > maxWalkSpeed)
                {
                    rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxWalkSpeed, rb.linearVelocity.y);
                }
                break;

            case AnimalState.Sleeping:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                break;
        }
    }

    private void ChooseNewState()
    {
        int random = Random.Range(0, 3);

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

            case 2:
                currentState = AnimalState.Sleeping;
                sr.sprite = animalData.sleepSprite;
                stateTimer = animalData.sleepDuration;
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
