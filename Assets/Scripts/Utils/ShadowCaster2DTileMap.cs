using System.Linq;
using System.Reflection;
using UnityEngine;


[RequireComponent(typeof(CompositeCollider2D))]
public class ShadowCaster2DTileMap : MonoBehaviour
{

    [Space]
    [SerializeField]
    private bool selfShadows = true;

    private CompositeCollider2D tilemapCollider;


    static readonly FieldInfo meshField = typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathField = typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo generateShadowMeshMethod = typeof(UnityEngine.Rendering.Universal.ShadowCaster2D)
                                    .Assembly
                                    .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
    public void Generate()
    {
        DestroyAllChildren();

        tilemapCollider = GetComponent<CompositeCollider2D>();

        for (int i = 0; i < tilemapCollider.pathCount; i++)
        {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            tilemapCollider.GetPath(i, pathVertices);
            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.parent = gameObject.transform;
            shadowCaster.transform.position = CalcCenter(pathVertices);
            UnityEngine.Rendering.Universal.ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<UnityEngine.Rendering.Universal.ShadowCaster2D>();
            shadowCasterComponent.selfShadows = this.selfShadows;

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++)
            {
                testPath[j] = pathVertices[j] - (Vector2)shadowCaster.transform.position;
            }

            shapePathField.SetValue(shadowCasterComponent, testPath);
            meshField.SetValue(shadowCasterComponent, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });
        }

        // Debug.Log("Generate");

    }
    public void DestroyAllChildren()
    {

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }

    }

    private Vector2 CalcCenter(Vector2[] pathVertices)
    {
        float minX = Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float maxY = -Mathf.Infinity;

        foreach (var v in pathVertices)
        {
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
            if (v.y < minY) minY = v.y;
            if (v.y > maxY) maxY = v.y;
        }

        return new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
    }
}