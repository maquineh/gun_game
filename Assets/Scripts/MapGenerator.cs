using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

    public Transform tilePrefab;
    public Vector2 maxMapSize;

    public Transform navmeshmaskPrefab;
    public Transform navmeshFloor;
	public Transform mapFloor;
    public Transform obstaculoPrefab;
    public Transform[] obstaculosTumbaPrefab;
    
    [Range(0,1)]
    public float outlinePercent;

    public float tileSize;

    List<Coordenadas> todosTilesCoordenadas;

    Queue<Coordenadas> embaralhadoTilesCoords;
    Queue<Coordenadas> embaralhadoOpenTilesCoords;

    Map currentMap;

    public Map[] maps;

    public int mapIndex;

    Transform[,] tileMap;

    void OnThisWave(int wave) {
        mapIndex = wave - 1;
        generateMap();
    }

    public void generateMap(){
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random rnd = new System.Random(currentMap.seed);

        //GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);

        //gera as coordenadas do mapa
        todosTilesCoordenadas = new List<Coordenadas>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                todosTilesCoordenadas.Add(new Coordenadas(x,y));
            }
        }

        //Cria um objeto que nomeia o mapa
        embaralhadoTilesCoords = new Queue<Coordenadas>(Utility.ShuffleArray(todosTilesCoordenadas.ToArray(), currentMap.seed));
        //mapCentre = new Coordenadas((int)currentMap.mapSize.x / 2, (int)currentMap.mapSize.y / 2);
        string holderName = "Mapa gerado";

        if(transform.FindChild(holderName)){
            DestroyImmediate(transform.FindChild(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        //Gerador dos tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                Vector3 posicaoTile = CoordToPosition(x,y);//new Vector3(-mapSize.x/2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, posicaoTile, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent)*tileSize;
                newTile.parent = mapHolder;

                tileMap[x, y] = newTile;
            }
        }

        //gerador de obstaculos
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstaculosCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;

        List<Coordenadas> todasCoordenadasEmAberto = new List<Coordenadas>(todosTilesCoordenadas);
        for(int i = 0; i < obstaculosCount; i++){
            Coordenadas randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (!randomCoord.Equals(currentMap.mapCentre) && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleheight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float) rnd.NextDouble());
                
                Vector3 obstaculoPosition  = CoordToPosition(randomCoord.x, randomCoord.y);

                int random = Random.Range(0, 10);

               Transform currentObstacle = obstaculosTumbaPrefab[random];

                Transform newObstaculo = Instantiate(/*obstaculoPrefab*/currentObstacle, obstaculoPosition /*+ Vector3.up * obstacleheight/2*/, Quaternion.identity) as Transform;
                newObstaculo.parent = mapHolder;
                newObstaculo.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleheight, (1 - outlinePercent) * tileSize);
                
                Renderer obstacleRenderer = newObstaculo.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPerc = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPerc);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                todasCoordenadasEmAberto.Remove(randomCoord);
            }
            else{
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        embaralhadoOpenTilesCoords = new Queue<Coordenadas>(Utility.ShuffleArray(todasCoordenadasEmAberto.ToArray(), currentMap.seed));
		
        //gera o navmesh mask dinamico
        Transform maskLeft = Instantiate(navmeshmaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * 5;

        Transform maskRight = Instantiate(navmeshmaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * 5;

        Transform maskTop = Instantiate(navmeshmaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * 5;

        Transform maskBottom = Instantiate(navmeshmaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * 5;



        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;

		mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }


    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {

        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coordenadas> queue = new Queue<Coordenadas>();
        queue.Enqueue(currentMap.mapCentre);
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

        int accessibleTileCount = 1;

        while(queue.Count > 0){
            Coordenadas tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++){
                for (int y = -1; y <= 1;y++){
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    if(x ==0 || y==0){
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) { 
                            if(!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]){
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coordenadas(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }  
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x , int y){
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 position) {
        int x = Mathf.RoundToInt (position.x / tileSize + (currentMap.mapSize.x - 1) /2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);

        return tileMap[x, y];

    }

    public Coordenadas GetRandomCoord(){
        Coordenadas randomCoord = embaralhadoTilesCoords.Dequeue();
        embaralhadoTilesCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile() {
        Coordenadas randomCoord = embaralhadoOpenTilesCoords.Dequeue();
        embaralhadoOpenTilesCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y]; 
    }

     [System.Serializable]
    public struct Coordenadas{
        public int x;
        public int y;

        public Coordenadas(int _x, int _y){
            x = _x;
            y = _y;
        }
    }

    void Awake() {
        generateMap();
        FindObjectOfType<Spawner>().OnThisWave += OnThisWave;
    }

    [System.Serializable]
    public class Map{

        public Coordenadas mapSize;

        [Range(0,1)]
        public float obstaclePercent;

        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coordenadas mapCentre {

            get {
                return new Coordenadas(mapSize.x/2, mapSize.y/2);
            }

        }

    }

}
