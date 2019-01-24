using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using Pathfinding;

namespace Subtegral.StealthAgent.Skills
{
    public class TimeEffector : MonoBehaviour
    {
        public float TimeCoeff = .1f;

        private List<GameObject> encapsulatedObjects = new List<GameObject>();

        /*Skills own effects*/
        private void OnTriggerEnter2D(Collider2D collision)
        {
            encapsulatedObjects.Add(collision.gameObject);
            Manipulate(collision, true);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            encapsulatedObjects.Remove(collision.gameObject);
            Manipulate(collision, false);
        }
        private void OnDestroy()
        {
            foreach (var item in encapsulatedObjects)
            {
                Manipulate(null, false, item);
            }
        }
        private void Manipulate(Collider2D collision, bool slowDown,GameObject target=null)
        {
            IDataController controller = target==null ? collision.GetComponent<IDataController>() : target.GetComponent<IDataController>();
            if (controller == null)
                return;
            switch (controller)
            {
                case Enemy e:
                    e.TimeCoefficient = slowDown ? TimeCoeff : 1f;
                    e.GetComponent<AILerp>().speed = slowDown ? e.MovementSpeed * TimeCoeff : e.MovementSpeed;
                    break;
            }
        }
    } 
}
