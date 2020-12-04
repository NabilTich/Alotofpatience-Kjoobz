using System;
using System.Collections.Generic;
using UnityEngine;

class Tile : MonoBehaviour
{
    public void SetTile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;
}

