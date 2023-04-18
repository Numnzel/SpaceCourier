using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileToScreenSize : MonoBehaviour {

    void Start() {

        // We get the reference to the renderer component
        Renderer renderer = GetComponent<Renderer>();

        // The ortographic size is half the height of the camera
        // Screen.width is the width of the screen in pixels
        // Screen.height is the height of the screen in pixels
        Vector2 backgroundHalfSize = new Vector2((Camera.main.orthographicSize * Screen.width / Screen.height), Camera.main.orthographicSize);

        // We set the scale of the bacground to fix it to the screen size
        transform.localScale = new Vector3(backgroundHalfSize.x * 2, backgroundHalfSize.y * 2, transform.localScale.z);

        // We adjust the tilling for it to be proportional to the scale
        renderer.material.SetTextureScale("_MainTex", backgroundHalfSize);
    }
}