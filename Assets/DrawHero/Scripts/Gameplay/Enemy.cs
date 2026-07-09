using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public float maxHP = 30f;
    public float damageThreshold = 2f;
    public float damageMultiplier = 4f;

    public event Action<Enemy> OnDeath;

    private float currentHP;
    private bool isDead;

    private SpriteRenderer sr;
    private Color baseColor;
    private Rigidbody2D body;

    private Image hpFill;
    private Canvas hpCanvas;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr != null ? sr.color : Color.red;
        body = GetComponent<Rigidbody2D>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        currentHP = maxHP;
        BuildHealthBar();
    }

    private void Start()
    {
        LevelController controller = FindObjectOfType<LevelController>();
        if (controller != null)
            controller.RegisterEnemy(this);
        else
            Debug.LogWarning("[DrawHero] Enemy could not find LevelController to register!");
    }

    private void BuildHealthBar()
    {
        GameObject canvasGo = new GameObject("HPCanvas");
        canvasGo.transform.SetParent(transform);
        canvasGo.transform.localPosition = new Vector3(0, 0.9f, 0);
        hpCanvas = canvasGo.AddComponent<Canvas>();
        hpCanvas.renderMode = RenderMode.WorldSpace;
        canvasGo.transform.localScale = Vector3.one * 0.01f;

        RectTransform canvasRt = canvasGo.GetComponent<RectTransform>();
        canvasRt.sizeDelta = new Vector2(120, 20);

        GameObject bg = new GameObject("BG", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(canvasGo.transform, false);
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(canvasGo.transform, false);
        hpFill = fill.GetComponent<Image>();
        hpFill.color = new Color(0.4f, 0.85f, 0.35f);
        hpFill.type = Image.Type.Filled;
        hpFill.fillMethod = Image.FillMethod.Horizontal;
        hpFill.fillOrigin = 0;
        hpFill.fillAmount = 1f;
        RectTransform fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = new Vector2(4, 4);
        fillRt.offsetMax = new Vector2(-4, -4);
    }

    private void LateUpdate()
    {
        if (hpCanvas != null)
            hpCanvas.transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        DrawnObject drawn = collision.gameObject.GetComponentInParent<DrawnObject>();
        Rigidbody2D otherBody = collision.rigidbody;

        if (otherBody == null) return;

        float impactForce = collision.relativeVelocity.magnitude * otherBody.mass;

        if (impactForce < damageThreshold) return;

        float damage = (impactForce - damageThreshold) * damageMultiplier;
        TakeDamage(damage);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHP -= amount;


        if (hpFill != null)
        {
            float ratio = Mathf.Clamp01(currentHP / maxHP);
            DOTween.To(() => hpFill.fillAmount, x => hpFill.fillAmount = x, ratio, 0.2f);
            hpFill.color = ratio > 0.4f ? new Color(0.4f, 0.85f, 0.35f) : new Color(0.9f, 0.5f, 0.2f);
        }

        if (sr != null)
        {
            sr.DOKill();
            sr.color = Color.white;
            sr.DOColor(baseColor, 0.2f);
        }

        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * 0.25f, 0.2f, 8, 0.6f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop", 0.8f);

        if (HapticManager.Instance != null)
            HapticManager.Instance.Medium();

        if (currentHP <= 0)
            Die();
    }

    public void KillInstant()
    {
        if (isDead) return;
        Die();
    }

    private void Die()
    {
        isDead = true;

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(0.5f, 0.3f);

        SimpleParticleBurst.Spawn(transform.position, baseColor, 10, 2f);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySfx("pop", 1.2f);

        if (HapticManager.Instance != null)
            HapticManager.Instance.Heavy();

        OnDeath?.Invoke(this);

        if (hpCanvas != null)
            hpCanvas.gameObject.SetActive(false);

        transform.DOScale(0f, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }

    public bool IsDead => isDead;

    public Vector3 StartPosition => startPosition;
    public Quaternion StartRotation => startRotation;
}
