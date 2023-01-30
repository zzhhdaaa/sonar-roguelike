using UnityEditor;
using UnityEngine;
#if USING_BRP && UNITY_EDITOR
using UnityEngine.Rendering.PostProcessing;
#endif

[ExecuteInEditMode]
public class ppBugFixer : MonoBehaviour
{
#if USING_BRP && UNITY_EDITOR

    [HideInInspector]
    public PostProcessResources postProcessResources;

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            if (postProcessResources == null)
            {
                postProcessResources = (PostProcessResources)AssetDatabase.LoadAssetAtPath("Packages/com.unity.postprocessing/PostProcessing/PostProcessResources.asset", typeof(PostProcessResources));
            }

            PostProcessLayer postProcessLayer = Camera.main.gameObject.GetComponent<PostProcessLayer>();
            postProcessLayer.Init(postProcessResources);
        }
    }
#endif
}