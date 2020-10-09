using UnityEngine;
using TMPro;

namespace Lean.Localization
{
	/// <summary>This component will update a TextMeshPro component's Font with a localized font, or use a fallback if none is found.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshPro))]
	[AddComponentMenu(LeanLocalization.ComponentPathPrefix + "Localized TextMeshPro Font")]
	public class LeanLocalizedTextMeshProFont : LeanLocalizedBehaviour
	{
		[Tooltip("If PhraseName couldn't be found, this font asset will be used")]
		public TMP_FontAsset FallbackFont;

		// This gets called every time the translation needs updating
		public override void UpdateTranslation(LeanTranslation translation)
		{
			// Get the TextMeshPro component attached to this GameObject
			var text = GetComponent<TextMeshPro>();

			// Use translation?
			if (translation != null && translation.Data is TMP_FontAsset)
			{
				text.font = (TMP_FontAsset)translation.Data;
			}
			// Use fallback?
			else
			{
				text.font = FallbackFont;
			}
		}

		protected virtual void Awake()
		{
			// Should we set FallbackFont?
			if (FallbackFont == null)
			{
				// Get the TextMeshPro component attached to this GameObject
				var text = GetComponent<TextMeshPro>();

				// Copy current text to fallback
				FallbackFont = text.font;
			}
		}
	}
}