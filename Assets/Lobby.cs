using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Lobby : NetworkManager
{

    const int maxPlayers = 2;
    Player[] playerSlots = new Player[2];


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Lobby::OnServerAddPlayer::playerControllerId " + playerControllerId);
        // find empty player slot
        for (int slot = 0; slot < maxPlayers ; slot++)
        {
            if (playerSlots[slot] == null)
            {
                var playerObj = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                var player = playerObj.GetComponent<Player>();

                player.playerId = slot;
                playerSlots[slot] = player;

                

                Debug.Log("Adding player in slot " + slot);
                NetworkServer.AddPlayerForConnection(conn, playerObj, playerControllerId);
                return;
            }
        }

        //TODO: graceful  disconnect
        conn.Disconnect();
    }

    /*public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController)
    {
        // remove players from slots
        var player = playerController.gameObject.GetComponent<Player>();
        playerSlots[player.playerId] = null;
        CardManager.RemovePlayer(player);

        base.OnServerRemovePlayer(conn, playerController);
    }*/

    /*public override void OnServerDisconnect(NetworkConnection conn)
    {
        foreach (var playerController in conn.playerControllers)
        {
            var player = playerController.gameObject.GetComponent<Player>();
            playerSlots[player.playerId] = null;
            CardManager.RemovePlayer(player);
        }

        base.OnServerDisconnect(conn);
    }*/

    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("Lobby::OnStartClient ");
        client.RegisterHandler(Card.CardMsgId, OnCardMsg);
    }

    void OnCardMsg(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<Card.CardMessage>();

        var other = ClientScene.FindLocalObject(msg.playerId);
        var player = other.GetComponent<Player>();
        player.MsgAddCard(msg.cardId);
    }

}
