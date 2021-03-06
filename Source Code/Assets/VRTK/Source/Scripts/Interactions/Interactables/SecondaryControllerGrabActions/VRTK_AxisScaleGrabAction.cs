﻿// Axis Scale Grab Action|SecondaryControllerGrabActions|60030

//October 2020
//Modified by Vincent Casamayou for SuperG
namespace VRTK.SecondaryControllerGrabActions
{
    using UnityEngine;
    using UnityEngine.UI;
    using Data;
    using VRTK.GrabAttachMechanics;

    /// <summary>
    /// Scales the grabbed Interactable Object along the given axes based on the position of the secondary grabbing Interact Grab.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_AxisScaleGrabAction` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Secondary Grab Action Script` parameter to denote use of the secondary grab action.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/043_Controller_SecondaryControllerActions` demonstrates the ability to grab an object with one controller and scale it by grabbing and pulling with the second controller.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Secondary Controller Grab Actions/VRTK_AxisScaleGrabAction")]
    public class VRTK_AxisScaleGrabAction : VRTK_BaseGrabAction
    {
        [Tooltip("The distance the secondary grabbing object must move away from the original grab position before the secondary grabbing object auto ungrabs the Interactable Object.")]
        public float ungrabDistance = 10f;
        [Tooltip("Locks the specified checked axes so they won't be scaled")]
        public Vector3State lockAxis = Vector3State.False;
        [Tooltip("If checked all the axes will be scaled together (unless locked)")]
        public bool uniformScaling = false;

        public GameObject additionalObjectToScale;
        public GameObject channel2toScale;
        public GameObject channel2toScale_cloud;
        public GameObject camera;

        protected GameObject grabPointOne;
        protected GameObject grabPointTwo;

        CloudData data;
        Vector3 newScale;
        Vector3 finalScale;
        
        Vector3 currentScale;
        public bool grabchan = false;
        public bool isLinked = false;
       // public bool focus = false;
        
        public float scaleFactor =0.0025f;
        public float sizeFactor = 0.0025f;
        public float brightFactor = 0.01f; 
        public float scaleThreshold = 0.01f;
        public float adjustedLength;
        public Quaternion relativeRotation;

        [Header("Obsolete Settings")]

