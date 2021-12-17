// Bibliotecas da Unity
using UnityEngine; // biblioteca da unity usado para acessar os componentes principais, como gameobjects e seus respectivos componentes
using UnityEngine.SceneManagement; // biblioteca do gerenciador de cenas da Unity
using System.IO; // biblioteca ultilizada para alterar e l� arquivos
using UnityEngine.UI; // biblioteca da Interface de Usuario da Unity

// Outras Bibliotecas
using System;
using System.Linq;
using System.Collections.Generic;

public class Menu_script : MonoBehaviour
{
    //variaveis principais
    public Text[] saves;

    public Dropdown ddpreso, ddpqua;
    public Toggle tgwin;

    // variaveis multuas, seus valores ir�o mudar constantemente
    private List<string> resolutions = new List<string>();
    private List<string> quality = new List<string>();

    // Acnontece uma vez, quando o game � iniciado
    void Start()
    {
        for(int i = 0; i < saves.Length; i++)
        {
            if (File.Exists(GameObject.Find("GameControll").GetComponent<Player_script>().file[i]))
                saves[i].text = "continuar";
            else
                saves[i].text = "------  New Save ------";
        }

        // encontra os objetos
        ddpreso = GameObject.Find("Dropdown_reso").GetComponent<Dropdown>();
        ddpqua = GameObject.Find("Dropdown_qua").GetComponent<Dropdown>();
        tgwin = GameObject.Find("fullScreen").GetComponent<Toggle>();

        Resolution[] arrResolution = Screen.resolutions; // Variavel da Unity, que recebe as resolu��es possiveis de rodar

        // ira procurar as resolu��es que o jogador podera selecionar,
        // a variavel "Screen.resolutions" possui mais dados do que os que o player seleciona, por isso � necessario fazer um filtro dessas resolu��es
        foreach (Resolution r in arrResolution)
        {
            resolutions.Add(string.Format("{0} X {1}", r.width, r.height)); // filtro de resolu��es
            // dentro da lista de resolu��es serao criadas varivaeis do tipo string contendo o nome das resolu��es, que apareceram na tela do jogador
        }

        ddpreso.AddOptions(resolutions); // adiciona as op��es ao Dropdown
        ddpreso.value = (resolutions.Count - 1); // define a quantidade de resolu��es (op��es do Dropdown)

        // coloca as possiveis qualidades dentro da lista
        quality = QualitySettings.names.ToList<string>();

        ddpqua.AddOptions(quality); // adiciona as op��es ao Dropdown
        ddpqua.value = QualitySettings.GetQualityLevel(); // define a quantidade de qualidades (op��es do Dropdown)

        GameObject.Find("configs").SetActive(false);
        GameObject.Find("Graphic").SetActive(false);
    }

    public void SetWindowsMode() // define a configura��o da janela
    {
        if (tgwin.isOn) Screen.fullScreen = false; 
        else Screen.fullScreen = true;
    }

    public void SetResolutions() // define a resolu��o
    {
        // ira l� a variavel de resolu��o, porem sera nescessario tirar o "X", pois � nescessario l� apenas o valor width e height cotidos na string

        string[] res = resolutions[ddpreso.value].Split('X'); // remove o X (que foi colocado so de "infeite") da string, e separa os dois numerais numa array 
        int w = Convert.ToInt16(res[0].Trim()); // width
        int h = Convert.ToInt16(res[1].Trim()); // height
        
        Screen.SetResolution(w, h, Screen.fullScreen); // muda a resolu��o
    }

    public void SetQuality() // define a qualidade
    {
        QualitySettings.SetQualityLevel(ddpqua.value, true); // a unity muda de qualidade, conforme o valor do Dropdown
    }

    public void GameOption(int save)
    {
        if (File.Exists(GameObject.Find("GameControll").GetComponent<Player_script>().file[save]))
            GameObject.Find("GameControll").GetComponent<Player_script>().Load(save);
        else
        {
            GameObject.Find("GameControll").GetComponent<Player_script>().theSave = save;
            GameObject.Find("GameControll").GetComponent<Player_script>().ChangeScene(1, save);
        }
        Debug.Log(save);
    }

    public void DeleteSaveGame(int save)
    {
        if (File.Exists(GameObject.Find("GameControll").GetComponent<Player_script>().file[save]))
            File.Delete(GameObject.Find("GameControll").GetComponent<Player_script>().file[save]);
        
        for (int i = 0; i < saves.Length; i++)
        {
            if (File.Exists(GameObject.Find("GameControll").GetComponent<Player_script>().file[i]))
                saves[i].text = "continuar";
            else
                saves[i].text = "------  New Save ------";
        }
    }

    public void ExitGame() // Sair do Jogo
    {
        Application.Quit();
    }

    public void loadAnimation(string animation)
    {
        this.gameObject.GetComponent<Animator>().Play(animation);
    }
}