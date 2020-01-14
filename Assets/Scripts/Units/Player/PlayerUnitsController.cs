using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SpaceOrigin.Inputs;
using SpaceOrigin.Utilities;
using SpaceOrigin.Data;
using SpaceOrigin.DesignPatterns;
using System.Linq;

namespace SpaceOrigin.MonsterFight
{
    public class PlayerUnitsController : Singleton<PlayerUnitsController>, IMouseClickCommand, IMouseMoveCommand
    {
        public LevelGrid levelGrid;
        public LayerMask gridPlaneLayer;
        public LayerMask playerLayer; 
        public LayerMask obstacleLayer;

        public GameObject moveHintIndicator;
        public Transform[] spawnPoints;
        public IntSO alivePlayersUnitsSO;

        private int _maxUnits = 2; //  maximum units is 2 for now
        private PlayerUnit[] _playerUnits = new PlayerUnit[2];
        private PlayerUnit _currentUnit;
        private Vector3 _lastMovePosition;
        private Camera _mainCamera;
        private AIUnitsController _aiUnitsCtrl;

        //events
        public UnityEvent playerClickFieldEvent;

        public delegate void AttackeDelegate();
        public static AttackeDelegate onPlayerUnitAttackPossibleEvent;
        public static AttackeDelegate onPlayerUnitAttackNotPossibleEvent;

        public delegate void PlayerTunrOverDelegate();
        public static PlayerTunrOverDelegate onPlayerTurnOverEvent;

        public delegate void PlayerEndTurnDelegate();
        public static PlayerEndTurnDelegate onPlayerEndTurnPossibleEvent;
        public static PlayerEndTurnDelegate onPlayerEndTurnNotPossibleEvent;

        void Start()
        {
            moveHintIndicator.SetActive(false);
            _aiUnitsCtrl = AIUnitsController.Instance;
        }

        public void InitializeUnits()
        {
            StartCoroutine(InitializeUnitsCR());
        }

        IEnumerator InitializeUnitsCR()
        {
            for (int i = 0; i < _maxUnits; i++)
            {
                GameAbstractFactory gameAbtractFactory = GameFactoryProducer.GetFactory("PlayerUnitsFactory");
                PlayerUnit newUnit = gameAbtractFactory.GetPlayerUnit();
                newUnit.alive = true;
                newUnit.gameObject.transform.position = spawnPoints[i].position;
                newUnit.gameObject.transform.eulerAngles = new Vector3(0,45,0);
                newUnit.gameObject.SetActive(true);
                _playerUnits[i] = newUnit;

                levelGrid.CloseCellAthisPosition(spawnPoints[i].position); // setting the cells as blocked
            }
            yield return null;
            _currentUnit = _playerUnits[0];
            alivePlayersUnitsSO.Value = _maxUnits;
        }

        public void Enter()
        {
            InputHandler.Instance.mouseMoveCommand = this;
            InputHandler.Instance.mouseClickCommand = this;

            PlayerUnit aliveUnit = null;
            for (int i = 0; i < _playerUnits.Length; i++)
            {
                if (_playerUnits[i].alive)
                {
                    _playerUnits[i].NewTurn();
                    if (aliveUnit ==  null) aliveUnit = _playerUnits[i]; // always choose the first ons
                }
            }
            aliveUnit.Selected();
            _currentUnit = aliveUnit;

            CheckForAttackIfPossible();

            if (onPlayerEndTurnPossibleEvent != null) onPlayerEndTurnPossibleEvent.Invoke();
        }

        public void Execute()
        {
            _currentUnit.Execute();
        }

        public void Exit()
        {
            InputHandler.Instance.mouseMoveCommand = null;
            InputHandler.Instance.mouseClickCommand = null;

            for (int i = 0; i < _playerUnits.Length; i++)
            {
                if (_playerUnits[i].alive)
                {
                    _playerUnits[i].UnSelected();
                }
            }
        }

        public void EndPlayerTurn()
        {
            if (onPlayerTurnOverEvent != null) onPlayerTurnOverEvent.Invoke();
        }

        public void ExecuteMouseMove(Vector3 inputPosition)
        {
            if (!_currentUnit.CanMove())
            {
               // moveHintIndicator.SetActive(false);
                return;
            }

            RaycastHit hitInfo = new RaycastHit();
            if (_mainCamera == null) _mainCamera = Camera.main;
            var ray = _mainCamera.ScreenPointToRay(inputPosition);

            if (Physics.Raycast(ray, out hitInfo,100, obstacleLayer)) //TODO: re write no two castin is neceesorry
            {
                moveHintIndicator.SetActive(false);
                return;
            }

            if (Physics.Raycast(ray, out hitInfo, 100, gridPlaneLayer))
            {
                if (!levelGrid.IsCellOpenAtLocation(hitInfo.point))
                {
                    moveHintIndicator.SetActive(false);
                    return;
                }
                Vector3 cellCenter = levelGrid.WorldToCellWorldPos(hitInfo.point);
                moveHintIndicator.SetActive(true);
                moveHintIndicator.transform.position = new Vector3(cellCenter.x, .01f, cellCenter.z);
                _lastMovePosition = cellCenter;
            }
            else
            {
                moveHintIndicator.SetActive(false);
            }
        }

