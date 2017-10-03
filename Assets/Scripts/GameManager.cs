using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	private bool _gameStarted = false;
	public bool gameStarted { get { return _gameStarted; } }

	private bool _gameEnded = false;
	public bool gameEnded { get { return _gameEnded; } }

	private bool readyToRestart = false;

	[SerializeField] private Physics2DRaycaster rayCaster;

	private SpawnManager spawnManager;
	private BombManager bombManager;
	private ScoreManager scoreManager;
	private EndManager endManager;
	private AudioManager audioManager;

	[SerializeField] private GameObject startOverlay;
	[SerializeField] private SceneFader sceneFader;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	void Start() {
		spawnManager = SpawnManager.instance;
		bombManager = BombManager.instance;
		scoreManager = ScoreManager.instance;
		endManager = EndManager.instance;
		audioManager = AudioManager.instance;

		spawnManager.enabled = false;
	}

	void Update() {
		if (Input.GetButtonDown("Fire1")) {
			if (!_gameStarted) {
				audioManager.PlaySound("ClickSound");
				_gameStarted = true;
				StartGame();
			} else if (readyToRestart) {
				audioManager.PlaySound("ClickSound");
				sceneFader.FadeTo(SceneManager.GetActiveScene().name);
			} else if (scoreManager.showingScore) {
				scoreManager.SkipShowScore();
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Application.platform == RuntimePlatform.Android) {
				AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				activity.Call<bool>("moveTaskToBack", true);
			} else {
				Application.Quit();
			}
		}
	}

	private void StartGame() {
		spawnManager.enabled = true;
		startOverlay.SetActive(false);
	}

	public void AddScore(int amount) {
		scoreManager.AddScore(amount);
	}

	public void EndGame(int procedure, Bomb bomb) {
		List<Bomb> bombs = bombManager.bombs;
		for (int i = 0; i < bombs.Count; i++) {
			bombs[i].GetComponent<Draggable>().enabled = false;
		}

		_gameEnded = true;
		StopAllMovement();
		endManager.EndGame(procedure, bomb);
	}

	public void SetRestart() {
		readyToRestart = true;
	}

	public void StopAllMovement() {
		spawnManager.enabled = false;
		rayCaster.enabled = false;

		List<Bomb> bombs = bombManager.bombs;
		List<Bomb> bombsInGrey = bombManager.bombsInGrey;
		List<Bomb> bombsInRed = bombManager.bombsInRed;

		for (int i = 0; i < bombs.Count; i++) {
			bombs[i].canMove = false;
			bombs[i].bombFuseSound.Pause();
		}
		for (int i = 0; i < bombsInGrey.Count; i++) {
			bombsInGrey[i].canMove = false;
		}
		for (int i = 0; i < bombsInRed.Count; i++) {
			bombsInRed[i].canMove = false;
		}
	}

	public void StartAllMovement() {
		spawnManager.enabled = true;
		rayCaster.enabled = true;

		List<Bomb> bombs = bombManager.bombs;
		List<Bomb> bombsInGrey = bombManager.bombsInGrey;
		List<Bomb> bombsInRed = bombManager.bombsInRed;

		for (int i = 0; i < bombs.Count; i++) {
			bombs[i].canMove = true;
			bombs[i].bombFuseSound.UnPause();
		}
		for (int i = 0; i < bombsInGrey.Count; i++) {
			bombsInGrey[i].canMove = true;
		}
		for (int i = 0; i < bombsInRed.Count; i++) {
			bombsInRed[i].canMove = true;
		}
	}
}
