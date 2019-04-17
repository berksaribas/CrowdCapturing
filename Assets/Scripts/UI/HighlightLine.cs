using UnityEngine;

namespace UI
{
	public class HighlightLine : MonoBehaviour
	{
		public static GameObject  HighlightLinePrefab;
		
		public static GameObject CreateNew(Vector3 from, Vector3 to, Transform parent, float thickness = 1f)
		{
			var difference = from - to;

			var center = from - difference / 2f;
			var rotation = Quaternion.Euler(90f, Mathf.Rad2Deg * Mathf.Atan2(difference.x, difference.z), 0);

			if (HighlightLinePrefab == null)
			{
				HighlightLinePrefab = Resources.Load<GameObject>("HighlighterLine");
			}
			
			var highlighter = Instantiate(HighlightLinePrefab, center, rotation, parent);
			highlighter.transform.localScale = new Vector3(thickness, difference.magnitude / 2f, thickness);

			return highlighter;
		}
	}
}
