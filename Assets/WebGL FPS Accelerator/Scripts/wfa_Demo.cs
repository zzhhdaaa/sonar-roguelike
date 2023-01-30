using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using UnityEditor;

namespace AG_WebGLFPSAccelerator
{
    public class wfa_Demo : MonoBehaviour
    {
        public static wfa_Demo instance;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern bool isAndroid2();

        [DllImport("__Internal")]
        public static extern bool isiOS2();
#endif
        
        [HideInInspector]
        public GameObject warningText;
        
        [HideInInspector]
        public GameObject requiredSettings;
        
        [HideInInspector]
        public GameObject postProcessVolume;
        
        private float shadowDistanceTemp;

        [HideInInspector]
        public bool isiOS;
        [HideInInspector]
        public bool isAndroid;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {

#if UNITY_2021_1_OR_NEWER && USING_URP
            VolumeProfile VolumeProfile1 = Resources.Load<VolumeProfile>("URP Volume Profile 2");
            postProcessVolume.GetComponent<Volume>().profile = VolumeProfile1;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            isiOS = isiOS2();
            isAndroid = isAndroid2();

            if (isiOS || isAndroid)
            {
                GameObject webglFpsAcceleratorInGameUI = WebGLFPSAccelerator.instance.webglFpsAcceleratorInGameUI;
                RectTransform rt = webglFpsAcceleratorInGameUI.GetComponent<RectTransform>();
                rt.localScale = new Vector3(0.85f, 0.85f, 1);
            }
#endif

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += playModeStateChanged;
#endif

            QualitySettings.vSyncCount = 0;
            QualitySettings.shadowDistance = 300f;

#if USING_URP
            var rpAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
            var urpAsset = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)rpAsset;

            shadowDistanceTemp = urpAsset.shadowDistance;
            urpAsset.shadowDistance = 200f;
#endif

            m1();
            generateMap.instance.m1();
        }

#if UNITY_EDITOR
        public void playModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
#if USING_URP
                var rpAsset = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
                var urpAsset = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)rpAsset;

                urpAsset.shadowDistance = shadowDistanceTemp;
#endif
            }
        }
#endif

        public void m1()
        {
#if UNITY_EDITOR
            if (warningText)
                warningText.SetActive(true);
#endif

            if (requiredSettings)
            {
#if !UNITY_EDITOR
                requiredSettings.SetActive(false);
#endif
            }
        }
    }
}