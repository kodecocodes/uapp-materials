using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour, IPoolable
{
    // Needed to return a projectile back to it's pool.
    ObjectPool ProjectilePool;

    public IEnumerator ExpireCoroutine()
    {
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
        gameObject.SetActive(true);
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        StartCoroutine(ExpireCoroutine());
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
