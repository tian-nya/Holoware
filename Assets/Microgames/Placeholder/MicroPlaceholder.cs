namespace Micro.Placeholder
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MicroPlaceholder : Microgame
    {
        public GameObject button;

        public void Game()
        {
            AddAvatar(0);
            StartCoroutine(GameCoroutine());
        }

        IEnumerator GameCoroutine()
        {
            while (timer > 0)
            {
                if (cleared)
                {
                    button.GetComponent<SpriteRenderer>().color = Color.green;
                    button.GetComponent<SFXManager>().PlaySFX(0);
                    avatars[0].SetExpression(1);
                    break;
                }
                yield return null;
            }
        }
    }
}