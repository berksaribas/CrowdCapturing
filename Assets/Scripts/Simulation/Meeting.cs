using UnityEngine;

namespace Simulation
{
    public class Meeting
    {
        public readonly Agent[] Group;
        private int remainingAgents;
        public bool AllArrived => remainingAgents == 0;

        public readonly Vector3 Position;
        
        public readonly Meeting NextMeeting;
        //public MeetingPoint[] PreviousMeetings;

        public Meeting(Agent[] group, Vector3 position, Meeting nextMeeting)
        {
            Group = group;
            remainingAgents = Group.Length;
            Position = position;
            NextMeeting = nextMeeting;
        }

        public void Arrived()
        {
            remainingAgents--;

            if (AllArrived)
                foreach (var agent in Group)
                    agent.CurrentMeeting = NextMeeting;
        }

        public Agent[] GetWholeGroup()
        {
            var meetingPointer = this;
            
            while (meetingPointer.NextMeeting != null)
                meetingPointer = meetingPointer.NextMeeting;

            return meetingPointer.Group;
        }
    }
}