using UnityEngine;
using Subtegral.StealthAgent.GameCore;

public static class ControllerFactory
{
    public static IDataController CreateFactory<T>(T t,IDataContainer container)
    {
        var controllerObject = Resources.Load(string.Format("Resources/{0}",typeof(T).GetType().ToString()));
        IDataController controller = (IDataController) Object.Instantiate(controllerObject, container.GetTransform().position, container.GetTransform().rotation);
        controller.Inject(container);
        return controller;
    }
}