using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

namespace AG_WebGLFPSAccelerator
{
    public class AG_inputManager2 : MonoBehaviour
    {
        private TextMeshProUGUI label;
        public TextMeshProUGUI labelHelper;
        private Animator selectorAnimator;

        public GameObject text;
        public TMP_InputField inputField;

        public UnityEvent onValueChanged;

        public float value;
        public float float1;
        public float minValue;
        public float maxValue;

        public void changeValue(float float1)
        {
            value = (float)Math.Round(float1 * 100f) / 100f;

            if (inputField.isFocused)
            {
                if (label != null)
                    label.text = "";
            }
            else
            {
                if (label != null)
                    label.text = value.ToString();
            }
        }

        void Start()
        {
            selectorAnimator = gameObject.GetComponent<Animator>();
            label = text.GetComponent<TextMeshProUGUI>();
            label.text = value.ToString();
        }

        public void m1()
        {
            label.text = "";
        }

        public void inputFieldM()
        {
            string string1 = inputField.text;
            if (!string.IsNullOrEmpty(string1))
            {
                float float1 = float.Parse(string1);
                if (!(float1 > maxValue) && !(float1 < minValue))
                {
                    value = float1;
                    onValueChanged.Invoke();

                    label.text = value.ToString();
                    inputField.text = "";
                }
                else
                {
                    inputField.text = "";
                    label.text = value.ToString();
                }
            }
            else
            {
                label.text = value.ToString();
            }
        }

        public void PreviousClick()
        {
            labelHelper.text = label.text;

            float nextValue = (float)Math.Round((value - float1) * 100f) / 100f;

            if (nextValue < minValue)
            {
                value = maxValue;
            }
            else
            {
                value = nextValue;
            }

            onValueChanged.Invoke();
            label.text = value.ToString();

            selectorAnimator.Play(null);
            selectorAnimator.StopPlayback();
            selectorAnimator.Play("Previous");
        }

        public void ForwardClick()
        {
            labelHelper.text = label.text;

            if (value + float1 > maxValue)
            {
                value = minValue;
            }
            else
            {
                float v = value;
                v += float1;
                float v2 = (float)Math.Round(v * 100f) / 100f;
                value = v2;
            }

            onValueChanged.Invoke();
            label.text = value.ToString();

            selectorAnimator.Play(null);
            selectorAnimator.StopPlayback();
            selectorAnimator.Play("Forward");
        }
    }
}
