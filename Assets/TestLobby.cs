using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;

public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartBeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;
    private const string gameMode = "Multi FPS";
    [SerializeField] TMP_InputField LobbyCodeInput;
    [SerializeField] TextMeshProUGUI Output;
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => { Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId); };
        WriteOutput("Signed In" + AuthenticationService.Instance.PlayerId);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerName = "Sam " + UnityEngine.Random.Range(10, 99);
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
        GetLobbies();
    }

    private async void GetLobbies()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;

            if (lobbyUpdateTimer < 0f)
            {
                float updateTimerMax = 1.5f;
                lobbyUpdateTimer = updateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }

    void WriteOutput(string output)
    {
        Output.text += output+"\n";
    }

    private async void HandleLobbyHeartBeat()
    {
        if(hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                float heartBeatTimerMax = 15f;
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async void CreateLobby()
    {
        try
        {
            string LobbyName = "FPSLobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                { { "GameMode", new DataObject(DataObject.VisibilityOptions.Public,gameMode)}}

            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(LobbyName, maxPlayers, options);
            hostLobby = lobby;
            joinedLobby = lobby;


            string output = "ID " + lobby.Id + " , " + lobby.Name + " has been created with code : " + lobby.LobbyCode;
            Debug.Log(output);
            WriteOutput(output);
            PrintPlayers(hostLobby);
        } 
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Count = 25,
                Filters = new List<QueryFilter> { new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0", QueryFilter.OpOptions.GT)},
                Order = new List<QueryOrder> { new QueryOrder(false, QueryOrder.FieldOptions.Created)}
            };

            QueryResponse queryResponde = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log("Nb of lobbies : "+queryResponde.Results.Count);
            foreach (var result in queryResponde.Results)
            {
                Debug.Log("Lobby name " + result.Name + " Game Mode : " + result.Data["GameMode"].Value);
                WriteOutput("Lobby name " + result.Name + " Game Mode : " + result.Data["GameMode"].Value);
            }
            
        }
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void JoinLobbyWithCode()
    {
        string lobbyCode = LobbyCodeInput.text;
        JoinLobbyByCode(lobbyCode);
    }
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

           Lobby Lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
           joinedLobby = Lobby;
           string output = "joined lobby with code " + lobbyCode +", Player inside the lobby "+ Lobby;

           Debug.Log("joined lobby with code " + lobbyCode);
           WriteOutput("joined lobby with code " + lobbyCode);

          // WriteOutput(output);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
        }

    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
        }
        
    }

    public void TestButton()
    {
        UpdateLobbyGameMode("New game Mode");
    }

    public void UpdateName()
    {
        UpdatePlayerName("coucou");
    }

    public async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
             }
        };
    }

    public void ClearConsole()
    {
        Output.text = "";
    }
    public void PrintPlayers()
    {
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Lobby " + lobby.Name  + " Player mode " + lobby.Data["GameMode"].Value);
        Debug.Log("My Player name : " + playerName);
        int nb = 0;
        foreach (Player player in lobby.Players)
        {
          nb++;
          Debug.Log(player.Data["PlayerName"].Value);
          WriteOutput("Player name "+player.Data["PlayerName"].Value +" Nb : "+nb);
        }
    }


    private async void UpdateLobbyGameMode(string gameMode)
    {
        try
        {
          hostLobby = await  Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {

                Data = new Dictionary<string, DataObject>
                { 
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) } }
                }
            );

            joinedLobby = hostLobby;

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void UpdatePlayerName(string newPlayerName)
    {

        try
        {
            playerName = newPlayerName;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId,

                new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                { { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }  }
                }

                );
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
      
    }

    async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            }
            );

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}
