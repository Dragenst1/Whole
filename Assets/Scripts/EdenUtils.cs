using System.Collections.Generic;
using UnityEngine;

namespace Eden.Utils
{
	public static class Standards
	{
		public static float CreateEpsilon(int bound, float sensitivity)
		{
			sensitivity = Mathf.Clamp01(sensitivity) / 1.6f + 0.375f;
			bound = Mathf.Clamp(bound, 2, 35); float v = 0.1f;

			for (int i = 0; i < bound; i++)
			{
				if (1f + v / (3.2f * sensitivity) != 1f)
				{
					v /= 3.2f * sensitivity;
				}
				else
				{
					break;
				}
			}
			return v;
		}
	}

	public static class V1
	{
		/*
		+ comp		Round + Floor + Ceil
		+ comp		FloorToNearest(float f, float nearest)
		+ comp		FloorToNearestWithOffset(float f, float offset)
		 */

		public static float FloorToNearest(float f, float nearest)
		{
			return Mathf.Floor(f / nearest) * nearest;
		}

		public static float FloorToNearestWithOffset01(float f, float nearest, float offset01)
		{
			return FloorToNearestWithOffset(f, nearest, offset01 * nearest);
		}

		public static float FloorToNearestWithOffset(float f, float nearest, float offset)
		{
			return Mathf.Floor((f + offset) / nearest) * nearest - offset;
		}

		public static float RoundToNearest(float f, float nearest)
		{
			return Mathf.Round(f / nearest) * nearest;
		}

		public static float RoundToNearestWithOffset01(float f, float nearest, float offset01)
		{
			return RoundToNearestWithOffset(f, nearest, offset01 * nearest);
		}
		public static float RoundToNearestWithOffset(float f, float nearest, float offset)
		{
			return Mathf.Round((f + offset) / nearest) * nearest - offset;
		}

		public static float CeilToNearest(float f, float nearest)
		{
			return Mathf.Ceil(f / nearest) * nearest;
		}

		public static float CeilToNearestWithOffset01(float f, float nearest, float offset01)
		{
			return CeilToNearestWithOffset(f, nearest, offset01 * nearest);
		}
		public static float CeilToNearestWithOffset(float f, float nearest, float offset)
		{
			return Mathf.Ceil((f + offset) / nearest) * nearest - offset;
		}

		public static bool InRange(float f, float min, float max)
		{
			return InRange(f, new Vector2(min, max));
		}
		public static bool InRange(float f, Vector2 range)
		{
			return f >= range.x && f <= range.y;
		}

		public static float SmoothLerp(float f, float n)
		{
			if (f != 0f && f != 0.5f && f != 1f)
			{
				if (f < 0.5f) { return SlerpLower(f, n) / 2f; }
				else { return SlerpUpper(f, n) / 2f + 0.5f; }
			}
			return f;
		}

		public static float SlerpUpper(float f, float n)
		{
			f = Mathf.Clamp01(f);
			n = Mathf.Clamp01(n);

			float n0 = Mathf.Pow(2 * n + 1, 2 * n + 1);
			return 1f - Mathf.Pow(1f - f, n0);
		}

		public static float SlerpLower(float f, float n)
		{
			f = Mathf.Clamp01(f);
			n = Mathf.Clamp01(n);

			float n0 = Mathf.Pow(2 * n + 1, 2 * n + 1);
			return Mathf.Pow(f, n0);
		}

		public static float SmoothMax(float a, float b, float k)
		{
			return SmoothMin(a, b, 0f - k);
		}

		public static float SmoothMin(float a, float b, float k)
		{
			if (k < 0f) { k = Mathf.Clamp(k, -2f, 0f); }
			else { k = Mathf.Clamp(k, 0f, 2f); }

			float h = Mathf.Min(1f, Mathf.Max(0, (b - a + k) / (2f * k)));
			return a * h + b * (1f - h) - h * k * (1f - h);
		}

		public static float SawtoothWave(float t, float m)
		{
			//inversion ("fix") for modulo's mirror behavior across x=0
			float f = t / m;
			return m * (f < 0f ? f + Mathf.Floor(Mathf.Abs(f)) + 1f : f - Mathf.Floor(Mathf.Abs(f)));
		}

		public static float TriangularWave(float t, float m)
		{
			float f = Mathf.Abs(t) / m;
			return m * (f % 2f > 1f ? 2f - (f % 2f) : f % 2f);
		}

