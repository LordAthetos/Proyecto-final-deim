using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour
{
    public GameObject BaseFloor_prefab;
    private GameObject baseFloor;
    

    public bool isCompound = true;

    private int minSizeX = 5;
    private int minSizeZ = 5;
    private int maxSizeX = 10;
    private int maxSizeZ = 10;
    private int maxSizeY = 5;
    public int maxNumDoors = 5;
    public int minNumDoors = 2;
    private Vector3 startPos;


    public int roomSizeX;
    public int roomSizeZ;
    public int roomSizeY;
    public int numDoors;
    

    // Start is called before the first frame update
    void Start()
    {
        DefineParameters();
        GenerateFloor();
        if (isCompound) 
        {

        }
    }


    public void DefineParameters() 
    {
        roomSizeX = Random.Range(minSizeX, maxSizeX);
        roomSizeZ = Random.Range(minSizeZ, maxSizeZ);
        roomSizeY = Random.Range(2, maxSizeY);
        numDoors = Random.Range(minNumDoors, maxNumDoors);
    }
    public void GenerateFloor() 
    {
        GameObject FloorSection = new GameObject("FloorSection");
        startPos = new Vector3(roomSizeX / 2, roomSizeZ / 2, 0);
        for (int i = 0; i < roomSizeX; i++)
        {
            for (int j = 0; j < roomSizeZ; j++)
            {
                if (i == 0 || j == 0 || i == roomSizeX-1 || j == roomSizeZ-1)
                {
                    if(i == 0 && j == 0 || i == 0 && j == roomSizeZ-1 || i == roomSizeX-1 && j == 0 || i == roomSizeX-1 && j == roomSizeZ-1) 
                    {
                        baseFloor = Instantiate(BaseFloor_prefab, startPos + new Vector3(i, 0f, j), Quaternion.identity);
                        baseFloor.transform.SetParent(FloorSection.transform);
                        baseFloor.tag = "FloorPerimeterCorner";
                    }
                    else 
                    {
                        baseFloor = Instantiate(BaseFloor_prefab, startPos + new Vector3(i, 0f, j), Quaternion.identity);
                        baseFloor.transform.SetParent(FloorSection.transform);
                        baseFloor.tag = "FloorPerimeter";
                    }
                    
                }
                else 
                {
                    baseFloor = Instantiate(BaseFloor_prefab, startPos + new Vector3(i, 0f, j), Quaternion.identity);
                    baseFloor.transform.SetParent(FloorSection.transform);
                    baseFloor.tag = "Floor";
                }
            }
        }
    }



}
