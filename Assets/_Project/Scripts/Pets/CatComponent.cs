using UnityEngine;

public class CatComponent : MonoBehaviour
{
    public CatData catData;

    [Header("State Sprites")]
    public Sprite idleSprite;
    public Sprite walkSprite;
    public Sprite sleepSprite;

    [Header("Movement Settings")]
    public float maxWalkSpeed = 2.5f; // Max horizontal movement speed

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera mainCamera;

    private enum CatState { Idle, Walking, Sleeping }
    private CatState currentState = CatState.Idle;

    private float stateTimer = 0f;
    private float walkDirection = 1f; // -1 = left, 1 = right

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        mainCamera = Camera.main;
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
            case CatState.Idle:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                break;

            case CatState.Walking:
                rb.AddForce(new Vector2(walkDirection * catData.walkSpeed * 10f, 0f));

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

                // Clamp max horizontal speed
                if (Mathf.Abs(rb.linearVelocity.x) > maxWalkSpeed)
                {
                    rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxWalkSpeed, rb.linearVelocity.y);
                }
                break;

            case CatState.Sleeping:
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
                currentState = CatState.Idle;
                sr.sprite = idleSprite;
                stateTimer = Random.Range(2f, 4f);
                break;

            case 1:
                currentState = CatState.Walking;
                sr.sprite = walkSprite;
                stateTimer = Random.Range(3f, 5f);
                walkDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
                sr.flipX = walkDirection == -1f; // Flip sprite based on direction
                break;

            case 2:
                currentState = CatState.Sleeping;
                sr.sprite = sleepSprite;
                stateTimer = catData.sleepDuration;
                break;
        }
    }
}
