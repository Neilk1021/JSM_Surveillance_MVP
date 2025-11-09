using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewRecipeBank", menuName = "Factory/RecipeBank")]
    public class RecipeBank : ScriptableObject
    {
        public Recipe[] recipes;

        public Recipe GetRecipeByName(string name)
        {
            return Array.Find(recipes, r => r.name == name);
        }

        public Recipe[] GetAllRecipes()
        {
            return recipes;
        }
    }
}
