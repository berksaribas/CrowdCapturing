using Simulation;
using UnityEngine;
using Util;
using World;

public class SimulationController : MonoBehaviour
{
    public static SimulationController Instance { get; private set; }

    public SequenceManager SequenceManager;
    public CrowdManager CrowdManager;
    public SimulationManager SimulationManager;
    public Building[] Buildings;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[1], 30, 45000, 600)); //UC->UC 2
        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[2], 80, 45000, 600,
            EasingFunction.Ease.EaseOutExpo)); //UC->FENS
        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[3], 130, 45000, 600,
            EasingFunction.Ease.EaseOutExpo)); //UC->FASS
        
        SequenceManager.InsertSequence(new Sequence(Buildings[1], Buildings[1], 100, 45000, 600)); //UC 2->FMAN
        SequenceManager.InsertSequence(new Sequence(Buildings[1], Buildings[5], 40, 45000, 600)); //UC 2->SL

        SequenceManager.InsertSequence(new Sequence(Buildings[2], Buildings[0], 300, 45000, 600,
            EasingFunction.Ease.EaseOutExpo)); //FENS->UC
        SequenceManager.InsertSequence(new Sequence(Buildings[2], Buildings[3], 70, 45000, 600)); //FENS->FASS
        
        SequenceManager.InsertSequence(new Sequence(Buildings[3], Buildings[0], 330, 45000, 600,
            EasingFunction.Ease.EaseOutExpo)); //FASS->UC
        SequenceManager.InsertSequence(new Sequence(Buildings[3], Buildings[2], 120, 45000, 600)); //FASS->FENS
        SequenceManager.InsertSequence(new Sequence(Buildings[3], Buildings[4], 60, 45000, 600)); //FASS->FMAN
        
        SequenceManager.InsertSequence(new Sequence(Buildings[4], Buildings[0], 50, 45000, 600)); //FMAN->UC
        SequenceManager.InsertSequence(new Sequence(Buildings[4], Buildings[1], 130, 45000, 600)); //FMAN->UC 2
    }
}