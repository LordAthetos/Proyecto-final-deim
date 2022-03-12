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



    private GameManager gameManagerScript;
    public static int currentDirection = 0;


    // Objectos para las paredes 
    public GameObject[] WallObjects;
    public GameObject[] FloorObjects;


    public GameObject BaseFloor_prefab;
    public GameObject BaseFloor_perimeter;
    public GameObject BaseFloor_corner;
    private GameObject baseFloor;
    private GameObject wall;

    public GameObject Debugger;

    private GameObject templateFloor;
    private Vector3 offset;
    // Parametros de habitacion
    

    public int roomSizeX;
    public int roomSizeZ;
    public int roomSizeY;
    public int numDoors;
    public int numSections;

    public Vector3[] sectionCoordinates = new Vector3[10];
    Vector3[] roomData = new Vector3[4];
    private int currentWallID;
    /*
    int minSizeX = 3;
    int minSizeZ = 3;
    int maxSizeX = 7;
    int maxSizeZ = 7;
    int maxSizeY = 5;
    int maxNumSections = 9;
    
    */

    int templateRange = 3;
    // east = 0
    // south = 1
    // west = 2
    // north = 3
    int[][] wallValue = new int[4][]{
        new int[10],
        new int[10],
        new int[10],
        new int[10]
    };
    int[][] checkMaxNum = new int[4][]{
        new int[10],
        new int[10],
        new int[10],
        new int[10]
    };

    int[] xValues = new int[10];
    int[] zValues = new int[10];

    Choice selectedChoice;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = FindObjectOfType<GameManager>();
        int roomNumber = GameManager.currentRoomNumber;

        int minSizeX = PlayerPrefs.GetInt("minSizeX");
        
        int minSizeZ = PlayerPrefs.GetInt("maxSizeX");
       
        int maxSizeX = PlayerPrefs.GetInt("maxSizeY");
        
        int maxSizeZ = PlayerPrefs.GetInt("minSizeZ");
        
        int maxSizeY = PlayerPrefs.GetInt("maxSizeZ");
        
        int maxNumSections = PlayerPrefs.GetInt("maxNumSections");
        
        int templateRange = PlayerPrefs.GetInt("templateRange");
        
        roomSizeX = Random.Range(minSizeX, maxSizeX) * 2 - 1;
        roomSizeZ = Random.Range(minSizeZ, maxSizeZ) * 2 - 1;
        roomSizeY = Random.Range(2, maxSizeY);
        numSections = Random.Range(1, maxNumSections);

        // General Template 
        templateChoices.Add(new Choice("puerta entre habitacion", 2, PlayerPrefs.GetInt("doorBtRoomsWeight")));
        templateChoices.Add(new Choice("vacio", 3, PlayerPrefs.GetInt("voidWeight")));
        templateChoices.Add(new Choice("estrechamiento", 4, PlayerPrefs.GetInt("narrowingWeight")));
        foreach (Choice entry in templateChoices)
        {
            totalTemplateWeight += entry.weight;
        }
        // Vertical wall slice types
        wallTypeChoices.Add(new Choice("SWall", 1, PlayerPrefs.GetInt("sWallWeight")));
        wallTypeChoices.Add(new Choice("Window", 2, PlayerPrefs.GetInt("window")));
        foreach (Choice entry in wallTypeChoices)
        {
            totalWallTypeWeight += entry.weight;
        }
        // Wall Decoration
        wallDecoration.Add(new Choice("SWall", 2, PlayerPrefs.GetInt("sWallDWeight")));
        wallDecoration.Add(new Choice("Graffitti_1", 5, PlayerPrefs.GetInt("graffitti1Weight")));
        wallDecoration.Add(new Choice("Graffitti_2", 6, PlayerPrefs.GetInt("graffitti2Weight")));
        wallDecoration.Add(new Choice("Graffitti_3", 7, PlayerPrefs.GetInt("graffitti3Weight")));
        wallDecoration.Add(new Choice("Wall_crack", 8, PlayerPrefs.GetInt("wallCrackWeight")));
        foreach (Choice entry in wallDecoration)
        {
            totalWallDecWeight += entry.weight;
        }
        // Floor Decoration
        floorDecorationChoices.Add(new Choice("Floor", 0, PlayerPrefs.GetInt("sFloorWeight")));
        floorDecorationChoices.Add(new Choice("Trapdoor", 1, PlayerPrefs.GetInt("trapdoorWeight")));
        floorDecorationChoices.Add(new Choice("Generator", 2, PlayerPrefs.GetInt("generatorWeight")));
        foreach (Choice entry in floorDecorationChoices)
        {
            totalFloorDecWeight += entry.weight;
        }

        
        

        GenerateTemplate();
        for (int sectionCounter = 0; sectionCounter <= numSections; sectionCounter += 1)
        {
            GenerateFloor(sectionCounter);
        }
        gameManagerScript.StoreRoomData(roomNumber, roomData);
        

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
                isAdjacent = false;
                checkPX = new Vector3(currentTile.x - 1, currentTile.y, currentTile.z);
                checkPZ = new Vector3(currentTile.x, currentTile.y, currentTile.z - 1);
                checkNX = new Vector3(currentTile.x + 1, currentTile.y, currentTile.z);
                checkNZ = new Vector3(currentTile.x, currentTile.y, currentTile.z + 1);

                //+x 
                if (sectionCoordinates.Contains(checkPX))
                {
                    isAdjacent = true;
                }
                //+z 
                if (sectionCoordinates.Contains(checkPZ))
                {
                    isAdjacent = true;
                }
                //-x 
                if (sectionCoordinates.Contains(checkNX))
                {
                    isAdjacent = true;
                }
                //-z 
                if (sectionCoordinates.Contains(checkNZ))
                {
                    isAdjacent = true;
                }
                if (!sectionCoordinates.Contains(currentTile))
                {
                    notRepeated = true;
                }


            }
            sectionCoordinates[sectionCounter] = new Vector3(currentTile.x, currentTile.y, currentTile.z);
            xValues[sectionCounter] = (int)currentTile.x;
            zValues[sectionCounter] = (int)currentTile.z;
            isAdjacent = false;
            notRepeated = false;

        }
        int maxX = xValues.Max();
        //Debug.Log($"Max X is {maxX}");
        int minX = xValues.Min();
        //Debug.Log($"Min X is {minX}");
        int maxZ = zValues.Max();
        //Debug.Log($"Max Z is {maxZ}");
        int minZ = zValues.Min();
        //Debug.Log($"Min Z is {minZ}");

        bool eastDoorBuilt = false;
        bool southDoorBuilt = false;
        bool westDoorBuilt = false;
        bool northDoorBuilt = false;

        Vector3 eastDoorCoords;
        Vector3 southDoorCoords;
        Vector3 westDoorCoords;
        Vector3 northDoorCoords;

        for (int sectionCounter = 0; sectionCounter <= numSections; sectionCounter += 1)
        {

            currentTile = sectionCoordinates[sectionCounter];

            checkPX = new Vector3(currentTile.x -1, currentTile.y, currentTile.z);
            checkPZ = new Vector3(currentTile.x, currentTile.y, currentTile.z -1); 
            checkNX = new Vector3(currentTile.x +1, currentTile.y, currentTile.z); 
            checkNZ = new Vector3(currentTile.x, currentTile.y, currentTile.z +1); 

            if (sectionCoordinates.Contains(checkPX))
            {
                
                selectedChoice = ChooseTemplate();
                wallValue[0][sectionCounter] = selectedChoice.choiceID;
            }
            else
            {
                if (!eastDoorBuilt && currentTile.x == minX)
                {
                    wallValue[0][sectionCounter] = 1;
                    eastDoorBuilt = true;
                    eastDoorCoords = new Vector3((currentTile.x * roomSizeX) - (roomSizeX / 2 + 1), currentTile.y +1, currentTile.z * roomSizeZ);
                    roomData[0] = eastDoorCoords;

                }
                else
                {
                    wallValue[0][sectionCounter] = 0;
                }
                
            }
            if (sectionCoordinates.Contains(checkPZ))
            {
                
                selectedChoice = ChooseTemplate();
                wallValue[1][sectionCounter] = selectedChoice.choiceID;

            }
            else
            {
                if (!southDoorBuilt && currentTile.z == minZ)
                {
                    wallValue[1][sectionCounter] = 1;
                    southDoorBuilt = true;
                    southDoorCoords = new Vector3(currentTile.x * roomSizeX, currentTile.y + 1, currentTile.z * roomSizeZ - (roomSizeZ / 2 + 1));
                    roomData[1] = southDoorCoords;
                }
                else
                {
                    wallValue[1][sectionCounter] = 0;
                }
                
            }
            if (sectionCoordinates.Contains(checkNX))
            {
                
                selectedChoice = ChooseTemplate();
                wallValue[2][sectionCounter] = selectedChoice.choiceID;

            }
            else
            {
                if (!westDoorBuilt && currentTile.x == maxX)
                {
                    wallValue[2][sectionCounter] = 1;
                    westDoorBuilt = true;
                    westDoorCoords = new Vector3((currentTile.x * roomSizeX) + (roomSizeX / 2 + 1) -2, currentTile.y + 1, currentTile.z * roomSizeZ);
                    roomData[2] = westDoorCoords;
                }
                else
                {
                    wallValue[2][sectionCounter] = 0; 
                }
                
            }
            if (sectionCoordinates.Contains(checkNZ))
            {
                
                selectedChoice = ChooseTemplate();
                wallValue[3][sectionCounter] = selectedChoice.choiceID;

            }
            else
            {
                if (!northDoorBuilt && currentTile.z == maxZ)
                {
                    wallValue[3][sectionCounter] = 1;
                    northDoorBuilt = true;
                    northDoorCoords = new Vector3(currentTile.x * roomSizeX -2, currentTile.y + 1, currentTile.z * roomSizeZ + (roomSizeZ / 2 + 1) -2);
                    roomData[3] = northDoorCoords;
                }
                else
                {
                    wallValue[3][sectionCounter] = 0;
                }
                
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

        Vector3 moveToTemplate = Vector3.zero;
        Vector3 Rotation0 = new Vector3(0, 0, 0);
        Vector3 Rotation90 = new Vector3(0, 90, 0);
        Vector3 Rotation180 = new Vector3(0, 180, 0);
        Vector3 Rotation270 = new Vector3(0, 270, 0);
        Vector3 RotationVector = Vector3.zero;

        int randomRotation;

        // Genera una cuadricula de prefabs de suelo, la taggea segun las propiedades y la emparenta a un nuevo game object root
        GameObject FloorSection = new GameObject($"FloorSectionRoot{sectionNumber}");
        offset = new Vector3(roomSizeX / 2 +1, 0, roomSizeZ / 2 +1);
        int wallcenterX = (int)offset.x;
        int wallcenterZ = (int)offset.z;
        
        
        for (int i = 0; i < roomSizeX; i++)
        {
            for (int j = 0; j < roomSizeZ; j++)
            {
                randomRotation = (Random.Range(0, 4));
                switch (randomRotation)
                {
                    case 0:
                        RotationVector = Rotation0;
                        break;
                    case 1:
                        RotationVector = Rotation90;
                        break;
                    case 2:
                        RotationVector = Rotation180;
                        break;
                    case 3:
                        RotationVector = Rotation270;
                        break;
                }
                // comprovacion de si el suelo que va a generar es perimetro o esquina
                if (i == 0 || j == 0 || i == roomSizeX - 1 || j == roomSizeZ - 1)
                {
                    if (i == 0 && j == 0 || i == 0 && j == roomSizeZ - 1 || i == roomSizeX - 1 && j == 0 || i == roomSizeX - 1 && j == roomSizeZ - 1)
                    {
                        baseFloor = Instantiate(FloorObjects[0], new Vector3(i, 0f, j), Quaternion.identity);
                        baseFloor.transform.position = new Vector3(baseFloor.transform.position.x, baseFloor.transform.position.y, baseFloor.transform.position.z) - offset;
                        baseFloor.transform.Rotate(RotationVector);
                        baseFloor.transform.SetParent(FloorSection.transform);
                        baseFloor.tag = "FloorPerimeterCorner";

                    }
                    else
                    {
                        baseFloor = Instantiate(FloorObjects[0], new Vector3(i, 0f, j), Quaternion.identity);
                        baseFloor.transform.position = new Vector3(baseFloor.transform.position.x, baseFloor.transform.position.y, baseFloor.transform.position.z) - offset;
                        baseFloor.transform.Rotate(RotationVector);
                        baseFloor.transform.SetParent(FloorSection.transform);
                        baseFloor.tag = "FloorPerimeter";
                        
                    }

                }
                else
                {
                    selectedChoice = ChooseFloorDecoration();
                    baseFloor = Instantiate(FloorObjects[selectedChoice.choiceID], new Vector3(i, 0f, j), Quaternion.identity);
                    baseFloor.transform.position = new Vector3(baseFloor.transform.position.x, baseFloor.transform.position.y, baseFloor.transform.position.z) - offset;
                    baseFloor.transform.Rotate(RotationVector);
                    baseFloor.transform.SetParent(FloorSection.transform);
                    baseFloor.tag = "Floor";
                }
            }
        }

        eastWall = wallValue[0][sectionNumber];
        GenerateWall(eastWall, roomSizeZ, roomSizeY, roomSizeX / 2 +1, 0, FloorSection);

        southWall = wallValue[1][sectionNumber];
        GenerateWall(southWall, roomSizeX, roomSizeY, roomSizeZ / 2 + 1, 1, FloorSection);

        westWall = wallValue[2][sectionNumber];
        GenerateWall(westWall, roomSizeZ, roomSizeY, roomSizeX / 2 + 1, 2, FloorSection);

        northWall = wallValue[3][sectionNumber];
        GenerateWall(northWall, roomSizeX, roomSizeY, roomSizeZ / 2 + 1, 3, FloorSection);



        moveToTemplate = sectionCoordinates[sectionNumber];
        FloorSection.transform.position = new Vector3(moveToTemplate.x * roomSizeX, 0 +1, moveToTemplate.z * roomSizeZ);
        FloorSection.transform.SetParent(gameObject.transform);
    }

    public void GenerateWall(int wallType, int wallLenght, int wallHeigh, int wallOffset, int direction, GameObject sectionParent)
    {
        Vector3 dirVector = Vector3.zero;
        Vector3 dirOffset = Vector3.zero;
        Vector3 rotVector = Vector3.zero;
        
        switch (direction)
        {
            case 0:
                dirVector = Vector3.forward;
                dirOffset = -offset;
                rotVector = Vector3.zero;
                
                break;
            case 1:
                dirVector = Vector3.right;
                dirOffset = -offset;
                rotVector = new Vector3(0, 270, 0);
                
                break;
            case 2:
                dirVector = Vector3.back;
                dirOffset = new Vector3(offset.x -2, offset.y, offset.z -2);
                rotVector = new Vector3(0, 180, 0);
                
                break;
            case 3:
                dirVector = Vector3.left;
                dirOffset = new Vector3(offset.x -2, offset.y, offset.z -2);
                rotVector = new Vector3(0, 90, 0);
                
                break;
        }
        
        int startPos = 0;
        int endPos = startPos + wallLenght;
        int wallCenter = endPos / 2 + 1;
        

        int currentWallStrip = 0;
        int currentWall = 0;

        int wallHole = Random.Range(wallLenght / 2, wallLenght);
        int wallHoleCenter = wallHole / 2 + 1;
        int wallHoleStart = wallHoleCenter - (wallHole / 2);
        int wallHoleEnd = wallHoleCenter + (wallHole / 2);

        for (int i = startPos; i < endPos; i++)
        {
            switch (wallType)
            {
                case 0:     //Perimetro
                    selectedChoice = ChooseWalltypes();
                    currentWallStrip = selectedChoice.choiceID;
                    break;

                case 1:     //Puerta perimetro
                    if (i == wallCenter)
                    {
                        currentWallStrip = 0;
                    }
                    else
                    {
                        selectedChoice = ChooseWalltypes();
                        currentWallStrip = selectedChoice.choiceID;
                    }
                    break;

                case 2:     //Puerta entre habitacion
                    if (i == wallCenter)
                    {
                        currentWallStrip = 4;
                    }
                    else
                    {
                        selectedChoice = ChooseWalltypes();
                        currentWallStrip = selectedChoice.choiceID;
                    }
                    break;

                case 3:     //Vacio
                    currentWallStrip = 3;
                    break;

                case 4:     // estrechamiento
                    if (i >= wallHoleStart && i <= wallHoleEnd)
                    {
                        currentWallStrip = 3;
                    }
                    else
                    {
                        currentWallStrip = 1;
                    }
                    break;
            }

            for (int h = 0; h < wallHeigh; h++)
            {
                switch (currentWallStrip)
                {
                    case 0:     // puerta
                        currentWall = 0;
                        currentWallStrip = 1;
                        break;

                    case 1:     // pared 
                        selectedChoice = ChooseWallDecoration();
                        currentWall = selectedChoice.choiceID;
                        break;

                    case 2:     // ventana 
                        currentWall = 1;
                        break;

                    case 3:     // vacio
                        currentWall = 3;
                        break;

                    case 4:     // puerta interior
                        currentWall = 4;
                        currentWallStrip = 1;
                        break;
                }
                wall = Instantiate(WallObjects[currentWall], new Vector3(dirVector.x * i, h, dirVector.z * i), Quaternion.identity);
                wall.transform.position = new Vector3(wall.transform.position.x, wall.transform.position.y, wall.transform.position.z) + dirOffset;
                wall.transform.Rotate(rotVector);
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