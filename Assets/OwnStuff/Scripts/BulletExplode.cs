using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Netcode;
using System.Threading;

public class BulletExplode : MonoBehaviour
{
    [SerializeField]
    private BulletExplodeSync bulletNetworkSync;
    //[SerializeField]
    //private ParticleSystem particleSystem;

    private bool exploded = false;
    private float timer = 0;
    private float lifeTime = 1f;

    private void Update()
    {
        if (!exploded && transform.forward != Vector3.zero)
        {
            Explode();
            //Explode(bulletNetworkSync.GetOrigin(), bulletNetworkSync.GetDirection());
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

    private void Explode(Vector3 origin, Vector3 direction)
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.transform.position = origin;
        particleSystem.transform.rotation = Quaternion.LookRotation(direction);
        particleSystem.Play();
    }
}
