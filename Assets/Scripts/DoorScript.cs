using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public int room;
    public float rotation;
    public int doorDirection;
    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = FindObjectOfType<GameManager>();
        room = GameManager.currentRoomNumber -1;
        rotation = gameObject.transform.rotation.eulerAngles.y;
        switch (rotation)
        {
            case 0:
                doorDirection = 2;
                break;
            case 90:
                doorDirection = 1;
                break;
            case 180:
                doorDirection = 0;
                break;
            case 270:
                doorDirection = 3;
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        gameManagerScript.HasPlayerCrossed(room, doorDirection);
        Debug.Log($"puerta {doorDirection} de habitacion {room} ha sido activada");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
