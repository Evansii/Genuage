using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMAPCameraMovement : MonoBehaviour
{
    public float moveSpeed = 0.01f;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
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


}
