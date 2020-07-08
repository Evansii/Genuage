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

    public Text helpText_1;
    public Text helpText_2;
    public Text helpText_3;

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera");

        camMoveScript = camera.GetComponent<SMAPCameraMovement>();

        helpText_1.enabled = false;
        helpText_2.enabled = false;
        helpText_3.enabled = false;

        dragScript = camera.GetComponent<DragMouseOrbit>();

        moveToggle.onValueChanged.AddListener(UpdateMovementType);
    }

    public void UpdateMovementType(bool select)
    {
        if(select)
        {
            camMoveScript.enabled = true;
            dragScript.enabled = false;

            helpText_1.enabled = true;
            helpText_2.enabled = true;
            helpText_3.enabled = true;
        }
        else
        {
            camMoveScript.enabled = false;
            dragScript.enabled = true;

            helpText_1.enabled = false;
            helpText_2.enabled = false;
            helpText_3.enabled = false;
        }
    }

}
