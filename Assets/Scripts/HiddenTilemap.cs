using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenTilemap : MonoBehaviour
{
    
    Tilemap tilemap;
    private Color color;

    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
        color = tilemap.color;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            color.a = 0.3f;
            tilemap.color = color;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            color.a = 1f;
            tilemap.color = color;
        }
    }
}
