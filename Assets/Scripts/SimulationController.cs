using Simulation;
using UnityEngine;
using Util;
using World;

public class SimulationController : MonoBehaviour
{
	private static SimulationController _instance;

	public static SimulationController Instance
	{
		get { return _instance; }
	}

	public SequenceManager SequenceManager;
	public CrowdManager CrowdManager;
	public SimulationManager SimulationManager;
	public Building[] Buildings;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}

		SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[1], 100, 45000, 600,
			EasingFunction.Ease.EaseOutExpo)); //UC->FENS
        SequenceManager.InsertSequence(new Sequence(Buildings[0], Buildings[2], 150, 45000, 600,
	        EasingFunction.Ease.EaseOutExpo)); //UC->FASS
        
        SequenceManager.InsertSequence(new Sequence(Buildings[1], Buildings[0], 350, 45000, 600,
	        EasingFunction.Ease.EaseOutExpo)); //FENS->UC
        SequenceManager.InsertSequence(new Sequence(Buildings[2], Buildings[0], 350, 45000, 600,
	        EasingFunction.Ease.EaseOutExpo)); //FASS->UC
        
        SequenceManager.InsertSequence(new Sequence(Buildings[1], Buildings[2], 70, 45000, 600)); //FENS->FASS
        SequenceManager.InsertSequence(new Sequence(Buildings[2], Buildings[1], 120, 45000, 600)); //FASS->FENS
	}
}