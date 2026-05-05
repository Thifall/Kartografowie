using Kartografowie.Assets.Scripts.Tutorial.Objectives;
using Kartografowie.Assets.Scripts.Tutorial.Steps.Abstract;

namespace Kartografowie.Assets.Scripts.Tutorial.Steps
{
    public class EndTutorialStep : TutorialStep
    {
        private readonly TutorialController _tutorialController;

        public EndTutorialStep(TutorialController tutorialController)
        {
            _tutorialController = tutorialController;
        }

        public override void EnterStep()
        {
            _tutorialController.SetTutorialPanelDetails(
                "That's it!",
                "You have completed the tutorial. Enjoy playing the game!",
                true);
        }

        public override void ExitStep()
        {
        }

        public override void HandleEvent(object eventData)
        {
            if (eventData is ContinueButtonClicked)
            {
                _tutorialController.NextStep();
            }
        }
    }
}
