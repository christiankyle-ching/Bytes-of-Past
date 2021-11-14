using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberLinkOpener : MonoBehaviour
{
    #region Ching

    string ching_github = "https://github.com/christiankyle-ching/";
    string ching_website = "https://christiankyleching.vercel.app/";

    public void ChingGithub()
    {
        Application.OpenURL(ching_github);
    }

    public void ChingWebsite()
    {
        Application.OpenURL(ching_website);
    }

    #endregion

}
