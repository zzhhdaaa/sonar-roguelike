using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

namespace AG_WebGLFPSAccelerator
{
    public class AG_inputManager1 : MonoBehaviour
    {
        private TextMeshProUGUI label;

        public GameObject text;
        public TMP_InputField inputField;

        public UnityEvent onValueChanged;

        public string valueString;

        public bool focused;

        void Update()
        {
            if (inputField.isFocused)
            {
                if (!focused)
                {
                    focused = true;
                    OnFocus();
                }
            }
            else
            {
                if (focused)
                {
                    focused = false;
                }
            }
        }

        public void OnFocus()
        {
            inputField.text = valueString;
            inputField.selectionAnchorPosition = 0;
            inputField.selectionFocusPosition = inputField.text.Length;
        }

        public void changeValueString(string string1)
        {
            float float1 = (float)Math.Round(float.Parse(string1) * 100f) / 100f;
            valueString = float1.ToString();

            if (inputField.isFocused)
            {
                label.text = "";
            }
            else
            {
                label.text = float1.ToString();
            }
        }

        void Start()
        {
            label = text.GetComponent<TextMeshProUGUI>();
            label.text = valueString;
        }

        public void inputFieldM()
        {
            string string1 = inputField.text;
            if (!string.IsNullOrEmpty(string1))
            {
                //int int1 = Convert.ToInt32(string1);

                valueString = string1;
                onValueChanged.Invoke();

                inputField.text = "";
            }
        }
    }
}