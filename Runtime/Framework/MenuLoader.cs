using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PhenomenalViborg.MUCOSDK
{
    public class MenuLoader : MonoBehaviour
    {
        [SerializeField] float m_Delay = 3.0f;
        void Start()
        {
            StartCoroutine(LoadMenu());
        }

        IEnumerator LoadMenu()
        {
            yield return new WaitForSeconds(m_Delay);
            MUCOApplication.GetApplicationManager().LoadMenu();
        }

    }
}
