using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Entity : MonoBehaviour
{
    [Header("Entity")]
    [Header("References")]
    public SpriteRenderer Renderer;
    public Transform Weapon;
    public List<Light2D> EntityLights;

    [Header("Properties")]
    public string EntityName;
    public float WeaponDistance = 1f;
    public float ShootingCooldown = .25f;
    public string BulletLayer;

    private bool alive = true;
    private float currentShootingCooldown = -9999f;
    private Health health;
    protected Rigidbody2D rb;
    protected GameColor currentColor;

    public bool Alive => alive;
    public GameColor CurrentColor => currentColor;
    public Health Health => health;
    public Rigidbody2D Rigidbody => rb;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {

    }

    public virtual void Respawn()
    {
        alive = true;
        health.Init();
    }

    public virtual void Shoot()
    {
        if (currentShootingCooldown >= Time.time) return;

        var bullet = GameManager.GetBullet();
        bullet.transform.position = Weapon.position;
        bullet.transform.rotation = Weapon.rotation;
        bullet.gameObject.layer = LayerMask.NameToLayer(BulletLayer);

        bullet.Launch(this);

        currentShootingCooldown = Time.time + ShootingCooldown;
    }

    public void SwapColor(int newColor)
    {
        currentColor = GameManager.GameColors.Colors[newColor];

        EntityLights.ForEach(l => l.color = currentColor.Color);
        Renderer.color = currentColor.Color;
    }

    public virtual void Die()
    {
        // Animación en la que muere el pj
        alive = false;
    }
}
