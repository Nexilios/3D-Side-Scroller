using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))] [RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private enum EPlayerStates
    {
        Idle,
        Run,
        Jump,
        Victory,
        Dead
    }

    private readonly Dictionary<string, int> _animHashes = new()
    {
        { "isMovingHash", Animator.StringToHash("isMoving")  },
        { "isIdleHash", Animator.StringToHash("isIdle") },
        { "isJumpingHash", Animator.StringToHash("isJumping") },
        { "stageCompleteHash", Animator.StringToHash("stageComplete") },
    };
    
    [Header("Input Actions")] 
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference pauseAction;
    
    [Header("Player Movement Settings")]
    [Range(0f, 20f)]
    public float playerSpeed = 10f;
    [Range(0f, 20f)]
    public float jumpVelocity = 12f;
    [Range(0f, 10f)]
    public float fallMultiplier = 2.5f;
    [Range(0f, 10f)]
    public float lowJumpMultiplier = 2f;

    private Rigidbody _rb;
    private Animator _animator;
    private Transform _playerRoot;
    private Vector2 _moveAmount;
    private bool _isGrounded;
    private bool _facingRight = true;

    private Dictionary<EPlayerStates, bool> _playerStates;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _playerRoot = GameObject.FindGameObjectWithTag("PlayerRoot").transform;
        
        _playerStates = new Dictionary<EPlayerStates, bool>
        {
            { EPlayerStates.Idle, true },
            { EPlayerStates.Run, false },
            { EPlayerStates.Jump, false },
            { EPlayerStates.Victory, false },
            { EPlayerStates.Dead, false }
        };
    }

    private void OnEnable()
    {
        EnableGameplayInput();
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        DisableGameplayInput();
        pauseAction.action.Disable();
    }

    public void EnableGameplayInput()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    public void DisableGameplayInput()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void Update()
    {
        _moveAmount = moveAction.action.ReadValue<Vector2>();

        if (_moveAmount.x > 0 && !_facingRight || _moveAmount.x < 0 && _facingRight)
        {
            Flip();
        }

        if (!_isGrounded) return;

        ChangeState(_moveAmount.x == 0 ? EPlayerStates.Idle : EPlayerStates.Run);

        if (jumpAction.action.WasPressedThisFrame())
        {
            _rb.AddForce(new Vector3(0, jumpVelocity, 0), ForceMode.Impulse);
            ChangeState(EPlayerStates.Jump);
            
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

    public void KillPlayer()
    {
        ChangeState(EPlayerStates.Dead);
        DisableGameplayInput();
    }

    public void StageComplete()
    {
        ChangeState(EPlayerStates.Victory);
        DisableGameplayInput();
    }
    
    private void ChangeState(EPlayerStates stateName)
    {
        if (_playerStates[stateName].Equals(true)) return;
        
        foreach (var st in _playerStates.ToList())
        {
            _playerStates[st.Key] = false;
        }

        foreach (var hash in _animHashes)
        {
            _animator.ResetTrigger(hash.Value);
        }
        
        switch (stateName)
        {
            case EPlayerStates.Idle:
                _playerStates[EPlayerStates.Idle] = true;
                _animator.SetTrigger(_animHashes["isIdleHash"]);
                break;
            case EPlayerStates.Run:
                _playerStates[EPlayerStates.Run] = true;
                _animator.SetTrigger(_animHashes["isMovingHash"]);
                break;
            case EPlayerStates.Jump:
                _playerStates[EPlayerStates.Jump] = true;
                _animator.SetTrigger(_animHashes["isJumpingHash"]);
                break;
            case EPlayerStates.Victory:
                _animator.SetTrigger(_animHashes["stageCompleteHash"]);
                _playerStates[EPlayerStates.Victory] = true;
                break;
            case EPlayerStates.Dead:
                _playerStates[EPlayerStates.Dead] = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stateName), stateName, null);
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