using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ScrollTexture : MonoBehaviour
{
    new Renderer renderer;
    public ScrollTextureReference[] textures;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < textures.Length; i++)
        {
            renderer.materials[textures[i].index].mainTextureOffset += new Vector2(textures[i].xSpeed * Time.deltaTime, textures[i].ySpeed * Time.deltaTime);
        }
    }

    [System.Serializable]
    public struct ScrollTextureReference
    {
        public int index;
        public float xSpeed, ySpeed;
    }
}