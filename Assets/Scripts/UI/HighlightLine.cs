using UnityEditor;
using UnityEngine;

namespace UI
{
	public static class HighlightLine
	{
		private static GameObject  highlightLinePrefab;
		
		public static GameObject CreateLine(Vector3 from, Vector3 to, Transform parent, float thickness = 1f)
		{
			var difference = from - to;

			var center = from - difference / 2f;
			var rotation = Quaternion.Euler(90f, Mathf.Rad2Deg * Mathf.Atan2(difference.x, difference.z), 0);

			if (highlightLinePrefab == null)
			{
				highlightLinePrefab = Resources.Load<GameObject>("HighlighterLine");
			}
			
			var highlighter = Object.Instantiate(highlightLinePrefab, center, rotation, parent);
			highlighter.transform.localScale = new Vector3(thickness, difference.magnitude / 2f, thickness);

			return highlighter;
		}

		public static void UpdateLine(GameObject highlightLine, Vector3 from, Vector3 to)
		{
			var difference = from - to;

			var center = from - difference / 2f;
			var rotation = Quaternion.Euler(90f, Mathf.Rad2Deg * Mathf.Atan2(difference.x, difference.z), 0);

			var t = highlightLine.transform;
			t.SetPositionAndRotation(center, rotation);

			var ls = t.localScale;
			ls.y = difference.magnitude / 2f;
			t.localScale = ls;
		}
	}
}
