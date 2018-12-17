using UnityEngine;
namespace Subtegral.StealthAgent.GameCore
{
    public interface IDataContainer
    {
        void AppendControllerData(Transform transform,IDataController controller);

        IDataController GetController();

        (Vector3 position, Quaternion rotation) GetTransform();
    }
}