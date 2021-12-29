using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("創建與加入房間欄位")]
    public InputField InputFieldCreateRoom;
    public InputField InputFieldJoinRoom;
    [Header("玩家控制物件")]
    public NetworkPrefabRef goPlayer;
    [Header("畫布連線")]
    public GameObject goCanvas;
    [Header("版本文字")]
    public Text textVersion;
    [Header("玩家生成位置")]
    public Transform[] spwanPoints;

    private string roomNameInput;
    private NetworkRunner runner;
    private string stringVersion = "Version ";
    private Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();

    private void Awake()
    {
        textVersion.text = stringVersion + Application.version;
    }

    public void BtCreate()
    {
        roomNameInput = InputFieldCreateRoom.text;
        print("創建房間：" + roomNameInput);
        StartGame(GameMode.Host);
    }

    public void BtJoin()
    {
        roomNameInput = InputFieldJoinRoom.text;
        print("加入房間：" + roomNameInput);
        StartGame(GameMode.Client);
    }

    private async void StartGame(GameMode mode)
    {
        print("<color=yellow>開始連線</color>");

        runner = gameObject.AddComponent<NetworkRunner>(); //添加元件:連線執行器
        runner.ProvideInput = true;                        //連線執行器.是否提供輸入 = 是

        //等待連線：遊戲連線模式、房間名稱、連線後的場景、場景管理器
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomNameInput,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        }) ;
        print("<color=yellow>連線完成</color>");
        goCanvas.SetActive(false);
    }

    #region fusion 回乎函式區
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    //玩家連線輸入行為
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData inputData = new NetworkInputData();
        if (Input.GetKey(KeyCode.W)) inputData.direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) inputData.direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) inputData.direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) inputData.direction += Vector3.right;

        inputData.inputFire = Input.GetKey(KeyCode.Mouse0);

        inputData.positionMouse = Input.mousePosition;
        inputData.positionMouse.z = 60;
        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(inputData.positionMouse);
        inputData.positionMouse = mouseToWorld;

        input.Set(inputData);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        int randomSpwanPoint = UnityEngine.Random.Range(0, spwanPoints.Length);
        NetworkObject playerNetworkObject = runner.Spawn(goPlayer,spwanPoints[randomSpwanPoint].position, Quaternion.identity, player);
        players.Add(player, playerNetworkObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (players.TryGetValue(player, out NetworkObject playerNetworkObject)) {
            runner.Despawn(playerNetworkObject);
            players.Remove(player);
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion

}
