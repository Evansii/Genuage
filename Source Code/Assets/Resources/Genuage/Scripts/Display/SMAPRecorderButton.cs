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

        CloudData data;

        SMAPAnimateCloud cloudAnim;

        GameObject camera;

        ScreenRecorder recorder;

        public Button recordButton;
        public Button saveKeyButton;
        public Button previewButton;

        public InputField recordTimeInputField;
        public InputField nameInputField;

        public Dropdown framerateDropdown;
        public Dropdown animSpeedDropdown;

        

        public float framerate = 30;
        public float recordTime = 10;
        public int nb_frames = 300;






        private void Awake()
        {
            button = GetComponent<Button>();
            initializeClickEvent();

            camera = GameObject.FindWithTag("MainCamera");
            
            recorder = camera.GetComponent<ScreenRecorder>();

            recordButton.onClick.AddListener(Record);

            previewButton.onClick.AddListener(PreviewAnimation);

            saveKeyButton.onClick.AddListener(SaveKeyframe);

            recordTimeInputField.text = recordTime.ToString();
            recordTimeInputField.onEndEdit.AddListener(UpdateRecordTime);

            nameInputField.text = "Outputname";
            nameInputField.onEndEdit.AddListener(UpdateRecordName);

            framerateDropdown.onValueChanged.AddListener(UpdateFrameRate);

            animSpeedDropdown.onValueChanged.AddListener(UpdateAnimationSpeed);


        }



        public override void Execute()
        {
            cloudAnim = GameObject.FindWithTag("PointCloud").GetComponent<SMAPAnimateCloud>();
            Debug.Log(cloudAnim);
            data = CloudUpdater.instance.LoadCurrentStatus(); 
      
        }

        public void Record()
        {

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

        public void SaveKeyframe()
        { 
            cloudAnim.AddKeyframe();
            if(!isAnimated)
                isAnimated = true;
        }






    }



}
