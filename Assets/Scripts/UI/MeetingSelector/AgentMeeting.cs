using System.Collections.Generic;
using JetBrains.Annotations;

namespace UI.MeetingSelector
{
    public class AgentMeeting
    {
        public class Node
        {
            [CanBeNull] public readonly Node Parent;
            public readonly List<Node> Children = new List<Node>(6);

            public Node(Node parent)
            {
                Parent = parent;
            }

            public int Size()
            {
                if (Children.Count == 0)
                {
                    return 1;
                }

                var totalChildSize = 0;
                foreach (var child in Children)
                {
                    totalChildSize += child.Size();
                }

                return totalChildSize;
            }

            public int Depth()
            {
                return Parent == null ? 1 : 1 + Parent.Depth();
            }
        }
    
        public readonly Node HeadNode;

        public AgentMeeting()
        {
            HeadNode = new Node(null);
            HeadNode.Children.Add(new Node(HeadNode));
        }

        public void AddChildTo(Node parentNode)
        {
            if (parentNode.Children.Count == 0)
            {
                parentNode.Children.Add(new Node(parentNode));
            }
        
            parentNode.Children.Add(new Node(parentNode));
        }

        public bool RemoveChild(Node childNode)
        {
            if (childNode.Parent == null)
            {
                return false;
            }
        
            var parentNode = childNode.Parent;
            var childIndex = parentNode.Children.IndexOf(childNode);

            if (childIndex == -1)
            {
                return false;
            }
        
            if (parentNode.Children.Count == 2)
            {
                parentNode.Children.Clear();

                return true;
            }
        
            childNode.Parent.Children.RemoveAt(childIndex);

            return true;
        }
    }
}