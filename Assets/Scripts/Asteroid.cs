using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float edgeBuffer = 0.4f;


    private Vector3 screenBounds;
    private float rightBound;
    private float upperBound;
    private bool active = false;
    private Vector3 velocity;



    // Start is called before the first frame update
    void Start()
    {
        screenBounds = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        rightBound = screenBounds.x + edgeBuffer;
        upperBound = screenBounds.y + edgeBuffer;
        velocity = new Vector3(2, 3, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);
        if (!active)
        {
            CheckActive();
        }

        if (active)
        {
            ScreenWrap();
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
    private void CheckActive()
    {
        if (-upperBound < transform.position.y & transform.position.x < upperBound &
        -rightBound < transform.position.x & transform.position.x < rightBound)
        {
            active = true;
        }
    }
}
