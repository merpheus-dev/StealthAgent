using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
namespace Subtegral.StealthAgent.Interactions
{
    [RequireComponent(typeof(LineManager))]
    public class Laser : MonoBehaviour, IWatcher, IDataController
    {
        private LineManager lineManager;

        private RaycastHit2D hit;

        public Transform Emitter;
        public Transform Receiver;

        [SerializeField]
        private LasersData _data;

        public void Inject(IDataContainer container)
        {
            _data = (LasersData)container;
            _data.AppendControllerData(transform,this);
        }

        public IDataContainer GetContainer()
        {
            _data.AppendControllerData(transform,this);
            return _data;
        }

        private void Awake()
        {
            lineManager = GetComponent<LineManager>();
        }

        private void Update()
        {
            hit = Physics2D.Linecast(Emitter.position, Receiver.position, _data.LayerMask);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                foreach (var item in enemies)
                {
                    if (item.CurrentEnemyState == Enemy.EnemyState.Chase)
                        continue;
                    item.CurrentEnemyState = Enemy.EnemyState.Notified;
                    item.Chase();
                }
            }

            DrawFOV();
        }

        public void DrawFOV()
        {
            lineManager.RenderLine(Emitter.position, Receiver.position, _data.lineProfile);
        }


    }

}