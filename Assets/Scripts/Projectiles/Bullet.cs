using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer Renderer;
    public Light2D Light;

    [Header("Properties")]
    public float Speed;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(GameColor gc)
    {
        gameObject.SetActive(true);
        Renderer.color = gc.Color;
        Light.color = gc.Color;

        rb.AddForce(transform.right * Speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);
    }
}
