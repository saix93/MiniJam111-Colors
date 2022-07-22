using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [Header("References")]
    public Image Crosshair;

    public void UpdateCrosshairPosition(Vector2 newPosition)
    {
        // Crosshair.transform.position = newPosition;
    }
}
