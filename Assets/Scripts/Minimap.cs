using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using AdjacencyList = System.Collections.Generic.Dictionary<UnityEngine.Vector2Int, 
	System.Collections.Generic.List<UnityEngine.Vector2Int>>;

public class Minimap : MonoBehaviour 
{
	[SerializeField] Image[] m_Rooms;
	[SerializeField] Image[] m_HorizontalHalls;
	[SerializeField] Image[] m_VerticalHalls;
	[SerializeField] Image m_Player;

	Transform player;

	int m_PlayerRoom = 0;
	float m_Size = 0f;
	float m_Margin = 0f;
	int m_Width = 0;
	int m_Height = 0;
	AdjacencyList m_Graph;

	int x, y;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	public void InitMinimap(float size, float margin, int width, int height, AdjacencyList graph)
	{
		m_Size = size;
		m_Margin = margin;
		m_Width = width;
		m_Height = height;
		m_Graph = graph;
		
		SetHallways();
	}

	public void SetHallways()
	{
		// Loop horizontal hallways
		for (int j = 0; j < m_Height; j++)
		{
			for (int i = 0; i < m_Width - 1; i++)
			{
				m_HorizontalHalls[(j * (m_Width-1)) + i].enabled = IsHallway(
					new Vector2Int(i, j), new Vector2Int(i+1, j));
			}
		}
		// Loop vertical hallways
		for (int j = 0; j < m_Height - 1; j++)
		{
			for (int i = 0; i < m_Width; i++)
			{
				m_VerticalHalls[j * m_Width + i].enabled = IsHallway(
					new Vector2Int(i, j), new Vector2Int(i, j+1));
			}
		}

	}

	void Update()
	{
		m_PlayerRoom = GetPlayerCoord();
		Vector3 pos = m_Rooms[m_PlayerRoom].rectTransform.position;
		m_Player.rectTransform.position = pos;
	}

	bool IsHallway(Vector2Int v, Vector2Int u)
	{
		return (m_Graph[v].Contains(u));
	}

	int GetPlayerCoord()
	{
		x = Mathf.RoundToInt(player.position.x / (m_Size + m_Margin));
		y = Mathf.RoundToInt(player.position.z / (m_Size + m_Margin));
		return y * m_Width + x;
	}
	
}
