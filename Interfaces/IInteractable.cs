namespace Subtegral.StealthAgent.GameCore
{
    public interface IInteractable
    {
        void Interact();

        bool IsCurrentlyInteractable(params object[] optionalObjects);
    }
}
