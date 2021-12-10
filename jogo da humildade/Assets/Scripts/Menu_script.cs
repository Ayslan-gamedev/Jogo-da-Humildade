using UnityEngine; // biblioteca da unity usado para acessar os componentes principais, como gameobjects e seus respectivos componentes
using UnityEngine.SceneManagement; // biblioteca do gerenciador de cenas da Unity
using System.IO; // biblioteca ultilizada para alterar e lê arquivos

public class Menu_script : MonoBehaviour
{

    public void GameOption(int save)
    {
        if (File.Exists(GameObject.Find("GameControll").GetComponent<Player_script>().file[save]))
            GameObject.Find("GameControll").GetComponent<Player_script>().Load(save);
        else
        {
            GameObject.Find("GameControll").GetComponent<Player_script>().theSave = save;
            SceneManager.LoadScene("New Scene 1");
        }
    }

    public void loadAnimation(string animation)
    {
        this.gameObject.GetComponent<Animator>().Play(animation);
    }
}