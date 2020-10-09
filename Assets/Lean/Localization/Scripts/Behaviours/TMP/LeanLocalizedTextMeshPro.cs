using UnityEngine;
using TMPro;

namespace Lean.Localization
{
	/// <summary>This component will update a TMPro.TextMeshPro component with localized text, or use a fallback if none is found.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshPro))]
	[AddComponentMenu(LeanLocalization.ComponentPathPrefix + "Localized TextMeshPro")]
	public class LeanLocalizedTextMeshPro : LeanLocalizedBehaviour
	{
		[Tooltip("If PhraseName couldn't be found, this text will be used")]
		public string FallbackText;

		// This gets called every time the translation needs updating
		public override void UpdateTranslation(LeanTranslation translation)
		{
			// Get the TextMeshPro component attached to this GameObject
			var text = GetComponent<TextMeshPro>();

			// Use translation?
			if (translation != null && translation.Data is string)
			{
				text.text = LeanTranslation.FormatText((string)translation.Data, text.text, this);
			}
			// Use fallback?
			else
			{
				text.text = LeanTranslation.FormatText(FallbackText, text.text, this);
			}
		}

		protected virtual void Awake()
		{
			// Should we set FallbackText?
			if (string.IsNullOrEmpty(FallbackText) == true)
			{
				// Get the TextMeshPro component attached to this GameObject
				var text = GetComponent<TextMeshPro>();

				// Copy current text to fallback
				FallbackText = text.text;
			}
		}
	}
}