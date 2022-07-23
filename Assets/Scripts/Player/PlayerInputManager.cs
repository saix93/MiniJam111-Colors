using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Properties")]
    public string GamepadControlScheme = "Gamepad";
    public string KBMControlScheme = "KBM";

    private PlayerController controller;

    public bool UsingGamepad => GameManager.CurrentControlScheme.Equals(GamepadControlScheme);

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == null) return;

        Debug.Log($"Input changed from {GameManager.CurrentControlScheme} to {input.currentControlScheme}");
        GameManager.CurrentControlScheme = input.currentControlScheme;
    }

    public void Move(InputAction.CallbackContext context)
    {
        var move = context.ReadValue<Vector2>();

        controller.UpdateMovement(move);
    }

    public void Look(InputAction.CallbackContext context)
    {
        var look = context.ReadValue<Vector2>();

        controller.UpdateAim(look);
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        var shoot = context.performed;

        if (shoot) controller.Shoot();
    }

    public void Reset(InputAction.CallbackContext context)
    {
        var reset = context.performed;

        if (reset)
        {
            GameManager.ResetGame();
        }
    }
}
