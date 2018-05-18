using System;
using System.Globalization;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI.Localization;

namespace DLC.Multiagent.DesktopApp.UI
{
	internal static class TelerikHelper
	{
		/// <summary>
		/// <see cref="RadToolTip"/> does not support multi-threaded GUI as it uses shared state internally 
		/// and will throw an "Cross-thread operation not valid" InvalidOperationException when used in such a scenario.
		/// To prevent that, this method will instead use a standard <see cref="ToolTip"/> control to display the tooltips.
		/// </summary>
		/// <param name="owner">The owner window.</param>
		/// <param name="tooltip">The tooltip control that will display the tooltips.</param>
		/// <param name="control">The Telerik control that needs to display tooltips.</param>
		public static void RegisterTooltipForRadControl(Form owner, ToolTip tooltip, RadControl control)
		{
			if (owner == null) throw new ArgumentNullException("owner");
			if (tooltip == null) throw new ArgumentNullException("tooltip");
			if (control == null) throw new ArgumentNullException("control");
			
			control.ShowItemToolTips = false;
			control.ToolTipTextNeeded +=
				(s, e) =>
				{
					if (!string.IsNullOrEmpty(e.ToolTipText))
					{
						tooltip.Hide(owner);
						tooltip.Show(e.ToolTipText, owner, owner.PointToClient(Cursor.Position));
					}
				};
		}

		public static void InitializeLocalizationProviders()
		{
			// pas thread-safe
			RadGridLocalizationProvider.CurrentProvider = new FrenchRadGridLocalizationProvider();
		}

