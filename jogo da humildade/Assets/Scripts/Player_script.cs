// Bibliotecas da Unity
using UnityEngine; // biblioteca da unity usado para acessar os componentes principais, como gameobjects e seus respectivos componentes
using UnityEngine.SceneManagement; // biblioteca do gerenciador de cenas da Unity
using UnityEngine.UI; // biblioteca a Interface de Usuario da Unity

// Outras Bibliotecas
using System.Collections;
using System.Collections.Generic;

// Classe, na qual ira herdar a classe MonoBehaviour(s� deus sabe onde se encontra), que atribui os comandos da Unity, como transforms, vetores, e etc
public class Player_script : MonoBehaviour
{
    // -=-=-=-=-=-=-=--=-=-=-=-=-=-=- variaveis gerais -=-=-=-=-=-=-=--=-=-=-=-=-=-=-
    private GameObject player; // o jogador
    public float startspeed, speedMax, jumpForce, dashForce; // movimenta��o, pulo e dash

    // variaveis quanticas (sempre estar�o em constante altera��o)
    private float speed, timerDash /* tempo no dash */ , coyoteTimer, directX /* dire��o X do dash */, directY/* dire��o Y do dash */, life, maxLife;
    private bool inL, inR, inDash, canMakeDash; //inL e inR verifica se o jogador olha para a direita ou esquerda
    private int jumps; // quantidades de pulos dados
    // -=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=--=-=-=-=-=-=-=-=-=-

    // Acontece antes da fase inicias 
    private void Awake() 
    {
        // o objeto n�o sera distruido, e passara de uma cena para outra
        DontDestroyOnLoad(this.gameObject); 
    }

    // Acontece quando a fase inicia
    private void Start()
    {
        this.transform.position = new Vector3(0, 0, 0); // Coloca o objeto no cntro do mapa

        if(life == 0) // se a vida n�o foi definida, logo � a primeira fase, portanto ser� dado os valores iniciais do game
        {
            life = 50;
            maxLife = 100;
        }

        // Atribui os status de vida
        GameObject.Find("lifeBar").GetComponent<Slider>().value = life;
        GameObject.Find("lifeBar").GetComponent<Slider>().maxValue = maxLife;

        player = GameObject.Find("player"); // procura o player
    }

    // Atualiza constantemente, independentemente do FPS
    private void FixedUpdate()
    {
        // Se o jogador for == null, logo o usuario esta em uma cena sem jogador (como o menu do game), por isso atribuise o "if (player != null)", para que n�o
        // der nenhum erro. isso � nescessario Tambem por que objeto carrega variaveis do Load.
        if (player != null)
        {
            // MOVIMENTA��O DO JOGADOR

            // -=-=-=-=-=-=-=- MOV -=-=-=-=-=-=-=-
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Input.GetAxisRaw("Horizontal"), player.GetComponent<Rigidbody2D>().velocity.y); // movimenta o jogador

            if (inDash == false) // se o jogador estiver usando o dash, ele n�o pode alterar a sua dire��o
            {

                // os ifs est�o separados, pois se estivesse no if abaixo, o else so aconteceria se ele estivesse no dash, oque causaria um erro na movimenta��o
                if (Input.GetAxisRaw("Horizontal") != 0) // se o jogador estiver movendo o analogico
                {
                    player.GetComponent<Animator>().Play("idle"); // inicia a anima��o de movimenta��o
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
                    player.GetComponent<Animator>().Play("idle"); // inicia a anima��o de parado

                    // diminui a velocidade atual gradativamente ate 0 conforme a velocidade (nesse caso � 2)
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

    private bool InGround() // verifica se o player esta tocando no ch�o
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
    }

    // =-=-=-=-=-=-=-=-=-= Apartir daqui o script n�o � mecanica basica, e passa a lidar com sistemas de salvamento, Serializa��o, entre outros recursos. =-=-=-=-=-=-=-=-=-=

    // ====================== save and Load System ======================

    public void Save()
    {

    }

    private void Load()
    {

    }
}

public class SaveData : Player_script 
{
    public float posx, posy;
}

// Made By: Auslan Vieira Fontes
// Nickname: PressStart1390