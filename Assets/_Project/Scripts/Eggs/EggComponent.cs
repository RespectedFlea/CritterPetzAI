using System.Collections;
using UnityEngine;
using CritterPetz;

[RequireComponent(typeof(Rigidbody2D))]
public class EggComponent : MonoBehaviour
{
    [Header("Egg Data")]
    public EggData eggData;

    [Header("Hatching")]
    public Animator animator; // assign this in prefab
    public Transform slotTransform; // assign the slot when spawning the egg

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform visualTransform;
    private Vector3 originalVisualScale;
    private Color originalColor;

    private bool isDragging = false;
    private Vector3 dragOffset;
    private float dragStrength = 15f;

    private bool isSquashing = false;
    private bool isWiggling = false;
    private bool isHatching = false;

    private Camera mainCamera;
    private float wrapBuffer = 0.1f;

    private Collider2D eggCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        eggCollider = GetComponent<Collider2D>();

        if (sr != null)
        {
            visualTransform = sr.transform;
            originalColor = sr.color;
            originalVisualScale = visualTransform.localScale;
        }
        else
        {
            visualTransform = transform;
            originalColor = Color.white;
            originalVisualScale = transform.localScale;
        }

        mainCamera = Camera.main;
    }

    public void Initialize(EggData data)
    {
        eggData = data;

        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null && data.eggSprite != null)
        {
            sr.sprite = data.eggSprite;
        }

        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
        {
            if (data.hatchAnimatorController != null)
                animator.runtimeAnimatorController = data.hatchAnimatorController; // <-- Set the animation!

            animator.Rebind();
            animator.Update(0f);
        }

        // Set up physics like gravity/bounciness etc here as before
    }


    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 target = new Vector2(mousePosition.x + dragOffset.x, mousePosition.y + dragOffset.y);
            Vector2 force = (target - rb.position) * dragStrength;
            rb.linearVelocity = force;
        }
    }

    private void LateUpdate()
    {
        HandleScreenWrap();
    }

    private void HandleScreenWrap()
    {
        if (mainCamera == null) return;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 wrappedViewportPos = viewportPos;
        bool wrapped = false;

        if (viewportPos.x > 1f + wrapBuffer)
        {
            wrappedViewportPos.x = -wrapBuffer;
            wrapped = true;
        }
        else if (viewportPos.x < -wrapBuffer)
        {
            wrappedViewportPos.x = 1f + wrapBuffer;
            wrapped = true;
        }

        if (viewportPos.y > 1f + wrapBuffer)
        {
            wrappedViewportPos.y = -wrapBuffer;
            wrapped = true;
        }
        else if (viewportPos.y < -wrapBuffer)
        {
            wrappedViewportPos.y = 1f + wrapBuffer;
            wrapped = true;
        }

        if (wrapped)
        {
            Vector3 newWorldPos = mainCamera.ViewportToWorldPoint(wrappedViewportPos);
            transform.position = new Vector3(newWorldPos.x, newWorldPos.y, transform.position.z);

            if (isDragging)
            {
                Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                dragOffset = transform.position - new Vector3(mousePosition.x, mousePosition.y, 0);
            }
        }
    }

    private void OnMouseDown()
    {
        if (isHatching) return; // No interactions while hatching

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        if (sr != null)
        {
            sr.color = Color.white;
        }

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        dragOffset = transform.position - new Vector3(mousePosition.x, mousePosition.y, 0);
        isDragging = true;

        LeanTween.rotateZ(visualTransform.gameObject, 0f, 0.3f).setEase(LeanTweenType.easeOutSine);
    }

    private void OnMouseUp()
    {
        if (rb != null)
        {
            rb.gravityScale = eggData.gravityScale;
        }

        isDragging = false;

        if (sr != null)
        {
            sr.color = originalColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDragging && !isSquashing)
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed >= eggData.minImpactSpeedForBoing)
            {
                StartCoroutine(DoSquash(impactSpeed));
            }
        }
    }

    private IEnumerator DoSquash(float impactSpeed)
    {
        isSquashing = true;

        float impactPercent = Mathf.Clamp01(impactSpeed / eggData.maxImpactSpeed);
        float squashBoost = 1f + impactPercent * (eggData.maxSquashMultiplier - 1f);

        float squashX = originalVisualScale.x * eggData.squashFactor * squashBoost;
        float squashY = originalVisualScale.y * eggData.stretchFactor * squashBoost;
        float squashDuration = eggData.boingSpeed;

        LeanTween.scale(visualTransform.gameObject, new Vector3(squashX, squashY, originalVisualScale.z), squashDuration).setEase(LeanTweenType.easeOutQuad);

        yield return new WaitForSeconds(squashDuration);

        LeanTween.scale(visualTransform.gameObject, originalVisualScale, squashDuration).setEase(LeanTweenType.easeOutElastic);

        yield return new WaitForSeconds(squashDuration);

        isSquashing = false;
    }

    // ==========================
    // Hatch Sequence
    // ==========================

    private IEnumerator WiggleRoutine()
    {
        while (isWiggling)
        {
            float baseRotation = visualTransform.rotation.eulerAngles.z;

            if (eggData.wiggleRelativeToCurrentRotation)
            {
                LeanTween.rotateZ(visualTransform.gameObject, baseRotation - eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(visualTransform.gameObject, baseRotation + eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(visualTransform.gameObject, baseRotation, eggData.wiggleDuration).setEase(LeanTweenType.easeOutElastic);
                yield return new WaitForSeconds(eggData.wiggleDuration);
            }
            else
            {
                LeanTween.rotateZ(visualTransform.gameObject, -eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(visualTransform.gameObject, eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(visualTransform.gameObject, 0f, eggData.wiggleDuration).setEase(LeanTweenType.easeOutElastic);
                yield return new WaitForSeconds(eggData.wiggleDuration);
            }

            yield return new WaitForSeconds(eggData.wiggleInterval);
        }
    }


    public void StartHatchWiggle()
    {
        if (!isWiggling)
        {
            isWiggling = true;
            StartCoroutine(WiggleRoutine());
        }
    }

    public void StopHatchWiggle()
    {
        isWiggling = false;
    }

    public void PlayHatchAnimation()
    {
        if (isHatching) return;
        isHatching = true;

        StopHatchWiggle();

        // Disable player input
        isDragging = false;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // Updated to use bodyType instead of isKinematic
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        if (eggCollider != null)
        {
            eggCollider.enabled = false;
        }

        // Stand egg upright
        if (visualTransform != null)
        {
            LeanTween.rotateZ(visualTransform.gameObject, 0f, 0.5f).setEase(LeanTweenType.easeOutSine);
        }

        // Play hatch animation
        if (animator != null)
        {
            animator.ResetTrigger("Hatch");
            animator.SetTrigger("Hatch");
        }

        StartCoroutine(HatchSequence());
    }

    private IEnumerator HatchSequence()
    {
        yield return new WaitForSeconds(0.5f); // Wait for stand-up
        yield return new WaitForSeconds(1.0f); // Wait for hatch animation to finish (adjust time)

        Vector3 spawnPosition = transform.position + new Vector3(0f, 0.5f, 0f); // Slightly above egg
        RoomManager.Instance.SpawnAnimalAtSlot(spawnPosition, eggData);

        // Clear the slot
        if (RoomManager.Instance != null && slotTransform != null)
        {
            int slotIndex = RoomManager.Instance.GetSlotIndexFromTransform(slotTransform);
            if (slotIndex != -1)
            {
                RoomManager.Instance.ClearSlot(slotIndex, false);
            }
        }

        // Now destroy the egg object
        Destroy(gameObject);

    }
}
