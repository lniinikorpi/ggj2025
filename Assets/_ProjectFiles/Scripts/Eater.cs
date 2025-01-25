using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Eater : MonoBehaviour
{
    public Vector3 followPosition;
    [SerializeField]
    private SphereCollider m_eatCollider;
    private SoftBodySphere m_softBodySphere;
    List<Eatable> m_toBeEaten = new List<Eatable>();

    private void Start()
    {
        m_softBodySphere = FindFirstObjectByType<SoftBodySphere>();
        /*for(int i = 0; i < 100; i++)
        {
            GrowBlob();
        }*/
    }

    private void Update()
    {
        transform.position = followPosition;
        if (m_toBeEaten.Count > 0) {
            List<Eatable> eated = new List<Eatable>();
            foreach (var collider in m_toBeEaten)
            {
                Eatable eater = collider.GetComponent<Eatable>();
                /*if (!IsColliderFullyInside(collider))
                {
                    continue;
                }*/
                if(!IsInside(eater))
                {
                    eated.Add(collider);
                    continue;
                }
                eater.isEaten = true;
                eater.followTarget = transform;
                eater.OnEaten += () =>
                {
                    GrowBlob();
                    GameManager.Instance.RemoveEatable(eater);
                };
                eated.Add(collider);
            }
            foreach (var eaten in eated)
            {
                m_toBeEaten.Remove(eaten);
            }
        }
    }

    public void SetSize(float size) {
        m_eatCollider.radius = size;
    }

    bool IsInside(Eatable eatable) { 
        if(eatable == null)
        {
            return false;
        }
        if(eatable.arbitarySize < transform.localScale.x)
        {
            return true;
        }
        return false;
    }

    bool IsColliderFullyInside(Collider eatCollider)
    {
        // Get the bounds of both colliders
        Bounds outerBounds = m_eatCollider.bounds;
        Bounds innerBounds = eatCollider.bounds;

        // Get the corners of the inner collider's bounds
        Vector3[] innerCorners = GetBoundsCorners(innerBounds);

        // Check if all corners are within the outer collider's bounds
        foreach (var corner in innerCorners)
        {
            if (!outerBounds.Contains(corner))
            {
                return false; // If any corner is outside, return false
            }
        }
        return true; // All corners are inside
    }
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
        Handles.Label(transform.position, transform.localScale.x.ToString(),style);
        
    }
    Vector3[] GetBoundsCorners(Bounds bounds)
    {
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;

        return new Vector3[]
        {
            new Vector3(min.x, min.y, min.z), // Bottom-left-front
            new Vector3(max.x, min.y, min.z), // Bottom-right-front
            new Vector3(min.x, max.y, min.z), // Top-left-front
            new Vector3(max.x, max.y, min.z), // Top-right-front
            new Vector3(min.x, min.y, max.z), // Bottom-left-back
            new Vector3(max.x, min.y, max.z), // Bottom-right-back
            new Vector3(min.x, max.y, max.z), // Top-left-back
            new Vector3(max.x, max.y, max.z)  // Top-right-back
        };
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("NotEatable"))
        {
            return;
        }
        Eatable eatable = other.GetComponent<Eatable>();
        if (eatable == null || eatable.isEaten)
        {
            return;
        }
        if(m_toBeEaten.Contains(eatable))
        {
            return;
        }
        m_toBeEaten.Add(eatable);
    }

    private void GrowBlob() {
        //m_eatCollider.radius += .5f;
        transform.localScale += Vector3.one * 0.005f;
        m_softBodySphere.Grow();
    }
}
