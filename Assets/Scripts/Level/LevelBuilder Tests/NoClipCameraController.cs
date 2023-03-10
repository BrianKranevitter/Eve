using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class NoClipCameraController : MonoBehaviour
{
    public float MoveSpeed;

    public float sensX;
    public float sensY;

    private float xRotation;
    private float yRotation;

    public bool detectingInput = true;
    public bool ableToMove = false;

    public Action<bool> onAbleToMove = delegate(bool b) {  };
    private void Awake()
    {
        xRotation = transform.rotation.eulerAngles.x;
        yRotation = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        if (!detectingInput) return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ableToMove)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                ableToMove = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                ableToMove = true;
            }

            onAbleToMove(ableToMove);
        }


        if (ableToMove)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            float originalMoveSpeed = MoveSpeed;
            float resultMoveSpeed = originalMoveSpeed;
            if (Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                resultMoveSpeed /= 2;
            }
            else if (Input.GetMouseButton((int) MouseButton.RightMouse))
            {
                resultMoveSpeed /= 4;
            }
            
            transform.position += transform.right * horizontalInput * Time.deltaTime * resultMoveSpeed;
            transform.position += transform.forward * verticalInput * Time.deltaTime * resultMoveSpeed;

            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += transform.up * Time.deltaTime * resultMoveSpeed;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.position += -transform.up * Time.deltaTime * resultMoveSpeed;
            }
        }
    }
}
