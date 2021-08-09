using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSpawner : MonoBehaviour
{
    public GameObject Enemy;
    public GameObject Gate;
    public GameObject Container;
    public float height = 5;
    public float offset = 0;

    private enum State { Ready, Raising, Lowering};
    private State state;
    public Vector3 origin;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Ready;
        origin = Gate.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Raising)
        {
            if (offset < height)
            {
                offset += Time.deltaTime;
                Gate.transform.Translate(new Vector3(0, 0, Time.deltaTime));
            } else
            {
                state = State.Lowering;
            }
        }

        if (state == State.Lowering)
        {
            if (offset > 0)
            {
                offset -= Time.deltaTime;
                Gate.transform.Translate(new Vector3(0, 0, -Time.deltaTime));
            } else
            {
                state = State.Ready;
            }
        }
    }

    public void SpawnEnemies(int number)
    {
        if (state == State.Ready)
        {
            for (int i = 0; i < number; i++)
            {
                GameObject enemy = Instantiate(Enemy, Gate.transform.parent);
                Vector3 forward = Gate.transform.forward;
                enemy.transform.localPosition = new Vector3(0, 0, 0);
                enemy.transform.parent = Container.transform;
            }
            state = State.Raising;
        }
    }
}
