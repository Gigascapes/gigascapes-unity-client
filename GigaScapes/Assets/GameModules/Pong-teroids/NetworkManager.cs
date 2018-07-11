using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;

public class NetworkManager : MonoBehaviour
{
    private string brokerHost = "m10.cloudmqtt.com";
    private string username = "unity-astroids";
    private string password = "unity-09876";
    private string clientId;
    private string topicPrefix = "gigascapes";
    private int brokerPort = 12173;

    private float sendFrequency = 0.1f; //seconds (this is also the update player2 frequency)

    public GameObject player1;
    public GameObject player2;//dummy network player2 in same space as player1

    private MqttClient client;
    private MWrapper myMessages;

    private int numberOfMessagesToStore = 10;//change as needed, more cost performance and memory, 3 may be enough
    //array of messages, indexed by id ( linear search to find of size numberOfMessagesToStore)
    private Dictionary<int, MqttMessage> messageDictionary;
    private int[] messageHistory;
    private int messageHistoryPosition = 0;
    private int messageId = 0;  //message count, we need to set this from server when it is hooked up (will remove this variable)

    // Use this for initialization
    void Start()
    {
        clientId = Guid.NewGuid().ToString();

        // create client instance
        client = new MqttClient(brokerHost, brokerPort, false, null);

        // register for disconnected event 
        client.MqttMsgDisconnected += client_MqttMsgDisconnected;

        // register for message received event 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        client.Connect(clientId, username, password);

        // subscribe to the topic "/home/temperature" with QoS 2 
        client.Subscribe(new string[] { AllPositionsTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        messageDictionary = new Dictionary<int, MqttMessage>();
        messageHistory = new int[numberOfMessagesToStore]; //these are keys to dictionary
        for (int x = 0; x < numberOfMessagesToStore; x++)
        {
            messageHistory[x] = Int32.MaxValue;//initialize values to max
        }
        StartCoroutine("SendMessage");//this starts the message loop with frequency sendFrequency seconds.
    }
    public string AllPositionsTopic
    {
        get { return topicPrefix + "/+/positions-ts"; }
    }
    public string SelfPositionsTopic
    {
        get { return topicPrefix + "/" + clientId + "/positions"; }
    }

    void client_MqttMsgDisconnected(object self, System.EventArgs e)
    {
        Debug.Log("Disconnected");
    }
    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string response = System.Text.Encoding.UTF8.GetString(e.Message);
        Debug.Log("Received: " + response);
        myMessages = JsonUtility.FromJson<MWrapper>(response);
        if (myMessages.clientId != clientId)
        {
            foreach (MqttMessage position in myMessages.positions)
            {
                messageHistory[messageHistoryPosition++] = messageId++;  //increment iterator and message count (until we receive from server)
                if (messageHistoryPosition == numberOfMessagesToStore)
                {
                    messageHistoryPosition = 0;
                }
                messageDictionary.Add(messageId, position);//add the MqttMessage to dict, referenced by the key id
            }
        }
        else
        {
            //this will be updated later to only subscribe to player2 messages & will remove the clientID check
        }
    }

    private void MoveTarget()
    {
        int mostRecentIndex = FindOldest();
        if (mostRecentIndex >= 0)
        {
            MqttMessage message = null;
            if (messageDictionary.TryGetValue(messageHistory[mostRecentIndex], out message))
            {
                float x = message.x;
                float y = message.y;
                player2.transform.position = new Vector2(x, y);
                messageDictionary.Remove(messageHistory[mostRecentIndex]);
                messageHistory[mostRecentIndex] = Int32.MaxValue;
                //Debug.Log("Received x: "+ x.ToString() + " y: "+y.ToString());
            }
            else
            {
                //Debug.Log("key " + mostRecentIndex + "not found.");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SendMessage()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(sendFrequency);
            string positions = "[{  \"type\":" + "\"type.Message\"" + ",\"objectid2\":" + "\"objectid2\"" +  ",\"x\":" + player1.transform.position.x + ",\"y\":" + player1.transform.position.y +"}]";
            string payload = "{ \"clientId\": \"" + clientId + "\",\"positions\":" + positions + "}";

            //we may want this payload sent on a separate thread
            //Debug.Log("sending on topic:" + SelfPositionsTopic);
            //Debug.Log("payload:" + payload);
            client.Publish(SelfPositionsTopic,
                System.Text.Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            //Debug.Log("sent");
            MoveTarget();
            //SendPackets();
        }
    }


    public void SendEventPackets(Dictionary<string, GameObject> events)
    {

    }

    public void SendPackets(Dictionary<string, GameObject> objects)
    {
        string positions = "[";
        foreach (KeyValuePair<string, GameObject> entry in objects)
        {
            positions += "{  \"type\":" + "\"position\"" + "\"objectid\":" + entry.Key + ",\"x\":" + entry.Value.transform.position.x + ",\"y\":" + entry.Value.transform.position.y + "}";
        }
        positions += "]";
        string payload = "{ \"clientId\": \"" + clientId + "\",\"positions\":" + positions + "}";
    }    



    private int FindOldest()
    {
        int min = Int32.MaxValue;
        for (int x = 0; x < numberOfMessagesToStore; x++)
        {
            if (messageHistory[x] < min)
            {
                min = x;
            }
        }
        if (min < Int32.MaxValue)
        {
            return min;
        }
        else
            return -1;
    }
}

[Serializable]
public class MWrapper
{
    public string clientId;
    public List<MqttMessage> positions;
}

[Serializable]
public class MqttMessage
{
    public float x;
    public float y;
    public Int64 serverUTCTime;
    public int id;
}
