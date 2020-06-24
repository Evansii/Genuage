using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Display;
using Data;


namespace DesktopInterface
{
    public class SMAPClippingPlaneButton : IButtonScript
    {
        public bool isInit = false;

        CloudData data;
        GameObject clipPlaneX;
        GameObject clipPlaneY;
        GameObject clipPlaneZ;

        public GameObject clipV2;


        public Slider sliderX;
        public Slider sliderY;
        public Slider sliderZ;

        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();

            sliderX.onValueChanged.AddListener(UpdateXClipping);
            sliderY.onValueChanged.AddListener(UpdateYClipping);
            sliderZ.onValueChanged.AddListener(UpdateZClipping);
        }

        public override void Execute()
        {
            if(isInit == false)
            {   
                //clipV2.SetActive(true);
                data = CloudUpdater.instance.LoadCurrentStatus();
                // clipPlaneX = GameObject.Find("Desktop Clipping Plane X");
                // sliderX.minValue = data.globalMetaData.xMin;
                // sliderX.maxValue = data.globalMetaData.xMax;

                // clipPlaneY = GameObject.Find("Desktop Clipping Plane Y");
                // sliderY.minValue = data.globalMetaData.yMin;
                // sliderY.maxValue = data.globalMetaData.yMax;

                // clipPlaneZ = GameObject.Find("Desktop Clipping Plane Z");
                // sliderZ.minValue = data.globalMetaData.zMin;
                // sliderZ.maxValue = data.globalMetaData.zMax;
                isInit = true;
            }

        }

        public void UpdateXClipping(float value)
        {
            data.globalMetaData.xMinThreshold = value;
            clipPlaneX.transform.localPosition = new Vector3(value/sliderX.maxValue,0,0);  
            CloudUpdater.instance.ChangeThreshold();          

        }
         public void UpdateYClipping(float value)
        {
            data.globalMetaData.yMinThreshold = value;
            clipPlaneY.transform.localPosition = new Vector3(0,value/sliderY.maxValue,0);  
            CloudUpdater.instance.ChangeThreshold();          

        }
         public void UpdateZClipping(float value)
        {
            data.globalMetaData.zMinThreshold = value;
            clipPlaneZ.transform.localPosition = new Vector3(0,0,value/sliderZ.maxValue);  
            CloudUpdater.instance.ChangeThreshold();          

        }
    }
}
