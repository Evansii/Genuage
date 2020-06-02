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


namespace DesktopInterface
{

    public class SMAPRecorderButton : IButtonScript
    {
        public bool isAnimated = false;

        public bool isRecording = false;

        public GameObject box;

        CloudData data;

        SMAPAnimateCloud cloudAnim;

        GameObject camera;

        ScreenRecorder recorder;

        public Button recordButton;
        public Button saveKeyButton;
        public Button previewButton;
        public Button deleteAnimButton;

        public Button deleteKeyButton;
        public Button rankUpButton;
        public Button rankDownButton;

        public InputField recordTimeInputField;
        public InputField nameInputField;

        public Dropdown framerateDropdown;
        public Dropdown animSpeedDropdown;
        public Dropdown keyframeManagerDropdown;

        

        public float framerate = 30;
        public float recordTime = 10;
        public int nb_frames = 300;

        public List<string> keyframeList;
        public int keyframeCount;






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



        }



        public override void Execute()
        {
            cloudAnim = GameObject.FindWithTag("PointCloud").GetComponent<SMAPAnimateCloud>();
            Debug.Log(cloudAnim);
            data = CloudUpdater.instance.LoadCurrentStatus(); 
            box = GameObject.Find("Box");
            box.SetActive(true);
      
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
            box.SetActive(false);
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



        public void PreviewAnimation()
        {
            cloudAnim.PlayAnimation();
        }


        public void DeleteAnimation()
        {
            cloudAnim.RemoveAnimation();
            isAnimated = false;
        }


        public void UpdateKeyframeManager()
        {
            keyframeManagerDropdown.ClearOptions();
            keyframeManagerDropdown.AddOptions(keyframeList);

        }

        public void SaveKeyframe()
        { 
            keyframeCount++;
            keyframeList.Add(keyframeCount.ToString());

            cloudAnim.AddKeyframe();
            if(!isAnimated)
                isAnimated = true;

            UpdateKeyframeManager();

        }




    }



}
