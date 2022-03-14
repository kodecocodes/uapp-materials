using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour, IPoolable
{
    ObjectPool ProjectilePool;

    public void SetPool(ObjectPool pool)
    {
        ProjectilePool = pool;
    }

    public void Reset()
    {
        // TODO: Reset needs to reactivate and clear the velocity of the particle.
    }

    public void Deactivate()
    {
        // TODO: Deactivate the gameObject when returning to a pool.
    }
}
