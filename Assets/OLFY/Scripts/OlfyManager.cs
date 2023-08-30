using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Olfy
{
    public class OlfyManager : MonoBehaviour
    {
        public static OlfyManager Instance;
        #region public members
        public string address;
        public string batteryLevel;
        public string deviceInformation;
        public bool isReady;
        #endregion

        #region private members
        private Dictionary<string, string> deviceDictionary;
        private int _requestToSend;
        private int _sentRequest;
        private bool _once;
        #endregion

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartCoroutine(ScanNetwork());
        }
        #region connexion methods
        IEnumerator ScanNetwork()
        {
          string addressStart = "";
            yield return null;
            deviceDictionary = new Dictionary<string, string>();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ipAddress in host.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    string[] temp = ipAddress.ToString().Split('.');
                    addressStart = temp[0] + "." + temp[1] + "." + temp[2] + ".";
                }
            }

            for (int i = 1; i <= 255; i++)
            {
                StartCoroutine(PingDevice(addressStart + i));
            }
        }

        IEnumerator PingDevice(string ip)
        {

            var ping = new Ping(ip);
            while (!ping.isDone)
            {
                yield return null;
            }

            _requestToSend++;
            Debug.Log(_requestToSend);
            StartCoroutine(CheckOlfy(ip));
        }
        IEnumerator CheckOlfy(string ip)
        {
            var request = new UnityWebRequest("http://" + ip + "/olfy/status", "GET");
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();
            Parser(request.downloadHandler.text, ip);
            _sentRequest++;

        }

        public void Parser(string requestAnswer, string ip)
        {
            if (requestAnswer.Contains("DNSname"))
            {
                requestAnswer = requestAnswer.Replace("}", String.Empty);
                requestAnswer = requestAnswer.Replace("{", String.Empty);
                string[] properties = requestAnswer.Split(',');

                foreach (string property in properties)
                {
                    string[] keyValue = property.Split(':');
                    for (int i = 0; i < keyValue.Length; i++)
                    {
                        keyValue[i] = keyValue[i].Trim('"');
                    }

                    if (keyValue[0] == "DNSname")
                    {

                        deviceDictionary.Add(keyValue[1], ip);
                        Debug.Log("Success :    " + keyValue[1]);
                        if (!_once)
                        {
                            address = ip;
                            StartCoroutine(GetBattery());
                            _once = !_once;
                            isReady = true;
                        }

                    }
                }
            }
        }
        #endregion

        #region communication methods
        public void SendSmellToOlfy(int duration, string channel, int intensity, int freq, bool booster)
        {
            StartCoroutine(SmellCoroutine(duration, channel, intensity, freq, booster));
        }

        private IEnumerator SmellCoroutine(int duration, string channel, int intensity, int freq, bool booster)
        {
            var request = new UnityWebRequest("http://" + address + "/olfy/diffuse", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"duration\": " + duration + ", \"channel\": " + channel + ",\"intensity\": " + intensity + ",\"booster\": false}"); // Contenue du body avec les informations
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log("Status Code: " + request.responseCode);
        }

        private IEnumerator DeepSleep()
        {
            var request = new UnityWebRequest("http://" + address + "/olfy/deepsleep", "POST"); 
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log("Deep slepp Status Code: " + request.responseCode);
        }
        private IEnumerator GetFirmwareInformation()
        {
            var request = new UnityWebRequest("http://" + address + "/olfy/firmware", "GET");
            byte[] bodyRaw = Encoding.UTF8.GetBytes("{\"channel\": " + /*channel*/1 + ",\"duration\": " + /*duration */6000 + "}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json"); 

            yield return request.SendWebRequest(); 
            deviceInformation = request.responseCode.ToString();
            Debug.Log("Firmware: " + request.responseCode); 
        }
        public IEnumerator GetBattery()
        {
            var request = new UnityWebRequest("http://" + address + "/olfy/getbatt", "GET"); 

            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest(); 
            batteryLevel = request.responseCode.ToString();
            Debug.Log(request.downloadHandler.text); 
        }

        private IEnumerator StopOlfy()
        {
            var request = new UnityWebRequest("http://" + address + "/olfy/stop_all", "POST"); 
            byte[] bodyRaw = Encoding.UTF8.GetBytes("{}"); 
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json"); 

            yield return request.SendWebRequest(); 

            Debug.Log("status: " + request.responseCode); 
        }
        #endregion

    }





}
