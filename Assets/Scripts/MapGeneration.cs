using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public GameObject indestructibleWallPrefab,destructibleWallPrefab , aiPrefab;
    public int row=7;
    public int column=7;
    
    // Start is called before the first frame update
    void Start()
    {
        PlaceWall();
    }


    public void PlaceWall()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (i == 0 && j == 0 || i == 0 && j == 1 || i == 1 && j == 0)
                {
                    continue;
                }
                if (i == 0 && j == column-1 || i == 0 && j == column - 2 || i == 1 && j == column-1)
                {
                    continue;
                }
                if (i == row-2 && j == 0 || i == row - 1 && j == 0||i==row-1&&j==1)
                {
                    continue;
                }
                if (i == row - 1 && j == column-1 || i == row - 2 && j == column-1 || i == row - 1 && j == column - 2 )
                {
                    continue;
                }

                if (i % 2 == 0 || j % 2 == 0)
                {
                    if (Random.value <= 0.5f)
                    {
                        if (Random.value <= 0.2f)
                        {
                            GameObject ai = Instantiate(aiPrefab, new Vector3(i, 0, j), Quaternion.identity);
                        }
                        else
                        {
                            GameObject dwall = Instantiate(destructibleWallPrefab, new Vector3(i, 0, j), Quaternion.identity);
                        }
                    }
                }
                
            }
        }

        for (int i = 1; i < row; i += 2)
        {
            for (int j = 1; j < column; j += 2)
            {
                GameObject iWall = Instantiate(indestructibleWallPrefab, new Vector3(i, 0, j), Quaternion.identity);
            }
        }
    }
}
