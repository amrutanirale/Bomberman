using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public static MapGeneration Instance = null;
    public GameObject indestructibleWallPrefab, destructibleWallPrefab, aiPrefab, floorPrefab;
    public int row = 7;
    public int column = 7;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        GenerateFloor();
        GenerateBorder();
        GenerateLevel();
    }

    public void GenerateFloor()
    {
        floorPrefab.transform.position = new Vector3(row / 2, -0.5f, column / 2);
        floorPrefab.transform.localScale = new Vector3(row / 5, 1, column / 5);
        floorPrefab.GetComponent<Renderer>().material.mainTextureScale = new Vector2((float)row / 2, (float)column / 2);
    }
    public void GenerateBorder()
    {
        for (int i = -1; i <= row; i++)
        {
            for (int j = -1; j <= column; j++)
            {
                if (i == -1 || j == -1 || i == row || j == column)
                {
                    GameObject iWall = Instantiate(indestructibleWallPrefab, new Vector3(i, 0, j), Quaternion.identity, transform.GetChild(0).transform);
                }
            }
        }
    }

    public void GenerateLevel()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (i == 0 && j == 0 || i == 0 && j == 1 || i == 1 && j == 0)
                {
                    continue;
                }
                if (i == 0 && j == column - 1 || i == 0 && j == column - 2 || i == 1 && j == column - 1)
                {
                    continue;
                }
                if (i == row - 2 && j == 0 || i == row - 1 && j == 0 || i == row - 1 && j == 1)
                {
                    continue;
                }
                if (i == row - 1 && j == column - 1 || i == row - 2 && j == column - 1 || i == row - 1 && j == column - 2)
                {
                    continue;
                }

                if (i % 2 == 0 || j % 2 == 0)
                {
                    if (Random.value <= 0.3f)
                    {
                        if (Random.value <= 0.2f)
                        {
                            GameObject ai = Instantiate(aiPrefab, new Vector3(i, 0, j), Quaternion.identity, transform.GetChild(2).transform);
                        }
                        else
                        {
                            GameObject dwall = Instantiate(destructibleWallPrefab, new Vector3(i, 0, j), Quaternion.identity, transform.GetChild(1).transform);
                        }
                    }
                }

            }
        }

        for (int i = 1; i < row; i += 2)
        {
            for (int j = 1; j < column; j += 2)
            {
                GameObject iWall = Instantiate(indestructibleWallPrefab, new Vector3(i, 0, j), Quaternion.identity, transform.GetChild(0).transform);
            }
        }
    }
}
