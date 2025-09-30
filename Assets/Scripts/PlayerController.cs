using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float maxJumpTime = 0.3f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float fallGravityMultiplier = 2f;
    [SerializeField] private float lowJumpMultiplier = 2.5f;
    
    [Header("Physics Interaction")]
    [SerializeField] private float pushPower = 5f;
    [SerializeField] private float pushUpwardForce = 0.5f;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference runAction;
    
    private CharacterController _controller;
    private Vector3 _velocity;
    private float _currentSpeed;
    private float _horizontalInput;
    private bool _isRunning;
    private bool _jumpHeld;
    private float _jumpTimeCounter;
    private bool _isJumping;
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Enable();
        if (jumpAction != null && jumpAction.action != null)
            jumpAction.action.Enable();
        if (runAction != null && runAction.action != null)
            runAction.action.Enable();
    }
    
    void OnDestroy()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();
        if (jumpAction != null && jumpAction.action != null)
            jumpAction.action.Disable();
        if (runAction != null && runAction.action != null)
            runAction.action.Disable();
    }
    
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleJump();
        ApplyCustomGravity();
        
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    void HandleInput()
    {
        if (moveAction && moveAction.action != null)
        {
            Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
            _horizontalInput = moveInput.x;
        }
        else
        {
            _horizontalInput = 0f;
        }
        
        if (runAction && runAction.action != null)
        {
            _isRunning = runAction.action.IsPressed();
        }
        else
        {
            _isRunning = false;
        }
        
        if (jumpAction && jumpAction.action != null)
        {
            _jumpHeld = jumpAction.action.IsPressed();
        }
        else
        {
            _jumpHeld = false;
        }
    }
    
    void HandleMovement()
    {
        float targetSpeed = _isRunning ? runSpeed : walkSpeed;
        
        _currentSpeed = _horizontalInput != 0 ? Mathf.MoveTowards(_currentSpeed, targetSpeed * _horizontalInput, acceleration * Time.deltaTime) : Mathf.MoveTowards(_currentSpeed, 0f, deceleration * Time.deltaTime);

        _velocity.x = _currentSpeed;
    }
    
    void HandleJump()
    {
        bool isGrounded = _controller.isGrounded;
        
        if (jumpAction && jumpAction.action != null)
        {
            bool jumpPressed = jumpAction.action.WasPressedThisFrame();
            
            if (jumpPressed && isGrounded)
            {
                // Start jump
                _isJumping = true;
                _jumpTimeCounter = maxJumpTime;
                _velocity.y = jumpForce;
            }
        }
        
        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
            _isJumping = false;
            _jumpTimeCounter = 0f;
        }
        
        if (_isJumping)
        {
            if (_jumpHeld && _jumpTimeCounter > 0)
            {
                _jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                _isJumping = false;
            }
        }
        
        if (!_jumpHeld && _isJumping)
        {
            _isJumping = false;
            _jumpTimeCounter = 0f;
        }
    }
    
    void ApplyCustomGravity()
    {
        if (_isJumping && _jumpHeld && _jumpTimeCounter > 0)
        {
            _velocity.y += gravity * Time.deltaTime;
        }
        else if (_velocity.y > 0 && !_jumpHeld)
        {
            _velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
        }
        else if (_velocity.y < 0)
        {
            _velocity.y += gravity * fallGravityMultiplier * Time.deltaTime;
        }
        else if (!_isJumping)
        {
            _velocity.y += gravity * Time.deltaTime;
        }
    }
    
    // Handle physics interactions with rigidbodies (like the soccer ball)
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        
        if (body == null || body.isKinematic)
            return;
        
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z) { y = pushUpwardForce };
        
        float currentMovementSpeed = Mathf.Abs(_currentSpeed);
        float speedMultiplier = _isRunning ? 1.5f : 1f;
        
        body.linearVelocity = pushDir * pushPower * currentMovementSpeed * speedMultiplier;
    }
}