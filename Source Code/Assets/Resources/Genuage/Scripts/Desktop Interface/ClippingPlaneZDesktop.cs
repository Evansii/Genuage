/**Vincent Casamayou
    EMBL Heidelberg Ries Group
    23/06/2020
    Clipping Plane Desktop Z Axis Tool - Genuage
**/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class ClippingPlaneZDesktop : MonoBehaviour
{
    //Everything related to the Z Axis is in fact the Y Axis of the Object (UI is in 2D)
    public float zMinBound;
    public float zMaxBound;

    public float zMinBack;
    public float zMaxBack;

    public float zOffset;

    public float initXPos;

    public bool isMoving = false;
    GameObject background;
    GameObject topAnchor;
    GameObject botAnchor;
    CloudData data;

    Bounds backBounds;
    public Vector3 previousPos;



    void OnEnable()
    {
        data = CloudUpdater.instance.LoadCurrentStatus();

        background = this.gameObject.transform.parent.gameObject;

        botAnchor = this.gameObject.transform.GetChild(0).gameObject; 
        topAnchor = this.gameObject.transform.GetChild(1).gameObject;


        backBounds = background.GetComponent<BoxCollider2D>().bounds;

        //Max and Min are inverted due to measurement of depth (higher value = minimal value)
        zMinBack = backBounds.max.y;
        zMaxBack = backBounds.min.y;

        //Used to keep the object on the X Axis
        initXPos = this.gameObject.transform.position.x;
    }

    void Update()
    {
        zOffset = topAnchor.transform.position.y - transform.position.y;

        Vector3 currentPos = this.gameObject.transform.position;
        if(topAnchor.transform.position.y    >= zMinBack) 
        {
            transform.position = new Vector3(transform.position.x,zMinBack-zOffset, transform.position.z);
        }
        if(botAnchor.transform.position.y <= zMaxBack)
        {
            transform.position = new Vector3(transform.position.x,zMaxBack+zOffset, transform.position.z);
        }
        if((this.gameObject.transform.localPosition.x > initXPos) || (this.gameObject.transform.localPosition.x < initXPos))
        {
            transform.position = new Vector3(initXPos,transform.position.y, transform.position.z );
        }
        
        if(currentPos != previousPos)
        {
            isMoving = true;
        }
        if(isMoving)
        {
            ClipData();
            isMoving = false;
            previousPos = currentPos;
        }
    }

    public void ClipData()
    {
        zMinBound = topAnchor.transform.position.y;
        zMaxBound =  botAnchor.transform.position.y;

        
        data.globalMetaData.zMinThreshold = Mathf.Lerp(data.globalMetaData.zMin, data.globalMetaData.zMax,((zMinBound - zMinBack)/(zMaxBack - zMinBack)));
        data.globalMetaData.zMaxThreshold = Mathf.Lerp(data.globalMetaData.zMin, data.globalMetaData.zMax, ((zMaxBound - zMinBack)/(zMaxBack - zMinBack)));
        
        CloudUpdater.instance.ChangeThreshold();
        

    }
}
