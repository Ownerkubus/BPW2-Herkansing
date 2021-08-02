using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneratorMenu : MonoBehaviour
{
    public enum Tile { Floor }

    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public int amountRooms;
    public int width = 100;
    public int height = 100;
    public int minRoomSize = 3;
    public int maxRoomSize = 7;
    public Room beginKamer;

    public System.Action OnDungeonGenerationDone;



    private Dictionary<Vector2Int, Tile> dungeonDictionary = new Dictionary<Vector2Int, Tile>();
    private List<Room> roomList = new List<Room>();
    private List<GameObject> allSpawnedObjects = new List<GameObject>();

    void Start()
    {
        GenerateDungeon();
        StartCoroutine(Coroutine());
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Input.GetKeyDown(KeyCode.G))
        {
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(1);
        Application.LoadLevel(Application.loadedLevel);
    }

        private void AllocateRooms()
    {
        for (int i = 0; i < amountRooms; i++)
        {
            Room room = new Room()
            {
                position = new Vector2Int(Random.Range(0, width), Random.Range(0, height)),
                size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize))
            };

            if (CheckIfRoomFitsInDungeon(room))
            {
                AddRoomToDungeon(room);
            }
            else
            {
                i--;
            }
        }
    }

    private void AddRoomToDungeon(Room room)
    {
        for (int xx = room.position.x; xx < room.position.x + room.size.x; xx++)
        {
            for (int yy = room.position.y; yy < room.position.y + room.size.y; yy++)
            {
                Vector2Int pos = new Vector2Int(xx, yy);
                dungeonDictionary.Add(pos, Tile.Floor);
            }
        }
        roomList.Add(room);

    }

    private bool CheckIfRoomFitsInDungeon(Room room)
    {
        for (int xx = room.position.x; xx < room.position.x + room.size.x; xx++)
        {
            for (int yy = room.position.y; yy < room.position.y + room.size.y; yy++)
            {
                Vector2Int pos = new Vector2Int(xx, yy);
                if (dungeonDictionary.ContainsKey(pos))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void AllocateCorridors()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            beginKamer = roomList[0];
            Room startRoom = roomList[i];
            Room otherRoom = roomList[(i + Random.Range(1, roomList.Count - 1)) % roomList.Count];

            int dirX = Mathf.RoundToInt(Mathf.Sign(otherRoom.position.x - startRoom.position.x));
            for (int x = startRoom.position.x; x != otherRoom.position.x; x += dirX)
            {
                Vector2Int pos = new Vector2Int(x, startRoom.position.y);
                if (!dungeonDictionary.ContainsKey(pos))
                {
                    dungeonDictionary.Add(pos, Tile.Floor);
                }
            }

            int dirY = Mathf.RoundToInt(Mathf.Sign(otherRoom.position.y - startRoom.position.y));
            for (int y = startRoom.position.y; y != otherRoom.position.y; y += dirY)
            {
                Vector2Int pos = new Vector2Int(otherRoom.position.x, y);
                if (!dungeonDictionary.ContainsKey(pos))
                {
                    dungeonDictionary.Add(pos, Tile.Floor);
                }
            }

        }
    }

    private void BuildDungeon()
    {
        foreach (KeyValuePair<Vector2Int, Tile> kv in dungeonDictionary)
        {
            GameObject floor = Instantiate(FloorPrefab, new Vector3Int(kv.Key.x, 0, kv.Key.y), Quaternion.identity);
            allSpawnedObjects.Add(floor);

            SpawnWallsForTile(kv.Key);
        }
    }

    private void SpawnWallsForTile(Vector2Int position)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(z)) { continue; }
                Vector2Int gridPos = position + new Vector2Int(x, z);
                if (!dungeonDictionary.ContainsKey(gridPos))
                {
                    Vector3 direction = new Vector3(gridPos.x, 0, gridPos.y) - new Vector3(position.x, 0, position.y);
                    GameObject wall = Instantiate(WallPrefab, new Vector3(position.x, 0, position.y), Quaternion.LookRotation(direction));
                    allSpawnedObjects.Add(wall);
                }
            }
        }
    }

    public void GenerateDungeon()
    {
        AllocateRooms();
        AllocateCorridors();
        BuildDungeon();
        OnDungeonGenerationDone?.Invoke();
    }
}

public class RoomMenu
{
    public Vector2Int position;
    public Vector2Int size;
}

