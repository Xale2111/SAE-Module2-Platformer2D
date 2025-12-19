using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    //---- ANIMATOR HASH ----
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int VelocityY = Animator.StringToHash("VelocityY");
    private static readonly int Hit = Animator.StringToHash("IsHit");
    
    //---- PUBLIC VARIABLES ----
    
    [Header("Movement")]
    [Space(5)]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _runningSpeed = 8f;
    [SerializeField] private float _jumpForce = 5f;
    
    [Header("Jump Gravity")]
    [Space(5)]
    [SerializeField] private float baseGravity = 1.5f;
    [SerializeField] private float jumpGravity = 0.6f;
    [SerializeField] private float fallGravity = 3.0f;
    [SerializeField] private float gravityIncreaseSpeed = 4f;
    [SerializeField] private float reverseGravity = -1.75f;
    
    [Header("Ground Detection")]
    [Space(10)]
    [SerializeField] private float groundRadius = .05f;
    [SerializeField] private Transform groundPointLeft, groundPointRight;
    [SerializeField] private LayerMask groundMask;

    [Header("Hit Effects")]
    [Space(10)]
    [SerializeField] private float hitForce = 10f;
    [SerializeField] private float hitDuration = 3f;
    [Space(2)]
    [Range(-10,0)][SerializeField] private int hitMalus = -5;
    
    [Header("Cam Follow")]
    [Space(10)]
    [SerializeField] private Transform camTarget;
    [SerializeField] private float camOffsetX = 4f;
    [SerializeField] private float camOffsetSpeed = 7.5f;
    
    [Header("Sprite container")]
    [Space(10)]
    [SerializeField] private GameObject spriteContainer;
    [Min(0.01f)][SerializeField] private float weightScaleMultiplier = 0.05f;
    
    [Header("UI")] [Space(15)] 
    [SerializeField] private UnityEvent OnAddScore;
    [Space(5)][SerializeField] private UnityEvent OnHit;
        
    [Header("Sound")] [Space(10)] 
    [SerializeField] private float walkSoundRate = 0.4f;
    [Space(5)][SerializeField] private float runSoundRate = 0.3f;
    [Space(7.5f)][SerializeField] private UnityEvent OnWalkPlaySound;
    
    //---- PRIVATE VARIABLES ----
    
    //Components to get
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    //Spawn Point
    private GameObject spawnPoint;
    
    //Movements related (Input and check)
    private float xMoveInput;
    private bool runIsPressed = false;
    private bool isGrounded = true;
    
    //Facing Direction
    private bool isFacingRight = true;
    
    //Coyote Time
    private float coyoteTime = 0.15f;
    private float coyoteTimeCounter = 0f;

    //Cam Follow
    private float camResetDelay = 3f;
    private float camResetTimer = 0f;

    //Movements related (Others)
    private float moveSpeed; //Movement speed (local var change depending on runIsPressed)
    bool canMove = true;
    
    //Sound related
    private float soundRate;
    float walkSoundTimer = 0f;
    
    //Damage
    private bool tookDamage = false;
    
    //Gravity direction
    bool isGravityReversed = false;

    //UI Visible values
    private int score;

    private const int MAX_HEALTH = 3;
    private int healthPoints = MAX_HEALTH;
    
    //Sprite Scale 
    private const float defaultScale = 1.5f;
    
    void Start()
    {
        healthPoints = 3;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = spriteContainer.GetComponent<SpriteRenderer>();
        camTarget.localPosition = new Vector3(camOffsetX, camTarget.localPosition.y, camTarget.localPosition.z);
        spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        transform.position = spawnPoint.transform.position;
        _rb.gravityScale = baseGravity;
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
        
        ManageAudio();
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            _rb.linearVelocity = new Vector2(xMoveInput * moveSpeed, _rb.linearVelocity.y);
        }
        else
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            xMoveInput = 0;
        }
        
        HandleJumpGravity();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            xMoveInput = context.ReadValue<Vector2>().x;
        }
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
                _rb.gravityScale = jumpGravity;
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
                camTarget.localPosition = new Vector2(Mathf.Lerp(camTarget.localPosition.x, camOffsetX, Time.deltaTime * camOffsetSpeed), camTarget.localPosition.y);
            }
            else
            {
                camTarget.localPosition = new Vector2(Mathf.Lerp(camTarget.localPosition.x, -camOffsetX, Time.deltaTime * camOffsetSpeed), camTarget.localPosition.y);
            }
            camResetTimer = camResetDelay;            
        }
        else
        {
            camResetTimer -= Time.deltaTime;
        }

        if (camResetTimer < 0)
        {
            camTarget.localPosition = new Vector2(Mathf.Lerp(camTarget.localPosition.x, 0, Time.deltaTime * camOffsetSpeed), camTarget.localPosition.y);   
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
        _rb.AddForce(direction * hitForce, ForceMode2D.Impulse);
        tookDamage = true;
        healthPoints--;
        OnHit.Invoke();
        AddScore(hitMalus);
        Coroutine coroutine = StartCoroutine(ReduceVelocityXWhenHit_CO());
        yield return new WaitForSeconds(hitDuration);
        StopCoroutine(coroutine);
        tookDamage = false;
        canMove = true;
        ManageHealth();
    }

    private IEnumerator ReduceVelocityXWhenHit_CO()
    {
        do
        {
            _rb.linearVelocity = new Vector2(Mathf.Lerp(_rb.linearVelocity.x, 0f, 0.2f), _rb.linearVelocity.y);
            yield return new WaitForSeconds(0.2f);
        } while (tookDamage);
    }
    
    private void HandleJumpGravity()
    {
        if (_rb.linearVelocity.y < 0)
        {
            _rb.gravityScale = fallGravity;
        }

        else if (_rb.linearVelocity.y > 0)
        {
            _rb.gravityScale = Mathf.Lerp(
                _rb.gravityScale,
                baseGravity,
                Time.fixedDeltaTime * gravityIncreaseSpeed
            );
        }
        
        else if (isGrounded)
        {
            _rb.gravityScale = baseGravity;
        }
        
        if (isGravityReversed)
        {
            _rb.gravityScale = reverseGravity;
        }
    }
    
    public void InverseGravity()
    {
        isGravityReversed = !isGravityReversed;
    }

    public void AddScore(int value)
    {
        score += value;
        OnAddScore.Invoke();
        ScalePlayerBasedOnScore();
    }

    public int GetScore()
    {
        return score;
    }

    private void ScalePlayerBasedOnScore()
    {
        Vector2 scale = spriteContainer.transform.localScale;
        scale.x = defaultScale + (0.05f * score);
        if (scale.x >= 0.5f)
        {
            spriteContainer.transform.localScale = scale;   
        }
    }

    private void ManageHealth()
    {
        if (healthPoints <= 0)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = spawnPoint.transform.position;
        healthPoints = MAX_HEALTH;
        OnHit.Invoke();
    }

    public int GetHealthPoints()
    {
        return healthPoints;
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    private void ManageAudio()
    {
        soundRate = runIsPressed ? runSoundRate : walkSoundRate;
        
        if (walkSoundTimer <= 0 && xMoveInput != 0 && isGrounded)
        {
            OnWalkPlaySound.Invoke();
            walkSoundTimer = soundRate;
        }
        
        walkSoundTimer -= Time.deltaTime;
    }

    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundPointLeft.position, groundRadius);
        Gizmos.DrawWireSphere(groundPointRight.position, groundRadius);
    }
}
