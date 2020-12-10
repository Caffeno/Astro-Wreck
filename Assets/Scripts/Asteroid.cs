using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : dangerousCollidable
{
    //[SerializeField] private Camera camera;
    [SerializeField] private float edgeBuffer = 0.4f;
    [SerializeField] private float mass = 100;
    [FMODUnity.EventRef]
    public string DestroyEvent = "";

    private Vector3 screenBounds;
    private float rightBound;
    private float upperBound;
    private Vector3 velocity = new Vector3(0, 0, 0);
    private bool locked = false;
    private Behaviour halo;
    private CircleCollider2D collider;
    [SerializeField] private Rigidbody2D coinPrefab;

    void Start()
    {
        halo = (Behaviour)GetComponent("Halo");
        collider = GetComponent<CircleCollider2D>();
        //screenBounds = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        //rightBound = screenBounds.x + edgeBuffer;
        //upperBound = screenBounds.y + edgeBuffer;
    }

    void Update()
    {
        if (!locked)
        {
            Move();
        }

        UpdateState();
    }

    private bool ScreenWrap()
    {

        float clampedx = Mathf.Clamp(transform.position.x, -rightBound, rightBound);
        float clampedy = Mathf.Clamp(transform.position.y, -upperBound, upperBound);
        bool changed = false;
        if (Mathf.Abs(clampedx) == rightBound)
        {
            clampedx *= -1f;
            changed = true;
        }
        if (Mathf.Abs(clampedy) == upperBound)
        {
            clampedy *= -1f;
            changed = true;
        }
        if (changed)
        {
            transform.position = new Vector3(clampedx, clampedy, 0);
        }
        return changed;
    }

    public override void Hit(Vector3 playerPosition, Vector3 playerVelocity)
    {
        FMODUnity.RuntimeManager.PlayOneShot(DestroyEvent, transform.position);
        Vector2 closestPointV2 = collider.ClosestPoint(playerPosition);
        Vector3 closestPoint = new Vector3(closestPointV2.x, closestPointV2.y, 0);
        Vector3 furthestPoint = closestPoint + 2 * (transform.position - closestPoint);
        Vector3 goldDirection = (transform.position - playerPosition).normalized;
        Rigidbody2D coinBody = Instantiate(coinPrefab, furthestPoint, Quaternion.identity);
        coinBody.velocity = goldDirection * Vector3.Dot(playerVelocity, goldDirection);
        GameObject.Destroy(gameObject);
    }

    public void SetStart(Vector3 direction)
    {
        velocity = direction * (Random.value * 5f + 5f);
    }

    public override void Freeze() 
    {
        locked = true;
        halo.enabled = true;
    }

    public override void UnFreeze()
    {
        locked = false;
        halo.enabled = false;
    }

    public override void ForceUpdate(Vector3 force)
    {
        transform.position += force / (mass * 2);
        velocity += force / (Time.deltaTime * mass);

    }

    public override void Move()
    {
        transform.position += velocity * Time.deltaTime;

    }

    public override float GetMass()
    {
        return mass;
    }


}
