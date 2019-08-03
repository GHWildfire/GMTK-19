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
        float radian = Mathf.Atan2(rigid2d.velocity.normalized.y, rigid2d.velocity.normalized.x);
        Debug.Log("r " + radian);
        float degree = radian * Mathf.Rad2Deg;
        Debug.Log("d " + degree);

        float finalDegree = degree - 90;
        Debug.Log("fd " + finalDegree);

        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, finalDegree));
    }
}