		private class FrenchRadGridLocalizationProvider
			: RadGridLocalizationProvider
		{
			public override CultureInfo Culture
			{
				get { return CultureInfo.CreateSpecificCulture("fr-CA"); }
			}

			public override string GetLocalizedString(string id)
			{
				// http://www.telerik.com/support/code-library/french-radgridviewlocalization-q2-2009-sp1
				switch (id)
				{
					case RadGridStringId.AddNewRowString: return "Cliquer pour ajouter une nouvelle ligne";
					case RadGridStringId.BestFitMenuItem: return "Ajuster la taille de la colonne";
					case RadGridStringId.ClearSortingMenuItem: return "Annuler les tris";
					case RadGridStringId.ClearValueMenuItem: return "Effacer la Valeur";
					case RadGridStringId.ColumnChooserFormCaption: return "Masqueur de colonnes";
					case RadGridStringId.ColumnChooserFormMessage: return "Ajouter ici une colonne\npour la faire disparaitre\ntemporairement de la vue";
					case RadGridStringId.ColumnChooserMenuItem: return "Masqueur de colonnes";
					case RadGridStringId.ConditionalFormattingBtnAdd: return "Ajouter";
					case RadGridStringId.ConditionalFormattingBtnApply: return "Appliquer";
					case RadGridStringId.ConditionalFormattingBtnCancel: return "Annuler";
					case RadGridStringId.ConditionalFormattingBtnOK: return "OK";
					case RadGridStringId.ConditionalFormattingBtnRemove: return "Supprimer";
					case RadGridStringId.ConditionalFormattingCaption: return "Surligneur conditionnel";
					case RadGridStringId.ConditionalFormattingChkApplyToRow: return "Appliquer aux lignes existantes";
					case RadGridStringId.ConditionalFormattingChooseOne: return "[Choisir un type]";
					case RadGridStringId.ConditionalFormattingContains: return "Contient [Valeur1]";
					case RadGridStringId.ConditionalFormattingDoesNotContain: return "Ne contient pas [Valeur1]";
					case RadGridStringId.ConditionalFormattingEndsWith: return "Finit par [Valeur1]";
					case RadGridStringId.ConditionalFormattingEqualsTo: return "Est égal à [Valeur1]";
					case RadGridStringId.ConditionalFormattingGrpConditions: return "Conditions";
					case RadGridStringId.ConditionalFormattingGrpProperties: return "Propriétés";
					case RadGridStringId.ConditionalFormattingIsBetween: return "Est compris entre [Valeur1] et [Valeur2]";
					case RadGridStringId.ConditionalFormattingIsGreaterThan: return "Est supérieur à [Valeur1]";
					case RadGridStringId.ConditionalFormattingIsGreaterThanOrEqual: return "Est supérieur ou égal à [Valeur1]";
					case RadGridStringId.ConditionalFormattingIsLessThan: return "Est inférieur à [Valeur1]";
					case RadGridStringId.ConditionalFormattingIsLessThanOrEqual: return "Est inférieur ou égal à [Valeur1]";
					case RadGridStringId.ConditionalFormattingIsNotBetween: return "Non compris entre [Valeur1] et [Valeur2]";
					case RadGridStringId.ConditionalFormattingIsNotEqualTo: return "Est différent de [Valeur1]";
					case RadGridStringId.ConditionalFormattingLblColumn: return "Colonne:";
					case RadGridStringId.ConditionalFormattingLblName: return "Nom:";
					case RadGridStringId.ConditionalFormattingLblType: return "Type:";
					case RadGridStringId.ConditionalFormattingLblValue1: return "Valeur 1:";
					case RadGridStringId.ConditionalFormattingLblValue2: return "Valeur 2:";
					case RadGridStringId.ConditionalFormattingMenuItem: return "Formatage conditionnel";
					case RadGridStringId.ConditionalFormattingRuleAppliesOn: return "La règle s'applique au champ:";
					case RadGridStringId.ConditionalFormattingStartsWith: return "Commence par [Valeur1]";
					case RadGridStringId.CopyMenuItem: return "Copier";
					case RadGridStringId.CustomFilterDialogBtnCancel: return "Annuler";
					case RadGridStringId.CustomFilterDialogBtnOk: return "OK";
					case RadGridStringId.CustomFilterDialogCaption: return "Filtre personnalisé";
					case RadGridStringId.CustomFilterDialogLabel: return "Afficher les lignes où";
					case RadGridStringId.CustomFilterDialogRbAnd: return "Et";
					case RadGridStringId.CustomFilterDialogRbOr: return "Ou";
					case RadGridStringId.CustomFilterMenuItem: return "Menu filtre personnalisé";
					case RadGridStringId.CutMenuItem: return "Couper";
					case RadGridStringId.DeleteRowMenuItem: return "Effacer la ligne";
					case RadGridStringId.EditMenuItem: return "Edition";
					case RadGridStringId.FilterFunctionBetween: return "Compris entre";
					case RadGridStringId.FilterFunctionContains: return "Contient";
					case RadGridStringId.FilterFunctionCustom: return "Filtre personnalisé";
					case RadGridStringId.FilterFunctionDoesNotContain: return "Ne contient pas";
					case RadGridStringId.FilterFunctionEndsWith: return "Finit par";
					case RadGridStringId.FilterFunctionEqualTo: return "Est égal à";
					case RadGridStringId.FilterFunctionGreaterThan: return "Est supérieur à";
					case RadGridStringId.FilterFunctionGreaterThanOrEqualTo: return "Est supérieur ou égal à";
					case RadGridStringId.FilterFunctionIsEmpty: return "Est vide";
					case RadGridStringId.FilterFunctionIsNull: return "Est nul";
					case RadGridStringId.FilterFunctionLessThan: return "Est inférieur à";
					case RadGridStringId.FilterFunctionLessThanOrEqualTo: return "Est inférieur ou égal à";
					case RadGridStringId.FilterFunctionNoFilter: return "Pas de filtre";
					case RadGridStringId.FilterFunctionNotBetween: return "Non compris entre";
					case RadGridStringId.FilterFunctionNotEqualTo: return "Est différent de";
					case RadGridStringId.FilterFunctionNotIsEmpty: return "Est non vide";
					case RadGridStringId.FilterFunctionNotIsNull: return "Est non nul";
					case RadGridStringId.FilterFunctionStartsWith: return "Commence par";
					case RadGridStringId.FilterLogicalOperatorAnd: return "Et";
					case RadGridStringId.FilterLogicalOperatorOr: return "Ou";
					case RadGridStringId.FilterMenuButtonCancel: return "Annuler";
					case RadGridStringId.FilterMenuButtonOK: return "OK";
					case RadGridStringId.FilterOperatorBetween: return "Compris entre";
					case RadGridStringId.FilterOperatorContains: return "Contient";
					case RadGridStringId.FilterOperatorCustom: return "Personnalisé";
					case RadGridStringId.FilterOperatorDoesNotContain: return "Ne contient pas";
					case RadGridStringId.FilterOperatorEndsWith: return "Finit par";
					case RadGridStringId.FilterOperatorEqualTo: return "Égal à";
					case RadGridStringId.FilterOperatorGreaterThan: return "Supérieur";
					case RadGridStringId.FilterOperatorGreaterThanOrEqualTo: return "Supérieur ou égal";
					case RadGridStringId.FilterOperatorIsContainedIn: return "Contenu dans";
					case RadGridStringId.FilterOperatorIsEmpty: return "Est vide";
					case RadGridStringId.FilterOperatorIsLike: return "Est similaire";
					case RadGridStringId.FilterOperatorIsNull: return "Est nul";
					case RadGridStringId.FilterOperatorLessThan: return "Inférieur";
					case RadGridStringId.FilterOperatorLessThanOrEqualTo: return "Inférieur ou égal";
					case RadGridStringId.FilterOperatorNoFilter: return "Pas de filtre";
					case RadGridStringId.FilterOperatorNotBetween: return "Pas compris entre";
					case RadGridStringId.FilterOperatorNotEqualTo: return "Différent de";
					case RadGridStringId.FilterOperatorNotIsContainedIn: return "Pas contenu dans";
					case RadGridStringId.FilterOperatorNotIsEmpty: return "Non vide";
					case RadGridStringId.FilterOperatorNotIsLike: return "Non similaire";
					case RadGridStringId.FilterOperatorNotIsNull: return "Non nul";
					case RadGridStringId.FilterOperatorStartsWith: return "Commence par";
					case RadGridStringId.GroupByThisColumnMenuItem: return "Grouper par cette colonne";
					case RadGridStringId.GroupingPanelDefaultMessage: return "Ajouter ici une colonne pour faire un regroupement par cette colonne";
					case RadGridStringId.GroupingPanelHeader: return "Grouper par";
					case RadGridStringId.HideGroupMenuItem: return "Cacher le groupe";
					case RadGridStringId.HideMenuItem: return "Masquer cette colonne";
					case RadGridStringId.NoDataText: return "Pas de données à afficher";
					case RadGridStringId.PasteMenuItem: return "Coller";
					case RadGridStringId.PinAtBottomMenuItem: return "Épingler en bas";
					case RadGridStringId.PinAtLeftMenuItem: return "Épingler à gauche";
					case RadGridStringId.PinAtRightMenuItem: return "Épingler à droite";
					case RadGridStringId.PinAtTopMenuItem: return "Épingler en haut";
					case RadGridStringId.PinMenuItem: return "Epingler";
					case RadGridStringId.SortAscendingMenuItem: return "Trier (ordre croissant)";
					case RadGridStringId.SortDescendingMenuItem: return "Trier (ordre décroissant)";
					case RadGridStringId.UngroupThisColumn: return "Dégrouper cette colonne";
					case RadGridStringId.UnpinMenuItem: return "Masquer automatiquement";
					case RadGridStringId.UnpinRowMenuItem: return "Dépingler";

					default: return base.GetLocalizedString(id);
				}
			}
		}
	}
}