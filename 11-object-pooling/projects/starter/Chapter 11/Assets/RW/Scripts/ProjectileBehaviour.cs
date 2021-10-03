using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour, IPoolable
{
    ObjectPool ProjectilePool;

    public IEnumerator ExpireCoroutine()
    {
        // TODO: this coroutine provides an automatic delay to return to the pool.
        yield return new WaitForSeconds(2);
    }

    public void SetPool(ObjectPool pool)
    {
        ProjectilePool = pool;
    }

    public ObjectPool GetPool()
    {
        return ProjectilePool;
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
