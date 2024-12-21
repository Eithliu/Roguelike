using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData 
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCellsList;

    public int Width, Height;
    public int minFood;
    public int maxFood;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public PlayerController Player;
    public FoodObject[] FoodPrefabs;
    public WallObject[] WallPrefabs;
    public ExitCellObject exitCellPrefab;

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();

        m_BoardData = new CellData[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                } else {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;

                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }
                
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }

        }
        Player.Spawn(this, new Vector2Int(1, 1));
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        addObject(Instantiate(exitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);


        GenerateWall();
        GenerateFood();
        GenerateFood();
    }
    void GenerateFood()
    {
        FoodObject foodPrefab;
        foodPrefab = FoodPrefabs[Random.Range(1, FoodPrefabs.Length)];
        int foodCount = Random.Range(minFood, maxFood);

        for (int i = 0; i < foodCount; i++)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject NewFood = Instantiate(foodPrefab);
            addObject(NewFood, coord);
        }
    }

    void GenerateWall()
    {   
        int wallCount = Random.Range(6, 10);

        for (int i = 0; i < wallCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);

            WallObject newWall = Instantiate(WallPrefabs[0]);
            addObject(newWall, coord);

            // Ici, ça génère un Step2, mais en même temps que Step1
            // Et ça ne détruit pas l'objet une fois son health à 0
            /*
            if (Player.Attacking() && i < 4)
            {
                Destroy(newWall);
                addObject(Instantiate(WallPrefabs[i]), coord);
            }
            */
        }
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }
        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    void addObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    public void Clean()
    {
        if (m_BoardData == null)
        {
            return;
        }
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }
                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }
}
