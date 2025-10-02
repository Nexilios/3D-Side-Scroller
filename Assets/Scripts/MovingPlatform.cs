using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private readonly int _bShakeHash = Animator.StringToHash("bShake");
    
    [Header("Platform Prerequisites")]
    public Transform platformVisual;
    public BoxCollider platformCollider;
    
    [Header("Platform Settings")]
    public bool canCollapse;
    [Range(0, 30)]
    public float timeToCollapse;
    [Range(0, 10)]
    public float respawnTime;
    [Range(0, 10)]
    public float delayDuration;
    [Range(0, 30)]
    public float moveDuration = 3f;
    public Vector3 targetPositionOffset;
    
    [Header("Platform Details | Debugging")]
    [SerializeField] private bool isMoving;
    [SerializeField] private float moveTimer;
    [SerializeField] private float delayTimer;
    [SerializeField] private bool isDelayed;
    [SerializeField] private bool isDestroyed;
    
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
        if (!isDestroyed) return;
        
        RespawnPlatform();
        isDestroyed = false;
    }
    
    void Start()
    {
        _originalPosition = transform.position;
        _targetPosition = _originalPosition + targetPositionOffset;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (isDelayed)
            {
                delayTimer += Time.fixedDeltaTime;

                if (delayTimer >= delayDuration)
                {
                    isDelayed = false;
                    delayTimer = 0f;
                }
            }
            else
            {
                moveTimer += Time.fixedDeltaTime;
                float t = Mathf.PingPong(moveTimer / moveDuration, 1f);
                t = Mathf.SmoothStep(0f, 1f, t);
                transform.position = Vector3.Lerp(_originalPosition, _targetPosition, t);
                
                if (delayDuration > 0 && t >= 1f || transform.position == _originalPosition)
                {
                    isDelayed = true;
                    delayTimer = 0f;
                }
            }
        }
        else
        {
            moveTimer = 0f;
            delayTimer = 0f;
            isDelayed = false;
        }
    }

    private void RespawnPlatform()
    {
        transform.position =  _originalPosition;
        if (_visualAnimator)
        {
            _visualAnimator.SetBool(_bShakeHash, false);
        }
    }

    public void SetPlatformMode(bool bEnable)
    {
        isMoving = bEnable;
    }

    private void DestroyPlatform()
    {
        isDestroyed = true;
        
    }

    public void StartPlatformCollapse()
    {
        if (!canCollapse) return;
        
        _visualAnimator.SetBool(_bShakeHash, true);
    }
    
    private void OnDrawGizmosSelected()
    {
        
        // Visualize platform movement range
        Gizmos.color = Color.green;

        if (!platformCollider) return;
        Vector3 origin = Application.isPlaying ? _originalPosition : transform.position + platformCollider.center;
        Vector3 target = origin + targetPositionOffset;

        Gizmos.DrawLine(origin, target);
        Gizmos.DrawWireCube(target, platformCollider.size);
    }
}
