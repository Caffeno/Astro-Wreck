using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEditorInternal;
using UnityEngine;

public class playerControler : MonoBehaviour
{
    [SerializeField] private float topSpeed = 10f;
    [SerializeField] private float thrustForce = 70f;
    [SerializeField] private float drag = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float edgeBuffer = 0.2f;
    [SerializeField] private float mass;

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
    private float timeSincePrint;
    private float targetRotation;
    private float currentRotation;
    private float newEulerAngle;
    private float maxLength = 0;
    private Vector3 zeroV = new Vector3(0, 0, 0);
    private bool angleLocked = false;


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
        float yIn = Input.GetAxisRaw("Vertical");
        if (yIn != 0)
        {
            velocity += transform.up * thrustForce * Time.deltaTime * yIn / mass;
        }

        velocity = Vector3.MoveTowards(velocity, zeroV, 0.25f * (velocity.magnitude) * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;

        HandleLock();

        HeadCollision();

        ScreenWrap();
    }

    private void HandleLock()
    {
        lockInput = Input.GetAxisRaw("Horizontal");
        lockDirection = null;
        if (lockInput != 0) { lockDirection = lockInput == 1 ? "Right" : "Left"; }

        if (lockInput != 0 && !lockedOn && Time.realtimeSinceStartup - lockRealeased > 0.1f)
        {
            attemptLock(lockDirection);
        }

        if (lockInput != 0 && lockedOn)
        {

            bool checkTeather = lockTarget.Move();
            if (checkTeather)
            {
                UnLock();
                return;
            }
            float seperation = Vector3.Distance(transform.position, lockTarget.transform.position);
            Vector3 vectorToTarget = (lockTarget.transform.position - transform.position).normalized;

            if (seperation > maxLength)
            {
                Debug.Log(seperation);
                Debug.Log(Mathf.Sqrt(Mathf.Pow(transform.position.x - lockTarget.transform.position.x, 2) + Mathf.Pow(transform.position.y - lockTarget.transform.position.y, 2)));

                float targetMass = lockTarget.GetMass();
                float totalForce = (2 * mass * targetMass * (seperation - maxLength) / (mass + targetMass));
                totalForce = totalForce > 1 ? 1 : totalForce;
                transform.position += vectorToTarget * totalForce / (2 * mass);
                velocity += vectorToTarget * totalForce / (Time.deltaTime * mass);
                lockTarget.ForceUpdate(-vectorToTarget * totalForce);
                seperation = Vector3.Distance(transform.position, lockTarget.transform.position);
                Debug.Log(totalForce);
                if (seperation > maxLength)
                {
                    maxLength = seperation;
                }
                HandleRotation(lockDirection);

            }
            else
            {
                maxLength = seperation;
            }

        }

        if (lockInput == 0 && lockedOn && lockTarget != null)
        {
            UnLock();
        }
    }

    private void HandleRotation(string lockSide)
    {
        float targetVx;
        float targetVy;
        Vector3 vectorToTarget = (lockTarget.transform.position - transform.position);
        if (lockSide == "Right") 
        { 
            targetVx = -vectorToTarget.y;
            targetVy = vectorToTarget.x;
        }
        else
        {
            targetVx = vectorToTarget.y;
            targetVy = -vectorToTarget.x;
        }
        {

        }
        if (targetVy == 0)
        {
            if (targetVx != 0)
            {
                targetRotation = targetVx > 0f ? 270f : 90f;
            }
        }
        else
        {
            targetRotation = Mathf.Atan(targetVx / targetVy) * 180 / (Mathf.PI);
            if (targetVy > 0)
            {
                targetRotation *= -1;
            }
            else
            {
                targetRotation = targetVx > 0 ? -90 - (targetRotation + 90) : 180 - (targetRotation);
            }
        }
        if (!angleLocked)
        {
            currentRotation = transform.eulerAngles.z;
            if (Mathf.Abs(targetRotation - currentRotation) > 180)
            {
                currentRotation -= 360;
            }
            //Debug.Log("target current");
            //Debug.Log(targetRotation);

            //Debug.Log(currentRotation);
            newEulerAngle = Mathf.MoveTowardsAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime * lockTarget.GetMass() / mass);
            transform.eulerAngles = new Vector3(0, 0, newEulerAngle);
            if (newEulerAngle == targetRotation && lockTarget.GetMass() >= mass)
            {
                angleLocked = true;
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, targetRotation);
        }
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
                dangerousCollidable target = collider.GetComponent<dangerousCollidable>();
                if (target != null) 
                {
                    lockedOn = true;
                    target.Freeze();
                    lockTarget = target;
                    maxLength = Vector3.Distance(transform.position, target.transform.position);
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
        angleLocked = false;
    }
}
