using System;
using System.Collections;
using UnityEngine;

public class Eatable : MonoBehaviour
{
    public bool isEaten = false;
    public Transform followTarget;
    [SerializeField]
    private Rigidbody rb;
    private bool isDigesting = false;

    public Action OnEaten;

    private void Update()
    {
        if (isEaten)
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
                Destroy(gameObject);
            }
            else
            {
                transform.localScale -= Vector3.one * Time.deltaTime * .5f;
            }
        }
    }

    void Eat() {
        if(isEaten)
        {
            isEaten = false;
            isDigesting = true;
        }
        if (rb.useGravity) { 
            rb.useGravity = false;
            GetComponent<Collider>().enabled = false;
        }
    }
}