        [System.Obsolete("`VRTK_AxisScaleGrabAction.lockXAxis` has been replaced with the `VRTK_AxisScaleGrabAction.lockAxis`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public bool lockXAxis = false;
        [System.Obsolete("`VRTK_AxisScaleGrabAction.lockYAxis` has been replaced with the `VRTK_AxisScaleGrabAction.lockAxis`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public bool lockYAxis = false;
        [System.Obsolete("`VRTK_AxisScaleGrabAction.lockZAxis` has been replaced with the `VRTK_AxisScaleGrabAction.lockAxis`. This parameter will be removed in a future version of VRTK.")]
        [ObsoleteInspector]
        public bool lockZAxis = false;

        protected Vector3 initialScale;

        public Vector3 initialGrabOnePos;
        public Vector3 initialGrabTwoPos;
        public Vector3 initialPos;
        
        protected Vector3 middlePosition;

        protected float initalLength;
        protected float initialScaleFactor;

        /// <summary>
        /// The Initalise method is used to set up the state of the secondary action when the Interactable Object is initially grabbed by a secondary Interact Grab.
        /// </summary>
        /// <param name="currentGrabbdObject">The Interactable Object script for the object currently being grabbed by the primary grabbing object.</param>
        /// <param name="currentPrimaryGrabbingObject">The Interact Grab script for the object that is associated with the primary grabbing object.</param>
        /// <param name="currentSecondaryGrabbingObject">The Interact Grab script for the object that is associated with the secondary grabbing object.</param>
        /// <param name="primaryGrabPoint">The point on the Interactable Object where the primary Interact Grab initially grabbed the Interactable Object.</param>
        /// <param name="secondaryGrabPoint">The point on the Interactable Object where the secondary Interact Grab initially grabbed the Interactable Object.</param>
        public override void Initialise(VRTK_InteractableObject currentGrabbdObject, VRTK_InteractGrab currentPrimaryGrabbingObject, VRTK_InteractGrab currentSecondaryGrabbingObject, Transform primaryGrabPoint, Transform secondaryGrabPoint)
        {
            base.Initialise(currentGrabbdObject, currentPrimaryGrabbingObject, currentSecondaryGrabbingObject, primaryGrabPoint, secondaryGrabPoint);
            data = CloudUpdater.instance.LoadCurrentStatus();
            currentGrabbdObject = GameObject.Find("CloudPoint").GetComponent<VRTK_InteractableObject>();
            grabbedObject = GameObject.Find("CloudPoint").GetComponent<VRTK_InteractableObject>();

            // focus = false;

            // camera = GameObject.Find("Camera (eye)");

            grabPointOne = GameObject.Find("[VRTK][AUTOGEN][LeftHand][StraightPointerRenderer_Cursor]");
            grabPointTwo = GameObject.Find("[VRTK][AUTOGEN][RightHand][StraightPointerRenderer_Cursor]");

            middlePosition = (grabPointOne.transform.position + grabPointTwo.transform.position)/2;

            initialScale = grabbedObject.transform.parent.localScale;
            initalLength = (primaryGrabbingObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
            initialScaleFactor = (currentGrabbdObject.transform.parent.localScale.x / initalLength );
            initialPos = grabbedObject.transform.parent.localPosition;


            relativeRotation = Quaternion.Inverse(additionalObjectToScale.transform.rotation) * grabbedObject.transform.parent.rotation;
            //this.gameObject.GetComponent<VRTK_ChildOfControllerGrabAttach>().moveLock = true;


#pragma warning disable 618
            if ((lockXAxis || lockYAxis || lockZAxis) && lockAxis == Vector3State.False)
            {
                lockAxis = new Vector3State(lockXAxis, lockYAxis, lockZAxis);
            }
#pragma warning restore 618
        }

        /// <summary>
        /// The ProcessUpdate method runs in every Update on the Interactable Object whilst it is being grabbed by a secondary Interact Grab.
        /// </summary>
        public override void ProcessUpdate()
        {
            base.ProcessUpdate();
            CheckForceStopDistance(ungrabDistance);
        }

        /// <summary>
        /// The ProcessFixedUpdate method runs in every FixedUpdate on the Interactable Object whilst it is being grabbed by a secondary Interact Grab and performs the scaling action.
        /// </summary>
        public override void ProcessFixedUpdate()
        {
            base.ProcessFixedUpdate();
            if (initialised)
            {
                if (uniformScaling)
                {
                    UniformScale();
                }
                else
                {
                    NonUniformScale();
                }
            }
        }

         public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
        {
            Vector3 A = target.transform.localPosition;
            Vector3 B = pivot;
        
            Vector3 C = A - B; // diff from object pivot to desired pivot/origin
        
            float RS = newScale.x / target.transform.localScale.x; // relative scale factor
        
            // calc final position post-scale
            Vector3 FP = B + C * RS;
        
            // finally, actually perform the scale/translation
            target.transform.localScale = newScale;
            target.transform.localPosition = FP;
        }

        protected virtual void ApplyScale(Vector3 newScale, GameObject box = null)
        {
            Vector3 existingScale = grabbedObject.transform.parent.localScale;

            float finalScaleX = (lockAxis.xState ? existingScale.x : newScale.x);
            float finalScaleY = (lockAxis.yState ? existingScale.y : newScale.y);
            float finalScaleZ = (lockAxis.zState ? existingScale.z : newScale.z);
        
            finalScale = new Vector3(finalScaleX, finalScaleY, finalScaleZ);
            if (finalScaleX > 0.5f && finalScaleY > 0.5f && finalScaleZ > 0.5f)
            {

                if(box)
                {
                    //box.transform.localScale = new Vector3(finalScaleX, finalScaleY*0.9985481f, finalScaleZ*0.2135878f);
                }
                if(!grabchan && isLinked)
                {
                    Debug.Log("Second Channel Catch");
                    channel2toScale = grabbedObject.transform.parent.transform.GetChild(1).gameObject;
                    channel2toScale_cloud = channel2toScale.transform.GetChild(0).gameObject;
                    grabchan = true;
                }

                // grabPointOne.transform.position = initialGrabOnePos;
                // grabPointTwo.transform.position = initialGrabTwoPos;

                additionalObjectToScale.transform.SetParent(grabbedObject.transform.parent.transform);

                // if(focus == false)
                // {
                //     camera.transform.position -= middlePosition;
                //     //camera.transform.position.z += 1; 
                //     focus = true;
                // }
                ScaleAround(grabbedObject.transform.parent.gameObject, middlePosition, finalScale);
                //grabbedObject.transform.parent.localScale = finalScale;
                grabbedObject.transform.position = box.transform.position;
                grabbedObject.transform.parent.rotation = box.transform.rotation * relativeRotation;


                currentScale = finalScale;

                //CloudUpdater.instance.ChangeCloudScale(finalScale);

                if(channel2toScale)
                {
                    // CloudUpdater.instance.ChangePointSize(finalScaleX, true);
                    // CloudUpdater.instance.ChangeBrightness(finalScaleX, true);
                    LocalizationSizeScale(true);
                    //LocalizationBrightScale(true);
                    
                    
                    channel2toScale_cloud.transform.position = box.transform.position;
                    
                }
                else
                {
                    LocalizationSizeScale(false);
                    //LocalizationBrightScale(false);
                    //CloudUpdater.instance.ChangeCloudScale(finalScale);

                    //data.globalMetaData.point_brightness;
                    //CloudUpdater.instance.ChangeCloudScale(finalScale);
                    //CloudUpdater.instance.ChangePointSize(data.globalMetaData.point_size*finalScaleX, false); 
                    //CloudUpdater.instance.ChangeBrightness(data.globalMetaData.point_brightness*finalScaleX, false);
                }
                //currentScale = finalScale;
                

            }
        }



        protected virtual void NonUniformScale()
        {
            Vector3 initialRotatedPosition = grabbedObject.transform.rotation * grabbedObject.transform.position;
            Vector3 initialSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryInitialGrabPoint.position;
            Vector3 currentSecondGrabRotatedPosition = grabbedObject.transform.rotation * secondaryGrabbingObject.transform.position;

            float newScaleX = CalculateAxisScale(initialRotatedPosition.x, initialSecondGrabRotatedPosition.x, currentSecondGrabRotatedPosition.x);
            float newScaleY = CalculateAxisScale(initialRotatedPosition.y, initialSecondGrabRotatedPosition.y, currentSecondGrabRotatedPosition.y);
            float newScaleZ = CalculateAxisScale(initialRotatedPosition.z, initialSecondGrabRotatedPosition.z, currentSecondGrabRotatedPosition.z);

            newScale = new Vector3(newScaleX, newScaleY, newScaleZ) + initialScale;
            ApplyScale(newScale, additionalObjectToScale);
        }

        protected virtual void LocalizationSizeScale(bool channel2)
        {
            if((initalLength-scaleThreshold) > adjustedLength)
            {
                CloudUpdater.instance.ChangePointSize(data.globalMetaData.point_size*(1-sizeFactor), channel2); 
            }
            if((initalLength+scaleThreshold) < adjustedLength)
            {
                CloudUpdater.instance.ChangePointSize(data.globalMetaData.point_size*(1+sizeFactor), channel2); 
            }
        }

        
        protected virtual void LocalizationBrightScale(bool channel2)
        {
           if(initalLength > (adjustedLength + scaleThreshold))
            {
                CloudUpdater.instance.ChangeBrightness(data.globalMetaData.point_brightness*(1-brightFactor), channel2); 
            }
            if(initalLength < (adjustedLength - scaleThreshold))
            {
                CloudUpdater.instance.ChangeBrightness(data.globalMetaData.point_brightness*(1+brightFactor), channel2); 
            }
        }

        protected virtual void UniformScale()
        {

            adjustedLength = (primaryGrabbingObject.transform.position - secondaryGrabbingObject.transform.position).magnitude;
            Debug.Log("Initial Length: "+ initalLength);
            Debug.Log("Current Length: "+ adjustedLength);
            if( (initalLength-scaleThreshold)  > adjustedLength)
            {
                newScale =  grabbedObject.transform.parent.localScale *(1-scaleFactor);
                ApplyScale(newScale, additionalObjectToScale);
            }
            if((initalLength+scaleThreshold) < adjustedLength)
            {
                newScale = grabbedObject.transform.parent.localScale  *(1+scaleFactor);
                ApplyScale(newScale, additionalObjectToScale);

            }
            // float adjustedScale = initialScaleFactor * adjustedLength ;
        }

        protected virtual float CalculateAxisScale(float centerPosition, float initialPosition, float currentPosition)
        {
            float distance = currentPosition - initialPosition;
            distance = (centerPosition < initialPosition ? distance : -distance);
            return distance;
        }

        public override void ResetAction()
        {
            //grabbedObject.transform.localScale = Vector3.one;
            base.ResetAction();

            //Debug.Log(finalScale.x);
            //CloudUpdater.instance.ChangeCloudScale(finalScale);

            

        }
    }
}