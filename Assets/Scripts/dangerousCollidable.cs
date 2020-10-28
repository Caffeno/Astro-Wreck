using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dangerousCollidable : MonoBehaviour
{
    public abstract void Hit();
    public abstract void Freeze();
    public abstract void UnFreeze();
    public abstract bool ForceUpdate(Vector3 force);
    public abstract bool Move();

    public abstract float GetMass();
}
