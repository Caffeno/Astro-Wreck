using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class playerControler : MonoBehaviour
{
    [SerializeField] private float topSpeed = 10f;
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float drag = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float edgeBuffer = 0.2f;
    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask dangerousLayers;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject weakSpot;
    [SerializeField] private GameObject rightLockScan;
    [SerializeField] private GameObject leftLockScan;


    private Vector3 velocity;
    private Vector3 screenBounds;
    private float rightBound;
    private float upperBound;
    private CapsuleCollider2D headCollider;
    private CapsuleCollider2D weakSpotCollider;
    private EdgeCollider2D rightLockCollider;
    private EdgeCollider2D leftLockCollider;
    private ContactFilter2D dangerousMask;
    private bool lockedOn = false;
    private float lockInput = 0;
    private dangerousCollidable lockTarget = null;
    private string lockDirection = null;
    private float lockRealeased = 0f;


    // Start is called before the first frame update
    void Start()
    {
        dangerousMask.useTriggers = false;
        dangerousMask.SetLayerMask(dangerousLayers);
        dangerousMask.useLayerMask = true;
        screenBounds = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rightBound = screenBounds.x + edgeBuffer;
        upperBound = screenBounds.y + edgeBuffer;
        headCollider = head.GetComponent<CapsuleCollider2D>();
        weakSpotCollider = weakSpot.GetComponent<CapsuleCollider2D>();
        rightLockCollider = rightLockScan.GetComponent<EdgeCollider2D>();
        leftLockCollider = leftLockScan.GetComponent<EdgeCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        lockInput = Input.GetAxisRaw("Horizontal");
        lockDirection = null;
        if (lockInput != 0) {lockDirection = lockInput == 1 ? "Right" : "Left";}

        if (lockInput != 0 && !lockedOn && Time.realtimeSinceStartup - lockRealeased > 0.1f)
        {
            attemptLock(lockDirection);
        }


        if (lockInput != 0 && lockedOn)
        {
            Vector3 vectorToTarget = lockTarget.transform.position - transform.position;
            Vector3 orientation = lockDirection == "Right" ? transform.right : - transform.right;
            float angleOffset = Vector3.SignedAngle(orientation, vectorToTarget, Vector3.forward);
            transform.Rotate(new Vector3(0f, 0f, Mathf.Sign(angleOffset) * Time.deltaTime * rotationSpeed));
        }

        if (lockInput == 0 && lockedOn && lockTarget != null)
        {
            UnLock();
        }

        float yIn = Input.GetAxisRaw("Vertical");
        if (yIn > 0)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, topSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.y = Mathf.MoveTowards(velocity.y, 0, drag * Time.deltaTime);
        }

        transform.Translate(velocity * Time.deltaTime);

        HeadCollision();
        ScreenWrap();
    }

    private void attemptLock(string direction)
    {
        //TODO: Find nearest Objectin area
        Collider2D[] lockTargetColliders = { null, null, null };
        EdgeCollider2D colliderLockDirection = direction == "Right" ? rightLockCollider : leftLockCollider;
        Physics2D.OverlapCollider(colliderLockDirection, dangerousMask, lockTargetColliders);
        foreach (Collider2D collider in lockTargetColliders)
        {
            if (collider != null)
            {
                Debug.Log("Huzzah");
                dangerousCollidable target = collider.GetComponent<dangerousCollidable>();
                if (target != null) 
                {
                    Debug.Log("Target Found");
                    lockedOn = true;
                    target.Freeze();
                    lockTarget = target;
                    return;
                }
            }
        }
    }

    private void HeadCollision()
    {
        Collider2D[] overLapping = {null, null, null, null, null};
        Physics2D.OverlapCollider(headCollider, dangerousMask, overLapping);
        foreach (Collider2D obsticle in overLapping)
        {
            if (obsticle != null)
            {
                dangerousCollidable crashedObject = obsticle.GetComponent<dangerousCollidable>();
                if (crashedObject == lockTarget) { UnLock(); }
                crashedObject.Hit();
            }
        }
    }

    private void ScreenWrap()
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
            Debug.Log("Hello World");

            transform.position = new Vector3(clampedx, clampedy, 0);
            if (lockedOn) { UnLock(); }
        }

    }

    private void UnLock()
    {
        lockedOn = false;
        lockTarget.UnFreeze();
        lockTarget = null;
        lockRealeased = Time.realtimeSinceStartup;
    }
}
