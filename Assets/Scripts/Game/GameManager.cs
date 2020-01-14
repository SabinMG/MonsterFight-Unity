using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SpaceOrigin.DesignPatterns;
using SpaceOrigin.Data;

namespace SpaceOrigin.MonsterFight
{
    public enum TurnState
    {
        Player,
        AI
    }

    public class GameManager : Singleton<GameManager>
    {
        public PlayerUnitsController playerUnitsController;
        public AIUnitsController aIUnitsController;
        public GameUIHandler gameUIHndlr;
        public IntSO alivePlayerUnitsSO;
        public IntSO aliveAIUnitsSO;

        private bool _isGameRunning = false;
        private TurnState currenTurnState = TurnState.Player; // starts with player turn

        //events
        public UnityEvent playerWInEvent;
        public UnityEvent playerLoseEvent;

        public delegate void TurnDelegate(TurnState state);
        public static TurnDelegate onTurnStateChangedEvent;
        public delegate void GameOverEvent();
        public static GameOverEvent onVictoryEvent;
        public static GameOverEvent onDefeatEvent;

        private void Start()
        {
            StartCoroutine(InitializeAllUnits(()=> 
            {
                gameUIHndlr.InitializeUI();
            })); 
        }

        private void Update()
        {
            if (!_isGameRunning) return;

            switch (currenTurnState)
            {
                case TurnState.Player:
                    playerUnitsController.Execute();
                    break;
                case TurnState.AI:
                    aIUnitsController.Execute();
                    break;
            }
        }
        private void OnEnable()
        {
            alivePlayerUnitsSO.onValueChanged.AddListener(OnAlivePlayerUnitsChanged);
            aliveAIUnitsSO.onValueChanged.AddListener(OnAliveAIUnitsChanged);

            PlayerUnitsController.onPlayerTurnOverEvent += OnPlayerTurnOverEvent;
            AIUnitsController.onAITurnOverEvent += OnAITurnOverEvent;  
        }

        private void OnDisable()
        {
            alivePlayerUnitsSO.onValueChanged.RemoveListener(OnAlivePlayerUnitsChanged);
            aliveAIUnitsSO.onValueChanged.RemoveListener(OnAliveAIUnitsChanged);

            PlayerUnitsController.onPlayerTurnOverEvent -= OnPlayerTurnOverEvent;
            AIUnitsController.onAITurnOverEvent -= OnAITurnOverEvent;
        }

        private void OnAlivePlayerUnitsChanged(int aliveUnitNo)
        {
            if (aliveUnitNo == 0)
            {
                if (onVictoryEvent != null) onVictoryEvent.Invoke();
                _isGameRunning = false;

                playerWInEvent?.Invoke();
            }
        }

        private void OnAliveAIUnitsChanged(int aliveUnitNo)
        {
            if (aliveUnitNo == 0)
            {
                if (onDefeatEvent != null) onDefeatEvent.Invoke();
                _isGameRunning = false;

                playerLoseEvent?.Invoke(); // 
            }
        }

        private void OnPlayerTurnOverEvent()
        {
            SwitchState(TurnState.AI);
        }

        private void OnAITurnOverEvent()
        {
            SwitchState(TurnState.Player);
        }

        private IEnumerator InitializeAllUnits(Action onComplete)
        {
            yield return new WaitForSeconds(.5f);
            playerUnitsController.InitializeUnits();
            yield return new WaitForSeconds(.5f); // 4 units init time
            aIUnitsController.InitializeUnits();
            yield return new WaitForSeconds(.6f);  // units init time
            _isGameRunning = true;
            SwitchState(TurnState.Player);
            onComplete.Invoke();
        }

        private void SwitchState(TurnState newState)
        {
            if (!_isGameRunning) return;
            currenTurnState = newState;
            switch (currenTurnState)
            {
                case TurnState.Player:
                    aIUnitsController.Exit();
                    playerUnitsController.Enter();
                    break;
                case TurnState.AI:
                    playerUnitsController.Exit();
                    aIUnitsController.Enter();
                    break;
            }

            if (onTurnStateChangedEvent != null) onTurnStateChangedEvent.Invoke(newState);
        }
    }
}
