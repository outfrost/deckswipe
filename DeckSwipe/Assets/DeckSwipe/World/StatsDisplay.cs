using DeckSwipe.Gamestate;
using Outfrost;
using UnityEngine;
using UnityEngine.UI;

namespace DeckSwipe.World {
	
	public class StatsDisplay : MonoBehaviour {
		
		public Image coalBar;
		public Image foodBar;
		public Image healthBar;
		public Image hopeBar;
		public float relativeMargin;

		public ActionPreview coalActionPreview;
		public ActionPreview foodActionPreview;
		public ActionPreview healthActionPreview;
		public ActionPreview hopeActionPreview;

		private float minFillAmount;
		private float maxFillAmount;
		
		private void Awake() {
			minFillAmount = Mathf.Clamp01(relativeMargin);
			maxFillAmount = Mathf.Clamp01(1.0f - relativeMargin);
			
			if (!Util.IsPrefab(gameObject)) {
				Stats.AddChangeListener(this);
				TriggerUpdate();
			}
		}
		
		public void TriggerUpdate() {
			coalBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, Stats.CoalPercentage);
			foodBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, Stats.FoodPercentage);
			healthBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, Stats.HealthPercentage);
			hopeBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, Stats.HopePercentage);
		}

		public void TriggerActionPreviewUpdate()
        {
			if (Stats.leftActionModification != null)
			{
				coalActionPreview.ChangeLeftActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.CoalPercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Coal + Stats.leftActionModification.coal) / Stats.MaxStatValue()));
				foodActionPreview.ChangeLeftActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.FoodPercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Food + Stats.leftActionModification.food) / Stats.MaxStatValue()));
				healthActionPreview.ChangeLeftActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.HealthPercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Health + Stats.leftActionModification.health) / Stats.MaxStatValue()));
				hopeActionPreview.ChangeLeftActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.HopePercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Hope + Stats.leftActionModification.hope) / Stats.MaxStatValue()));
			}
			if (Stats.rightActionModification != null)
			{
				coalActionPreview.ChangeRightActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.CoalPercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Coal + Stats.rightActionModification.coal) / Stats.MaxStatValue()));
				foodActionPreview.ChangeRightActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.FoodPercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Food + Stats.rightActionModification.food) / Stats.MaxStatValue()));
				healthActionPreview.ChangeRightActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.HealthPercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Health + Stats.rightActionModification.health) / Stats.MaxStatValue()));
				hopeActionPreview.ChangeRightActionPreview(Mathf.Lerp(minFillAmount, maxFillAmount, Stats.HopePercentage),
					Mathf.Lerp(minFillAmount, maxFillAmount, (float)(Stats.Hope + Stats.rightActionModification.hope) / Stats.MaxStatValue()));
			}
		}

		public void TriggerUpdateActionPreviewAlpha(float leftAlpha, float rightAlpha)
        {
			coalActionPreview.UpdateActionPreviewAlpha(leftAlpha, rightAlpha);
			foodActionPreview.UpdateActionPreviewAlpha(leftAlpha, rightAlpha);
			healthActionPreview.UpdateActionPreviewAlpha(leftAlpha, rightAlpha);
			hopeActionPreview.UpdateActionPreviewAlpha(leftAlpha, rightAlpha);
		}
		
	}
	
}
