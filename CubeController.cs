using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    Color[] Colors = {new Color(0,0,0,0), new Color(1,1,1,1), new Color(1,0,0,1), new Color(0,1,0,1), new Color(0,0,1,1), new Color(1,1,0,1), new Color(1,0.5f,0,1)};
    Blok[,,] blokarray = new Blok[3,3,3];
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        int KleurRechterkant, KleurLinkerkant, KleurOnderkant, KleurBovenkant, KleurAchterkant, KleurVoorkant;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (x == 0)
                    {
                        KleurRechterkant = 5;
                    }
                    else
                    {
                        KleurRechterkant = 0;
                    }

                    if(x == 2)
                    {
                        KleurLinkerkant = 4;
                    }
                    else
                    {
                        KleurLinkerkant = 0;
                    }

                    if (y == 0)
                    {
                        KleurOnderkant = 1;
                    }
                    else
                    {
                        KleurOnderkant = 0;
                    }

                    if (y == 2)
                    {
                        KleurBovenkant = 6;
                    }
                    else
                    {
                        KleurBovenkant = 0;
                    }

                    if (z == 0)
                    {
                        KleurAchterkant = 3;
                    }
                    else
                    {
                        KleurAchterkant = 0;
                    }

                    if (z == 2)
                    {
                        KleurVoorkant = 2;
                    }
                    else
                    {
                        KleurVoorkant = 0;
                    }

                    blokarray[x, y, z] = new Blok(x, y, z, KleurVoorkant, KleurAchterkant, KleurBovenkant, KleurOnderkant, KleurLinkerkant, KleurRechterkant);
                }
            }
        }
        Teken();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Counter clockwise rechtsboven
            int tempkleur = blokarray[0, 2, 2].KleurVoor;
            blokarray[0, 2, 2].KleurVoor = blokarray[0, 2, 0].KleurRechts;
            blokarray[0, 2, 0].KleurRechts = blokarray[2, 2, 0].KleurAchter;
            blokarray[2, 2, 0].KleurAchter = blokarray[2, 2, 2].KleurLinks;
            blokarray[2, 2, 2].KleurLinks = tempkleur;

            //Counter clockwise linksboven
            tempkleur = blokarray[2, 2, 2].KleurVoor;
            blokarray[2, 2, 2].KleurVoor = blokarray[0, 2, 2].KleurRechts;
            blokarray[0, 2, 2].KleurRechts = blokarray[0, 2, 0].KleurAchter;
            blokarray[0, 2, 0 ].KleurAchter = blokarray[2, 2, 0].KleurLinks;
            blokarray[2, 2, 0].KleurLinks = tempkleur;

            //Counter clockwise middenboven
            tempkleur = blokarray[1, 2, 2].KleurVoor;
            blokarray[1, 2, 2].KleurVoor = blokarray[0, 2, 1].KleurRechts;
            blokarray[0, 2, 1].KleurRechts = blokarray[1, 2, 0].KleurAchter;
            blokarray[1, 2, 0].KleurAchter = blokarray[2, 2, 1].KleurLinks;
            blokarray[2, 2, 1].KleurLinks = tempkleur;

            //Counter clockwise bovenop hoek
            tempkleur = blokarray[2, 2, 2].KleurBoven;
            blokarray[2, 2, 2].KleurBoven = blokarray[0, 2, 2].KleurBoven;
            blokarray[0, 2, 2].KleurBoven = blokarray[0, 2, 0].KleurBoven;
            blokarray[0, 2, 0].KleurBoven = blokarray[2, 2, 0].KleurBoven;
            blokarray[2, 2, 0].KleurBoven = tempkleur;

            //Counter clockwise bovenop miden
            tempkleur = blokarray[1, 2, 2].KleurBoven;
            blokarray[1, 2, 2].KleurBoven = blokarray[0, 2, 1].KleurBoven;
            blokarray[0, 2, 1].KleurBoven = blokarray[1, 2, 0].KleurBoven;
            blokarray[1, 2, 0].KleurBoven = blokarray[2, 2, 1].KleurBoven;
            blokarray[2, 2, 1].KleurBoven = tempkleur;

            Teken();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            //Counter clockwise rechtsboven
            int tempkleur = blokarray[0, 1, 2].KleurVoor;
            blokarray[0, 1, 2].KleurVoor = blokarray[0, 1, 0].KleurRechts;
            blokarray[0, 1, 0].KleurRechts = blokarray[2, 1, 0].KleurAchter;
            blokarray[2, 1, 0].KleurAchter = blokarray[2, 1, 2].KleurLinks;
            blokarray[2, 1, 2].KleurLinks = tempkleur;

            //Counter clockwise linksboven
            tempkleur = blokarray[2, 1, 2].KleurVoor;
            blokarray[2, 1, 2].KleurVoor = blokarray[0, 1, 2].KleurRechts;
            blokarray[0, 1, 2].KleurRechts = blokarray[0, 1, 0].KleurAchter;
            blokarray[0, 1, 0].KleurAchter = blokarray[2, 1, 0].KleurLinks;
            blokarray[2, 1, 0].KleurLinks = tempkleur;

            //Counter clockwise middenboven
            tempkleur = blokarray[1, 1, 2].KleurVoor;
            blokarray[1, 1, 2].KleurVoor = blokarray[0, 1, 1].KleurRechts;
            blokarray[0, 1, 1].KleurRechts = blokarray[1, 1, 0].KleurAchter;
            blokarray[1, 1, 0].KleurAchter = blokarray[2, 1, 1].KleurLinks;
            blokarray[2, 1, 1].KleurLinks = tempkleur;

            Teken();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //Counter clockwise rechtsboven
            int tempkleur = blokarray[0, 0, 2].KleurVoor;
            blokarray[0, 0, 2].KleurVoor = blokarray[0, 0, 0].KleurRechts;
            blokarray[0, 0, 0].KleurRechts = blokarray[2, 0, 0].KleurAchter;
            blokarray[2, 0, 0].KleurAchter = blokarray[2, 0, 2].KleurLinks;
            blokarray[2, 0, 2].KleurLinks = tempkleur;

            //Counter clockwise linksboven
            tempkleur = blokarray[2, 0, 2].KleurVoor;
            blokarray[2, 0, 2].KleurVoor = blokarray[0, 0, 2].KleurRechts;
            blokarray[0, 0, 2].KleurRechts = blokarray[0, 0, 0].KleurAchter;
            blokarray[0, 0, 0].KleurAchter = blokarray[2, 0, 0].KleurLinks;
            blokarray[2, 0, 0].KleurLinks = tempkleur;

            //Counter clockwise middenboven
            tempkleur = blokarray[1, 0, 2].KleurVoor;
            blokarray[1, 0, 2].KleurVoor = blokarray[0, 0, 1].KleurRechts;
            blokarray[0, 0, 1].KleurRechts = blokarray[1, 0, 0].KleurAchter;
            blokarray[1, 0, 0].KleurAchter = blokarray[2, 0, 1].KleurLinks;
            blokarray[2, 0, 1].KleurLinks = tempkleur;

            //Counter clockwise bovenop hoek
            tempkleur = blokarray[2, 0, 2].KleurOnder;
            blokarray[2, 0, 2].KleurOnder = blokarray[0, 0, 2].KleurOnder;
            blokarray[0, 0, 2].KleurOnder = blokarray[0, 0, 0].KleurOnder;
            blokarray[0, 0, 0].KleurOnder = blokarray[2, 0, 0].KleurOnder;
            blokarray[2, 0, 0].KleurOnder = tempkleur;

            //Counter clockwise bovenop midden
            tempkleur = blokarray[1, 0, 2].KleurOnder;
            blokarray[1, 0, 2].KleurOnder = blokarray[0, 0, 1].KleurOnder;
            blokarray[0, 0, 1].KleurOnder = blokarray[1, 0, 0].KleurOnder;
            blokarray[1, 0, 0].KleurOnder = blokarray[2, 0, 1].KleurOnder;
            blokarray[2, 0, 1].KleurOnder = tempkleur;

            Teken();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) //(rechter rij)
        {
            // Richting boven draaien rechtsboven
            int tempkleur = blokarray[0, 2, 2].KleurVoor;
            blokarray[0, 2, 2].KleurVoor = blokarray[0, 0, 2].KleurOnder;
            blokarray[0, 0, 2].KleurOnder = blokarray[0, 0, 0].KleurAchter;
            blokarray[0, 0, 0].KleurAchter = blokarray[0, 2, 0].KleurBoven;
            blokarray[0, 2, 0].KleurBoven = tempkleur;

            //richting boven draaien rechtsmidden
            tempkleur = blokarray[0, 1, 2].KleurVoor;
            blokarray[0, 1, 2].KleurVoor = blokarray[0, 0, 1].KleurOnder;
            blokarray[0, 0, 1].KleurOnder = blokarray[0, 1, 0].KleurAchter;
            blokarray[0, 1, 0].KleurAchter = blokarray[0, 2, 1].KleurBoven;
            blokarray[0, 2, 1].KleurBoven = tempkleur;

            //richting boven draaien rechtsonder
            tempkleur = blokarray[0, 0, 2].KleurVoor;
            blokarray[0, 0, 2].KleurVoor = blokarray[0, 0, 0].KleurOnder;
            blokarray[0, 0, 0].KleurOnder = blokarray[0, 2, 0].KleurAchter;
            blokarray[0, 2, 0].KleurAchter = blokarray[0, 2, 2].KleurBoven;
            blokarray[0, 2, 2].KleurBoven = tempkleur;

            //richting boven draaien rechter zijkant hoeken
            tempkleur = blokarray[0, 2, 2].KleurRechts;
            blokarray[0, 2, 2].KleurRechts = blokarray[0, 0, 2].KleurRechts;
            blokarray[0, 0, 2].KleurRechts = blokarray[0, 0, 0].KleurRechts;
            blokarray[0, 0, 0].KleurRechts = blokarray[0, 2, 0].KleurRechts;
            blokarray[0, 2, 0].KleurRechts = tempkleur;

            //richting boven draaien rechter zijkant midden
            tempkleur = blokarray[0, 1, 2].KleurRechts;
            blokarray[0, 1, 2].KleurRechts = blokarray[0, 0, 1].KleurRechts;
            blokarray[0, 0, 1].KleurRechts = blokarray[0, 1, 0].KleurRechts;
            blokarray[0, 1, 0].KleurRechts = blokarray[0, 2, 1].KleurRechts;
            blokarray[0, 2, 1].KleurRechts = tempkleur;

            Teken();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //richting boven draaien midden boven
            int tempkleur = blokarray[1, 2, 2].KleurVoor;
            blokarray[1, 2, 2].KleurVoor = blokarray[1, 0, 2].KleurOnder;
            blokarray[1, 0, 2].KleurOnder = blokarray[1, 0, 0].KleurAchter;
            blokarray[1, 0, 0].KleurAchter = blokarray[1, 2, 0].KleurBoven;
            blokarray[1, 2, 0].KleurBoven = tempkleur;

            //richting boven draaien midden midden
            tempkleur = blokarray[1, 1, 2].KleurVoor;
            blokarray[1, 1, 2].KleurVoor = blokarray[1, 0, 1].KleurOnder;
            blokarray[1, 0, 1].KleurOnder = blokarray[1, 1, 0].KleurAchter;
            blokarray[1, 1, 0].KleurAchter = blokarray[1, 2, 1].KleurBoven;
            blokarray[1, 2, 1].KleurBoven = tempkleur;

            //richting boven draaien midden onder
            tempkleur = blokarray[1, 0, 2].KleurVoor;
            blokarray[1, 0, 2].KleurVoor = blokarray[1, 0, 0].KleurOnder;
            blokarray[1, 0, 0].KleurOnder = blokarray[1, 2, 0].KleurAchter;
            blokarray[1, 2, 0].KleurAchter = blokarray[1, 2, 2].KleurBoven;
            blokarray[1, 2, 2].KleurBoven = tempkleur;

            Teken();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            //richting boven draaien links boven
            int tempkleur = blokarray[2, 2, 2].KleurVoor;
            blokarray[2, 2, 2].KleurVoor = blokarray[2, 0, 2].KleurOnder;
            blokarray[2, 0, 2].KleurOnder = blokarray[2, 0, 0].KleurAchter;
            blokarray[2, 0, 0].KleurAchter = blokarray[2, 2, 0].KleurBoven;
            blokarray[2, 2, 0].KleurBoven = tempkleur;

            //richting boven draaien links midden
            tempkleur = blokarray[2, 1, 2].KleurVoor;
            blokarray[2, 1, 2].KleurVoor = blokarray[2, 0, 1].KleurOnder;
            blokarray[2, 0, 1].KleurOnder = blokarray[2, 1, 0].KleurAchter;
            blokarray[2, 1, 0].KleurAchter = blokarray[2, 2, 1].KleurBoven;
            blokarray[2, 2, 1].KleurBoven = tempkleur;

            //richting boven draaien links onder
            tempkleur = blokarray[2, 0, 2].KleurVoor;
            blokarray[2, 0, 2].KleurVoor = blokarray[2, 0, 0].KleurOnder;
            blokarray[2, 0, 0].KleurOnder = blokarray[2, 2, 0].KleurAchter;
            blokarray[2, 2, 0].KleurAchter = blokarray[2, 2, 2].KleurBoven;
            blokarray[2, 2, 2].KleurBoven = tempkleur;

            //richting boven draaien linker zijkant hoeken
            tempkleur = blokarray[2, 2, 2].KleurLinks;
            blokarray[2, 2, 2].KleurLinks = blokarray[2, 0, 2].KleurLinks;
            blokarray[2, 0, 2].KleurLinks = blokarray[2, 0, 0].KleurLinks;
            blokarray[2, 0, 0].KleurLinks = blokarray[2, 2, 0].KleurLinks;
            blokarray[2, 2, 0].KleurLinks = tempkleur;

            //richting boven draaien linker zijkant midden
            tempkleur = blokarray[2, 1, 2].KleurLinks;
            blokarray[2, 1, 2].KleurLinks = blokarray[2, 0, 1].KleurLinks;
            blokarray[2, 0, 1].KleurLinks = blokarray[2, 1, 0].KleurLinks;
            blokarray[2, 1, 0].KleurLinks = blokarray[2, 2, 1].KleurLinks;
            blokarray[2, 2, 1].KleurLinks = tempkleur;
                

            Teken();
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            //Hele voorkant met klok mee draaien voorkant hoeken
            int tempkleur = blokarray[2, 2, 2].KleurVoor;
            blokarray[2, 2, 2].KleurVoor = blokarray[2, 0, 2].KleurVoor;
            blokarray[2, 0, 2].KleurVoor = blokarray[0, 0, 2].KleurVoor;
            blokarray[0, 0, 2].KleurVoor = blokarray[0, 2, 2].KleurVoor;
            blokarray[0, 2, 2].KleurVoor = tempkleur;

            //Voorkant met klok mee draaien voorkant midden
            tempkleur = blokarray[1, 2, 2].KleurVoor;
            blokarray[1, 2, 2].KleurVoor = blokarray[2, 1, 2].KleurVoor;
            blokarray[2, 1, 2].KleurVoor = blokarray[1, 0, 2].KleurVoor;
            blokarray[1, 0, 2].KleurVoor = blokarray[0, 1, 2].KleurVoor;
            blokarray[0, 1, 2].KleurVoor = tempkleur;

            //voorkant met klok mee, bovenlinks
            tempkleur = blokarray[2, 2, 2].KleurBoven;
            blokarray[2, 2, 2].KleurBoven = blokarray[2, 0, 2].KleurLinks;
            blokarray[2, 0, 2].KleurLinks = blokarray[0, 0, 2].KleurOnder;
            blokarray[0, 0, 2].KleurOnder = blokarray[0, 2, 2].KleurRechts;
            blokarray[0, 2, 2].KleurRechts = tempkleur;

            //voorkant met klok mee, boven midden
            tempkleur = blokarray[1, 2, 2].KleurBoven;
            blokarray[1, 2, 2].KleurBoven = blokarray[2, 1, 2].KleurLinks;
            blokarray[2, 1, 2].KleurLinks = blokarray[1, 0, 2].KleurOnder;
            blokarray[1, 0, 2].KleurOnder = blokarray[0, 1, 2].KleurRechts;
            blokarray[0, 1, 2].KleurRechts = tempkleur;

            //voorkant met klok mee, boven rechts
            tempkleur = blokarray[0, 2, 2].KleurBoven;
            blokarray[0, 2, 2].KleurBoven = blokarray[2, 2, 2].KleurLinks;
            blokarray[2, 2, 2].KleurLinks = blokarray[2, 0, 2].KleurOnder;
            blokarray[2, 0, 2].KleurOnder = blokarray[0, 0, 2].KleurRechts;
            blokarray[0, 0, 2].KleurRechts = tempkleur;

            Teken();
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            //Middelste rij met klok mee, linksboven
            int tempkleur = blokarray[2, 2, 1].KleurBoven;
            blokarray[2, 2, 1].KleurBoven = blokarray[2, 0, 1].KleurLinks;
            blokarray[2, 0, 1].KleurLinks = blokarray[0, 0, 1].KleurOnder;
            blokarray[0, 0, 1].KleurOnder = blokarray[0, 2, 1].KleurRechts;
            blokarray[0, 2, 1].KleurRechts = tempkleur;

            //middelste rij met klok mee, boven midden
            tempkleur = blokarray[1, 2, 1].KleurBoven;
            blokarray[1, 2, 1].KleurBoven = blokarray[2, 1, 1].KleurLinks;
            blokarray[2, 1, 1].KleurLinks = blokarray[1, 0, 1].KleurOnder;
            blokarray[1, 0, 1].KleurOnder = blokarray[0, 1, 1].KleurRechts;
            blokarray[0, 1, 1].KleurRechts = tempkleur;

            //middelste rij met klok mee, boven rechts
            tempkleur = blokarray[0, 2, 1].KleurBoven;
            blokarray[0, 2, 1].KleurBoven = blokarray[2, 2, 1].KleurLinks;
            blokarray[2, 2, 1].KleurLinks = blokarray[2, 0, 1].KleurOnder;
            blokarray[2, 0, 1].KleurOnder = blokarray[0, 0, 1].KleurRechts;
            blokarray[0, 0, 1].KleurRechts = tempkleur;

            Teken();
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            //Achterkant met klok mee, boven links
            int tempkleur = blokarray[2, 2, 0].KleurBoven;
            blokarray[2, 2, 0].KleurBoven = blokarray[2, 0, 0].KleurLinks;
            blokarray[2, 0, 0].KleurLinks = blokarray[0, 0, 0].KleurOnder;
            blokarray[0, 0, 0].KleurOnder = blokarray[0, 2, 0].KleurRechts;
            blokarray[0, 2, 0].KleurRechts = tempkleur;

            //Achterkant met klok mee boven midden
            tempkleur = blokarray[1, 2, 0].KleurBoven;
            blokarray[1, 2, 0].KleurBoven = blokarray[2, 1, 0].KleurLinks;
            blokarray[2, 1, 0].KleurLinks = blokarray[1, 0, 0].KleurOnder;
            blokarray[1, 0, 0].KleurOnder = blokarray[0, 1, 0].KleurRechts;
            blokarray[0, 1, 0].KleurRechts = tempkleur;

            //Achterkant met klok mee, boven rechts
            tempkleur = blokarray[0, 2, 0].KleurBoven;
            blokarray[0, 2, 0].KleurBoven = blokarray[2, 2, 0].KleurLinks;
            blokarray[2, 2, 0].KleurLinks = blokarray[2, 0, 0].KleurOnder;
            blokarray[2, 0, 0].KleurOnder = blokarray[0, 0, 0].KleurRechts;
            blokarray[0, 0, 0].KleurRechts = tempkleur;

            //Achterkant met klok mee, achterkant hoeken
            tempkleur = blokarray[2, 2, 0].KleurAchter;
            blokarray[2, 2, 0].KleurAchter = blokarray[2, 0, 0].KleurAchter;
            blokarray[2, 0, 0].KleurAchter = blokarray[0, 0, 0].KleurAchter;
            blokarray[0, 0, 0].KleurAchter = blokarray[0, 2, 0].KleurAchter;
            blokarray[0, 2, 0].KleurAchter = tempkleur;

            //achterkant met klok mee, achterkant midden
            tempkleur = blokarray[1, 2, 0].KleurAchter;
            blokarray[1, 2, 0].KleurAchter = blokarray[2, 1, 0].KleurAchter;
            blokarray[2, 1, 0].KleurAchter = blokarray[1, 0, 0].KleurAchter;
            blokarray[1, 0, 0].KleurAchter = blokarray[0, 1, 0].KleurAchter;
            blokarray[0, 1, 0].KleurAchter = tempkleur;

            Teken();
        }


    }

    void Teken()
    {
        var blokjes = GameObject.FindGameObjectsWithTag("blokje");
        foreach (var blokje in blokjes)
        {
            Destroy(blokje);
        }

        for (int i = 0; i < blokarray.GetLength(0);i++)
        {
            for (int j = 0; j < blokarray.GetLength(1); j++)
            {
                for (int k = 0; k < blokarray.GetLength(2); k++)
                {
                    if(blokarray[i, j, k] != null)
                    {
                        Blok HuidigBlok = blokarray[i, j, k];
                        GameObject newCube = Instantiate(prefab, new Vector3(HuidigBlok.posX, HuidigBlok.posY, HuidigBlok.posZ), Quaternion.identity);
                        newCube.transform.Find("Bovenblokje").GetComponent<Renderer>().material.color = Colors[HuidigBlok.KleurBoven];
                        newCube.transform.Find("Onderblokje").GetComponent<Renderer>().material.color = Colors[HuidigBlok.KleurOnder];
                        newCube.transform.Find("Voorblokje").GetComponent<Renderer>().material.color = Colors[HuidigBlok.KleurVoor];
                        newCube.transform.Find("Achterblokje").GetComponent<Renderer>().material.color = Colors[HuidigBlok.KleurAchter];
                        newCube.transform.Find("Linksblokje").GetComponent<Renderer>().material.color = Colors[HuidigBlok.KleurLinks];
                        newCube.transform.Find("Rechtsblokje").GetComponent<Renderer>().material.color = Colors[HuidigBlok.KleurRechts];
                    }
                }
            }
        }
    }

}

public class Blok
{
    public int KleurVoor;
    public int KleurAchter;
    public int KleurBoven;
    public int KleurOnder;
    public int KleurLinks;
    public int KleurRechts;

    public int posX;
    public int posY;
    public int posZ;

    public Blok(int x, int y, int z, int kleurvoor, int kleurachter, int kleurboven, int kleuronder, int kleurlinks, int kleurrechts)
    {
        posX = x;
        posY = y;
        posZ = z;

        KleurVoor = kleurvoor;
        KleurAchter = kleurachter;
        KleurBoven = kleurboven;
        KleurOnder = kleuronder;
        KleurLinks = kleurlinks;
        KleurRechts = kleurrechts;
    }
}
