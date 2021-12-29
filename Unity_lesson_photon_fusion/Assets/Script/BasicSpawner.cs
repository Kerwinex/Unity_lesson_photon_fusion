using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("�ЫػP�[�J�ж����")]
    public InputField InputFieldCreateRoom;
    public InputField InputFieldJoinRoom;
    [Header("���a�����")]
    public NetworkPrefabRef goPlayer;
    [Header("�e���s�u")]
    public GameObject goCanvas;
    [Header("������r")]
    public Text textVersion;
    [Header("���a�ͦ���m")]
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
        print("�Ыةж��G" + roomNameInput);
        StartGame(GameMode.Host);
    }

    public void BtJoin()
    {
        roomNameInput = InputFieldJoinRoom.text;
        print("�[�J�ж��G" + roomNameInput);
        StartGame(GameMode.Client);
    }

    private async void StartGame(GameMode mode)
    {
        print("<color=yellow>�}�l�s�u</color>");

        runner = gameObject.AddComponent<NetworkRunner>(); //�K�[����:�s�u���澹
        runner.ProvideInput = true;                        //�s�u���澹.�O�_���ѿ�J = �O

        //���ݳs�u�G�C���s�u�Ҧ��B�ж��W�١B�s�u�᪺�����B�����޲z��
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomNameInput,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        }) ;
        print("<color=yellow>�s�u����</color>");
        goCanvas.SetActive(false);
    }

    #region fusion �^�G�禡��
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

    //���a�s�u��J�欰
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
