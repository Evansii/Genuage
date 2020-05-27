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

        public bool isRecording = false;

        CloudData data;

        GameObject camera;

        ScreenRecorder recorder;

        public Button recordButton;
        public InputField recordTimeInputField;
        public Dropdown framerateDropdown;

        public int framerate = 30;
        public int recordTime = 10;
        public int nb_frames = 300;






        private void Awake()
        {
            camera = GameObject.FindWithTag("MainCamera");
            recorder = camera.GetComponent<ScreenRecorder>();

            recordButton.onClick.AddListener(Record);

            recordTimeInputField.text = recordTime.ToString();
            recordTimeInputField.onEndEdit.AddListener(UpdateRecordTime);

            framerateDropdown.onValueChanged.AddListener(UpdateFrameRate);


        }



        public override void Execute()
        {
            data = CloudUpdater.instance.LoadCurrentStatus(); 
      
        }

        public void Record()
        {
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
                framerate = Convert.ToInt32(j);
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
                    recordTime = j;
                    UpdateFrameNumber();
                }
            }
            else
            {
                Debug.Log("Record Time could not be parsed.");
                recordTimeInputField.text = recordTime.ToString();
            }



        }

        public void UpdateFrameNumber()
        {
            nb_frames = recordTime* framerate;
            recorder.maxFrames = nb_frames;

        }





    }



}
