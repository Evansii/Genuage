/**
Vincent Casamayou
RIES GROUP
SMAP Recorder UI
27/05/2020
**/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using Display;
using IO;
using VRTK;


namespace DesktopInterface
{

    public class SMAPRecorderButton : IButtonScript
    {
        public bool isAnimated = false;

        public bool isRecording = false;

        public bool hiddenShadows = false;

        public MeshRenderer box;

        CloudData data;

        SMAPAnimateCloud cloudAnim;

        SMAPAnimateCloud cameraAnim;

        public SMAPAnimateCloud clipAnim;

        public SMAPAnimateCloud clipZAnim;

        GameObject camera;

        GameObject cloudpoint;

        GameObject cloudpointShadow;

        GameObject boxobject;


        public GameObject clipPlaneXY;   
        public GameObject clipPlaneZ;
        public GameObjectActivationButton clipScript;
        public Texture shadowTex;

        ScreenRecorder recorder;

        public Button recordButton;
        public Button saveKeyButton;
        public Button previewButton;
        public Button recordAnimButton;
        
        public Button deleteKeyButton;
        public Button updateKeyButton;

        public InputField recordTimeInputField;
        public InputField nameInputField;

        public Dropdown framerateDropdown;
        public Dropdown animSpeedDropdown;
        public Dropdown keyframeManagerDropdown;

        public Toggle hideShadowToggle;

        public Text updateText;

        public float framerate = 30;
        public float recordTime = 10;
        public int nb_frames = 300;

        public List<string> keyframeList;
        public int keyframeCount;

        public List<GameObject> shadowsList = new List<GameObject>();

        public string previousColorMap;

        public List<Vector3[]> transfoList;

        public List<Vector2[]> clipXYList;

        public List<Vector2[]> clipZList;
        

        




        private void Awake()
        {
            keyframeList = new List<string>();

            transfoList = new List<Vector3[]>();
            clipXYList = new List<Vector2[]>();
            clipZList = new List<Vector2[]>();

            button = GetComponent<Button>();

            initializeClickEvent();

            camera = GameObject.FindWithTag("MainCamera");
            
            recorder = camera.GetComponent<ScreenRecorder>();

            recordButton.onClick.AddListener(Record);
            recordAnimButton.onClick.AddListener(RecordAnimation);

            previewButton.onClick.AddListener(PreviewAnimation);

            saveKeyButton.onClick.AddListener(SaveKeyframe);

            recordTimeInputField.text = recordTime.ToString();
            recordTimeInputField.onEndEdit.AddListener(UpdateRecordTime);

            nameInputField.text = "Output_name";
            nameInputField.onEndEdit.AddListener(UpdateRecordName);

            framerateDropdown.onValueChanged.AddListener(UpdateFrameRate);

            animSpeedDropdown.onValueChanged.AddListener(UpdateAnimationSpeed);


            //Keyframe Manager
            deleteKeyButton.onClick.AddListener(DeleteKeyframe);
            updateKeyButton.onClick.AddListener(UpdateKeyframe);
            keyframeManagerDropdown.onValueChanged.AddListener(UpdateTransformToKeyframe);
            keyframeManagerDropdown.onValueChanged.AddListener(UpdateClippingPlaneToKeyframe);
            
            //keyframeManagerDropdown.onValueChanged.AddListener(HideKeyframeShadows);
            //hideShadowToggle.onValueChanged.AddListener(UpdateHiddenShadows);





        }



        public override void Execute() 
        {
            clipScript.Execute();
            cloudpoint = GameObject.FindWithTag("PointCloud");
            cloudAnim = cloudpoint.GetComponent<SMAPAnimateCloud>();
            cameraAnim = camera.GetComponent<SMAPAnimateCloud>();
            data = CloudUpdater.instance.LoadCurrentStatus(); 
            boxobject =  GameObject.Find("Box");
            box = GameObject.Find("Box").GetComponent<MeshRenderer>();
            box.enabled = true;

      
        }

