using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public float normalSpeed;
    public float fastSpeed;
    private float movementSpeed;
    public float movementTime;
    public float rotationSpeed;
    public float rotationSpeedMouse;
    public float zoomSpeed;
    public float zoomSpeedMouse;
    public float zoomTrajectoryQuotient; // This number is essentially the "a" in the y=ax+b formula for the cameras movement. The higher the number the lower the angle between the ground and the camera, when zoomed in.
    public float cameraMinimumY;
    public float cameraMaximumY;

    private Vector3 zoomAmount;
    private Vector3 zoomAmountMouse;
    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;
    public float speedMultiplier;

    private bool keyboardZoom; //used to stop camera instantly if zoom is performed with keyboard

    //private Vector3 dragStartPosition;
    //private Vector3 dragCurrentPosition;

    private Vector3 cameraMinimum;
    private Vector3 cameraMaximum;


    // Start is called before the first frame update
    void Start()
    {
        cameraMinimum = new Vector3(0f, cameraMinimumY, 0f);
        cameraMaximum = new Vector3(0f, cameraMaximumY, 0f);
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        zoomAmount = new Vector3(0, (-zoomSpeed/50), (zoomSpeed / (50*zoomTrajectoryQuotient)));
        zoomAmountMouse = new Vector3(0, (-zoomSpeedMouse / 5), (zoomSpeedMouse / (5*zoomTrajectoryQuotient)));

        keyboardZoom = false;
        speedMultiplier = 1f;

        zoomTrajectoryQuotient = Mathf.Abs(zoomTrajectoryQuotient);
        if (zoomTrajectoryQuotient == 0f) zoomTrajectoryQuotient = 1f;
        CalculateZoomBoundaryYZ();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeed();
        HandleMouseInput();
        HandleMovementInput();
        MoveCamera();
    }

    void HandleMouseInput()
    {

        //TRANSLATION

        /**
        if (Input.GetMouseButtonDown(0)) //Left Click
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(0)) //Left Click
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;

            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
        **/

        //ROTATION
        if (Input.GetMouseButtonDown(2)) //MMB
        {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2)) //MMB
        {
            rotateCurrentPosition = Input.mousePosition;

            Vector3 difference = rotateCurrentPosition - rotateStartPosition;
            
            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (difference.x / 5f));
        }

        //ZOOM
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmountMouse;
        }
    }

    void HandleMovementInput()
    {
        //DETERMINE SPEED
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            movementSpeed = (fastSpeed/100)*speedMultiplier;
        } 
        else
        {
            movementSpeed = (normalSpeed/100)*speedMultiplier;
        }


        //TRANSLATION
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }


        //ROTATION
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationSpeed/10);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationSpeed/10);
        }

        //ZOOM
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
            keyboardZoom = true;
        }

        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
            keyboardZoom = true;
        }
        if (keyboardZoom == true && !Input.GetKey(KeyCode.F) && !Input.GetKey(KeyCode.R))
        {
            newZoom = cameraTransform.localPosition;
            keyboardZoom = false;
        }

    }

    void CalculateZoomBoundaryYZ()
    {
        //y = -az+c
        float c, a;
        float yMax, zMax;
        float yMin, zMin;

        a = -zoomTrajectoryQuotient;
        c = (cameraTransform.localPosition.y - cameraTransform.localPosition.z*a);
        yMax = cameraMaximum.y;
        yMin = cameraMinimum.y;
        zMax = (yMax - c) / a;
        zMin = (yMin - c) / a;

        cameraMinimum.z = zMin;
        cameraMaximum.z = zMax;
    }

    void UpdateSpeed()
    {
        speedMultiplier = 1f + (cameraTransform.localPosition.y / (cameraMaximum.y - cameraMinimum.y))*3;
    }
    
    void MoveCamera()
    {
        //MOVE CAMERA
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);

        float targetY = newZoom.y;
        if (targetY > cameraMaximum.y) newZoom = cameraMaximum;
        else if (targetY < cameraMinimum.y) newZoom = cameraMinimum;
        cameraTransform.localPosition = Vector3.MoveTowards(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
        cameraTransform.LookAt(transform.position);
    }
}
