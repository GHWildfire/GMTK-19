using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Collider2D MainCollider;
    public Collider2D SpriteCollider;

    public bool IsPlayerAbleToPickUp { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsEnabled { get; private set; }
    public float InitSpeed { get; private set; }
    public float Damage { get; private set; }
    public float TravelTime { get; private set; }

    private const float DURATION_BEFORE_PLAYER_CAN_PICK_UP = 1;
    private const float DURATION_BEFORE_STOP_MOVING = 1;
    private const int NUMBER_OF_REBOUND_BEFORE_PLAYER_CAN_PICK_UP = 1;

    private Rigidbody2D rigid2d;

    private float currentPickUpDuration;
    private float currentStopMovingDuration;

    private int reboundNumber;

    public void Init()
    {
        InitSpeed = UpgradeParameters.BulletSpeed;
        Damage = UpgradeParameters.BulletDamages;
        TravelTime = UpgradeParameters.BulletTime;
        transform.localScale *= UpgradeParameters.BulletScaleFactor;
    }

    public void PauseResumeGame(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    private void Awake()
    {
        IsPlayerAbleToPickUp = false;
        IsMoving = true;
        IsEnabled = true;
        InitSpeed = 10;
        reboundNumber = 0;
        currentStopMovingDuration = 0;
        currentPickUpDuration = DURATION_BEFORE_PLAYER_CAN_PICK_UP;
    }

    private void Start()
    {
        rigid2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsEnabled)
        {
            UpdatePickUp();

            if (IsMoving)
            {
                UpdateTravelTime();
            }
            else
            {
                UpdatePushedVelocity();
            }
        }
        else
        {
            rigid2d.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UpdateVelocityWhenTrigger(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        UpdateVelocityWhenTrigger(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == Utility.FromTag(Utility.Tag.WALL))
        {
            reboundNumber++;

            UpdateOrientation(rigid2d.velocity.normalized, transform);
        }
    }

    private void UpdateVelocityWhenTrigger(Collider2D collision)
    {
        if (!IsMoving)
        {
            if (collision.tag == Utility.FromTag(Utility.Tag.ENNEMY))
            {
                SlimeController slimeController = collision.GetComponent<SlimeController>();
                Vector2 velocity = SpriteCollider.transform.position - slimeController.MainCollider.transform.position;

                InitSpeed = slimeController.Rigid2d.velocity.magnitude * slimeController.Rigid2d.mass;

                currentStopMovingDuration = 0;

                reboundNumber++;

                UpdateOrientation(velocity.normalized, transform);
            }
            else if (collision.tag == Utility.FromTag(Utility.Tag.WALL))
            {
                InitSpeed = 1;

                currentStopMovingDuration = 0;
            }
        }
    }

    /// <summary>
    /// Update the orientation of the bullet
    /// </summary>
    private void UpdateOrientation(Vector2 direction, Transform transform)
    {
        float radian = Mathf.Atan2(direction.y, direction.x);
        float degree = radian * Mathf.Rad2Deg;
        float finalDegree = degree - 90;

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, finalDegree));
    }

    private void UpdatePickUp()
    {
        currentPickUpDuration -= Time.deltaTime;

        if (currentPickUpDuration <= 0 || reboundNumber >= NUMBER_OF_REBOUND_BEFORE_PLAYER_CAN_PICK_UP)
        {
            IsPlayerAbleToPickUp = true;
        }
    }

    private void UpdatePushedVelocity()
    {
        currentStopMovingDuration += Time.deltaTime;

        float stopMovingPerc = currentStopMovingDuration / DURATION_BEFORE_STOP_MOVING;
        float speed = Mathf.Lerp(InitSpeed, 0, stopMovingPerc);

        rigid2d.velocity = transform.up * speed;
    }

    private void UpdateTravelTime()
    {
        TravelTime -= Time.deltaTime;

        if (TravelTime <= 0)
        {
            currentStopMovingDuration += Time.deltaTime;

            float stopMovingPerc = currentStopMovingDuration / DURATION_BEFORE_STOP_MOVING;
            float speed = Mathf.Lerp(InitSpeed, 0, stopMovingPerc);

            rigid2d.velocity = transform.up * speed;

            if (speed <= 0)
            {
                IsMoving = false;
                Damage = 0;
            }
        }
        else
        {
            rigid2d.velocity = transform.up * InitSpeed;
        }
    }
}
