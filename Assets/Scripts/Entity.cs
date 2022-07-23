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

    private float currentShootingCooldown = -9999f;
    private Health health;
    protected GameColor currentColor;

    public GameColor CurrentColor => currentColor;
    public Health Health => health;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
    }

    protected virtual void Start()
    {

    }

    public void Shoot()
    {
        if (currentShootingCooldown >= Time.time) return;

        var bullet = GameManager.GetBullet();
        bullet.transform.position = Weapon.position;
        bullet.transform.rotation = Weapon.rotation;
        bullet.gameObject.layer = LayerMask.NameToLayer(BulletLayer);

        bullet.Launch(this);

        currentShootingCooldown = Time.time + ShootingCooldown;
    }

    public virtual void SwapColor(int newColor)
    {
        currentColor = GameManager.GameColors.Colors[newColor];

        EntityLights.ForEach(l => l.color = currentColor.Color);
        Renderer.color = currentColor.Color;
    }

    public virtual void Die()
    {
        // Animación en la que muere el pj

    }
}
