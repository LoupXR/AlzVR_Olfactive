using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Unity.VRTemplate {
    /// <summary>
    ///     Controls the steps in the in coaching card.
    /// </summary>
    public class StepManager : MonoBehaviour {
        [SerializeField] public TextMeshProUGUI m_StepButtonTextField;

        [SerializeField] private List<Step> m_StepList = new();

        private int m_CurrentStepIndex;

        public void Next() {
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(false);
            m_CurrentStepIndex = (m_CurrentStepIndex + 1) % m_StepList.Count;
            m_StepList[m_CurrentStepIndex].stepObject.SetActive(true);
            m_StepButtonTextField.text = m_StepList[m_CurrentStepIndex].buttonText;
        }

        [Serializable]
        private class Step {
            [SerializeField] public GameObject stepObject;

            [SerializeField] public string buttonText;
        }
    }
}