using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarDot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float alpha = 1f;
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
        if (alpha > alphaMin)
        {
            alpha /= 1.04f;
            spriteRenderer.color = new Color(1, 1, 1, alpha);
        }
        else if (alpha < 0.01f)
        {
            SonarManager.instance.SonarUpdated.RemoveListener(this.UpdateDotColor);
            Destroy(gameObject);
        }
    }

    private void DownGradeColor()
    {
        if (alpha > alphaMin)
        {
            alpha = alphaMin;
        }
        spriteRenderer.color = new Color(1, 1, 1, alpha);
    }
}
