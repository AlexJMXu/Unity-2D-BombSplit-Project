using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	public static ScoreManager instance;

	private int score = 0;
	[SerializeField] private Text scoreCount;
	[SerializeField] private Text highscoreText;
	[SerializeField] private GameObject restartText;

	private GameManager gameManager;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	void Start() {
		gameManager = GameManager.instance;
	}

	public void AddScore(int amount) {
		score += amount;
	}

	public void ShowScore() {
		StartCoroutine(ShowScoreCoroutine());
	}

	private IEnumerator ShowScoreCoroutine() {
		int counter = 0;

		while (counter < score) {
			counter++;
			scoreCount.text = counter.ToString();
			yield return new WaitForSeconds(0.1f);
		}

		int oldHighscore = PlayerPrefs.GetInt("highscore");
		if (score > oldHighscore) {
			PlayerPrefs.SetInt("highscore", score);
			highscoreText.text = "Highscore: " + score.ToString();
		} else {
			highscoreText.text = "Highscore: " + oldHighscore.ToString();
		}

		highscoreText.gameObject.SetActive(true);
		restartText.SetActive(true);
		gameManager.SetRestart();
	}
}
