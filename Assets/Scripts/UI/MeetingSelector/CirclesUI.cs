using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace UI.MeetingSelector
{
    public class Circle
    {
        public AgentMeeting.Node AgentNode;
        public Vector2 Center;
        public float Radius;

        public Color Color;
        public bool HasColor = false;

        public Circle(AgentMeeting.Node agentNode, Vector2 center, float radius)
        {
            AgentNode = agentNode;
            Center = center;
            Radius = radius;
        }

        public void SetColor(Color color)
        {
            HasColor = true;
            Color = color;
        }

        public void SetDefaultColor()
        {
            HasColor = false;
        }
    }

    [RequireComponent(typeof(Image))]
    public class CirclesUI : MonoBehaviour
    {
        public AgentMeetingUIConfig Config;
    
        public List<Circle> Circles;

        private Material mat;
        public Color DefaultCircleColor = Color.white;

        private RectTransform rectTransform;
        private Rect rect;

        private void Awake()
        {
            Circles = new List<Circle>(Config.MaxCount);
        
            rectTransform = transform as RectTransform;
            Debug.Assert(rectTransform != null, nameof(rectTransform) + " != null");
            rect = rectTransform.rect;

            mat = GetComponent<Image>().material;

            //    This will set the maximum size of expected arrays
            mat.SetVectorArray("_Circles", new Vector4[Config.MaxCount]);
            mat.SetColorArray("_CircleColors", new Color[Config.MaxCount]);
        }

        public void UpdateUI(List<AgentMeetingUI.Space> spaces)
        {
            Circles.Clear();

            foreach (var space in spaces)
            {
                Circles.Add(new Circle(space.AgentNode, space.Position, space.Radius));
            }
        
            UpdateMaterial();
        }

        public void UpdateMaterial()
        {
            mat.SetFloat("_ScreenRatio", rect.width / rect.height);

            var circles = new Vector4[Config.MaxCount];

            var limit = Math.Min(Circles.Count, Config.MaxCount);
            for (var i = 0; i < limit; i++)
            {
                var mb = Circles[i];
                circles[i] = new Vector4(mb.Center.x, mb.Center.y, mb.Radius, 1f);
            }

            mat.SetVectorArray("_Circles", circles);
        
            var circleColors = Circles
                .Take(Config.MaxCount)
                .Select(mb => mb.HasColor ? mb.Color : DefaultCircleColor)
                .ToArray();
        
            mat.SetColorArray("_CircleColors", circleColors);
        }
    
        public (AgentMeeting.Node, float distance) GetClosestCircle(Vector2 worldPosition)
        {
            var smallestDistance = float.MaxValue;
            AgentMeeting.Node closestAgent = null;

            foreach (var circle in Circles)
            {
                var distance = Vector2.Distance(worldPosition, circle.Center);
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    closestAgent = circle.AgentNode;
                }
            }

            return (closestAgent, smallestDistance);
        }

        public (AgentMeeting.Node, float distance) GetClosestCircle(Vector2 worldPosition, float maxDistance)
        {
            var (closestCircle, distance) = GetClosestCircle(worldPosition);
        
            if (distance < maxDistance)
            {
                return (closestCircle, distance);
            }

            return (null, distance);
        }
    }
}