using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private readonly int _speedHash = Animator.StringToHash("Speed");
    private readonly int _jumpHash = Animator.StringToHash("Jump");

    [Header("Input Actions")] 
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    
    [Header("Player Movement")]
    public float playerSpeed = 10f;
    [Range(0f, 50f)]
    public float jumpVelocity = 12f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody _rb;
    private Animator _animator;
    private Vector2 _moveAmount;
    private bool _isGrounded;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
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
        
        if (jumpAction.action.WasPressedThisFrame() && _isGrounded)
        {
            _rb.AddForce(new Vector3(0, jumpVelocity, 0), ForceMode.Impulse);
            _isGrounded = false;
        }
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
        //_animator.SetFloat(_speedHash, _moveAmount.y);
        
        Vector3 targetVelocity = transform.forward * (_moveAmount.x * playerSpeed);
        
        _rb.linearVelocity = new Vector3(targetVelocity.x, _rb.linearVelocity.y, targetVelocity.z);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            _isGrounded = true;
        }
    }
}