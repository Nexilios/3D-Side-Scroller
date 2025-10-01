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
        targetPosition = GetTargetPosition();

        if (useLookAhead)
        {
            ApplyLookAhead();
            targetPosition += lookAheadOffset;
        }
        
        if (useBounds)
        {
            targetPosition = ApplyBounds(targetPosition);
        }
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / followSpeed);
    }
    
    private Vector3 GetTargetPosition()
    {
        return playerTarget.position + offset;
    }
    
    private void ApplyLookAhead()
    {
        Vector3 playerMoveDirection = GetPlayerMoveDirection();
        
        Vector3 targetLookAhead = playerMoveDirection * lookAheadDistance;
        
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
    
    public void SetTarget(Transform newTarget)
    {
        playerTarget = newTarget;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
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
        
        if (playerTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(GetTargetPosition(), new Vector3(2f, 2f, 0.1f));
        }
    }
}