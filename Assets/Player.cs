using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public float speed = 8f;
	
	Vector2Int currentDirection = Vector2Int.zero;
	
	float mouthAnimTime = 0f;
	
	GameObject visual;
	SpriteRenderer visualRenderer;
	public Sprite[] mouths;
	
	void Awake() {
		visual = transform.GetChild(0).gameObject;
		visualRenderer = visual.GetComponent<SpriteRenderer>();
	}
	
	// Version of sign that says "actually i don't like floating-point numbers
	// representing approximations, i actually want zero to have a sign of 0."
	int correctSign(float x) => x == 0f ? 0 : (int)Mathf.Sign(x);
	
	int directionToIndex(Vector2Int direction) {
		if (direction == Vector2Int.right) return 0;
		if (direction == Vector2Int.down) return 1;
		if (direction == Vector2Int.left) return 2;
		if (direction == Vector2Int.up) return 3;
		return 0;
	}
	
	Vector2 nearestTile(Vector2 v) => new Vector2( 
		Mathf.Round(v.x), Mathf.Round(v.y)
	);
	
	Vector2 rotate90(Vector2 v) => new Vector2(-v.y, v.x);
	
	bool tileInDirectionIsEmpty(Vector2 direction, float spread = 0.2f) =>
	DebugDrawSquare(nearestTile(transform.position + (Vector3)(direction - rotate90(direction) * spread)), 0.5f) &&
	DebugDrawSquare(nearestTile(transform.position + (Vector3)(direction + rotate90(direction) * spread)), 0.5f) &&
	Physics2D.OverlapBox(
		nearestTile(transform.position + (Vector3)(direction - rotate90(direction) * spread)),
		Vector2.one * 0.5f, 0f,
		1 << LayerMask.NameToLayer("Board")
	) == null && spread > 0f && Physics2D.OverlapBox(
		nearestTile(transform.position + (Vector3)(direction + rotate90(direction) * spread)),
		Vector2.one * 0.5f, 0f,
		1 << LayerMask.NameToLayer("Board")
	) == null;
	
	bool DebugDrawSquare(Vector2 position, float radius) {
		for (int i = 0; i < 2; i++) {
			Debug.DrawLine(
				position + new Vector2(-radius, (i > 0) ? radius : -radius),
				position + new Vector2(+radius, (i > 0) ? radius : -radius)
			);
			Debug.DrawLine(
				position + new Vector2((i > 0) ? radius : -radius, -radius),
				position + new Vector2((i > 0) ? radius : -radius, +radius)
			);
		}
		return true;
	}
	
	void Update() {
		Vector2 movef = new Vector2(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		);
		Vector2Int move = new Vector2Int(
			correctSign(movef.x), correctSign(movef.y)
		);
		
		bool allowHorizontal = currentDirection.x == 0;
		bool allowVertical = currentDirection.y == 0;
		bool changed = false;
		
		if (allowHorizontal) {
			if (move.x < 0 && tileInDirectionIsEmpty(Vector2.left)) { currentDirection = Vector2Int.left; changed = true; }
			if (move.x > 0 && tileInDirectionIsEmpty(Vector2.right)) { currentDirection = Vector2Int.right; changed = true; }
		}
		if (allowVertical) {
			// Nobody makes 2D games in +Y-up worlds, Unity.
			if (move.y < 0 && tileInDirectionIsEmpty(Vector2.down)) { currentDirection = Vector2Int.down; changed = true; }
			if (move.y > 0 && tileInDirectionIsEmpty(Vector2.up)) { currentDirection = Vector2Int.up; changed = true; }
		}
		
		if (changed)
			transform.position = nearestTile(transform.position);
		
		Collider2D facingCollider = Physics2D.OverlapBox(
			nearestTile(transform.position + (Vector3)(Vector2)currentDirection / 2f),
			Vector2.one * 0.49f, 0f,
			1 << LayerMask.NameToLayer("Board")
		);
		if (facingCollider) {
			currentDirection = Vector2Int.zero;
			transform.position = nearestTile(transform.position);
		} else if (currentDirection.sqrMagnitude > 0f) {
			transform.Translate((Vector2)currentDirection * speed * Time.deltaTime);
			mouthAnimTime += Time.deltaTime * speed;
			
			int dirIndex = directionToIndex(currentDirection);
			visualRenderer.sprite = mouths[dirIndex * 4 + Mathf.FloorToInt((mouthAnimTime % 1f) * mouths.Length / 4)];
		}
	}
}
