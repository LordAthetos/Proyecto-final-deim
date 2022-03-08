using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// Pesos 
public class Choice
{
    public string choiceName;
    public int weight;
    public int choiceID;
    public Choice(string newChoiceName, int newchoiceID, int newChoiceWeight)
    {
        choiceName = newChoiceName;
        weight = newChoiceWeight;
        choiceID = newchoiceID;
    }
}

public class GenerateRoom : MonoBehaviour
{
    List<Choice> templateChoices = new List<Choice>();
    int totalTemplateWeight;

    List<Choice> wallTypeChoices = new List<Choice>();
    int totalWallTypeWeight;

    List<Choice> wallDecoration = new List<Choice>();
    int totalWallDecWeight;

    List<Choice> floorDecorationChoices = new List<Choice>();
    int totalFloorDecWeight;



    // Objectos para las paredes 
    public GameObject[] WallObjects;



    public GameObject BaseFloor_prefab;
    public GameObject BaseFloor_perimeter;
    public GameObject BaseFloor_corner;
    private GameObject baseFloor;
    private GameObject wall;

    public GameObject Debugger;

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

    private int currentWallID;



    // east = 0
    // south = 1
    // west = 2
    // north = 3
    int[][] wallValue = new int[4][]{
        new int[10]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        new int[10]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        new int[10]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        new int[10]{1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
    };
    int[][] checkAdjacent = new int[4][]{
        new int[10],
        new int[10],
        new int[10],
        new int[10]
    };

    Choice selectedChoice;

    // Start is called before the first frame update
    void Start()
    {

        // General Template 
        templateChoices.Add(new Choice("wall", 1, 25));
        templateChoices.Add(new Choice("door", 2, 25));
        templateChoices.Add(new Choice("space", 3, 50));
        foreach (Choice entry in templateChoices)
        {
            totalTemplateWeight += entry.weight;
        }
        // Vertical wall slice types
        wallTypeChoices.Add(new Choice("SWall", 1, 100));
        wallTypeChoices.Add(new Choice("Window", 2, 10));
        foreach (Choice entry in wallTypeChoices)
        {
            totalWallTypeWeight += entry.weight;
        }
        // Wall Decoration
        wallDecoration.Add(new Choice("SWall", 2, 50));
        wallDecoration.Add(new Choice("Graffitti_1", 3, 5));
        wallDecoration.Add(new Choice("Graffitti_2", 4, 5));
        wallDecoration.Add(new Choice("Poster_1", 5, 5));
        wallDecoration.Add(new Choice("Poster_2", 6, 5));
        wallDecoration.Add(new Choice("Wall_crack", 7, 5));
        foreach (Choice entry in wallDecoration)
        {
            totalWallDecWeight += entry.weight;
        }
        // Floor Decoration
        floorDecorationChoices.Add(new Choice("Floor", 1, 100));
        floorDecorationChoices.Add(new Choice("Trapdoor", 2, 5));
        floorDecorationChoices.Add(new Choice("Generator", 3, 2));
        foreach (Choice entry in floorDecorationChoices)
        {
            totalFloorDecWeight += entry.weight;
        }


        DefineParameters();
        GenerateTemplate();
        for (int sectionCounter = 0; sectionCounter <= numSections; sectionCounter += 1)
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
        Vector3 checkPX;
        Vector3 checkPZ;
        Vector3 checkNX;
        Vector3 checkNZ;
        bool isAdjacent = false;
        bool notRepeated = false;



        for (int sectionCounter = 1; sectionCounter <= numSections; sectionCounter += 1)
        {
            while (!isAdjacent || !notRepeated)
            {
                currentTile = new Vector3(Random.Range(-templateRange, templateRange), 0, Random.Range(-templateRange, templateRange));
                notRepeated = false;
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
        
        for (int sectionCounter = 0; sectionCounter <= numSections; sectionCounter += 1)
        {

            currentTile = sectionCoordinates[sectionCounter];

            checkPX = new Vector3(currentTile.x -1, currentTile.y, currentTile.z);
            checkPZ = new Vector3(currentTile.x, currentTile.y, currentTile.z -1); 
            checkNX = new Vector3(currentTile.x +1, currentTile.y, currentTile.z); 
            checkNZ = new Vector3(currentTile.x, currentTile.y, currentTile.z +1); 

            //Debug.Log($"tile number {sectionCounter} is: {currentTile}");
            //Debug.Log($"Px from {sectionCounter} is: {checkPX}");
            //Debug.Log($"Pz from {sectionCounter} is: {checkPZ}");
            //Debug.Log($"Nx from {sectionCounter} is: {checkNX}");
            //Debug.Log($"Nz from {sectionCounter} is: {checkNZ}");

            if (sectionCoordinates.Contains(checkPX))
            {
                //Debug.Log($"Px from {sectionCounter} is: positive");
                wallValue[0][sectionCounter] = 1;

            }
            else
            {
                //Debug.Log($"Px from {sectionCounter} is: negative");
                wallValue[0][sectionCounter] = 0;
            }

        }

    }

    public void GenerateFloor(int sectionNumber)
    {
        int northWall;
        int southWall;
        int eastWall;
        int westWall;

        

        // Listas para generar las paredes (TBI)
        List<GameObject> northPerimeter = new List<GameObject>();
        List<GameObject> southPerimeter = new List<GameObject>();
        List<GameObject> eastPerimeter = new List<GameObject>();
        List<GameObject> westPerimeter = new List<GameObject>();

        Vector3 moveToTemplate = new Vector3(0, 0, 0);

        // Genera una cuadricula de prefabs de suelo, la taggea segun las propiedades y la emparenta a un nuevo game object root
        GameObject FloorSection = new GameObject($"FloorSectionRoot{sectionNumber}");
        offset = new Vector3(roomSizeX / 2, 0, roomSizeZ / 2);
        int wallcenterX = (int)offset.x;
        int wallcenterZ = (int)offset.z;
        Debug.Log($"offset is:{offset}");
        //offset = Vector3.zero;
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

        eastWall = wallValue[0][sectionNumber];
        Debug.Log($"east: {eastWall}");
        //Instantiate(Debugger, new Vector3(i, 0, j), Quaternion.identity);
        GenerateWall(wallcenterX, eastWall, roomSizeZ, roomSizeY, roomSizeX / 2, 0, FloorSection, offset);

        moveToTemplate = sectionCoordinates[sectionNumber];
        FloorSection.transform.position = new Vector3(moveToTemplate.x * roomSizeX, 0, moveToTemplate.z * roomSizeZ);

    }

    public void GenerateWall(int wallCenter, int wallType, int wallLenght, int wallHeigh, int wallOffset, int direction, GameObject sectionParent, Vector3 dirOffset)
    {
        //Debug.Log($"");
        Debug.Log($"WallCenter = {wallCenter}");
        Debug.Log($"WallLenght = {wallLenght}");
        Vector3 dirMult = Vector3.zero;
        switch (direction)
        {
            case 0:
                dirMult = Vector3.forward;
                break;
            case 1:
                dirMult = Vector3.back;
                break;
            case 2:
                dirMult = Vector3.right;
                break;
            case 3:
                dirMult = Vector3.forward;
                break;
        }

        switch (wallType)
        {
            case 0:     //Perimetro
                GeneratePerimeter(wallCenter, wallLenght, wallHeigh, wallOffset, direction, dirMult, false, sectionParent, dirOffset);
                break;
            case 1:     //Pared entre seccion
                //GeneratePerimeter(wallCenter, wallLenght, wallHeigh, wallOffset, direction, dirMult, false, sectionParent,  dirOffset);
                break;
            case 2:     //Puerta entre seccion
                //GeneratePerimeter(wallCenter, wallLenght, wallHeigh, wallOffset, direction, dirMult, false, sectionParent,  dirOffset);
                break;
            case 3:     //Vacio
                //GeneratePerimeter(wallCenter, wallLenght, wallHeigh, wallOffset, direction, dirMult, false, sectionParent,  dirOffset);
                break;
            case 4:     //Puerta entre habitacion
                //GeneratePerimeter(wallCenter, wallLenght, wallHeigh, wallOffset, direction, dirMult, false, sectionParent, dirOffset);
                break;
        }
    }

    // Funciones para diferentes tipos de muros

    public void GeneratePerimeter(int wallCenter, int wallLenght, int wallHeigh, int wallOffset, int direction, Vector3 dirVector, bool hasDoor, GameObject sectionParent, Vector3 dirOffset)
    {
        // [0] Puerta/Root;  [1] Pared normal;  [2] 
        //construir paredes
        
        int startPos = wallCenter - (wallLenght / 2);
        int endPos = startPos + wallLenght;
        Debug.Log($"StartPos = {startPos}");
        
        int currentWallStrip = 0;
        int currentWall = 0;
        Debug.Log("Wall  G E N E R A T E D");
        //bool hasDoorBeenBuilt = false;
        for (int i = startPos; i < endPos; i++)
        {
            if (i == wallCenter && hasDoor)
            {
                currentWallStrip = 0;
            }
            else
            {
                selectedChoice = ChooseWalltypes();
                //Debug.Log($"{selectedChoice.choiceName} {selectedChoice.choiceID}");
                currentWallStrip = selectedChoice.choiceID;
            }

            for (int h = 0; h < wallHeigh; h++)
            {
                switch (currentWallStrip)
                {
                    case 0: // puerta
                        currentWall = 2;
                        break;
                    case 1: // pared 
                        selectedChoice = ChooseWallDecoration();
                        currentWall = selectedChoice.choiceID;

                        break;
                    case 2: // ventana 
                        currentWall = 1;
                        break;
                }

                wall = Instantiate(WallObjects[currentWall], new Vector3(dirVector.x * i, h, dirVector.z * i), Quaternion.identity);
                wall.transform.position = new Vector3(wall.transform.position.x, wall.transform.position.y, wall.transform.position.z);
                wall.transform.SetParent(sectionParent.transform);
                //instantiate current wall


            }



        }

    }


    Choice ChooseTemplate()
    {
        int randomNumber = Random.Range(1, totalTemplateWeight + 1);
        int pos = 0;
        for (int i = 0; i < templateChoices.Count; i++)
        {
            if (randomNumber <= templateChoices[i].weight + pos)
            {
                return templateChoices[i];
            }
            pos += templateChoices[i].weight;
        }
        Debug.Log("nothing to choose from 1");
        return null;
    }
    Choice ChooseWalltypes()
    {
        int randomNumber = Random.Range(1, totalWallTypeWeight + 1);
        int pos = 0;
        for (int i = 0; i < wallTypeChoices.Count; i++)
        {
            if (randomNumber <= wallTypeChoices[i].weight + pos)
            {
                return wallTypeChoices[i];
            }
            pos += wallTypeChoices[i].weight;
        }
        Debug.Log("nothing to choose from 2");
        return null;
    }
    Choice ChooseWallDecoration()
    {
        int randomNumber = Random.Range(1, totalWallDecWeight + 1);
        int pos = 0;
        for (int i = 0; i < wallDecoration.Count; i++)
        {
            if (randomNumber <= wallDecoration[i].weight + pos)
            {
                return wallDecoration[i];
            }
            pos += wallDecoration[i].weight;
        }
        Debug.Log("nothing to choose from 3");
        return null;
    }
    Choice ChooseFloorDecoration()
    {
        int randomNumber = Random.Range(1, totalFloorDecWeight + 1);
        int pos = 0;
        for (int i = 0; i < floorDecorationChoices.Count; i++)
        {
            if (randomNumber <= floorDecorationChoices[i].weight + pos)
            {
                return floorDecorationChoices[i];
            }
            pos += floorDecorationChoices[i].weight;
        }
        Debug.Log("nothing to choose from 4");
        return null;
    }



}