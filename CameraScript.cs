using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    GameObject player;
    Vector3 offset;
    bool hasParent;
    private void Start()
    { 
        transform.rotation = Quaternion.Euler(new Vector3(15, 230, 0));
        offset = new Vector3(9f, 5f, 9f);
        player = GameObject.FindGameObjectWithTag("Player");
        
    }
    private void Update()
    {
        transform.position = player.transform.position + offset;
        if (hasParent)
            transform.LookAt(transform.parent);
    }
    public void SetParent(Transform parent)
    {
        transform.parent = parent;
        hasParent = true;
    }
}
