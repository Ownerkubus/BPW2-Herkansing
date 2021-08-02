using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject Enemy;

    public DungeonGenerator dungeonGenerator;

    public Room room;

    // Start is called before the first frame update
    private void Awake()
    {
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();
    }

    void Start()
    {
        dungeonGenerator.OnDungeonGenerationDone += SpawnEnemy;
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        GameObject newEnemy;
        newEnemy = Instantiate(Enemy, new Vector3(room.position.x, 0.3f, room.position.y), Quaternion.identity);
    }

}
