namespace Subtegral.StealthAgent.GameCore
{
    public interface IDataController
    {
        void Inject(IDataContainer container);

        IDataContainer GetContainer();
    }
}