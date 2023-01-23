using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SonarManager : MonoBehaviour
{
    public static SonarManager instance;

    [SerializeField] private float distance;
    [SerializeField] private int dotCount = 20;

    private List<Vector3> directions = new List<Vector3>();

    public UnityEvent SonarUpdated;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        CalDirection();
    }

    public void SonarDetect(Vector3 originPos)
    {
        SonarUpdated?.Invoke();
        foreach (Vector3 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(originPos, direction, distance);

            if (hit.collider != null)
            {
                Instantiate(Resources.Load<GameObject>("SonarDot"), hit.point, Quaternion.identity, this.transform).name = "SonarDot";
            }
            else
            {
                Instantiate(Resources.Load<GameObject>("SonarTempDot"), originPos + direction*distance, Quaternion.identity, this.transform).name = "SonarTempDot";
            }
        }
    }

    private void CalDirection()
    {
        float angle = 0f;
        for (int i = 0; i < dotCount; i++)
        {
            var direction = Quaternion.AngleAxis(angle, transform.forward) * transform.up;
            directions.Add(direction);
            print(direction);
            angle += 360/dotCount;
        }
    }
}
