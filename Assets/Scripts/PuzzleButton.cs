using UnityEngine;
using UnityEngine.Events;

public class PuzzleButton : MonoBehaviour
{
    public UnityEvent buttonPressed;
    public UnityEvent buttonReleased;
    
    [Header("Button Prerequisites")]
    public Transform buttonObj;
    
    [Header("Button Settings")]
    public Vector3 buttonPressedOffset;
    public float animateDuration = 1f;

    [Header("Button Details || Debugging")]
    [SerializeField] private bool isPressed;
    [SerializeField] private bool isReleased;
    [SerializeField] private Vector3 buttonOriginalPosition;
    [SerializeField] private Vector3 buttonTargetPosition;
    [SerializeField] private float animateTimer;

    private void Start()
    {
        if (!buttonObj) return;
        
        buttonOriginalPosition = buttonObj.position;
        buttonTargetPosition = buttonObj.position + buttonPressedOffset;
    }

    private void AnimateButton()
    {
        if ((!isPressed && !isReleased) || (isPressed && isReleased)) return;
        
        animateTimer += Time.deltaTime;
        float t = animateTimer / animateDuration;
        
        if (t >= 1f)
        {
            buttonObj.position = isPressed ? buttonTargetPosition : buttonOriginalPosition;
            animateTimer = 0f;
            
            isPressed = false;
            isReleased = false;
        }
        else
        {
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            buttonObj.position = isPressed ? Vector3.Lerp(buttonOriginalPosition, buttonTargetPosition, smoothT) : Vector3.Lerp(buttonTargetPosition, buttonOriginalPosition, smoothT);
        }
    }

    private void FixedUpdate()
    {
        if (isPressed || isReleased)
        {
            AnimateButton();
        }
    }
    
    public void OnButtonPressed()
    {
        buttonPressed.Invoke();
        isPressed = true;
    }

    public void OnButtonReleased()
    {
        buttonReleased.Invoke();
        isReleased = true;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!buttonObj) return;
        
        Gizmos.color = Color.blue;

        var col = buttonObj.GetComponent<BoxCollider>();
        if (!col) return;
        
        Vector3 origin = buttonObj.TransformPoint(col.center);
        Vector3 worldSize = Vector3.Scale(col.size, buttonObj.lossyScale);
        Vector3 target = origin + buttonPressedOffset;

        Gizmos.DrawLine(origin, target);
        Gizmos.DrawWireCube(target, worldSize);
    }
}
