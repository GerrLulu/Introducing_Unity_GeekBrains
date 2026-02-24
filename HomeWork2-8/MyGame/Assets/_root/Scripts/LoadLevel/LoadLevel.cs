using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LoadLevel : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
                SceneManager.LoadScene(2);
        }
    }
}