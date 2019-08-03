using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime
{
    private float speed;
    private float damages;
    private float size;
    private float bounceness;

    private Sprite sprite;

    public Slime(float speed, float damages, float size, float bounceness, Sprite sprite)
    {
        this.speed = speed;
        this.damages = damages;
        this.size = size;
        this.bounceness = bounceness;
        this.sprite = sprite;
    }

    public void Instantiate(Vector3 pos, GameObject parent)
    {
        GameObject slime = new GameObject("Slime");
        SpriteRenderer renderer = slime.AddComponent<SpriteRenderer>();
        CircleCollider2D collider = slime.AddComponent<CircleCollider2D>();
        renderer.sprite = sprite;
        renderer.sortingLayerName = "Walls";
        
        slime.transform.position = pos;
        slime.transform.localScale = new Vector3(size, size, size);
        slime.transform.parent = parent.transform;

        collider.isTrigger = true;
    }
}