		public static float PickStickModulo(float t, float m)
		{
			return TriangularWave(t, m) + Mathf.Floor(t);
		}

		public static float PingPong(float f)
		{
			return PingPong(f, 1f);
		}
		public static float PingPong(float f, float p)
		{
			return Mathf.Abs((f + p) % (2f * p) - p);
		}
	}

	public static class V2
	{
		public static float DistanceFromLine(Vector2 k, Vector2 pivot, float angle)
		{
			return DistanceFromLine(k.x, k.y, pivot.x, pivot.y, pivot.x + Mathf.Cos(angle), pivot.y + Mathf.Sin(angle));
		}
		public static float DistanceFromLine(Vector2 k, Vector2 start, Vector2 end)
		{
			return DistanceFromLine(k.x, k.y, start.x, start.y, end.x, end.y);
		}
		public static float DistanceFromLine(Vector2 k, float x1, float y1, float x2, float y2)
		{
			return DistanceFromLine(k.x, k.y, x1, y1, x2, y2);
		}
		public static float DistanceFromLine(float kx, float ky, float x1, float y1, float x2, float y2)
		{
			return Vector2.Distance(new Vector2(kx, ky), ClosestPointOnLine(kx, ky, x1, y1, x2, y2));
		}

		//0 yz, 1 xz, 2 xy
		public static Vector2 IsolateFromVector3(Vector3 v, int axis)
		{
			return new Vector2(axis == 0 ? v.y : v.x, axis == 2 ? v.y : v.z);
		}
		public static Vector2 IsolateFromVector3(Vector3 v, int axis, bool normalizeOutput)
		{
			Vector2 result = new Vector2(axis == 0 ? v.y : v.x, axis == 2 ? v.y : v.z);
			return normalizeOutput ? result.normalized : result;
		}
		public static Vector2 IsolateFromVector3(Vector3 v, int axis, out float other)
		{
			other = v[axis]; return new Vector2(axis == 0 ? v.y : v.x, axis == 2 ? v.y : v.z);
		}
		public static Vector2 IsolateFromVector3(Vector3 v, int axis, bool normalizeOutput, out float other)
		{
			Vector2 result = new Vector2(axis == 0 ? v.y : v.x, axis == 2 ? v.y : v.z);
			other = v[axis]; return normalizeOutput ? result.normalized : result;
		}

		public static Vector2 ClosestPointOnLine(Vector2 k, Vector2 pivot, float angle)
		{
			return ClosestPointOnLine(k.x, k.y, pivot.x, pivot.y, pivot.x + Mathf.Cos(angle), pivot.y + Mathf.Sin(angle));
		}
		public static Vector2 ClosestPointOnLine(Vector2 k, Vector2 start, Vector2 end)
		{
			return ClosestPointOnLine(k.x, k.y, start.x, start.y, end.x, end.y);
		}
		public static Vector2 ClosestPointOnLine(Vector2 k, float x1, float y1, float x2, float y2)
		{
			return ClosestPointOnLine(k.x, k.y, x1, y1, x2, y2);
		}
		public static Vector2 ClosestPointOnLine(float kx, float ky, float x1, float y1, float x2, float y2)
		{
			if (x1 == x2) { return new Vector2(x1, ky); }
			if (y1 == y2) { return new Vector2(kx, y1); }

			float m = (y2 - y1) / (x2 - x1);
			float x0 = (ky - y1 + m * x1 + 1 / m * kx) / (m + (1 / m));
			float y0 = (0f - 1 / m) * (x0 - kx) + ky;

			return new Vector2(x0, y0);
		}

		public static float DistanceFromSegment(Vector2 k, Vector2 p1, Vector2 p2)
		{
			return DistanceFromSegment(k.x, k.y, p1.x, p1.y, p2.x, p2.y);
		}
		public static float DistanceFromSegment(float kx, float ky, float x1, float y1, float x2, float y2)
		{
			Vector2 k = new Vector2(kx, ky);
			Vector2 p1 = new Vector2(x1, y1);
			Vector2 p2 = new Vector2(x2, y2);

			if (x1 == x2)
			{
				return Vector2.Distance(k, new Vector2(x1, ky));
			}
			if (y1 == y2)
			{
				return Vector2.Distance(k, new Vector2(kx, y1));
			}

			float m1 = (y2 - y1) / (x2 - x1);
			float m2 = 0 - (1 / m1);

			Vector2 pn = new Vector2(1f, m2);
			float de1 = DistanceFromLine(k, p1, pn + p1);
			float de2 = DistanceFromLine(k, p2, pn + p2);

			//return de1 + de2 - Vector2.Distance(p1, p2);

			if (de1 + de2 > Vector2.Distance(p1, p2) + 0.002f) //outside segment bounds, distance should be from k to one of the end-points
			{
				return Mathf.Min(Vector2.Distance(p1, k), Vector2.Distance(p2, k));
			}
			return DistanceFromLine(k, p1, p2);
		}

