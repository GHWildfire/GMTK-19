using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletModel = null;
    [SerializeField] private GameObject weapon = null;
    public CircleCollider2D MainCollider;

    public GameObject BulletRef { get; private set; }

    private const float MOVE_SPEED = 15;

    private float bulletSpeed = 40;

    private bool isBulletReady;

    private Vector2 currentMove;

    private Rigidbody2D rigid2d;

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        currentMove = new Vector2();
        isBulletReady = true;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerOrientation();

        CheckInputs();
    }

    private void FixedUpdate()
    {
        ApplyMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckBulletPickUp(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckBulletPickUp(collision);
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

    private void ApplyMove()
    {
        rigid2d.velocity = currentMove * MOVE_SPEED;
    }

    private void ShootBullet()
    {
        if (isBulletReady)
        {
            isBulletReady = false;

            GameObject bullet = Instantiate(bulletModel);
            bullet.transform.position = weapon.transform.position;
            bullet.transform.up = transform.up;

            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.InitSpeed = bulletSpeed;
            // Ignore collision between the player and the bullet (trigger ok)
            Physics2D.IgnoreCollision(MainCollider, bulletController.MainCollider);

            BulletRef = bullet;
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

        transform.up = direction;
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
}
