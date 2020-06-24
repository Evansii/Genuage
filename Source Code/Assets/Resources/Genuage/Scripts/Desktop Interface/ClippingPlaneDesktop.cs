/**Vincent Casamayou
    EMBL Heidelberg Ries Group
    23/06/2020
    Clipping Plane Desktop Tool - Genuage
**/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class ClippingPlaneDesktop : MonoBehaviour
{
    public float xMinBound;
    public float xMaxBound;
    public float yMinBound;
    public float yMaxBound;

    public float xMinBack;
    public float xMaxBack;
    public float yMinBack;
    public float yMaxBack;

    public float xOffset;
    public float yOffset;

    public bool isMoving = false;
    
    GameObject background;

    public GameObject topRightAnchor;
    public GameObject botLeftAnchor;
    CloudData data;

    Bounds backBounds;
    public Vector3 previousPos;




    // Start is called before the first frame update
    void OnEnable()
    {
        data = CloudUpdater.instance.LoadCurrentStatus();

        background = this.gameObject.transform.parent.gameObject;

        //topRightAnchor = this.gameObject.transform.GetChild(0).gameObject;
        //botLeftAnchor = this.gameObject.transform.GetChild(3).gameObject; 


        backBounds = background.GetComponent<BoxCollider2D>().bounds;

        xMinBack = backBounds.min.x;
        xMaxBack = backBounds.max.x;
        yMinBack = backBounds.min.y;
        yMaxBack = backBounds.max.y;


    }

    // Update is called once per frame
    void Update()
    {
        xOffset = topRightAnchor.transform.position.x - transform.position.x;
        yOffset = topRightAnchor.transform.position.y - transform.position.y; 
        Vector3 currentPos = this.gameObject.transform.position;
        if(botLeftAnchor.transform.position.x <= xMinBack) 
        {
            this.gameObject.transform.position = new Vector3(xMinBack+xOffset,transform.position.y, transform.position.z);
        }
        if(botLeftAnchor.transform.position.y <= yMinBack)
        {
            this.gameObject.transform.position = new Vector3(transform.position.x, yMinBack+yOffset, transform.position.z);   
        } 
        if(topRightAnchor.transform.position.x >= xMaxBack)
        {
            this.gameObject.transform.position = new Vector3(xMaxBack-xOffset, transform.position.y, transform.position.z);
        }
        if(topRightAnchor.transform.position.y >= yMaxBack)
        {
            this.gameObject.transform.position = new Vector3(transform.position.x, yMaxBack-yOffset, transform.position.z);
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
        xMinBound = botLeftAnchor.transform.position.x;
        xMaxBound =  topRightAnchor.transform.position.x;
        yMinBound =  botLeftAnchor.transform.position.y;
        yMaxBound =  topRightAnchor.transform.position.y;

        
        data.globalMetaData.xMinThreshold = Mathf.Lerp(data.globalMetaData.xMin, data.globalMetaData.xMax,((xMinBound - xMinBack)/(xMaxBack - xMinBack)));
        data.globalMetaData.xMaxThreshold = Mathf.Lerp(data.globalMetaData.xMin, data.globalMetaData.xMax, ((xMaxBound - xMinBack)/(xMaxBack - xMinBack)));
        data.globalMetaData.yMinThreshold = Mathf.Lerp(data.globalMetaData.yMin, data.globalMetaData.yMax,((yMinBound - yMinBack)/(yMaxBack - yMinBack)));       
        data.globalMetaData.yMaxThreshold = Mathf.Lerp(data.globalMetaData.yMin, data.globalMetaData.yMax,((yMaxBound - yMinBack)/(yMaxBack - yMinBack)));

        CloudUpdater.instance.ChangeThreshold();
        

    }
}