		public static bool PointInTriangleStrong(Vector2 p, Vector2 a, Vector2 b, Vector2 c, int strength)
		{
			strength = Mathf.Clamp(strength, 1, 6);

			/*int total = 0;							old
			if (PointInTriangle(p, a, b, c)) total++;
			if (PointInTriangle(p, b, c, a)) total++;
			if (PointInTriangle(p, c, a, b)) total++;
			if (PointInTriangle(p, a, c, b)) total++;
			if (PointInTriangle(p, c, b, a)) total++;
			if (PointInTriangle(p, b, a, c)) total++;
			*/
			for (int i = 0, t = 0; i < 6; i++)
			{
				if (PointInTriangle(p, a, b, c)) t++;
				if (i % 2 == 0)
				{
					Vector2 temp = a;
					a = b;
					b = temp;
				}
				else
				{
					Vector2 temp = b;
					b = c;
					c = temp;
				}
				/*			attempt
				SwapTwoVariable(a, i % 2 == 0 ? b : c, out Vector2 one, out Vector2 two);
				a = one;
				if (i % 2 == 0) b = two; else c = two;
				*/
				if (t >= strength)
				{
					return true;
				}
			}
			return false;

			//return total >= strength;		old
		}

		public static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
		{
			float s1 = c.y - a.y;
			float s2 = c.x - a.x;
			float s3 = b.y - a.y;
			float s4 = p.y - a.y;

			float w1 = (a.x * s1 + s4 * s2 - p.x * s1) / (s3 * s2 - (b.x - a.x) * s1);
			float w2 = (s4 - w1 * s3) / s1;
			return w1 >= 0f && w2 >= 0f && (w1 + w2) <= 1f;
		}
		public static void SwapTwo(Vector2 a, Vector2 b, out Vector2 aa, out Vector2 bb)
		{
			aa = b;
			bb = a;
		}

		public static bool InRange(Vector2 f, float min, float max)
		{
			return InRange(f, Vector2.one * min, Vector2.one * max);
		}
		public static bool InRange(Vector2 f, Vector2 min, Vector2 max)
		{
			return f.x >= min.x && f.x <= max.x && f.y >= min.y && f.y <= max.y;
		}

		public static Vector2 Clamp(Vector2 f, float min, float max)
		{
			return Clamp(f, Vector2.one * min, Vector2.one * max);
		}
		public static Vector2 Clamp(Vector2 f, Vector2 min, Vector2 max)
		{
			return new Vector2(Mathf.Clamp(f.x, min.x, max.x), Mathf.Clamp(f.y, min.y, max.y));
		}

		public static Vector2 PingPong(Vector2 v)
		{
			return new Vector2(V1.PingPong(v.x, 1f), V1.PingPong(v.y, 1f));
		}
		public static Vector2 PingPong(Vector2 v, Vector2 p)
		{
			return new Vector2(V1.PingPong(v.x, p.x), V1.PingPong(v.y, p.y));
		}
	}

	public static class V3
	{
		public static Vector3 FlattenPlane(Vector3 v, int axis)
		{
			return new Vector3(axis == 0 ? 0f : v.x, axis == 1 ? 0f : v.y, axis == 2 ? 0f : v.z);
			//return new Vector3(axis != 0 ? v.x : 0f, axis != 1 ? v.y : 0f, axis != 2 ? v.z : 0f);
		}

		public static Vector3 FlattenAxis(Vector3 v, int keep)
		{
			return new Vector3(keep == 0 ? v.x : 0f, keep == 1 ? v.y : 0f, keep == 2 ? v.z : 0f);
		}

		public static float LateralDistance(Vector3 p1, Vector3 p2, Vector3 axis)
		{
			Vector3 mult = Vector3.one - axis;
			return Vector3.Distance(Vector3.Cross(p1, mult), Vector3.Cross(p2, mult));
		}

