using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The tilemap renderer will create multiple quads, sampled from a texture atlas, and add them to a single mesh.
/// </summary>
public class TileMap : MonoBehaviour
{
    // Public member variables
    [Header("Rendering")]
        public Texture m_texture;
        public Shader m_shader;

    [Header("UV Sampling")]
        [Range(1, 16)]  public int m_numberOfHorizontalUVQuads = 4;
        [Range(1, 16)]  public int m_numberOfVerticalUVQuads = 4;

    [Header("Other Settings")]
        [Tooltip("The width, in scene units, to add tiles within. The amount of horizontal quads added will depend on the textures width. ")]public int m_width = 64;
        [Tooltip("The height, in scene units, to add tiles within. The amount of vertical quads added will depend on the textures height. ")]public int m_height = 64;
        [Range(0f,1f)]  public float m_fillPercentage = 0.25f;
        [Range(0f,1f)]  public float m_randomOffset = 0f;
        [Tooltip(   // Forgive this uggly string, but I felt it was needed to make the editor formatting look somewhat readable
            "Flat: Everything is placed at 0 local depth.\n   Useful for backgrounds, like a layer of grass or\n   dirt.\n\n" +
            "Height: Each tile follows the SpriteRenderer logic.\n   This is useful for things that should occlude units\n   and items. To work properly a SpriteRenderer\n   should be applied as well.\n\n" +
            "Individual: Each tile is placed with a slight depth\n   offset, to ensure that they can each be\n   rendererd over each other without causing\n   z-fighting. This is useful for background details.")] 
            public TileMapDepthMode m_depthMode = TileMapDepthMode.Height;

    // Private member variables
    Mesh m_mesh;
    MeshFilter m_meshFilter;
    Material m_material;
    MeshRenderer m_meshRenderer;
    List<TileMapQuad> m_quads;

    // Getters
    public Material material { get { return m_material; } }


    void Start()
    {
        // ---   S E T U P   --- \\
        m_mesh = new Mesh();
        m_mesh.name = "TileMap_" + name;
        m_mesh.MarkDynamic();

        m_quads = new List<TileMapQuad>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        Vector2 quadSize = new Vector2(
            (m_texture.width / (float)m_numberOfHorizontalUVQuads) / 32f,
            (m_texture.height / (float)m_numberOfVerticalUVQuads) / 32f);

        Vector2 quadUvSize = new Vector2(
            1f / (float)m_numberOfHorizontalUVQuads,
            1f / (float)m_numberOfVerticalUVQuads);


        // ---   C R E A T E   M E S H   --- \\
        int horizontalQuads = Mathf.CeilToInt(m_width / quadSize.x);
        int verticalQuads = Mathf.CeilToInt(m_height / quadSize.y);
        for (int x = 0; x < horizontalQuads; x++)
        {
            for(int y = 0; y < verticalQuads; y++)
            {
                if(Random.value <= m_fillPercentage)
                {
                    m_quads.Add(
                        new TileMapQuad(
                            x,
                            y,
                            ref quadSize,
                            ref m_randomOffset,
                            Random.Range(0, m_numberOfHorizontalUVQuads),
                            Random.Range(0, m_numberOfVerticalUVQuads),
                            ref quadUvSize,
                            ref vertices,
                            ref uvs,
                            ref triangles,
                            ref m_depthMode));
                }
            }
        }

        m_mesh.SetVertices(vertices);
        m_mesh.SetUVs(0, uvs);
        m_mesh.SetTriangles(triangles, 0);


        // ---   C R E A T E   R E N D E R I N G   C O M P O N E N T S   --- \\
        // Mesh
        m_meshFilter = gameObject.AddComponent<MeshFilter>();
        m_meshFilter.sharedMesh = m_mesh;

        // Material
        m_material = new Material(m_shader);
        m_material.mainTexture = m_texture;
        m_meshRenderer = gameObject.AddComponent<MeshRenderer>();
        m_meshRenderer.material = m_material;
        m_meshRenderer.receiveShadows = false;
        m_meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        m_meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
    }


    private void OnDestroy()
    {
        // Avoid a memory leak in case this objects get destroyed, yay for Unity specific garbage handling that doesn't work on its own :)
        Destroy(m_material);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Render a green quad in the editor to show the area we are working with.

        Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
        Vector3 size = new Vector3(m_width, m_height, 0.001f);
        Vector3 position = transform.position + size * 0.5f;

        Gizmos.DrawCube(position, size);
        Gizmos.DrawWireCube(position, size);
    }
#endif
}
