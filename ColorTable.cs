using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTable : MonoBehaviour
{
    public Material[] table;
    public int ToMaterial(Material color)
    {
        for (int i = 0; i < table.Length; i++)
        {
            if (color == table[i])
            {
                return i;
            }
        }
        throw new System.Exception("color input not in colortable");
    }
    public Material ToMaterial(int integer)
    {
        return table[integer];
    }
}
