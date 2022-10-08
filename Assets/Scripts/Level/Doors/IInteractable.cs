namespace Game.Level
{
    public interface IInteractable
    {
        void Interact();

        void Highlight();

        void Unhighlight();

        void InSight();

        void OutOfSight();
    }
}