using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Netcode;
using System.Threading;

public class BulletExplode : MonoBehaviour
{
    [SerializeField]
    private BulletExplodeSync bulletNetworkSync;

    private bool exploded = false;
    private float timer = 0;
    private float lifeTime = 1f;

    private void Update()
    {
        // check if this bullet has exploded yet or not, if it hasn't, then explode
        if (!exploded)
        {
            Explode();
            exploded = true;
        }

        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void Explode()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Play();
    }
}
