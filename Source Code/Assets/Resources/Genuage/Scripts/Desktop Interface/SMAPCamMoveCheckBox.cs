using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Display;

public class SMAPCamMoveCheckBox : MonoBehaviour
{

    GameObject camera;
    
    SMAPCameraMovement camMoveScript;
    DragMouseOrbit dragScript;

    public Toggle moveToggle;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");

        camMoveScript = camera.GetComponent<SMAPCameraMovement>();
        camMoveScript.enabled = false;

        dragScript = camera.GetComponent<DragMouseOrbit>();

        moveToggle.onValueChanged.AddListener(UpdateMovementType);
    }

    public void UpdateMovementType(bool select)
    {
        if(select)
        {
            camMoveScript.enabled = true;
            dragScript.enabled = false;
        }
        else
        {
            camMoveScript.enabled = false;
            dragScript.enabled = true;
        }
    }

}
