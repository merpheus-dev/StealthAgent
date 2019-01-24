using UnityEngine;
using Subtegral.StealthAgent.GameCore;
public static class ControllerFactory
{
    //Generics bitch!
    public static (IDataController controller,GameObject gameObj) CreateFactory<T>(DataContainer container)
    {
        string trgtPath = string.Format("Prefabs/{0}", typeof(T).Name);
        Debug.Log(trgtPath);
        var controllerObject = Resources.Load<GameObject>(trgtPath);
        GameObject gameObj = Object.Instantiate(controllerObject, container.GetTransform().position, container.GetTransform().rotation);
        IDataController controller = gameObj.GetComponent<IDataController>();
        controller.Inject(container);
        return (controller, gameObj);
    }
}