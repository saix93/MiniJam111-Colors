using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public List<Light2D> PlayerLights;
    public SpriteRenderer Renderer;
    public Transform CameraFollowTarget;
    public Transform Crosshair;
    public Transform Weapon;

    [Header("Properties")]
    public string PlayerName;
    public SO_GameColors PlayerColors;
    public float SwapColorCooldown;
    public float ShootingCooldown = .25f;
    public float WeaponDistance = 1f;
    public float MovementSpeed = 3f;
    public float GamepadCrosshairDistance = 2f;
    public float GamepadAimStickDeathzone = .1f;
    public bool NormalizeAimWithGamepad = true;
    public MinMaxFloat MinMaxCrosshairDistanceToCalcOffset = new MinMaxFloat(1f, 10f);
    public MinMaxFloat MinMaxCameraOffset = new MinMaxFloat(1f, 3f);

    private Camera cam;
    private Rigidbody2D rb;
    private PlayerInputManager inputManager;
    private Vector2 currentMovement;
    private Vector2 lastAimPos;
    private Vector2 currentAimPos;
    private GameColor currentColor;
    private float currentSwapColorCooldown = -9999f;
    private float currentShootingCooldown = -9999f;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        SwapColor(0);
    }

    private void Update()
    {
        Aim();
        UpdateCameraTargetPosition();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.MovePosition(rb.position + currentMovement * MovementSpeed * Time.fixedDeltaTime);
    }
    private void Aim()
    {
        var aimPosition = cam.ScreenToWorldPoint(currentAimPos);

        if (inputManager.UsingGamepad)
        {
            if (currentAimPos.magnitude <= GamepadAimStickDeathzone) currentAimPos = lastAimPos;

            aimPosition = (Vector2)transform.position + currentAimPos * GamepadCrosshairDistance;
        }

        aimPosition.z = 0;
        var dir = (aimPosition - transform.position).normalized;

        Crosshair.position = aimPosition;
        Weapon.position = transform.position + dir * WeaponDistance;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        lastAimPos = currentAimPos;
    }
    private void UpdateCameraTargetPosition()
    {
        var lookingDir = Crosshair.position - transform.position;
        var a = MinMaxCrosshairDistanceToCalcOffset.InverseLerp(lookingDir.magnitude);
        var b = MinMaxCameraOffset.Lerp(a);

        CameraFollowTarget.position = transform.position + lookingDir.normalized * b;
    }

    public void UpdateMovement(Vector2 newMovement)
    {
        currentMovement = newMovement;
    }
    public void UpdateAim(Vector2 position)
    {
        currentAimPos = position;
    }
    public void Shoot()
    {
        if (currentShootingCooldown >= Time.time) return;

        var bullet = GameManager.GetBullet();
        bullet.transform.position = Weapon.position;
        bullet.transform.rotation = Weapon.rotation;

        bullet.Launch(currentColor);

        currentShootingCooldown = Time.time + ShootingCooldown;
    }

    public void SwapColor(int newColor)
    {
        if (currentSwapColorCooldown >= Time.time) return;

        currentColor = PlayerColors.Colors[newColor];

        PlayerLights.ForEach(l => l.color = currentColor.Color);
        Renderer.color = currentColor.Color;

        currentSwapColorCooldown = Time.time + SwapColorCooldown;
    }
}