        public void Record()
        {
            HideBox();
            //hiddenShadows = true;
            //HideKeyframeShadows(0);
            // if(isAnimated)
            // {
            //     recordTime = cloudAnim.animationTime + 1f;
            //     UpdateFrameNumber();
            //     cloudAnim.PlayAnimation();
            // }
            isRecording = true;
            Debug.Log("Record ON");
            updateText.text = "Recording...";
            recorder.enabled = true;


        }

        public void RecordAnimation()
        {
            recordTime = cloudAnim.animationTime + 1f;
            //recordTime += 1f;
            UpdateFrameNumber();
            cloudAnim.PlayAnimation();
            cameraAnim.PlayAnimation();
            clipAnim.PlayAnimation();
            clipZAnim.PlayAnimation();
            Record();
        }


        public void HideBox()
        {
            box.enabled = false;
        }

        //LEGACY OLD SHADOW SYSTEM
        // public void CreateKeyframeShadow()
        // {
        //     cloudpointShadow = GameObject.Instantiate(cloudpoint);
        //     cloudpointShadow.name ="Keyframe Shadow " + keyframeManagerDropdown.options.Count;
        //     cloudpointShadow.tag = "Shadow";
        //     cloudpointShadow.GetComponent<MeshRenderer>().material.SetTexture("_ColorTex", shadowTex);
        //     Destroy(cloudpointShadow.GetComponent<CloudData>());
        //     Destroy(cloudpointShadow.GetComponent<CloudBox>());
        //     Destroy(cloudpointShadow.GetComponent<SMAPAnimateCloud>());
        //     Destroy(cloudpointShadow.GetComponent<VRTK_InteractableObject>());
        //     //Destroy(cloudpointShadow.GetComponent<VRTK_ChildOfControllerGrabAttach>());
        //     Destroy(cloudpointShadow.GetComponent<VRTK_RigidbodyFollow>());
        //     Destroy(cloudpointShadow.GetComponent<Rigidbody>());
        //     Destroy(cloudpointShadow.GetComponent<Animation>());

        //     shadowsList.Add(cloudpointShadow);
            
        //     if(hiddenShadows)
        //     {
        //         cloudpointShadow.SetActive(false);
        //     }           
        // }


        public void SaveTransformToList()
        {
            Vector3[] transToSave = new Vector3[3];
            transToSave[0] = cloudpoint.transform.position;
            transToSave[1] = cloudpoint.transform.localScale;
            transToSave[2] = cloudpoint.transform.localEulerAngles;
            transfoList.Add(transToSave);
    
        }

        public void SaveClippingToList(GameObject clipPlane ,List<Vector2[]> clipList)
        {
            Vector2[] clipToSave = new Vector2[2];
            RectTransform clipTransfo = clipPlane.GetComponent<RectTransform>();

            clipToSave[0] = clipTransfo.anchoredPosition;
            clipToSave[1] = clipTransfo.sizeDelta;

            clipList.Add(clipToSave);
        }


        //LEGACY OLD SHADOW SYSTEM
        // public void HideKeyframeShadows(int id)
        // {
        //     GameObject[] shadows = GameObject.FindGameObjectsWithTag("Shadow");
        //     foreach(GameObject shad in shadowsList)
        //     {
        //         if(hiddenShadows)
        //         {
        //             shad.SetActive(false);
        //         }
        //         else
        //         {
        //             if(shad.name != "Keyframe Shadow "+ (keyframeManagerDropdown.value+1))
        //             {
        //                 shad.GetComponent<MeshRenderer>().enabled = false;
        //                 shad.SetActive(false);

        //             }
        //             else
        //             {
        //                 shad.SetActive(true);
        //             }
        //         }
        //     }
        //     hiddenShadows = false;
        // }

