using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    /// <summary>
    /// Set the object pool.
    /// </summary>
    /// <param name="pool"></param>
    void SetPool(ObjectPool pool);

    /// <summary>
    /// Get the object pool that this belongs to.
    /// </summary>
    /// <returns></returns>
    ObjectPool GetPool();

    /// <summary>
    /// Reset the object to ready state
    /// </summary>
    void Reset();

    /// <summary>
    /// Any needed steps to be done to deactivate the object to store in pool.
    /// </summary>
    void Deactivate();
}
