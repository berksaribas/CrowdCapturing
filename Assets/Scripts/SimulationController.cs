using Simulation;
using UnityEngine;
using World;

public class SimulationController : MonoBehaviour
{
    public SequenceManager SequenceManager;
    public Building[] Buildings;

    private void Awake()
    {
        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[1], 100, 0, 120));
//        SequenceManager.InsertSequence(new Sequence(Buildings[1], Buildings[0], 300, 0, 120));
    }
}