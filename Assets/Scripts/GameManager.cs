using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameCanvas GameCanvas;
    public Bullet BulletPrefab;

    [Header("Properties")]
    public bool Debug = true;

    private static GameManager _;
    private static Pool<Bullet> bulletPool;

    private void Awake()
    {
        _ = this;

        var poolGO = new GameObject("BulletPool");
        bulletPool = new Pool<Bullet>(BulletPrefab, 10, poolGO.transform);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public static void UpdateCrosshairPosition(Vector2 newPos)
    {
        _.GameCanvas.UpdateCrosshairPosition(newPos);
    }

    public static Bullet GetBullet()
    {
        return bulletPool.GetItem();
    }
}
