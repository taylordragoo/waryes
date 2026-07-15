using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace uSimRTS
{
    public class uSimRTS_MapGrid : MonoBehaviour
    {

        [System.Serializable]
        public class GridTile
        {
            public int posX;
            public int posY;            
            public GameObject tileObject;
        }
        [Tooltip("Set a Fog of war tile here")]
        public GameObject tileObjectPrefab;
        [Tooltip("Set the local scale of each tile")]
        public float tileSize;
        [Tooltip("Set the size of the fog of war grid.")]
        public Vector2 gridSize;
        public GridTile[,] gridObjects;

        public void InitializeGrid()
        {

            gridObjects = new GridTile[(int)gridSize.x, (int)gridSize.y];

            //gridPlane = transform.Find ("gridPlane").gameObject;
            //if (gridPlane != null)
            //DestroyImmediate (gridPlane);

            //gridPlane = (GameObject)Instantiate (gridModel, transform);

            //gridPlane.name = "gridPlane";

            SpawnTileObjects();

        }

        void SpawnTileObjects ()
        {
            for (int y = 0; y < gridObjects.GetLength(1); y++)
            {
                for (int x = 0; x < gridObjects.GetLength(0); x++)
                {
                    gridObjects[x, y] = new GridTile();
                    gridObjects[x, y].tileObject = Instantiate(tileObjectPrefab, transform);
                    gridObjects[x, y].tileObject.transform.localPosition = new Vector3(tileSize * x, 0f, tileSize * y);
                }
            }
         }

        // Start is called before the first frame update
        void Start()
        {
            InitializeGrid();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
