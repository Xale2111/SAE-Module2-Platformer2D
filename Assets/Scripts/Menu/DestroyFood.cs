using System;
using UnityEngine;

public class DestroyFood : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 3f);   
    }

    /*
    private void OnBecameInvisible()
    {
        Destroy(gameObject, 0.2f);   
    }*/
}
