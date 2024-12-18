// Buoyancy.cs
// by Alex Zhdankin
// Version 2.1
//
// http://forum.unity3d.com/threads/72974-Buoyancy-script
//
// Terms of use: do whatever you like

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Buoyancy : MonoBehaviour
{
	public List<GameObject> compartments = new List<GameObject>();
	public GameObject boat;
	public float density = 500;
	public int slicesPerAxis = 2;
	public bool isConcave = false;
    public int voxelsLimit = 16;
    public WaterSurface water;

	private const float DAMPFER = 0.005f;
	private const float WATER_DENSITY = 1000;

	private float voxelHalfHeight;
	private Vector3 localArchimedesForce;
	private List<Vector3> voxels;
	private bool isMeshCollider;
	private List<Vector3[]> forces; // For drawing force gizmos
    private new Collider collider;
    private new Rigidbody rigidbody;
    WaterSearchParameters Search;
    WaterSearchResult SearchResult;
	/// <summary>
	/// Provides initialization.
	/// </summary>
	private void Start()
	{
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

		forces = new List<Vector3[]>(); // For drawing force gizmos

		// Store original rotation and position
		var originalRotation = transform.rotation;
		var originalPosition = transform.position;
		boat.transform.rotation = Quaternion.identity;
		boat.transform.position = Vector3.zero;

		// The object must have a collider
		if (collider == null)
		{
			gameObject.AddComponent<MeshCollider>();
            collider = gameObject.GetComponent<MeshCollider>();
			Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no collider. MeshCollider has been added.", name));
		}
		isMeshCollider = GetComponent<MeshCollider>() != null;
		Debug.Log("Collider: "+collider.bounds);
		var bounds = collider.bounds;
		if (bounds.size.x < bounds.size.y)
		{
			voxelHalfHeight = bounds.size.x;
		}
		else
		{
			voxelHalfHeight = bounds.size.y;
		}
		if (bounds.size.z < voxelHalfHeight)
		{
			voxelHalfHeight = bounds.size.z;
		}
		voxelHalfHeight /= 2 * slicesPerAxis;

		// The object must have a RidigBody
		if (rigidbody == null)
		{
			gameObject.AddComponent<Rigidbody>();
            rigidbody = gameObject.GetComponent<Rigidbody>();
			Debug.LogWarning(string.Format("[Buoyancy.cs] Object \"{0}\" had no Rigidbody. Rigidbody has been added.", name));
		}
		//rigidbody.centerOfMass = new Vector3(0, -bounds.extents.y * 0f, 0) + transform.InverseTransformPoint(bounds.center);

		voxels = SliceIntoVoxels(isMeshCollider && isConcave, collider);
		int i = 0;

		foreach (Vector3 item in voxels){
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = boat.transform;
			gameObject.name = "compartment"+i;
			BoxCollider hitbox = gameObject.AddComponent<BoxCollider>();
			hitbox.excludeLayers = LayerMask.GetMask("Game Logic");
			hitbox.center = item-originalPosition;
			hitbox.size = new Vector3(bounds.extents.x/2,bounds.extents.y,bounds.extents.z/2);
			hitDetection hitDetection = hitbox.AddComponent<hitDetection>();
			
			Compartment compartment = gameObject.AddComponent(typeof(Compartment)) as Compartment;
			compartment.position = item-originalPosition;
			compartment.volume = 100;
			compartment.currentRepairmen = 0;
			compartment.compartmentName = i;
			compartments.Add(gameObject);
			i++;
		}

		foreach (GameObject compartment in compartments){
			//compartment.GetComponent<Compartment>().findNeighbors(compartments);
		}

		// Restore original rotation and position
		boat.transform.rotation = originalRotation;
		boat.transform.position = originalPosition;

		float volume = rigidbody.mass / density;

		WeldPoints(voxels, voxelsLimit);

		float archimedesForceMagnitude = WATER_DENSITY * Mathf.Abs(Physics.gravity.y) * volume;
		localArchimedesForce = new Vector3(0, archimedesForceMagnitude, 0) / voxels.Count;

		Debug.Log(string.Format("[Buoyancy.cs] Name=\"{0}\" volume={1:0.0}, mass={2:0.0}, density={3:0.0}", name, volume, rigidbody.mass, density));
	}

	/// <summary>
	/// Slices the object into number of voxels represented by their center points.
	/// <param name="concave">Whether the object have a concave shape.</param>
	/// <returns>List of voxels represented by their center points.</returns>
	/// </summary>
	private List<Vector3> SliceIntoVoxels(bool concave, Collider collider)
	{
		var points = new List<Vector3>(slicesPerAxis * slicesPerAxis * slicesPerAxis);

		if (concave)
		{
			var meshCol = GetComponent<MeshCollider>();

			var convexValue = meshCol.convex;
			meshCol.convex = false;

			// Concave slicing
			var bounds = collider.bounds;
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis / 2; iy++)		// Only two slices in Y direction needed
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y*2 / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

						var p = transform.InverseTransformPoint(new Vector3(x, y, z));

						if (PointIsInsideMeshCollider(meshCol, p))
						{
							points.Add(p);
						}
					}
				}
			}
			if (points.Count == 0)
			{
				points.Add(bounds.center);
			}

			meshCol.convex = convexValue;
		}
		else
		{
			// Convex slicing
			var bounds = GetComponent<Collider>().bounds;
			for (int ix = 0; ix < slicesPerAxis; ix++)
			{
				for (int iy = 0; iy < slicesPerAxis / 2; iy++)		// Only two slices in Y direction needed
				{
					for (int iz = 0; iz < slicesPerAxis; iz++)
					{
						float x = bounds.min.x + bounds.size.x / slicesPerAxis * (0.5f + ix);
						float y = bounds.min.y + bounds.size.y*2 / slicesPerAxis * (0.5f + iy);
						float z = bounds.min.z + bounds.size.z / slicesPerAxis * (0.5f + iz);

						var p = transform.InverseTransformPoint(new Vector3(x, y, z));

						points.Add(p);
					}
				}
			}
		}
		return points;
	}

	/// <summary>
	/// Returns whether the point is inside the mesh collider.
	/// </summary>
	/// <param name="c">Mesh collider.</param>
	/// <param name="p">Point.</param>
	/// <returns>True - the point is inside the mesh collider. False - the point is outside of the mesh collider. </returns>
	private static bool PointIsInsideMeshCollider(Collider c, Vector3 p)
	{
		Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

		foreach (var ray in directions)
		{
			RaycastHit hit;
			if (c.Raycast(new Ray(p - ray * 1000, ray), out hit, 1000f) == false)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Returns two closest points in the list.
	/// </summary>
	/// <param name="list">List of points.</param>
	/// <param name="firstIndex">Index of the first point in the list. It's always less than the second index.</param>
	/// <param name="secondIndex">Index of the second point in the list. It's always greater than the first index.</param>
	private static void FindClosestPoints(IList<Vector3> list, out int firstIndex, out int secondIndex)
	{
		float minDistance = float.MaxValue, maxDistance = float.MinValue;
		firstIndex = 0;
		secondIndex = 1;

		for (int i = 0; i < list.Count - 1; i++)
		{
			for (int j = i + 1; j < list.Count; j++)
			{
				float distance = Vector3.Distance(list[i], list[j]);
				if (distance < minDistance)
				{
					minDistance = distance;
					firstIndex = i;
					secondIndex = j;
				}
				if (distance > maxDistance)
				{
					maxDistance = distance;
				}
			}
		}
	}

	/// <summary>
	/// Welds closest points.
	/// </summary>
	/// <param name="list">List of points.</param>
	/// <param name="targetCount">Target number of points in the list.</param>
	private static void WeldPoints(IList<Vector3> list, int targetCount)
	{
		if (list.Count <= 2 || targetCount < 2)
		{
			return;
		}

		while (list.Count > targetCount)
		{
			int first, second;
			FindClosestPoints(list, out first, out second);

			var mixed = (list[first] + list[second]) * 0.5f;
			list.RemoveAt(second); // the second index is always greater that the first => removing the second item first
			list.RemoveAt(first);
			list.Add(mixed);
		}
	}
	/// <summary>
	/// Returns the water level at given location.
	/// </summary>
	/// <param name="x">x-coordinate</param>
	/// <param name="z">z-coordinate</param>
	/// <returns>Water level</returns>
	private float GetWaterLevel(float x, float z)
	{
        //Search.startPosition = new Vector3(x,0f,z);
        //water.FindWaterSurfaceHeight(Search, out SearchResult);
		//return SearchResult.height;
		return(1);
	}

	/// <summary>
	/// Calculates physics. FixedUpdate is called 50 times / second.
	/// </summary>
	private void FixedUpdate()
	{
		forces.Clear(); // For drawing force gizmos
		int i = 0;

		foreach (GameObject compartmentGameObject in compartments)
		{
			Compartment compartment = compartmentGameObject.GetComponent<Compartment>();
			var wp = transform.TransformPoint(compartment.position);
			float waterLevel = GetWaterLevel(wp.x, wp.z);
			compartment.Repair();
			compartment.spreadToNeighbors();

			// If object is submerged
			if (wp.y - voxelHalfHeight < waterLevel)
			{
				Debug.Log("its submerged");
				compartment.UpdateBuoyancy(waterLevel);
				float k = (waterLevel - wp.y) / (2 * voxelHalfHeight) + 0.5f;
				if (k > 1)
				{
					k = 1f;
				}
				else if (k < 0)
				{
					k = 0f;
				}
				var velocity = rigidbody.GetPointVelocity(compartment.position);
				var localDampingForce = -velocity * DAMPFER * rigidbody.mass;
				var force = localDampingForce + Mathf.Sqrt(k) * localArchimedesForce*1.8f*compartment.LiftFactor();
				rigidbody.AddForceAtPosition(force, wp);

				forces.Add(new[] { wp, force }); // For drawing force gizmos
			}
			i++;
		}
	}
/// <summary>
	/// Draws gizmos.
	/// </summary>
	private void OnDrawGizmos()
	{
		if (voxels == null || forces == null)
		{
			return;
		}

		const float gizmoSize = 0.05f;
		Gizmos.color = Color.yellow;

		foreach (var p in voxels)
		{
			Gizmos.DrawCube(transform.TransformPoint(p), new Vector3(gizmoSize, gizmoSize, gizmoSize));
		}

		Gizmos.color = Color.cyan;

		foreach (var force in forces)
		{
			Gizmos.DrawCube(force[0], new Vector3(gizmoSize, gizmoSize, gizmoSize));
			Gizmos.DrawLine(force[0], force[0] + force[1] / rigidbody.mass);
		}
	}

}