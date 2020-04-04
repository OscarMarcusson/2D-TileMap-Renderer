using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple class that will set the z value of a transforms position depending on its y value.
/// This is useful for rendering 3D objects to a 2D style game, where objects further down on the screen should occlude objects above them no matter what the camera says. 
/// </summary>
public class SpriteRenderer : MonoBehaviour
{
    /// <summary>
    /// If you can't use the SpriteRenderer, this function can be called to perform the same functionality manually. Remember that the default SpriteRenderer calls this function like "HeightToDepth(position.y - camera.position.y)", if you don't do this as well you will get the shift in local space. 
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public static float HeightToDepth(float height)
    {
        return height * 0.1f;
    }

    // Set the z value based on the y position. This is camera relative, so that we don't need to have a farclip of 1e7 in case of large vertical scrolling.
    private void LateUpdate()
    {
        transform.position = new Vector3(
            transform.position.x, 
            transform.position.y,
            HeightToDepth(transform.position.y - Camera.main.transform.position.y));
    }
}
