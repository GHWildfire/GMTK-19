using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public CircleCollider2D MainCollider;

    public float InitSpeed { get; set; }
    public SlimeManager.SlimeType Type { get; set; }

    private Rigidbody2D rigid2d;

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid2d.velocity = transform.up * InitSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
