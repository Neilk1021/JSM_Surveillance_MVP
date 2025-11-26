using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.Surveillance
{
    [CreateAssetMenu(fileName = "NewRecipeBank", menuName = "JSM/Surveillance/RecipeBank")]
    public class RecipeBank : ScriptableObject
    {
        public Recipe[] recipes;

        /// <summary>
        /// Finds a recipe by name
        /// </summary>
        public Recipe GetRecipeByName(string name)
        {
            return Array.Find(recipes, r => r.name == name);
        }

        /// <summary>
        /// Gets all stored recipes
        /// </summary>
        public Recipe[] GetAllRecipes()
        {
            return recipes;
        }
    }
}