		public static float DistanceFromLine(Vector3 k, Vector3 v1, Vector3 v2)
		{
			return Vector3.Cross(k - v1, k - v2).magnitude / (v2 - v1).magnitude;
			/*
			Vector3 d = v2 - v1;
			Vector3 a = k - v1;
			Vector3 b = k - v2;

			Vector3 ab = Vector3.Cross(a, b);
			return ab.magnitude / d.magnitude;
			*/
		}
		/*
		public static float DistanceFromPlane(Vector3 k, Vector3 pOrigin, Vector3 pNormal)
		{

			return 0f;
		}
		*/

		public static bool InRange(Vector3 f, float min, float max)
		{
			return InRange(f, Vector3.one * min, Vector3.one * max);
		}
		public static bool InRange(Vector3 f, Vector3 min, Vector3 max)
		{
			return f.x >= min.x && f.x <= max.x && f.y >= min.y && f.y <= max.y && f.z >= min.z && f.z <= max.z;
		}

		public static Vector3 Clamp(Vector3 f, float min, float max)
		{
			return Clamp(f, Vector3.one * min, Vector3.one * max);
		}
		public static Vector3 Clamp(Vector3 f, Vector3 min, Vector3 max)
		{
			return new Vector3(Mathf.Clamp(f.x, min.x, max.x), Mathf.Clamp(f.y, min.y, max.y), Mathf.Clamp(f.z, min.z, max.z));
		}

		public static Vector3 PingPong(Vector3 v)
		{
			return new Vector3(V1.PingPong(v.x, 1f), V1.PingPong(v.y, 1f), V1.PingPong(v.z, 1f));
		}
		public static Vector3 PingPong(Vector3 v, Vector3 p)
		{
			return new Vector3(V1.PingPong(v.x, p.x), V1.PingPong(v.y, p.y), V1.PingPong(v.z, p.z));
		}
	}

	public static class V4
	{
		public static bool InRange(Vector4 f, float min, float max)
		{
			return InRange(f, Vector4.one * min, Vector4.one * max);
		}
		public static bool InRange(Vector4 f, Vector4 min, Vector4 max)
		{
			bool walk = f.x >= min.x && f.x <= max.x && f.y >= min.y && f.y <= max.y;
			walk = walk && f.z >= min.z && f.z <= max.z && f.w >= min.w && f.w <= max.w;
			return walk; /*
			bool x = f.x >= min.x && f.x <= max.x;
			bool y = f.y >= min.y && f.y <= max.y;
			bool z = f.z >= min.z && f.z <= max.z;
			bool w = f.w >= min.w && f.w <= max.w;
			return x && y && z && w; */
		}

		public static Vector4 PingPong(Vector4 v)
		{
			return new Vector4(V1.PingPong(v.x, 1f), V1.PingPong(v.y, 1f), V1.PingPong(v.z, 1f), V1.PingPong(v.w, 1f));
		}
		public static Vector4 PingPong(Vector4 v, Vector4 p)
		{
			return new Vector4(V1.PingPong(v.x, p.x), V1.PingPong(v.y, p.y), V1.PingPong(v.z, p.z), V1.PingPong(v.w, p.w));
		}
	}

	public class TriVal
	{
		public int value
		{
			get { value = Mathf.Clamp(value, 1, 3); return value; }
			set { this.value = Mathf.Clamp(value, 1, 3); }
		}

		public bool Equals(int value)
		{
			return this.value == value;
		}

		public TriVal(int value)
		{
			this.value = Mathf.Clamp(value, 1, 3);
		}
		/*
		 * expansion ideas:
		
		public string binary;
		public string binVal;
		public bool bin0;
		public bool bin1;

		public TriBool(bool bin0, bool bin1)
		{
			//if not bin0 then bin1 isn't needed, value is 1
			//if bin0 and bin 1 then value is 2 \\ if bin0 and not bin 1
			//( 00(FF) = 0(F) = 1 | 10(TF) = 2 | 11(TT) = 3 )
			value = bin0 ? (bin1 ? 3 : 2) : 1;

			binary = bin0 ? (bin1 ? "11" : "10") : "01";
			binVal = binary == "01" ? "00" : binary;

			this.bin0 = bin0;
			this.bin1 = bin1;
		}*/
	}

