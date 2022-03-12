using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] roomlist = new GameObject[64];
    private Vector3[] roomCoordList = new Vector3[64];
    private int[] roomPriority = new int[64]; /*
                                                 0 = main active room
                                                 1 = adjacent to main
                                                 2 = chance of swap
                                                 3 = in  *T H E  P I L E*
                                               */

    //private List<Vector3>[] doorCoordList = new List<Vector3>[4];
    private Vector3[][] doorCoordList = new Vector3[4][]{
        new Vector3[64],
        new Vector3[64],
        new Vector3[64],
        new Vector3[64]
    };
    public static int currentRoomNumber = 0;
    
    public GameObject Room;

    bool initialSpawn = false;

    int startRoomNum = 9;

    GameObject newRoom;

    int playerCurrentRoom;

    // Start is called before the first frame update
    void Start()
    {
        playerCurrentRoom = 0;
        roomPriority[0] = 0;
        SpawnInitial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnInitial()
    {
        if (currentRoomNumber < startRoomNum)
        {
            SpawnRoom();
        }
        else
        {
            initialSpawn = true;
            AttachRoom(0, 1, 0);
            AttachRoom(0, 2, 1);
            AttachRoom(0, 3, 2);
            AttachRoom(0, 4, 3);

        }
    }

    public void SpawnRoom()
    {
        newRoom = Instantiate(Room, Vector3.zero, Quaternion.identity);
    }
    public void StoreRoomData(int roomNumber, Vector3[] roomData)
    {
        doorCoordList[0][roomNumber] = roomData[0];
        doorCoordList[1][roomNumber] = roomData[1];
        doorCoordList[2][roomNumber] = roomData[2];
        doorCoordList[3][roomNumber] = roomData[3];

        roomlist[roomNumber] = newRoom;
        roomCoordList[roomNumber] = new Vector3(0, -roomNumber * 10, 0);
        //Debug.Log(roomCoordList[roomNumber]);
        newRoom.transform.position = roomCoordList[roomNumber];
        currentRoomNumber++;

        if (!initialSpawn)
        {
            SpawnInitial();
        }
        
        
    }
    public void HasPlayerCrossed(int roomID, int doorID)
    {
        if (playerCurrentRoom == roomID)
        {
            // falsa alarma
        }
        else
        {
            playerCurrentRoom = roomID;
        }
    }
    public void RoomSwap()
    {

    }
    public void AttachRoom(int roomDestination, int roomToMove, int direction)
    {

        Vector3 offset = Vector3.zero;
        int directionInv = 0;
        switch (direction)
        {
            case 0:
                directionInv = 2;
                offset = new Vector3(-1, 0, 2);
                break;
            case 1:
                directionInv = 3;
                offset = new Vector3(0, 0, -1);
                break;
            case 2:
                directionInv = 0;
                offset = new Vector3(1, 0, -2);
                break;
            case 3:
                directionInv = 1;
                offset = new Vector3(0, 0, 1);
                break;
        }
        GameObject room = roomlist[roomToMove];
        
        Vector3 totalOffset = doorCoordList[direction][roomDestination] - doorCoordList[directionInv][roomToMove] + offset;
        room.transform.position = totalOffset;
    }
}
