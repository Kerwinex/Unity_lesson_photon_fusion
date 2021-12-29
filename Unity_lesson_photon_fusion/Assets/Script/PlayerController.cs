using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [Header("���ʳt��"), Range(0, 100)]
    public float speed = 7.5f;
    [Header("�o�g�l�u���j"), Range(0, 1.5f)]
    public float intervalFire = 0.35f;
    [Header("�l�u����")]
    public Bullet bullet;
    [Header("�l�u�ͦ���m")]
    public Transform pointFire;
    [Header("����")]
    public Transform traTower;    

    public TickTimer interval { get; set; }

    private InputField inputMessage;
    private Text allMessage;
    private NetworkCharacterController ncc;

    private void Awake()
    {
        ncc = GetComponent<NetworkCharacterController>();
        inputMessage = GameObject.Find("��ѿ�J�ϰ�").GetComponent<InputField>();
        inputMessage.onEndEdit.AddListener((string message) => { InputMessage(message); });

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("�l�u")) Destroy(gameObject);
    }

    public override void FixedUpdateNetwork()
    {
        Move();
        Fire();
    }

    private void Move()
    {
        if(GetInput(out NetworkInputData dataInput)) {
            ncc.Move(speed * dataInput.direction * Runner.DeltaTime);
            Vector3 positionMouse = dataInput.positionMouse;
            positionMouse.y = traTower.position.y;
            traTower.forward = positionMouse - transform.position;
        }
    }

    private void Fire()
    {
        if(GetInput(out NetworkInputData dataInput)) {
            if (interval.ExpiredOrNotRunning(Runner)){
                if (dataInput.inputFire) {
                    interval = TickTimer.CreateFromSeconds(Runner, intervalFire);
                    Runner.Spawn(
                        bullet,
                        pointFire.position,
                        pointFire.rotation,
                        Object.InputAuthority,
                        (runner, objectSpwan) =>
                        {
                            objectSpwan.GetComponent<Bullet>().Init();
                        });
                }
            }
        }
    }

    private void InputMessage(string message)
    {
        if (Object.HasInputAuthority) {
            RPC_SendMessage(message);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_SendMessage(string message, RpcInfo info = default)
    {
        allMessage.text += (message + "\n");
    }
}
