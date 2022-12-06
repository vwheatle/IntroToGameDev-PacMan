using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	GameObject visual;
	SpriteRenderer visualRenderer;
	
	void Awake() {
		visual = transform.GetChild(0).gameObject;
		visualRenderer = visual.GetComponent<SpriteRenderer>();
	}
	
	public float speed = 6f;
	Vector2Int currentDirection = Vector2Int.zero;
	
	public Sprite[] mouths;
	float mouthAnimTime = 0f;
	
	// Version of sign that says "actually i don't like floating-point numbers
	// representing approximations, i actually want zero to have a sign of 0."
	int correctSign(float x) => x == 0f ? 0 : (int)Mathf.Sign(x);
	
	private static List<Vector2Int> directions = new List<Vector2Int>() {
		Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up
	};
	
	int directionToIndex(Vector2Int direction) => directions.IndexOf(direction); 
	// Vector2Int indexToDirection(int direction) => directions[direction % directions.Count]; 
	
	Vector2 round(Vector2 v) => new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
	Vector2Int roundToInt(Vector2 v) => new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
	
	Vector2 roundUnusedAxis(Vector2 pos, Vector2Int dir) => new Vector2(
		dir.x == 0 ? Mathf.Round(pos.x) : pos.x,
		dir.y == 0 ? Mathf.Round(pos.y) : pos.y
	);
	
	bool tileInDirectionIsEmpty(Vector2 direction, float spread = 0.2f) {
		spread = Mathf.Clamp01(spread);
		Vector2 perpendicular = Vector2.Perpendicular(direction) * spread;
		
		Vector2 difference = round(direction - perpendicular) - round(direction + perpendicular);
		Vector2 box = (difference + Vector2.one) / 2 - (Vector2.one / 10);
		
		return DebugDrawSquare(round(transform.position) + direction, box,
		Physics2D.OverlapBox( round(transform.position) + direction, box, 0f, 1 << LayerMask.NameToLayer("Board") ) == null );
	}
	
	bool tileIsEmpty(Vector2Int position) =>
		Physics2D.OverlapBox( position, Vector2.one / 2, 0f, 1 << LayerMask.NameToLayer("Board") ) == null;
	
	bool DebugDrawSquare(Vector2 position, Vector2 radius, bool cond) {
		Color c = cond ? Color.green : Color.white;
		Debug.DrawLine( position + new Vector2(-radius.x, +radius.y), position + new Vector2(+radius.x, +radius.y), c );
		Debug.DrawLine( position + new Vector2(+radius.x, -radius.y), position + new Vector2(+radius.x, +radius.y), c );
		Debug.DrawLine( position + new Vector2(-radius.x, -radius.y), position + new Vector2(+radius.x, -radius.y), c );
		Debug.DrawLine( position + new Vector2(-radius.x, -radius.y), position + new Vector2(-radius.x, +radius.y), c );
		return cond;
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
			if (move.x < 0 && tileInDirectionIsEmpty(Vector2Int.left)) { currentDirection = Vector2Int.left; changed = true; }
			if (move.x > 0 && tileInDirectionIsEmpty(Vector2Int.right)) { currentDirection = Vector2Int.right; changed = true; }
		}
		if (allowVertical) {
			// Nobody makes 2D games in +Y-up worlds, Unity.
			if (move.y < 0 && tileInDirectionIsEmpty(Vector2Int.down)) { currentDirection = Vector2Int.down; changed = true; }
			if (move.y > 0 && tileInDirectionIsEmpty(Vector2Int.up)) { currentDirection = Vector2Int.up; changed = true; }
		}
		
		if (changed) transform.position = roundUnusedAxis(transform.position, currentDirection);
		
		// Technical Information: I'm treating 1 Unity unit as equal to 1 tile in the original Pac-Man tile map.
		
		// This means that adding half a tile to the position in the current direction means the side of
		// Pac-Man's hitbox associated with the direction he is moving in.
		
		// Dividing by 4 should never be used. Half-tiles should never be used.
		
		if (currentDirection.sqrMagnitude > 0f) {
			Vector2Int lastTile = roundToInt((Vector2)transform.position + ((Vector2)currentDirection / 2));
			
			transform.Translate((Vector2)currentDirection * speed * Time.deltaTime);
			mouthAnimTime += Time.deltaTime * speed;
			
			int dirIndex = directionToIndex(currentDirection);
			visualRenderer.sprite = mouths[dirIndex * 4 + Mathf.FloorToInt((mouthAnimTime % 1f) * mouths.Length / 4)];
			
			Vector2Int newTile = roundToInt((Vector2)transform.position + ((Vector2)currentDirection / 2));
			if (lastTile != newTile && !tileIsEmpty(newTile)) {
				transform.position = round(transform.position);
				currentDirection = Vector2Int.zero;
			}
		}
	}
}
