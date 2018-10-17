using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AdjacencyList = System.Collections.Generic.Dictionary<UnityEngine.Vector2Int, 
	System.Collections.Generic.List<UnityEngine.Vector2Int>>;

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

	[SerializeField] GameObject start_prefab;
	[SerializeField] GameObject end_prefab;
	[SerializeField] GameObject room_prefab;
	[SerializeField] GameObject hall_prefab;

	void Start() { GenerateDungeon(); }

	void GenerateDungeon()
	{
		// Place Hallways
		AdjacencyList graph = PlaceHallways();

		// Place Rooms
		PlaceRooms(graph);
	}

	[ContextMenu("Place Rooms")]
	void PlaceRooms(AdjacencyList graph)
	{
		// find longest path
		List<Vector2Int> leaves = new List<Vector2Int>();
		foreach (Vector2Int v in graph.Keys)
		{
			if (graph[v].Count == 1) { leaves.Add(v); }
		}

		// then path from each leaf to each leaf
		Vector2Int s, t; int distance = 0; // should be long enough;
		s = Vector2Int.zero;
		t = Vector2Int.zero;
		int d;
		foreach (Vector2Int v1 in leaves)
		{
			foreach (Vector2Int u1 in leaves)
			{
				if (v1 != u1)
				{
					d = GetPathLength(graph, v1, u1);
					if (d > distance)
					{
						distance = d;
						s = v1;
						t = u1;
					}
				}
			}
		}

		// Place the remainder of the rooms
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				if (s.x == i && s.y == j) { InstantiateRoom(start_prefab, i, j, room_size, margin); }
				else if (t.x == i && t.y == j) { InstantiateRoom(end_prefab, i, j, room_size, margin); }
				else { InstantiateRoom(room_prefab, i, j, room_size, margin); }
			}
		}
	}

	void InstantiateRoom(GameObject prefab, int x, int y, float size, float margin)
	{
		Instantiate(prefab, (Vector3.right * x * (size + margin)) +
			(Vector3.forward * y * (size + margin)), Quaternion.identity);
	}

	[ContextMenu("Place Hallway")]
	AdjacencyList PlaceHallways()
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

		// So, I think that if I just get the leaves of the spanning tree, then check the
		// distance from every leaf to every other leaf to get the longest path. 

		foreach (Edge e in connections)
		{
			Instantiate(hall_prefab, e.GetEdgePos(room_size, margin), Quaternion.identity);
		}

		// turn connections into an adjacency list,
		AdjacencyList graph = new AdjacencyList();
		// build an adjacency list from the connections

		foreach (Edge e in connections)
		{
			if (!graph.ContainsKey(e.v)) { graph.Add(e.v, new List<Vector2Int>()); }
			if (!graph.ContainsKey(e.u)) { graph.Add(e.u, new List<Vector2Int>()); }
			graph[e.v].Add(e.u); graph[e.u].Add(e.v);
		}

		return graph;
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

	// shortest path from v to u
	int GetPathLength(AdjacencyList graph, Vector2Int v, Vector2Int u)
	{
		// BFS, and return the length
		Dictionary<Vector2Int, int> distance = new Dictionary<Vector2Int, int>();
		Queue<Vector2Int> q = new Queue<Vector2Int>();
		q.Enqueue(v);
		distance.Add(v, 0);
		Vector2Int s;
		while (q.Count != 0)
		{
			s = q.Dequeue();
			// add in each child of t
			foreach (Vector2Int t in graph[s])
			{
				if (!distance.ContainsKey(t)) 
				{ 
					distance.Add(t, distance[s] + 1); 
					q.Enqueue(t);
				}
			}
		}
		return distance[u];
	}

}
