using System.Collections.Generic;
using Eden.Utils;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class Hole : MonoBehaviour
{
	public float speed;
	public float holeRadius;
	[Space] [Space]
	public Material holeMaterial;
	[Min(3)] public int resolution;
	public float resIter;
	MeshFilter filter;
	MeshCollider collider;
	Mesh mesh;
	GameObject visual;
	public GameObject hitBox;

	private void Start()
	{
		visual = new GameObject("visual");
		visual.AddComponent<MeshRenderer>().sharedMaterial = holeMaterial;
		visual.AddComponent<MeshFilter>();
		visual.transform.SetParent(transform);
		visual.transform.localPosition = Vector3.up * 0.1f;

		if (!filter) filter = GetComponent<MeshFilter>();
		if (!collider) collider = GetComponent<MeshCollider>();

		UpdateMesh();
		hitBox.transform.localScale = Vector3.one * 100f * holeRadius;
	}

	private void Update()
	{
		//float for player movements
		float x = Input.GetAxisRaw("Horizontal"); // left and right
		float z = Input.GetAxisRaw("Vertical"); // forward and back

		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x + x * speed * Time.deltaTime, holeRadius, 64f - holeRadius), 0f,
			Mathf.Clamp(transform.position.z + z * speed * Time.deltaTime, holeRadius, 64f - holeRadius));
	}

	private void UpdateMesh()
	{
		mesh = new Mesh();
		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();

		resIter = 360f / resolution * Mathf.Deg2Rad; //fraction of full circumference for use iteratively as we loop around the circle to create the mesh

		for (int j = 0, i = 0; j < resolution; j++)
		{
			float a0 = (j + 0f) * resIter;
			float a1 = (j + 1f) * resIter;

			verts.Add(new Vector3(Mathf.Cos(a0), 0f, Mathf.Sin(a0)) * (holeRadius - 0.3f));
			verts.Add(new Vector3(Mathf.Cos(a1), 0f, Mathf.Sin(a1)) * (holeRadius - 0.3f));
			verts.Add(new Vector3(Mathf.Cos(a0), 0f, Mathf.Sin(a0)) * (holeRadius + 1.8f));
			verts.Add(new Vector3(Mathf.Cos(a1), 0f, Mathf.Sin(a1)) * (holeRadius + 1.8f));

			tris.Add(i + 0);
			tris.Add(i + 1);
			tris.Add(i + 2);

			tris.Add(i + 1);
			tris.Add(i + 3);
			tris.Add(i + 2);

			i += 4;
		}

		mesh.vertices = verts.ToArray();
		mesh.triangles = tris.ToArray();
		filter.mesh = mesh;
		collider.sharedMesh = mesh;

		visual.GetComponent<MeshFilter>().mesh = mesh;
	}
}