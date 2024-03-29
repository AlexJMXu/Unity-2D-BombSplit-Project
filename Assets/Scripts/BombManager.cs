﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour {

	public static BombManager instance;

	private List<Bomb> _bombs = new List<Bomb>();
	public List<Bomb> bombs { get { return _bombs; } }

	private List<Bomb> _bombsInGrey = new List<Bomb>();
	public List<Bomb> bombsInGrey { get { return _bombsInGrey; } }

	private List<Bomb> _bombsInRed = new List<Bomb>();
	public List<Bomb> bombsInRed { get { return _bombsInRed; } }

	private int _outsideZone = 0;
	public int outsideZone { get { return _outsideZone; } }

	private int _insideGrey = 0;
	public int insideGrey { get { return _insideGrey; } }
	
	private int _insideRed = 0;
	public int insideRed { get { return _insideRed; } }

	[SerializeField] private int limit = 20;
	private float waitTime = 1f;

	[SerializeField] private Animator[] doorAnimator;
	[SerializeField] private Transform[] collectionPoint;

	private GameManager gameManager;
	private AudioManager audioManager;

	void Awake() {
		if (instance == null) {
			instance = this;
		}
	}

	void Start() {
		gameManager = GameManager.instance;
		audioManager = AudioManager.instance;
	}


	public void DetonateBombsOutside() {
		StartCoroutine(DetonateBombsOutsideCoroutine());
	}

	public void DetonateBombsInGrey() {
		StartCoroutine(DetonateBombsInGreyCoroutine());
	}

	public void DetonateBombsInRed() {
		StartCoroutine(DetonateBombsInRedCoroutine());
	}

	public void CollectGreyBombs() {
		if (_insideGrey == 0) return;
		audioManager.PlaySound("WhistleSound");
		StartCoroutine(CollectGreyBombsCoroutine());
	}

	public void CollectRedBombs() {
		if (_insideRed == 0) return;
		audioManager.PlaySound("WhistleSound");
		StartCoroutine(CollectRedBombsCoroutine());
	}

	private IEnumerator DetonateBombsOutsideCoroutine() {
		for (int i = 0; i < _bombs.Count; i++) {
			yield return new WaitForSeconds(0.1f);
			_bombs[i].Explode();
			_outsideZone--;
		}

		_bombs.Clear();
	}

	private IEnumerator DetonateBombsInGreyCoroutine() {
		for (int i = 0; i < _bombsInGrey.Count; i++) {
			yield return new WaitForSeconds(0.1f);
			_bombsInGrey[i].Explode();
			_insideGrey--;
		}

		_bombsInGrey.Clear();
	}

	private IEnumerator DetonateBombsInRedCoroutine() {
		for (int i = 0; i < _bombsInRed.Count; i++) {
			yield return new WaitForSeconds(0.1f);
			_bombsInRed[i].Explode();
			_insideRed--;
		}

		_bombsInRed.Clear();
	}


	private IEnumerator CollectGreyBombsCoroutine() {
		gameManager.StopAllMovement();

		yield return new WaitForSeconds(waitTime);

		audioManager.PlaySound("DoorSound");
		doorAnimator[0].Play("Open");

		for (int i = 0; i < _bombsInGrey.Count; i++) {
			_bombsInGrey[i].SetTarget(collectionPoint[0]);
			_bombsInGrey[i].beingCollected = true;
			_bombsInGrey[i].gameObject.GetComponent<CircleCollider2D>().enabled = false;
		}

		yield return new WaitUntil(() => _insideGrey == 0);

		doorAnimator[0].Play("Close");

		_bombsInGrey.Clear();

		if (!gameManager.gameEnded) gameManager.StartAllMovement();
	}

	private IEnumerator CollectRedBombsCoroutine() {
		gameManager.StopAllMovement();

		yield return new WaitForSeconds(waitTime);

		audioManager.PlaySound("DoorSound");
		doorAnimator[1].Play("Open");

		for (int i = 0; i < _bombsInRed.Count; i++) {
			_bombsInRed[i].SetTarget(collectionPoint[1]);
			_bombsInRed[i].beingCollected = true;
			_bombsInRed[i].gameObject.GetComponent<CircleCollider2D>().enabled = false;
		}

		yield return new WaitUntil(() => _insideRed == 0);

		doorAnimator[1].Play("Close");

		_bombsInRed.Clear();

		if (!gameManager.gameEnded)  gameManager.StartAllMovement();
	}


	public void AddBomb(Bomb bomb) {
		_bombs.Add(bomb);
		_outsideZone++;
	}

	public void AddToGreyZone(Bomb bomb) {
		RemoveFromOutside(bomb);
		_bombsInGrey.Add(bomb);
		_insideGrey++;

		if (_insideGrey >= limit) {
			CollectGreyBombs();
		}
	}

	public void AddToRedZone(Bomb bomb) {
		RemoveFromOutside(bomb);
		_bombsInRed.Add(bomb);
		_insideRed++;

		if (_insideRed >= limit) {
			CollectRedBombs();
		}
	}

	public void RemoveFromOutside(Bomb bomb) {
		_bombs.Remove(bomb);
		_outsideZone--;
	}

	public void RemoveFromGreyZone(Bomb bomb) {
		_bombsInGrey.Remove(bomb);
		_insideGrey--;
		gameManager.AddScore(1);
	}

	public void RemoveFromRedZone(Bomb bomb) {
		_bombsInRed.Remove(bomb);
		_insideRed--;
		gameManager.AddScore(1);
	}
}
