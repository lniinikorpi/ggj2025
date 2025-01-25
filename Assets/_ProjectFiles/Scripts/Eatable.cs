using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Eatable : MonoBehaviour
{
    public bool isEaten = false;
    public Transform followTarget;
    private Rigidbody rb;
    private bool isDigesting = false;
    public float arbitarySize = .5f;

    public Action OnEaten;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isEaten && !isDigesting)
        {
            Eat();
        }
        if (isDigesting)
        {
            Vector3 dir = (followTarget.position - transform.position).normalized;
            transform.position += dir * Time.deltaTime * 5;
            rb.linearVelocity = Vector3.zero;
            if (transform.localScale.x < 0.1f)
            {
                OnEaten?.Invoke();
                OnEaten = null;
                Destroy(gameObject);
            }
            else
            {
                transform.localScale -= Vector3.one * Time.deltaTime * .5f;
            }
        }
    }

    void Eat() {
        isDigesting = true;
        if (rb.useGravity) { 
            rb.useGravity = false;
            GetComponent<Collider>().enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arbitarySize);
#if UNITY_EDITOR
        // Create a custom GUI style
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;
        style.fontSize = 12; // You can customize the font size if needed

        // Draw the text label at the object's position plus an offset
        Handles.Label(transform.position, arbitarySize.ToString(), style);
#endif
    }
}
