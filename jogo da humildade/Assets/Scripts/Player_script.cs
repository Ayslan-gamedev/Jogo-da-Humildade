// Bibliotecas da Unity
using UnityEngine; // biblioteca da unity usado para acessar os componentes principais, como gameobjects e seus respectivos componentes
using UnityEngine.SceneManagement; // biblioteca do gerenciador de cenas da Unity
using UnityEngine.UI; // biblioteca da Interface de Usuario da Unity

// Outras Bibliotecas
using System.IO; // biblioteca ultilizada para alterar arquivos
using System.Xml.Serialization; // biblioteca ultilizada para serializar arquivos XML

// Classe, na qual ira herdar a classe MonoBehaviour(só deus sabe onde se encontra), que atribui os comandos da Unity, como transforms, vetores, e etc
// [SerializeField] torna a classe Serizalizavel
[SerializeField] public class Player_script : MonoBehaviour
{
    // -=-=-=-=-=-=-=--=-=-=-=-=-=-=- variaveis gerais -=-=-=-=-=-=-=--=-=-=-=-=-=-=-
    private GameObject player; // o jogador
    public float startspeed, speedMax, jumpForce, dashForce; // movimentação, pulo e dash

    // variaveis quanticas (sempre estarão em constante alteração)
    private float speed, timerDash /* tempo no dash */ , coyoteTimer, directX /* direção X do dash */, directY/* direção Y do dash */;
    private bool inL, inR, inDash, canMakeDash; //inL e inR verifica se o jogador olha para a direita ou esquerda
    private int jumps; // quantidades de pulos dados
    private string file, thatScene;
    // -=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-

    // Acontece antes da fase inicias 
    private void Awake() 
    {
        file = Application.persistentDataPath + "/saveData1.dat";

        // o objeto não sera distruido, e passara de uma cena para outra
        DontDestroyOnLoad(this.gameObject); 
    }

    // Acontece quando a fase inicia
    private void Start()
    {
        if (thatScene == null)
            thatScene = "NUM1";

        this.transform.position = new Vector3(0, 0, 0); // Coloca o objeto no cntro do mapa

        if(GameObject.Find("lifeBar").GetComponent<Slider>().value == 0) // se a vida não foi definida, logo é a primeira fase, portanto será dado os valores iniciais do game
        {
            GameObject.Find("lifeBar").GetComponent<Slider>().value = 50;
            GameObject.Find("lifeBar").GetComponent<Slider>().maxValue = 100;
        }

        player = GameObject.Find("player"); // procura o player
    }

    // Atualiza constantemente, independentemente do FPS
    private void FixedUpdate()
    {
        // Se o jogador for == null, logo o usuario esta em uma cena sem jogador (como o menu do game), por isso atribuise o "if (player != null)", para que não
        // der nenhum erro. isso é nescessario Tambem por que objeto carrega variaveis do Load.
        if (player != null)
        {
            // MOVIMENTAÇÃO DO JOGADOR

            // -=-=-=-=-=-=-=- MOV -=-=-=-=-=-=-=-
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Input.GetAxisRaw("Horizontal"), player.GetComponent<Rigidbody2D>().velocity.y); // movimenta o jogador

            if (inDash == false) // se o jogador estiver usando o dash, ele não pode alterar a sua direção
            {

                // os ifs estão separados, pois se estivesse no if abaixo, o else so aconteceria se ele estivesse no dash, oque causaria um erro na movimentação
                if (Input.GetAxisRaw("Horizontal") != 0) // se o jogador estiver movendo o analogico
                {
                    player.GetComponent<Animator>().Play("idle"); // inicia a animação de movimentação
                                                                  // gira o personagem 
                    Flip();
                    if (Input.GetAxisRaw("Horizontal") > 0) inL = false;
                    else inL = true;

                    // aumenta a velocidade atual ate seu limite 'speedMax'
                    if (speed < 0.50f) speed = startspeed;
                    if (speed <= speedMax) speed += 0.25F;
                }
                else
                {
                    player.GetComponent<Animator>().Play("idle"); // inicia a animação de parado

                    // diminui a velocidade atual gradativamente ate 0 conforme a velocidade (nesse caso é 2)
                    if (speed > 0) speed -= 2;
                    if (speed < 0) speed = 0;
                }
            }

            // -=-=-=-=-=-=-=- DASH -=-=-=-=-=-=-=-
            if (Input.GetButton("Fire1") && inDash == false && canMakeDash == true)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                    directX = 1;
                else if (Input.GetAxisRaw("Horizontal") < 0)
                    directX = -1;
                else
                    directX = 0;

                if (Input.GetAxisRaw("Vertical") > 0)
                    directY = 1;
                else if (Input.GetAxisRaw("Vertical") < 0)
                    directY = -1;
                else
                    directY = 0;

                if (directY != 0 || directX != 0)
                    inDash = true;
            }

            if (inDash == true)
            {
                player.GetComponent<Animator>().Play("dash");
                player.GetComponent<Rigidbody2D>().gravityScale = 0;
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(dashForce * directX, dashForce * directY);
                timerDash += Time.deltaTime;
                speed = 0;

                if (timerDash >= 0.4f)
                {
                    timerDash = 0;
                    inDash = false;
                    canMakeDash = false;
                }
            }
            else
            {
                player.GetComponent<Rigidbody2D>().gravityScale = 10;

                directX = 0;
                directY = 0;
            }

            // -=-=-=-=-=-=-=- PULAR -=-=-=-=-=-=-=-
            if (InGround() != true)
                coyoteTimer += Time.deltaTime;
            else
                coyoteTimer = 0;

            if (Input.GetButton("Fire3") && (InGround() == true || coyoteTimer <= 0.2f && jumps == 0)) // Pular
            {
                player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                jumps++;
            }
            // -=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-
        }
    }

    private bool InGround() // verifica se o player esta tocando no chão
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(player.transform.position.x, player.transform.position.y - 1.5f, player.transform.position.z), Vector3.down, 0.2f, 1 << 8);
        if (hit.collider != null)
        {
            jumps = 0;
            canMakeDash = true;
            return true;
        }

        return false;
    }

    private void Flip() // gira o Player
    {
        if ((inL && !inR) || (!inL && inR))
        {
            inR = !inR;
            player.transform.localScale = new Vector2(player.transform.localScale.x * -1, transform.localScale.y);
        }
    }

    public void ChangeLife(float newLife, float newMax) // mudar barra de vida
    {
        GameObject.Find("lifeBar").GetComponent<Slider>().value += newLife;
        GameObject.Find("lifeBar").GetComponent<Slider>().maxValue += newMax;
    }

    public void ChangeScene(string newScene) // muda cena atual
    {
        SceneManager.LoadScene(newScene);
        thatScene = newScene;
    }

    // =-=-=-=-=-=-=-=-= Apartir daqui o script não é só de mecanica basica, e passa a lidar com sistemas de salvamento, Serialização, entre outros recursos. =-=-=-=-=-=-=-=-=

    // ====================== save and Load System ======================

    public void Save()
    {
        if (File.Exists(file))
            File.Delete(file);
        else
            File.Create(file);

        XmlSerializer x = new XmlSerializer(typeof(SaveData));
        StreamWriter writer = new StreamWriter(file, true);

        SaveData sd = new SaveData();
        sd.posx = player.transform.position.x;
        sd.posy = player.transform.position.y;

        sd.atuallife = GameObject.Find("lifeBar").GetComponent<Slider>().value;
        sd.MaxLife = GameObject.Find("lifeBar").GetComponent<Slider>().maxValue;

        sd.cena = thatScene;

        x.Serialize(writer, sd);
        writer.Close();
    }

    private void Load()
    {
        XmlSerializer x = new XmlSerializer(typeof(SaveData));
        StreamReader reader = new StreamReader(file);

        SaveData sd = (SaveData)x.Deserialize(reader);
        player.transform.position = new Vector2(sd.PosX, sd.PosY);

        GameObject.Find("lifeBar").GetComponent<Slider>().value = sd.Atuallife;
        GameObject.Find("lifeBar").GetComponent<Slider>().maxValue = sd.MaxLife;

        if (thatScene != sd.cena)
            ChangeScene(sd.cena);
        else
            thatScene = sd.Cena;

        reader.Close();
    }
}
[XmlRoot("gameData")]

public class SaveData : Player_script 
{
    // Possição
    public float posx;
    public float posy;

    [XmlElement("PosX",typeof(float))]
    public float PosX
    {
        get { return this.posx; }
        set { this.posx = value; }
    }

    [XmlElement("PosY", typeof(float))]
    public float PosY
    {
        get { return this.posy; }
        set { this.posy = value; }
    }

    // Vida
    public float atuallife;
    public float maxLife;

    [XmlElement("Atuallife", typeof(float))]
    public float Atuallife
    {
        get { return this.atuallife; }
        set { this.atuallife = value; }
    }

    [XmlElement("MaxLife", typeof(float))]
    public float MaxLife
    {
        get { return this.maxLife; }
        set { this.maxLife = value; }
    }

    // Cena
    public string cena;

    [XmlElement("Cena",typeof(string))]
    public string Cena 
    {
        get { return this.cena; }
        set { this.cena = value; }
    }

}

// Made By: Auslan Vieira Fontes
// Nickname: PressStart1390