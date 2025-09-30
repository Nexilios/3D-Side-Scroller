using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTarget;
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);
    
    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minXBound = -10f;
    [SerializeField] private float maxXBound = 10f;
    [SerializeField] private float minYBound = -5f;
    [SerializeField] private float maxYBound = 5f;
    
    [Header("Look Ahead (Optional)")]
    [SerializeField] private bool useLookAhead = true;
    [SerializeField] private float lookAheadDistance = 2f;
    [SerializeField] private float lookAheadSpeed = 2f;
    
    private Vector3 currentVelocity;
    private Vector3 lookAheadOffset;
    private Vector3 targetPosition;
    
    private PlayerController _playerController;
    private CharacterController _characterController;
    
    private void Start()
    {
        _playerController = playerTarget.GetComponent<PlayerController>();
        _characterController = playerTarget.GetComponent<CharacterController>();
        
        // If no target is assigned, try to find the player automatically
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
            else
            {
                Debug.LogWarning("CameraFollow: No player target assigned and no GameObject with 'Player' tag found!");
            }
        }
        
        // Set initial camera position to target position with offset
        if (playerTarget != null)
        {
            transform.position = GetTargetPosition();
        }
    }
    
    private void FixedUpdate()
    {
        if (!playerTarget) return;
        
        FollowPlayer();
    }
    
    private void FollowPlayer()
    {
        // Calculate target position
        targetPosition = GetTargetPosition();
        
        // Apply look ahead if enabled
        if (useLookAhead)
        {
            ApplyLookAhead();
            targetPosition += lookAheadOffset;
        }
        
        // Apply bounds if enabled
        if (useBounds)
        {
            targetPosition = ApplyBounds(targetPosition);
        }
        
        // Smoothly move camera towards target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / followSpeed);
    }
    
    private Vector3 GetTargetPosition()
    {
        return playerTarget.position + offset;
    }
    
    private void ApplyLookAhead()
    {
        // Get player's input or movement direction for look ahead
        Vector3 playerMoveDirection = GetPlayerMoveDirection();
        
        // Calculate target look ahead offset
        Vector3 targetLookAhead = playerMoveDirection * lookAheadDistance;
        
        // Smoothly interpolate towards target look ahead
        lookAheadOffset = Vector3.Lerp(lookAheadOffset, targetLookAhead, lookAheadSpeed * Time.deltaTime);
    }
    
    private Vector3 GetPlayerMoveDirection()
    {
        if (_playerController)
        {
            return new Vector3(Mathf.Sign(playerTarget.localScale.x), 0f, 0f);
        }
        
        if (_characterController && _characterController.velocity.magnitude > 0.1f)
        {
            Vector3 velocity = _characterController.velocity;
            return new Vector3(Mathf.Sign(velocity.x), 0f, 0f).normalized;
        }
        
        return new Vector3(Mathf.Sign(playerTarget.localScale.x), 0f, 0f);
    }
    
    private Vector3 ApplyBounds(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minXBound, maxXBound);
        position.y = Mathf.Clamp(position.y, minYBound, maxYBound);
        return position;
    }
    
    // Public method to change target at runtime
    public void SetTarget(Transform newTarget)
    {
        playerTarget = newTarget;
    }
    
    // Public method to change offset at runtime
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    // Gizmos for visualizing bounds and look ahead
    private void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (minXBound + maxXBound) * 0.5f,
                (minYBound + maxYBound) * 0.5f,
                transform.position.z
            );
            Vector3 size = new Vector3(maxXBound - minXBound, maxYBound - minYBound, 0.1f);
            Gizmos.DrawWireCube(center, size);
        }
        
        if (useLookAhead && playerTarget != null)
        {
            Gizmos.color = Color.blue;
            Vector3 lookAheadPos = GetTargetPosition() + lookAheadOffset;
            Gizmos.DrawLine(GetTargetPosition(), lookAheadPos);
            Gizmos.DrawWireSphere(lookAheadPos, 0.3f);
        }
        
        // Draw camera follow area
        if (playerTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(GetTargetPosition(), new Vector3(2f, 2f, 0.1f));
        }
    }
}