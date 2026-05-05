using Kartografowie.Assets.Scripts.Tutorial.Objectives;
using Kartografowie.Assets.Scripts.Tutorial.Objectives.Abstract;
using Kartografowie.Assets.Scripts.Tutorial.Steps;
using Kartografowie.Assets.Scripts.Tutorial.Steps.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kartografowie.Assets.Scripts.Tutorial
{
    public class TutorialController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _tutorialPanel;
        [SerializeField] private TextMeshProUGUI _tutorialTitle;
        [SerializeField] private TextMeshProUGUI _tutorialDescription;
        [SerializeField] private Button ContinueButton;

        private readonly List<TutorialStep> _tutorialSteps = new();
        private int _currentStepIndex = -1;
        private TutorialStep _currentStep;

        public void ForwardEvent(TutorialObjective eventData)
        {
            _currentStep.HandleEvent(eventData);
        }

        public void NextStep()
        {
            _currentStep?.ExitStep();

            _currentStepIndex++;
            if (_currentStepIndex >= _tutorialSteps.Count)
            {
                EndTutorial();
                return;
            }

            _currentStep = _tutorialSteps[_currentStepIndex];
            _currentStep.EnterStep();
        }

        private void OnEnable()
        {
            ContinueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        private void OnDisable()
        {
            ContinueButton.onClick.RemoveListener(OnContinueButtonClicked);
        }

        private void Start()
        {
            _tutorialSteps.Add(new WelcomeStep(this));
            _tutorialSteps.Add(new EndTutorialStep(this));
            NextStep();
        }

        private void EndTutorial()
        {
            _tutorialPanel.SetActive(false);
            Debug.Log("Tutorial completed!");
        }

        public void SetTutorialPanelDetails(string title, string description, bool shouldSetPanelActive = true)
        {
            _tutorialPanel.SetActive(shouldSetPanelActive);
            _tutorialTitle.text = title;
            _tutorialDescription.text = description;
        }

        public void OnContinueButtonClicked()
        {
            ForwardEvent(new ContinueButtonClicked());
        }
    }
}
