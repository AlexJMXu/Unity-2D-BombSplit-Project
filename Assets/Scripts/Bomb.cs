using UnityEngine;

public class Bomb : MonoBehaviour {

	private float speed = 1f;
	private Vector2 direction;

	[SerializeField] private int type;
	private Transform target;

	public bool beingDragged = false;
	public bool canMove = true;
	public bool beingCollected = false;

	private bool isSafe = false;

	private float explodeTimer = 7f;
	private float explodeCounter = 0;

	[SerializeField] private GameObject warningCanvas;
	[SerializeField] private GameObject explosionParticles;
	[SerializeField] private GameObject fuseParticles;
	[SerializeField] private LayerMask whatToHit;

	private GameManager gameManager;
	private BombManager bombManager;
	private AudioManager audioManager;

	public AudioSource bombFuseSound;

	void Start () {
		gameManager = GameManager.instance;
		bombManager = BombManager.instance;
		audioManager = AudioManager.instance;

		direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		fuseParticles.SetActive(true);
		bombFuseSound.Play();
	}
	
	void Update () {
		if (!canMove || isSafe || beingDragged) return;

		explodeCounter += Time.deltaTime;
		if (explodeCounter >= 3.5f) {
			warningCanvas.SetActive(true);
		}

		if (explodeCounter >= explodeTimer) {
			gameManager.EndGame(0, this);
		}
	}

	void FixedUpdate() {
		// Set z pos to y pos to create a sense of depth in this 2D view
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
		
		if (beingCollected) MoveToTarget();
		else Move();
	}

	private void Move() {
		if (beingDragged || !canMove) return;

		transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
	}

	public void Explode() {
		audioManager.PlaySound("ExplosionSound");
		GameObject go = (GameObject) Instantiate(explosionParticles, transform.position, Quaternion.identity);
		Destroy(go, 3);
		Destroy(this.gameObject);
	}

	public void SetTarget(Transform t) {
		target = t;
	}

	public void MoveToTarget() {
		transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
		if (Vector2.Distance(transform.position, target.position) <= 0.1) {
			if (type == 0) bombManager.RemoveFromGreyZone(this);
			else if (type == 1) bombManager.RemoveFromRedZone(this);
			Destroy(this.gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) {
			direction = Vector3.Reflect(direction, col.contacts[0].normal);
		}
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Greyzone") {
			if (type != 0) {
				gameManager.EndGame(1, this);
			} else {
				bombManager.AddToGreyZone(this);
				SetSafe();
			}
		} else if (col.gameObject.tag == "Redzone") {
			if (type != 1) {
				gameManager.EndGame(2, this);
			} else {
				bombManager.AddToRedZone(this);
				SetSafe();
			}
		}
	}

	void SetSafe() {
		fuseParticles.SetActive(false);
		isSafe = true;
		GetComponent<Draggable>().enabled = false;
		warningCanvas.SetActive(false);
		bombFuseSound.Stop();
	}
}
