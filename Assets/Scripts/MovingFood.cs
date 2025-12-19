using UnityEngine;

public class MovingFood : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");

    [SerializeField] private BoxCollider2D _triggerCollider2d;
    [SerializeField] private BoxCollider2D _containerCollider2d;
    
    Animator _animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetBool(Move,true);
            Destroy(_triggerCollider2d);
            Destroy(_containerCollider2d,2f);
        }
    }
}
