using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

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
    public VisualEffect VFX_Absorb;
    public VisualEffect VFX_Wave;

    [Header("Properties")]
    public MinMaxInt MinMaxNumberOfEnemies = new MinMaxInt(5, 99);
    public float TimeBetweenColorChange = 5f;
    public bool ChangeToRandomColor = true;
    public int RandomSpawnPoints = 5;
    public int DifficultyUpgrade = 1;

    private PlayerController player;
    private float nextColorChange = -9999f;
    private int currentColorIndex = -1;
    private int currentNumberOfEnemies = 0;
    private int score;
    private bool reincarnating = false;

    private static GameManager _;
    private static Pool<Bullet> bulletPool;
    private static Pool<EnemyController> enemyPool;

    public static string CurrentControlScheme;
    public static GameCanvas MainCanvas => _.GameCanvas;
    public static SO_GameColors GameColors => _.SO_Colors;
    public static PlayerController Player => _.player;
    public static bool PlayerAlive => Player != null && Player.Alive;
    public static float NextColorChange => _.nextColorChange;
    public static int Score => _.score;
    public static bool UsingGamepad => Player.Input.UsingGamepad;
    public static bool HasInstance => _ != null;

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

    private void Start()
    {
        score = 0;
        ResetDifficulty();

        var gradient = new Gradient();
        var colorKey = new GradientColorKey[SO_Colors.Colors.Count];

        for (int i = 0; i < SO_Colors.Colors.Count; i++)
        {
            var color = SO_Colors.Colors[i].Color;
            var time = 1f / SO_Colors.Colors.Count * (i + 1);

            colorKey[i].color = color;
            colorKey[i].time = time;
        }

        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);
        gradient.mode = GradientMode.Fixed;

        VFX_Absorb.SetGradient(Shader.PropertyToID("Colors"), gradient);
        VFX_Wave.SetGradient(Shader.PropertyToID("Colors"), gradient);
    }

    private void Update()
    {
        if (PlayerAlive && enemyPool.ActiveItems.Count < currentNumberOfEnemies)
        {
            SpawnEnemy();
        }

        if (nextColorChange <= Time.time)
        {
            nextColorChange = Time.time + TimeBetweenColorChange;

            var newColor = GetNextColor();
            currentColorIndex = newColor;

            if (PlayerAlive) Player.SwapColor(currentColorIndex);
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

        (target as EnemyController).EnemyBehaviour.StopMovement();

        _.PlayerCam.Follow = target.transform;
        _.PlayerCam.LookAt = target.transform;

        _.GameCanvas.ShowGameOver();

        VFX_Absorb.transform.position = target.transform.position;
        VFX_Absorb.transform.parent = target.transform;
        VFX_Absorb.gameObject.SetActive(true);
        Invoke("ResetVFXAbsorb", 3f);

        yield return new WaitUntil(() => reincarnating);

        enemyPool.AllItems.ForEach(e =>
        {
            e.Die();
        });

        _.reincarnating = false;
        _.score = 0;

        currentColorIndex = SO_Colors.GetColorIndex(target.CurrentColor);
        Player.Respawn();
        Player.SwapColor(currentColorIndex);
        Player.transform.position = target.transform.position;

        VFX_Wave.transform.position = Player.transform.position;
        VFX_Wave.transform.parent = Player.transform;
        VFX_Wave.gameObject.SetActive(true);
        Invoke("ResetVFXWave", 3f);

        _.PlayerCam.Follow = Player.CameraFollowTarget;
        _.PlayerCam.LookAt = Player.CameraFollowTarget;

        nextColorChange = Time.time + TimeBetweenColorChange;

        target.Die();

        ResetDifficulty();
    }

    private void ResetVFXAbsorb()
    {
        VFX_Absorb.gameObject.SetActive(false);
    }
    private void ResetVFXWave()
    {
        VFX_Wave.gameObject.SetActive(false);
    }

    private void ResetDifficulty()
    {
        currentNumberOfEnemies = MinMaxNumberOfEnemies.Min;
    }
    public static void UpgradeDifficulty()
    {
        _.currentNumberOfEnemies = _.MinMaxNumberOfEnemies.Clamp(_.currentNumberOfEnemies + _.DifficultyUpgrade);
        _.score++;
    }

    public static void Reincarnate()
    {
        if (PlayerAlive) return;

        _.reincarnating = true;
        _.GameCanvas.Reincarnate();
    }
    public static void QuitGame()
    {
        if (PlayerAlive) return;

        Application.Quit();
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
