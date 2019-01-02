using UnityEngine;
using Subtegral.StealthAgent.GameCore;
public static class ControllerFactory
{
    //Generics bitch!
    public static IDataController CreateFactory<T>(DataContainer container)
    {
        string trgtPath = string.Format("Prefabs/{0}", typeof(T).Name);
        Debug.Log(trgtPath);
        var controllerObject = Resources.Load<GameObject>(trgtPath);
        IDataController controller = Object.Instantiate(controllerObject, container.GetTransform().position, container.GetTransform().rotation).GetComponent<IDataController>();
        controller.Inject(container);
        return controller;
    }
}