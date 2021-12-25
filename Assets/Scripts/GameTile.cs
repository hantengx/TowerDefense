using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
	[SerializeField] 
	private Transform arrow = default;

	private static readonly Quaternion NorthRotation = Quaternion.Euler(90f, 0f, 0f);
	private static readonly Quaternion SouthRotation = Quaternion.Euler(90f, 180f, 0f);
	private static readonly Quaternion EastRotation = Quaternion.Euler(90f, 90f, 0f);
	private static readonly Quaternion WestRotation = Quaternion.Euler(90f, -90f, 0f);
	private GameTile north, east, south, west, nextOnPath;
	private int distance;
	private GameTileContent content;
	
	public bool HasPath => distance != int.MaxValue;
	public GameTile NextTileOnPath => nextOnPath;

	public GameTileContent Content
	{
		get => content;
		set
		{
			Debug.Assert(value != null, "Null assigned to content!");
			if (content != null)
			{
				content.Recycle();
			}

			content = value;
			content.transform.localPosition = transform.localPosition;
		}
	}
	
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

	public void BecameDestination()
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

		return neighbor.Content.Type == Game.GameTileContentType.Wall ? null : neighbor;
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
			arrow.localRotation = NorthRotation;
        }
        else if(nextOnPath == south)
        {
			arrow.localRotation = SouthRotation;
		}
        else if (nextOnPath == east)
        {
			arrow.localRotation = EastRotation;
        }
		else
        {
			arrow.localRotation = WestRotation;
		}
	}

	public void HidePath()
	{
		arrow.gameObject.SetActive(false);
	}
}
