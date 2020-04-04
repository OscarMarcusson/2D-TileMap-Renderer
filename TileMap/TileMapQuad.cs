using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapQuad
{
    readonly int m_uvId;
    readonly Vector2 m_quadUvSize;


    public TileMapQuad(int x, int y, ref Vector2 quadSize, ref float randomOffset, int uvX, int uvY, ref Vector2 quadUvSize, ref List<Vector3> vertices, ref List<Vector2> uvs, ref List<int> triangles, ref TileMapDepthMode depthMode)
    {
        // ---   S E T U P   --- \\
        // Save member variables
        m_uvId = uvs.Count;
        m_quadUvSize = quadUvSize;
        int triangleId = vertices.Count;

        // Create offset
        float xOffset = 0f;
        float yOffset = 0f; 
        if(randomOffset != 0f)
        {
            xOffset = Random.Range(-randomOffset, randomOffset) * quadSize.x;
            yOffset = Random.Range(-randomOffset, randomOffset) * quadSize.y;
            RoundToPixel(ref xOffset);
            RoundToPixel(ref yOffset);
        }

        // Create depth
        float depth = 0f;
        if (depthMode == TileMapDepthMode.Height)
            depth = SpriteRenderer.HeightToDepth((y * quadSize.y) + yOffset);
        else if (depthMode == TileMapDepthMode.Individual)
            depth = m_uvId * 0.01f;


        // ---   A P P L Y   T O   M E S H   --- \\
        // Apply the quad vertices. 
        float leftX     = xOffset + (x * quadSize.x);
        float rightX    = xOffset + ((x + 1) * quadSize.x);
        float upperY    = yOffset + ((y + 1) * quadSize.y);
        float lowerY    = yOffset + (y * quadSize.y);
        vertices.Add(new Vector3(leftX,     upperY,     depth));     // Top left
        vertices.Add(new Vector3(rightX,    upperY,     depth));     // Top right
        vertices.Add(new Vector3(leftX,     lowerY,     depth));     // Bot left
        vertices.Add(new Vector3(rightX,    lowerY,     depth));     // Bot right

        // Apply the quad UVs (create empty entries and call the standard method for the sake of simplicity, if needed this can be optimized slightly by copy pasting the vectors of SetUVs directly into .Add() calls here instead)
        uvs.Add(new Vector2());
        uvs.Add(new Vector2());
        uvs.Add(new Vector2());
        uvs.Add(new Vector2());
        SetUVs(uvX, uvY, ref uvs);

        // Apply top left triangle
        triangles.Add(triangleId);        //  0   * * *   1
        triangles.Add(triangleId + 1);    //      * *
        triangles.Add(triangleId + 2);    //  2   *       3

        // Apply bot right triangle
        triangles.Add(triangleId + 1);    //  0       *   1
        triangles.Add(triangleId + 3);    //        * *   
        triangles.Add(triangleId + 2);    //  2   * * *   3
    }


    void RoundToPixel(ref float f)
    {
        f *= 32f;
        f = Mathf.Round(f);
        f /= 32f;
    }


    public void SetUVs(int uvX, int uvY, ref List<Vector2> uvs)
    {
        uvs[m_uvId]       = GetUV(uvX, uvY + 1);        // Top left
        uvs[m_uvId + 1]   = GetUV(uvX + 1, uvY + 1);    // Top right
        uvs[m_uvId + 2]   = GetUV(uvX, uvY);            // Bot left
        uvs[m_uvId + 3]   = GetUV(uvX + 1, uvY);        // Bot right
    }


    Vector2 GetUV(int x, int y)
    {
        return new Vector2(
            x * m_quadUvSize.x,
            y * m_quadUvSize.y);
    }
}
