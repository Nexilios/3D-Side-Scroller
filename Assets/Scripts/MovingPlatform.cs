using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private readonly int _bShakeHash = Animator.StringToHash("bShake");
    
    public Transform platformVisual;
    public BoxCollider platformCollider;
    public float timeToCollapse;
    public float respawnTime;
    [Range(0, 10)]
    public float delayDuration;
    public Vector3 lastPosition;
    [Range(0, 30)]
    public float moveDuration = 3f;
    
    [SerializeField]
    private bool isMoving;
    
    private Animator _visualAnimator;
    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private float _moveTimer;
    private float _delayTimer;
    private bool _isDelayed;
    private bool _isDestroyed;
    
    private void Awake()
    {
        if (!_visualAnimator && platformVisual)
        {
            _visualAnimator = platformVisual.GetComponent<Animator>();
        }

        if (platformCollider)
        {
            platformCollider = gameObject.GetComponent<BoxCollider>();
        }
    }

    private void OnEnable()
    {
        if (!_isDestroyed) return;
        
        RespawnPlatform();
        _isDestroyed = false;
    }
    
    void Start()
    {
        _originalPosition = transform.position;
        _targetPosition = _originalPosition + lastPosition;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (_isDelayed)
            {
                _delayTimer += Time.fixedDeltaTime;

                if (_delayTimer >= delayDuration)
                {
                    _isDelayed = false;
                    _delayTimer = 0f;
                }
            }
            else
            {
                _moveTimer += Time.fixedDeltaTime;
                float t = Mathf.PingPong(_moveTimer / moveDuration, 1f);
                t = Mathf.SmoothStep(0f, 1f, t);
                transform.position = Vector3.Lerp(_originalPosition, _targetPosition, t);
                
                if (delayDuration > 0 && t >= 1f || transform.position == _originalPosition)
                {
                    _isDelayed = true;
                    _delayTimer = 0f;
                }
            }
        }
        else
        {
            _moveTimer = 0f;
            _delayTimer = 0f;
            _isDelayed = false;
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
        _isDestroyed = true;
        
    }

    public void StartPlatformCollapse()
    {
        
    }
    
    private void OnDrawGizmosSelected()
    {
        
        // Visualize platform movement range
        Gizmos.color = Color.green;

        if (!platformCollider) return;
        Vector3 origin = Application.isPlaying ? _originalPosition : transform.position + platformCollider.center;
        Vector3 target = origin + lastPosition;

        Gizmos.DrawLine(origin, target);
        Gizmos.DrawWireCube(target, platformCollider.size);
    }
}
