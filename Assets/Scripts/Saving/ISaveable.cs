namespace RPG.Saving
{
    internal interface ISaveable
    {
        object CaptureState();

        void RestoreState(object state);
    }
}
