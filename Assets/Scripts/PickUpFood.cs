using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PickUpFood : MonoBehaviour
{
    [SerializeField] int kgValue;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.AddScore(kgValue);
            }
            Destroy(gameObject);
        }
    }
}
