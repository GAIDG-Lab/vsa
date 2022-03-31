using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{

    [SerializeField] private GameObject obstacle;
    public int enviromentSize = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void GenerateObstacle()
    {
        for (int x = -enviromentSize + 1; x <= enviromentSize; x += 2)
        {
            for (int y = -enviromentSize + 1; y <= enviromentSize; y += 2)
            {
                if (Random.value > 0.7f)
                {
                    Vector3 pos = new Vector3(x, 1f, y);
                    Instantiate(obstacle, pos, Quaternion.identity);
                }
            }
        }
    }
}
