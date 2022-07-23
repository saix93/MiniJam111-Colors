using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Properties")]
    public int MaxHealth;
    public bool CheckAttackColor = true;

    private int currentHealth;
    private Entity entity;

    private void Awake()
    {
        entity = GetComponent<Entity>();

        Init();
    }

    public void Init()
    {
        currentHealth = MaxHealth;
    }

    public void GetHit(GameColor color, Entity attacker)
    {
        if (CheckAttackColor && entity.CurrentColor != color) return;

        currentHealth -= color.Damage;

        if (currentHealth <= 0)
        {
            Die(attacker);
        }
    }

    public void Die(Entity attacker)
    {
        if (entity is PlayerController)
        {
            GameManager.ControlEntity(attacker);
        }

        entity.Die();
    }
}
