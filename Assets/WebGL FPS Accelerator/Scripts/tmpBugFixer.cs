using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace AG_WebGLFPSAccelerator
{
    [ExecuteInEditMode]
    public class tmpBugFixer : MonoBehaviour
    {
        [HideInInspector]
        public List<TMP_Text> TMP_Text_List;

        [HideInInspector]
        public List<TMP_Text> fixedDpi_List;

        [HideInInspector]
        public Vector2 Vector21;

        [HideInInspector]
        public Vector2 Vector22;

        public void m1()
        {
            foreach (var item in TMP_Text_List)
            {
                item.alignment = TextAlignmentOptions.TopLeft;
                item.fontSizeMin = 3;

                GameObject GameObject1 = item.gameObject;
                string name = GameObject1.name;
                if (name.StartsWith("Placeholder") || name.StartsWith("textInput") || name.StartsWith("Text Helper (1)"))
                {
                    RectTransform rectTransform1 = GameObject1.GetComponent<RectTransform>();

                    rectTransform1.offsetMax = Vector21;
                    rectTransform1.offsetMin = Vector22;
                }
            }

            foreach (var item in fixedDpi_List)
            {
                item.alignment = TextAlignmentOptions.Center;

                GameObject GameObject1 = item.gameObject;
                string name = GameObject1.name;
                if (name.StartsWith("Placeholder") || name.StartsWith("textInput") || name.StartsWith("Text Helper (1)"))
                {
                    RectTransform rectTransform1 = GameObject1.GetComponent<RectTransform>();

                    rectTransform1.offsetMax = Vector21;
                    rectTransform1.offsetMin = Vector22;
                }
            }
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
            {
                m1();
            }
        }

        void Update()
        {

        }
    }
}
