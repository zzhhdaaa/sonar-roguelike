using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarDot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float alpha = 1f;
    private float alphaDivider = 1.04f;
    private float alphaMin = 0.06f;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SonarManager.instance.SonarUpdated.AddListener(UpdateDotColor);
        SonarManager.instance.SonarDownGrade.AddListener(DownGradeColor);
        //SonarManager.instance.SonarUpdated += UpdateDotColor;
    }

    private void UpdateDotColor()
    {
        if (alpha >= alphaMin)
        {
            alpha /= alphaDivider;
            spriteRenderer.color = new Color(1, 1, 1, alpha);
        }
        else if (alpha < 0.01f)
        {
            SonarManager.instance.SonarUpdated.RemoveListener(this.UpdateDotColor);
            SonarManager.instance.SonarDownGrade.RemoveListener(this.DownGradeColor);
            Destroy(gameObject);
        }
    }

    private void DownGradeColor()
    {
        if (alpha >= alphaMin)
        {
            alpha = alphaMin;
        }
        else if(Random.value < 0.5f)
        {
            SonarManager.instance.SonarUpdated.RemoveListener(this.UpdateDotColor);
            SonarManager.instance.SonarDownGrade.RemoveListener(this.DownGradeColor);
            Destroy(gameObject);
            return;
        }

        alphaMin /= 2f;
        //alphaDivider *= 2f;
        Debug.Log(alphaMin);
        //Debug.Log(alphaDivider);
        spriteRenderer.color = new Color(1, 1, 1, alpha);
    }
}
