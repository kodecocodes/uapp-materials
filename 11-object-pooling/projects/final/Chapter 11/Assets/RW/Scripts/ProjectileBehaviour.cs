using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour, IPoolable
{
    ObjectPool ProjectilePool;

    public async void DieAsync()
    {
        // 1. 
        await Task.Delay(2000);
        // 2. 
        // Destroy(gameObject);
        // 3.
        if (ProjectilePool)
        {
            ProjectilePool.Return(gameObject);
        }
    }

    public IEnumerator ExpireCoroutine()
    {
        // This coroutine provides an automatic delay to return to the pool.
        yield return new WaitForSeconds(2);
        if (ProjectilePool)
        {
            ProjectilePool.Return(gameObject);
        }
    }

    public void Reset()
    {
        // 1.
        gameObject.SetActive(true);
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        // 2.
        StartCoroutine(ExpireCoroutine());
    }

    public void Deactivate()
    {
        // 1.
        gameObject.SetActive(false);
    }

    public void SetPool(ObjectPool pool)
    {
        ProjectilePool = pool;
    }
}
