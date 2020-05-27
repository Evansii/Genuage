using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IO
{


    public class SMAPCommunicator : ThreadCommunicator
    {

        TcpListener listener;
        String msg;
        string datatype;


        protected override void Run()
        {
            dataList = new List<float[]>();
            listener = new TcpListener(IPAddress.Any, 5555);
            Debug.Log("is listening");
            ReceiveOnePointData();
            DateTime firstTime = DateTime.Now;
            DateTime timeLimit = firstTime.AddMinutes(1.0);

        }

        protected override ReceiveStatus ReceivePointValues()
        {
            listener.Start();

            while(true)
            {
                 if(!listener.Pending())
                {
                } 
                else 
                {
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream ns = client.GetStream();


                    switch(datatype)
                    {
                        case "":
                            datatype = DatatypeRequest(client, ns);
                            break;
                        
                        case "Localizations":
                            Debug.Log("Packages incoming");
                            ReceiveLocalizationsPackages(client, ns);
                            break;


                           



                        default:
                            Debug.Log("Wrong Datatype");
                            break;
                    }



          
                }

            }
        
        
        }


        public string DatatypeRequest(TcpClient client, NetworkStream ns)
        {
            Byte[] bytes = new Byte[client.ReceiveBufferSize];
            Int32 bytesread = ns.Read(bytes, 0, bytes.Length);
            var msg = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesread);
            client.Close();
            return msg;

        }

        public void ReceiveLocalizationsPackages(TcpClient client, NetworkStream ns)
        {
            Byte[] bytes = new Byte[client.ReceiveBufferSize];
            Int32 bytesread = ns.Read(bytes, 0, bytes.Length);
            var locs = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, locs, 0, bytes.Length);

            float[] locX = new float[locs.Length/4];
            float[] locY = new float[locs.Length/4];
            float[] locZ = new float[locs.Length/4];
            float[] locPre = new float[locs.Length/4];

            

            for( int i = 0; i<locs.Length; i += 4)
            {



            }


        }



        protected override void StopConnection()
        {
            listener.Stop();
            isRunning = false;

        }
    
        


    }

}