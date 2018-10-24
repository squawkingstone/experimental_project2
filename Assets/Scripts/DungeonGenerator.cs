using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AdjacencyList = System.Collections.Generic.Dictionary<UnityEngine.Vector2Int, 
	System.Collections.Generic.List<UnityEngine.Vector2Int>>;

/* TODO:
 *  - Enemy placement
 *  - 'Treasure' placement
 */

[System.Serializable]
public class RoomPrefabs
{
	public GameObject[] OneDoor; // oriented +z
	public GameObject[] TwoDoorStraight; // oriented +-z
	public GameObject[] TwoDoorL; // oriented +x and +z
	public GameObject[] ThreeDoor; // oriented +-x and +z
	public GameObject[] FourDoor;
}

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

	[SerializeField] RoomPrefabs room_prefabs;
	[SerializeField] GameObject  hall_prefab;

	[SerializeField] GameObject player;
	[SerializeField] GameObject level_transition;

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
				if (s.x == i && s.y == j) { InstantiateRoom(i, j, true, false, room_prefabs, graph); }
				else if (t.x == i && t.y == j) { InstantiateRoom(i, j, false, true, room_prefabs, graph); }
				else { InstantiateRoom(i, j, false, false, room_prefabs, graph); }
			}
		}
	}

	void InstantiateRoom(int x, int y, bool start, bool end, RoomPrefabs prefabs, AdjacencyList graph)
	{
		GameObject prefab = null; Quaternion q = Quaternion.identity;
		Vector2Int v = new Vector2Int(x, y);

		switch (graph[v].Count)
		{
			case 1:
				prefab = prefabs.OneDoor[Random.Range(0, prefabs.OneDoor.Length)];
				q = Quaternion.AngleAxis(Vector2.SignedAngle(graph[v][0]-v, Vector2.up), Vector3.up);
				break;
			case 2:
				if (Mathf.Approximately(Vector2.Dot(graph[v][0]-v, graph[v][1]-v), 0))
				{
					prefab = prefabs.TwoDoorL[Random.Range(0, prefabs.TwoDoorL.Length)];
					Vector2Int u = (Vector2.SignedAngle(graph[v][0]-v, graph[v][1]-v) < 0f)
						? graph[v][0]
						: graph[v][1];
					q = Quaternion.AngleAxis(Vector2.SignedAngle(u-v, Vector2.up), Vector3.up);
				}
				else
				{
					prefab = prefabs.TwoDoorStraight[Random.Range(0, prefabs.TwoDoorStraight.Length)];
					q = Quaternion.AngleAxis(Vector2.Angle(graph[v][0]-v, Vector2.up), Vector3.up);
				}
				break;
			case 3:
				prefab = prefabs.ThreeDoor[Random.Range(0, prefabs.ThreeDoor.Length)];
				Vector2Int w = (Mathf.Approximately(Vector2.Dot(graph[v][0]-v, graph[v][1]-v), -1f))
					? graph[v][2]
					: (Mathf.Approximately(Vector2.Dot(graph[v][0]-v, graph[v][2]-v), -1f) 
						? graph[v][1]
						: graph[v][0]
					);
				q = Quaternion.AngleAxis(Vector2.SignedAngle(w-v, Vector2.up), Vector3.up);
				break;
			default:
				prefab = prefabs.FourDoor[Random.Range(0, prefabs.FourDoor.Length)];
				break;
		}
		
		if (start)
		{
			prefab = prefabs.OneDoor[0];
		}
		if (end)
		{
			prefab = prefabs.OneDoor[0];
		}

		GameObject room = Instantiate(prefab, (Vector3.right * x * (room_size + margin)) +
			(Vector3.forward * y * (room_size + margin)), q);

		if (start)
		{
			player.transform.position = room.transform.position + Vector3.up;
		}
		if (end)
		{
			level_transition.transform.position = room.transform.position;
		}
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
			Instantiate(hall_prefab, e.GetEdgePos(room_size, margin), 
				(e.v.x == e.u.x) ? Quaternion.identity : Quaternion.AngleAxis(90, Vector3.up));
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
