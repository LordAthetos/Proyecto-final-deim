using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    private PlayerController playerControllerScript;

    public float speed = 15f;
    public ParticleSystem explosionParticleSystem;
    private AudioSource cameraAudioSource;
    private AudioSource playerAudioSource;
    public AudioClip shootClip;
    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
        playerAudioSource = GetComponent<AudioSource>();
        cameraAudioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        cameraAudioSource.PlayOneShot(shootClip, 1);
    }
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Generator"))
        {
            explosionParticleSystem.Play();
            Destroy(other.gameObject);
            playerControllerScript.updatePlayerEnergy(200);
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
   
}
