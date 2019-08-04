using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject spritesParent;
    [SerializeField] private List<SpriteRenderer> sprites;
    [SerializeField] private GameObject bulletModel = null;
    [SerializeField] private GameObject weapon = null;
    [SerializeField] private GameObject currentLifeBar = null;

    public CircleCollider2D MainCollider;

    public GameObject BulletRef { get; private set; }
    public float MaxLife { get; private set; }
    public float CurrentLife { get; private set; }
    public bool IsEnabled { get; private set; }

    private const float INVINCIBLE_DURATION = 2;
    private const float ONE_INVINCIBLE_BLINK_DURATION = 0.15f;

    private float oneLifePointOnLifeBar;
    private float currentInvincibleDurationAvailable;
    private float currentInvincibleBlinkDurationAvailable;

    private bool isBulletReady;
    private bool isInvincible;
    private bool isBlinkingOn;

    private Vector2 currentMove;

    private Rigidbody2D rigid2d;

    private float initCurrentLifeBarWidth;

    private AudioSource audioSource;
    private AudioClip gunShot;
    private AudioClip emptyGunShot;

    public void Init(GameObject level, AudioSource audioSource, AudioClip gunShot, AudioClip emptyGunShot)
    {
        this.audioSource = audioSource;
        this.gunShot = gunShot;
        this.emptyGunShot = emptyGunShot;

        // Update the players max life
        float prevLife = MaxLife;
        MaxLife = UpgradeParameters.PlayerLife;
        CurrentLife += MaxLife - prevLife;

        // Heal player if possible
        if (UpgradeParameters.DidPlayerHeal)
        {
            UpgradeParameters.DidPlayerHeal = false;

            CurrentLife = Mathf.Min(CurrentLife + MaxLife * UpgradeParameters.HealPlayerFactor, MaxLife);
        }

        // Spawn the player on a random spawn
        List<GameObject> spawnPoints = new List<GameObject>();
        Transform spawnsParent = level.transform.Find("Spawns");
        for (int i = 0; i < spawnsParent.childCount; i++)
        {
            spawnPoints.Add(spawnsParent.GetChild(i).gameObject);
        }
        GameObject spawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
        transform.position = spawn.transform.position;

        currentMove = new Vector2();
        isBulletReady = true;
        isInvincible = false;
        isBlinkingOn = false;
        currentInvincibleDurationAvailable = 0;
        currentInvincibleBlinkDurationAvailable = 0;
        oneLifePointOnLifeBar = initCurrentLifeBarWidth / MaxLife;

        if (BulletRef != null)
        {
            Destroy(BulletRef);
        }

        IsEnabled = true;
    }

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();

        initCurrentLifeBarWidth = currentLifeBar.transform.localScale.x;

        MaxLife = 0;
        CurrentLife = 0;
    }

    private void OnEnable()
    {
        GameHandler.OnPauseResumeGameEvent += PauseResumeGame;
        GameHandler.OnRestartGameEvent += RestartGame;
    }

    private void OnDisable()
    {
        GameHandler.OnPauseResumeGameEvent -= PauseResumeGame;
        GameHandler.OnRestartGameEvent -= RestartGame;
    }

    private void Update()
    {
        if (IsEnabled)
        {
            UpdatePlayerOrientation();

            CheckInputs();
            CheckInvincibility();

            // Update life bar
            currentLifeBar.transform.localScale = new Vector2(oneLifePointOnLifeBar * CurrentLife, currentLifeBar.transform.localScale.y);
        }
    }

    private void FixedUpdate()
    {
        ApplyMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckBulletPickUp(collision);
        CheckPlayersDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckBulletPickUp(collision);
        CheckPlayersDamage(collision);
    }

    private void RestartGame()
    {
        Destroy(gameObject);
    }

    private void PauseResumeGame(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    private void CheckInputs()
    {
        currentMove = Vector2.zero;

        float moveY = Input.GetAxisRaw("Vertical");
        float moveX = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootBullet();
        }

        if (moveY > 0)
        {
            currentMove.y = 1;
        }
        else if (moveY < 0)
        {
            currentMove.y = -1;
        }

        if (moveX > 0)
        {
            currentMove.x = 1;
        }
        else if (moveX < 0)
        {
            currentMove.x = -1;
        }

        if (currentMove.y != 0 && currentMove.x != 0)
        {
            currentMove.y /= Mathf.Sqrt(2);
            currentMove.x /= Mathf.Sqrt(2);
        }
    }

    private void CheckInvincibility()
    {
        currentInvincibleDurationAvailable -= Time.deltaTime;

        isInvincible = currentInvincibleDurationAvailable > 0;

        if (isInvincible)
        {
            currentInvincibleBlinkDurationAvailable -= Time.deltaTime;

            if (currentInvincibleBlinkDurationAvailable <= 0)
            {
                float alpha;

                foreach (SpriteRenderer item in sprites)
                {
                    alpha = 1;

                    if (!isBlinkingOn)
                    {
                        alpha = 0.2f;
                    }

                    item.color = new Color(item.color.r, item.color.g, item.color.b, alpha);
                }

                isBlinkingOn = !isBlinkingOn;
                currentInvincibleBlinkDurationAvailable = ONE_INVINCIBLE_BLINK_DURATION;
            }
        }
        else
        {
            isBlinkingOn = false;

            foreach (SpriteRenderer item in sprites)
            {
                item.color = new Color(item.color.r, item.color.g, item.color.b, 1);
            }
        }
    }

    private void ApplyMove()
    {
        rigid2d.velocity = currentMove * UpgradeParameters.PlayerSpeed;
    }

    private void ShootBullet()
    {
        if (isBulletReady)
        {
            isBulletReady = false;

            GameObject bullet = Instantiate(bulletModel);
            bullet.transform.position = weapon.transform.position;
            bullet.transform.up = spritesParent.transform.up;

            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.Init();

            // Ignore collision between the player and the bullet (trigger ok)
            Physics2D.IgnoreCollision(MainCollider, bulletController.MainCollider);

            BulletRef = bullet;

            audioSource.clip = gunShot;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = emptyGunShot;
            audioSource.Play();
        }
    }

    private void UpdatePlayerOrientation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = GameHandler.SharedCam.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(
            mousePos.x - transform.position.x,
            mousePos.y - transform.position.y
        );

        spritesParent.transform.up = direction;
    }

    private void CheckBulletPickUp(Collider2D collision)
    {
        if (collision.transform.tag == Utility.FromTag(Utility.Tag.BULLET))
        {
            if (collision.GetComponent<BulletController>().IsPlayerAbleToPickUp)
            {
                Destroy(collision.gameObject);
                isBulletReady = true;
            }
        }
    }

    private void CheckPlayersDamage(Collider2D collision)
    {
        if (collision.tag == Utility.FromTag(Utility.Tag.ENNEMY) && !isInvincible)
        {
            CurrentLife--;
            currentInvincibleDurationAvailable = INVINCIBLE_DURATION;

            if (CurrentLife <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
