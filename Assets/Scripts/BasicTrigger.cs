using UnityEngine;
using UnityEngine.Events;

public class BasicTrigger : MonoBehaviour
{
    public bool triggerOnPlayer;
    public bool triggerOnBall;
    
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;
    public UnityEvent onBallEnter;
    public UnityEvent onBallExit;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && triggerOnPlayer)
        {
            onPlayerEnter.Invoke();
        }

        if (other.CompareTag("Ball") && triggerOnBall)
        {
            onBallEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && triggerOnPlayer)
        {
            onPlayerExit.Invoke();
        }

        if (other.CompareTag("Ball") && triggerOnBall)
        {
            onBallExit.Invoke();
        }
    }
}