	//todo Bernstein / DeCasteljau / Polynomial Coefficients
	public static class Bez2
	{
		public static Vector2 EvaluateQuadratic(Vector2 p1, Vector2 p2, Vector2 p3, float t)
		{
			Vector2 a = Vector2.Lerp(p1, p2, t);
			Vector2 b = Vector2.Lerp(p2, p3, t);

			return Vector2.Lerp(a, b, t);
		}

		public static Vector2 EvaluateCubic(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float t)
		{
			Vector2 a = EvaluateQuadratic(p1, p2, p3, t);
			Vector2 b = EvaluateQuadratic(p2, p3, p4, t);

			return Vector2.Lerp(a, b, t);
		}

		public static Vector2 EvaluateFromArray(Vector2[] points, float t)
		{
			if (points.Length == 0) { return Vector2.zero; }
			if (points.Length == 1) { return points[0]; }

			Vector2[] nest = points;

			for (int i = 0; i < points.Length; i++)
			{
				if (nest.Length == 2)
				{
					return Vector2.Lerp(nest[0], nest[1], t);
				}

				List<Vector2> l = new List<Vector2>();

				for (int j = 0; j < nest.Length - 1; j++)
				{
					l.Add(Vector2.Lerp(nest[j], nest[j + 1], t));
				}

				nest = l.ToArray();
			}

			return nest[0];
		}
	}

	public static class Bez3
	{
		public static Vector3 EvaluateQuadratic(Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			Vector3 a = Vector3.Lerp(p1, p2, t);
			Vector3 b = Vector3.Lerp(p2, p3, t);

			return Vector3.Lerp(a, b, t);
		}

		public static Vector3 EvaluateCubic(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
		{
			Vector3 a = EvaluateQuadratic(p1, p2, p3, t);
			Vector3 b = EvaluateQuadratic(p2, p3, p4, t);

			return Vector3.Lerp(a, b, t);
		}

		public static Vector3 EvaluateFromArray(Vector3[] points, float t)
		{
			if (points.Length == 0) { return Vector3.zero; }
			if (points.Length == 1) { return points[0]; }

			Vector3[] nest = points;

			for (int i = 0; i < points.Length; i++)
			{
				if (nest.Length == 2)
				{
					return Vector3.Lerp(nest[0], nest[1], t);
				}

				List<Vector3> l = new List<Vector3>();

				for (int j = 0; j < nest.Length - 1; j++)
				{
					l.Add(Vector3.Lerp(nest[j], nest[j + 1], t));
				}

				nest = l.ToArray();
			}

			return nest[0];
		}
	}

	public enum GradientRepeatType
	{
		NoneExtend,
		SawtoothWave,
		TriangularWave,
		Truncate
	}

	public static class ColFunc
	{
		public static Color AverageArray(params Color[] array)
		{
			Vector3 rolling = new Vector3(0, 0, 0);
			foreach (Color col in array)
			{
				rolling.x += col.r * col.r;
				rolling.y += col.g * col.g;
				rolling.z += col.b * col.b;
			}

			rolling /= array.Length;

			return new Color(Mathf.Sqrt(rolling.x), Mathf.Sqrt(rolling.y), Mathf.Sqrt(rolling.z));
		}
		public static float EvaluateFromGradientRepeatType(float f, GradientRepeatType t)
		{
			switch (t)
			{
				case GradientRepeatType.NoneExtend:
				{
					return Mathf.Clamp01(f);
				}
				case GradientRepeatType.Truncate:
				{
					return (Mathf.Clamp01(f) == f) ? f : -1f;
				}
				case GradientRepeatType.SawtoothWave:
				{
					return V1.SawtoothWave(f, 1f);
				}
				case GradientRepeatType.TriangularWave:
				{
					return V1.TriangularWave(f, 1f);
				}
			}
			return f;
		}
		public static Color[] IterateFromGradient(Gradient g, int stepCount, GradientRepeatType repeatType)
		{
			return IterateFromGradient(g, 1f / Mathf.Max(stepCount + 0f, 1f), 0f, stepCount, repeatType);
		}
		public static Color[] IterateFromGradient(Gradient g, float stepSize, float startPoint, int stepCount, GradientRepeatType repeatType)
		{
			Color[] colors = new Color[stepCount];

			for (int i = 0; i < stepCount; i++)
			{
				colors[i] = g.Evaluate(EvaluateFromGradientRepeatType(startPoint + i * stepSize, repeatType));
			}

			return colors;
		}
	}
}