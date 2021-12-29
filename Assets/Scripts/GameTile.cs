using UnityEngine;

public enum Direction { North, East, South, West };
public enum DirectionChange { None, TurnRight, TurnAround, TurnLeft};

public static class DirectionExtension
{
	private static Quaternion[] rotations =
	{
		Quaternion.identity,
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f),
	};

	private static Vector3[] halfVectors =
	{
		Vector3.forward * 0.5f,
		Vector3.right * 0.5f,
		Vector3.back * 0.5f,
		Vector3.left * 0.5f,
	};

	public static Quaternion GetRotation(this Direction direction)
    {
		return rotations[(int)direction];
    }

	public static DirectionChange GetDirectionChangeTo(this Direction from, Direction to)
    {
		var delta = to - from;
		delta = delta < 0 ? delta + 4 : delta;
		return (DirectionChange)delta;
    }

	public static float GetAngle(this Direction direction)
    {
		return (int)direction * 90f;
    }

	public static Vector3 GetHalfVector(this Direction direction)
	{
		return halfVectors[(int)direction];
	}
}

public class GameTile : MonoBehaviour
{
	[SerializeField] 
	private Transform arrow = default;

	private static readonly Quaternion NorthRotation = Quaternion.Euler(90f, 0f, 0f);
	private static readonly Quaternion EastRotation = Quaternion.Euler(90f, 90f, 0f);
	private static readonly Quaternion SouthRotation = Quaternion.Euler(90f, 180f, 0f);
	private static readonly Quaternion WestRotation = Quaternion.Euler(90f, 270f, 0f);
	private GameTile north, east, south, west, nextOnPath;
	private int distance;
	private GameTileContent content;
	
	public bool HasPath => distance != int.MaxValue;
	public bool IsAlternative { get; set; }
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
	public GameTile NextTileOnPath => nextOnPath;
	public Vector3 ExitPoint { get; private set; }
	public Direction PathDirection { get; private set; }
	
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
		ExitPoint = transform.localPosition;
	}

	public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
	public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
	public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
	public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

	/// <summary>
	/// create path, The direction is opposite to the path 
	/// </summary>
	/// <param name="neighbor"></param>
	/// <returns></returns>
	private GameTile GrowPathTo(GameTile neighbor, Direction direction)
	{
		if (!HasPath || neighbor == null || neighbor.HasPath)
        {
			return null;
        }

		neighbor.distance = distance + 1;
		neighbor.nextOnPath = this;
		// neighbor.ExitPoint = Vector3.Lerp(neighbor.transform.localPosition, transform.localPosition, 0.5f);
		neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
		neighbor.PathDirection = direction;

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
