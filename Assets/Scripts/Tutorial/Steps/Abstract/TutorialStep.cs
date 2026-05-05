namespace Kartografowie.Assets.Scripts.Tutorial.Steps.Abstract
{
    public abstract class TutorialStep
    {
        public abstract void EnterStep();
        public abstract void ExitStep();

        public abstract void HandleEvent(object eventData);
    }
}
