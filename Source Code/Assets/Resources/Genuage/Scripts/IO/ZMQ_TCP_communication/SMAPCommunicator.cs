//Vincent Casamayou
//SMAP Communicator
//Last version 12/08/2020

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace IO
{
    public class SMAPCommunicator : ThreadCommunicator
    {

        // Use this for initialization
        TcpListener listener;

        public string request_type ="";
            
        public string roi_processed;

        public int nb_packages = 10;

        public int package_count = 0;

        public int nb_channels;

        public int channels_count = 0;

        public bool isLoading = false;

        public bool isColorSet = false;

        public List<float> col_x;
        public List<float> col_y;
        public List<float> col_z;
        public List<float> col_pre;

        public List<float> col_frame;

        public List<float> col_hide;

        public Dictionary<string ,string> colorTransition;



        protected override void Run() 
        {

            dataList = new List<float[]>();
            dataList_ch2 = new List<float[]>();
            
            col_x = new List<float>();
            col_y = new List<float>();
            col_z = new List<float>();
            col_pre = new List<float>();
            col_frame = new List<float>();
            col_hide = new List<float>();
            
            colorTransition = new Dictionary<string ,string>();

            // progressUI = GameObject.Find("Modal Window").transform.GetChild(0).GetComponent<Text>();

            listener = new TcpListener(IPAddress.Any,5555);

            InitialiseColorMapTransition();
            ReceiveOnePointData();
        }


        public void InitialiseColorMapTransition()
        {
            colorTransition.Add("blue cold", "Blues");
            colorTransition.Add("green cold", "Greens");
            colorTransition.Add("red hot", "hot");
            colorTransition.Add("jet", "jet");
            colorTransition.Add("hsv", "hsv");
        }

        public async void WaitForLoading()
        {
            Debug.Log("Loading...");
            while(isLoading == false)
            {
                await Task.Yield();
            }
            Debug.Log("Loading complete");
        
        }



        // Update is called once per frame
        protected override ReceiveStatus ReceivePointValues()
        {
            
            listener.Start();
            Debug.Log("is listening (Vincent Version)");
            progressUI = "Run SMAP Plugin to load Data";

            while(true)
            {
                if (!listener.Pending())
                {
                } 
                else 
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream ns = client.GetStream();
                    // if(request_type == "")
                    // {
                    //     request_type = GetDataType(client,ns);
                    //     return ReceiveStatus.DATA_TYPE;
                    // }
                    switch(request_type)
                    {
                        case "":
                            request_type = GetDataType(client,ns);
                            break;

                        // case "ID":
                        //     Debug.Log("you got ID!");
                        //     roi_processed = GetRoiID(client,ns);
                        //     request_type ="";
                        //     return ReceiveStatus.SUCCESS;
                        //     break;

                        case "CH":
                            GetNumberChannels(client,ns);
                            Debug.Log("You got "+nb_channels+" channels");
                            request_type= "";
                            break;


                        case "LC":
                            isLoading = false;
                            progressUI = package_count +"/"+nb_packages+" received...";
                            if(channels_count > 0)
                            {
                                dataList_ch2 = GetLocData(client,ns);
                            }
                            else
                            {
                                dataList = GetLocData(client,ns);
                            }

                        
                            request_type ="";
                            break;
                        
                        case "CO":
                            SMAPColorMap = SetColorMap(client, ns);
                            Debug.Log("Color Map changed to " + SMAPColorMap);
                            isColorSet = true;
                            request_type ="";
                            //return ReceiveStatus.SET_COLOR;
                            break;
                        
                        case "CF":
                            SMAPColorField = SetColorField(client,ns);
                            Debug.Log("Color coding set to Column " + SMAPColorField);
                            request_type = "";
                            break;


                        // case "ALL":
                        //     GetSiteList(client,ns);
                        //     Debug.Log("Site List Loaded");
                        //     request_type = "";  
                        //     return ReceiveStatus.SUCCESS;
                        //     break;

                        case "PK":
                            GetNumberPackages(client,ns);
                            Debug.Log("You got "+nb_packages+" packages");
                            request_type ="";
                            break;

                        default:
                            Debug.Log(request_type +" is a wrong datatype");
                            return ReceiveStatus.INVALID_FORMAT;
                            break;

                    }

                    if((package_count == nb_packages) && (isColorSet == true))
                    {
                        package_count = 0;
                        isColorSet = false;
                        Debug.Log("All Data acquired");
                        progressUI = "All Packages acquired";
                        channels_count ++;
                        if (channels_count == nb_channels)
                        {
                            return ReceiveStatus.SUCCESS;
                        }
                    }

                }
            }
        }



        public void SendMovieRequest()
        {
            TcpClient client = new TcpClient("127.0.0.1",5000);
            NetworkStream nwstream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("MOVIE"); 
            nwstream.Write(bytesToSend,0,bytesToSend.Length);
            Debug.Log("Request sent to reconstruct movie");
            client.Close();

        }

        public void SendSiteRequest(string site_id)
        {
            TcpClient client = new TcpClient("127.0.0.1",5000);
            NetworkStream nwstream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(site_id);

            Debug.Log("Sending request for the site " + site_id);
            nwstream.Write(bytesToSend,0,bytesToSend.Length);
            client.Close();

        }

        public void SendColorMapUpdate(string color_map)
        {
            var color_translation = colorTransition
                            .FirstOrDefault(x => x.Value.Contains(color_map))
                            .Key;


            TcpClient client = new TcpClient("127.0.0.1",5000);
            NetworkStream nwstream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("COLOR"); 
            nwstream.Write(bytesToSend,0,bytesToSend.Length);
            client.Close();

            client = new TcpClient("127.0.0.1",5000);
            nwstream = client.GetStream();
            bytesToSend = ASCIIEncoding.ASCII.GetBytes(color_translation); 
            nwstream.Write(bytesToSend,0,bytesToSend.Length);
            client.Close();

            Debug.Log("Request sent to update Color Map to " + color_translation);


        }

        public string SetColorMap(TcpClient client, NetworkStream ns)
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];
            var bytes_read = ns.Read(bytes,0,(int)client.ReceiveBufferSize);
            string map = Encoding.UTF8.GetString(bytes,0, bytes_read);
            if (colorTransition.ContainsKey(map))
                return colorTransition[map];
            else
                return "autumn";    
            
        }

         public int SetColorField(TcpClient client, NetworkStream ns)
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];
            var bytes_read = ns.Read(bytes,0,(int)client.ReceiveBufferSize);
            int color_field = BitConverter.ToInt32(bytes,0);
            return color_field;
            
        }
        
        public List<float[]> GetLocData(TcpClient client, NetworkStream ns)
        {
            List<float[]> dataset = new List<float[]>();

            byte[] bytes = new byte[client.ReceiveBufferSize];
            var bytes_read = ns.Read(bytes,0,(int)client.ReceiveBufferSize);
            var data_to_process = new float[bytes_read/4];
            Buffer.BlockCopy(bytes,0,data_to_process,0,bytes_read);
            var whole_data = new float[data_to_process.Length];

            var index = 0;
            for(int i =0; i<data_to_process.Length; i += 6)
            {

                float[] tmp = new float[6];
                Array.Copy(data_to_process,i,tmp,0,6);

                col_x.Add(tmp[0]);
                col_y.Add(tmp[1]);
                col_z.Add(tmp[2]);
                col_pre.Add(tmp[3]);
                col_frame.Add(tmp[4]);
                col_hide.Add(tmp[5]);

                index++;               

            }
            package_count++;
            isLoading = true;
            if(package_count == nb_packages)
            {
                dataset.Add(col_x.ToArray());
                col_x.Clear();
                dataset.Add(col_y.ToArray());
                col_y.Clear();
                dataset.Add(col_z.ToArray());
                col_z.Clear();
                dataset.Add(col_pre.ToArray());
                col_pre.Clear();
                dataset.Add(col_frame.ToArray());
                col_frame.Clear();
                dataset.Add(col_hide.ToArray());
                col_hide.Clear();
            }

            return dataset;

            
            
        }


        public void GetNumberChannels(TcpClient client, NetworkStream ns)
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];
            var bytes_read = ns.Read(bytes,0,(int)client.ReceiveBufferSize);
            nb_channels = BitConverter.ToInt32(bytes,0);
        }

        public void GetNumberPackages(TcpClient client, NetworkStream ns)
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];
            var bytes_read = ns.Read(bytes,0,(int)client.ReceiveBufferSize);
            nb_packages = BitConverter.ToInt32(bytes,0);

        }


        public string GetDataType(TcpClient client, NetworkStream ns)
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];
            var bytes_read = ns.Read(bytes,0,(int)client.ReceiveBufferSize);
            string msg = Encoding.UTF8.GetString(bytes,0, bytes_read);
            return msg;

        }


        protected override void StopConnection()
        {
            listener.Stop();
            isRunning = false;

        }

        

    }

}