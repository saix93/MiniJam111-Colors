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
    private string currentControlScheme;

    public bool UsingGamepad => currentControlScheme.Equals(GamepadControlScheme);

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void OnControlsChanged(PlayerInput input)
    {
        Debug.Log($"Input changed from {currentControlScheme} to {input.currentControlScheme}");
        currentControlScheme = input.currentControlScheme;
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

    public void SwapTo0(InputAction.CallbackContext context)
    {
        var swap = context.performed;

        if (swap)
        {
            controller.SwapColor(0);
        }
    }
    public void SwapTo1(InputAction.CallbackContext context)
    {
        var swap = context.performed;

        if (swap)
        {
            controller.SwapColor(1);
        }
    }
    public void SwapTo2(InputAction.CallbackContext context)
    {
        var swap = context.performed;

        if (swap)
        {
            controller.SwapColor(2);
        }
    }
    public void SwapTo3(InputAction.CallbackContext context)
    {
        var swap = context.performed;

        if (swap)
        {
            controller.SwapColor(3);
        }
    }
}
