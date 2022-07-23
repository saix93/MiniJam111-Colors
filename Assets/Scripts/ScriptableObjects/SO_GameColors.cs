using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameColors", menuName = "ScriptableObjects/GameColors")]
public class SO_GameColors : ScriptableObject
{
    public List<GameColor> Colors;

    public int GetColorIndex(GameColor color)
    {
        return Colors.IndexOf(color);
    }
}

[System.Serializable]
public class GameColor
{
    public string Name;
    public Color Color;
    public int Damage;
}
