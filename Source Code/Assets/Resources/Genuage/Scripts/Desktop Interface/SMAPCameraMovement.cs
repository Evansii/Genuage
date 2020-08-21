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
    public float moveSpeed = 0.005f;
    public float rotationSpeed = 1f;
    public bool faster = true;

    
    
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

        if(Input.GetKey(KeyCode.U))
            this.gameObject.transform.Translate(Vector3.up*moveSpeed);

        if(Input.GetKey(KeyCode.J))
            this.gameObject.transform.Translate(Vector3.down*moveSpeed);    
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
    
    public void FastSpeed()
    {  
        if(!faster)
        {
            moveSpeed = 0.005f;
            faster = true;
        }
        else if(faster)
        {
            moveSpeed = 0.0005f;
            faster = false;
        }
    }


    

    // Update is called once per frame
    void FixedUpdate()
    {   
        if(!Input.GetKey(KeyCode.LeftControl))
        {
            UpdatePosition();
        }
        else
            UpdateRotation();

        if(Input.GetKey(KeyCode.R))
            ResetPosition();
        

        if(Input.GetKey(KeyCode.P))
        {
            FastSpeed();
        }

    }


}
