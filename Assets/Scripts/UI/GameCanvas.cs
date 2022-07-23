using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI ChangeColorTimer;

    private void Update()
    {
        var timer = (GameManager.NextColorChange - Time.time).ToString();
        ChangeColorTimer.text = string.Format("{0:0}", timer);
    }
}
