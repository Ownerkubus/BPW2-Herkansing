using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public enum Tile { Floor }

    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public int amountRooms;
    public int width = 100;
    public int height = 100;
    public int minRoomSize = 3;
    public int maxRoomSize = 7;
    public GameObject player;
    public Vector3 playerStartPosition;
    public Room beginKamer;
    public GameObject EnemySpawner;

    public System.Action OnDungeonGenerationDone;
    


    private Dictionary<Vector2Int, Tile> dungeonDictionary = new Dictionary<Vector2Int, Tile>();
    private List<Room> roomList = new List<Room>();
    private List<GameObject> allSpawnedObjects = new List<GameObject>();

    void Start()
    {
        GenerateDungeon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
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
        GameObject EnemySpawn = Instantiate(EnemySpawner, new Vector3(room.position.x + room.size.x / 2, 0.3f, room.position.y + room.size.y / 2), EnemySpawner.transform.rotation);
        EnemySpawn.GetComponent<EnemySpawner>().room = room;
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
            for(int x = startRoom.position.x; x != otherRoom.position.x; x += dirX)
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
        foreach(KeyValuePair<Vector2Int, Tile> kv in dungeonDictionary)
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
                if(Mathf.Abs(x) == Mathf.Abs(z)) { continue; }
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
        GameObject newPlayer;
        newPlayer = Instantiate(player, new Vector3(beginKamer.position.x + beginKamer.size.x / 2, 0.3f, beginKamer.position.y + beginKamer.size.y / 2), player.transform.rotation);
        newPlayer.SetActive(true);
        OnDungeonGenerationDone?.Invoke();

    }
}

public class Room
{
    public Vector2Int position;
    public Vector2Int size;
}