        public void UpdateTransformToKeyframe(int id)
        {
            boxobject.transform.position = transfoList[id][0];
            boxobject.transform.localEulerAngles = transfoList[id][2];


            cloudpoint.transform.position = transfoList[id][0];
            cloudpoint.transform.localScale = transfoList[id][1];
            cloudpoint.transform.localEulerAngles = transfoList[id][2];

            
        }
        
        public void UpdateClippingPlaneToKeyframe(int id)
        {
            //Update Clipping Plane XY
            RectTransform clipTransfo = clipPlaneXY.GetComponent<RectTransform>();
            clipTransfo.anchoredPosition = clipXYList[id][0];
            clipTransfo.sizeDelta = clipXYList[id][1];

            //Update Clipping Plane Z
            clipTransfo =  clipPlaneZ.GetComponent<RectTransform>();
            clipTransfo.anchoredPosition = clipZList[id][0];
            clipTransfo.sizeDelta = clipZList[id][1];
        }

        //LEGACY OLD SHADOW SYSTEM
        // public void UpdateHiddenShadows(bool select)
        // {
        //     if(hideShadowToggle.isOn)
        //     {
        //         hiddenShadows = true;
        //     }
        //     else
        //     {
        //         hiddenShadows = false;
        //     }
        //     HideKeyframeShadows(0);
        // }

        //LEGACY OLD SHADOW SYSTEM
        // public void UpdateShadowPosition()
        // {
        //     GameObject currentShadow = GameObject.Find("Keyframe Shadow "+ (keyframeManagerDropdown.value+1));
        //     currentShadow.transform.localPosition = cloudpoint.transform.localPosition;
        //     currentShadow.transform.localEulerAngles = cloudpoint.transform.localEulerAngles;
        //     currentShadow.transform.localScale = cloudpoint.transform.localScale;
        // }
    

        public void UpdateFrameRate(int id)
        {
            if (Single.TryParse(framerateDropdown.options[id].text, out float j))
            {
                recorder.frameRate = Convert.ToInt32(j);
                framerate = j;
                UpdateFrameNumber();
            }
            else
            {
                Debug.Log("Framerate could not be parsed.");
            }

        }

        public void UpdateRecordTime(string s)
        {
            if (Int32.TryParse(s, out int j))
            {
                if (j >= 0)
                {
                    recordTime = (float)j;
                    UpdateFrameNumber();
                }
            }
            else
            {
                Debug.Log("Record Time could not be parsed.");
                recordTimeInputField.text = recordTime.ToString();
            }

        }

        public void UpdateRecordName(string s)
        {
            recorder.outputname = s;
        }


        public void UpdateFrameNumber()
        {
            nb_frames = Convert.ToInt32(recordTime* framerate);
            recorder.maxFrames = nb_frames;

        }

        public void UpdateAnimationSpeed(int id)
        {
            if (Single.TryParse(animSpeedDropdown.options[id].text, out float speed))
            {
                cloudAnim.SetAnimationSpeed(speed);
                cameraAnim.SetAnimationSpeed(speed);
                clipAnim.SetAnimationSpeed(speed);
                clipZAnim.SetAnimationSpeed(speed);
            }
            else
                Debug.Log("Animation Speed could not be parsed");
        }

        public void UpdateKeyframe()
        {
            cloudAnim.UpdateKeyframe(keyframeManagerDropdown.value+1);
            cameraAnim.UpdateKeyframe(keyframeManagerDropdown.value+1);
            clipAnim.UpdateKeyframe(keyframeManagerDropdown.value+1);
            clipZAnim.UpdateKeyframe(keyframeManagerDropdown.value+1);
            
            UpdateTransfoPosition(keyframeManagerDropdown.value);
            UpdateClippingPlanePosition(keyframeManagerDropdown.value);
            
            //LEGACY(not compatible with ClippingPlane)
            //UpdateShadowPosition();


        }

        public void UpdateTransfoPosition(int id)
        {
            transfoList[id][0] = cloudpoint.transform.position;
            transfoList[id][1] = cloudpoint.transform.localScale;
            transfoList[id][2] = cloudpoint.transform.localEulerAngles;
        }

