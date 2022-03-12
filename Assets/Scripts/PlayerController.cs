using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float decaySpeed = 0.3f;

    private Rigidbody playerRigidbody;
    Animator animator;

    public float playerEnergy = 1000;
    bool isGameOver = false;

    private Vector3 speedVector;

    public TextMeshProUGUI energyText;
    private SceneFlow sceneFlow;
    

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        sceneFlow = FindObjectOfType<SceneFlow>();
    }

    void Update()
    {
        playerEnergy = playerEnergy - decaySpeed;
        energyText.text = $"Energy: {playerEnergy}";
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        speedVector = new Vector3(horizontalInput * speed, playerRigidbody.velocity.y, verticalInput * speed);
        speedVector = Quaternion.Euler(0, 45, 0) * speedVector;
        playerRigidbody.velocity = speedVector;
        if (horizontalInput == 0 && verticalInput == 0)
        {
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("IsRunning", true);
        }
        if (playerEnergy < 0)
        {
            sceneFlow.ChangeGameOver();

        }
        
    }


    public void updatePlayerEnergy(float amount)
    {
        playerEnergy = playerEnergy + amount;
    }
}
