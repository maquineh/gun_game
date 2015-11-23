using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public Wave[] waves;
	public Enemy enemy;

	Wave currentWave;
	int currentWaveNumber;

	int enemiesRemainingToSpawn;
	int enemiesRemainingAlive;
	float nextSpawnTime;

    LivingObject playerEnt;

    Transform playerT;

    MapGenerator map;

    float timeEntreCamperChecks = 2;
    float camperThresholdDistance = 1.5f; 
    float nextCamperCheckTime;
    Vector3 camperPositionAnt;
    bool isCamper;

    bool isDisabled;

    public event System.Action<int> OnThisWave;


	[System.Serializable]
	public class Wave{
		public int enemyCounter;
		public float timeEachRespawn;
	}

	void Start(){
        playerEnt = FindObjectOfType<Player>();
        playerT = playerEnt.transform;

        nextCamperCheckTime = timeEntreCamperChecks + Time.time;
        camperPositionAnt = playerT.position;
        playerEnt.OnDeath += OnPlayerDeath;
        map = FindObjectOfType<MapGenerator>();
		NextWave ();
	}

	void Update(){
        if (!isDisabled){
            if (Time.time > nextCamperCheckTime)
            {
                nextCamperCheckTime = Time.time + timeEntreCamperChecks;

                isCamper = (Vector3.Distance(playerT.position, camperPositionAnt) < camperThresholdDistance);
                camperPositionAnt = playerT.position;
            }

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeEachRespawn;

                StartCoroutine(SpawnEnemy());
            }
        }
	}

    IEnumerator SpawnEnemy() {

        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        Transform spawnTile = map.GetRandomOpenTile();

        if (isCamper) {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnerTime = 0;

        while(spawnerTime < spawnDelay){

            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnerTime* tileFlashSpeed, 1));

            spawnerTime += Time.deltaTime;
            yield return null;
        }

        Enemy spawnEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnEnemy.OnDeath += OnDeathEnemy;
    }

    void OnPlayerDeath() {
        isDisabled = true; 
    }

	void OnDeathEnemy(){
		enemiesRemainingAlive--;
		if(enemiesRemainingAlive == 0){
			NextWave();
		}
		print("Enemy died");
	}

	void NextWave(){

		currentWaveNumber++;
		if (currentWaveNumber - 1 < waves.Length) {

			currentWave = waves [currentWaveNumber - 1];

			enemiesRemainingToSpawn = currentWave.enemyCounter;
			enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnThisWave != null) {
                OnThisWave(currentWaveNumber);
            }

            ResetPosicaoPlayer();
        }
	}

    void ResetPosicaoPlayer() {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;

    }

}
