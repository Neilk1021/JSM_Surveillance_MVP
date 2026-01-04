using System;
using System.Collections;
using System.Collections.Generic;
using JSM.Surveillance.UI;
using UnityEngine;

namespace JSM.Surveillance
{
    public abstract class ProcessorObject : MachineObject 
    {
        [Header("Processor Info")]
        [SerializeField] private ProcessorData data;

        [SerializeField] private RecipeBank availableRecipes;
        [SerializeField] private Recipe selectedRecipe;

        public Recipe[] AvailableRecipes => availableRecipes.recipes;
        
        public Recipe Recipe => selectedRecipe;
        public ProcessorData Data => data;


        protected override void OnMouseDown()
        {
            if (Placed) {
                Grid.UIManager.SwitchUI(this);
            }

            base.OnMouseDown();
        }

        /// <summary>
        /// called to set what recipe processor will run
        /// </summary>
        public virtual void SetRecipe(Recipe recipe)
        {
            selectedRecipe = recipe;
            _grid.Modified();
        }

        public override void Sell()
        {
            SurveillanceGameManager.instance.MoneyManager.ChangeMoneyBy(data.UpfrontCost);
            base.Sell();
        }

        public override MachineInstance BuildInstance()
        {
            return new ProcessorInstance(selectedRecipe, data, inventorySize);
        }

        public void LoadRecipeById(string machineRecipeId)
        {
            selectedRecipe = availableRecipes.GetRecipeById(machineRecipeId);
        }
    }
}