using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] public CircleCollider2D MainCollider;

    public bool IsPlayerAbleToPickUp { get; private set; }
    public float InitSpeed { get; private set; }
    public float Damage { get; private set; }
    public float TravelTime { get; private set; }

    private const float DURATION_BEFORE_PLAYER_CAN_PICK_UP = 1;
    private const int NUMBER_OF_REBOUND_BEFORE_PLAYER_CAN_PICK_UP = 1;

    private Rigidbody2D rigid2d;

    private float currentPickUpDuration;

    private int reboundNumber;

    public void Init(float initSpeed, float damage, float travelTime)
    {
        InitSpeed = initSpeed;
        Damage = damage;
        TravelTime = travelTime;
    }

    private void Awake()
    {
        IsPlayerAbleToPickUp = false;
        InitSpeed = 10;
        reboundNumber = 0;
        currentPickUpDuration = DURATION_BEFORE_PLAYER_CAN_PICK_UP;
    }

    private void Start()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        rigid2d.velocity = transform.up * InitSpeed;
    }

    private void Update()
    {
        UpdatePickUp();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == Utility.FromTag(Utility.Tag.WALL))
        {
            reboundNumber++;

            UpdateOrientation();
        }
    }

    /// <summary>
    /// Update the orientation of the bullet
    /// </summary>
    private void UpdateOrientation()
    {
        float radian = Mathf.Atan2(rigid2d.velocity.normalized.y, rigid2d.velocity.normalized.x);
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
}
