using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using UnityEngine.UI;
namespace Subtegral.StealthAgent.Interactions
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HackableObject : MonoBehaviour, IInterruptableInteractable,IDataController
    {
        public bool IsHacked = false;


        [HideInInspector]
        public SpriteRenderer Sprite;

        public Color CachedColor;

        [SerializeField]
        private HackableData _data;

        public void Inject(IDataContainer container)
        {
            _data = (HackableData)container;
        }

        public IDataContainer GetContainer()
        {
            _data.AppendControllerData(transform,this);
            return _data;
        }



        void Awake()
        {
            Sprite = GetComponent<SpriteRenderer>();
            CachedColor = Sprite.color;
        }

        public void ResetSpriteColor()
        {
            Sprite.color = CachedColor;
        }

        public void Interact()
        {
            PlayerEventHandler.OnHackStarted(this);
        }

        public bool IsCurrentlyInteractable(params object[] optionalObjects)
        {
            return !IsHacked;
        }

        public void InterruptInteraction()
        {
            if (!IsHacked)
                PlayerEventHandler.OnHackInterrupted(this);
        }

    }

}