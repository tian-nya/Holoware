using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedRenderer : MonoBehaviour
{
    new public Renderer renderer;
    public Vector2 tiles;
    public float framesPerSecond = 10;

    Vector2 size, offset;
    int set, index;

    void Start()
    {
        set = Random.Range(0, (int)tiles.y);
        index = Random.Range(0, (int)tiles.x);
        size = new Vector2(1 / tiles.x, 1 / tiles.y);
        StartCoroutine(AnimateTexture());
    }

    IEnumerator AnimateTexture()
    {
        while (true)
        {
            offset = new Vector2(index * size.x, 1 - (set * size.y));
            renderer.material.SetTextureOffset("_MainTex", offset);
            renderer.material.SetTextureScale("_MainTex", size);
            yield return new WaitForSeconds(1 / framesPerSecond);
            index++;
            if (index == tiles.x) index = 0;
        }
    }
}