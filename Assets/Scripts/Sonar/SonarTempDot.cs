using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarTempDot : MonoBehaviour
{
    private void OnEnable()
    {
        SonarManager.instance.SonarUpdated.AddListener(DestroySelf);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
