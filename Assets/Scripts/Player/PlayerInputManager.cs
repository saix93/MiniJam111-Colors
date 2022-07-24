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
    private bool shooting;

    public bool UsingGamepad => GameManager.CurrentControlScheme.Equals(GamepadControlScheme);

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (shooting) controller.Shoot();
    }

    public void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == null) return;

        // Debug.Log($"Input changed from {GameManager.CurrentControlScheme} to {input.currentControlScheme}");
        GameManager.CurrentControlScheme = input.currentControlScheme;

        if (GameManager.HasInstance) GameManager.MainCanvas.UpdateControls();
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
        var shoot = context.ReadValue<float>();

        shooting = shoot > 0;
    }

    public void Reincarnate(InputAction.CallbackContext context)
    {
        var reincarnate = context.performed;

        if (reincarnate)
        {
            GameManager.Reincarnate();
        }
    }
    public void CloseGame(InputAction.CallbackContext context)
    {
        var reincarnate = context.performed;

        if (reincarnate)
        {
            GameManager.QuitGame();
        }
    }
}
