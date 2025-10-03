using UnityEngine;
using UnityEngine.Audio;

public class AmbienceSound : MonoBehaviour
{
    [Header("Ambience Settings")]
    public AudioResource[] audioResources;
    [Range(0, 100)]
    public float playChance = 0.2f;
    
    private AudioSource _audioSource;
    private float _chanceCooldownTimer;
    private const float ChanceCooldown = 1f;
    private AudioResource _currentAudioResource;
    private bool _isCooldown;

    private void Awake()
    {
        if (!_audioSource)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!_audioSource) return;
        if (_isCooldown)
        {
            _chanceCooldownTimer += Time.deltaTime;

            if (_chanceCooldownTimer >= ChanceCooldown)
            {
                _chanceCooldownTimer = 0;
                _isCooldown = false;
            }
            
            return;
        }

        if (_audioSource.isPlaying)
        {
            _isCooldown = true;
        }
        
        if (audioResources is { Length: > 0 })
        {
            if (Random.Range(0, 100) <= playChance)
            {
                _currentAudioResource = GetRandomElement(audioResources);
                _audioSource.resource = _currentAudioResource;
                _audioSource.Play();
                _isCooldown = true;
            }
        }
    }
    
    private T GetRandomElement<T>(T[] array)
    {
        if (array.Length == 0)
        {
            return default(T);
        }

        int randomIndex = Random.Range(0, array.Length);
        return array[randomIndex];
    }
    
    private int GetRandomIndex(int arrayLength)
    {
        return Random.Range(0, arrayLength);
    }
}
