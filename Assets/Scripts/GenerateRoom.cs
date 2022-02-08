using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// Pesos 
public class Choice
{
    public string choiceName;
    public int weight;
    public Choice(string newChoiceName, int newChoiceWeight)
    {
        choiceName = newChoiceName;
        weight = newChoiceWeight;
    }
}



public class GenerateRoom : MonoBehaviour
{
    List<Choice> choices = new List<Choice>();
    int totalWeight;

    public GameObject BaseFloor_prefab;
    public GameObject BaseFloor_perimeter;
    public GameObject BaseFloor_corner;
    private GameObject baseFloor;

    private GameObject templateFloor;

    // Controla si la habitacion tendra mas de una seccion (TBI)
    public bool isCompound = true;

    // Parametros de habitacion
    private int minSizeX = 3;
    private int minSizeZ = 3;
    private int maxSizeX = 7;
    private int maxSizeZ = 7;
    private int maxSizeY = 5;
    public int maxNumDoors = 5;
    public int minNumDoors = 2;
    private Vector3 offset;
    private int maxNumSections = 9;
    public int templateRange = 3;

    public int roomSizeX;
    public int roomSizeZ;
    public int roomSizeY;
    public int numDoors;
    public int numSections;

    public Vector3[] sectionCoordinates = new Vector3[10];

    
    
    // east = 0
    // south = 1
    // west = 2
    // north = 3
    int[][] wallValue = new int[4][];



    //public GameObject[] templatePrefabs = new GameObject[10];
    //private int sectionCounter;


    // Start is called before the first frame update
    void Start()
    {

        choices.Add(new Choice("Wall", 2));
        choices.Add(new Choice("Door", 2));
        choices.Add(new Choice("OpenSpace", 2));
        foreach (Choice entry in choices)
        {
            totalWeight += entry.weight;
        }

        DefineParameters();
        GenerateTemplate();
        for (int sectionCounter = 0; sectionCounter <= maxNumSections; sectionCounter += 1)
        {
            GenerateFloor(sectionCounter);
        }

    }

    // Genera numeros aleatorios segun los parametros maximos y minimos
    public void DefineParameters()
    {
        roomSizeX = Random.Range(minSizeX, maxSizeX) * 2 - 1;
        roomSizeZ = Random.Range(minSizeZ, maxSizeZ) * 2 - 1;
        roomSizeY = Random.Range(2, maxSizeY);
        numDoors = Random.Range(minNumDoors, maxNumDoors);
        numSections = Random.Range(1, maxNumSections);
    }


    void GenerateTemplate()
    {
        templateFloor = Instantiate(BaseFloor_prefab, new Vector3(0, 0, 0), Quaternion.identity);
        templateFloor.tag = "Template";
        Vector3 currentTile = new Vector3(0, 0, 0);
        sectionCoordinates[0] = new Vector3(currentTile.x, currentTile.y, currentTile.z);
        Vector3 adjacentOperator = new Vector3(0, 0, 0);

        bool isAdjacent = false;
        bool notRepeated = false;



        for (int sectionCounter = 1; sectionCounter <= maxNumSections; sectionCounter += 1)
        {
            while (!isAdjacent || !notRepeated)
            {
                currentTile = new Vector3(Random.Range(-templateRange, templateRange), 0, Random.Range(-templateRange, templateRange));

                //+x 
                adjacentOperator = new Vector3(currentTile.x + 1, 0, currentTile.z);
                if (sectionCoordinates.Contains(adjacentOperator))
                {
                    isAdjacent = true;
                }
                //+z 
                adjacentOperator = new Vector3(currentTile.x, 0, currentTile.z + 1);
                if (sectionCoordinates.Contains(adjacentOperator))
                {
                    isAdjacent = true;
                }
                //-x 
                adjacentOperator = new Vector3(currentTile.x - 1, 0, currentTile.z);
                if (sectionCoordinates.Contains(adjacentOperator))
                {
                    isAdjacent = true;
                }
                //-z 
                adjacentOperator = new Vector3(currentTile.x, 0, currentTile.z - 1);
                if (sectionCoordinates.Contains(adjacentOperator))
                {
                    isAdjacent = true;
                }
                if (!sectionCoordinates.Contains(currentTile))
                {
                    notRepeated = true;
                }


            }
            sectionCoordinates[sectionCounter] = new Vector3(currentTile.x, currentTile.y, currentTile.z);
            templateFloor = Instantiate(BaseFloor_prefab, currentTile, Quaternion.identity);
            templateFloor.tag = "Template";
            isAdjacent = false;
            notRepeated = false;

        }
        for (int sectionCounter = 0; sectionCounter <= maxNumSections; sectionCounter += 1)
        {

            currentTile = sectionCoordinates[sectionCounter];
            currentTile = new Vector3(Random.Range(-templateRange, templateRange), 0, Random.Range(-templateRange, templateRange));

            // 0 = perimetro externo

            //+x  east
            adjacentOperator = new Vector3(currentTile.x + 1, 0, currentTile.z);
            if (sectionCoordinates.Contains(adjacentOperator))
            {
                
            }
            else
            {
                wallValue[0][sectionCounter] = 0;
            }
            //-z  south
            adjacentOperator = new Vector3(currentTile.x, 0, currentTile.z - 1);
            if (sectionCoordinates.Contains(adjacentOperator))
            {

            }
            else
            {

            }
            //-x  west
            adjacentOperator = new Vector3(currentTile.x - 1, 0, currentTile.z);
            if (sectionCoordinates.Contains(adjacentOperator))
            {
                
            }
            else
            {

            }
            //+z  north
            adjacentOperator = new Vector3(currentTile.x, 0, currentTile.z + 1);
            if (sectionCoordinates.Contains(adjacentOperator))
            {

            }
            else
            {

            }

        }

    }

