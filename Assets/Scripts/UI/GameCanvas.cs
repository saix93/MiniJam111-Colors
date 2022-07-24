using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI ChangeColorTimer;
    public TextMeshProUGUI Score;
    public Image ReincarnateButton;
    public Image CloseGameButton;
    public GameObject GameOverPanel;
    public Sprite KeyboardReincarnateSprite;
    public Sprite GamepadReincarnateSprite;
    public Sprite KeyboardCloseGameSprite;
    public Sprite GamepadCloseGameSprite;

    private void Start()
    {
        GameOverPanel.SetActive(false);
        UpdateControls();
    }

    private void Update()
    {
        var timer = (GameManager.NextColorChange - Time.time).ToString("0");
        ChangeColorTimer.text = timer;
    }
    
    public void ShowGameOver()
    {
        Score.text = $"Score: {GameManager.Score}";
        GameOverPanel.SetActive(true);
    }

    public void Reincarnate()
    {
        GameOverPanel.SetActive(false);
    }

    public void UpdateControls()
    {
        ReincarnateButton.sprite = GameManager.UsingGamepad ? GamepadReincarnateSprite : KeyboardReincarnateSprite;
        CloseGameButton.sprite = GameManager.UsingGamepad ? GamepadCloseGameSprite : KeyboardCloseGameSprite;
    }
}
