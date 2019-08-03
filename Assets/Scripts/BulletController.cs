using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float initSpeed = 0;

    private Rigidbody2D rigid2d;

    private void Start()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        rigid2d.velocity = transform.up * initSpeed;
    }

    private void Update()
    {
        // Compute the new orientation of the bullet
        float radian = Mathf.Atan2(rigid2d.velocity.normalized.y, rigid2d.velocity.normalized.x);
        float degree = radian * Mathf.Rad2Deg;
        float finalDegree = degree - 90;

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, finalDegree));
    }
}
