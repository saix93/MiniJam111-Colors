using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Entity
{
    [Header("Properties")]
    public float MaxDistanceToShootPlayer = 15f;
    public LayerMask LayerMask;

    private EnemyBehaviour enemyBehaviour;

    public EnemyBehaviour EnemyBehaviour => enemyBehaviour;

    private Vector2 vectorToPlayer => GameManager.Player.transform.position - transform.position;

    protected override void Awake()
    {
        base.Awake();

        enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

    protected override void Start()
    {
        Init();
    }

    private void Init()
    {
        gameObject.SetActive(true);
        Respawn();

        var rnd = Random.Range(0, GameManager.GameColors.Colors.Count);
        SwapColor(rnd);

        // StartCoroutine(ShootCR());
    }

    private void Update()
    {
        Aim();

        if (GameManager.PlayerAlive && enemyBehaviour.ShouldShoot) Shoot();
    }

    public void Spawn(Vector2 position)
    {
        transform.position = position;
        Init();
        Health.Init();
        enemyBehaviour.Init();
    }

    public override void Shoot()
    {
        var hit = Physics2D.Raycast(transform.position, vectorToPlayer.normalized, vectorToPlayer.magnitude, LayerMask);

        if (hit || vectorToPlayer.magnitude > MaxDistanceToShootPlayer) return;

        base.Shoot();
    }

    private void Aim()
    {
        var dir = GameManager.PlayerAlive ? vectorToPlayer.normalized : Vector2.zero;

        if (enemyBehaviour.CurrentBehaviour == AgentBehaviour.Searching)
        {
            dir = enemyBehaviour.MovingDirection;
        }

        Weapon.position = (Vector2)transform.position + dir * WeaponDistance;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public override void Die()
    {
        base.Die();

        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
