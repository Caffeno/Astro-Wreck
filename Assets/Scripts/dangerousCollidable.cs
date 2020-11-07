using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dangerousCollidable : MonoBehaviour
{
    public float activeAreaRadius;
    public float despawnRadius;
    public GameObject player;
    public bool active;


    public void SetActiveRadius(float radius) 
    {
        activeAreaRadius = radius;
    }

    public void SetDespawnRadius(float radius)
    {
        despawnRadius = radius;
    }
    public void SetPlayer(GameObject playerObject)
    {
        //Debug.Log("Hello World");

        player = playerObject;
    }

    public void CheckActive()
    {
        active = Vector3.Distance(transform.position, player.transform.position) < activeAreaRadius ? true : false;
    }

    public void CheckDespawn()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > despawnRadius)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void UpdateState()
    {
        CheckActive();
        if (!active)
        {
            CheckDespawn();
        }
    }

    public abstract void Hit();
    public abstract void Freeze();
    public abstract void UnFreeze();
    public abstract bool ForceUpdate(Vector3 force);
    public abstract bool Move();

    public abstract float GetMass();
}
