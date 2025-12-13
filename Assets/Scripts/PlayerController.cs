using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int VelocityY = Animator.StringToHash("VelocityY");
    private static readonly int Hit = Animator.StringToHash("IsHit");
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _runningSpeed = 8f;
    [SerializeField] private float _jumpForce = 5f;
    
    [SerializeField] private Transform groundPointLeft, groundPointRight;
    [SerializeField] private float groundRadius = .05f;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float hitForce = 10f;
    [SerializeField] private float hitDuration = 3f;
    
    [SerializeField] private Transform camTarget;
    [SerializeField] private float camOffsetX = 4f;
    
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private GameObject spawnPoint;
    
    private float xMoveInput;
    private bool runIsPressed = false;
    private bool isGrounded = true;
    
    private bool isFacingRight = true;
    
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter = 0f;

    private float camResetDelay = 3f;
    private float camResetTimer = 0f;

    private float moveSpeed;
    
    private bool tookDamage = false;
    
    bool canMove = true;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        camTarget.localPosition = new Vector3(camOffsetX, camTarget.localPosition.y, camTarget.localPosition.z);
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        transform.position = spawnPoint.transform.position;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundPointLeft.position, groundRadius, groundMask) || Physics2D.OverlapCircle(groundPointRight.position, groundRadius, groundMask);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        moveSpeed = runIsPressed ? _runningSpeed : _speed;

        if (canMove)
        {
            FlipSprite();
            FlipCamera();
        }
        
        ManageAnimator();
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            _rb.linearVelocity = new Vector2(xMoveInput * moveSpeed, _rb.linearVelocity.y);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        xMoveInput = context.ReadValue<Vector2>().x;
    }
    
    public void OnRun(InputAction.CallbackContext context)
    {
        runIsPressed = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (context.performed && coyoteTimeCounter > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            }

            if (context.canceled && _rb.linearVelocity.y > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
                coyoteTimeCounter = 0f;
            }
        }
    }

    private void FlipCamera()
    {
        if (_rb.linearVelocity.x != 0)
        {
            if (isFacingRight)
            {
                camTarget.localPosition = new Vector3(camOffsetX, camTarget.localPosition.y, camTarget.localPosition.z);
            }
            else
            {
                camTarget.localPosition = new Vector3(camOffsetX*-1f, camTarget.localPosition.y, camTarget.localPosition.z);
            }
            camResetTimer = camResetDelay;            
        }
        else
        {
            camResetTimer -= Time.deltaTime;
        }

        if (camResetTimer < 0)
        {
            camTarget.localPosition = new Vector3(0f, camTarget.localPosition.y, camTarget.localPosition.z);   
        }
    }
    
    private void FlipSprite()
    {
        if (xMoveInput > 0.1f)
        {
            isFacingRight = true;
        }
        else if (xMoveInput < -0.1f)
        {
            isFacingRight = false;
        }
        
        _spriteRenderer.flipX = isFacingRight;
    }
    
    private void ManageAnimator()
    {
        if (xMoveInput != 0)
        {
            _animator.SetBool(IsRunning, true);
        }
        else
        {
            _animator.SetBool(IsRunning, false);
        }

        _animator.SetFloat(VelocityY, _rb.linearVelocity.y);

        _animator.SetBool(Hit,tookDamage);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard") && !tookDamage)
        {
            StartCoroutine(TakeDamage_CO(other.transform));
        }
    }

    private IEnumerator TakeDamage_CO(Transform hitPoint)
    {
        canMove = false;
        _rb.linearVelocity = Vector2.zero;
        xMoveInput = 0;
        Vector2 direction = transform.position - hitPoint.position;
        direction.y = 1;
        if (direction.x == 0)
        {
            direction.x = 1;
        }
        direction.x = Mathf.Sign(direction.x);
        direction.Normalize();
        Debug.Log(direction);
        _rb.AddForce(direction * hitForce, ForceMode2D.Impulse);
        tookDamage = true;
        Coroutine coroutine = StartCoroutine(ReduceVelocityXWhenHit_CO());
        yield return new WaitForSeconds(hitDuration);
        StopCoroutine(coroutine);
        tookDamage = false;
        canMove = true;
    }

    private IEnumerator ReduceVelocityXWhenHit_CO()
    {
        do
        {
            _rb.linearVelocity = new Vector2(Mathf.Lerp(_rb.linearVelocity.x, 0f, 0.2f), _rb.linearVelocity.y);
            yield return new WaitForSeconds(0.2f);
        } while (tookDamage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPointLeft.position, groundRadius);
        Gizmos.DrawWireSphere(groundPointRight.position, groundRadius);
    }
}