        public void UpdateClippingPlanePosition(int id)
        {
           //Update Clipping Plane XY
            RectTransform clipTransfo = clipPlaneXY.GetComponent<RectTransform>();
            clipXYList[id][0] = clipTransfo.anchoredPosition;
            clipXYList[id][1] = clipTransfo.sizeDelta;

            //Update Clipping Plane Z
            clipTransfo =  clipPlaneZ.GetComponent<RectTransform>();
            clipXYList[id][0] = clipTransfo.anchoredPosition;
            clipZList[id][1] = clipTransfo.sizeDelta;
        }   
    



        public void PreviewAnimation()
        {
            cloudAnim.PlayAnimation();
            cameraAnim.PlayAnimation();
            clipAnim.PlayAnimation();
            clipZAnim.PlayAnimation();
            
        }


        public void DeleteKeyframe()
        {
            //Single.TryParse(framerateDropdown.options[framerateDropdown.value].text, out float j);
            //recordTime -= j;

            //Remove Keyframe
            cloudAnim.RemoveKeyframe(keyframeManagerDropdown.value+1);
            cameraAnim.RemoveKeyframe(keyframeManagerDropdown.value+1);
            clipAnim.RemoveKeyframe(keyframeManagerDropdown.value+1);
            clipZAnim.RemoveKeyframe(keyframeManagerDropdown.value+1);

            keyframeList.RemoveAt(keyframeManagerDropdown.value);

            transfoList.RemoveAt(keyframeManagerDropdown.value);
            clipXYList.RemoveAt(keyframeManagerDropdown.value);
            clipZList.RemoveAt(keyframeManagerDropdown.value);

            if(keyframeManagerDropdown.value != keyframeList.Count)
            {
                for(int i = keyframeManagerDropdown.value; i<keyframeList.Count; i++)
                {
                    int key = Int32.Parse(keyframeList[i]);
                    keyframeList[i] = (key - 1).ToString(); 
                }
            }

            keyframeCount--;

            //Remove Keyframe Shadow (LEGACY)
            // Destroy(shadowsList[keyframeManagerDropdown.value]);
            // shadowsList.RemoveAt(keyframeManagerDropdown.value);

            // for(int j = 0; j < shadowsList.Count;j++)
            // {
            //     shadowsList[j].name ="Keyframe Shadow "+(j+1);
            // }

            UpdateKeyframeManager();

        }


        public void UpdateKeyframeManager()
        {
            keyframeManagerDropdown.ClearOptions();
            keyframeManagerDropdown.AddOptions(keyframeList);
            keyframeManagerDropdown.value = keyframeManagerDropdown.options.Count-1;

        }

        public void SaveKeyframe()
        {
            //Single.TryParse(framerateDropdown.options[framerateDropdown.value].text, out float j);
            //recordTime += j;
            string currentColorMap = cloudpoint.GetComponent<CloudData>().globalMetaData.colormapName;
            
            cloudAnim.AddKeyframe();
            cameraAnim.AddKeyframe();
            clipAnim.AddKeyframe();
            clipZAnim.AddKeyframe();
            if(!isAnimated)
                isAnimated = true;

            if(currentColorMap != previousColorMap)
            {
                Debug.Log("Event added");
                cloudAnim.AddAnimationEvent("ColorMap", currentColorMap);
            }
            
            SaveTransformToList();

            SaveClippingToList(clipPlaneXY, clipXYList);
            SaveClippingToList(clipPlaneZ, clipZList);

            keyframeCount++;
            keyframeList.Add(keyframeCount.ToString());
            UpdateKeyframeManager();

            //LEGACY (not compatible with Clipping Plane)
            //CreateKeyframeShadow();
            previousColorMap = currentColorMap;
            


            updateText.text = "Animation ready";
        }

        void Update()
        {
            if(recorder.isVideoReady)
            {
               updateText.text = "Video is ready";
               recorder.isVideoReady = false; 
            }

        }




    }



}
