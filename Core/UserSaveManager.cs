using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.Skills;

namespace Subtegral.StealthAgent.GameCore
{
    public static class UserSaveManager 
    {
        private static SkillElement[] SkillPocket = new SkillElement[3];

        public static void ChangeSkillPocket(int pocketIndex, SkillElement element)
        {
            if (pocketIndex > 2 || pocketIndex < 0)
                return;
            SkillPocket[pocketIndex] = element;
        }

        public static SkillElement GetSkillElement(int pocketIndex)
        {
            if (pocketIndex > 2 || pocketIndex < 0)
                throw new Exception("Index is not in ranges of pocket!!!");
            return SkillPocket[pocketIndex];
        }
    }

    [System.Serializable]
    public class SkillElement
    {
        public Skill Skill;
    }
}