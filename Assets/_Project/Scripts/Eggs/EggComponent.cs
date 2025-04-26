using UnityEngine;
using CritterPetz;

[RequireComponent(typeof(Rigidbody2D))]
public class EggComponent : MonoBehaviour
{
    [Header("Egg Data")]
    public EggData eggData;

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

    private Camera mainCamera;
    private float wrapBuffer = 0.1f;

    private Collider2D eggCollider;

    public void Initialize(EggData data)
    {
        eggData = data;

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && data.eggSprite != null)
        {
            sr.sprite = data.eggSprite;
        }

        if (sr != null)
        {
            visualTransform = sr.transform;
            originalColor = sr.color;
            originalVisualScale = visualTransform.localScale;
        }

        if (rb != null && data != null)
        {
            rb.gravityScale = data.gravityScale;
            rb.linearDamping = data.drag;
            rb.angularDamping = data.angularDrag;

            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                PhysicsMaterial2D mat = new PhysicsMaterial2D("EggMaterial_" + data.eggName)
                {
                    bounciness = data.bounciness,
                    friction = 0.4f
                };
                collider.sharedMaterial = mat;
            }
        }
    }

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

    private System.Collections.IEnumerator DoSquash(float impactSpeed)
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

    public void StartHatchWiggle()
    {
        if (!isWiggling)
        {
            isWiggling = true;
            StartCoroutine(WiggleRoutine());
        }
    }

    private System.Collections.IEnumerator WiggleRoutine()
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

    public void StopHatchWiggle()
    {
        isWiggling = false;
    }
}
