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
    public string[] file = new string[3]; // arquivo de save

    // variaveis quanticas (sempre estarão em constante alteração)
    private float speed, timerDash /* tempo no dash */ , coyoteTimer, directX /* direção X do dash */, directY/* direção Y do dash */, life, maxLife;
    private bool inL, inR, inDash, canMakeDash, loaded; //inL e inR verifica se o jogador olha para a direita ou esquerda
    public Vector2 posLoaded; // posição do jogador que foi serealizada
    private int jumps, thatScene; // quantidades de pulos dados

    public int theSave;
    // -=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-

    // Acontece antes da fase inicias 
    private void Awake() 
    {
        // define onde será salvo o arquivo do game, sendo "Application.persistentDataPath" definido automaticamente pela Unity
        // A Application.persistentDataPath fica em AppData/ nome do game /arquivo dat
        file[0] = Application.persistentDataPath + "/saveData1.dat";
        file[1] = Application.persistentDataPath + "/saveData2.dat";
        file[2] = Application.persistentDataPath + "/saveData3.dat";

        // o objeto não sera distruido, e passara de uma cena para outra
        DontDestroyOnLoad(this.gameObject);
    }

    // Acontece quando a fase inicia
    private void Start()
    {
        this.transform.position = new Vector3(0, 0, 0); // Coloca o objeto no cntro do mapa

        if(life == 0)
        {
            life = 50;
            maxLife = 100;
        }
    }

    // Atualiza constantemente, independentemente do FPS
    private void FixedUpdate()
    {
        if(player == null)
            player = GameObject.Find("player"); // procura o player

        if(loaded == true && player != null) // carrega dados ao trocar de cena
        {
            XmlSerializer x = new XmlSerializer(typeof(SaveData)); // define qual classe tera suas variaveis serializada
            StreamReader reader = new StreamReader(file[theSave]); // lê o arquivo

            SaveData sd = (SaveData)x.Deserialize(reader); // Deserialisa o arquivo de save na classe de dados

            // posição do player
            posLoaded = new Vector2(sd.PosX, sd.PosY);
            player.transform.position = posLoaded;

            if (GameObject.Find("lifeBar").GetComponent<Slider>() != null)
            {
                //vida
                life = sd.Atuallife;
                maxLife = sd.MaxLife;
            }

            reader.Close(); // fecha o arquivo
            loaded = false;
        }

        if (GameObject.Find("lifeBar") != null) // carrega a barra de vida
        {
            GameObject.Find("lifeBar").GetComponent<Slider>().value = life;
            GameObject.Find("lifeBar").GetComponent<Slider>().maxValue = maxLife;
        }

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

            // Coyote time, quando o jogador sai da plataforma. ele terá uma "segunda chance" para pular
            if (InGround() != true) coyoteTimer += Time.deltaTime;
            else coyoteTimer = 0;

            // -=-=-=-=-=-=-=- DASH -=-=-=-=-=-=-=-
            if (Input.GetButton("Fire1") && inDash == false && canMakeDash == true) // botão de dash
            {
                // define a direção do Dash em X
                if (Input.GetAxisRaw("Horizontal") > 0) directX = 1;
                else if (Input.GetAxisRaw("Horizontal") < 0) directX = -1;
                else directX = 0;

                // define a direção do Dash em Y
                if (Input.GetAxisRaw("Vertical") > 0) directY = 1;
                else if (Input.GetAxisRaw("Vertical") < 0) directY = -1;
                else directY = 0;

                // Inicia o Dash
                if (directY != 0 || directX != 0) inDash = true;
            }

            if (inDash == true) // Quando está no dash
            {
                player.GetComponent<Animator>().Play("dash"); // Animação de dash
                player.GetComponent<Rigidbody2D>().gravityScale = 0; // muda a gravidade para 0
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(dashForce * directX, dashForce * directY); // O dash
                speed = 0;

                // timer para acabar o dash
                timerDash += Time.deltaTime;
                if (timerDash >= 0.4f)
                {
                    timerDash = 0;
                    inDash = false;
                    canMakeDash = false;
                }
            }
            else // Acabou o dash
            {
                player.GetComponent<Rigidbody2D>().gravityScale = 10; // volta a gravidade pro normal

                // zerar posições do dash
                directX = 0;
                directY = 0;
            }

            // -=-=-=-=-=-=-=- PULAR -=-=-=-=-=-=-=-
            if (Input.GetButton("Fire3") && (InGround() == true || coyoteTimer <= 0.2f && jumps == 0)) // botão pular
            {
                player.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // pula
                jumps++; // Aumenta a quantidade de pulos
            }

            // -=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Save(theSave);
            Save(theSave);
            Save(theSave);
        }

        Debug.Log(thatScene);
    }

    private bool InGround() // verifica se o player esta tocando no chão
    {
        // o RayCast sera lançado na posição do player para baixo, e ira ignorar todas as layers menos a 8, que é layer de plataforma
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(player.transform.position.x, player.transform.position.y - 1.5f, player.transform.position.z), Vector3.down, 0.2f, 1 << 8);
        
        if (hit.collider != null) // se colidiu na plataforma 
        {
            jumps = 0; // zera a quantidade de pulos dados
            canMakeDash = true; // só pode dá dash denovo se tocar no chão
            return true;
        }
        return false;
    }

    private void Flip() // gira o Player
    {
        if ((inL && !inR) || (!inL && inR)) // se ele trocou a direção
        {
            inR = !inR; // faz com que não gire infinitamente
            player.transform.localScale = new Vector2(player.transform.localScale.x * -1, player.transform.localScale.y); // troca direção
        }
    }

    public void ChangeLife(float newLife, float newMax) // mudar barra de vida
    {
        GameObject.Find("lifeBar").GetComponent<Slider>().value += newLife; // vida atual
        GameObject.Find("lifeBar").GetComponent<Slider>().maxValue += newMax; // vida maxima
    }

    public void ChangeScene(int newScene, int save) // muda cena atual
    {
        theSave = save;
        thatScene = newScene;
        SceneManager.LoadSceneAsync(newScene);
        Start();
    }

    // ====================== save and Load System ======================

    public void Save(int save)
    {
        theSave = save; // define qual save

        // verifica se o arquivo de Save já existe
        if (File.Exists(file[theSave])) File.Delete(file[theSave]); // o arquivo sera apagado para da lugar ao "novo", na visao do usuario ele foi apenas reescrito
        else File.Create(file[theSave]); // criar novo arquivo de save

        XmlSerializer x = new XmlSerializer(typeof(SaveData)); // define qual classe tera suas variaveis serializada
        StreamWriter writer = new StreamWriter(file[theSave], true); // escritor de arquivo

        SaveData sd = new SaveData(); // tras as variaveis
        if(GameObject.Find("player") != null)
        {
            // posição do player
            sd.posx = player.transform.position.x;
            sd.posy = player.transform.position.y;
        }

        if(GameObject.Find("lifeBar").GetComponent<Slider>() != null)
        {
            // life
            sd.atuallife = GameObject.Find("lifeBar").GetComponent<Slider>().value;
            sd.MaxLife = GameObject.Find("lifeBar").GetComponent<Slider>().maxValue;
        }
        
        // cena
        sd.cena = thatScene;

        x.Serialize(writer, sd); // escreve o arquivo 
        writer.Close(); // fecha o arquivo
    }

    public void Load(int save)
    {
        theSave = save; // define qual save

        XmlSerializer x = new XmlSerializer(typeof(SaveData)); // define qual classe tera suas variaveis serializada
        StreamReader reader = new StreamReader(file[theSave]); // lê o arquivo

        SaveData sd = (SaveData)x.Deserialize(reader); // Deserialisa o arquivo de save na classe de dados

        // muda a cena se estiver numa diferente
        if (thatScene != sd.cena) ChangeScene(sd.cena, theSave);
        else thatScene = sd.Cena;

        reader.Close(); // fecha o arquivo

        loaded = true;
        Start();
    }
}

// Classe de dados a serem serializadas, na qual herdara do Player_Script
// o xmlRoot determina qual será o arquivo raiz do xml
[XmlRoot("gameData")] public class SaveData 
{
    // Possição
    public float posx;
    public float posy;
    [XmlElement("PosX", typeof(float))]
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
    public int cena;

    [XmlElement("Cena", typeof(int))]
    public int Cena 
    {
        get { return this.cena; }
        set { this.cena = value; }
    }
}

// Made By: Auslan Vieira Fontes
// Nickname: PressStart1390