using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Properties")]
    public int MaxHealth;
    public bool CheckAttackColor = true;
    public bool Invulnerable = false;

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

    public void GetHit(GameColor color, Entity attacker, Vector2 bulletPos)
    {
        if (!entity.Alive || Invulnerable) return;

        if (CheckAttackColor && entity.CurrentColor != color)
        {
            if (entity is EnemyController)
            {
                (entity as EnemyController).EnemyBehaviour.Knockback(bulletPos);
            }

            return;
        }

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

            // TODO: Incrementar puntuación del player
        }

        entity.Die();
    }
}
