using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public GameObject projectile;
    private PlayerController playerControllerScript;
    public AudioClip shootClip;
    private AudioSource cameraAudioSource;
    private AudioSource playerAudioSource;
    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
        playerControllerScript.updatePlayerEnergy(-100);
        playerAudioSource = GetComponent<AudioSource>();
        cameraAudioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {

            Instantiate(projectile, transform.position, projectile.transform.rotation = transform.rotation);
            cameraAudioSource.PlayOneShot(shootClip, 1);
        }
    }
}
