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

    private Vector3 zoomAmount;
    private Vector3 zoomAmountMouse;
    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

    private bool keyboardZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;

    private readonly Vector3 cameraMinimum = new Vector3(0f, 2f, -6f);
    private readonly Vector3 cameraMaximum = new Vector3(0f, 30f, -20f);

    private static Vector3 cameraTargetPos;


    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        zoomAmount = new Vector3(0, (-zoomSpeed/50), (zoomSpeed / 100));
        zoomAmountMouse = new Vector3(0, (-zoomSpeedMouse / 5), (zoomSpeedMouse / 10));
        keyboardZoom = false;
    }

    // Update is called once per frame
    void Update()
    {
        handleMouseInput();
        handleMovementInput();
        moveCamera();
    }

    void handleMouseInput()
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

    void handleMovementInput()
    {
        //DETERMINE SPEED
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            movementSpeed = fastSpeed/100;
        } 
        else
        {
            movementSpeed = normalSpeed/100;
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

    void moveCamera()
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
