using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Rendering;

#if USING_URP
using UnityEngine.Rendering.Universal;
#endif

namespace AG_WebGLFPSAccelerator
{
    public class WebGLFPSAccelerator : MonoBehaviour
    {

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern float getDefaultDPR();

    [DllImport("__Internal")]
    private static extern void _setDPR(float float1);
#endif

        public static WebGLFPSAccelerator instance;

        [Header("___SETTINGS____________________________________________________________________________________________________________________")]
        [Read_Only_Field]
        public float dpi = 1;
        public bool dynamicResolutionSystem;
        public bool ShowHideUI;
        public float dpiDecrease = 0.050f;
        public float dpiIncrease = 0.050f;
        public int fpsMax = 35;
        public int fpsMin = 30;
        public float dpiMin = 0.6f;
        public float dpiMax = 1f;
        public float measurePeriod = 4f;
        public float fixedDPI = 1f;
        public bool useRenderScaleURP;

        [Header("___UI ELEMENTS____________________________________________________________________________________________________________________")]
        public TMP_Text dpi_TMP_Text;
        public AG_inputManager1 AG_inputManager1_fpsMax;
        public AG_inputManager1 AG_inputManager1_fpsMin;
        public AG_inputManager1 AG_inputManager1_dpiMax;
        public AG_inputManager1 AG_inputManager1_dpiMin;
        public AG_inputManager1 AG_inputManager1_dpiDecrease;
        public AG_inputManager1 AG_inputManager1_dpiIncrease;
        public AG_inputManager1 AG_inputManager1_measurePeriod;
        public AG_inputManager2 AG_inputManager2_fixedDPI;
        public Toggle Toggle1;
        public Toggle Toggle2;
        public GameObject dpiUIElement;
        public GameObject fixedDPIUIElement;
        public GameObject webglFpsAcceleratorInGameUI;

        [HideInInspector]
        public int fps;

        [HideInInspector]
        public float dpr = 0;

        [HideInInspector]
        private int m_FpsAccumulator = 0;

        [HideInInspector]
        public float m_FpsNextPeriod = 1.5f;

        //[HideInInspector]
        public float defaultDPR = 0f;

        [HideInInspector]
        public float lastDPR;

        [HideInInspector]
        public bool lastDynamicResolutionSystem;

        [HideInInspector]
        public bool lastShowHideUI;

        [HideInInspector]
        public bool urp;

#if USING_URP
        private UniversalRenderPipelineAsset urpAsset;
#endif

        private bool wait = true;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            requestDefaultDPR();

            Toggle1.isOn = dynamicResolutionSystem;
            Toggle2.isOn = useRenderScaleURP;
            webglFpsAcceleratorInGameUI.transform.parent.gameObject.GetComponent<Canvas>().enabled = ShowHideUI;

            Invoke("waitForOneSecond", 1);

#if USING_URP
            var rpAsset = GraphicsSettings.renderPipelineAsset;
            urpAsset = (UniversalRenderPipelineAsset)rpAsset;

            urp = true;
            Toggle2.transform.parent.gameObject.SetActive(true);
            RectTransform rt = webglFpsAcceleratorInGameUI.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, 580);
#endif
        }

        public void __setDPR(float float1)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        _setDPR(float1);
#endif
        }

        public void requestDefaultDPR()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        defaultDPR = getDefaultDPR();
