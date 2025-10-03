using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private readonly int _bShakeHash = Animator.StringToHash("bShake");
    
    [Header("Platform Prerequisites")]
    public Transform platformVisual;
    public BoxCollider platformCollider;
    
    [Header("Platform Settings")]
    public bool canCollapse;
    public bool startDelayed;
    [Range(0, 10)]
    public float startDelayedDuration;
    [Range(0, 30)]
    public float collapseDuration;
    [Range(0, 10)]
    public float respawnTime;
    [Range(0, 10)]
    public float delayDuration;
    [Range(0, 30)]
    public float moveDuration = 3f;
    public Vector3 targetPositionOffset;
    
    
    [Header("Platform Details | Debugging")]
    [SerializeField] private bool canMove;
    [SerializeField] private float moveTimer;
    [SerializeField] private float delayTimer;
    [SerializeField] private float collapseTimer;
    [SerializeField] private float respawnTimer;
    [SerializeField] private bool isMovingToTarget = true;
    [SerializeField] private bool isDelayed;
    [SerializeField] private bool isCollapsing;
    [SerializeField] private bool hasCollapsed;
    
    private Animator _visualAnimator;
    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    
    private void Awake()
    {
        if (!platformVisual) return;
        
        if (!_visualAnimator)
        {
            _visualAnimator = platformVisual.GetComponent<Animator>();
        }

        if (!platformCollider)
        {
            platformCollider = gameObject.GetComponent<BoxCollider>();
        }
    }

    private void OnEnable()
    {
        if (!hasCollapsed) return;
        
        RespawnPlatform();
        hasCollapsed = false;
    }
    
    private void Start()
    {
        _originalPosition = transform.position;
        _targetPosition = _originalPosition + targetPositionOffset;
        
        isMovingToTarget = true;
        if (startDelayed) isDelayed = true;
    }

    private void Update()
    {
        if (hasCollapsed)
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer >= respawnTime)
            {
                RespawnPlatform();
            }
            
            return;
        }
        
        if (!isCollapsing) return;
        
        collapseTimer += Time.deltaTime;
        
        if (collapseTimer >= collapseDuration)
        {
            DestroyPlatform();
        }
    }

    private void FixedUpdate()
    {
        if (hasCollapsed) return;
        
        MovePlatform();
    }

    private void MovePlatform()
    {
        if (canMove)
        {
            if (isDelayed)
            {
                delayTimer += Time.deltaTime;

                if (delayTimer >= (startDelayed ? startDelayedDuration : delayDuration))
                {
                    delayTimer = 0f;
                    if (startDelayed)
                    {
                        startDelayed = false;
                    }
                    isDelayed = false;
                }
            }
            else
            {
                moveTimer += Time.deltaTime;
                float t = moveTimer / moveDuration;
                
                if (t >= 1f)
                {
                    transform.position = isMovingToTarget ? _targetPosition : _originalPosition;
                    
                    if (delayDuration > 0)
                    {
                        isDelayed = true;
                        delayTimer = 0f;
                    }
                    
                    moveTimer = 0f;
                    isMovingToTarget = !isMovingToTarget;
                }
                else
                {
                    float smoothT = Mathf.SmoothStep(0f, 1f, t);

                    transform.position = isMovingToTarget ? Vector3.Lerp(_originalPosition, _targetPosition, smoothT) : Vector3.Lerp(_targetPosition, _originalPosition, smoothT);
                }
            }
        }
    }
    private void ResetPlatformMovementTimer()
    {
        moveTimer = 0f;
        delayTimer = 0f;
        isDelayed = false;
    }

    private void RespawnPlatform()
    {
        hasCollapsed = false;
        
        transform.position =  _originalPosition;
        
        respawnTimer = 0f;
        platformVisual.gameObject.SetActive(true);
        platformCollider.enabled = true;
    }

    private void DestroyPlatform()
    {
        isCollapsing = false;
        collapseTimer = 0f;
        
        _visualAnimator.SetBool(_bShakeHash, false);
        platformVisual.gameObject.SetActive(false);
        platformCollider.enabled = false;
        
        ResetPlatformMovementTimer();
        
        hasCollapsed = true;
    }

    public void StartPlatformCollapse()
    {
        if (!canCollapse) return;

        isCollapsing = true;
        _visualAnimator.SetBool(_bShakeHash, true);
    }

    public void EnablePlatformMovement()
    {
        canMove = true;
    }

    public void DisablePlatformMovement()
    {
        canMove = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize platform movement range
        Gizmos.color = Color.green;

        if (!platformCollider) return;
        Vector3 origin = transform.position + platformCollider.center;
        Vector3 target = origin + targetPositionOffset;

        Gizmos.DrawLine(origin, target);
        Gizmos.DrawWireCube(target, platformCollider.size);
    }
}
