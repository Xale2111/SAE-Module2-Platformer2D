using UnityEngine;
using UnityEngine.Events;

public class PickUpFood : MonoBehaviour
{
    [SerializeField] int kgValue;
    [SerializeField] UnityEvent _OnPickUp;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
