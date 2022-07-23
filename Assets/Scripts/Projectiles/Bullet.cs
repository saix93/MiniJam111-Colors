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
    private GameColor currentColor;
    private Entity entity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Entity attacker)
    {
        entity = attacker;
        currentColor = entity.CurrentColor;

        gameObject.SetActive(true);
        Renderer.color = currentColor.Color;
        Light.color = currentColor.Color;

        rb.AddForce(transform.right * Speed, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var targetEntity = collision.transform.GetComponent<Entity>();
        if (targetEntity) targetEntity.Health.GetHit(currentColor, entity);

        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }
}
