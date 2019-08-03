using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public enum SlimeType
    {
        STANDARD,
        FAST,
        SLOW
    }

    public CircleCollider2D MainCollider;

    public float InitSpeed { get; private set; }
    public SlimeType Type { get; private set; }
    public float CurrentLife { get; private set; }

    private Rigidbody2D rigid2d;

    public void Init(SlimeType type)
    {
        Type = type;
        InitSpeed = GetInitSpeed(type);
        CurrentLife = GetInitLife(type);
    }

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid2d.velocity = transform.up * InitSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Utility.FromTag(Utility.Tag.BULLET))
        {
            CurrentLife -= collision.GetComponent<BulletController>().Damage;

            if (CurrentLife <= 0)
            {
                Destroy(gameObject);
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
        }

        return 0;
    }
}
