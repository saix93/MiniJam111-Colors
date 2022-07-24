using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : Entity
{
    [Header("PlayerController")]
    [Header("References")]
    public Transform CameraFollowTarget;
    public Transform Crosshair;
    public Transform Visual;

    [Header("Properties")]
    public float MovementSpeed = 3f;
    public float GamepadCrosshairDistance = 2f;
    public float GamepadAimStickDeathzone = .1f;
    public bool NormalizeAimWithGamepad = true;
    public MinMaxFloat MinMaxCrosshairDistanceToCalcOffset = new MinMaxFloat(1f, 10f);
    public MinMaxFloat MinMaxCameraOffset = new MinMaxFloat(1f, 3f);
    public bool Invisible;

    private Camera cam;
    private PlayerInputManager inputManager;
    private Vector2 currentMovement;
    private Vector2 lastAimPos;
    private Vector2 currentAimPos;

    public PlayerInputManager Input => inputManager;

    protected override void Awake()
    {
        base.Awake();

        cam = Camera.main;
        inputManager = GetComponent<PlayerInputManager>();
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

    public override void Respawn()
    {
        base.Respawn();

        Visual.gameObject.SetActive(true);
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

    public override void Die()
    {
        base.Die();

        Visual.gameObject.SetActive(false);
    }
}
