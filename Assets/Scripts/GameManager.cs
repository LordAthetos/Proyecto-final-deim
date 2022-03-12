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
    private int[][] adjacentRooms = new int[4][]{
        new int[64],
        new int[64],
        new int[64],
        new int[64]
    };
    private bool[][] isPopulated = new bool[4][]{
        new bool[64],
        new bool[64],
        new bool[64],
        new bool[64]
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
            adjacentRooms[0][0] = 1;
            roomPriority[adjacentRooms[0][0]] = 1;
            isPopulated[0][0] = true;

            AttachRoom(0, 2, 1);
            adjacentRooms[0][1] = 2;
            roomPriority[adjacentRooms[0][1]] = 1;
            isPopulated[0][1] = true;

            AttachRoom(0, 3, 2);
            adjacentRooms[0][2] = 3;
            roomPriority[adjacentRooms[0][2]] = 1;
            isPopulated[0][2] = true;

            AttachRoom(0, 4, 3);
            adjacentRooms[0][3] = 4;
            roomPriority[adjacentRooms[0][3]] = 1;
            isPopulated[0][3] = true;

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
        /*
        if (playerCurrentRoom == roomID)
        {
            // falsa alarma
        }
        else
        {
            playerCurrentRoom = roomID;
            if (isPopulated[roomID][0])
            {
                //adjacentRooms[roomID][0]
            }
            else
            {
                SpawnRoom();
                AttachRoom(roomID, currentRoomNumber - 1, 0);
            }
            if (isPopulated[roomID][1])
            {
                //adjacentRooms[roomID][0]
            }
            else
            {
                SpawnRoom();
                AttachRoom(roomID, currentRoomNumber - 1, 1);
            }
            if (isPopulated[roomID][2])
            {
                //adjacentRooms[roomID][0]
            }
            else
            {
                SpawnRoom();
                AttachRoom(roomID, currentRoomNumber - 1, 2);
            }
            if (isPopulated[roomID][3])
            {
                //adjacentRooms[roomID][0]
            }
            else
            {
                SpawnRoom();
                AttachRoom(roomID, currentRoomNumber - 1, 3);
            }
            
        }
        */
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
