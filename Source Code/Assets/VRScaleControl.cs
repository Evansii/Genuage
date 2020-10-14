
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Display;
using Data;
using IO;


namespace VR_Interaction
{
    public class VRScaleControl : MonoBehaviour
    {
        protected VRTK_ControllerEvents _controller;

        public bool menuActivated = false;
        public bool channel2 = false;
        public bool scaleLock = false;
        
        CloudData data;

        public GameObject cloudPoint;
        public GameObject box;

        public Vector3 boxScaling;

        public float scaleFactor = 0.01f;
        public float sizeFactor = 0.02f;
        public float brightFactor = 0.02f;

        public float initialBright;
        public float initialSize;

        public void OnEnable() 
        {
            data = CloudUpdater.instance.LoadCurrentStatus();
            cloudPoint = GameObject.Find("CloudPoint");
            box = GameObject.Find("Box");

            boxScaling = box.transform.localScale;

            _controller = GetComponent<VRTK_ControllerEvents>();
            _controller.ButtonOnePressed += OnButtonOnePressed;
            _controller.TouchpadAxisChanged += OnTouchpadAxisChanged;
            _controller.TriggerPressed += OnTriggerPressed;
            _controller.TriggerReleased += OnTriggerReleased;
            initialBright = data.globalMetaData.point_brightness;
            initialSize = data.globalMetaData.point_size;
             

        }

        public VRScaleControl(VRTK_ControllerEvents controller)
        {
            _controller = controller;
            
        }

        public void OnTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {  
            Debug.Log("changed");
            if(menuActivated == false)
            {
                if (e.touchpadAxis.y >= 0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    cloudPoint.transform.parent.localScale = cloudPoint.transform.parent.localScale  *(1+scaleFactor);
                    CloudUpdater.instance.ChangePointSize(data.globalMetaData.point_size*(1+sizeFactor), channel2);
                    CloudUpdater.instance.ChangeBrightness(data.globalMetaData.point_brightness*(1+brightFactor), channel2);
                }

                 else if (e.touchpadAxis.y <= -0.15 && e.touchpadAxis.x < (Mathf.Sqrt(2f) / 2) && e.touchpadAxis.x > (-Mathf.Sqrt(2f) / 2))
                {
                    //down
                    cloudPoint.transform.parent.localScale = cloudPoint.transform.parent.localScale*(1-scaleFactor);
                    CloudUpdater.instance.ChangePointSize(data.globalMetaData.point_size*(1-sizeFactor), channel2);
                    CloudUpdater.instance.ChangeBrightness(data.globalMetaData.point_brightness*(1-brightFactor), channel2);
                }
            }
            
        }

        public void OnButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            Debug.Log("RESET SCALE");
            if(menuActivated == false || scaleLock== false)
            {
                cloudPoint.transform.parent.localScale = new Vector3(1f,1f,1f); 
                cloudPoint.transform.localScale = new Vector3(1f,1f,1f); 
                box.transform.localScale = new Vector3(1f, 0.9985481f, 0.2135878f);
                CloudUpdater.instance.ChangePointSize(initialSize, channel2);
                CloudUpdater.instance.ChangeBrightness(initialBright, channel2);
            }
        }

        public void OnTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            scaleLock = true;
        }

        
        public void OnTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            scaleLock = false;
        }


    }

}