using UnityEngine;

public class ParticleSystemsPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particleSystems;
    public void PlayAll()
    {
        foreach (var ps in _particleSystems)
        {
            if (ps != null && !ps.isPlaying)
            {
                ps.Play();
            }
        }
    }
}
