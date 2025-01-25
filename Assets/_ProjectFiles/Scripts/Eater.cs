using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Eater : MonoBehaviour
{
    public Vector3 followPosition;
    [SerializeField]
    private SphereCollider m_eatCollider;
    private SoftBodySphere m_softBodySphere;
    List<Collider> m_toBeEatenColliders = new List<Collider>();

    private void Start()
    {
        m_softBodySphere = FindFirstObjectByType<SoftBodySphere>();
    }

    private void Update()
    {
        transform.position = followPosition;
        if (m_toBeEatenColliders.Count > 0) {
            List<Collider> eatenColliders = new List<Collider>();
            foreach (var collider in m_toBeEatenColliders)
            {
                if (!IsColliderFullyInside(collider))
                {
                    continue;
                }

                Eatable eater = collider.GetComponent<Eatable>();
                eater.isEaten = true;
                eater.followTarget = transform;
                eater.OnEaten += GrowBlob;
                eatenColliders.Add(collider);
            }
            foreach (var collider in eatenColliders)
            {
                m_toBeEatenColliders.Remove(collider);
            }
        }
    }

    public void SetSize(float size) {
        m_eatCollider.radius = size;
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
        m_toBeEatenColliders.Add(other);
    }

    private void GrowBlob() {
        //m_eatCollider.radius += .5f;
        transform.localScale += Vector3.one * 0.005f;
        m_softBodySphere.Grow();
    }
}
