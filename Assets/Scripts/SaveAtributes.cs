using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveAtributes : MonoBehaviour
{
    private int[] atributes = new int[20];
    public TMP_InputField[] inputs = new TMP_InputField[20];
    public TMP_InputField[] outputs = new TMP_InputField[20];
    public string[] atributeNames = new string[20];  

    void Start()
    {
        for (int i = 0; i <= 19; i++)
        {
            atributes[i] = PlayerPrefs.GetInt(atributeNames[i]);
            inputs[i].placeholder.GetComponent<TextMeshProUGUI>().text = atributes[i].ToString();

        }

    }
    public void SaveChanges()
    {
        string why;
        for (int i = 0; i <= 19; i++)
        {
            // Esto tira un error de formato al convertir de string a int, no se como solucionarlo pero parece que funciona de todas formas
            atributes[i] = int.Parse(outputs[i].text);


            PlayerPrefs.SetInt(atributeNames[i], atributes[i]);            
        }
    }
    public void ResetDefaults()
    {
        PlayerPrefs.SetInt("minSizeX", 3);
        PlayerPrefs.SetInt("maxSizeX", 7);
        PlayerPrefs.SetInt("maxSizeY", 5);
        PlayerPrefs.SetInt("minSizeZ", 3);
        PlayerPrefs.SetInt("maxSizeZ", 7);
        PlayerPrefs.SetInt("maxNumSections", 9);
        PlayerPrefs.SetInt("templateRange", 3);

        PlayerPrefs.SetInt("doorBtRoomsWeight", 5);
        PlayerPrefs.SetInt("voidWeight", 20);
        PlayerPrefs.SetInt("narrowingWeight", 40);

        PlayerPrefs.SetInt("sWallWeight", 100);
        PlayerPrefs.SetInt("window", 10);

        PlayerPrefs.SetInt("sWallDWeight", 50);
        PlayerPrefs.SetInt("graffitti1Weight", 5);
        PlayerPrefs.SetInt("graffitti2Weight", 5);
        PlayerPrefs.SetInt("graffitti3Weight", 5);
        PlayerPrefs.SetInt("wallCrackWeight", 5);

        PlayerPrefs.SetInt("sFloorWeight", 200);
        PlayerPrefs.SetInt("trapdoorWeight", 5);
        PlayerPrefs.SetInt("generatorWeight", 2);
        for (int i = 0; i <= 19; i++)
        {
            atributes[i] = PlayerPrefs.GetInt(atributeNames[i]);
            inputs[i].placeholder.GetComponent<TextMeshProUGUI>().text = atributes[i].ToString();
        }
    }
    
}
