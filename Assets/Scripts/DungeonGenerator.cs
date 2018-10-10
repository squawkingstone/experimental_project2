using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour 
{

	[SerializeField] int width;
	[SerializeField] int height;
	[SerializeField] float room_size;
	[SerializeField] float margin;

	[SerializeField] GameObject room_prefab;

	void Start() { GenerateDungeon(); }

	void GenerateDungeon()
	{
		// Place Rooms
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Instantiate(room_prefab, (Vector3.right * i * (room_size + margin)) +
					(Vector3.forward * j * (room_size + margin)), Quaternion.identity);
			}
		}

		// Place Hallways

		// Place Enemies

		// Place "Treasure"
	}

	

}
