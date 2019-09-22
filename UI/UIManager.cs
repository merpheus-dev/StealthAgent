using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using Subtegral.StealthAgent.Interactions;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Subtegral.StealthAgent.UI
{
    public class UIManager : MonoBehaviour
    {
        //Noise Generator
        public Button NoiseObjectDropButton;

        //Take-down Enemy
        public Button DragEnemyCorpseButton;
        public Button ReleaseEnemyCorpseButton;
        public Image PlayerProgressBar;

        //Taser Gun
        public Button TaserThrowButton;

        //Hackables
        IEnumerator cachedBlinkCoroutine = null;
        IEnumerator cachedProgressBarCoroutine = null;

        //Door lockpick
        IEnumerator cachedProgressBarForDoor = null;

        //Hostile
        public Button TakeHostile;
        public Button DropHostile;

        //Win/Lose GUI
        public GameObject WinScreen;
        public GameObject LoseScreen;

        private Player player;
        void Start()
        {
            player = FindObjectOfType<Player>();
            ClearCache();
            PopulateEvents();
        }

        private void ClearCache()
        {
            PlayerEventHandler.OnItemGrabbed = null;
            PlayerEventHandler.OnKnockingEnemy = null;
            PlayerEventHandler.OnKnockingEnemyInterrupted = null;
            PlayerEventHandler.OnDeadEnemyCollisionEnter = null;
            PlayerEventHandler.OnDeadEnemyCollisionExit = null;
            PlayerEventHandler.OnHackStarted = null;
            PlayerEventHandler.OnHackInterrupted = null;
            PlayerEventHandler.OnHackSucceed = null;
            PlayerEventHandler.OnHostileEnter = null;
            PlayerEventHandler.OnHostileExit = null;
            PlayerEventHandler.OnDoorInteractionStart = null;
            PlayerEventHandler.OnDoorInteractionEnd = null;
            PlayerEventHandler.OnGameOver = null;
        }

        private void PopulateEvents()
        {
            //ITEM EVENTS
            PlayerEventHandler.OnItemGrabbed += (InventoryItem item) =>
            {
                switch (item)
                {
                    case NoiseGeneratorItem f:
                        NoiseObjectDropButton.gameObject.SetActive(true);
                        NoiseObjectDropButton.onClick.AddListener(() =>
                        {
                            Instantiate(f.Prefab, player.transform.position, Quaternion.identity);
                            NoiseObjectDropButton.gameObject.SetActive(false);
                        });
                        break;
                    case TaserItem t:
                        TaserThrowButton.gameObject.SetActive(true);
                        TaserThrowButton.onClick.AddListener(() =>
                        {
                            TaserThrowButton.gameObject.SetActive(false);
                            StartCoroutine(WaitForTaserDropPoint(t));
                        });
                        break;
                }
            };

            //ENEMY TAKE DOWN
            PlayerEventHandler.OnKnockingEnemy += (Enemy val) =>
            {
                Debug.Log("TAKING DOWN...");

                StartCoroutine(FillProgressBarForTakeDown(val));
            };

            PlayerEventHandler.OnKnockingEnemyInterrupted += (Enemy val) =>
              {
                  Debug.Log("INTERRUPTION");
                  StopAllCoroutines();
                  AfterEnemyTakeDownInteraction();
                  //If enemy couldnt successfully taken down, enemy can move.
                  val.ImMovable = false;

                  //Dont be an idiot and trigger chase
                  StartCoroutine(TriggerChase(val));
              };

            //ENEMY DRAG CORPSE
            PlayerEventHandler.OnDeadEnemyCollisionEnter += (Transform t) =>
              {
                  //Show drag button and attach dragging to button onClick event
                  DragEnemyCorpseButton.gameObject.SetActive(true);
                  DragEnemyCorpseButton.onClick.AddListener(() =>
                  {
                      t.SetParent(player.transform, true);
                      DragEnemyCorpseButton.gameObject.SetActive(false);
                      ReleaseEnemyCorpseButton.gameObject.SetActive(true);
                      ReleaseEnemyCorpseButton.onClick.RemoveAllListeners();
                      ReleaseEnemyCorpseButton.onClick.AddListener(() =>
                      {
                          PlayerEventHandler.OnDeadEnemyCollisionExit(t);
                      });
                  });
              };

            PlayerEventHandler.OnDeadEnemyCollisionExit += (Transform t) =>
              {
                  //Hide drag button,change
                  DragEnemyCorpseButton.gameObject.SetActive(false);
                  ReleaseEnemyCorpseButton.gameObject.SetActive(false);
                  DragEnemyCorpseButton.onClick.RemoveAllListeners();
                  t.SetParent(null, true);
                  t.GetComponent<Enemy>().ImMovable = true;
                  player.CurrentEnemyTarget = null;
              };


            //HACKABLES
            PlayerEventHandler.OnHackStarted += (HackableObject h) =>
            {
                cachedProgressBarCoroutine = FillProgressBarForHack(h);
                cachedBlinkCoroutine = BlinkHackable(h);

                StartCoroutine(cachedProgressBarCoroutine);
                StartCoroutine(cachedBlinkCoroutine);
            };
            //Probably not gonna work
            PlayerEventHandler.OnHackInterrupted += (HackableObject h) =>
            {
                StopHackCoroutines();

                h.ResetSpriteColor();
                AfterEnemyTakeDownInteraction();
            };

            PlayerEventHandler.OnHackSucceed += (HackableObject h) =>
              {
                  StopHackCoroutines();
                  h.Sprite.color = ((HackableData)h.GetContainer()).HackSuccessColor;
                  h.IsHacked = true;
                  AfterEnemyTakeDownInteraction();
              };

            PlayerEventHandler.OnHostileEnter += (Hostile h) =>
              {
                  TakeHostile.gameObject.SetActive(true);
                  TakeHostile.onClick.RemoveAllListeners();
                  TakeHostile.onClick.AddListener(() =>
                  {
                      TakeHostile.gameObject.SetActive(false);
                      DropHostile.gameObject.SetActive(true);
                      DropHostile.onClick.RemoveAllListeners();
                      DropHostile.onClick.AddListener(() =>
                      {
                          PlayerEventHandler.OnHostileExit(h);
                      });

                      h.SetTarget(player.transform);
                      h.Parented = true;
                  });
              };
            PlayerEventHandler.OnHostileExit += (Hostile h) =>
              {
                  TakeHostile.gameObject.SetActive(false);
                  if (h.Parented)
                  {
                      DropHostile.gameObject.SetActive(false);
                      h.SetTarget(null);
                      h.Parented = false;
                  }
              };
            //DOOR HACK EVENTS
            PlayerEventHandler.OnDoorInteractionStart += (Door d) =>
            {
                DoorData dat = d.GetContainer() as DoorData;
                cachedProgressBarForDoor = FillProgressBar(dat.Interval);
                StartCoroutine(cachedProgressBarForDoor);
            };

            PlayerEventHandler.OnDoorInteractionEnd += (Door d) =>
              {
                  if (cachedProgressBarForDoor != null)
                      StopCoroutine(cachedProgressBarForDoor);
                  cachedProgressBarForDoor = null;
              };

            //GAME OVER EVENTS
            PlayerEventHandler.OnGameOver += (bool status) =>
            {
                StopAllCoroutines();
                WinScreen.SetActive(status);
                LoseScreen.SetActive(!status);
                player.gameObject.SetActive(false);
            };
        }

        #region Shared Methods
        private void AfterEnemyTakeDownInteraction()
        {
            PlayerProgressBar.fillAmount = 0f;
            PlayerProgressBar.transform.parent.gameObject.SetActive(false);
        }

        private IEnumerator FillProgressBar(float limit)
        {
            PlayerProgressBar.transform.parent.gameObject.SetActive(true);
            PlayerProgressBar.GetComponentInParent<Canvas>().transform.position = (Vector2)player.transform.position + new Vector2(0f, .3f);
            float timeCounter = 0f;
            while (PlayerProgressBar.fillAmount < 1f)
            {
                if (timeCounter < limit)
                    timeCounter += Time.deltaTime;
                PlayerProgressBar.fillAmount = timeCounter / limit;
                yield return null;
            }
        }
        #endregion

        #region Take Down
        private IEnumerator FillProgressBarForTakeDown(Enemy enemy)
        {
            enemy.ImMovable = true;
            yield return FillProgressBar(((EnemyData)enemy.GetContainer()).EnemyTakeDownTime);

            //Deactivate enemy
            enemy.CurrentEnemyState = Enemy.EnemyState.Dead;
            enemy.gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
            yield return new WaitForSeconds(.2f);

            AfterEnemyTakeDownInteraction();

            //Trigger dragging
            PlayerEventHandler.OnDeadEnemyCollisionEnter(enemy.transform);
        }



        private IEnumerator TriggerChase(Enemy enemy)
        {
            //Trigger only failed take down attempted enemy
            yield return new WaitForSeconds(((EnemyData)enemy.GetContainer()).ChaseTriggerInterval);
            enemy.CurrentEnemyState = Enemy.EnemyState.Notified;
            enemy.Chase();
        }

        #endregion


        #region Hackables

        private void StopHackCoroutines()
        {
            if (cachedBlinkCoroutine != null)
            {
                StopCoroutine(cachedBlinkCoroutine);
            }
            if (cachedProgressBarCoroutine != null)
            {
                StopCoroutine(cachedProgressBarCoroutine);
            }

        }
        private IEnumerator FillProgressBarForHack(HackableObject hackableObject)
        {
            yield return FillProgressBar(((HackableData)hackableObject.GetContainer()).HackTime);
            PlayerEventHandler.OnHackSucceed(hackableObject);
        }

        private IEnumerator BlinkHackable(HackableObject hackableObject)
        {
            HackableData data = ((HackableData)hackableObject.GetContainer());
            while (!hackableObject.IsHacked)
            {
                hackableObject.Sprite.color = Color.Lerp(hackableObject.CachedColor, data.HackInProgressColor, Mathf.PingPong(Time.time, 1f) * data.HackBlinkDamping);
                yield return null;
            }
            hackableObject.Sprite.color = data.HackSuccessColor;
        }
        #endregion

        #region Taser
        private IEnumerator WaitForTaserDropPoint(TaserItem item)
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            GameObject taserBomb = Instantiate(item.Prefab, player.transform.position, Quaternion.identity);
            while (Vector2.Distance(taserBomb.transform.position, targetPosition) > .1f)
            {
                taserBomb.transform.position = Vector3.MoveTowards(taserBomb.transform.position, targetPosition, Time.deltaTime * 5f);
                yield return null;
            }
            taserBomb.GetComponent<TaserBomb>().TaseNearby(targetPosition);
        }
        #endregion

    }

}