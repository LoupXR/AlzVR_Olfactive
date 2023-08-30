using System.Globalization;
using UnityEngine;
using Olfy;

namespace OlfyDemo {
    public class Demo : MonoBehaviour
    {
        public GameObject connectionEstablished;
        public GameObject waitingForConnection;
        public GameObject buses;
        public TMPro.TextMeshProUGUI ipText;
        public TMPro.TextMeshProUGUI batteryText;

        public TMPro.TextMeshProUGUI intensityText;
        public TMPro.TextMeshProUGUI durationText;
        public TMPro.TextMeshProUGUI frequencyText;
        private int _intensityValue = 50;
        private float _durationValue = 2f;
        private int _frequencyValue = 110000;

        void Update()
        {
            if(OlfyManager.Instance.isReady && connectionEstablished.activeInHierarchy == false)
            {
                connectionEstablished.SetActive(true);
                buses.SetActive(true);
                waitingForConnection.SetActive(false);
                ipText.text = "IP : " + OlfyManager.Instance.address;
                batteryText.text = OlfyManager.Instance.batteryLevel;
            }
        }

        public void SendToOlfy(string buse)
        {
            OlfyManager.Instance.SendSmellToOlfy((int)_durationValue * 1000, buse, _intensityValue, _frequencyValue, false);
        }

        public void SetIntensity(float i)
        {
            _intensityValue = (int)i;
            intensityText.text = i.ToString(CultureInfo.InvariantCulture);
        }
        public void SetDuration(float d)
        {
            _durationValue = d;
            float dur = Mathf.Round(_durationValue);
            durationText.text = dur.ToString(CultureInfo.InvariantCulture);
        }
        public void SetFrequency(float f)
        {
            _frequencyValue = (int)f;
            int freq = (int)f / 1000;
            frequencyText.text = freq.ToString() + " k";
        }

    }
}
