using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayer : MonoBehaviour
{
    private float speed = 3.5f;
    public Animator animator;

    

    void Start()
    {
        
        animator = GetComponent<Animator>();
        
    }

    
    void Update()
    {
        
        float horizontal = Input.GetAxis("Horizontal2");
        float vertical = Input.GetAxis("Vertical2");
 
        Vector3 newPos = new Vector3(horizontal, 0, vertical);
        newPos = Quaternion.Euler(0, 45, 0) * newPos;
        Vector3 currentPos = gameObject.transform.position;
        Vector3 facePos = currentPos + newPos;
        transform.LookAt(facePos);
        
    }
}
