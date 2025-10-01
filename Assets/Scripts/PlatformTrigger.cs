using UnityEngine;
using UnityEngine.Events;

public class PlatformTrigger : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter.Invoke();
        }
    }
}
