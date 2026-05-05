using Kartografowie.Assets.Scripts.Tutorial.Objectives;
using Kartografowie.Assets.Scripts.Tutorial.Steps.Abstract;
namespace Kartografowie.Assets.Scripts.Tutorial
{
    public class WelcomeStep : TutorialStep
    {
        private readonly TutorialController _tutorialController;

        public WelcomeStep(TutorialController controller)
        {
            _tutorialController = controller;
        }

        public override void EnterStep()
        {
           _tutorialController.SetTutorialPanelDetails(
               "Welcome to the Game!",
               @"
               In this tutorial, you will learn the basics of gameplay.
               Click the button to continue.",
               true);
        }

        public override void ExitStep()
        {
            //unblock input/buttons
            //hide welcome message
            //hide "next" button
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