        public void ExecuteMouseClick(Vector3 inputPosition)
        {
            if (!_currentUnit.CanMove())
            {
                return;
            }

            if (_mainCamera == null) _mainCamera = Camera.main;
            var ray = _mainCamera.ScreenPointToRay(inputPosition);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, 100, playerLayer)) // no need to cast two rays here and the next line  ToDO : rwrite
            {
                _currentUnit.UnSelected();
                _currentUnit = hitInfo.collider.gameObject.GetComponent<PlayerUnit>();
                _currentUnit.Selected();
                CheckForAttackIfPossible();
                return;
            }

            if (Physics.Raycast(ray, out hitInfo, 100, gridPlaneLayer))
            {
                if (!levelGrid.IsCellOpenAtLocation(hitInfo.point))
                {
                    moveHintIndicator.SetActive(false);
                    return;
                }

                playerClickFieldEvent?.Invoke();

                Vector3 cellCenter = levelGrid.WorldToCellWorldPos(hitInfo.point);
                moveHintIndicator.SetActive(true);
                moveHintIndicator.transform.position = new Vector3(cellCenter.x, .01f, cellCenter.z);
                _lastMovePosition = cellCenter;
                _currentUnit.OnReachTarget = OnUnitReachTarget; // funtion pointer
                levelGrid.OpenCellAthisPosition(_currentUnit.transform.position); // opening the grid cell 
                levelGrid.CloseCellAthisPosition(_lastMovePosition); // closing the grid cell on next move location
                _currentUnit.Move(_lastMovePosition);
                _currentUnit.attackTarget = null;
                if (onPlayerEndTurnNotPossibleEvent != null) onPlayerEndTurnNotPossibleEvent.Invoke();
                if (onPlayerUnitAttackNotPossibleEvent != null) onPlayerUnitAttackNotPossibleEvent.Invoke();
            }
        }

        public void Attack()
        {
            if (onPlayerEndTurnNotPossibleEvent != null) onPlayerEndTurnNotPossibleEvent.Invoke();
            _currentUnit.Attack();
            _currentUnit.OnAttackFinished = OnAttackFinished;
            if (onPlayerUnitAttackNotPossibleEvent != null) onPlayerUnitAttackNotPossibleEvent.Invoke();
        }

        public void OnUnitReachTarget()
        {
            moveHintIndicator.SetActive(false);
            CheckForAttackIfPossible();
            if (onPlayerEndTurnPossibleEvent != null) onPlayerEndTurnPossibleEvent.Invoke();
        }

        public void OnAttackFinished()
        {
            if (onPlayerEndTurnPossibleEvent != null) onPlayerEndTurnPossibleEvent.Invoke();
        }

        private void CheckForAttackIfPossible()
        {
            List<AIUnit> closeAIUnits = _aiUnitsCtrl.GetAIUnitsCloseby(_currentUnit);

            if (closeAIUnits.Count > 0)
            {
                AIUnit attackUnit = closeAIUnits[0];
                _currentUnit.attackTarget = attackUnit;

                if (onPlayerUnitAttackPossibleEvent != null && _currentUnit.CanDoAnotherAttack()) onPlayerUnitAttackPossibleEvent.Invoke();
            }
            else { if (onPlayerUnitAttackNotPossibleEvent != null) onPlayerUnitAttackNotPossibleEvent.Invoke(); }
          
        }

        public List<PlayerUnit> GetAllAlivePlayerUnits()
        {
            List<PlayerUnit> aliveUnits = new List<PlayerUnit>();
            for (int i = 0; i < _playerUnits.Length; i++)
            {
                if (_playerUnits[i].alive) aliveUnits.Add(_playerUnits[i]);
            }
           return aliveUnits;
        }

        public List<DistanceData> GetClosetPlayerUnitsData(AIUnit aiUnit) // retunrs ordered list
        {
            List<DistanceData> closetUnitsData = new List<DistanceData>();

            List<PlayerUnit> allAlivePlayerUnits = GetAllAlivePlayerUnits();
            for (int i = 0; i < allAlivePlayerUnits.Count; i++)
            {
                PlayerUnit unit = allAlivePlayerUnits[i];

                float distance = Vector3.Distance(aiUnit.transform.position, unit.transform.position);

                DistanceData distanceData= new DistanceData();
                distanceData.unit = unit;
                distanceData.distanceFromUnit = distance;

                closetUnitsData.Add(distanceData);
            }

            closetUnitsData = closetUnitsData.OrderBy(x => x.distanceFromUnit).ToList();
            return closetUnitsData;
        }
    }
}
