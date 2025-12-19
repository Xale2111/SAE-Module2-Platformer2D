using System;
using UnityEngine;
using UnityEngine.Events;

public class ReverseGravity : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _bottomConfiner;
    [SerializeField] private BoxCollider2D _topConfiner;
    [SerializeField] UnityEvent OnReverseGravity;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _bottomConfiner.enabled = true;
            OnReverseGravity.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnReverseGravity.Invoke();
            BoxCollider2D gravityCollider = GetComponent<BoxCollider2D>();
            Destroy(gravityCollider);
            _topConfiner.enabled = true;
        }
    }
}

