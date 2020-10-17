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
    private bool locked;


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
        if (Input.GetAxisRaw("Horizontal") == 1)
        {
            attemptLock("Right");
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

        //float xIn = Input.GetAxisRaw("Horizontal");
        //if (xIn != 0f)
        //{
        //    transform.Rotate(new Vector3(0f, 0f, -xIn * Time.deltaTime * rotationSpeed));
        //}
        transform.Translate(velocity * Time.deltaTime);

        ScreenWrap();
        HeadCollision();
    }


    private void attemptLock(string direction)
    {
        Collider2D[] test = { null };
        Physics2D.OverlapCollider(rightLockCollider, dangerousMask, test);
        foreach (Collider2D x in test)
        {
            if (x != null)
            {
                Debug.Log("Huzzah");
            }

        }
    }

    private void HeadCollision()
    {
        Collider2D[] overLapping = {null, null, null, null, null};
        Physics2D.OverlapCollider(headCollider, dangerousMask, overLapping);
        foreach (Collider2D thing in overLapping)
        {
            if (thing != null)
            {
                dangerousCollidable x = thing.GetComponent<dangerousCollidable>();
                x.Hit();
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
        }
    }
}