    public void GenerateFloor(int sectionNumber)
    {
        int northWall = Random.Range(0, 5);
        int southWall = Random.Range(0, 5);
        int eastWall = Random.Range(0, 5);
        int westWall = Random.Range(0, 5);


        // Listas para generar las paredes (TBI)
        List<GameObject> northPerimeter = new List<GameObject>();
        List<GameObject> southPerimeter = new List<GameObject>();
        List<GameObject> eastPerimeter = new List<GameObject>();
        List<GameObject> westPerimeter = new List<GameObject>();

        Vector3 moveToTemplate = new Vector3(0, 0, 0);

        // Genera una cuadricula de prefabs de suelo, la taggea segun las propiedades y la emparenta a un nuevo game object root
        GameObject FloorSection = new GameObject($"FloorSectionRoot{sectionNumber}");
        offset = new Vector3(roomSizeX / 2, 0, roomSizeZ / 2);
        for (int i = 0; i < roomSizeX; i++)
        {
            for (int j = 0; j < roomSizeZ; j++)
            {
                // comprovacion de si el suelo que va a generar es perimetro o esquina
                if (i == 0 || j == 0 || i == roomSizeX - 1 || j == roomSizeZ - 1)
                {
                    if (i == 0 && j == 0 || i == 0 && j == roomSizeZ - 1 || i == roomSizeX - 1 && j == 0 || i == roomSizeX - 1 && j == roomSizeZ - 1)
                    {
                        baseFloor = Instantiate(BaseFloor_corner, new Vector3(i, 0f, j), Quaternion.identity);
                        baseFloor.transform.position = new Vector3(baseFloor.transform.position.x, baseFloor.transform.position.y, baseFloor.transform.position.z) - offset;
                        baseFloor.transform.SetParent(FloorSection.transform);
                        baseFloor.tag = "FloorPerimeterCorner";

                    }
                    else
                    {
                        baseFloor = Instantiate(BaseFloor_perimeter, new Vector3(i, 0f, j), Quaternion.identity);
                        baseFloor.transform.position = new Vector3(baseFloor.transform.position.x, baseFloor.transform.position.y, baseFloor.transform.position.z) - offset;
                        baseFloor.transform.SetParent(FloorSection.transform);
                        baseFloor.tag = "FloorPerimeter";
                        if (i == 0)
                        {
                            eastPerimeter.Add(baseFloor);
                        }
                        if (j == 0)
                        {
                            southPerimeter.Add(baseFloor);
                        }
                        if (i == roomSizeX)
                        {
                            westPerimeter.Add(baseFloor);
                        }
                        if (i == roomSizeZ)
                        {
                            northPerimeter.Add(baseFloor);
                        }
                    }

                }
                else
                {
                    baseFloor = Instantiate(BaseFloor_prefab, new Vector3(i, 0f, j), Quaternion.identity);
                    baseFloor.transform.position = new Vector3(baseFloor.transform.position.x, baseFloor.transform.position.y, baseFloor.transform.position.z) - offset;
                    baseFloor.transform.SetParent(FloorSection.transform);
                    baseFloor.tag = "Floor";
                }
            }
        }
        moveToTemplate = sectionCoordinates[sectionNumber];
        FloorSection.transform.position = new Vector3(moveToTemplate.x * roomSizeX, 0, moveToTemplate.z * roomSizeZ);

    }
    public void GenerateWalls()
    {

    }


    Choice ChooseFromOptions()
    {
        int randomNumber = Random.Range(1, totalWeight + 1);
        int pos = 0;
        for (int i = 0; i < choices.Count; i++)
        {
            if (randomNumber <= choices[i].weight + pos)
            {
                return choices[i];
            }
            pos += choices[i].weight;
        }
        Debug.Log("nothing to choose from");
        return null;
    }


}