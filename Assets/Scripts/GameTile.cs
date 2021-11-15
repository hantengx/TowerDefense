using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
	[SerializeField] 
	private Transform arrow = default;

	public bool HasPath => distance != int.MaxValue;

	private GameTile north, east, south, west, nextOnPath;
	private int distance;

	private static readonly Quaternion northRotation = Quaternion.Euler(90f, 0f, 0f);
	private static readonly Quaternion southRotation = Quaternion.Euler(90f, 180f, 0f);
	private static readonly Quaternion eastRotation = Quaternion.Euler(90f, 90f, 0f);
	private static readonly Quaternion westRotation = Quaternion.Euler(90f, -90f, 0f);

	public static void MakeEastWestNeighbors(GameTile east, GameTile west)
	{
		Debug.Assert(west.east == null && east.west == null, "Redefined neighbors!");
		west.east = east;
		east.west = west;
	}

	public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
	{
		Debug.Assert(north.south == null && south.north == null, "Redefined neighbors!");
		south.north = north;
		north.south = south;
	}

	public void ClearPath()
	{
		distance = int.MaxValue;
		nextOnPath = null;
	}

	public void BecameDistination()
	{
		distance = 0;
		nextOnPath = null;
	}

	public GameTile GrowPathNorth() => GrowPathTo(north);
	public GameTile GrowPathSouth() => GrowPathTo(south);
	public GameTile GrowPathEast() => GrowPathTo(east);
	public GameTile GrowPathWest() => GrowPathTo(west);

	/// <summary>
	/// create path, The direction is opposite to the path 
	/// </summary>
	/// <param name="neighbor"></param>
	/// <returns></returns>
	private GameTile GrowPathTo(GameTile neighbor)
	{
		if (!HasPath || neighbor == null || neighbor.HasPath)
        {
			return null;
        }

		neighbor.distance = distance + 1;
		neighbor.nextOnPath = this;

		return neighbor;
    }

	public void ShowPath()
    {
        if (distance == 0 || distance == int.MaxValue)
        {
			arrow.gameObject.SetActive(false);
			return;
        }

		arrow.gameObject.SetActive(true);
        if (nextOnPath == north)
        {
			arrow.localRotation = northRotation;
        }
        else if(nextOnPath == south)
        {
			arrow.localRotation = southRotation;
		}
        else if (nextOnPath == east)
        {
			arrow.localRotation = eastRotation;
        }
		else
        {
			arrow.localRotation = westRotation;
		}

	}
}
