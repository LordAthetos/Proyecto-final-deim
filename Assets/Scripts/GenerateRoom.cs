using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour
{
    public GameObject BaseFloor_prefab;
    private GameObject baseFloor;
    
    // Controla si la habitacion tendra mas de una seccion (TBI)
    public bool isCompound = true;

    // Parametros de habitacion
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
        GenerateFloor(1);
        if (isCompound) 
        {
            //TBI
        }
    }

    // Genera numeros aleatorios segun los parametros maximos y minimos
    public void DefineParameters() 
    {
        roomSizeX = Random.Range(minSizeX, maxSizeX);
        roomSizeZ = Random.Range(minSizeZ, maxSizeZ);
        roomSizeY = Random.Range(2, maxSizeY);
        numDoors = Random.Range(minNumDoors, maxNumDoors);
    }
    public void GenerateFloor(int sectionNumber) 
    {
        // Listas para generar las paredes (TBI)
        List<GameObject> northPerimeter;
        List<GameObject> southPerimeter;
        List<GameObject> eastPerimeter;
        List<GameObject> westPerimeter;

        // Genera una cuadricula de prefabs de suelo, la taggea segun las propiedades y la emparenta a un nuevo game object root
        GameObject FloorSection = new GameObject($"FloorSectionRoot{sectionNumber}");
        startPos = new Vector3(roomSizeX / 2, roomSizeZ / 2, 0);
        for (int i = 0; i < roomSizeX; i++)
        {
            // comprovacion de si el suelo que va a generar es perimetro o esquina
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
