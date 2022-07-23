using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Entity
{
    [Header("EnemyController")]
    public int asd;

    protected override void Start()
    {
        Init();
    }

    private void Init()
    {
        gameObject.SetActive(true);

        var rnd = Random.Range(0, GameManager.GameColors.Colors.Count);
        SwapColor(rnd);

        StartCoroutine(ShootCR());
    }

    private void Update()
    {
        Aim();
    }

    public void Spawn(Vector2 position)
    {
        transform.position = position;
        Health.Init();
        Init();
    }

    private IEnumerator ShootCR()
    {
        while(true)
        {
            yield return new WaitForSeconds(2.5f);

            Shoot();
        }
    }

    private void Aim()
    {
        if (!GameManager.Player) return;

        var dir = (GameManager.Player.transform.position - transform.position).normalized;

        Weapon.position = transform.position + dir * WeaponDistance;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Weapon.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public override void Die()
    {
        base.Die();

        gameObject.SetActive(false);
    }
}
