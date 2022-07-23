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
    public float TimeBetweenColorChange = 5f;
    public bool ChangeToRandomColor = true;
    public int RandomSpawnPoints = 5;

    private PlayerController player;
    private float nextColorChange = -9999f;
    private int currentColorIndex = -1;

    private static GameManager _;
    private static Pool<Bullet> bulletPool;
    private static Pool<EnemyController> enemyPool;

    public static string CurrentControlScheme;
    public static SO_GameColors GameColors => _.SO_Colors;
    public static PlayerController Player => _.player;
    public static float NextColorChange => _.nextColorChange;

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
        if (enemyPool.ActiveItems.Count < MinNumberOfEnemies)
        {
            SpawnEnemy();
        }

        if (nextColorChange <= Time.time)
        {
            nextColorChange = Time.time + TimeBetweenColorChange;

            var newColor = GetNextColor();
            currentColorIndex = newColor;

            Player.SwapColor(currentColorIndex);
        }

        // TODO: Añadir dificultad creciente cuando el player aguanta mucho
        // reducir la dificultad cuando el player muere.
    }

    private int GetNextColor()
    {
        var index = (currentColorIndex + 1) % SO_Colors.Colors.Count;

        if (ChangeToRandomColor)
        {
            index = Random.Range(0, SO_Colors.Colors.Count);
            if (index == currentColorIndex) return GetNextColor();
        }

        return index;
    }

    private void SpawnEnemy()
    {
        var enemy = GetEnemy();
        var randomSP = SelectSpawnPoint();

        enemy.Spawn(randomSP.position);
    }

    private Transform SelectSpawnPoint()
    {
        var spawnPoints = new List<Transform>();

        foreach (Transform child in SpawnPointsContainer)
        {
            spawnPoints.Add(child);
        }

        spawnPoints.SortByDistance(Player.transform.position, false);
        var rnd = Random.Range(0, RandomSpawnPoints);

        return spawnPoints[rnd];
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
        enemyPool.AllItems.ForEach(e =>
        {
            e.EnemyBehaviour.Init();
        });

        // TODO: Efecto visual en el que se pasan los "poderes" del player a la nueva entidad
        // mostrar mensaje en interfaz, etc...

        yield return null;

        var newPlayer = Instantiate(_.PlayerPrefab);
        currentColorIndex = SO_Colors.GetColorIndex(target.CurrentColor);
        newPlayer.Respawn();
        newPlayer.SwapColor(currentColorIndex);
        newPlayer.transform.position = target.transform.position;
        _.PlayerCam.Follow = newPlayer.CameraFollowTarget;
        _.PlayerCam.LookAt = newPlayer.CameraFollowTarget;

        _.player = newPlayer;

        nextColorChange = Time.time + TimeBetweenColorChange;

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
