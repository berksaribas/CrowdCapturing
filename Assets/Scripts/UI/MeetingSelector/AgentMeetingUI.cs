using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MeetingSelector
{
    [RequireComponent(typeof(AgentMeetingUIConfig))]
    public class AgentMeetingUI : MonoBehaviour, IPointerDownHandler
    {
        public AgentMeetingUIConfig Config;

        public readonly AgentMeeting AgentMeeting = new AgentMeeting();

        public struct Space
        {
            public AgentMeeting.Node AgentNode;
            public int ChildCount;
            public Rect Rect;
            public Vector2 Position;
            public float Radius;

            public Space(AgentMeeting.Node agentNode, Rect rect) : this()
            {
                AgentNode = agentNode;
                ChildCount = agentNode.Size();

                Rect = rect;

                const float heightMargin = 0.1f;
                const float availableHeight = 1f - 2f * heightMargin;
                const float widthMargin = 0.1f;
                const float availableWidth = 1f - 2f * widthMargin;
                Position = new Vector2(
                    widthMargin + rect.x + rect.width / 2f,
                    heightMargin + availableHeight * (rect.y + rect.height / 2f)
                );

                Radius = 0.036f + Mathf.Pow(agentNode.Size() * 0.00002f, 0.36f);
            }
        }

        private List<Space> spaces;

        public CirclesUI CirclesUI;
        public ArrowsUI ArrowsUI;
        public TextsUI TextsUI;

        private RectTransform rectTransform;
        private Rect rect;

        void Awake()
        {
            rectTransform = transform as RectTransform;
            Debug.Assert(rectTransform != null, nameof(rectTransform) + " != null");
            rect = rectTransform.rect;

            UpdateUI();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var bottomLeftCorner = (Vector2) rectTransform.position + rect.position;
            var rectSize = rect.size;

            var relativeMousePos = eventData.position - bottomLeftCorner;
            var normalizedMousePos = relativeMousePos / rectSize;

            print($"Normalized pos: {normalizedMousePos.ToString()}");

            var (closestAgent, _) = CirclesUI.GetClosestCircle(normalizedMousePos, 20f);

            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if (closestAgent.Depth() < Config.MaxDepth)
                    {
                        AgentMeeting.AddChildTo(closestAgent);
                    }
                    break;

                case PointerEventData.InputButton.Right:
                    AgentMeeting.RemoveChild(closestAgent);
                    break;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            spaces = CalculateSpacing(
                AgentMeeting.HeadNode,
                new Rect(0, 0, Config.ColumnSize, 1f)
            );

            CirclesUI.UpdateUI(spaces);
            TextsUI.UpdateUI(spaces);
            ArrowsUI.UpdateUI(spaces);
        }

        private List<Space> CalculateSpacing(AgentMeeting.Node agentNode, Rect rect)
        {
            var totalChildAgentCount = agentNode.Size();

            if (totalChildAgentCount == 0)
            {
                return null;
            }

            var spaces = new List<Space>(totalChildAgentCount);

            spaces.Add(new Space(agentNode, rect));

            if (totalChildAgentCount == 1)
            {
                return spaces;
            }

            var childAgentCount = agentNode.Children.Count;

            const float heightGap = 0.1f;
            var availableHeight = rect.height - (childAgentCount - 1) * heightGap;
            var perAgentHeight = availableHeight / totalChildAgentCount;

            var childSpace = new Rect(rect);
            childSpace.x += rect.width;

            for (var i = 0; i < childAgentCount; i++)
            {
                childSpace.height = perAgentHeight * agentNode.Children[i].Size();

                spaces.AddRange(
                    CalculateSpacing(agentNode.Children[i], childSpace)
                );

                childSpace.y += childSpace.height + heightGap;
            }

            return spaces;
        }

        public Vector2 World2Local(Vector2 worldPos)
        {
            return rectTransform.InverseTransformPoint(worldPos);
        }

        public Vector2 Local2World(Vector2 localPos)
        {
            return rectTransform.TransformPoint(localPos);
        }
    }
}