using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MeetingSelector
{
    public class ArrowsUI : Graphic
    {
        public float ArrowHeadLength = 30f;
        public float ArrowWidth = 14f;
    
        private readonly List<Vector3[]> allQuads = new List<Vector3[]>();

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            print("OnPopulateMesh Called");

            vh.Clear();
            var arrowColor = color;
            foreach (var quad in allQuads)
            {
                vh.AddUIVertexQuad(new[]
                {
                    new UIVertex {position = quad[0], color = arrowColor},
                    new UIVertex {position = quad[1], color = arrowColor},
                    new UIVertex {position = quad[2], color = arrowColor},
                    new UIVertex {position = quad[3], color = arrowColor}
                });
            }
        }

        public void UpdateUI(List<AgentMeetingUI.Space> spaces)
        {
            var agent2space = new Dictionary<AgentMeeting.Node, AgentMeetingUI.Space>(spaces.Count);

            foreach (var space in spaces)
            {
                agent2space.Add(space.AgentNode, space);
            }

            allQuads.Clear();
            var rectSize = rectTransform.rect.size;
            var pivot = rectTransform.pivot;
            foreach (var space in spaces)
            {
                if (space.AgentNode.Parent == null) continue;

                var parentSpace = agent2space[space.AgentNode.Parent];

                var direction = (parentSpace.Position - space.Position).normalized;
                var tail = (space.Position - pivot + direction * space.Radius) * rectSize;
                var head = (parentSpace.Position - pivot - direction * parentSpace.Radius) * rectSize;
            
//            tail += space.Radius * 100f * direction;
//            head -= parentSpace.Radius * 100f * direction;

                allQuads.AddRange(GenerateArrowPositions(tail, head));
            }

            UpdateGeometry();
        }

        private List<Vector3[]> GenerateArrowPositions(Vector2 head, Vector2 tail)
        {
            var direction = (Vector3) (head - tail).normalized;
            var perpendicularDirection = new Vector3(direction.y, -direction.x);

            var headMid = (Vector3) tail;
            var headTop = headMid + (perpendicularDirection * ArrowWidth) + (direction * ArrowHeadLength);
            var headBot = headMid - (perpendicularDirection * ArrowWidth) + (direction * ArrowHeadLength);

            var tailMid = (Vector3) head - (direction * ArrowHeadLength);
            var tailTop = tailMid + (perpendicularDirection * ArrowWidth) + (direction * ArrowHeadLength);
            var tailBot = tailMid - (perpendicularDirection * ArrowWidth) + (direction * ArrowHeadLength);

            return new List<Vector3[]>(2)
            {
                new[]
                {
                    tailMid,
                    headMid,
                    headTop,
                    tailTop
                },
                new[]
                {
                    tailBot,
                    headBot,
                    headMid,
                    tailMid
                }
            };
        }
    }
}