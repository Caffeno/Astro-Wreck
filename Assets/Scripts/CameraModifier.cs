using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModifier : MonoBehaviour
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private CinemachineVirtualCamera camera;

    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float minZoom = 5;



    private float currentZoom = 5f;
    private float playerTopSpeed;
    private float playerVelocity;
    private playerControler player;

    // Start is called before the first frame update
    void Start()
    {
        player = playerObject.GetComponent<playerControler>();
        playerTopSpeed = player.GetTopSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        playerVelocity = player.GetVelocity().magnitude;
        currentZoom = Mathf.MoveTowards(currentZoom, Mathf.Clamp((playerVelocity / playerTopSpeed) * maxZoom, minZoom, maxZoom), panSpeed * Time.deltaTime);
        camera.m_Lens.OrthographicSize = currentZoom;
    }
}
