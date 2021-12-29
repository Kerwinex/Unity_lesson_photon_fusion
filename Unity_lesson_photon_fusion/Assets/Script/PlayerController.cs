using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [Header("移動速度"), Range(0, 100)]
    public float speed = 7.5f;
    [Header("發射子彈間隔"), Range(0, 1.5f)]
    public float intervalFire = 0.35f;
    [Header("子彈物件")]
    public Bullet bullet;
    [Header("子彈生成位置")]
    public Transform pointFire;
    [Header("砲塔")]
    public Transform traTower;    

    public TickTimer interval { get; set; }

    private InputField inputMessage;
    private Text allMessage;
    private NetworkCharacterController ncc;

    private void Awake()
    {
        ncc = GetComponent<NetworkCharacterController>();
        inputMessage = GameObject.Find("聊天輸入區域").GetComponent<InputField>();
        inputMessage.onEndEdit.AddListener((string message) => { InputMessage(message); });

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("子彈")) Destroy(gameObject);
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
