using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

	public static SpawnManager instance;

	[SerializeField] private Transform[] spawnPoint;
	[SerializeField] private GameObject[] bombType;

	private float spawnTimer = 3f;
	private float spawnCounter = 0;
	private int spawnAmount = 1;

	[SerializeField] private Animator[] doorAnimator;

	private BombManager bombManager;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	void Start() {
		bombManager = BombManager.instance;
	}
	
	void Update () {
		spawnCounter += Time.deltaTime;
		if (spawnCounter >= spawnTimer) {
			for (int i = 0; i < spawnAmount; i++) {
				int point = Random.Range(0, spawnPoint.Length);
				int type = Random.Range(0, bombType.Length);

				doorAnimator[point].Play("OpenClose");

				GameObject go = (GameObject) Instantiate(bombType[type], spawnPoint[point].position, Quaternion.identity);
				bombManager.AddBomb(go.GetComponent<Bomb>());
			}

			spawnTimer -= 0.05f;
			spawnCounter = 0;

			if (spawnTimer <= 1f) {
				spawnTimer = 3f;
				spawnAmount++;
			}
		}
	}
}
