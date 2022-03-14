using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    // 1.
    void Reset();

    // 2.
    void Deactivate();

    // 3.
    void SetPool(ObjectPool pool);
}
