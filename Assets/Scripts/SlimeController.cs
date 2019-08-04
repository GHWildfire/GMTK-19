using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public enum SlimeType
    {
        STANDARD,
        FAST,
        SLOW,
        BOSS1,
        BOSS2,
        BOSS3,
        FINAL_BOSS
    }

    public CircleCollider2D MainCollider;
    public GameObject Body;
    public GameObject BodySprite;
    [SerializeField] private GameObject lifeBarParent;
    [SerializeField] private GameObject currentLifeBar;
    [SerializeField] private Animator animator;
    
    private AudioSource audio;
    private AudioClip deathSound;

    public float InitSpeed { get; private set; }
    public float CurrentLife { get; private set; }
    public SlimeType Type { get; private set; }
    public Rigidbody2D Rigid2d { get; private set; }

    private const float DAMAGING_DURATION = 1;

    private float initRotationSpeed;
    private float oneLifePointOnLifeBar;
    private float currentDamagingDuration;

    public void Init(SlimeType type, AudioSource audio, AudioClip deathSound)
    {
        this.audio = audio;
        this.deathSound = deathSound;

        Type = type;
        InitSpeed = GetInitSpeed(type) / UpgradeParameters.EnnemySpeedFactor;
        CurrentLife = GetInitLife(type);
        initRotationSpeed = GetInitRotationSpeed(type);
        oneLifePointOnLifeBar = currentLifeBar.transform.localScale.x / CurrentLife;

        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            initRotationSpeed *= -1;
        }
    }

    private void Awake()
    {
        Rigid2d = GetComponent<Rigidbody2D>();

        currentDamagingDuration = 0;

        lifeBarParent.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigid2d.velocity = Body.transform.up * InitSpeed;
    }

    private void Update()
    {
        currentDamagingDuration -= Time.deltaTime;

        animator.SetBool("Damaged", currentDamagingDuration > 0);

        // Rotate sprite
        BodySprite.transform.Rotate(transform.forward * initRotationSpeed);

        // Update life bar
        currentLifeBar.transform.localScale = new Vector2(oneLifePointOnLifeBar * CurrentLife, currentLifeBar.transform.localScale.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Utility.FromTag(Utility.Tag.BULLET))
        {
            if (collision.GetComponent<BulletController>().Damage > 0)
            {
                CurrentLife -= collision.GetComponent<BulletController>().Damage;
                lifeBarParent.SetActive(true);
                currentDamagingDuration = DAMAGING_DURATION;

                if (CurrentLife <= 0)
                {
                    audio.clip = deathSound;
                    audio.Play();
                    Destroy(gameObject);
                }
            }
        }
    }

    private float GetInitSpeed(SlimeType type)
    {
        switch (type)
        {
            case SlimeType.STANDARD:
                return 12;

            case SlimeType.FAST:
                return 16;

            case SlimeType.SLOW:
                return 8;

            case SlimeType.BOSS1:
                return 16;

            case SlimeType.BOSS2:
                return 12;

            case SlimeType.BOSS3:
                return 60;

            case SlimeType.FINAL_BOSS:
                return 25;
        }

        return 0;
    }

    private float GetInitLife(SlimeType type)
    {
        switch (type)
        {
            case SlimeType.STANDARD:
                return 2;

            case SlimeType.FAST:
                return 1;

            case SlimeType.SLOW:
                return 4;

            case SlimeType.BOSS1:
                return 10;

            case SlimeType.BOSS2:
                return 30;

            case SlimeType.BOSS3:
                return 15;

            case SlimeType.FINAL_BOSS:
                return 100;
        }

        return 0;
    }

    private float GetInitRotationSpeed(SlimeType type)
    {
        switch (type)
        {
            case SlimeType.STANDARD:
                return 6;

            case SlimeType.FAST:
                return 10;

            case SlimeType.SLOW:
                return 2;

            case SlimeType.BOSS1:
                return 2;

            case SlimeType.BOSS2:
                return 1;
                
            case SlimeType.BOSS3:
                return 10;

            case SlimeType.FINAL_BOSS:
                return 2;
        }

        return 0;
    }
}
