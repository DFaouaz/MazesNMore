using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

// Boton que abre la app o pagina web correspondiente para publicar un post

namespace MazesAndMore
{
    public class URLButton : MonoBehaviour
    {
        const string twitterAddress = "http://twitter.com/intent/tweet";
        const string facebookAddress = "http://www.facebook.com/";

        private void ShareToTwitter(string text, string url,
                                 string related, string lang = "en")
        {
            Application.OpenURL(twitterAddress +
                                "?text=" + UnityWebRequest.EscapeURL(text) +
                                "&amp;url=" + UnityWebRequest.EscapeURL(url) +
                                "&amp;related=" + UnityWebRequest.EscapeURL(related) +
                                "&amp;lang=" + UnityWebRequest.EscapeURL(lang));
        }

        private void ShareToFacebook(string linkParameter, string descriptionParameter)
        {
            Application.OpenURL(facebookAddress +
                                 "/share.php?u=" + UnityWebRequest.EscapeURL(linkParameter) +
                                 "&quote=" + UnityWebRequest.EscapeURL(descriptionParameter));
        }

        public void Tweet()
        {
            ShareToTwitter("I'm playing Maze&More right now! It's an amazing game. Download now on the GooglePlay!", "", "", "en");
        }

        public void FacebookPost()
        {
            ShareToFacebook("https://play.google.com/store/apps/details?id=com.leodesol.games.classic.maze.labyrinth&hl=es&gl=US", "I'm playing Maze&More right now! It's an amazing game. Download now on the GooglePlay!");
        }
    }
}
