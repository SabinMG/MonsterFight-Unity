using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SpaceOrigin.MonsterFight
{
    public class GameUIHandler : MonoBehaviour
    {
        public Button attackButton;
        public Button endTurnButton;
        public Image enemyTurnImage;
        public Image vicotoryImage;
        public Image defeatImage;
        private float enemyTurnImageYPos;

        private void Start()
        {
            attackButton.gameObject.SetActive(false);
            endTurnButton.gameObject.SetActive(false);
            vicotoryImage.gameObject.SetActive(false);
            defeatImage.gameObject.SetActive(false);
            enemyTurnImageYPos = enemyTurnImage.rectTransform.position.y;
        }

        private void OnEnable()
        {
            PlayerUnitsController.onPlayerUnitAttackPossibleEvent += OnPlayerUnitAttackPossibleEvent;
            PlayerUnitsController.onPlayerUnitAttackNotPossibleEvent += OnPlayerUnitAttackNotPossibleEvent;
            PlayerUnitsController.onPlayerEndTurnPossibleEvent += OnPlayerEndTurnPossibleEvent;
            PlayerUnitsController.onPlayerEndTurnNotPossibleEvent += OnPlayerEndTurnNotPossibleEvent;

            GameManager.onTurnStateChangedEvent += OnTurnStateChangedEvent;
            GameManager.onVictoryEvent += OnVictoryEvent;
            GameManager.onDefeatEvent += OnDefeatEvent;

            attackButton.onClick.AddListener(OnAttackButtonClick);
            endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
        }

        private void OnDisable()
        {
            PlayerUnitsController.onPlayerUnitAttackPossibleEvent -= OnPlayerUnitAttackPossibleEvent;
            PlayerUnitsController.onPlayerUnitAttackNotPossibleEvent -= OnPlayerUnitAttackNotPossibleEvent;
            PlayerUnitsController.onPlayerEndTurnPossibleEvent -= OnPlayerEndTurnPossibleEvent;
            PlayerUnitsController.onPlayerEndTurnNotPossibleEvent -= OnPlayerEndTurnNotPossibleEvent;

            GameManager.onTurnStateChangedEvent -= OnTurnStateChangedEvent;
            GameManager.onVictoryEvent -= OnVictoryEvent;
            GameManager.onDefeatEvent -= OnDefeatEvent;

            attackButton.onClick.RemoveListener(OnAttackButtonClick);
            endTurnButton.onClick.RemoveListener(OnEndTurnButtonClick);
        }

        public void InitializeUI()
        {
            attackButton.gameObject.SetActive(true);
            endTurnButton.gameObject.SetActive(true);
            attackButton.interactable = false;
        }

        public void OnAttackButtonClick()
        {
            PlayerUnitsController.Instance.Attack();
        }

        public void OnEndTurnButtonClick()
        {
            PlayerUnitsController.Instance.EndPlayerTurn();
        }

        private void OnPlayerUnitAttackPossibleEvent()
        {
            attackButton.interactable = true;
        }

        private void OnPlayerUnitAttackNotPossibleEvent()
        {
            attackButton.interactable = false;
        }

        private void OnPlayerEndTurnPossibleEvent()
        {
            endTurnButton.interactable = true;
        }

        private void OnPlayerEndTurnNotPossibleEvent()
        {
            endTurnButton.interactable = false;
        }

        private void OnVictoryEvent()
        {
            DesableUIElements();
            vicotoryImage.gameObject.SetActive(true);
        }

        private void OnDefeatEvent()
        {
            DesableUIElements();
            defeatImage.gameObject.SetActive(true);
        }

        private void DesableUIElements()
        {
            attackButton.interactable = false;
            endTurnButton.interactable = false;
            enemyTurnImage.transform.DOMoveY(enemyTurnImageYPos, 1.0f);
        }

        private void OnTurnStateChangedEvent(TurnState newState)
        {
            switch (newState)
            {
                case TurnState.Player:
                    //attackButton.interactable = false;
                    endTurnButton.interactable = true;
                    enemyTurnImage.transform.DOMoveY(enemyTurnImageYPos, 1.0f);
                    break;
                case TurnState.AI:
                    enemyTurnImage.transform.DOMoveY(enemyTurnImageYPos - 150.0f, 1.0f);
                    attackButton.interactable = false;
                    endTurnButton.interactable = false;
                    break;
            }
        }
    }
}
