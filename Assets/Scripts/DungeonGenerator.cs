using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:
 *  - Enemy placement
 *  - 'Treasure' placement
 */

public class DungeonGenerator : MonoBehaviour 
{
	private class Edge
	{
		public Vector2Int v;
		public Vector2Int u;

		public Edge(Vector2Int v, Vector2Int u)
		{
			this.v = v; this.u = u;
		}

		public Vector3 GetEdgePos(float size, float margin)
		{
			Vector3 vf = new Vector3(v.x * (size + margin), 0f, v.y * (size + margin));
			Vector3 uf = new Vector3(u.x * (size + margin), 0f, u.y * (size + margin));
			return (vf + uf) / 2f;
		}
	}

	[SerializeField] int width;
	[SerializeField] int height;
	[SerializeField] float room_size;
	[SerializeField] float margin;

	[SerializeField] GameObject room_prefab;
	[SerializeField] GameObject hall_prefab;

	void Start() { GenerateDungeon(); }

	void GenerateDungeon()
	{
		// Place Rooms
		PlaceRooms();

		// Place Hallways
		PlaceHallways();
	}

	[ContextMenu("Place Rooms")]
	void PlaceRooms()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Instantiate(room_prefab, (Vector3.right * i * (room_size + margin)) +
					(Vector3.forward * j * (room_size + margin)), Quaternion.identity);
			}
		}
	}

	[ContextMenu("Place Hallway")]
	void PlaceHallways()
	{
		// Implementation of the random walk spanning tree method, runs in worst case O(n^3) which
		// frankly boggles my mind
		List<Vector2Int> visited = new List<Vector2Int>();
		Vector2Int v = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
		visited.Add(v);

		List<Edge> connections = new List<Edge>();

		Vector2Int u = Vector2Int.zero; // will be used later

		// While we haven't visited all nodes
		while (visited.Count < (width * height)) 
		{
			Debug.Log(visited.Count);
			// Select a random direction 
			u = v + GetRandomDirection(v);
			if (!visited.Contains(u))
			{
				// add u to visited, and add {v,u} to the connections
				connections.Add(new Edge(v, u));
				visited.Add(u);
			}
			v = u;
		}

		foreach (Edge e in connections)
		{
			Instantiate(hall_prefab, e.GetEdgePos(room_size, margin), Quaternion.identity);
		}
	}

	Vector2Int GetRandomDirection(Vector2Int v)
	{
		List<Vector2Int> directions = new List<Vector2Int>();
		if (v.x > 0)          directions.Add(Vector2Int.left);
		if (v.x < width - 1)  directions.Add(Vector2Int.right);
		if (v.y > 0)          directions.Add(Vector2Int.down);
		if (v.y < height - 1) directions.Add(Vector2Int.up);
		return directions[Random.Range(0, directions.Count)];
	}

}
