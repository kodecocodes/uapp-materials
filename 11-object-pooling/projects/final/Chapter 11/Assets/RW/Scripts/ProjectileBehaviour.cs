using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour, IPoolable
{
    ObjectPool ProjectilePool;

    public IEnumerator ExpireCoroutine()
    {
        // This coroutine provides an automatic delay to return to the pool.
        yield return new WaitForSeconds(2);
        if (ProjectilePool)
        {
            ProjectilePool.Return(gameObject);
        }
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
        // Reset needs to reactivate and clear the velocity of the particle.
        gameObject.SetActive(true);
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        // Restart a counter to return back to the pool.
        StartCoroutine(ExpireCoroutine());
    }

    public void Deactivate()
    {
        // Deactivate the gameObject when returning to a pool.
        gameObject.SetActive(false);
    }
}
