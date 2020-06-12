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

        GameObject camera;

        GameObject cloudpoint;

        GameObject cloudpointShadow;
        public Texture shadowTex;

        ScreenRecorder recorder;

        public Button recordButton;
        public Button saveKeyButton;
        public Button previewButton;
        public Button deleteAnimButton;

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





        private void Awake()
        {
            keyframeList = new List<string>();

            button = GetComponent<Button>();

            initializeClickEvent();

            camera = GameObject.FindWithTag("MainCamera");
            
            recorder = camera.GetComponent<ScreenRecorder>();

            recordButton.onClick.AddListener(Record);

            previewButton.onClick.AddListener(PreviewAnimation);

            saveKeyButton.onClick.AddListener(SaveKeyframe);

            deleteAnimButton.onClick.AddListener(DeleteAnimation);

            recordTimeInputField.text = recordTime.ToString();
            recordTimeInputField.onEndEdit.AddListener(UpdateRecordTime);

            nameInputField.text = "Output_name";
            nameInputField.onEndEdit.AddListener(UpdateRecordName);

            framerateDropdown.onValueChanged.AddListener(UpdateFrameRate);

            animSpeedDropdown.onValueChanged.AddListener(UpdateAnimationSpeed);


            //Keyframe Manager
            deleteKeyButton.onClick.AddListener(DeleteKeyframe);
            updateKeyButton.onClick.AddListener(UpdateKeyframe);
            keyframeManagerDropdown.onValueChanged.AddListener(HideKeyframeShadows);
            hideShadowToggle.onValueChanged.AddListener(UpdateHiddenShadows);





        }



        public override void Execute()
        {
            cloudpoint = GameObject.FindWithTag("PointCloud");
            cloudAnim = cloudpoint.GetComponent<SMAPAnimateCloud>();
            data = CloudUpdater.instance.LoadCurrentStatus(); 
            box = GameObject.Find("Box").GetComponent<MeshRenderer>();
            box.enabled = true;
      
        }

        public void Record()
        {
            HideBox();
            if(isAnimated)
            {
                recordTime = cloudAnim.animationTime + 1f;
                UpdateFrameNumber();
                cloudAnim.PlayAnimation();
            }

            if(!isRecording)
            {
                isRecording = true;
                Debug.Log("Record ON");
                updateText.text = "Recording...";
                recorder.enabled = true;
            }
            else
            {
                Debug.Log("Record OFF");
                isRecording = false;
                recorder.enabled = false;
            }

        }


        public void HideBox()
        {
            box.enabled = false;
        }

        public void CreateKeyframeShadow()
        {
            cloudpointShadow = GameObject.Instantiate(cloudpoint);
            cloudpointShadow.name ="Keyframe Shadow " + keyframeManagerDropdown.options.Count;
            cloudpointShadow.tag = "Shadow";
            cloudpointShadow.GetComponent<MeshRenderer>().material.SetTexture("_ColorTex", shadowTex);
            Destroy(cloudpointShadow.GetComponent<CloudData>());
            Destroy(cloudpointShadow.GetComponent<CloudBox>());
            Destroy(cloudpointShadow.GetComponent<SMAPAnimateCloud>());
            Destroy(cloudpointShadow.GetComponent<VRTK_InteractableObject>());
            //Destroy(cloudpointShadow.GetComponent<VRTK_ChildOfControllerGrabAttach>());
            Destroy(cloudpointShadow.GetComponent<VRTK_RigidbodyFollow>());
            Destroy(cloudpointShadow.GetComponent<Rigidbody>());
            Destroy(cloudpointShadow.GetComponent<Animation>());

            shadowsList.Add(cloudpointShadow);
            
            if(hiddenShadows)
            {
                cloudpointShadow.SetActive(false);
            }           
        }


        public void HideKeyframeShadows(int id)
        {
            //GameObject[] shadows = GameObject.FindGameObjectsWithTag("Shadow");
            Debug.Log("Keyframe Shadow "+ (keyframeManagerDropdown.value+1));
            foreach(GameObject shad in shadowsList)
            {
                if(hiddenShadows)
                {
                    shad.SetActive(false);
                }
                else
                {
                    if(shad.name != "Keyframe Shadow "+ (keyframeManagerDropdown.value+1))
                    {
                        //shad.GetComponent<MeshRenderer>().enabled = false;
                        shad.SetActive(false);

                    }
                    else
                    {
                        shad.SetActive(true);
                    }
                }
            }
        }

        public void UpdateHiddenShadows(bool select)
        {
            if(hideShadowToggle.isOn)
            {
                hiddenShadows = true;
            }
            else
            {
                hiddenShadows = false;
            }
            HideKeyframeShadows(0);
        }

        public void UpdateShadowPosition()
        {
            GameObject currentShadow = GameObject.Find("Keyframe Shadow "+ (keyframeManagerDropdown.value+1));
            currentShadow.transform.localPosition = cloudpoint.transform.localPosition;
            currentShadow.transform.localEulerAngles = cloudpoint.transform.localEulerAngles;
            currentShadow.transform.localScale = cloudpoint.transform.localScale;
        }
    

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
            }
            else
                Debug.Log("Animation Speed could not be parsed");
        }

        public void UpdateKeyframe()
        {
            cloudAnim.UpdateKeyframe(keyframeManagerDropdown.value+1);
            UpdateShadowPosition();

        }



        public void PreviewAnimation()
        {
            cloudAnim.PlayAnimation();
        }


        public void DeleteAnimation()
        {
            cloudAnim.RemoveAnimation();
            isAnimated = false;
            updateText.text = "No Animation detected";
            keyframeList.Clear();
            UpdateKeyframeManager();
            keyframeCount = 0;

        }

        public void DeleteKeyframe()
        {
            //Remove Keyframe
            cloudAnim.RemoveKeyframe(keyframeManagerDropdown.value+1);

            keyframeList.RemoveAt(keyframeManagerDropdown.value);

            if(keyframeManagerDropdown.value != keyframeList.Count)
            {
                for(int i = keyframeManagerDropdown.value; i<keyframeList.Count; i++)
                {
                    int key = Int32.Parse(keyframeList[i]);
                    keyframeList[i] = (key - 1).ToString(); 
                }
            }

            keyframeCount--;
            //Remove Keyframe Shadow
            Destroy(shadowsList[keyframeManagerDropdown.value]);
            shadowsList.RemoveAt(keyframeManagerDropdown.value);

            for(int j = 0; j < shadowsList.Count;j++)
            {
                shadowsList[j].name ="Keyframe Shadow "+(j+1);
            }

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

            string currentColorMap = cloudpoint.GetComponent<CloudData>().globalMetaData.colormapName;

            Debug.Log(currentColorMap); 
            
            cloudAnim.AddKeyframe();
            if(!isAnimated)
                isAnimated = true;

            if(currentColorMap != previousColorMap)
            {
                Debug.Log("Event added");
                cloudAnim.AddAnimationEvent("ColorMap", currentColorMap);
            }

            keyframeCount++;
            keyframeList.Add(keyframeCount.ToString());
            UpdateKeyframeManager();

            CreateKeyframeShadow();

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
