using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer Renderer;
    public Light2D Light;
    public AudioClip SFX_Laser_Player;
    public AudioClip SFX_Laser_Enemies;
    public float SFX_Volume_Player = 1;
    public float SFX_Volume_Enemies = .25f;

    [Header("Properties")]
    public float Speed;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private GameColor currentColor;
    private Entity entity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Launch(Entity attacker)
    {
        entity = attacker;
        currentColor = entity.CurrentColor;

        gameObject.SetActive(true);
        Renderer.color = currentColor.Color;
        Light.color = currentColor.Color;

        rb.AddForce(transform.right * Speed, ForceMode2D.Impulse);

        var clip = SFX_Laser_Enemies;
        var volume = SFX_Volume_Enemies;

        if (entity is PlayerController)
        {
            clip = SFX_Laser_Player;
            volume = SFX_Volume_Player;
        }

        audioSource.PlayOneShot(clip, volume);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var targetEntity = collision.transform.GetComponent<Entity>();
        if (targetEntity) targetEntity.Health.GetHit(currentColor, entity, transform.position);

        Hit();
    }

    public void Hit()
    {
        gameObject.SetActive(false);
        rb.velocity = Vector2.zero;
    }
}
