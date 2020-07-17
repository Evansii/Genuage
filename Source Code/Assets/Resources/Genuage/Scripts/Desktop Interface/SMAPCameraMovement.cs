/**
Vincent Casamayou
RIES GROUP
SMAP Camera Movements for Desktop
06/07/2020
**/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMAPCameraMovement : MonoBehaviour
{
    public float moveSpeed = 0.01f;
    public float rotationSpeed = 1f;

    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void UpdatePosition()
    {
         //Change positions with Arrow Keys
        if(Input.GetKey(KeyCode.UpArrow))
            this.gameObject.transform.Translate(Vector3.forward*moveSpeed);

        if(Input.GetKey(KeyCode.DownArrow))
            this.gameObject.transform.Translate(Vector3.back*moveSpeed);

        if(Input.GetKey(KeyCode.LeftArrow))
            this.gameObject.transform.Translate(Vector3.left*moveSpeed);
        
        if(Input.GetKey(KeyCode.RightArrow))
            this.gameObject.transform.Translate(Vector3.right*moveSpeed);
    }

    public void UpdateRotation()
    {
        if(Input.GetKey(KeyCode.UpArrow) &&Input.GetKey(KeyCode.LeftControl))
        {
            this.gameObject.transform.Rotate(-Vector3.right*rotationSpeed);
        }
        if(Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftControl))
        {
            this.gameObject.transform.Rotate(Vector3.right*rotationSpeed);
        }
        if(Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.LeftControl))
        {
            this.gameObject.transform.Rotate(-Vector3.up*rotationSpeed);
        }
        if(Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftControl))
        {
            this.gameObject.transform.Rotate(Vector3.up*rotationSpeed);
        }
    }

    public void ResetPosition()
    {
        this.gameObject.transform.position = new Vector3(0,0,-2);
        this.gameObject.transform.localEulerAngles = Vector3.zero;
    }


    // Update is called once per frame
    void Update()
    {   
        if(!Input.GetKey(KeyCode.LeftControl))
        {
            UpdatePosition();
        }
        else
            UpdateRotation();

        if(Input.GetKey(KeyCode.R))
            ResetPosition();
        
    }


}
