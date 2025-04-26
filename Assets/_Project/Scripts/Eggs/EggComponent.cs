using UnityEngine;
using CritterPetz;

[RequireComponent(typeof(Rigidbody2D))]
public class EggComponent : MonoBehaviour
{
    [Header("Egg Data")]
    public EggData eggData;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isDragging = false;
    private Vector3 dragOffset;
    private float dragStrength = 15f;
    private Color originalColor;
    private bool isSquashing = false;
    private bool isWiggling = false;
    private Vector3 originalVisualScale;
    private Transform spriteTransform;

    public void Initialize(EggData data)
    {
        eggData = data;

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && data.eggSprite != null)
        {
            sr.sprite = data.eggSprite;
            originalColor = sr.color;
            spriteTransform = sr.transform;
            originalVisualScale = spriteTransform.localScale;
        }

        if (rb != null && data != null)
        {
            rb.gravityScale = data.gravityScale;
            rb.linearDamping = data.drag;
            rb.angularDamping = data.angularDrag;

            var collider = GetComponent<Collider2D>();
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            originalColor = sr.color;
            spriteTransform = sr.transform;
            originalVisualScale = spriteTransform.localScale;
        }
        else
        {
            originalColor = Color.white;
            spriteTransform = transform;
            originalVisualScale = transform.localScale;
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 target = new Vector2(mousePosition.x + dragOffset.x, mousePosition.y + dragOffset.y);
            Vector2 force = (target - rb.position) * dragStrength;
            rb.linearVelocity = force;
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

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragOffset = transform.position - new Vector3(mousePosition.x, mousePosition.y, 0);
        isDragging = true;

        LeanTween.rotateZ(spriteTransform.gameObject, 0f, 0.3f).setEase(LeanTweenType.easeOutSine);
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

        LeanTween.scale(spriteTransform.gameObject, new Vector3(squashX, squashY, originalVisualScale.z), squashDuration).setEase(LeanTweenType.easeOutQuad);

        yield return new WaitForSeconds(squashDuration);

        LeanTween.scale(spriteTransform.gameObject, originalVisualScale, squashDuration).setEase(LeanTweenType.easeOutElastic);

        yield return new WaitForSeconds(squashDuration);

        isSquashing = false;
    }

    // HATCH WIGGLE
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
            float baseRotation = spriteTransform.rotation.eulerAngles.z;

            if (eggData.wiggleRelativeToCurrentRotation)
            {
                LeanTween.rotateZ(spriteTransform.gameObject, baseRotation - eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(spriteTransform.gameObject, baseRotation + eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(spriteTransform.gameObject, baseRotation, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);
            }
            else
            {
                LeanTween.rotateZ(spriteTransform.gameObject, -eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(spriteTransform.gameObject, eggData.wiggleRotationAmount, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
                yield return new WaitForSeconds(eggData.wiggleDuration);

                LeanTween.rotateZ(spriteTransform.gameObject, 0f, eggData.wiggleDuration).setEase(LeanTweenType.easeInOutSine);
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