#endif
        }

        public void Toggle1Event()
        {
            dynamicResolutionSystem = Toggle1.isOn;
        }

        public void Toggle2Event()
        {
            useRenderScaleURP = Toggle2.isOn;
        }

        public void getAverageFPS()
        {
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup >= m_FpsNextPeriod)
            {
                fps = (int)(m_FpsAccumulator / measurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod = Time.realtimeSinceStartup + measurePeriod;

                dynamicResolutionSystemMethod();
            }
        }

        public void dynamicResolutionSystemMethod()
        {
            if (fps > fpsMax)
            {
                dpi += dpiIncrease;
            }
            else if (fps < fpsMin)
            {
                dpi -= dpiDecrease;
            }
            dpi = Mathf.Clamp(dpi, dpiMin, dpiMax);

            if (useRenderScaleURP && urp)
            {
                dpr = dpi;

                if (dpr != lastDPR)
                {
#if USING_URP
                    urpAsset.renderScale = dpr;
#endif
                }
            }
            else
            {
                dpr = dpi * defaultDPR;

                if (dpr != lastDPR)
                {
                    __setDPR(dpr);
                }
            }

            lastDPR = dpr;
        }

        void Update()
        {
            if (defaultDPR != 0 && !wait)
            {
                if (!dynamicResolutionSystem)
                {
                    if (dynamicResolutionSystem != lastDynamicResolutionSystem || dpr == 0)
                    {
                        lastDynamicResolutionSystem = dynamicResolutionSystem;

                        fixedDPIUIElement.SetActive(true);
                        dpiUIElement.SetActive(false);

                        lastDPR = 0;
                    }

                    dpi = fixedDPI;

                    if (useRenderScaleURP && urp)
                    {
                        dpr = dpi;

                        if (dpr != lastDPR)
                        {
#if USING_URP
                            urpAsset.renderScale = dpr;
#endif
                        }
                    }
                    else
                    {
                        dpr = dpi * defaultDPR;

                        if (dpr != lastDPR)
                        {
                            __setDPR(dpr);
                        }
                    }

                    lastDPR = dpr;

                }
                else
                {
                    if (dynamicResolutionSystem != lastDynamicResolutionSystem || dpr == 0)
                    {
                        lastDynamicResolutionSystem = dynamicResolutionSystem;
                        m_FpsNextPeriod = Time.realtimeSinceStartup + measurePeriod;
                        m_FpsAccumulator = 0;

                        fixedDPIUIElement.SetActive(false);
                        dpiUIElement.SetActive(true);

                        lastDPR = 0;
                    }

                    getAverageFPS();
                }
            }

            dpi = (float)Math.Round(dpi * 100f) / 100f;
            dpi_TMP_Text.text = dpi.ToString();

            updateUI();

            if (ShowHideUI != lastShowHideUI)
            {
                webglFpsAcceleratorInGameUI.transform.parent.gameObject.GetComponent<Canvas>().enabled = ShowHideUI;
                lastShowHideUI = ShowHideUI;
            }
        }

        public void m_fpsMax()
        {
            fpsMax = int.Parse(AG_inputManager1_fpsMax.valueString);
        }

        public void m_fpsMin()
        {
            fpsMin = int.Parse(AG_inputManager1_fpsMin.valueString);
        }

        public void m_dpiMax()
        {
            dpiMax = float.Parse(AG_inputManager1_dpiMax.valueString);
        }

        public void m_dpiMin()
        {
            dpiMin = float.Parse(AG_inputManager1_dpiMin.valueString);
        }

        public void m_dpiDecrease()
        {
            dpiDecrease = float.Parse(AG_inputManager1_dpiDecrease.valueString);
        }

        public void m_dpiIncrease()
        {
            dpiIncrease = float.Parse(AG_inputManager1_dpiIncrease.valueString);
        }

        public void m_measurePeriod()
        {
            measurePeriod = float.Parse(AG_inputManager1_measurePeriod.valueString);
        }

        public void m_fixedDPI()
        {
            fixedDPI = AG_inputManager2_fixedDPI.value;
        }

        public void updateUI()
        {
            AG_inputManager1_fpsMax.changeValueString(fpsMax.ToString());
            AG_inputManager1_fpsMin.changeValueString(fpsMin.ToString());
            AG_inputManager1_dpiMin.changeValueString(dpiMin.ToString());
            AG_inputManager1_dpiMax.changeValueString(dpiMax.ToString());
            AG_inputManager1_dpiIncrease.changeValueString(dpiIncrease.ToString());
            AG_inputManager1_dpiDecrease.changeValueString(dpiDecrease.ToString());
            AG_inputManager1_measurePeriod.changeValueString(measurePeriod.ToString());

            if (fixedDPIUIElement.activeSelf)
                AG_inputManager2_fixedDPI.changeValue(fixedDPI);
        }

        public void waitForOneSecond()
        {
            wait = false;
        }
    }
}
