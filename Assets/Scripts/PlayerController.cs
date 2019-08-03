using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject bulletModel = null;
    [SerializeField] private GameObject weapon = null;

    private bool isBulletReady;

    private Vector3 weaponLauncherPos;

    private void Awake()
    {
        isBulletReady = false;
        weaponLauncherPos = weapon.transform.position;// + weapon.transform.up * weapon.GetComponent<RectTransform>().rect.height / 2;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        if (isBulletReady)
        {
            isBulletReady = false;

            GameObject bullet = Instantiate(bulletModel);
            bullet.transform.position = weaponLauncherPos;
            bullet.transform.up = transform.up;
            bullet.GetComponent<BulletController>().InitSpeed = 20;
        }
    }

    private void UpdatePlayerOrientation()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

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
