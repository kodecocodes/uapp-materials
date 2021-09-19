using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    // Needed to return a projectile back to it's pool.
    public ObjectPool ProjectilePool;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ExpireCoroutine()
    {
        yield return new WaitForSeconds(2);
        if (ProjectilePool)
        {
            ProjectilePool.Return(gameObject);
        }
    }
}
