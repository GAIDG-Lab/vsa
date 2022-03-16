using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoundaryControl : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject NorthWall;
    [SerializeField] private GameObject SouthWall;
    [SerializeField] private GameObject EastWall;
    [SerializeField] private GameObject WestWall;

    [SerializeField] private GameObject obstacle;
    
    public int enviromentSize = 10;

    public GameObject spawner;

    [SerializeField] NavMeshSurface[] navMeshSurfaces;

    private void Awake()
    {
        NorthWall.transform.position = new Vector3(NorthWall.transform.position.x, NorthWall.transform.position.y, enviromentSize);
        SouthWall.transform.position = new Vector3(SouthWall.transform.position.x, SouthWall.transform.position.y, enviromentSize * -1.0f);
        EastWall.transform.position = new Vector3(enviromentSize, EastWall.transform.position.y, EastWall.transform.position.z);
        WestWall.transform.position = new Vector3(enviromentSize * -1.0f, WestWall.transform.position.y, WestWall.transform.position.z);

        GenerateObstacle();

    }
    void Start()
    {

        for (int i = 0; i < navMeshSurfaces.Length; i++) {
            navMeshSurfaces[i].BuildNavMesh();
        }
        var agentSpawner = spawner.gameObject.GetComponent<RandomPointOnNavMesh>();

        agentSpawner.SpawnAgents(enviromentSize);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateObstacle() {
        for (int x = -enviromentSize + 1; x <= enviromentSize; x+=2) {
            for (int y = -enviromentSize + 1; y <= enviromentSize; y+=2) {
                if (Random.value > 0.7f) {
                    Vector3 pos = new Vector3(x, 1f, y);
                    Instantiate(obstacle, pos, Quaternion.identity, transform);
                }
            }
        }
    }

}
