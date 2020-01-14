using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceOrigin.Utilities;
using SpaceOrigin.DesignPatterns;
using SpaceOrigin.Data;
using System.Linq;

namespace SpaceOrigin.MonsterFight
{
    public class AIUnitsController : Singleton<AIUnitsController>
    {
        public LevelGrid levelGrid;
        public Transform[] spawnPoints;
        public IntSO aliveAIUnitsSO;

        private int _maxUnits = 4; //  maximum units is 4 for now
        private AIUnit[] _aiUnits = new AIUnit[4];
        private AIUnit _currentAIUnit;
        private Stack<AIUnit> _aiTurnStack; // individual units executes based on this stack
        private PlayerUnitsController _playerUnitsCtrl;

        public delegate void AITurnOverDelegate();
        public static AITurnOverDelegate onAITurnOverEvent;

        // Start is called before the first frame update
        void Start()
        {
            _playerUnitsCtrl = PlayerUnitsController.Instance;
        }

        public void InitializeUnits()
        {
            StartCoroutine(InitializeUnitsCR());
        }

        IEnumerator InitializeUnitsCR()
        {
            for (int i = 0; i < _maxUnits; i++)
            {
                GameAbstractFactory gameAbtractFactory = GameFactoryProducer.GetFactory("AIUnitsFactory");
                AIUnit newUnit = gameAbtractFactory.GetAIUnit();
                newUnit.alive = true;
                newUnit.gameObject.transform.position = spawnPoints[i].position;
                newUnit.gameObject.transform.eulerAngles = new Vector3(0, 225, 0);
                newUnit.gameObject.SetActive(true);
                newUnit.onUnitLostLifeEvent += OnUnitLostLifeEvent;
                newUnit.levelGrid = levelGrid;
                _aiUnits[i] = newUnit;
                levelGrid.CloseCellAthisPosition(spawnPoints[i].position); // setting the cells as blocked

                yield return new WaitForSeconds(.1f);
            }
            _currentAIUnit = _aiUnits[0];
            aliveAIUnitsSO.Value = _maxUnits;
        }
        public void Enter()
        {
            _currentAIUnit = null;
            Invoke("NewTurn",2.0f); // little delay for showing enemy turn UI
        }

        public void Execute()
        {
            if(_currentAIUnit != null)
            {
                _currentAIUnit.Execute();
            }
        }

        public void Exit()
        {
        
        }

        private void NewTurn()
        {
            BuildTurnStack();
            NextUntsTurn();
        }

        private void BuildTurnStack() // distance based stack, basic AI for now
        {
            List<AIUnit> allAliveAIUnits = GetAllAliveAIUnits();
            for (int i = 0; i < allAliveAIUnits.Count; i++)
            {
                AIUnit aiUnit = allAliveAIUnits[i];
                aiUnit.closetPlayerUnitsData = _playerUnitsCtrl.GetClosetPlayerUnitsData(aiUnit);
            }

            allAliveAIUnits = allAliveAIUnits.OrderByDescending(x => x.closetPlayerUnitsData[0].distanceFromUnit).ToList();

            _aiTurnStack = new Stack<AIUnit>();
            for (int i = 0; i < allAliveAIUnits.Count; i++)
            {
                _aiTurnStack.Push(allAliveAIUnits[i]);
            }
        }

        private void NextUntsTurn()
        {
            if (_aiTurnStack.Count == 0) { EndAITurn(); return; }
          
            if (_currentAIUnit != null) _currentAIUnit.UnSelected();
            _currentAIUnit = _aiTurnStack.Pop();
            _currentAIUnit.Selected();
            _currentAIUnit.onUnitFinishTurn += OnUnitFinishTurn;
            _currentAIUnit.NewTurn();
        }

        private void OnUnitFinishTurn(AIUnit unit)
        {
            unit.onUnitFinishTurn -= OnUnitFinishTurn;
            NextUntsTurn();
        }

        private void OnUnitLostLifeEvent(Unit unit)
        {
            unit.onUnitLostLifeEvent -= OnUnitLostLifeEvent;
            levelGrid.OpenCellAthisPosition(unit.transform.position); // setting the cells as blocked
        }

        public List<AIUnit> GetAllAliveAIUnits()
        {
            List<AIUnit> aliveUnits = new List<AIUnit>();
            for (int i = 0; i < _aiUnits.Length; i++)
            {
                if (_aiUnits[i].alive) aliveUnits.Add(_aiUnits[i]);
            }

            return aliveUnits;
        }

        public List<AIUnit> GetAIUnitsCloseby(PlayerUnit playerUnit)
        {
            List<AIUnit> closeByAIUnits = new List<AIUnit>();
            List<AIUnit> allAliveAIUnits = GetAllAliveAIUnits();

            for (int i = 0; i < allAliveAIUnits.Count; i++)
            {
                AIUnit unit = allAliveAIUnits[i];
                bool close = levelGrid.IsTwoPositionsCloseByOnGrid(playerUnit.transform.position, unit.transform.position);

                if (close) closeByAIUnits.Add(unit);
            }

            return closeByAIUnits;
        }

        public void EndAITurn()
        {
            _currentAIUnit.UnSelected();
            if (onAITurnOverEvent != null) onAITurnOverEvent.Invoke();
        }
    }
}
