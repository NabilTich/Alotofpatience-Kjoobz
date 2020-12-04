using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

class Tile : MonoBehaviour
{
    Color highLightColor;
    Color regularColor;
    MeshRenderer meshRenderer;
    public void SetMaterial(Material material)
    {
        regularColor = material.color;
        meshRenderer.material = material;
    }
    public void SetMeshRenderer(MeshRenderer m)
    {
        meshRenderer = m;
    }
    public void SetHighlightColor(Color color)
    {
        highLightColor = color;
    }
    public void SetTile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;
    public void Highlight()
    {
        StartCoroutine(HighlightCor());
    }
    IEnumerator HighlightCor()
    {
        float lerpvalue = 0f;
        for(int a = 0; a < 2; a++)
        {
            for (int i = 0; i < 10; i++)
            {
                lerpvalue += 0.1f;
                meshRenderer.material.color = Color.Lerp(regularColor, highLightColor, lerpvalue);
                yield return new WaitForSeconds(0.01f);
            }
            for (int i = 0; i < 10; i++)
            {
                lerpvalue -= 0.1f;
                meshRenderer.material.color = Color.Lerp(regularColor, highLightColor, lerpvalue);
                yield return new WaitForSeconds(0.01f);
            }
        }
        meshRenderer.material.color = regularColor;
        
    }
}

