using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dangerousCollidable : MonoBehaviour
{
    public float activeAreaRadius;
    public float despawnRadius;
    public GameObject player;
    public Vector2 playAreaSize;
    public bool screenWrapper = true;

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


    public void CheckScreenwrap()
    {
        float yOffset = player.transform.position.y - transform.position.y;
        float xOffset = player.transform.position.x - transform.position.x;

        if (screenWrapper)
        {
            if (Mathf.Abs(xOffset) > playAreaSize.x)
            {
                transform.position = new Vector3(player.transform.position.x + playAreaSize.x * Mathf.Sign(xOffset), transform.position.y);
            }
            if (Mathf.Abs(yOffset) > playAreaSize.y)
            {
                transform.position = new Vector3(transform.position.x, player.transform.position.y + playAreaSize.y * Mathf.Sign(yOffset));
            }
        }
        else if(Mathf.Abs(xOffset) > playAreaSize.x || Mathf.Abs(yOffset) > playAreaSize.y)
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void UpdateState()
    {
        CheckScreenwrap();
    }

    public void SetPlayAreaSize(Vector2 size)
    {
        playAreaSize = size;
    }

    public abstract void Hit(Vector3 position, Vector3 velocity);
    public abstract void Freeze();
    public abstract void UnFreeze();
    public abstract void ForceUpdate(Vector3 force);
    public abstract void Move();

    public abstract float GetMass();
}
