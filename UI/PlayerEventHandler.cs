using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using UnityEngine.UI;
using Subtegral.StealthAgent.Interactions;

public static class PlayerEventHandler
{
    public static PlayerEvent<InventoryItem> OnItemGrabbed;
    public static PlayerEvent<Enemy> OnKnockingEnemy;
    public static PlayerEvent<Enemy> OnKnockingEnemyInterrupted;
    public static PlayerEvent<Transform> OnDeadEnemyCollisionEnter;
    public static PlayerEvent<Transform> OnDeadEnemyCollisionExit;
    public static PlayerEvent<HackableObject> OnHackStarted;
    public static PlayerEvent<HackableObject> OnHackInterrupted;
    public static PlayerEvent<HackableObject> OnHackSucceed;
    public static PlayerEvent<Hostile> OnHostileEnter;
    public static PlayerEvent<Hostile> OnHostileExit;
}
