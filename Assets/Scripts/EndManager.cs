using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndManager : MonoBehaviour {

	public static EndManager instance;

	[SerializeField] private GameObject endOverlay;

	private ScoreManager scoreManager;
	private BombManager bombManager;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	void Start() {
		scoreManager = ScoreManager.instance;
		bombManager = BombManager.instance;
	}

	public void EndGame(int procedure, Bomb bomb) {
		StartCoroutine(EndGameProcedure(procedure, bomb));
	}

	private IEnumerator EndGameProcedure(int procedure, Bomb bomb) {
		yield return new WaitForSeconds(1f);

		bombManager.bombs.Remove(bomb);
		bomb.Explode();
		bombManager.RemoveFromOutside(bomb);

		switch (procedure) {
			case 0:
				yield return new WaitForSeconds(0.5f);
				bombManager.DetonateBombsOutside();
				yield return new WaitUntil(() => bombManager.outsideZone == 0);
				bombManager.CollectGreyBombs();
				bombManager.CollectRedBombs();
				yield return new WaitUntil(() => bombManager.insideGrey == 0);
				yield return new WaitUntil(() => bombManager.insideRed == 0);
				scoreManager.ShowScore();
				break;
			case 1:
				yield return new WaitForSeconds(0.5f);
				bombManager.DetonateBombsOutside();
				bombManager.DetonateBombsInGrey();
				yield return new WaitUntil(() => bombManager.outsideZone == 0);
				yield return new WaitUntil(() => bombManager.insideGrey == 0);
				bombManager.CollectRedBombs();
				yield return new WaitUntil(() => bombManager.insideRed == 0);
				scoreManager.ShowScore();
				break;
			case 2:
				yield return new WaitForSeconds(0.5f);
				bombManager.DetonateBombsOutside();
				bombManager.DetonateBombsInRed();
				yield return new WaitUntil(() => bombManager.outsideZone == 0);
				yield return new WaitUntil(() => bombManager.insideRed == 0);
				bombManager.CollectGreyBombs();
				yield return new WaitUntil(() => bombManager.insideGrey == 0);
				scoreManager.ShowScore();
				break;
		}

		endOverlay.SetActive(true);
	}
}
