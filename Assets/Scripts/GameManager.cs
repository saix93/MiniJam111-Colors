using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameCanvas GameCanvas;
    public CinemachineVirtualCamera PlayerCam;
    public Transform SpawnPointsContainer;
    public SO_GameColors SO_Colors;
    public Bullet BulletPrefab;
    public EnemyController EnemyPrefab;
    public PlayerController PlayerPrefab;

    [Header("Properties")]
    public bool Debug = true;
    public int MinNumberOfEnemies = 5;

    private PlayerController player;

    private static GameManager _;
    private static Pool<Bullet> bulletPool;
    private static Pool<EnemyController> enemyPool;

    public static string CurrentControlScheme;
    public static SO_GameColors GameColors => _.SO_Colors;
    public static PlayerController Player => _.player;

    private void Awake()
    {
        _ = this;

        player = FindObjectOfType<PlayerController>();

        var bulletPoolGO = new GameObject("BulletPool");
        bulletPool = new Pool<Bullet>(BulletPrefab, 10, bulletPoolGO.transform);

        var enemyPoolGO = new GameObject("EnemyPool");
        enemyPool = new Pool<EnemyController>(EnemyPrefab, 10, enemyPoolGO.transform);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        // Mirar numero de enemigos y spawnear X

        if (enemyPool.ActiveItems < MinNumberOfEnemies)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        var enemy = GetEnemy();
        var randomSP = SpawnPointsContainer.GetChild(Random.Range(0, SpawnPointsContainer.childCount));

        enemy.Spawn(randomSP.position);
    }

    public static Bullet GetBullet()
    {
        return bulletPool.GetItem();
    }
    public static EnemyController GetEnemy()
    {
        return enemyPool.GetItem();
    }

    public static void ControlEntity(Entity target)
    {
        _.StartCoroutine(_.ControlEntityCR(target));
    }

    private IEnumerator ControlEntityCR(Entity target)
    {
        // TODO: Efecto visual en el que se pasan los "poderes" del player a la nueva entidad
        // cuando termina...

        yield return null;

        var newPlayer = Instantiate(_.PlayerPrefab);
        newPlayer.transform.position = target.transform.position;
        _.PlayerCam.Follow = newPlayer.CameraFollowTarget;
        _.PlayerCam.LookAt = newPlayer.CameraFollowTarget;

        _.player = newPlayer;

        target.Die();
    }

    public static void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (Transform child in SpawnPointsContainer)
        {
            Gizmos.DrawWireSphere(child.position, .25f);
        }
    }
}
