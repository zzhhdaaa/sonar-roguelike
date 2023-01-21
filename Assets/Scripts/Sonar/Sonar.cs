using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private int dotCount = 20;

    private List<Vector3> directions = new List<Vector3>();

    void Start()
    {
        CalDirection();
    }

    public void SonarDetect()
    {
        foreach (Vector3 direction in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);
            
            Ray ray = new Ray(transform.position, direction);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red);

            if (hit.collider != null)
            {
                Instantiate(Resources.Load<GameObject>("SonarDot"), hit.point, Quaternion.identity).name = "SonarDot";
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
