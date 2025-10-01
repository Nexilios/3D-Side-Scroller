using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private readonly int _isMovingHash = Animator.StringToHash("isMoving");
    private readonly int _isIdleHash = Animator.StringToHash("isIdle");
    private readonly int _isJumpingHash = Animator.StringToHash("isJumping");
    private readonly int _jumpEndHash = Animator.StringToHash("jumpEnd");

    [Header("Input Actions")] 
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    
    [Header("Player Movement")]
    [Range(0f, 20f)]
    public float playerSpeed = 10f;
    [Range(0f, 20f)]
    public float jumpVelocity = 12f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody _rb;
    private Animator _animator;
    private Transform _playerRoot;
    private Vector2 _moveAmount;
    private bool _isGrounded;
    private bool _facingRight = true;

    [Header("Player States")]
    [SerializeField] private bool isIdle = true;
    [SerializeField] private bool isRunning;
    [SerializeField] private bool isJumping;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerRoot = GameObject.FindGameObjectWithTag("PlayerRoot").transform;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void Update()
    {
        _moveAmount = moveAction.action.ReadValue<Vector2>();

        if (!_isGrounded) return;
        
        if (_moveAmount.x == 0)
        {
            isIdle = true;
            _animator.SetTrigger(_isIdleHash);
        }
        else
        {
            isRunning = true;
            _animator.SetTrigger(_isMovingHash);

            if (_moveAmount.x > 0 && !_facingRight || _moveAmount.x < 0 && _facingRight)
            {
                Flip();
            }
        }
            
        if (jumpAction.action.WasPressedThisFrame())
        {
            _rb.AddForce(new Vector3(0, jumpVelocity, 0), ForceMode.Impulse);
            isJumping = true;
            _animator.SetTrigger(_isJumpingHash);
            _isGrounded = false;
        }
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 theScale = _playerRoot.localScale;
        theScale.y *= -1;
        _playerRoot.localScale = theScale;
    }
    
    private void UpdateState(string stateName, bool state)
    {
        
    }
    
    
    private void FixedUpdate()
    {
        Running();
        JumpPhysics();
    }

    private void JumpPhysics()
    {
        if (_rb.linearVelocity.y < 0)
        { 
            _rb.linearVelocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        } 
        else if (_rb.linearVelocity.y > 0 && !jumpAction.action.IsPressed())
        {
            _rb.linearVelocity += Vector3.up * (Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }

    private void Running()
    {
        Vector3 targetVelocity = transform.forward * (_moveAmount.x * playerSpeed);
        
        _rb.linearVelocity = new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.z);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            _isGrounded = true;
            _animator.SetTrigger(_jumpEndHash);
        }
    }
}