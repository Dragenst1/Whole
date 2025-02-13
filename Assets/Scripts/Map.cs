using System.Collections.Generic;
using UnityEngine;
using Eden.Utils;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class Map : MonoBehaviour
{
	public Vector2 holePos;
	public float holeRadius;
	public Hole holeScript;
	[Min(3)] public int MapDimension;
	MeshFilter filter;
	MeshCollider collider;
	Mesh mesh;

	private void Start()
	{
		if (!filter) filter = GetComponent<MeshFilter>();
		if (!collider) collider = GetComponent<MeshCollider>();
		if (!holeScript)
		{
			holePos = Vector2.one * 5f;
			holeRadius = 5f;
		}
		else
		{
			//fetching variables from script if the script object exists
			holePos = V2.IsolateFromVector3(holeScript.transform.position, 1);
			holeRadius = holeScript.holeRadius;
		}
	}

	private void Update()
	{
		mesh = new Mesh();
		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();

		if (holeScript)
		{
			//fetching variables from script if the script object exists (this time per update)
			//(?) likely only needs done once on initial startup but just to be safe
			holePos = V2.IsolateFromVector3(holeScript.transform.position, 1);
			holeRadius = holeScript.holeRadius;
		}

		//"bounding box" coordinate variables
		//  a = bottom (-), b = top (+)
		float ax = holePos.x - holeRadius;
		float ay = holePos.y - holeRadius;
		float bx = holePos.x + holeRadius;
		float by = holePos.y + holeRadius;
		float mm = MapDimension;

		//manually creating & appending vertices (better than procedural at this scale)
		verts.Add(new Vector3(0f, 0f, 0f));
		verts.Add(new Vector3(mm, 0f, 0f));

		verts.Add(new Vector3(0f, 0f, ay));
		verts.Add(new Vector3(ax, 0f, ay));
		verts.Add(new Vector3(bx, 0f, ay));
		verts.Add(new Vector3(mm, 0f, ay));

		verts.Add(new Vector3(0f, 0f, by));
		verts.Add(new Vector3(ax, 0f, by));
		verts.Add(new Vector3(bx, 0f, by));
		verts.Add(new Vector3(mm, 0f, by));

		verts.Add(new Vector3(0f, 0f, mm));
		verts.Add(new Vector3(mm, 0f, mm));

		//triangles are identified as integers, which corrospond to indices of the "verts" array, "0 2 5" together form the first triangle, and subsequently every 3
		tris.AddRange(new List<int>() {
			0, 2, 5, 0, 5, 1,
			2, 6, 7, 2, 7, 3,
			4, 8, 9, 4, 9, 5,
			6, 10, 11, 6, 11, 9
		});

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		filter.mesh = mesh;
		collider.sharedMesh = mesh;
	}
}